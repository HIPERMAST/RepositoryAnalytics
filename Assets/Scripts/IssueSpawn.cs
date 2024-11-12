using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

public class IssueSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject issuePrefab;           // Prefab for each issue object
    public Text titleText;                   // UI Text for issue title
    public Text statusText;                  // UI Text for issue status
    public Text assigneesText;               // UI Text for issue assignees
    public Text milestoneText;               // UI Text for issue milestone
    public float slideDuration = 0.5f;       // Duration for sliding animation

    private List<Issue> issues = new List<Issue>();               // List to store loaded issues
    private List<GameObject> spawnedIssues = new List<GameObject>(); // List to track instantiated issues
    private int currentPage = 0;
    private const int itemsPerPage = 5;
    private float itemSpacing = 2.0f;                            // Spacing between items

    public void LoadDataFromJSON()
    {
        string path = Path.Combine(Application.dataPath, "Stats/stats.json");
        if (!File.Exists(path))
        {
            Debug.LogError("stats.json not found.");
            return;
        }

        string json = File.ReadAllText(path);
        IssueData issueData = JsonConvert.DeserializeObject<IssueData>(json);
        issues = issueData.issues;

        DisplayPage(0);
        ClearUI();  // Initially clear UI
    }

    void DisplayPage(int page)
    {
        StartCoroutine(AnimatePageChange(page));
    }

    IEnumerator AnimatePageChange(int page)
    {
        // Animate existing issues sliding out to the left
        yield return StartCoroutine(SlideOutIssues());

        // Clear previous issues after sliding out
        ClearPreviousIssues();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, issues.Count);
        int itemsOnPage = endIndex - startIndex;

        // Calculate the starting position for centering the items
        float startXPosition = transform.position.x - (itemsOnPage - 1) * itemSpacing / 2;

        for (int i = startIndex; i < endIndex; i++)
        {
            var issue = issues[i];
            Vector3 targetPosition = new Vector3(startXPosition + (i - startIndex) * itemSpacing, transform.position.y, transform.position.z);
            GameObject issueObj = Instantiate(issuePrefab, targetPosition + Vector3.right * 15, Quaternion.identity); // Start off-screen to the right
            issueObj.name = issue.title;

            // Attach IssueInfo script and set issue data
            var issueInfo = issueObj.AddComponent<IssueInfo>();
            issueInfo.SetIssueData(issue, this);  // Pass reference to IssueSpawn

            spawnedIssues.Add(issueObj);

            // Animate each issue sliding in
            StartCoroutine(SlideInIssue(issueObj, targetPosition));
        }
    }

    IEnumerator SlideOutIssues()
    {
        foreach (var issueObj in spawnedIssues)
        {
            Vector3 targetPosition = issueObj.transform.position + Vector3.left * 15; // Move 5 units to the left
            float elapsedTime = 0;

            while (elapsedTime < slideDuration)
            {
                issueObj.transform.position = Vector3.Lerp(issueObj.transform.position, targetPosition, elapsedTime / slideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            issueObj.transform.position = targetPosition;
        }
    }

    IEnumerator SlideInIssue(GameObject issueObj, Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = issueObj.transform.position;

        while (elapsedTime < slideDuration)
        {
            issueObj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        issueObj.transform.position = targetPosition;
    }

    void ClearPreviousIssues()
    {
        foreach (var issueObj in spawnedIssues)
        {
            Destroy(issueObj);
        }
        spawnedIssues.Clear();
    }

    public void DisplayIssueInfo(Issue issue)
    {
        titleText.text = "Title: " + issue.title;
        statusText.text = "Status: " + issue.status;

        // Display assignees as a comma-separated list
        assigneesText.text = "Assignees: " + (issue.assignees != null ? string.Join(", ", issue.assignees) : "None");
        
        milestoneText.text = "Milestone: " + (string.IsNullOrEmpty(issue.milestone) ? "None" : issue.milestone);
    }

    public void ClearUI()
    {
        titleText.text = "";
        statusText.text = "";
        assigneesText.text = "";
        milestoneText.text = "";
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < issues.Count)
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
    public class IssueData
    {
        public List<Issue> issues; // Make sure this matches your JSON structure
    }

    [System.Serializable]
    public class Issue
    {
        public string title;
        public string status;
        public List<string> assignees; // List to handle multiple assignees
        public string milestone;
    }
}
