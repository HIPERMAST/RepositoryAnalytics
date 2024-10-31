using UnityEngine;

public class PullRequestInfo : MonoBehaviour
{
    private PullRequestSpawn.PullRequest pullRequestData;
    private PullRequestSpawn pullRequestSpawn;

    public void SetPullRequestData(PullRequestSpawn.PullRequest pullRequest, PullRequestSpawn spawner)
    {
        pullRequestData = pullRequest;
        pullRequestSpawn = spawner;
    }

    void OnMouseDown()
    {
        if (pullRequestSpawn != null && pullRequestData != null)
        {
            pullRequestSpawn.DisplayPullRequestInfo(pullRequestData);
        }
    }
}
