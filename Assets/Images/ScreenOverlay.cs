using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System; // Action callback ke liye

public class FailOverlayEffect : MonoBehaviour
{
    public static FailOverlayEffect Instance;
    private Image overlayImage;

    void Awake()
    {
        Instance = this;
        SetupOverlay(); // Code khud UI Canvas aur Image banayega!
    }

    void SetupOverlay()
    {
        // 1. Canvas Create karna
        GameObject canvasObj = new GameObject("FailOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Sab se oopar (Top UI)

        // 2. Maroon Image Create karna
        GameObject imageObj = new GameObject("MaroonImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        overlayImage = imageObj.AddComponent<Image>();

        // 3. Image ko Full Screen par stretch karna
        RectTransform rect = overlayImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        // 4. Shuru mein bilkul invisible (Alpha = 0)
        overlayImage.color = new Color(0.5f, 0f, 0f, 0f); 
    }

    // Yeh function aapka GameManager call karega fail hone par
    public void PlayFailEffect(Action onComplete)
    {
        StartCoroutine(FailRoutine(onComplete));
    }

    IEnumerator FailRoutine(Action onComplete)
    {
        // Maroon Color, 50% Opacity (R=0.5, G=0, B=0, Alpha=0.5)
        overlayImage.color = new Color(0.5f, 0f, 0.1f, 0.5f); 
        
        Debug.Log("🔴 Maroon Fail Screen Active! Waiting 3 seconds...");

        // 3 seconds ka wait (Realtime taake timeScale=0 par bhi chale)
        yield return new WaitForSecondsRealtime(3f);
        
        // 3 second baad original scene switch wala function chala do
        if(onComplete != null) onComplete.Invoke();
    }
}