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
        Debug.Log("[PlayerHealth] Player took fatal damage!");

        if (_controller != null)
            _controller.enabled = false;

        if (_audio != null)
            _audio.PlaySFX("PlayerDeath");

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        else
            Debug.LogError("[PlayerHealth] GameManager.Instance is null! Cannot notify death.");

        Invoke(nameof(ReEnableController), 0.1f);
    }

    private void ReEnableController()
    {
        if (_controller != null)
            _controller.enabled = true;
    }

    private void OnRespawn(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        _spriteRenderer.enabled = true;
    }

    /// <summary>
    /// Optional: For future health system expansion
    /// </summary>
    public void TakeDamage(int amount)
    {
        TakeDamage();
    }
}