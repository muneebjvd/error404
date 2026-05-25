using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; // UI ke liye

public class MiniGame3Manager : MonoBehaviour
{
    public static MiniGame3Manager Instance;
    
    public float survivalTime = 30f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public GameObject hintText;
    public GameObject playerObject;
    public Camera mainCam;

    private bool isGameOver = false;
    public bool gameStarted = false;
    private float initialTime;

    void Awake() => Instance = this;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main; 
        initialTime = survivalTime;
        Time.timeScale = 0f; 
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        if (hintText != null) hintText.SetActive(true);
        int count = 3;
        while (count > 0)
        {
            if (countdownText != null) countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        if (countdownText != null) countdownText.text = "BREACH START!";
        yield return new WaitForSecondsRealtime(0.5f);
        
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (hintText != null) hintText.SetActive(false);

        Time.timeScale = 1f; 
        gameStarted = true;
        Debug.Log("🚀 GAME STARTED! GRAVITY IS ON.");
    }

    void Update()
    {
        if (!gameStarted || isGameOver) return;

        survivalTime -= Time.deltaTime;
        float elapsed = initialTime - survivalTime;
        timerText.text = "BREACHING: " + Mathf.CeilToInt((elapsed / initialTime) * 100) + "%";

        CheckBounds();

        if (survivalTime <= 0) GameOver(true);
    }

    void CheckBounds()
    {
        if (playerObject == null || mainCam == null) return;
        Vector3 viewPos = mainCam.WorldToViewportPoint(playerObject.transform.position);
        
        if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
        {
            Debug.Log("💀 PLAYER OUT OF BOUNDS! ViewPos: " + viewPos);
            GameOver(false);
        }
    }

    public void GameOver(bool won)
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 1f; 
        
        CoreGameLogic core = FindObjectOfType<CoreGameLogic>();

        if (won)
        {
            Debug.Log("🏆 GAME WON! Telling CoreGameLogic...");
            if (core != null) core.OnMiniGameWon();
        }
        else
        {
            Debug.Log("❌ GAME LOST! Telling CoreGameLogic...");
            
            if (core != null) 
            {
                core.OnMiniGameLost();
            }
            else 
            {
                Debug.LogWarning("CoreGameLogic nahi mila! Fallback Loading Screen...");
                // Fallback Standalone Loading Screen
                StartCoroutine(StandaloneLoading(SceneManager.GetActiveScene().name)); 
            }
        }
    }

    public float GetCurrentDifficultyMultiplier()
    {
        if (!gameStarted) return 0f;
        float elapsed = initialTime - survivalTime;
        return (elapsed >= 10f) ? 1.5f : 1f;
    }

    public bool IsDualSideActive() { return gameStarted && (initialTime - survivalTime) >= 15f; }

    // --- Standalone Fallback Loading Screen ---
    IEnumerator StandaloneLoading(string sceneName)
    {
        GameObject canvasObj = new GameObject("LoadingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99999; 
        DontDestroyOnLoad(canvasObj); 

        GameObject bgObj = new GameObject("BlackBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.black; 
        bgImage.rectTransform.anchorMin = Vector2.zero;
        bgImage.rectTransform.anchorMax = Vector2.one;
        bgImage.rectTransform.sizeDelta = Vector2.zero;

        yield return new WaitForSecondsRealtime(0.5f);

        GameObject textObj = new GameObject("LoadingText");
        textObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = "L O A D I N G"; 
        tmpText.fontSize = 80;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.yellow; 
        tmpText.rectTransform.anchorMin = Vector2.zero;
        tmpText.rectTransform.anchorMax = Vector2.one;
        tmpText.rectTransform.sizeDelta = Vector2.zero;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;
        Destroy(canvasObj);
    }
}