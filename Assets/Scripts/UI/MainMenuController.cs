using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    [Header("Continue Info (Optional)")]
    [Tooltip("Optional: Ein TextMeshPro-Text der den Speicherstand-Zeitstempel anzeigt")]
    [SerializeField] private TextMeshProUGUI saveInfoText;

    // NEU: Settings-Panel direkt im MainMenu Canvas
    [Header("Settings")]
    [SerializeField] private string firstLevelScene = "Level_01";
    [SerializeField] private GameObject mainMenuPanel;   // Das Haupt-Panel (mit den Buttons)
    [SerializeField] private GameObject settingsPanel;   // Das Settings-Panel

    private void Start()
    {
        Time.timeScale = 1f;

        // Alten GameManager zerstören (DontDestroyOnLoad-Objekt aus vorherigem Spieldurchlauf)
        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        EnsureSaveManager();
        UpdateUI();

        // Settings-Panel beim Start schließen
        if (settingsPanel) settingsPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    private void EnsureSaveManager()
    {
        if (SaveManager.Instance == null)
        {
            GameObject go = new GameObject("SaveManager");
            go.AddComponent<SaveManager>();
            Debug.Log("[MainMenuController] SaveManager wurde neu erstellt.");
        }
    }

    private void UpdateUI()
    {
        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSave();

        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (saveInfoText != null)
        {
            if (hasSave)
            {
                SaveData data = SaveManager.Instance.Load();
                if (data != null)
                    saveInfoText.text = $"Letzter Speicherstand: {data.saveTime}  |  {data.sceneName}  |  ♥ {data.lives}";
                else
                    saveInfoText.text = "";
            }
            else
            {
                saveInfoText.text = "Kein Spielstand vorhanden";
            }
        }

        if (hasSave && continueButton != null)
            continueButton.Select();
        else if (newGameButton != null)
            newGameButton.Select();
    }

    // -------------------------------------------------------
    // Button Events
    // -------------------------------------------------------

    public void ContinueGame()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave())
        {
            Debug.LogWarning("[MainMenuController] Kein Spielstand zum Fortsetzen gefunden!");
            return;
        }

        SaveData data = SaveManager.Instance.Load();
        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            Debug.LogError("[MainMenuController] Spielstand beschädigt oder leer!");
            return;
        }

        SaveManager.Instance.SetContinuing(true);
        SceneManager.LoadScene(data.sceneName);
    }

    public void StartNewGame()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        SceneManager.LoadScene(firstLevelScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>Öffnet das Settings-Panel und blendet das Hauptmenü aus.</summary>
    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[MainMenuController] SettingsPanel nicht zugewiesen!");
            return;
        }

        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        SettingsMenuController ctrl = settingsPanel.GetComponent<SettingsMenuController>();
        if (ctrl != null)
            ctrl.Open(mainMenuPanel); // mainMenuPanel wird beim Close wieder eingeblendet
        else
            settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        Debug.Log("Credits not yet implemented");
    }
}