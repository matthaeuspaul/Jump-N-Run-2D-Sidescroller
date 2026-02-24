using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }

    public void RestartLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.RestartGame();
        }
    }

    public void OpenSettings()
    {
        // UIManager kümmert sich um Panel-Wechsel (PauseMenu aus, Settings ein)
        if (GameManager.Instance != null)
            GameManager.Instance.UI?.ShowSettingsPanel();
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
    }
}