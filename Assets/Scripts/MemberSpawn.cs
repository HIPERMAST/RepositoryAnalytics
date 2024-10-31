using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class MemberSpawn : MonoBehaviour
{
    public GameObject cubePrefab;
    public Text loginText;
    public Text totalCommitsText;
    public Text linesWrittenText;
    public Text linesDeletedText;

    private List<Member> members = new List<Member>();
    private List<GameObject> spawnedCubes = new List<GameObject>();
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
        members = organizationData.repository_members;

        DisplayPage(0);
        ClearUI();  // Initially clear UI
    }

    void DisplayPage(int page)
    {
        ClearPreviousCubes();

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, members.Count);
        Vector3 startPosition = transform.position;

        for (int i = startIndex; i < endIndex; i++)
        {
            var member = members[i];
            Vector3 position = startPosition + new Vector3(i - startIndex, 0, 0);
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.name = member.login;

            // Attach MemberInfo script and set member data
            var memberInfo = cube.AddComponent<MemberInfo>();
            memberInfo.SetMemberData(member, this);  // Pass reference to SpawnerScript

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

    public void DisplayMemberInfo(Member member)
    {
        loginText.text = member.login;
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
