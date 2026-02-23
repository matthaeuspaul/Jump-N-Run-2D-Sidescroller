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

    [Header("Settings")]
    [SerializeField] private string firstLevelScene = "Level_01";

    private void Start()
    {
        Time.timeScale = 1f;

        // Alten GameManager zerstören (DontDestroyOnLoad-Objekt aus vorherigem Spieldurchlauf)
        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        // SaveManager muss existieren - falls nicht vorhanden, erstellen
        EnsureSaveManager();

        UpdateUI();
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

        // Continue-Button nur aktiv wenn ein Spielstand vorhanden ist
        if (continueButton != null)
            continueButton.interactable = hasSave;

        // Spielstand-Info anzeigen (z.B. "Zuletzt gespielt: 23.02.2026 14:32 – Level_02")
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

        // Focus auf den sinnvollsten Button setzen
        if (hasSave && continueButton != null)
            continueButton.Select();
        else if (newGameButton != null)
            newGameButton.Select();
    }

    // -------------------------------------------------------
    // Button Events
    // -------------------------------------------------------

    /// <summary>
    /// Setzt das Continue-Flag und lädt die gespeicherte Scene.
    /// </summary>
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

    /// <summary>
    /// Löscht den alten Spielstand und startet ein neues Spiel.
    /// </summary>
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

    public void OpenOptions()
    {
        Debug.Log("Options menu not yet implemented");
    }

    public void OpenCredits()
    {
        Debug.Log("Credits not yet implemented");
    }
}