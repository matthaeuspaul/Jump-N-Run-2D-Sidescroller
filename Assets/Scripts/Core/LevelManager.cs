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
            LoadFromSave();
        else if (playerSpawnPoint != null)
        {
            GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);
            SpawnPlayer(playerSpawnPoint.position);
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

        GameManager.Instance.ApplySaveData(data);

        Vector3 savedPos = new Vector3(data.checkpointX, data.checkpointY, 0f);
        GameManager.Instance.Checkpoints.SetCheckpointFromSave(data.checkpointX, data.checkpointY);

        if (playerSpawnPoint != null)
            GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);

        SpawnPlayer(savedPos);

        SaveManager.Instance.SetContinuing(false);
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

        if (GameManager.Instance?.UI?.Fader != null)
            yield return GameManager.Instance.UI.Fader.FadeIn(0.5f);
    }

    public void LevelComplete()
    {
        GameManager.Instance.LevelComplete(nextLevelScene, totalCoins);
    }
}