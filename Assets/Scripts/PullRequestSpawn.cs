using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class PullRequestSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject pullRequestPrefab;       // Prefab for pull request object
    public Text titleText;                     // UI Text to show pull request title
    public Text assigneesText;                 // UI Text to show pull request assignees
    public Text reviewersText;                 // UI Text to show pull request reviewers
    public Text statusText;                    // UI Text to show pull request status

    private List<PullRequest> pullRequests = new List<PullRequest>();
    private List<GameObject> spawnedPullRequests = new List<GameObject>();
    private int currentPage = 0;
    private const int itemsPerPage = 5;

    public void LoadDataFromJSON()
    {
        string path = Path.Combine(Application.dataPath, "Stats/stats.json");
        if (!File.Exists(path))
        {
            Debug.LogError("stats.json not found.");
            return;
        }

        string json = File.ReadAllText(path);
        OrganizationData organizationData = JsonConvert.DeserializeObject<OrganizationData>(json);
        pullRequests = organizationData.pull_requests;

        DisplayPage(0);
        ClearUI();  // Initially clear UI
    }

    void DisplayPage(int page)
    {
        ClearPreviousPullRequests();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, pullRequests.Count);
        Vector3 startPosition = transform.position;

        for (int i = startIndex; i < endIndex; i++)
        {
            var pullRequest = pullRequests[i];
            Vector3 position = startPosition + new Vector3(0, 0, i - startIndex); // Arrange along Z-axis
            GameObject pullRequestObj = Instantiate(pullRequestPrefab, position, Quaternion.identity);
            pullRequestObj.name = pullRequest.title;

            // Attach PullRequestInfo script and set pull request data
            var pullRequestInfo = pullRequestObj.AddComponent<PullRequestInfo>();
            pullRequestInfo.SetPullRequestData(pullRequest, this);  // Pass reference to PullRequestSpawn

            spawnedPullRequests.Add(pullRequestObj);
        }
    }

    void ClearPreviousPullRequests()
    {
        foreach (var pullRequestObj in spawnedPullRequests)
        {
            Destroy(pullRequestObj);
        }
        spawnedPullRequests.Clear();
    }

    public void DisplayPullRequestInfo(PullRequest pullRequest)
    {
        titleText.text = pullRequest.title;

        // Display assignees as a comma-separated list
        assigneesText.text = "Assignees: " + (pullRequest.assignees != null ? string.Join(", ", pullRequest.assignees) : "None");
        
        // Display reviewers as a comma-separated list
        reviewersText.text = "Reviewers: " + (pullRequest.reviewers != null ? string.Join(", ", pullRequest.reviewers) : "None");

        statusText.text = "Status: " + pullRequest.status;
    }

    public void ClearUI()
    {
        titleText.text = "";
        assigneesText.text = "";
        reviewersText.text = "";
        statusText.text = "";
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < pullRequests.Count)
        {
            currentPage++;
            DisplayPage(currentPage);
        }
        else
        {
            Debug.Log("No more pages.");
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayPage(currentPage);
        }
        else
        {
            Debug.Log("Already on the first page.");
        }
    }

    [System.Serializable]
    public class OrganizationData
    {
        public List<PullRequest> pull_requests;
    }

    [System.Serializable]
    public class PullRequest
    {
        public string title;
        public List<string> assignees;
        public List<string> reviewers;
        public string status;
    }
}