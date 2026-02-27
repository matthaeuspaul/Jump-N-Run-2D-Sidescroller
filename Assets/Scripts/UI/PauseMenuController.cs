using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance?.ResumeGame();
    }

    public void RestartLevel()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.ResumeGame();
        GameManager.Instance.RestartGame();
    }

    public void OpenSettings()
    {
        GameManager.Instance?.UI?.ShowSettingsPanel();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance?.ReturnToMainMenu();
    }
}