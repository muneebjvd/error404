using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    [Header("UI & Timer Settings")]
    public float timeLimit = 60f;
    public TextMeshProUGUI timerText; 
    public GameObject interactText;   

    private float timeLeft;
    private bool gameEnded = false;
    private bool isPlayerInZone = false;

    void Start()
    {
        timeLeft = timeLimit;
        if (interactText != null) interactText.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        timeLeft -= Time.deltaTime;
        
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft).ToString();
        }

        if (timeLeft <= 0)
        {
            gameEnded = true;
            Debug.Log("Time Up! Telling CoreGameLogic...");
            
            CoreGameLogic coreLogic = FindObjectOfType<CoreGameLogic>();

            if (coreLogic != null)
            {
                coreLogic.OnMiniGameLost();
            }
            else
            {
                Debug.LogWarning("CoreGameLogic nahi mili! Fallback Loading Screen...");
                StartCoroutine(StandaloneLoading(SceneManager.GetActiveScene().name));
            }
        }

        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            gameEnded = true;
            Debug.Log("Portal Entered! Calling CoreGameLogic...");
            
            CoreGameLogic coreLogic = FindObjectOfType<CoreGameLogic>();
            if (coreLogic != null)
            {
                coreLogic.OnMiniGameWon();
            }
            else
            {
                Debug.LogWarning("CoreGameLogic nahi mili! Shayad aap 2D scene direct test kar rahe hain.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (interactText != null) interactText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (interactText != null) interactText.SetActive(false);
        }
    }

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