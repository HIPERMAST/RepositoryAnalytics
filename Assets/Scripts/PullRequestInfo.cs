using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullRequestInfo : MonoBehaviour
{
    private PullRequestSpawn.PullRequest pullRequestData;
    private PullRequestSpawn pullRequestSpawn;

    public void SetPullRequestData(PullRequestSpawn.PullRequest pullRequest, PullRequestSpawn spawner)
    {
        pullRequestData = pullRequest;
        pullRequestSpawn = spawner;
    }

    private void OnEnable()
    {
        // Get the XRBaseInteractable component and add a listener for selection events
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
        // Remove the listener when the object is disabled
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEnter);
        }
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Display the pull request info when the VR controller interacts with this object
        if (pullRequestSpawn != null && pullRequestData != null)
        {
            pullRequestSpawn.DisplayPullRequestInfo(pullRequestData);
        }
    }
}
