using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerHealth))]
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
    [SerializeField] private float jumpCooldown = 0.5f;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SwingController _swingController; // NEU
    private bool _isGrounded;
    private Vector2 _moveInput;
    private float _coyoteTimeCounter;
    private float _jumpCooldownTimer;

    // NEU: MoveInput als Property damit SwingController es lesen kann
    public Vector2 MoveInput => _moveInput;

    // Animation parameter names
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int VelocityY = Animator.StringToHash("velocityY");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _swingController = GetComponent<SwingController>(); // NEU

        // Auto-get SpriteRenderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Auto-create ground check if missing
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerRespawn += Respawn;
            Debug.Log("[PlayerController] Subscribed to GameManager.OnPlayerRespawn");
        }
        else
        {
            Debug.LogError("[PlayerController] GameManager.Instance is null! This should not happen!");
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerRespawn -= Respawn;
            Debug.Log("[PlayerController] Unsubscribed from GameManager.OnPlayerRespawn");
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

        UpdateAnimations();
        HandleSpriteFlip();
    }

    private void FixedUpdate()
    {
        // Ground Check
        Vector2 leftPoint = new Vector2(groundCheck.position.x - 0.2f, groundCheck.position.y);
        Vector2 centerPoint = groundCheck.position;
        Vector2 rightPoint = new Vector2(groundCheck.position.x + 0.2f, groundCheck.position.y);

        bool leftGrounded = Physics2D.OverlapCircle(leftPoint, groundCheckRadius, groundLayer);
        bool centerGrounded = Physics2D.OverlapCircle(centerPoint, groundCheckRadius, groundLayer);
        bool rightGrounded = Physics2D.OverlapCircle(rightPoint, groundCheckRadius, groundLayer);

        _isGrounded = leftGrounded || centerGrounded || rightGrounded;

        // NEU: Normales Movement nur wenn NICHT am Schwingen
        // Sonst kämpft die Velocity gegen den DistanceJoint
        if (_swingController == null || !_swingController.IsSwinging)
        {
            _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
        }
    }

    private void UpdateAnimations()
    {
        _animator.SetBool(IsGrounded, _isGrounded);
        _animator.SetBool(IsRunning, Mathf.Abs(_moveInput.x) > 0.01f);
        _animator.SetFloat(VelocityY, _rb.linearVelocity.y);
    }

    private void HandleSpriteFlip()
    {
        if (_moveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (_moveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        // Schwingt gerade → loslassen
        if (_swingController != null && _swingController.IsSwinging)
        {
            _swingController.DetachFromChain();
            return;
        }

        // In Reichweite einer Kette → greifen
        if (_swingController != null && _swingController.IsNearChain)
        {
            _swingController.AttachToNearbyChain();
            return;
        }

        // Normal springen
        if (_coyoteTimeCounter > 0f && _jumpCooldownTimer <= 0f)
        {
            Jump();
        }
        else if (_jumpCooldownTimer > 0f)
        {
            Debug.Log($"Jump on cooldown! Wait {_jumpCooldownTimer:F1}s");
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        _coyoteTimeCounter = 0f;
        _jumpCooldownTimer = jumpCooldown;

        if (GameManager.Instance != null && GameManager.Instance.Audio != null)
        {
            GameManager.Instance.Audio.PlaySFX("Jump");
        }

        Debug.Log("Jumped! Cooldown started.");
    }

    public void Respawn(Vector3 position)
    {
        Debug.Log($"[PlayerController] Respawning at {position}");

        transform.position = position;
        _rb.linearVelocity = Vector2.zero;
        _coyoteTimeCounter = 0f;
        _jumpCooldownTimer = 0f;

        // NEU: Beim Respawn von Kette lösen falls nötig
        if (_swingController != null && _swingController.IsSwinging)
        {
            _swingController.DetachFromChain();
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Vector3 leftPoint = groundCheck.position + Vector3.left * 0.2f;
            Vector3 centerPoint = groundCheck.position;
            Vector3 rightPoint = groundCheck.position + Vector3.right * 0.2f;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(leftPoint, groundCheckRadius);
            Gizmos.DrawWireSphere(centerPoint, groundCheckRadius);
            Gizmos.DrawWireSphere(rightPoint, groundCheckRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, groundCheck.position);
        }
    }
}