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
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return _lastCheckpointPosition;
    }

    public void ResetCheckpoints()
    {
        _lastCheckpointPosition = _initialSpawnPosition;
    }
}