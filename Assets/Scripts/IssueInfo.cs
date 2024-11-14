using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IssueInfo : MonoBehaviour
{
    private IssueSpawn.Issue issueData;
    private IssueSpawn spawner;

    public void SetIssueData(IssueSpawn.Issue issue, IssueSpawn spawner)
    {
        this.issueData = issue;
        this.spawner = spawner;
    }

    private void OnEnable()
    {
        // Assume that XRBaseInteractable is present as part of the prefab setup
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEnter);
        }
        else
        {
            Debug.LogWarning($"XRBaseInteractable component is missing on {gameObject.name}. Please ensure the prefab includes it.");
        }
    }

    private void OnDisable()
    {
        // Assume that XRBaseInteractable is present as part of the prefab setup
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEnter);
        }
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Display the issue info when the VR controller interacts with this object
        if (spawner != null && issueData != null)
        {
            spawner.DisplayIssueInfo(issueData);
        }
    }
}
