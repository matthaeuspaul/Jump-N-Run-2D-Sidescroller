using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    private UIManager _uiManager;
    private CheckpointManager _checkpointManager;

    public UIManager UI => _uiManager;
    public AudioManager Audio => AudioManager.Instance;
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
        _checkpointManager = GetComponentInChildren<CheckpointManager>();

        InitializeManagers();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Settings hat Priorität – zuerst Settings schließen, dann erst PauseMenu
            if (_uiManager != null && _uiManager.IsSettingsPanelOpen())
            {
                _uiManager.HideSettingsPanel();
                _uiManager.ShowPauseMenu();
                return;
            }

            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void InitializeManagers()
    {
        _uiManager?.Initialize();
        _checkpointManager?.Initialize();

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);
    }

    // -------------------------------------------------------
    // Save / Load State
    // -------------------------------------------------------

    public void ApplySaveData(SaveData data)
    {
        _currentLives = data.lives;
        _coinsCollected = data.coins;

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);

        Debug.Log($"[GameManager] Save data applied → Lives: {_currentLives} | Coins: {_coinsCollected}");
    }

    public SaveData BuildSaveData()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 cp = _checkpointManager != null
            ? _checkpointManager.GetLastCheckpointPosition()
            : Vector3.zero;

        return new SaveData(currentScene, _currentLives, _coinsCollected, cp.x, cp.y);
    }

    // -------------------------------------------------------
    // Game Flow
    // -------------------------------------------------------

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
            GameOver();
        else
            RespawnPlayer();
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
        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        Time.timeScale = 0f;
        _uiManager?.ShowGameOverScreen();
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private System.Collections.IEnumerator RestartGameCoroutine()
    {
        if (_uiManager != null && _uiManager.Fader != null)
            yield return _uiManager.Fader.FadeOut(0.5f);
        else
            yield return new WaitForSecondsRealtime(0.5f);

        _uiManager?.HideGameOverScreen();

        Time.timeScale = 1f;
        _currentLives = 3;
        _coinsCollected = 0;
        _checkpointManager?.ResetCheckpoints();

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);

        if (SaveManager.Instance != null)
            SaveManager.Instance.SetContinuing(false);

        SceneManager.LoadScene("Level_01");
    }

    public void LevelComplete()
    {
        if (SaveManager.Instance != null)
        {
            SaveData data = BuildSaveData();
            SaveManager.Instance.Save(data);
        }

        _uiManager?.ShowLevelCompleteScreen(_coinsCollected);
    }

    public void LoadNextLevel(string nextLevelScene)
    {
        StartCoroutine(LoadNextLevelCoroutine(nextLevelScene));
    }

    private System.Collections.IEnumerator LoadNextLevelCoroutine(string nextLevelScene)
    {
        if (_uiManager != null && _uiManager.Fader != null)
            yield return _uiManager.Fader.FadeOut(0.5f);
        else
            yield return new WaitForSecondsRealtime(0.5f);

        _uiManager?.HideLevelCompleteScreen();

        _coinsCollected = 0;
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);

        yield return new WaitForSecondsRealtime(0.3f);

        if (!string.IsNullOrEmpty(nextLevelScene))
        {
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.DeleteSave();

            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}