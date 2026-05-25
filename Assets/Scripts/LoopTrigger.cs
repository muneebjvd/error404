using UnityEngine;

public class LoopTrigger : MonoBehaviour
{
    private CoreGameLogic gameLogic;
    private bool canTrigger = true;
    public float cooldownTime = 5f; // Player ko 5 second baad dobara trigger karne ijazat milegi

    void Start()
    {
        gameLogic = FindObjectOfType<CoreGameLogic>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canTrigger)
        {
            gameLogic.OnLoopTriggerCrossed();
            
            canTrigger = false; // Lock trigger
            Invoke("ResetTrigger", cooldownTime); // 5 second baad unlock karo
        }
    }

    void ResetTrigger()
    {
        canTrigger = true;
    }
}