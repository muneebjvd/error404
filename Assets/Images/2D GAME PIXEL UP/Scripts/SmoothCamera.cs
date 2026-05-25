using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;        
    public float smoothTime = 0.3f; // Jitna zyada, utna smooth delay (0.3s best hai)
    public Vector3 offset = new Vector3(0, 2, -10); // Player se thora upar aur peeche
    
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target != null)
        {
            // Target position jahan camera ko pohnchna hai
            Vector3 targetPosition = target.position + offset;

            // SmoothDamp: Ye asli "Butter Smooth" effect deta hai
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}