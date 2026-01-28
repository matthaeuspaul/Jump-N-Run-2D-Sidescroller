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

    public void LevelComplete()
    {
        GameManager.Instance.LevelComplete();
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        if (!string.IsNullOrEmpty(nextLevelScene))
        {
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}