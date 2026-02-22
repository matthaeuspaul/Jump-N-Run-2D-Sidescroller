using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Detection")]
    [SerializeField] private float alertRange = 3f;

    [Header("Stomp")]
    [SerializeField] private float stompBounceForce = 8f;

    private enum State { Idle, Alert, Walk, Dead }
    private State currentState = State.Idle;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private float moveDirection = 1f;
    private bool alertTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Alert:
                HandleAlert();
                break;
            case State.Walk:
                HandleWalk();
                break;
        }
    }

    private void HandleIdle()
    {
        rb.linearVelocity = Vector2.zero;

        if (PlayerInRange())
        {
            currentState = State.Alert;
            animator.SetBool("isAlert", true);
        }
    }

    private void HandleAlert()
    {
        rb.linearVelocity = Vector2.zero;

        if (!alertTriggered)
        {
            alertTriggered = true;
            StartCoroutine(StartWalkAfterAlert());
        }
    }

    private System.Collections.IEnumerator StartWalkAfterAlert()
    {
        yield return new WaitForSeconds(GetAnimationLength("Enemy_01_Alert"));

        moveDirection = player.position.x > transform.position.x ? 1f : -1f;
        FlipSprite();

        animator.SetBool("isAlert", false);
        animator.SetBool("isWalking", true);
        currentState = State.Walk;
    }

    private void HandleWalk()
    {
        float distanceX = player.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > 0.1f)
        {
            moveDirection = distanceX > 0 ? 1f : -1f;
            FlipSprite();
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        bool isStompedFromAbove = collision.gameObject.transform.position.y > transform.position.y;

        if (isStompedFromAbove)
        {
            if (playerRb != null)
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
            Die();
        }
        else
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage();
        }
    }

    private bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= alertRange;
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
        {
            if (clip.name == clipName) return clip.length;
        }
        return 1f;
    }

    public void Die()
    {
        if (currentState == State.Dead) return;
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isDead");
        Destroy(gameObject, GetAnimationLength("Enemy_01_Death"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRange);
    }
}