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

    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float stompBounceForce = 10f;

    [Header("Health Bar")]
    [SerializeField] private EnemyHealthBar healthBarPrefab;


    private int _currentHealth;
    private Rigidbody2D _rb;
    private EnemyHealthBar _healthBarInstance;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        _currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            _healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);

            _healthBarInstance.Initialize(transform);
            _healthBarInstance.SetHealth(_currentHealth, maxHealth);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius && distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);

            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        bool isFalling = playerRb.linearVelocity.y < 0;
        bool isAbove = collision.transform.position.y > transform.position.y + 0.3f;

        if (isFalling && isAbove)
        {
            TakeDamage(1);
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
        }
    }

    private void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_healthBarInstance != null)
            _healthBarInstance.SetHealth(_currentHealth, maxHealth);

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (_healthBarInstance != null)
            Destroy(_healthBarInstance.gameObject);

        Destroy(gameObject);
    }
}
