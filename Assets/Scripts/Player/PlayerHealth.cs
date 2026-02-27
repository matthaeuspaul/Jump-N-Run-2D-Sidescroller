using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.1f;

    private AudioManager _audio;
    private SpriteRenderer _spriteRenderer;

    private bool _isInvincible = false;

    private void Awake()
    {
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

    public void TakeDamage()
    {
        if (_isInvincible) return;

        _audio?.PlaySFX("PlayerDeath");

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        else
            Debug.LogError("[PlayerHealth] GameManager.Instance is null!");
    }

    public void TakeDamage(int amount) => TakeDamage();

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
}