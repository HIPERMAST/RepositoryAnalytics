using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Button3D : MonoBehaviour
{
    public MonoBehaviour paginatableObject; // Reference to any MonoBehaviour that implements IPaginatable
    private IPaginatable paginatable;       // Interface reference for pagination

    private void Start()
    {
        // Check if the assigned object implements IPaginatable
        if (paginatableObject is IPaginatable)
        {
            paginatable = (IPaginatable)paginatableObject;
        }
        else
        {
            Debug.LogError("Assigned object does not implement IPaginatable interface.");
        }

        // Get the XRBaseInteractable component and add listener for select events
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

    private void OnDestroy()
    {
        // Clean up the listener on destroy
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEnter);
        }
    }

    // Function called when the VR controller interacts with the 3D button
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log($"Button {gameObject.name} selected!");
        OnButtonClick();
    }

    // Handle button click based on the button's name or intended function
    private void OnButtonClick()
    {
        if (paginatable == null) return;

        if (gameObject.name == "NextButton")
        {
            paginatable.NextPage();
        }
        else if (gameObject.name == "PreviousButton")
        {
            paginatable.PreviousPage();
        }
    }
}
