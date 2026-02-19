using UnityEngine;

// Dieses Script sitzt auf dem Chain_End GameObject.
// Es erkennt wenn der Spieler in Reichweite ist und informiert den SwingController.
[RequireComponent(typeof(Rigidbody2D))]
public class ChainEnd : MonoBehaviour
{
    // Der SwingController des Spielers der gerade in Reichweite ist
    private SwingController _playerInRange;

    // Input wird komplett über PlayerController.OnJump gehandelt
    // ChainEnd stellt nur die Referenz und AttachToChain bereit

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<SwingController>(out var swing))
        {
            _playerInRange = swing;
            _playerInRange.SetNearChain(true, this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<SwingController>(out _))
        {
            _playerInRange.SetNearChain(false);
            _playerInRange = null;
        }
    }
}