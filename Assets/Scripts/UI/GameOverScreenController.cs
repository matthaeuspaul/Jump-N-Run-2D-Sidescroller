using UnityEngine;

public class GameOverScreenController : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance?.RestartGame();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance?.ReturnToMainMenu();
    }
}