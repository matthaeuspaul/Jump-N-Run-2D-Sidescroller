using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance?.Checkpoints != null)
            GameManager.Instance.Checkpoints.SetInitialSpawn(transform.position);
        else
            Debug.LogWarning("[PlayerSpawnPoint] GameManager or CheckpointManager not found!");
    }
}