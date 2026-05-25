using UnityEngine;

public class TriggerActivation : MonoBehaviour
{
    public RotateOnTrigger boxToRotate; // Inspector mein Target Box yahan drag karein

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check karo ke takrane wala Player hai
        if (other.CompareTag("Player"))
        {
            if (boxToRotate != null)
            {
                boxToRotate.ActivateRotation();
            }
        }
    }
}