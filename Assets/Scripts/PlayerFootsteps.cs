using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource footstepSource;
    
    // Ab humein AudioClip yahan script mein dene ki zaroorat nahi
    // Wo direct AudioSource ke andar hi lagana hai

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check karte hain ke player move kar raha hai ya nahi
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        if (isMoving)
        {
            // Agar player chal raha hai aur awaz nahi aa rahi, toh Play karo
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            // Agar player ruk gaya hai aur awaz aa rahi hai, toh foran rok do
            if (footstepSource.isPlaying)
            {
                footstepSource.Pause(); // Pause karne se agli baar wahin se shuru hoga
            }
        }
    }
}