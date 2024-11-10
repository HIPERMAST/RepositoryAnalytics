import json
import matplotlib.pyplot as plt
from datetime import datetime

# Load data from stats.json
with open('Assets/Stats/stats.json', 'r') as f:
    data = json.load(f)

# Helper function to save and show each plot
def save_plot(plt, title):
    plt.title(title)
    plt.tight_layout()
    plt.savefig(f'Assets/Stats/{title.replace(" ", "_").lower()}.png')
    plt.close()

# 1. Members Insights
def plot_member_activity():
    members = data.get('repository_members', [])
    if not members:
        print("No member data found for plotting activity.")
        return
    logins = [member['login'] for member in members]
    total_commits = [member['total_commits'] for member in members]
    
    plt.figure(figsize=(10, 6))
    plt.bar(logins, total_commits)
    plt.xlabel('Members')
    plt.ylabel('Total Commits')
    save_plot(plt, 'Member Activity Levels')

def plot_top_contributors():
    members = sorted(data.get('repository_members', []), key=lambda x: x['total_commits'], reverse=True)[:5]
    if not members:
        print("No member data found for plotting top contributors.")
        return
    logins = [member['login'] for member in members]
    lines_written = [member['lines_written'] for member in members]
    lines_deleted = [member['lines_deleted'] for member in members]
    
    plt.figure(figsize=(10, 6))
    plt.bar(logins, lines_written, label='Lines Written')
    plt.bar(logins, lines_deleted, label='Lines Deleted', bottom=lines_written)
    plt.xlabel('Top Contributors')
    plt.ylabel('Lines of Code')
    plt.legend()
    save_plot(plt, 'Top Contributors')

# 2. Issues Insights
def plot_issue_resolution_time():
    issues = data.get('issues', [])
    resolution_times = [(datetime.fromisoformat(issue['closed_at']) - datetime.fromisoformat(issue['created_at'])).days
                        for issue in issues if issue.get('closed_at') and issue.get('created_at')]
    if not resolution_times:
        print("No issue resolution times found for plotting.")
        return
    
    plt.figure(figsize=(10, 6))
    plt.hist(resolution_times, bins=20, color='skyblue')
    plt.xlabel('Resolution Time (days)')
    plt.ylabel('Number of Issues')
    save_plot(plt, 'Issue Resolution Time Distribution')

def plot_most_active_labels():
    from collections import Counter
    labels = [label for issue in data.get('issues', []) for label in issue.get('labels', [])]
    label_counts = Counter(labels)

    if not label_counts:
        print("No labels found in issues to plot.")
        return
    
    labels, counts = zip(*label_counts.most_common(10))
    
    plt.figure(figsize=(10, 6))
    plt.bar(labels, counts, color='purple')
    plt.xlabel('Labels')
    plt.ylabel('Number of Issues')
    save_plot(plt, 'Most Active Issue Labels')

# 3. Pull Requests Insights
def plot_merge_times():
    pull_requests = data.get('pull_requests', [])
    merge_times = [(datetime.fromisoformat(pr['merged_at']) - datetime.fromisoformat(pr['created_at'])).days
                   for pr in pull_requests if pr.get('merged_at') and pr.get('created_at')]
    if not merge_times:
        print("No pull request merge times found for plotting.")
        return
    
    plt.figure(figsize=(10, 6))
    plt.hist(merge_times, bins=20, color='green')
    plt.xlabel('Merge Time (days)')
    plt.ylabel('Number of PRs')
    save_plot(plt, 'Pull Request Merge Times')

def plot_pr_approval_rate():
    pull_requests = data.get('pull_requests', [])
    approved_count = sum(1 for pr in pull_requests if pr['status'] == 'closed' and pr.get('merged_at'))
    closed_without_merge_count = sum(1 for pr in pull_requests if pr['status'] == 'closed' and not pr.get('merged_at'))
    
    if approved_count + closed_without_merge_count == 0:
        print("No pull request approval data found for plotting.")
        return

    plt.figure(figsize=(6, 6))
    plt.pie([approved_count, closed_without_merge_count], labels=['Approved', 'Closed Without Merge'],
            autopct='%1.1f%%', startangle=140, colors=['#66b3ff', '#ff9999'])
    save_plot(plt, 'Pull Request Approval Rate')

# 4. Branches Insights
def plot_branch_activity():
    branches = data.get('branches', [])
    active = sum(1 for branch in branches if branch['current_status'] == 'Active')
    inactive = len(branches) - active
    
    if not branches:
        print("No branch data found for plotting activity.")
        return

    plt.figure(figsize=(6, 6))
    plt.pie([active, inactive], labels=['Active', 'Inactive'], autopct='%1.1f%%', startangle=140, colors=['#99ff99', '#ff6666'])
    save_plot(plt, 'Branch Activity')

# Merges Insights
def plot_merge_frequency():
    branches = data.get('branches', [])
    merge_counts = [branch.get('merge_count', 0) for branch in branches]
    
    if not merge_counts:
        print("No merge count data found in branches for plotting frequency.")
        return

    plt.figure(figsize=(10, 6))
    plt.hist(merge_counts, bins=20, color='orange')
    plt.xlabel('Number of Merges')
    plt.ylabel('Frequency')
    save_plot(plt, 'Branch Merge Frequency')

# Run all insight functions
plot_member_activity()
plot_top_contributors()
plot_issue_resolution_time()
plot_most_active_labels()
plot_merge_times()
plot_pr_approval_rate()
plot_branch_activity()
plot_merge_frequency()

print("All plots saved to 'Assets/Stats/' directory.")