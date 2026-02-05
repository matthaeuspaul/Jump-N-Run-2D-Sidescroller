using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletionScreen;

    [Header("Search Settings")]
    [SerializeField] private bool autoFindUIElements = true;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (autoFindUIElements)
        {
            FindUIElements();
        }
        Initialize();
    }

    public void Initialize()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (levelCompletionScreen) levelCompletionScreen.SetActive(false);
    }

    private void FindUIElements()
    {
        // Find HUD elements
        if (livesText == null)
        {
            GameObject livesObj = GameObject.Find("LivesText");
            if (livesObj != null)
                livesText = livesObj.GetComponent<TextMeshProUGUI>();
        }

        if (coinsText == null)
        {
            GameObject coinsObj = GameObject.Find("CoinsText");
            if (coinsObj != null)
                coinsText = coinsObj.GetComponent<TextMeshProUGUI>();
        }

        // Find menu panels
        if (pauseMenu == null)
            pauseMenu = GameObject.Find("PauseMenu");

        if (gameOverScreen == null)
            gameOverScreen = GameObject.Find("GameOverScreen");

        if (levelCompletionScreen == null)
            levelCompletionScreen = GameObject.Find("LevelCompletionScreen");

        Debug.Log($"[UIManager] Found UI elements in scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"LivesText: {(livesText != null ? "Found" : "Missing")}");
        Debug.Log($"CoinsText: {(coinsText != null ? "Found" : "Missing")}");
        Debug.Log($"PauseMenu: {(pauseMenu != null ? "Found" : "Missing")}");
        Debug.Log($"GameOverScreen: {(gameOverScreen != null ? "Found" : "Missing")}");
        Debug.Log($"LevelCompletionScreen: {(levelCompletionScreen != null ? "Found" : "Missing")}");
    }

    #region HUD Updates

    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
        else
            Debug.LogWarning("[UIManager] LivesText is null!");
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
        else
            Debug.LogWarning("[UIManager] CoinsText is null!");
    }

    #endregion

    #region Pause Menu

    public void ShowPauseMenu()
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[UIManager] PauseMenu is null!");
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
        }
    }

    #endregion

    #region Game Over Screen

    public void ShowGameOverScreen()
    {
        if (gameOverScreen)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[UIManager] GameOverScreen is null!");
        }
    }

    #endregion

    #region Level Complete Screen

    public void ShowLevelCompleteScreen(int coinsCollected)
    {
        if (levelCompletionScreen)
        {
            levelCompletionScreen.SetActive(true);

            // Try to setup the level complete screen if it has the controller
            LevelCompleteScreenController controller = levelCompletionScreen.GetComponent<LevelCompleteScreenController>();
            if (controller != null)
            {
                // You can add total coins logic here if needed
                controller.Setup(coinsCollected, coinsCollected);
            }
        }
        else
        {
            Debug.LogWarning("[UIManager] LevelCompletionScreen is null!");
        }
    }

    #endregion

    /// <summary>
    /// Manually refresh UI element references (useful if UI is spawned dynamically)
    /// </summary>
    public void RefreshUIReferences()
    {
        FindUIElements();
        Initialize();
    }
}