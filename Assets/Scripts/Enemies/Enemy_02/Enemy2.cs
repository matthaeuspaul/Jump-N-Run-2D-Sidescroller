using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Stomp")]
    [SerializeField] private float stompBounceForce = 8f;

    [Header("Raycasts")]
    [SerializeField] private float groundRayLength = 2f;
    [SerializeField] private LayerMask groundLayer;

    private enum State { Walk, Dead }
    private State currentState = State.Walk;

    private Rigidbody2D rb;
    private Animator animator;

    private float moveDirection = 1f;
    private float flipCooldown = 0.2f;
    private float lastFlipTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentState == State.Walk)
            HandleWalk();
    }

    private void HandleWalk()
    {
        if (NoGroundAhead() || WallAhead())
        {
            if (Time.time > lastFlipTime + flipCooldown)
            {
                moveDirection *= -1f;
                FlipSprite();
                lastFlipTime = Time.time;
            }
        }

        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    private bool NoGroundAhead()
    {
        Vector2 rayOrigin = new Vector2(
            transform.position.x + moveDirection * 0.5f,
            transform.position.y
        );
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundRayLength, groundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down * groundRayLength, hit.collider ? Color.green : Color.red);
        return hit.collider == null;
    }

    private bool WallAhead()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = new Vector2(moveDirection, 0f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 0.6f, groundLayer);
        Debug.DrawRay(rayOrigin, rayDirection * 0.6f, hit.collider ? Color.blue : Color.cyan);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dead) return;

        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            if (Time.time > lastFlipTime + flipCooldown)
            {
                moveDirection *= -1f;
                FlipSprite();
                lastFlipTime = Time.time;
            }
            return;
        }

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
        Destroy(gameObject, GetAnimationLength("Enemy_02_Death"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 rayOrigin = new Vector2(transform.position.x + moveDirection * 0.5f, transform.position.y);
        Gizmos.DrawRay(rayOrigin, Vector2.down * groundRayLength);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, new Vector2(moveDirection, 0f) * 0.6f);
    }
}