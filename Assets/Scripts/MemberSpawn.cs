using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

public class MemberSpawn : MonoBehaviour, IPaginatable // Implement IPaginatable interface
{
    public GameObject cubePrefab;               // Prefab for member cube
    public Text loginText;                      // UI Text to show member login
    public Text totalCommitsText;               // UI Text to show total commits
    public Text linesWrittenText;               // UI Text to show lines written
    public Text linesDeletedText;               // UI Text to show lines deleted
    public float slideDuration = 0.5f;          // Duration for sliding animation

    private List<Member> members = new List<Member>();
    private List<GameObject> spawnedCubes = new List<GameObject>();
    private int currentPage = 0;
    private const int itemsPerPage = 5;
    private float itemSpacing = 2.0f;           // Spacing between items

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
        members = organizationData.repository_members;

        DisplayPage(0);
        ClearUI();  // Initially clear UI
    }

    void DisplayPage(int page)
    {
        StartCoroutine(AnimatePageChange(page));
    }

    IEnumerator AnimatePageChange(int page)
    {
        // Animate existing cubes sliding out to the left
        yield return StartCoroutine(SlideOutCubes());

        // Clear previous cubes after sliding out
        ClearPreviousCubes();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, members.Count);
        int itemsOnPage = endIndex - startIndex;

        // Calculate the starting position for centering the items
        float startXPosition = transform.position.x - (itemsOnPage - 1) * itemSpacing / 2;

        for (int i = startIndex; i < endIndex; i++)
        {
            var member = members[i];
            Vector3 targetPosition = new Vector3(startXPosition + (i - startIndex) * itemSpacing, transform.position.y, transform.position.z);
            GameObject cube = Instantiate(cubePrefab, targetPosition + Vector3.left * 15, Quaternion.identity); // Start off-screen to the right
            cube.name = member.login;

            // Attach MemberInfo script and set member data
            var memberInfo = cube.AddComponent<MemberInfo>();
            memberInfo.SetMemberData(member, this);  // Pass reference to MemberSpawn

            spawnedCubes.Add(cube);

            // Animate each cube sliding in
            StartCoroutine(SlideInCube(cube, targetPosition));
        }
    }

    IEnumerator SlideOutCubes()
    {
        foreach (var cube in spawnedCubes)
        {
            Vector3 targetPosition = cube.transform.position + Vector3.right * 15; // Move 5 units to the left
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

    public void DisplayMemberInfo(Member member)
    {
        loginText.text = "Login: " + member.login;
        totalCommitsText.text = "Total Commits:\n" + member.total_commits;
        linesWrittenText.text = "Lines Additions:\n" + member.lines_written;
        linesDeletedText.text = "Lines Deletions:\n" + member.lines_deleted;
    }

    public void ClearUI()
    {
        loginText.text = "";
        totalCommitsText.text = "";
        linesWrittenText.text = "";
        linesDeletedText.text = "";
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < members.Count)
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
        public List<Member> repository_members;
    }

    [System.Serializable]
    public class Member
    {
        public string login;
        public string total_commits;
        public string lines_written;
        public string lines_deleted;
    }
}