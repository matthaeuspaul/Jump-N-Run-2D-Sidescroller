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
    [SerializeField] private GameObject levelCompleteScreen;

    public void Initialize()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (levelCompleteScreen) levelCompleteScreen.SetActive(false);
    }

    public void UpdateLivesDisplay(int lives)
    {
        if (livesText) livesText.text = "Lives: " + lives;
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText) coinsText.text = "Coins: " + coins;
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen) gameOverScreen.SetActive(true);
    }

    public void ShowLevelCompleteScreen(int coinsCollected)
    {
        if (levelCompleteScreen) levelCompleteScreen.SetActive(true);
    }
}