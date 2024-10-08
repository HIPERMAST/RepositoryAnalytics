import requests
import json
import time

# Configuración
GITHUB_API_URL = "https://api.github.com"
ORGANIZATION = "ISIS3510Team35"
REPOSITORY = "KotlinTeam"

# Incluye tu token aquí
GITHUB_TOKEN = "TOKEN"  # Reemplaza 'TU_TOKEN_AQUÍ' con tu token real

HEADERS = {
    "Accept": "application/vnd.github.v3+json",
    "Authorization": f"token {GITHUB_TOKEN}"
}

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
            print(f"Error al obtener datos: {response.status_code}")
            break
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

def get_repo_branches(owner, repo):
    url = f"{GITHUB_API_URL}/repos/{owner}/{repo}/branches"
    branches = get_paginated_data(url)
    return branches

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
    workflows = get_paginated_data(url)
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
    response = requests.get(url, headers=headers)
    if response.status_code == 202:
        # Stats are being generated, need to wait and retry
        print("Generando estadísticas de contribuciones, esperando 3 segundos...")
        time.sleep(3)
        return get_repo_contributor_stats(owner, repo)
    elif response.status_code != 200:
        print(f"Error al obtener estadísticas de contribuciones: {response.status_code}")
        return None
    return response.json()

def main():
    data = {}

    # Obtener perfil de la organización
    org_profile = get_org_profile(ORGANIZATION)
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
    members = get_org_members(ORGANIZATION)
    data['organization_members'] = [{'login': member['login'], 'id': member['id'], 'avatar_url': member['avatar_url']} for member in members]

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
    data['branches'] = [branch['name'] for branch in branches]

    # Obtener commits del repositorio
    commits = get_repo_commits(ORGANIZATION, REPOSITORY)
    data['commits'] = [{
        'sha': commit['sha'],
        'author': commit['commit']['author']['name'] if commit['commit']['author'] else None,
        'date': commit['commit']['author']['date'] if commit['commit']['author'] else None,
        'message': commit['commit']['message']
    } for commit in commits]

    # Obtener issues del repositorio, incluyendo etiquetas y asignaciones
    issues = get_repo_issues(ORGANIZATION, REPOSITORY)
    data['issues'] = [{
        'number': issue['number'],
        'title': issue['title'],
        'state': issue['state'],
        'created_at': issue['created_at'],
        'closed_at': issue.get('closed_at'),
        'user': issue['user']['login'],
        'labels': [label['name'] for label in issue.get('labels', [])],
        'assignees': [assignee['login'] for assignee in issue.get('assignees', [])]
    } for issue in issues if 'pull_request' not in issue]

    # Obtener pull requests del repositorio
    pulls = get_repo_pulls(ORGANIZATION, REPOSITORY)
    data['pull_requests'] = [{
        'number': pr['number'],
        'title': pr['title'],
        'state': pr['state'],
        'created_at': pr['created_at'],
        'closed_at': pr.get('closed_at'),
        'merged_at': pr.get('merged_at'),
        'user': pr['user']['login'],
        'labels': [label['name'] for label in pr.get('labels', [])],
        'assignees': [assignee['login'] for assignee in pr.get('assignees', [])]
    } for pr in pulls]

    # # Obtener workflows (GitHub Actions)
    # workflows = get_repo_workflows(ORGANIZATION, REPOSITORY)
    # data['workflows'] = [{
    #     'id': workflow['id'],
    #     'name': workflow['name'],
    #     'state': workflow['state'],
    #     'created_at': workflow['created_at'],
    #     'updated_at': workflow['updated_at'],
    #     'html_url': workflow['html_url']
    # } for workflow in workflows]

    # Obtener discusiones del repositorio (NO FUNCIONA)
    discussions = get_repo_discussions(ORGANIZATION, REPOSITORY)
    data['discussions'] = [{
        'number': discussion['number'],
        'title': discussion['title'],
        'state': discussion['state'],
        'created_at': discussion['created_at'],
        'user': discussion['user']['login'],
        'comments': discussion['comments']
    } for discussion in discussions]

    # Obtener proyectos de la organización
    projects = get_org_projects(ORGANIZATION)
    data['projects'] = [{
        'id': project['id'],
        'name': project['name'],
        'body': project.get('body'),
        'state': project['state'],
        'created_at': project['created_at'],
        'updated_at': project['updated_at']
    } for project in projects]

    # Obtener estadísticas de contribución del repositorio
    contributor_stats = get_repo_contributor_stats(ORGANIZATION, REPOSITORY)
    data['contributor_stats'] = [{
        'author': stat['author']['login'],
        'total_commits': stat['total'],
        'weeks': stat['weeks']
    } for stat in contributor_stats] if contributor_stats else []

    # Guardar datos en stats.json
    with open('stats.json', 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=4)

    print("El archivo stats.json ha sido creado con éxito.")

if __name__ == "__main__":
    main()
