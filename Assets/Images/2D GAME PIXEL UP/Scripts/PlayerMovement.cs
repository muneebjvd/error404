using UnityEngine;

public class MiniGamePlayer : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 8f;
    public float jumpForce = 8f;

    [Header("Sliding Settings")]
    public float slopeLimit = 45f;      // Kitne degree par slide shuru ho (e.g. 45)
    public float slideSpeed = 12f;     // Niche phisalne ki raftaar
    public LayerMask groundLayer;
    public float rayDistance = 0.1f;    // BoxCast ke liye isay thora chota (0.1) hi rakhna behtar hai

    [Header("Audio Settings")]
    public AudioSource audioSource;    // Player ka AudioSource yahan aayega
    public AudioClip jumpSoundClip;    // Jump ki MP3/WAV file yahan aayegi

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private bool isGrounded;
    private bool isSliding;
    private Vector2 slopeNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;
        
        // Physics ki smoothness ke liye
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        CheckGroundAndSlope();
        
        if (!isSliding)
        {
            HandleMovement();
            HandleJump();
        }
        else
        {
            ApplySlide();
        }

        // Animations update
        anim.SetBool("isGrounded", isGrounded);
    }

    void CheckGroundAndSlope()
    {
        // BoxCast ka size tayyar kar rahe hain. 
        // Width ko 0.9f se multiply kiya hai taake player deewar ke sath chipke toh wall ko ground na samajh le.
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * 0.9f, 0.1f);
        float castDistance = (boxCollider.bounds.size.y / 2) + rayDistance;

        // Raycast ki jagah BoxCast use kar rahe hain jo edge-to-edge check karega
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.down, castDistance, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
            slopeNormal = hit.normal; // Surface kis taraf face kar rahi hai

            // Angle calculate karna
            float slopeAngle = Vector2.Angle(Vector2.up, slopeNormal);

            if (slopeAngle > slopeLimit)
            {
                isSliding = true;
            }
            else
            {
                isSliding = false;
            }
        }
        else
        {
            isGrounded = false;
            isSliding = false;
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
        {
            anim.SetBool("isWalking", true);
            spriteRenderer.flipX = (moveInput < 0);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");

            // --- NAYA LOGIC: Yahan jump ki awaz chalegi ---
            if (audioSource != null && jumpSoundClip != null)
            {
                audioSource.PlayOneShot(jumpSoundClip);
            }
        }
    }

    void ApplySlide()
    {
        // Slope ke parallel niche ki taraf force lagana
        float slideDirection = (slopeNormal.x > 0) ? 1 : -1;
        
        // Smooth sliding velocity
        rb.linearVelocity = new Vector2(slopeNormal.x * slideSpeed, rb.linearVelocity.y);
        
        // Sliding ke waqt walk animation rok do
        anim.SetBool("isWalking", false);
    }

    // Yeh function tumhe Scene view mein Ground Detection Box dikhayega (Sirf Editor mein)
    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * 0.9f, 0.1f);
            float castDistance = (boxCollider.bounds.size.y / 2) + rayDistance;
            Vector2 center = (Vector2)boxCollider.bounds.center + Vector2.down * castDistance;
            Gizmos.DrawWireCube(center, boxSize);
        }
    }
}