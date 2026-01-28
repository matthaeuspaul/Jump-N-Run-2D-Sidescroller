using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = Object.FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _levelManager?.LevelComplete();
        }
    }
}