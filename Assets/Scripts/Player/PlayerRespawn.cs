using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerRespawn += HandleRespawn;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerRespawn -= HandleRespawn;
    }

    private void HandleRespawn(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;
    }
}