using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MemberInfo : MonoBehaviour
{
    private MemberSpawn.Member memberData;
    private MemberSpawn memberSpawn;

    public void SetMemberData(MemberSpawn.Member member, MemberSpawn spawner)
    {
        memberData = member;
        memberSpawn = spawner;
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
        // Display the member info when the VR controller interacts with this object
        if (memberSpawn != null && memberData != null)
        {
            memberSpawn.DisplayMemberInfo(memberData);
        }
    }
}
