using UnityEngine;

public class Button3D : MonoBehaviour
{
    public MonoBehaviour paginatableObject; // Reference to any MonoBehaviour that implements IPaginatable
    private IPaginatable paginatable;       // Interface reference for pagination

    void Start()
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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object clicked is this button
                if (hit.transform.gameObject == gameObject)
                {
                    Debug.Log($"Button {gameObject.name} clicked!");
                    OnButtonClick();
                }
            }
        }
    }

    // Function called when the 3D button is clicked
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
