using UnityEngine;
using System.IO;

public class CanvasController : MonoBehaviour
{
    public GameObject canvasLoad;    // Reference to the GameObject of CanvasLoad
    public GameObject canvasCreate;  // Reference to the GameObject of CanvasCreate

    void Start()
    {
        // Initially hide both canvases
        canvasLoad.SetActive(false);
        canvasCreate.SetActive(false);

        // Check for stats.json and show the appropriate canvas
        if (File.Exists("Assets/Stats/stats.json"))
        {
            canvasLoad.SetActive(true);
        }
        else
        {
            canvasCreate.SetActive(true);
        }
    }

    public void ShowCanvasLoad()
    {
        canvasLoad.SetActive(true);
        canvasCreate.SetActive(false);
    }

    public void ShowCanvasCreate()
    {
        canvasCreate.SetActive(true);
        canvasLoad.SetActive(false);
    }
}
