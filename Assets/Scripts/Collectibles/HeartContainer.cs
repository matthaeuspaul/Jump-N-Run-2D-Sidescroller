using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddLife();
            }

            Destroy(gameObject);
        }
    }
}