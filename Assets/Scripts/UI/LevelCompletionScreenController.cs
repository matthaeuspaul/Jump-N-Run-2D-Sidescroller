using UnityEngine;
using TMPro;

public class LevelCompleteScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinsText;

    public void Setup(int coinsCollected, int totalCoins)
    {
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {coinsCollected}/{totalCoins}";
        }
    }

    public void NextLevel()
    {
        // LevelManager handled das Level-Laden automatisch
        // Dieser Button ist optional, Level wechselt auch automatisch nach 2 Sekunden
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}