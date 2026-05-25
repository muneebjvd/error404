using UnityEngine;
using UnityEngine.SceneManagement; // Scene change karne ke liye

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        // Shuru mein panel hide rakho aur game ki speed normal (1) rakho
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Jab bhi player Escape (Esc) dabaye
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true); // Panel show karo
        Time.timeScale = 0f; // Game freeze kar do (Animation/Movement ruk jayegi)
        isPaused = true;
        
        // Mouse cursor screen par le aao taake UI pe click ho sake
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Panel wapas hide karo
        Time.timeScale = 1f; // Game wapas normal speed par
        isPaused = false;

        // Agar tumhara FPS controller hai, toh cursor wapas lock kar do
        // (Agar third-person/point-and-click hai toh in do lines ko hata dena)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu()
    {
        // ZAROORI BAAT: Scene change karne se pehle time wapas 1 karna zaroori hai
        // warna tumhara Main Menu bhi freeze ho jayega!
        Time.timeScale = 1f; 
        
        // "MainMenu" ki jagah apne menu scene ka exact naam likhna jo Build Settings mein hai
        SceneManager.LoadScene("MainMenu"); 
    }
}