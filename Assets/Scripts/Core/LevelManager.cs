using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevelScene = "";
    [SerializeField] private int totalCoins = 3;
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        if (GameManager.Instance == null) return;

        bool isContinuing = SaveManager.Instance != null && SaveManager.Instance.IsContinuing;

        if (isContinuing)
        {
            // Continue: Spawn-Position und Spielstand aus dem Save laden
            LoadFromSave();
        }
        else
        {
            // New Game / normaler Level-Start: Standard-Spawnpunkt nutzen
            if (playerSpawnPoint != null)
            {
                GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);
                SpawnPlayer(playerSpawnPoint.position);
            }
        }

        StartCoroutine(FadeInAfterSpawn());
    }

    private void LoadFromSave()
    {
        SaveData data = SaveManager.Instance.Load();

        if (data == null)
        {
            Debug.LogWarning("[LevelManager] Continue requested but no save data found. Using default spawn.");
            if (playerSpawnPoint != null)
            {
                GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);
                SpawnPlayer(playerSpawnPoint.position);
            }
            return;
        }

        // Lives & Coins wiederherstellen
        GameManager.Instance.ApplySaveData(data);

        // Checkpoint-Position wiederherstellen
        Vector3 savedPos = new Vector3(data.checkpointX, data.checkpointY, 0f);
        GameManager.Instance.Checkpoints.SetCheckpointFromSave(data.checkpointX, data.checkpointY);

        // Falls ein playerSpawnPoint existiert als Fallback für den InitialSpawn
        if (playerSpawnPoint != null)
            GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);

        SpawnPlayer(savedPos);

        // IsContinuing zurücksetzen damit beim nächsten Level-Start normal gespawnt wird
        SaveManager.Instance.SetContinuing(false);

        Debug.Log($"[LevelManager] Continued from save → Position: {savedPos}");
    }

    private void SpawnPlayer(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.position = position;
    }

    private IEnumerator FadeInAfterSpawn()
    {
        yield return null;

        if (GameManager.Instance != null && GameManager.Instance.UI != null && GameManager.Instance.UI.Fader != null)
            yield return GameManager.Instance.UI.Fader.FadeIn(0.5f);
    }

    public void LevelComplete()
    {
        GameManager.Instance.LevelComplete(nextLevelScene, totalCoins);
    }
}