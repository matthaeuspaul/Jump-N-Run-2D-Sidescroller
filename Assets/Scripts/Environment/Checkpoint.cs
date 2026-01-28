using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool _activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_activated && other.CompareTag("Player"))
        {
            _activated = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.Checkpoints.RegisterCheckpoint(transform.position);
            }
        }
    }
}