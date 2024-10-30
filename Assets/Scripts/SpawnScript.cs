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

    public void LoadDataFromJSON()
    {
        string path = Path.Combine(Application.dataPath, "Resources/stats.json");
        if (!File.Exists(path))
        {
            Debug.LogError("stats.json not found.");
            return;
        }

        string json = File.ReadAllText(path);
        OrganizationData organizationData = JsonConvert.DeserializeObject<OrganizationData>(json);
        members = organizationData.organization_members;

        // Display the first page when data is loaded
        DisplayPage(0);
    }

    void DisplayPage(int page)
    {
        ClearPreviousCubes();

        // Calculate the start and end indices for the page
        int startIndex = page * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, members.Count);

        Vector3 startPosition = transform.position;

        // Instantiate cubes for the members on the current page
        for (int i = startIndex; i < endIndex; i++)
        {
            Vector3 position = startPosition + new Vector3(i - startIndex, 0, 0);
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.name = members[i].login;
        }

        Debug.Log($"Displaying page {page + 1}/{(members.Count + itemsPerPage - 1) / itemsPerPage}");
    }

    public void NextPage()
    {
        // Check if there are more pages to display
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
        // Check if there is a previous page to display
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

    void ClearPreviousCubes()
    {
        // Destroy all previously spawned cubes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    [System.Serializable]
    public class OrganizationData
    {
        public List<Member> organization_members;
    }

    [System.Serializable]
    public class Member
    {
        public string login;
        public int id;
        public string avatar_url;
    }
}
