using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using System.Collections.Generic;

public class CoreGameLogic : MonoBehaviour
{
    public static CoreGameLogic Instance;
    private static bool isFreshGameStart = true;

    [Header("Game State (4-0-4 Logic)")]
    public int currentLevel = 1; 
    public int loopCounter = 0; 
    public int failureCount = 0; 

    [Header("Interactions")]
    public bool isPCInteractable = false;
    private bool isSequenceDoorInteractable = true;

    [Header("Dynamic References")]
    public DoorScript.Door[] sequenceDoors; 
    public DoorScript.Door[] normalDoors;
    public Light[] spotLights; 

    [Header("Settings")]
    public float flickerSpeed = 0.5f; 
    public float defaultIntensity = 1.5f; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        if (isFreshGameStart)
        {
            ResetAllData();
            isFreshGameStart = false; 
        }
    }

    void Start()
    {
        LoadGameState();
        RefreshReferences(); 
        CheckConditions();

        // --- FIX 1: Game start hote hi agar Main Menu hai tou Mouse azaad kardo ---
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (failureCount == 1 && spotLights != null && spotLights.Length > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FlickerLightsRoutine());
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // --- FIX 2: Naya scene aate hi pichla phansa hua loading screen urra do ---
        GameObject loadingCanvas = GameObject.Find("LoadingCanvas_Munoob");
        if (loadingCanvas != null)
        {
            Destroy(loadingCanvas);
        }

        // --- FIX 3: Agar Main Menu load hua hai tou Mouse dikhao ---
        if (scene.buildIndex == 0 || scene.name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (scene.name == "MainGame") 
        {
            RefreshReferences();
            CheckConditions();
            
            if (failureCount == 1)
            {
                StopAllCoroutines(); 
                StartCoroutine(FlickerLightsRoutine());
            }
        }
    }

    void RefreshReferences()
    {
        GameObject[] sDoors = GameObject.FindGameObjectsWithTag("SequenceDoor");
        List<DoorScript.Door> sList = new List<DoorScript.Door>();
        foreach(GameObject g in sDoors) sList.Add(g.GetComponent<DoorScript.Door>());
        sequenceDoors = sList.ToArray();

        GameObject[] nDoors = GameObject.FindGameObjectsWithTag("NormalDoor");
        List<DoorScript.Door> nList = new List<DoorScript.Door>();
        foreach(GameObject g in nDoors) nList.Add(g.GetComponent<DoorScript.Door>());
        normalDoors = nList.ToArray();

        GameObject[] lights = GameObject.FindGameObjectsWithTag("FlickerLight");
        List<Light> lList = new List<Light>();
        foreach(GameObject g in lights) lList.Add(g.GetComponent<Light>());
        spotLights = lList.ToArray();
    }

    public void OnLoopTriggerCrossed()
    {
        loopCounter++;
        if (loopCounter >= 5) loopCounter = 0;
        CheckConditions();
    }

    void CheckConditions()
    {
        bool isTargetMet = false;
        if (currentLevel == 1 && loopCounter == 4) isTargetMet = true;
        else if (currentLevel == 2 && loopCounter == 0) isTargetMet = true;
        else if (currentLevel == 3 && loopCounter == 4) isTargetMet = true;

        if (isTargetMet)
        {
            isPCInteractable = true;
            isSequenceDoorInteractable = true; 
        }
        else
        {
            isPCInteractable = false;
            isSequenceDoorInteractable = false; 

            foreach (DoorScript.Door door in sequenceDoors)
                if (door != null && door.open) door.OpenDoor(); 
            
            foreach (DoorScript.Door door in normalDoors)
                if (door != null && door.open) door.OpenDoor(); 
        }
    }

    public bool CanPlayerOpenThisDoor(DoorScript.Door doorToCheck)
    {
        foreach (DoorScript.Door d in normalDoors)
            if (d == doorToCheck) return true;
        return isSequenceDoorInteractable;
    }

    public void InteractWithPC()
    {
        if (!isPCInteractable) return;
        SaveGameState();
        
        if (currentLevel == 1) LoadSceneWithLoadingScreen("Game2d1");
        else if (currentLevel == 2) LoadSceneWithLoadingScreen("Game2d2");
        else if (currentLevel == 3) LoadSceneWithLoadingScreen("Game2d3");
    }

    public void OnMiniGameWon()
    {
        currentLevel++;
        loopCounter = 0; failureCount = 0; 
        SaveGameState();
        Debug.Log("🌟 MINI GAME WON! Current Level is now: " + currentLevel);
        
        LoadSceneWithLoadingScreen("MainGame"); 
    }

    public void OnMiniGameLost()
    {
        failureCount++;
        loopCounter = 0; 
        
        if (failureCount >= 2)
        {
            ResetAllData();
            isFreshGameStart = true; 
            StartCoroutine(ShowSavageFailScreen());
        }
        else
        {
            SaveGameState();
            LoadSceneWithLoadingScreen("MainGame");
        }
    }

    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        StartCoroutine(LoadingScreenRoutine(sceneName));
    }

    IEnumerator LoadingScreenRoutine(string sceneName)
    {
        // --- FIX 4: Canvas ko specific naam diya taake aasaani se remove ho sakay ---
        GameObject canvasObj = new GameObject("LoadingCanvas_Munoob");
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
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // --- FIX 5: Fallback safety. Wese tou OnSceneLoaded isay destroy kardega, par agar na kare tou 0.5s baad khud hojaye ---
        yield return new WaitForSecondsRealtime(0.5f);
        if (canvasObj != null) Destroy(canvasObj);
    }

    IEnumerator ShowSavageFailScreen()
    {
        // --- FIX 6: Fail hone par foran Mouse unlock kardo taake aglay scene me masla na ho ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameObject canvasObj = new GameObject("FailCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.transform.SetParent(this.transform); 
        canvas.sortingOrder = 9999; 

        GameObject bgObj = new GameObject("MaroonBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.5f, 0f, 0f, 0f); 
        bgImage.rectTransform.anchorMin = Vector2.zero;
        bgImage.rectTransform.anchorMax = Vector2.one;
        bgImage.rectTransform.sizeDelta = Vector2.zero;

        GameObject textObj = new GameObject("FailText");
        textObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = ""; 
        tmpText.fontSize = 90;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white; 
        
        tmpText.rectTransform.anchorMin = Vector2.zero;
        tmpText.rectTransform.anchorMax = Vector2.one;
        tmpText.rectTransform.sizeDelta = Vector2.zero;

        float fadeDuration = 3f; 
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            bgImage.color = new Color(0.5f, 0f, 0f, alpha); 
            yield return null; 
        }
        bgImage.color = new Color(0.5f, 0f, 0f, 1f);

        string[] words = { "YOU", "NOOB!", "BYE", "BYE" };
        string currentText = "";
        
        for (int i = 0; i < words.Length; i++)
        {
            currentText += words[i] + " ";
            tmpText.text = currentText.Trim(); 
            yield return new WaitForSeconds(1f); 
        }

        yield return new WaitForSeconds(2f); 
        Destroy(canvasObj); 
        
        LoadSceneWithLoadingScreen("MainMenu"); 
    }

    IEnumerator FlickerLightsRoutine()
    {
        while (true)
        {
            foreach (Light l in spotLights) if (l != null) l.intensity = 0f;
            yield return new WaitForSeconds(flickerSpeed);
            foreach (Light l in spotLights) if (l != null) l.intensity = defaultIntensity;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    void SaveGameState()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("LoopCounter", loopCounter);
        PlayerPrefs.SetInt("FailureCount", failureCount);
    }

    void LoadGameState() {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        loopCounter = PlayerPrefs.GetInt("LoopCounter", 0);
        failureCount = PlayerPrefs.GetInt("FailureCount", 0);
    }

    public void ResetAllData() {
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("LoopCounter");
        PlayerPrefs.DeleteKey("FailureCount");
        currentLevel = 1;
        loopCounter = 0;
        failureCount = 0;
    }
}