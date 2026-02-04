using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Damage Settings")]
    [SerializeField] private float invincibilityTime = 1f;
    private float _invincibilityTimer = 0f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool _isKnockedBack = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool hideOnDeath = true;

    [Header("Death Settings")]
    [SerializeField] private bool freezeGameOnDeath = true;
    [SerializeField] private float freezeDelay = 0.5f; // Delay before freezing (so you can see what happened)

    private Rigidbody2D _rb;
    private PlayerController _playerController;

    private void Awake()
    {
        currentHealth = maxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();

        // Auto-find sprite renderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        // Invincibility timer countdown
        if (_invincibilityTimer > 0f)
        {
            _invincibilityTimer -= Time.deltaTime;

            // Flicker effect during invincibility
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = (Time.time * 10) % 2 < 1;
            }
        }
        else
        {
            // Make sure sprite is visible when not invincible
            if (spriteRenderer != null && !spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Also check triggers (if enemy uses trigger collider)
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(collision.gameObject);
        }
    }

    public void TakeDamage(GameObject attacker = null)
    {
        // Can't take damage if invincible
        if (_invincibilityTimer > 0f) return;

        currentHealth--;
        _invincibilityTimer = invincibilityTime;

        Debug.Log($"Player took damage! Health: {currentHealth}/{maxHealth}");

        // Apply knockback
        if (attacker != null && _rb != null)
        {
            ApplyKnockback(attacker.transform.position);
        }

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector2 enemyPosition)
    {
        // Calculate knockback direction (away from enemy)
        Vector2 knockbackDirection = ((Vector2)transform.position - enemyPosition).normalized;

        // Apply knockback force
        _rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, knockbackForce * 0.5f);

        // Temporarily disable player control during knockback
        if (_playerController != null)
        {
            StartCoroutine(DisableControlTemporarily());
        }
    }

    private System.Collections.IEnumerator DisableControlTemporarily()
    {
        _isKnockedBack = true;
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (_playerController != null)
        {
            _playerController.enabled = true;
        }
        _isKnockedBack = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // IMPORTANT: Stop all physics movement first
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Static; // Make rigidbody static (stops gravity)
        }

        // Hide player sprite
        if (hideOnDeath && spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Disable player controls
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }

        // Disable collider so player doesn't interact with anything
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Freeze game after delay
        if (freezeGameOnDeath)
        {
            StartCoroutine(FreezeGameAfterDelay());
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    private System.Collections.IEnumerator FreezeGameAfterDelay()
    {
        // Wait a bit so player can see what happened
        yield return new WaitForSeconds(freezeDelay);

        // Freeze the game
        Time.timeScale = 0f;
        Debug.Log("Game Frozen!");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed! Health: {currentHealth}/{maxHealth}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        _invincibilityTimer = 0f;

        // Unfreeze game
        Time.timeScale = 1f;

        // Re-enable rigidbody physics
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic; // Make it dynamic again
            _rb.linearVelocity = Vector2.zero;
        }

        // Re-enable sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Re-enable controls
        if (_playerController != null)
        {
            _playerController.enabled = true;
        }

        // Re-enable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        Debug.Log("Game Unfrozen!");
    }
}