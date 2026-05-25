using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PacketController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 8f; 
    public float tiltSpeed = 5f;       
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.5f; 
        rb.constraints = RigidbodyConstraints2D.None; 
    }

    void Update()
    {
        if (MiniGame3Manager.Instance != null && !MiniGame3Manager.Instance.gameStarted) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("✅ PLAYER JUMPED!");
            rb.linearVelocity = Vector2.up * jumpForce;

            // Jab bhi jump hoga, sound chalega
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    void FixedUpdate()
    {
        if (MiniGame3Manager.Instance != null && !MiniGame3Manager.Instance.gameStarted) return;
        float targetZRotation = Mathf.Clamp(rb.linearVelocity.y * tiltSpeed, -45f, 20f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetZRotation), Time.deltaTime * tiltSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("⚠️ TRIGGER HIT DETECTED WITH: " + other.gameObject.name);
        if (other.CompareTag("Obstacle")) 
        {
             MiniGame3Manager.Instance.GameOver(false); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("💥 HARD COLLISION DETECTED WITH: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Obstacle")) 
        {
             MiniGame3Manager.Instance.GameOver(false); 
        }
    }
}