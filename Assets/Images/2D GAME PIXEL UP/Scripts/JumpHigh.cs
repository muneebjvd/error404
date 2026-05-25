using UnityEngine;

public class JumpModifier : MonoBehaviour
{
    public float newJumpForce = 12f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check karo ke takrane wala object Player hai
        if (other.CompareTag("Player"))
        {
            // Player ki script dhoondo
            MiniGamePlayer playerScript = other.GetComponent<MiniGamePlayer>();

            if (playerScript != null)
            {
                // Jump force ko update kar do
                playerScript.jumpForce = newJumpForce;
                
                // Optional: Agar tum chahte ho collider gayab ho jaye (power-up ki tarah)
                // gameObject.SetActive(false); 
                
                Debug.Log("Player Jump Force updated to: " + newJumpForce);
            }
        }
    }
}