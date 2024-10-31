using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SpawnerScript : MonoBehaviour
{
    public GameObject cubePrefab;
    private List<Member> members = new List<Member>();

    private int currentPage = 0;  // Track the current page
    private const int itemsPerPage = 5;  // Number of members per page

    // Declare spawnedCubes at the class level
    private List<GameObject> spawnedCubes = new List<GameObject>();

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

        DisplayPage(0);  // Display the first page
    }

    void DisplayPage(int page)
    {
        ClearPreviousCubes();  // Clear old cubes

        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, members.Count);

        Vector3 startPosition = transform.position;

        Debug.Log($"Displaying page {page + 1}/{(members.Count + itemsPerPage - 1) / itemsPerPage}");

        for (int i = startIndex; i < endIndex; i++)
        {
            var member = members[i];

            Vector3 position = startPosition + new Vector3(i - startIndex, 0, 0);
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.name = member.login;

            // Attach the MemberInfo script and set the member data
            var memberInfo = cube.AddComponent<MemberInfo>();
            memberInfo.SetMemberData(member);

            spawnedCubes.Add(cube);  // Track the spawned cube
        }
    }

    void ClearPreviousCubes()
    {
        foreach (var cube in spawnedCubes)
        {
            Destroy(cube);  // Destroy each spawned cube
        }
        spawnedCubes.Clear();  // Clear the list of spawned cubes
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
        public int total_commits;
        public int lines_written;
        public int lines_deleted;
    }
}
