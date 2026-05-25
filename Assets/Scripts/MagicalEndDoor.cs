using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class MagicalEndDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorModel; 

    private bool isTriggered = false;

    void Update()
    {
        // 1. Agar CoreGameLogic hi nahi hai, toh aagay mat jao
        if (CoreGameLogic.Instance == null) return;

        // 2. Main Logic: Check if Level is 4 or higher
        if (CoreGameLogic.Instance.currentLevel >= 4)
        {
            if (doorModel != null && !doorModel.activeSelf) 
            {
                // Force unhide!
                doorModel.SetActive(true); 
                Debug.Log("🔥 MAGIC DOOR IS UNHIDDEN! Player is at Level 4.");
            }
        }
        else
        {
            if (doorModel != null && doorModel.activeSelf) 
            {
                // Agar level 4 se kam hai toh chupa do
                doorModel.SetActive(false); 
            }
        }
    }

    public void PlayEndingSequence()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            Debug.Log("🎬 Ending Sequence Started!");
            StartCoroutine(EndGameSequence());
        }
    }

    IEnumerator EndGameSequence()
    {
        GameObject canvasObj = new GameObject("EndGameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; 

        GameObject bgObj = new GameObject("BlackBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0f); 
        bgImage.rectTransform.anchorMin = Vector2.zero;
        bgImage.rectTransform.anchorMax = Vector2.one;
        bgImage.rectTransform.sizeDelta = Vector2.zero;

        GameObject textObj = new GameObject("WinText");
        textObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = ""; 
        tmpText.fontSize = 80;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = new Color(0f, 1f, 0.2f, 1f); // Hacker Green Color
        
        tmpText.rectTransform.anchorMin = Vector2.zero;
        tmpText.rectTransform.anchorMax = Vector2.one;
        tmpText.rectTransform.sizeDelta = Vector2.zero;

        // Black Screen Fade
        float fadeDuration = 5f;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration; 
            bgImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null; 
        }
        bgImage.color = Color.black; 

        // Typewriter Word Reveal
        string[] words = { "YOU", "CRACKED", "THE", "GAME" };
        string currentText = "";
        
        for (int i = 0; i < words.Length; i++)
        {
            currentText += words[i] + " ";
            tmpText.text = currentText.Trim(); 
            yield return new WaitForSeconds(1f); 
        }

        yield return new WaitForSeconds(3f);

        if (CoreGameLogic.Instance != null)
        {
            CoreGameLogic.Instance.ResetAllData(); 
        }
        
        SceneManager.LoadScene("MainMenu"); 
    }
}