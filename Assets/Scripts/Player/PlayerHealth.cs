using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void TakeDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }
}