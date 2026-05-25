using UnityEngine;
using CameraDoorScript;

public class PaperState : MonoBehaviour
{
    void Start()
    {
        // Agar paper pehle hi collect ho chuka hai, toh usay hide kar do
        if (CameraOpenDoor.isPaperCollected)
        {
            gameObject.SetActive(false);
        }
    }
}