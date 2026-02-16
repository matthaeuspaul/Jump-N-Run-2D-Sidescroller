using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingEnemyFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float smoothing = 5f; // makes movement softer

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackDuration = 0.25f;
    private bool _isKnockedBack = false;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f; // IMPORTANT: No gravity for flying enemy
        _rb.freezeRotation = true;

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_isKnockedBack || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius && distance > stopDistance)
        {
            FollowPlayer();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 targetVelocity = direction * moveSpeed;

        // Smooth movement
        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, smoothing * Time.fixedDeltaTime);

        // Flip sprite
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyKnockback(collision.transform.position);
        }
    }

    private void ApplyKnockback(Vector2 playerPosition)
    {
        Vector2 direction = ((Vector2)transform.position - playerPosition).normalized;
        _rb.linearVelocity = direction * knockbackForce;

        StartCoroutine(KnockbackCoroutine());
    }

    private System.Collections.IEnumerator KnockbackCoroutine()
    {
        _isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        _isKnockedBack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
