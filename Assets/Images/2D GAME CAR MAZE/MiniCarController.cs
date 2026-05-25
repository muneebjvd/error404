using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
public class MiniCarController : MonoBehaviour
{
    [Header("Engine & Steering Settings")]
    public float moveSpeed = 7f;         
    public float turnSpeed = 250f;       
    public float dragAmount = 2f; // 'linearDrag' ko 'dragAmount' kar diya confusion khatam karne ke liye        
    public float maxSpeed = 15f;         
    
    [Header("Slip / Drift Settings")]
    [Range(0f, 1f)]
    public float slipFactor = 0.9f; 

    [Header("Audio Settings")]
    public AudioSource engineAudioSource; // Yahan car ka audio source aayega

    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // --- FIXED: Purani Unity naming use ki hai (drag aur angularDrag) ---
        rb.linearDamping = dragAmount;
        rb.angularDamping = dragAmount * 2f; 
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");

        // --- NAYA LOGIC: Engine Sound ---
        HandleEngineSound();
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteeringForce();
        HandleSidewaysSlip();
        
        // --- FIXED: velocity.sqrMagnitude use kiya hai ---
        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void ApplyEngineForce()
    {
        rb.AddForce(transform.up * moveInput * moveSpeed * 10f * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    void ApplySteeringForce()
    {
        float forwardVelocity = Vector2.Dot(rb.linearVelocity, transform.up);
        float steeringScale = Mathf.Clamp01(forwardVelocity / 5f); 

        rb.AddTorque(turnInput * -turnSpeed * steeringScale * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    void HandleSidewaysSlip()
    {
        float forwardVelocity = Vector2.Dot(rb.linearVelocity, transform.up);
        float sidewaysVelocity = Vector2.Dot(rb.linearVelocity, transform.right);

        float targetSidewaysVelocity = sidewaysVelocity * slipFactor;
        Vector2 localVelocity = new Vector2(targetSidewaysVelocity, forwardVelocity);

        // --- FIXED: velocity assign karne ka tareeqa ---
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    void HandleEngineSound()
    {
        if (engineAudioSource == null) return;

        // Agar player aagay ya peechay ja raha hai
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            if (!engineAudioSource.isPlaying)
            {
                engineAudioSource.Play();
            }
        }
        else
        {
            // Jab player button chhod de
            if (engineAudioSource.isPlaying)
            {
                engineAudioSource.Pause();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedArea"))
        {
            TriggerFail();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("RedArea"))
        {
            TriggerFail();
        }
    }

    // --- FIXED: Dynamic Manager Search ---
    void TriggerFail()
    {
        Debug.Log("Hit Red Area!");
        if (MiniGame2Manager.Instance != null)
        {
            // Level exit nahi hoga, sirf gari start par jayegi
            MiniGame2Manager.Instance.ResetCarOnly(); 
        }
    }
}