using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.3f;
    private bool _isKnockedBack = false;

    [Header("Patrol (Optional)")]
    [SerializeField] private bool patrolWhenIdle = false;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolDistance = 3f;

    private Rigidbody2D _rb;
    private Vector2 _startPosition;
    private float _patrolDirection = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        _startPosition = transform.position;

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        // Don't move if knocked back
        if (_isKnockedBack) return;

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within detection radius
        if (distanceToPlayer <= detectionRadius && distanceToPlayer > stopDistance)
        {
            // Follow player
            FollowPlayer();
        }
        else if (distanceToPlayer <= stopDistance)
        {
            // Stop moving (too close)
            _rb.linearVelocity = Vector2.zero;
        }
        else if (patrolWhenIdle)
        {
            // Patrol when player is out of range
            Patrol();
        }
        else
        {
            // Idle (stop moving)
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enemy gets knocked back when hitting player
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyKnockback(collision.transform.position);
        }
    }

    private void ApplyKnockback(Vector2 playerPosition)
    {
        // Calculate knockback direction (away from player)
        Vector2 knockbackDirection = ((Vector2)transform.position - playerPosition).normalized;

        // Apply knockback force
        _rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, knockbackForce * 0.3f);

        // Stop enemy AI temporarily
        StartCoroutine(KnockbackCoroutine());
    }

    private System.Collections.IEnumerator KnockbackCoroutine()
    {
        _isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        _isKnockedBack = false;
    }

    private void FollowPlayer()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move toward player
        _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);

        // Optional: Flip sprite to face player
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Patrol()
    {
        // Simple left-right patrol
        _rb.linearVelocity = new Vector2(_patrolDirection * patrolSpeed, _rb.linearVelocity.y);

        // Check if reached patrol limit
        float distanceFromStart = transform.position.x - _startPosition.x;
        if (Mathf.Abs(distanceFromStart) >= patrolDistance)
        {
            _patrolDirection *= -1; // Reverse direction
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1); // Flip sprite
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw stop distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Draw patrol range if enabled
        if (patrolWhenIdle)
        {
            Gizmos.color = Color.blue;
            Vector3 startPos = Application.isPlaying ? _startPosition : transform.position;
            Gizmos.DrawLine(startPos + Vector3.left * patrolDistance,
                           startPos + Vector3.right * patrolDistance);
        }
    }
}