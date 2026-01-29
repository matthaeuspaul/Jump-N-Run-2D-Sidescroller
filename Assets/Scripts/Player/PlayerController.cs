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
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Input")]
    [SerializeField] private PlayerInputActions playerInputActions;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private float _moveInput;
    private float _coyoteTimeCounter;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Initialize Input Actions if not assigned
        if (playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();
        }
    }

    private void OnEnable()
    {
        // Enable input actions
        playerInputActions.Enable();

        // Subscribe to jump action
        playerInputActions.Player.Jump.performed += OnJumpPerformed;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerRespawn += OnRespawn;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from jump action
        playerInputActions.Player.Jump.performed -= OnJumpPerformed;

        // Disable input actions
        playerInputActions.Disable();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerRespawn -= OnRespawn;
        }
    }

    private void Update()
    {
        // Read move input
        _moveInput = playerInputActions.Player.Move.ReadValue<Vector2>().x;

        // Coyote Time Counter
        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Ground Check
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Movement
        _rb.linearVelocity = new Vector2(_moveInput * moveSpeed, _rb.linearVelocity.y);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Jump with coyote time
        if (_coyoteTimeCounter > 0f)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        _coyoteTimeCounter = 0f;
    }

    private void OnRespawn(Vector3 position)
    {
        transform.position = position;
        _rb.linearVelocity = Vector2.zero;
        _coyoteTimeCounter = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}