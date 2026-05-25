using UnityEngine;

public class EndPathTrigger : MonoBehaviour
{
    // Inspector mein lanes ke number set karo: 0, 1, 2, 3 ya 4
    public int pathID; 
    
    // GameManager dhoondne ke liye reference
    private MiniGame2Manager manager;

    void Start()
    {
        manager = FindObjectOfType<MiniGame2Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            // Player ke object aur is lane ka number Manager ko bhej do
            manager.PlayerEnteredPath(pathID, other.gameObject);
        }
    }
}