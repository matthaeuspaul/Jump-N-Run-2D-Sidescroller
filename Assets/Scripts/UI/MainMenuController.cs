using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References (Optional)")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string firstLevelScene = "Level_01";

    private void Start()
    {
        // Auto-select first button for controller/keyboard navigation
        if (startButton != null)
        {
            startButton.Select();
        }

        // Ensure time scale is reset (in case coming from paused game)
        Time.timeScale = 1f;

        // Optional: Clear any existing GameManager instance
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(firstLevelScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Optional: Add options menu
    public void OpenOptions()
    {
        // Implement options menu
        Debug.Log("Options menu not yet implemented");
    }

    // Optional: Add credits
    public void OpenCredits()
    {
        // Implement credits screen
        Debug.Log("Credits not yet implemented");
    }
}