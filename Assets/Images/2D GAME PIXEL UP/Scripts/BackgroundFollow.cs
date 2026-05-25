using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cameraTransform; // Yahan Inspector mein 'Main Camera' drag karein
    public float smoothTime = 0.2f;   // Aapka manga hua 0.2s delay
    
    private Vector3 velocity = Vector3.zero;
    private float initialZ;

    void Start()
    {
        // Background ki asli Z-position save kar lo taake wo camera ke aage na aaye
        initialZ = transform.position.z;
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        // Camera ki target position (Z ko ignore karte hue)
        Vector3 targetPosition = new Vector3(cameraTransform.position.x, cameraTransform.position.y, initialZ);

        // SmoothDamp function 0.2s ka delay/smoothing add karega
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}