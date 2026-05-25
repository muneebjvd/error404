using UnityEngine;

public class LevelObjectController : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject; // Woh model jise hide/unhide karna hai

    void Update()
    {
        // CoreGameLogic check
        if (CoreGameLogic.Instance != null)
        {
            // Level 4 ya us se bara ho tou unhide (true), warna hide (false)
            if (CoreGameLogic.Instance.currentLevel >= 4)
            {
                if (!targetObject.activeSelf) targetObject.SetActive(true);
            }
            else
            {
                if (targetObject.activeSelf) targetObject.SetActive(false);
            }
        }
    }
}