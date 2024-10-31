from datetime import datetime, timedelta, timezone
import requests
import json
import os
import sys

# GitHub API URL
GITHUB_API_URL = "https://api.github.com"

# Accept command-line arguments for the organization, repository, and token
ORGANIZATION = sys.argv[1] if len(sys.argv) > 1 else "DefaultOrg"
REPOSITORY = sys.argv[2] if len(sys.argv) > 2 else "DefaultRepo"
GITHUB_TOKEN = sys.argv[3] if len(sys.argv) > 3 else None

# Configure request headers
HEADERS = {"Accept": "application/vnd.github.v3+json"}
if GITHUB_TOKEN and GITHUB_TOKEN != "None":
    HEADERS["Authorization"] = f"token {GITHUB_TOKEN}"

def get_paginated_data(url, params=None, headers=None):
    if params is None:
        params = {}
    if headers is None:
        headers = HEADERS
    params['per_page'] = 100  # Número máximo de elementos por página
    data = []
    while url:
        response = requests.get(url, headers=headers, params=params)
        if response.status_code != 200:
            print(f"Error al obtener datos de {url.split('/')[-1]}: {response.status_code}")
            break
        if 'workflows' in url:
            page_data = response.json().get('workflows', [])
        else:
            page_data = response.json()
        data.extend(page_data)

        # Obtener el enlace a la siguiente página desde el encabezado 'Link'
        link_header = response.headers.get('Link', None)
        if link_header and 'rel="next"' in link_header:
            # Extraer el URL de la siguiente página
            next_url = None
            links = link_header.split(',')
            for link in links:
                if 'rel="next"' in link:
                    next_url = link[link.find('<')+1 : link.find('>')]
                    break
            url = next_url
            params = None  # Los parámetros ya están incluidos en el URL de la siguiente página
        else:
            url = None
    return data

# Function to check if a branch is active or closed
def get_branch_status(commit_date_str):
    THRESHOLD_DAYS = 90  # Define the threshold for "active" status

    # Parse the commit date with timezone awareness
    commit_date = datetime.fromisoformat(commit_date_str.replace("Z", "+00:00"))

    # Use timezone-aware datetime in UTC
    current_date = datetime.now(timezone.utc)

    # Determine if the branch is active or closed based on the commit date
    return "Active" if (current_date - commit_date) <= timedelta(days=THRESHOLD_DAYS) else "Inactive"


def get_org_profile(organization):
    url = f"{GITHUB_API_URL}/orgs/{organization}"
    response = requests.get(url, headers=HEADERS)
    if response.status_code != 200:
        print(f"Error al obtener el perfil de la organización: {response.status_code}")
        return None
    return response.json()

def get_org_members(organization):
    url = f"{GITHUB_API_URL}/orgs/{organization}/members"
    members = get_paginated_data(url)
    return members

def get_org_repos(organization):
    url = f"{GITHUB_API_URL}/orgs/{organization}/repos"
    repos = get_paginated_data(url)
    return repos

# Function to get all branches of the repository
def get_repo_branches(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/branches"
    branches = get_paginated_data(url)
    return get_repo_branches_info(owner, repo, branches)

def get_repo_branches_info(owner, repo, branches):
    branches_info = []  # Collect information for all branches

    for branch in branches:
        url = f"https://api.github.com/repos/{owner}/{repo}/branches/{branch['name']}"
        response = requests.get(url, headers=HEADERS)

        branch_data = response.json()  # Extract JSON response

        # Extract necessary information from the branch data
        branch_info = {
            'name': branch_data['name'],
            'author': branch_data['commit']['commit']['author']['name'],
            'current_status': get_branch_status(branch_data['commit']['commit']['author']['date']),
            'commit': {
                'date': datetime.fromisoformat(
                    branch_data['commit']['commit']['author']['date'].replace("Z", "+00:00")
                ).strftime("%Y-%m-%d %H:%M:%S"),  # Format date as string
                'message': branch_data['commit']['commit']['message'],
            }
        }

        branches_info.append(branch_info)  # Collect the branch info

    return branches_info  # Return all collected branch information


def get_repo_commits(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/commits"
    commits = get_paginated_data(url)
    return commits

def get_repo_issues(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/issues"
    params = {"state": "all"}
    issues = get_paginated_data(url, params)
    return issues

def get_repo_pulls(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/pulls"
    params = {"state": "all"}
    pulls = get_paginated_data(url, params)
    return pulls


def get_repo_workflows(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/actions/workflows"
    workflows = get_paginated_data(url, headers=HEADERS)
    return workflows

def get_repo_discussions(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/discussions"
    headers = HEADERS.copy()
    headers['Accept'] = 'application/vnd.github.v3+json, application/vnd.github.echo-preview+json'
    discussions = get_paginated_data(url, headers=headers)
    return discussions

def get_org_projects(organization):
    url = f"{GITHUB_API_URL}/orgs/{organization}/projects"
    headers = HEADERS.copy()
    headers['Accept'] = 'application/vnd.github.inertia-preview+json'
    projects = get_paginated_data(url, headers=headers)
    return projects

def get_repo_contributor_stats(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/stats/contributors"
    headers = HEADERS.copy()
    contributorStats = get_paginated_data(url, headers=headers)
    return process_contributor_stats(contributorStats)

# Extract lines written and deleted from contributor stats
def process_contributor_stats(contributors):
    contributors_info = []
    for contributor in contributors:
        login = contributor['author']['login']
        total_commits = contributor['total']
        lines_written = sum(week['a'] for week in contributor['weeks'])
        lines_deleted = sum(week['d'] for week in contributor['weeks'])

        contributors_info.append({
            'login': login,
            'total_commits': total_commits,
            'linesWritten': lines_written,
            'linesDeleted': lines_deleted
        })
    return contributors_info

def main():
    data = {}

    # Obtener perfil de la organización
    org_profile = get_org_profile(ORGANIZATION)
    if org_profile:
        data['organization_profile'] = {
            'login': org_profile.get('login'),
            'name': org_profile.get('name'),
            'description': org_profile.get('description'),
            'blog': org_profile.get('blog'),
            'location': org_profile.get('location'),
            'email': org_profile.get('email'),
            'avatar_url': org_profile.get('avatar_url'),
            'html_url': org_profile.get('html_url'),
            'public_repos': org_profile.get('public_repos'),
            'public_members': org_profile.get('public_members_url'),
        }

    # Obtener miembros de la organización
    if GITHUB_TOKEN:
        members = get_org_members(ORGANIZATION)
        data['organization_members'] = [{
            'login': member['login'], 
            'id': member['id'], 
            'avatar_url': member['avatar_url']
        } for member in members]
    else:
        data['organization_members'] = []

    # Obtener repositorios de la organización con más detalles
    repos = get_org_repos(ORGANIZATION)
    data['organization_repos'] = [{
        'name': repo['name'],
        'full_name': repo['full_name'],
        'private': repo['private'],
        'description': repo['description'],
        'html_url': repo['html_url'],
        'language': repo['language'],
        'created_at': repo['created_at'],
        'updated_at': repo['updated_at'],
        'pushed_at': repo['pushed_at'],
        'size': repo['size'],
        'stargazers_count': repo['stargazers_count'],
        'watchers_count': repo['watchers_count'],
        'forks_count': repo['forks_count'],
        'open_issues_count': repo['open_issues_count'],
        'license': repo['license']['name'] if repo['license'] else None,
        'default_branch': repo['default_branch'],
        'topics': repo.get('topics', [])
    } for repo in repos]

    # Obtener ramas del repositorio específico
    branches = get_repo_branches(ORGANIZATION, REPOSITORY)
    data['branches'] = [{
        'name': branch['name'],
        'author': branch['author'],  # Extract author name
        'current_status': branch['current_status'],  # Extract status
        'commit_date': branch['commit']['date'],  # Extract commit date
        'commit_message': branch['commit']['message']  # Extract commit message
        } for branch in branches if branch['name'] != 'HEAD']

    # Obtener commits del repositorio
    commits = get_repo_commits(ORGANIZATION, REPOSITORY)
    data['commits'] = [{
        'author': commit['commit']['author']['name'] if commit['commit']['author'] else None,
        'date': commit['commit']['author']['date'] if commit['commit']['author'] else None,
        'message': commit['commit']['message']
    } for commit in commits]

    # Obtener issues del repositorio, incluyendo etiquetas y asignaciones
    issues = get_repo_issues(ORGANIZATION, REPOSITORY)
    data['issues'] = [{
        'title': issue['title'],
        'assignees': [assignee['login'] for assignee in issue.get('assignees', [])] or [issue['user']['login']],
        'labels': [label['name'] for label in issue.get('labels', [])],
        'milestone': issue['milestone']['title'] if issue.get('milestone') else None,
        'status': issue['state']
    } for issue in issues if 'pull_request' not in issue]


    # Obtener pull requests del repositorio si hay token
    if GITHUB_TOKEN:
        pulls = get_repo_pulls(ORGANIZATION, REPOSITORY)
        data['pull_requests'] = [{
            'title': pr['title'],
            'assignees': [assignee['login'] for assignee in pr.get('assignees', [])] or [pr['user']['login']],
            'reviewers': [reviewer['login'] for reviewer in pr.get('requested_reviewers', [])],
            'status': pr['state']
        } for pr in pulls]
    else:
        data['pull_requests'] = []


    # Obtener workflows (GitHub Actions) si hay token
    if GITHUB_TOKEN:
        workflows = get_repo_workflows(ORGANIZATION, REPOSITORY)
        data['workflows'] = [{
            'id': workflow['id'],
            'name': workflow['name'],
            'state': workflow['state'],
            'created_at': workflow['created_at'],
            'updated_at': workflow['updated_at'],
            'html_url': workflow['html_url']
        } for workflow in workflows]
    else:
        data['workflows'] = []

    # Obtener discusiones del repositorio si hay token
    if GITHUB_TOKEN:
        discussions = get_repo_discussions(ORGANIZATION, REPOSITORY)
        data['discussions'] = [{
            'number': discussion['number'],
            'title': discussion['title'],
            'state': discussion['state'],
            'created_at': discussion['created_at'],
            'user': discussion['user']['login'],
            'comments': discussion['comments']
        } for discussion in discussions]
    else:
        data['discussions'] = []

    # Obtener proyectos de la organización si hay token
    if GITHUB_TOKEN:
        projects = get_org_projects(ORGANIZATION)
        data['projects'] = [{
            'id': project['id'],
            'name': project['name'],
            'body': project.get('body'),
            'state': project['state'],
            'created_at': project['created_at'],
            'updated_at': project['updated_at']
        } for project in projects]
    else:
        data['projects'] = []

    # Obtener estadísticas de contribución del repositorio si hay token
    if GITHUB_TOKEN:
        contributor_stats = get_repo_contributor_stats(ORGANIZATION, REPOSITORY)
        data['repository_members'] = [{
            'login': stat['login'],
            'total_commits': stat['total_commits'],
            'lines_written': stat['linesWritten'],
            'lines_deleted': stat['linesDeleted']
        } for stat in contributor_stats]
    else:
        data['repository_members'] = []

    # Save the data in the StreamingAssets folder
    save_path = os.path.join('Assets', 'Stats', 'stats.json')
    os.makedirs(os.path.dirname(save_path), exist_ok=True)  # Ensure folder exists

    with open(save_path, 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=4)

    print("stats.json has been successfully created.")

if __name__ == "__main__":
    main()
