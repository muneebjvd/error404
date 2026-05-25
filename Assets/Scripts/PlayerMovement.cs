using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    public CharacterController controller;
    public float walkSpeed = 6f;
    public float crouchSpeed = 3f;
    public float gravity = -19.62f; // Feels snappier than standard gravity
    public float jumpHeight = 1.5f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Look Parameters")]
    public Transform playerCamera; // Assign the Main Camera here in the Inspector
    public float mouseSensitivity = 2f;
    [Range(0f, 0.5f)] public float lookSmoothness = 0.05f;

    private float xRotation = 0f;
    private Vector2 currentMouseLook;
    private Vector2 smoothMoveVelocity;

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock and hide the cursor for proper FPS look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ---> PAUSE FIX: Agar game pause hai toh aage ka koi movement/rotation code nahi chalega <---
        if (Time.timeScale == 0f) return;

        // 0. Mouse Look Logic
        if (playerCamera != null)
        {
            // Get raw input for better smoothing control
            Vector2 targetMouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            
            // Apply smoothing to the mouse input
            currentMouseLook = Vector2.SmoothDamp(currentMouseLook, targetMouseDelta, ref smoothMoveVelocity, lookSmoothness);

            // Vertical rotation (look up/down)
            xRotation -= currentMouseLook.y * mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp to prevent looking past straight up/down
            
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Horizontal rotation (look left/right)
            transform.Rotate(Vector3.up * (currentMouseLook.x * mouseSensitivity));
        }

        // 1. Check if on the ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Forces player to stick to the floor
        }

        // 2. Get inputs (WASD or Arrows)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // 3. Sit / Crouch Logic (Hold Left Control)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            controller.Move(move * crouchSpeed * Time.deltaTime);
        }
        else
        {
            // Stand back up
            controller.height = defaultHeight;
            controller.Move(move * walkSpeed * Time.deltaTime);
        }

        // 4. Jump Logic (Spacebar)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}