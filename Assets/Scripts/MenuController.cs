using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Scene change karne ke liye zaroori hai

public class MenuIntro : MonoBehaviour {
    public CanvasGroup titleLogo;
    public CanvasGroup[] buttons;

    [Header("UI Panels")]
    public GameObject creditsCanvas; // Wo hided canvas jo credits show karega

    [Header("Animation Settings")]
    public float initialLogoScale = 0.25f;
    public Transform background;

    private Vector3 originalLogoScale;
    private Vector3 originalBgScale;

    void Start() {
        // Shuru mein sab gayab (except titleLogo which scales instead of fading now)
        titleLogo.alpha = 1f; // fade ki jagah scale use kar rahe hain
        originalLogoScale = titleLogo.transform.localScale;
        titleLogo.transform.localScale = originalLogoScale * initialLogoScale;

        if (background != null) {
            originalBgScale = background.localScale;
        }

        foreach (var b in buttons) b.alpha = 0;

        // Credits panel ko shuru mein hide rakho
        if(creditsCanvas != null) creditsCanvas.SetActive(false);

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro() {
        // 1. Pehle Title scale in
        float progress = 0;
        while (progress < 1) {
            progress += Time.deltaTime * 0.8f;
            titleLogo.transform.localScale = Vector3.Lerp(originalLogoScale * initialLogoScale, originalLogoScale, progress);
            yield return null;
        }
        titleLogo.transform.localScale = originalLogoScale;

        StartCoroutine(BreathingAnimation());

        yield return new WaitForSeconds(1f); // Thoda suspense

        // 2. Phir Buttons ek ek karke
        foreach (var btn in buttons) {
            while (btn.alpha < 1) {
                btn.alpha += Time.deltaTime * 2f;
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator BreathingAnimation() {
        Vector3 logoTargetScale = originalLogoScale * 1.2f;
        Vector3 bgTargetScale = originalBgScale * 1.2f;

        while (true) {
            // Zoom in for 20 seconds
            float t = 0;
            while (t < 20f) {
                t += Time.deltaTime;
                titleLogo.transform.localScale = Vector3.Lerp(originalLogoScale, logoTargetScale, t / 6f);
                if (background != null) {
                    background.localScale = Vector3.Lerp(originalBgScale, bgTargetScale, t / 6f);
                }
                yield return null;
            }

            // Go back in 15 seconds
            t = 0;
            while (t < 15f) {
                t += Time.deltaTime;
                titleLogo.transform.localScale = Vector3.Lerp(logoTargetScale, originalLogoScale, t / 5f);
                if (background != null) {
                    background.localScale = Vector3.Lerp(bgTargetScale, originalBgScale, t / 5f);
                }
                yield return null;
            }
        }
    }

    // --- BUTTON FUNCTIONS ---

    public void PlayGame() {
        // MainGame scene load karega (Build settings mein name check kar lena)
        SceneManager.LoadScene("MainGame");
    }

    public void ShowCredits() {
        // Credits panel ko show karega
        if(creditsCanvas != null) creditsCanvas.SetActive(true);
    }

    public void HideCredits() {
        // Credits panel ko wapas hide karne ke liye (Close button pe laga dena)
        if(creditsCanvas != null) creditsCanvas.SetActive(false);
    }

    public void QuitGame() {
        Debug.Log("Game Quit!"); // Editor mein check karne ke liye
        Application.Quit(); // Asal game band kar dega
    }
}