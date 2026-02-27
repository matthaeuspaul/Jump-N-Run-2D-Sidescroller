using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Hearts UI")]
    [SerializeField] private GameObject heartsContainer;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private int maxLives = 3;

    [Header("Coins UI")]
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletionScreen;
    [SerializeField] private GameObject settingsPanel;

    [Header("Screen Transitions")]
    [SerializeField] private ScreenFade screenFader;

    public ScreenFade Fader => screenFader;

    private Image[] _heartImages;

    private void Awake()
    {
        if (screenFader == null)
            screenFader = GetComponentInChildren<ScreenFade>();
    }

    public void Initialize()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (levelCompletionScreen) levelCompletionScreen.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);

        SpawnHearts();
    }

    private void SpawnHearts()
    {
        if (heartsContainer == null || heartPrefab == null)
        {
            Debug.LogWarning("[UIManager] HeartsContainer or HeartPrefab not assigned!");
            return;
        }

        foreach (Transform child in heartsContainer.transform)
            Destroy(child.gameObject);

        _heartImages = new Image[maxLives];

        for (int i = 0; i < maxLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer.transform);
            _heartImages[i] = heart.GetComponent<Image>();
            _heartImages[i].sprite = fullHeartSprite;
        }
    }

    #region HUD Updates

    public void UpdateLivesDisplay(int lives)
    {
        if (_heartImages == null)
        {
            Debug.LogWarning("[UIManager] Heart images not initialized!");
            return;
        }

        for (int i = 0; i < _heartImages.Length; i++)
            _heartImages[i].sprite = i < lives ? fullHeartSprite : emptyHeartSprite;
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "x " + coins;
        else
            Debug.LogWarning("[UIManager] CoinsText is not assigned!");
    }

    #endregion

    #region Pause Menu

    public void ShowPauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(true);
        else Debug.LogWarning("[UIManager] PauseMenu is not assigned!");
    }

    public void HidePauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    #endregion

    #region Settings Panel

    public void ShowSettingsPanel()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[UIManager] SettingsPanel is not assigned!");
            return;
        }

        if (pauseMenu) pauseMenu.SetActive(false);

        SettingsMenuController ctrl = settingsPanel.GetComponent<SettingsMenuController>();
        if (ctrl != null)
            ctrl.Open(pauseMenu);
        else
            settingsPanel.SetActive(true);
    }

    public void HideSettingsPanel()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public bool IsSettingsPanelOpen()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }

    #endregion

    #region Game Over Screen

    public void ShowGameOverScreen()
    {
        if (gameOverScreen) gameOverScreen.SetActive(true);
        else Debug.LogWarning("[UIManager] GameOverScreen is not assigned!");
    }

    public void HideGameOverScreen()
    {
        if (gameOverScreen) gameOverScreen.SetActive(false);
    }

    #endregion

    #region Level Complete Screen

    public void ShowLevelCompleteScreen(int coinsCollected, int totalCoins, string nextLevelScene)
    {
        if (levelCompletionScreen)
        {
            levelCompletionScreen.SetActive(true);
            LevelCompleteScreenController controller = levelCompletionScreen.GetComponent<LevelCompleteScreenController>();
            if (controller != null)
                controller.Setup(coinsCollected, totalCoins, nextLevelScene);
        }
        else
        {
            Debug.LogWarning("[UIManager] LevelCompletionScreen is not assigned!");
        }
    }

    public void HideLevelCompleteScreen()
    {
        if (levelCompletionScreen) levelCompletionScreen.SetActive(false);
    }

    #endregion
}