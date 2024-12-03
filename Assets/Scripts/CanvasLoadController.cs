using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;

public class CanvasLoadController : MonoBehaviour
{
    public TMP_InputField orgInput;  // Organization Input Field
    public TMP_InputField repoInput; // Repository Input Field
    public Button loadButton;        // Load Button
    public Button cancelButton;      // Cancel Button
    public CanvasController canvasController; // Reference to CanvasController
    public GameObject canvasLoad;  // The Main Menu UI (Canvas or Panel)
      public MemberSpawn memberSpawn;  // Reference to the MemberSpawn
    public BranchSpawn branchSpawn;  // Reference to the BranchSpawn
    public IssueSpawn issueSpawn;  // Reference to the IssueSpawn
    public PullRequestSpawn pullRequestSpawn;  // Reference to the PullRequestSpawn

    private string statsFilePath = "Assets/Stats/stats.json";

    void Start()
    {
        // Pause the game while the main menu is active
        Time.timeScale = 0;

        // Add button listeners
        loadButton.onClick.AddListener(OnLoadDataClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // Load stats.json data
        LoadStatsData();
    }
 
    void LoadStatsData()
    {
        if (File.Exists(statsFilePath))
        {
            try
            {
                // Read and parse the JSON file
                string jsonData = File.ReadAllText(statsFilePath);
                JObject stats = JObject.Parse(jsonData);

                // Extract organization_profile and organization_repos
                var organizationProfile = stats["organization_profile"];
                var organizationRepos = stats["organization_repos"] as JArray;

                Debug.Log($"Organization Profile: {organizationProfile}");
                Debug.Log($"Organization Repositories: {organizationRepos}");

                // Validate the data and find the selected repository
                if (organizationProfile != null && organizationRepos != null && organizationRepos.Count > 0)
                {
                    bool foundSelectedRepo = false;

                    foreach (var repo in organizationRepos)
                    {
                        if (repo["selected"] != null && (bool)repo["selected"] == true)
                        {
                            // Populate the selected repository
                            orgInput.text = organizationProfile["login"]?.ToString() ?? "Unknown Organization";
                            repoInput.text = repo["name"]?.ToString() ?? "Unknown Repository";

                            // Disable input fields to prevent editing
                            orgInput.interactable = false;
                            repoInput.interactable = false;

                            Debug.Log($"Selected Organization: {orgInput.text}, Repository: {repoInput.text}");
                            foundSelectedRepo = true;
                            break;
                        }
                    }

                    // If no selected repository was found, show CanvasCreate
                    if (!foundSelectedRepo)
                    {
                        Debug.LogWarning("No repository with selected: true found in the JSON.");
                        canvasController.ShowCanvasCreate();
                    }
                }
                else
                {
                    Debug.LogWarning("No organization profile or repositories found in the JSON.");
                    canvasController.ShowCanvasCreate(); // Show CanvasCreate if no valid data
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing stats.json: {e.Message}");
                canvasController.ShowCanvasCreate(); // Show CanvasCreate in case of error
            }
        }
        else
        {
            Debug.LogWarning("stats.json not found.");
            canvasController.ShowCanvasCreate(); // Show CanvasCreate if the file is missing
        }
    }

    void OnLoadDataClicked()
    {
        // Resume the game when loading starts
        Time.timeScale = 1;

        // Hide the main menu UI
        canvasLoad.SetActive(false);

        Debug.Log($"Loading data for Org: {orgInput.text}, Repo: {repoInput.text}");
        memberSpawn.LoadDataFromJSON();
        branchSpawn.LoadDataFromJSON();
        issueSpawn.LoadDataFromJSON();
        pullRequestSpawn.LoadDataFromJSON();
    }

    void OnCancelClicked()
    {
        canvasController.ShowCanvasCreate();
    }
}
