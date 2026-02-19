using UnityEngine;

// Dieses Script sitzt auf dem Player GameObject.
// Es verwaltet den DistanceJoint2D während des Schwingens.
[RequireComponent(typeof(Rigidbody2D))]
public class SwingController : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float swingForce = 5f;      // Kraft mit der der Spieler pumpen kann
    [SerializeField] private float releaseUpwardBoost = 2f; // Kleiner Upward-Boost beim Loslassen

    private Rigidbody2D _rb;
    private DistanceJoint2D _joint;
    private PlayerController _playerController;
    private ChainEnd _nearbyChain; // Referenz auf den ChainEnd in Reichweite

    public bool IsSwinging { get; private set; } = false;
    public bool IsNearChain { get; private set; } = false;

    public void SetNearChain(bool value, ChainEnd chain = null)
    {
        IsNearChain = value;
        _nearbyChain = value ? chain : null;
    }

    public void AttachToNearbyChain()
    {
        if (_nearbyChain != null)
            AttachToChain(_nearbyChain);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!IsSwinging) return;

        // Input wird komplett über PlayerController.OnJump gehandelt
    }

    private void FixedUpdate()
    {
        if (!IsSwinging) return;

        // Horizontale Kraft zum Schwingen aufbauen
        // (Input kommt vom PlayerController via MoveInput Property)
        float horizontalInput = _playerController.MoveInput.x;
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            _rb.AddForce(new Vector2(horizontalInput * swingForce, 0f), ForceMode2D.Force);
        }
    }

    public void AttachToChain(ChainEnd chainEnd)
    {
        if (IsSwinging) return;

        IsSwinging = true;

        // DistanceJoint2D dynamisch erstellen
        _joint = gameObject.AddComponent<DistanceJoint2D>();
        _joint.connectedBody = chainEnd.GetComponent<Rigidbody2D>();
        _joint.autoConfigureDistance = true;   // Distanz automatisch setzen
        _joint.maxDistanceOnly = false;         // Feste Länge, kein Gummiband
        _joint.enableCollision = false;

        Debug.Log("[SwingController] Attached to chain!");
    }

    public void DetachFromChain()
    {
        if (!IsSwinging) return;

        IsSwinging = false;

        // Kleinen Upward-Boost geben damit es sich besser anfühlt
        _rb.AddForce(Vector2.up * releaseUpwardBoost, ForceMode2D.Impulse);

        // Joint entfernen
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        Debug.Log("[SwingController] Detached from chain!");
    }
}