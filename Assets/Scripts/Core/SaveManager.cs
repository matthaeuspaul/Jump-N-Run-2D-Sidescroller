using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string _savePath;

    // Wird von LevelManager geprüft um zu entscheiden ob Continue-Daten geladen werden sollen
    public bool IsContinuing { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        Debug.Log($"[SaveManager] Save path: {_savePath}");
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"[SaveManager] Game saved → Scene: {data.sceneName} | Lives: {data.lives} | Coins: {data.coins}");
    }

    public SaveData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] No save file found!");
            return null;
        }

        string json = File.ReadAllText(_savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"[SaveManager] Game loaded → Scene: {data.sceneName} | Lives: {data.lives} | Coins: {data.coins}");
        return data;
    }

    public bool HasSave()
    {
        return File.Exists(_savePath);
    }

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(_savePath);
            Debug.Log("[SaveManager] Save file deleted.");
        }
        IsContinuing = false;
    }

    /// <summary>
    /// Wird vom MainMenu aufgerufen bevor die Scene geladen wird.
    /// LevelManager liest dieses Flag beim Start aus.
    /// </summary>
    public void SetContinuing(bool value)
    {
        IsContinuing = value;
    }
}