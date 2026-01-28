using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void RestartLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.RestartGame();
        }
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}