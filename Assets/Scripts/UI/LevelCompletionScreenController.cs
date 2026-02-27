using UnityEngine;
using TMPro;

public class LevelCompleteScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinsText;

    private string _nextLevelScene;

    public void Setup(int coinsCollected, int totalCoins, string nextLevelScene)
    {
        _nextLevelScene = nextLevelScene;

        if (coinsText != null)
        {
            coinsText.text = $"Coins: {coinsCollected}/{totalCoins}";
        }
    }

    public void NextLevel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadNextLevel(_nextLevelScene);
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
    }
}