using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GhostEnemy : MonoBehaviour
{
    [Header("Targets & Door")]
    public Transform player;           
    public DoorScript.Door lockedDoor; 
    
    [Header("Ghost Settings")]
    public float enemySpeed = 4f;      // Player se 2 kam rakhna
    public float killDistance = 1.2f; 
    public float chaseDuration = 30f; 

    private bool isChasing = false;
    private float chaseTimer = 0f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // --- GHOST PHYSICS SETUP ---
        rb.useGravity = false; 
        rb.isKinematic = true; // Taake deewaron se nikal sakay
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Start position save kar lo
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (player == null) return;

        // 1. Door Signal Check
        if (!isChasing && lockedDoor != null)
        {
            if (lockedDoor.open) 
            {
                StartChase();
            }
        }

        // 2. Chase Logic
        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;

            if (chaseTimer > 0)
            {
                MoveGhost();
                CheckProximity();
            }
            else
            {
                ResetGhost();
            }
        }
    }

    void StartChase()
    {
        isChasing = true;
        chaseTimer = chaseDuration;
        Debug.Log("👻 GHOST IS ANGRY! Chase started for " + chaseDuration + "s");
    }

    void MoveGhost()
    {
        // Player ki taraf rukh (Look At)
        Vector3 direction = (player.position - transform.position).normalized;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // FORCE MOVEMENT: NavMesh ke bagair seedha peecha
        // MovePosition use kar rahe hain kyunke rb.isKinematic = true hai
        rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
    }

    void CheckProximity()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        
        // Agar 1.2 points ke kareeb aa jaye
        if (dist <= killDistance)
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            Debug.Log("💀 GHOST CAUGHT THE PLAYER!");
            
            if (CoreGameLogic.Instance != null)
            {
                CoreGameLogic.Instance.OnMiniGameLost(); 
                CoreGameLogic.Instance.OnMiniGameLost();
            }
            
            ResetGhost();
        }
    }

    public void ResetGhost()
    {
        isChasing = false;
        chaseTimer = 0;
        
        // Wapas purani jagah phenk do
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // Darwaza reset (taake dobara kholne par trigger ho)
        if (lockedDoor != null)
        {
            lockedDoor.open = false; 
        }
        
        Debug.Log("👻 Ghost reset to starting point.");
    }
}