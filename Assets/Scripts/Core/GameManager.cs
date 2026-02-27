using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private UIManager _uiManager;
    private CheckpointManager _checkpointManager;

    public UIManager UI => _uiManager;
    public AudioManager Audio => AudioManager.Instance;
    public CheckpointManager Checkpoints => _checkpointManager;

    [Header("Game State")]
    [SerializeField] private int _maxLives = 3;
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

    public void ApplySaveData(SaveData data)
    {
        _currentLives = data.lives;
        _coinsCollected = data.coins;

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);
    }

    public SaveData BuildSaveData()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 cp = _checkpointManager != null
            ? _checkpointManager.GetLastCheckpointPosition()
            : Vector3.zero;

        return new SaveData(currentScene, _currentLives, _coinsCollected, cp.x, cp.y);
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
        if (_currentLives >= _maxLives) return;

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
        SaveManager.Instance?.DeleteSave();
        Time.timeScale = 0f;
        _uiManager?.ShowGameOverScreen();
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private System.Collections.IEnumerator RestartGameCoroutine()
    {
        if (_uiManager?.Fader != null)
            yield return _uiManager.Fader.FadeOut(0.5f);
        else
            yield return new WaitForSecondsRealtime(0.5f);

        _uiManager?.HideGameOverScreen();

        Time.timeScale = 1f;
        _currentLives = _maxLives;
        _coinsCollected = 0;
        _checkpointManager?.ResetCheckpoints();

        _uiManager?.UpdateLivesDisplay(_currentLives);
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);

        SaveManager.Instance?.SetContinuing(false);

        SceneManager.LoadScene("Level_01");
    }

    public void LevelComplete(string nextLevelScene, int totalCoins)
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Save(BuildSaveData());

        Time.timeScale = 0f;
        _uiManager?.ShowLevelCompleteScreen(_coinsCollected, totalCoins, nextLevelScene);
    }

    public void LoadNextLevel(string nextLevelScene)
    {
        StartCoroutine(LoadNextLevelCoroutine(nextLevelScene));
    }

    private System.Collections.IEnumerator LoadNextLevelCoroutine(string nextLevelScene)
    {
        if (_uiManager?.Fader != null)
            yield return _uiManager.Fader.FadeOut(0.5f);
        else
            yield return new WaitForSecondsRealtime(0.5f);

        _uiManager?.HideLevelCompleteScreen();

        Time.timeScale = 1f;
        _coinsCollected = 0;
        _uiManager?.UpdateCoinsDisplay(_coinsCollected);

        yield return new WaitForSecondsRealtime(0.3f);

        if (!string.IsNullOrEmpty(nextLevelScene))
        {
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            SaveManager.Instance?.DeleteSave();
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