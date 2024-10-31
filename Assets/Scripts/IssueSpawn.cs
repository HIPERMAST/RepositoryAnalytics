using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class IssueSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject issuePrefab;  // Prefab for each issue object
    public Text titleText;
    public Text statusText;
    public Text assigneesText;
    public Text milestoneText;

    private List<Issue> issues = new List<Issue>();
    private List<GameObject> spawnedIssues = new List<GameObject>();
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
        Vector3 startPosition = transform.position;

        for (int i = startIndex; i < endIndex; i++)
        {
            var issue = issues[i];
            Vector3 position = startPosition + new Vector3(-i - startIndex, 0, 0); // Arrange along Z-axis
            GameObject issueObj = Instantiate(issuePrefab, position, Quaternion.identity);
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
        assigneesText.text = "Assignees: " + string.Join(", ", issue.assignees);
        
        milestoneText.text = "Milestone: " + issue.milestone;
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
        public List<string> assignees; // Changed to a list of strings to handle array in JSON
        public string milestone;
    }
}
