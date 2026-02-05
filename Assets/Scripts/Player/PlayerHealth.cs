using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private AudioManager _audio;
    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            _audio = GameManager.Instance.Audio;
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] GameManager.Instance is null at Start!");
        }
    }

    /// <summary>
    /// Called when player takes fatal damage (from Hazard.cs, enemies, etc.)
    /// </summary>
    public void TakeDamage()
    {
        Debug.Log("[PlayerHealth] Player took fatal damage!");

        // Disable player input temporarily to prevent movement during death
        if (_controller != null)
        {
            _controller.enabled = false;
        }

        // Play death sound
        if (_audio != null)
        {
            _audio.PlaySFX("PlayerDeath");
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            Debug.LogError("[PlayerHealth] GameManager.Instance is null! Cannot notify death.");
        }

        // Re-enable controller after a short delay (will be disabled again on respawn if needed)
        Invoke(nameof(ReEnableController), 0.1f);
    }

    private void ReEnableController()
    {
        if (_controller != null)
        {
            _controller.enabled = true;
        }
    }

    /// <summary>
    /// Optional: For future health system expansion
    /// </summary>
    public void TakeDamage(int amount)
    {
        // For now, any damage is instant death
        // Later you can add: currentHealth -= amount; if (currentHealth <= 0) Die();
        TakeDamage();
    }
}