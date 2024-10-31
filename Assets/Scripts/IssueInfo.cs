using UnityEngine;

public class IssueInfo : MonoBehaviour
{
    private IssueSpawn.Issue issueData;
    private IssueSpawn spawner;

    public void SetIssueData(IssueSpawn.Issue issue, IssueSpawn spawner)
    {
        this.issueData = issue;
        this.spawner = spawner;
    }

    void OnMouseDown()
    {
        if (spawner != null && issueData != null)
        {
            spawner.DisplayIssueInfo(issueData);
        }
    }
}
