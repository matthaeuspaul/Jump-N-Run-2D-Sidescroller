using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Jump Cooldown")]
    [SerializeField] private float jumpCooldown = 0.5f; // Time to wait between jumps

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private Vector2 _moveInput;
    private float _coyoteTimeCounter;
    private float _jumpCooldownTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Auto-create ground check if missing
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    private void Update()
    {
        // Coyote Time Counter
        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Cooldown Timer
        if (_jumpCooldownTimer > 0f)
        {
            _jumpCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Ground Check - check multiple points for better detection
        Vector2 leftPoint = new Vector2(groundCheck.position.x - 0.2f, groundCheck.position.y);
        Vector2 centerPoint = groundCheck.position;
        Vector2 rightPoint = new Vector2(groundCheck.position.x + 0.2f, groundCheck.position.y);

        bool leftGrounded = Physics2D.OverlapCircle(leftPoint, groundCheckRadius, groundLayer);
        bool centerGrounded = Physics2D.OverlapCircle(centerPoint, groundCheckRadius, groundLayer);
        bool rightGrounded = Physics2D.OverlapCircle(rightPoint, groundCheckRadius, groundLayer);

        _isGrounded = leftGrounded || centerGrounded || rightGrounded;

        // Movement
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        // Check if can jump: must have coyote time AND cooldown must be finished
        if (value.isPressed && _coyoteTimeCounter > 0f && _jumpCooldownTimer <= 0f)
        {
            Jump();
        }
        else if (value.isPressed && _jumpCooldownTimer > 0f)
        {
            Debug.Log($"Jump on cooldown! Wait {_jumpCooldownTimer:F1}s");
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        _coyoteTimeCounter = 0f;
        _jumpCooldownTimer = jumpCooldown; // Start cooldown

        Debug.Log("Jumped! Cooldown started.");
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        _rb.linearVelocity = Vector2.zero;
        _coyoteTimeCounter = 0f;
        _jumpCooldownTimer = 0f; // Reset cooldown on respawn
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // Draw 3 check points
            Vector3 leftPoint = groundCheck.position + Vector3.left * 0.2f;
            Vector3 centerPoint = groundCheck.position;
            Vector3 rightPoint = groundCheck.position + Vector3.right * 0.2f;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(leftPoint, groundCheckRadius);
            Gizmos.DrawWireSphere(centerPoint, groundCheckRadius);
            Gizmos.DrawWireSphere(rightPoint, groundCheckRadius);

            // Draw line to ground check
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, groundCheck.position);
        }
    }
}