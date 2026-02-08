using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevelScene = "";
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        if (playerSpawnPoint != null && GameManager.Instance != null)
        {
            GameManager.Instance.Checkpoints.SetInitialSpawn(playerSpawnPoint.position);
            SpawnPlayer();

            // Fade in after player is positioned
            StartCoroutine(FadeInAfterSpawn());
        }
    }

    private void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerSpawnPoint.position;
        }
    }

    private System.Collections.IEnumerator FadeInAfterSpawn()
    {
        // Kurze Pause damit Player garantiert positioniert ist
        yield return null;

        // Fade from black to clear
        if (GameManager.Instance != null && GameManager.Instance.UI != null && GameManager.Instance.UI.Fader != null)
        {
            yield return GameManager.Instance.UI.Fader.FadeIn(0.5f);
        }
    }

    public void LevelComplete()
    {
        GameManager.Instance.LevelComplete();
        GameManager.Instance.LoadNextLevel(nextLevelScene);
    }
}