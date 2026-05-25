using UnityEngine;

public class RotateOnTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 10f; // Kitni tezi se rotate ho
    private bool shouldRotate = false;
    private Quaternion targetRotation;

    void Start()
    {
        // Shuruat mein current rotation save kar lo
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Agar trigger ho chuka hai, toh smooth rotate karo
        if (shouldRotate)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Yeh function Trigger Box call karega
    public void ActivateRotation()
    {
        if (!shouldRotate)
        {
            shouldRotate = true;
            // Current rotation mein 180 degree add kar do
            targetRotation *= Quaternion.Euler(0, 0, 180f);
        }
    }
}