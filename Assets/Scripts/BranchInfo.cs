using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BranchInfo : MonoBehaviour
{
    private BranchSpawn.Branch branchData;
    private BranchSpawn spawner;

    public void SetBranchData(BranchSpawn.Branch branch, BranchSpawn spawner)
    {
        this.branchData = branch;
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
        // Display the branch info when the VR controller interacts with this object
        if (spawner != null && branchData != null)
        {
            spawner.DisplayBranchInfo(branchData);
        }
    }
}
