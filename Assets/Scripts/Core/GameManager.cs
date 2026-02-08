using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    private UIManager _uiManager;
    private AudioManager _audioManager;
    private CheckpointManager _checkpointManager;

    public UIManager UI => _uiManager;
    public AudioManager Audio => _audioManager;
    public CheckpointManager Checkpoints => _checkpointManager;

    [Header("Game State")]
    private int _currentLives = 3;
    private int _coinsCollected = 0;
    private bool _isPaused = false;

    public int CurrentLives => _currentLives;
    public int CoinsCollected => _coinsCollected;
    public bool IsPaused => _isPaused;

    public System.Action<Vector3> OnPlayerRespawn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _uiManager = GetComponentInChildren<UIManager>();
        _audioManager = GetComponentInChildren<AudioManager>();
        _checkpointManager = GetComponentInChildren<CheckpointManager>();

        InitializeManagers();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void InitializeManagers()
    {
        _uiManager?.Initialize();
        _audioManager?.Initialize();
        _checkpointManager?.Initialize();

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        _uiManager?.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _uiManager?.HidePauseMenu();
    }

    public void PlayerDied()
    {
        _currentLives--;
        _uiManager?.UpdateLivesDisplay(_currentLives);

        if (_currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        Vector3 respawnPos = _checkpointManager.GetLastCheckpointPosition();
        OnPlayerRespawn?.Invoke(respawnPos);
    }

    public void AddLife()
    {
        _currentLives++;
        _uiManager?.UpdateLivesDisplay(_currentLives);
    }

    public void CollectCoin()
    {
        _coinsCollected++;
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        _uiManager?.ShowGameOverScreen();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        _currentLives = 3;
        _coinsCollected = 0;
        _checkpointManager?.ResetCheckpoints();

        SceneManager.LoadScene("Level_01");
    }

    public void LevelComplete()
    {
        _uiManager?.ShowLevelCompleteScreen(_coinsCollected);
    }

    public void LoadNextLevel(string nextLevelScene)
    {
        StartCoroutine(LoadNextLevelCoroutine(nextLevelScene));
    }

    private System.Collections.IEnumerator LoadNextLevelCoroutine(string nextLevelScene)
    {
        // Fade to black
        if (_uiManager != null && _uiManager.Fader != null)
        {
            yield return _uiManager.Fader.FadeOut(0.5f);
        }
        else
        {
            // Fallback: wait without fade
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Verstecke Level Completion Screen
        _uiManager?.HideLevelCompleteScreen();

        // Kurze Pause auf schwarzem Bildschirm
        yield return new WaitForSecondsRealtime(0.3f);

        // Lade nächstes Level oder Main Menu
        if (!string.IsNullOrEmpty(nextLevelScene))
        {
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Note: Fade-In passiert im LevelManager nach dem Player-Spawn
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}