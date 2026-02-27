using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.1f;

    private AudioManager _audio;
    private PlayerController _controller;
    private SpriteRenderer _spriteRenderer;

    private bool _isInvincible = false;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            _audio = GameManager.Instance.Audio;
            GameManager.Instance.OnPlayerRespawn += OnRespawn;
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] GameManager.Instance is null at Start!");
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerRespawn -= OnRespawn;
    }

    /// <summary>
    /// Called when player takes fatal damage (from Hazard.cs, enemies, etc.)
    /// </summary>
    public void TakeDamage()
    {
        if (_isInvincible)
        {
            Debug.Log("[PlayerHealth] Damage ignored – player is invincible.");
            return;
        }

        Debug.Log("[PlayerHealth] Player took fatal damage!");

        if (_audio != null)
            _audio.PlaySFX("PlayerDeath");

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        else
            Debug.LogError("[PlayerHealth] GameManager.Instance is null! Cannot notify death.");
    }

    private void OnRespawn(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        _isInvincible = true;
        float timer = 0f;

        while (timer < blinkDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        _spriteRenderer.enabled = true;
        _isInvincible = false;
    }

    /// <summary>
    /// Optional: For future health system expansion
    /// </summary>
    public void TakeDamage(int amount)
    {
        TakeDamage();
    }
}