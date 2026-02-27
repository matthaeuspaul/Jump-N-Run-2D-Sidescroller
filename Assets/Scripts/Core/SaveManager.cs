using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string _savePath;

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
    }

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(_savePath, json);
    }

    public SaveData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] No save file found!");
            return null;
        }

        string json = File.ReadAllText(_savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public bool HasSave()
    {
        return File.Exists(_savePath);
    }

    public void DeleteSave()
    {
        if (HasSave())
            File.Delete(_savePath);

        IsContinuing = false;
    }

    public void SetContinuing(bool value)
    {
        IsContinuing = value;
    }
}