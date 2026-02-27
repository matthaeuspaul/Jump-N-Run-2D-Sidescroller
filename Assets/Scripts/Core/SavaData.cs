using System;

[Serializable]
public class SaveData
{
    public string sceneName;
    public int lives;
    public int coins;
    public float checkpointX;
    public float checkpointY;
    public string saveTime;

    public SaveData() { }

    public SaveData(string sceneName, int lives, int coins, float checkpointX, float checkpointY)
    {
        this.sceneName = sceneName;
        this.lives = lives;
        this.coins = coins;
        this.checkpointX = checkpointX;
        this.checkpointY = checkpointY;
        this.saveTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }
}