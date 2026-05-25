using UnityEngine;

public class CameraFollowBounds : MonoBehaviour
{
    [Header("Targets")]
    public Transform playerCar;          
    public SpriteRenderer mapBackground; 

    [Header("Camera Settings")]
    public float smoothSpeed = 10f;      
    
    [Header("Fine Tuning")]
    [Tooltip("Isay barhane se camera borders se mazeed andar rukega")]
    public float borderPadding = 0.2f;   // 2 pixels ke liye 0.1 ya 0.2 kaafi hota hai units mein

    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;
    
    private Vector3 minBounds;
    private Vector3 maxBounds;

    void Start()
    {
        cam = Camera.main;

        if (mapBackground != null)
        {
            camHalfHeight = cam.orthographicSize;
            camHalfWidth = camHalfHeight * cam.aspect;

            minBounds = mapBackground.bounds.min;
            maxBounds = mapBackground.bounds.max;
        }
    }

    void LateUpdate()
    {
        if (playerCar == null || mapBackground == null) return;

        Vector3 desiredPosition = playerCar.position;
        desiredPosition.z = transform.position.z;

        // --- FIXED: Added borderPadding to push the camera further inside ---
        float clampedX = Mathf.Clamp(desiredPosition.x, 
            minBounds.x + camHalfWidth + borderPadding, 
            maxBounds.x - camHalfWidth - borderPadding);

        float clampedY = Mathf.Clamp(desiredPosition.y, 
            minBounds.y + camHalfHeight + borderPadding, 
            maxBounds.y - camHalfHeight - borderPadding);

        desiredPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}