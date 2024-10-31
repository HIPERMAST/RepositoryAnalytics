using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField orgInput;  // Organization Name Input Field
    public TMP_InputField repoInput;  // Repository Name Input Field
    public TMP_InputField tokenInput;  // GitHub Token Input Field
    public Button loadButton;  // Load Data Button
    public Button exitButton;  // Exit Button
    public GameObject mainMenuUI;  // The Main Menu UI (Canvas or Panel)
    public MemberSpawn memberSpawn;  // Reference to the MemberSpawn
    public BranchSpawn branchSpawn;  // Reference to the BranchSpawn
    public IssueSpawn issueSpawn;  // Reference to the IssueSpawn
    public PullRequestSpawn pullRequestSpawn;  // Reference to the PullRequestSpawn

    void Start()
    {
        // Pause the game while the main menu is active
        Time.timeScale = 0;

        // Add listeners to the buttons
        loadButton.onClick.AddListener(OnLoadDataClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    void OnLoadDataClicked()
    {
        // Resume the game when loading starts
        Time.timeScale = 1;

        // Hide the main menu UI
        mainMenuUI.SetActive(false);

        string orgName = orgInput.text;
        string repoName = repoInput.text;
        string githubToken = tokenInput.text;

        if (string.IsNullOrEmpty(orgName) || string.IsNullOrEmpty(repoName))
        {
            Debug.LogError("Organization and repository names cannot be empty.");
            return;
        }

        if (githubToken == "None")
        {
            Debug.LogWarning("No GitHub token provided. Stats may be limited.");
        }

        // Run the Python script and load data
        PythonRunner.RunPythonScript(orgName, repoName, githubToken);
        memberSpawn.LoadDataFromJSON();
        branchSpawn.LoadDataFromJSON();
        issueSpawn.LoadDataFromJSON();
        pullRequestSpawn.LoadDataFromJSON();
    }

    void OnExitClicked()
    {
        Debug.Log("Exiting the game...");
        Application.Quit();  // Exits the game when built

        // Note: This won't quit the game in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
