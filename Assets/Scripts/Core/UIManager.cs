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
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
    }

    #endregion

    #region Pause Menu

    public void ShowPauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
    }

    #endregion

    #region Game Over Screen

    public void ShowGameOverScreen()
    {
        if (gameOverScreen) gameOverScreen.SetActive(true);
    }

    #endregion

    #region Level Complete Screen

    public void ShowLevelCompleteScreen(int coinsCollected)
    {
        if (levelCompletionScreen)
        {
            levelCompletionScreen.SetActive(true);
        }
    }

    #endregion
}