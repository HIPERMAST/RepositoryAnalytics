using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class IssueSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject issuePrefab;           // Prefab for each issue object
    public Text titleText;                   // UI Text for issue title
    public Text statusText;                  // UI Text for issue status
    public Text assigneesText;               // UI Text for issue assignees
    public Text milestoneText;               // UI Text for issue milestone

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
        ClearPreviousIssues();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, issues.Count);
        int itemsOnPage = endIndex - startIndex;

        // Calculate the starting position for centering the items
        float startXPosition = transform.position.x - (itemsOnPage - 1) * itemSpacing / 2;

        for (int i = startIndex; i < endIndex; i++)
        {
            var issue = issues[i];
            Vector3 position = new Vector3(startXPosition + (i - startIndex) * itemSpacing, transform.position.y, transform.position.z);
            GameObject issueObj = Instantiate(issuePrefab, position, Quaternion.identity); // No rotation applied
            issueObj.name = issue.title;

            // Attach IssueInfo script and set issue data
            var issueInfo = issueObj.AddComponent<IssueInfo>();
            issueInfo.SetIssueData(issue, this);  // Pass reference to IssueSpawn

            spawnedIssues.Add(issueObj);
        }
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