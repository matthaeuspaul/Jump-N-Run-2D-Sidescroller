using UnityEngine;
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

    [Header("Screen Transitions")]
    [SerializeField] private ScreenFade screenFader;

    public ScreenFade Fader => screenFader;

    private void Awake()
    {
        // Auto-find ScreenFader if not assigned
        if (screenFader == null)
        {
            screenFader = GetComponentInChildren<ScreenFade>();
        }
    }

    public void Initialize()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (levelCompletionScreen) levelCompletionScreen.SetActive(false);
    }

    #region HUD Updates

    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
        else
            Debug.LogWarning("[UIManager] LivesText is not assigned!");
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
        else
            Debug.LogWarning("[UIManager] CoinsText is not assigned!");
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
            Debug.LogWarning("[UIManager] PauseMenu is not assigned!");
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
            Debug.LogWarning("[UIManager] GameOverScreen is not assigned!");
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
                controller.Setup(coinsCollected, coinsCollected);
            }
        }
        else
        {
            Debug.LogWarning("[UIManager] LevelCompletionScreen is not assigned!");
        }
    }

    public void HideLevelCompleteScreen()
    {
        if (levelCompletionScreen)
        {
            levelCompletionScreen.SetActive(false);
        }
    }

    #endregion
}