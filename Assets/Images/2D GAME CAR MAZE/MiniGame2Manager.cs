using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using System.Collections;

public class MiniGame2Manager : MonoBehaviour
{
    public static MiniGame2Manager Instance;

    [Header("UI & Timer")]
    public float timeLimit = 200f;
    public TextMeshProUGUI timerText;

    [Header("Teleport Points")]
    public Transform mapStartPoint;         
    public Transform sequenceRetryPoint;   
    public GameObject playerObject; 

    private float timeLeft;
    private bool gameEnded = false;
    private int puzzleStep = 0; 

    void Awake() { Instance = this; }
    
    void Start()
    {
        timeLeft = timeLimit;
        puzzleStep = 0; 
        Debug.Log("Puzzle Started. Step 0: Go to Path 4.");
    }

    void Update()
    {
        if (gameEnded) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0)
        {
            GameOver(false); 
        }
    }

    public void PlayerEnteredPath(int pathNumber, GameObject player)
    {
        if (gameEnded) return;

        bool correctMove = false;
        switch (puzzleStep)
        {
            case 0: if (pathNumber == 4) { correctMove = true; puzzleStep = 1; } break;
            case 1: if (pathNumber == 0) { correctMove = true; puzzleStep = 2; } break;
            case 2: if (pathNumber == 4) { GameOver(true); return; } break; 
        }

        if (correctMove)
        {
            Debug.Log("Sahi raasta! Re-appearing...");
            TeleportPlayer(player, sequenceRetryPoint);
        }
        else
        {
            Debug.Log("Ghalat raasta! Back to start.");
            ResetCarOnly();
        }
    }

    public void TeleportPlayer(GameObject player, Transform targetPoint)
    {
        if (player != null && targetPoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) 
            { 
                rb.linearVelocity = Vector2.zero; 
                rb.angularVelocity = 0f; 
            }
            player.transform.position = targetPoint.position;
            player.transform.rotation = targetPoint.rotation;
        }
    }

    public void ResetCarOnly()
    {
        if (gameEnded) return;
        Debug.Log("Resetting Car Position...");
        puzzleStep = 0; 
        TeleportPlayer(playerObject, mapStartPoint);
    }

    public void GameOver(bool won)
    {
        if (gameEnded) return;
        gameEnded = true;

        CoreGameLogic coreLogic = FindObjectOfType<CoreGameLogic>();

        if (won)
        {
            Debug.Log("Sequence Complete! Level Won.");
            if (coreLogic != null) coreLogic.OnMiniGameWon();
        }
        else
        {
            Debug.Log("Time Out! Level Lost. Telling CoreGameLogic...");
            
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
    }

    public void FailLevel()
    {
        ResetCarOnly();
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