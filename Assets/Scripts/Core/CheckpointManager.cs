using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector3 _lastCheckpointPosition;
    private Vector3 _initialSpawnPosition;

    public void Initialize()
    {
        _initialSpawnPosition = Vector3.zero;
        _lastCheckpointPosition = _initialSpawnPosition;
    }

    public void SetInitialSpawn(Vector3 position)
    {
        _initialSpawnPosition = position;
        _lastCheckpointPosition = position;
    }

    public void RegisterCheckpoint(Vector3 position)
    {
        _lastCheckpointPosition = position;
        Debug.Log($"[CheckpointManager] Checkpoint registered at {position}");

        // Spielstand bei jedem Checkpoint autom. speichern
        if (SaveManager.Instance != null && GameManager.Instance != null)
        {
            SaveData data = GameManager.Instance.BuildSaveData();
            SaveManager.Instance.Save(data);
        }
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return _lastCheckpointPosition;
    }

    public void ResetCheckpoints()
    {
        _lastCheckpointPosition = _initialSpawnPosition;
    }

    /// <summary>
    /// Setzt die Checkpoint-Position direkt aus einem geladenen Save (für Continue).
    /// </summary>
    public void SetCheckpointFromSave(float x, float y)
    {
        _lastCheckpointPosition = new Vector3(x, y, 0f);
        Debug.Log($"[CheckpointManager] Checkpoint restored from save: {_lastCheckpointPosition}");
    }
}