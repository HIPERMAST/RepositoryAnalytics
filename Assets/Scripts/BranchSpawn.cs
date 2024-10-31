using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class BranchSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject cubePrefab;                // Prefab for branch cube
    public Text nameText;                        // UI Text to show branch name
    public Text authorText;                      // UI Text to show branch author
    public Text currentStatusText;               // UI Text to show branch status
    public Text lastCommitDateText;              // UI Text to show last commit date
    //public Text commitMessageText;               // UI Text to show commit message

    private List<Branch> branches = new List<Branch>();   // List to store loaded branches
    private List<GameObject> spawnedCubes = new List<GameObject>(); // List to track instantiated cubes
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
        BranchData branchData = JsonConvert.DeserializeObject<BranchData>(json);
        branches = branchData.branches;

        DisplayPage(0);  // Display the first page of branches
        ClearUI();       // Initially clear UI
    }

    void DisplayPage(int page)
    {
        ClearPreviousCubes();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, branches.Count);
        Vector3 startPosition = transform.position;

        for (int i = startIndex; i < endIndex; i++)
        {
            var branch = branches[i];
            Vector3 position = startPosition + new Vector3(0, 0, i - startIndex); // Increment along Z-axis
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.name = branch.name;

            // Attach BranchInfo script and set branch data
            var branchInfo = cube.AddComponent<BranchInfo>();
            branchInfo.SetBranchData(branch, this);

            spawnedCubes.Add(cube);
        }
    }

    void ClearPreviousCubes()
    {
        foreach (var cube in spawnedCubes)
        {
            Destroy(cube);
        }
        spawnedCubes.Clear();
    }

    public void DisplayBranchInfo(Branch branch)
    {
        nameText.text = "Name: " + branch.name;
        authorText.text = "Author: " + branch.author;
        currentStatusText.text = "Status: " + branch.current_status;
        lastCommitDateText.text = "Last Commit Date: " + branch.commit_date;
        //commitMessageText.text = "Commit Message: " + branch.commit_message;
    }

    public void ClearUI()
    {
        nameText.text = "";
        authorText.text = "";
        currentStatusText.text = "";
        lastCommitDateText.text = "";
        //commitMessageText.text = "";
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < branches.Count)
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
    public class BranchData
    {
        public List<Branch> branches;
    }

    [System.Serializable]
    public class Branch
    {
        public string name;
        public string author;
        public string current_status;
        public string commit_date;
        public string commit_message;
    }
}
