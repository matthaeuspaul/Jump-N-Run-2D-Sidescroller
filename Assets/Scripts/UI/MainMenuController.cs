using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;

    [Header("Continue Info (Optional)")]
    [SerializeField] private TextMeshProUGUI saveInfoText;

    [Header("Settings")]
    [SerializeField] private string firstLevelScene = "Level_01";
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        EnsureSaveManager();
        UpdateUI();

        if (settingsPanel) settingsPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    private void EnsureSaveManager()
    {
        if (SaveManager.Instance == null)
            new GameObject("SaveManager").AddComponent<SaveManager>();
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
                saveInfoText.text = data != null
                    ? $"Letzter Speicherstand: {data.saveTime}  |  {data.sceneName}  |  ♥ {data.lives}"
                    : "";
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

    public void ContinueGame()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave()) return;

        SaveData data = SaveManager.Instance.Load();
        if (data == null || string.IsNullOrEmpty(data.sceneName)) return;

        SaveManager.Instance.SetContinuing(true);
        SceneManager.LoadScene(data.sceneName);
    }

    public void StartNewGame()
    {
        SaveManager.Instance?.DeleteSave();
        SceneManager.LoadScene(firstLevelScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        SettingsMenuController ctrl = settingsPanel.GetComponent<SettingsMenuController>();
        if (ctrl != null)
            ctrl.Open(mainMenuPanel);
        else
            settingsPanel.SetActive(true);
    }

    public void OpenCredits() { }
}