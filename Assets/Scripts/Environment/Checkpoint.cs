using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool _activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger hit by: " + other.gameObject.name + " | Tag: " + other.gameObject.tag);

        if (!_activated && other.CompareTag("Player"))
        {
            Debug.Log("Checkpoint activated! GM: " + GameManager.Instance + " | Checkpoints: " + GameManager.Instance?.Checkpoints);

            _activated = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.Checkpoints.RegisterCheckpoint(transform.position);
            }
        }
    }
}