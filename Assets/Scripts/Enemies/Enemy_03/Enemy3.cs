using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float sinAmplitude = 0.5f;
    [SerializeField] private float sinFrequency = 2f;

    [Header("Stomp")]
    [SerializeField] private float stompBounceForce = 8f;

    private enum State { Fly, Dead }
    private State currentState = State.Fly;

    private Rigidbody2D rb;
    private Animator animator;

    private float moveDirection = 1f;
    private Vector3 startPosition;
    private float sinTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (currentState == State.Fly)
            HandleFly();
    }

    private void HandleFly()
    {
        sinTimer += Time.deltaTime;

        transform.position += new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0);

        float sinY = Mathf.Sin(sinTimer * sinFrequency) * sinAmplitude;
        transform.position = new Vector3(transform.position.x, startPosition.y + sinY, transform.position.z);

        float distanceFromStart = transform.position.x - startPosition.x;
        if (Mathf.Abs(distanceFromStart) >= patrolDistance)
        {
            moveDirection *= -1f;
            FlipSprite();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        bool isStompedFromAbove = collision.contacts[0].normal.y < -0.5f;

        if (isStompedFromAbove)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
            Die();
        }
        else
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage();
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = moveDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private float GetAnimationLength(string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 1f;
    }

    public void Die()
    {
        if (currentState == State.Dead) return;
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isDead");
        Destroy(gameObject, GetAnimationLength("Enemy_03_Death"));
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(startPosition.x - patrolDistance, startPosition.y, 0),
            new Vector3(startPosition.x + patrolDistance, startPosition.y, 0)
        );
    }
}