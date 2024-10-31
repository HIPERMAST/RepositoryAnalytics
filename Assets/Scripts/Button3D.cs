using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button3D : MonoBehaviour
{
    public MemberSpawn memberSpawn; // Reference to the MemberSpawn

    // Update is called once per frame
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
        if (gameObject.name == "NextButton")
        {
            memberSpawn.NextPage();
        }
        else if (gameObject.name == "PreviousButton")
        {
            memberSpawn.PreviousPage();
        }
    }
}
