using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

public class BranchSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject cubePrefab;                // Prefab for branch cube
    public Text nameText;                        // UI Text to show branch name
    public Text authorText;                      // UI Text to show branch author
    public Text currentStatusText;               // UI Text to show branch status
    public Text lastCommitDateText;              // UI Text to show last commit date
    public float slideDuration = 0.5f;           // Duration for sliding animation

    private List<Branch> branches = new List<Branch>();   // List to store loaded branches
    private List<GameObject> spawnedCubes = new List<GameObject>(); // List to track instantiated cubes
    private int currentPage = 0;
    private const int itemsPerPage = 5;
    private float itemSpacing = 2.0f;            // Spacing between items

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
        StartCoroutine(AnimatePageChange(page));
    }

    IEnumerator AnimatePageChange(int page)
    {
        // Animate old cubes sliding out to the left
        yield return StartCoroutine(SlideOutCubes());

        // Clear previous cubes after sliding out
        ClearPreviousCubes();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, branches.Count);
        int itemsOnPage = endIndex - startIndex;

        // Calculate the starting position for centering the items along the Z-axis
        float startZPosition = transform.position.z - (itemsOnPage - 1) * itemSpacing / 2;

        for (int i = startIndex; i < endIndex; i++)
        {
            var branch = branches[i];
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startZPosition + (i - startIndex) * itemSpacing);
            GameObject cube = Instantiate(cubePrefab, targetPosition + Vector3.back * 15, Quaternion.Euler(0, -90, 0)); // Start to the left
            cube.name = branch.name;

            // Attach BranchInfo script and set branch data
            var branchInfo = cube.AddComponent<BranchInfo>();
            branchInfo.SetBranchData(branch, this);

            spawnedCubes.Add(cube);

            // Animate each cube sliding in
            StartCoroutine(SlideInCube(cube, targetPosition));
        }
    }

    IEnumerator SlideOutCubes()
    {
        foreach (var cube in spawnedCubes)
        {
            Vector3 targetPosition = cube.transform.position + Vector3.forward * 15; // Move 5 units to the left
            float elapsedTime = 0;

            while (elapsedTime < slideDuration)
            {
                cube.transform.position = Vector3.Lerp(cube.transform.position, targetPosition, elapsedTime / slideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cube.transform.position = targetPosition;
        }
    }

    IEnumerator SlideInCube(GameObject cube, Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = cube.transform.position;

        while (elapsedTime < slideDuration)
        {
            cube.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = targetPosition;
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
    }

    public void ClearUI()
    {
        nameText.text = "";
        authorText.text = "";
        currentStatusText.text = "";
        lastCommitDateText.text = "";
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
