using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

public class PullRequestSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject pullRequestPrefab;       // Prefab for pull request object
    public Text titleText;                     // UI Text to show pull request title
    public Text assigneesText;                 // UI Text to show pull request assignees
    public Text reviewersText;                 // UI Text to show pull request reviewers
    public Text statusText;                    // UI Text to show pull request status
    public float slideDuration = 0.5f;         // Duration for sliding animation

    private List<PullRequest> pullRequests = new List<PullRequest>();
    private List<GameObject> spawnedPullRequests = new List<GameObject>();
    private int currentPage = 0;
    private const int itemsPerPage = 5;
    private float itemSpacing = 2.0f;          // Spacing between items

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
        StartCoroutine(AnimatePageChange(page));
    }

    IEnumerator AnimatePageChange(int page)
    {
        // Animate existing pull request objects sliding out to the left
        yield return StartCoroutine(SlideOutPullRequests());

        // Clear previous pull requests after sliding out
        ClearPreviousPullRequests();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, pullRequests.Count);
        int itemsOnPage = endIndex - startIndex;

        // Calculate the starting position for centering along the Z-axis
        float startZPosition = transform.position.z - (itemsOnPage - 1) * itemSpacing / 2;

        for (int i = startIndex; i < endIndex; i++)
        {
            var pullRequest = pullRequests[i];
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startZPosition + (i - startIndex) * itemSpacing);
            GameObject pullRequestObj = Instantiate(pullRequestPrefab, targetPosition + Vector3.forward * 15, Quaternion.Euler(0, -90, 0)); // Start off-screen to the right
            pullRequestObj.name = pullRequest.title;

            // Attach PullRequestInfo script and set pull request data
            var pullRequestInfo = pullRequestObj.AddComponent<PullRequestInfo>();
            pullRequestInfo.SetPullRequestData(pullRequest, this);  // Pass reference to PullRequestSpawn

            spawnedPullRequests.Add(pullRequestObj);

            // Animate each pull request sliding in
            StartCoroutine(SlideInPullRequest(pullRequestObj, targetPosition));
        }
    }

    IEnumerator SlideOutPullRequests()
    {
        foreach (var pullRequestObj in spawnedPullRequests)
        {
            Vector3 targetPosition = pullRequestObj.transform.position + Vector3.back * 15; // Move 5 units to the left
            float elapsedTime = 0;

            while (elapsedTime < slideDuration)
            {
                pullRequestObj.transform.position = Vector3.Lerp(pullRequestObj.transform.position, targetPosition, elapsedTime / slideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            pullRequestObj.transform.position = targetPosition;
        }
    }

    IEnumerator SlideInPullRequest(GameObject pullRequestObj, Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = pullRequestObj.transform.position;

        while (elapsedTime < slideDuration)
        {
            pullRequestObj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pullRequestObj.transform.position = targetPosition;
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
