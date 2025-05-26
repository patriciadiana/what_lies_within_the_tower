using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static readonly string saveFolder = Path.Combine(Application.persistentDataPath, "SavedProgress");
    private static readonly string savePath = Path.Combine(saveFolder, "player.data");

    public static void SavePlayer(GameObject player)
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            PlayerData data = new PlayerData(player);
            formatter.Serialize(stream, data);
        }

        Debug.Log("Saved player to: " + savePath);
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Open))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                Debug.Log("Loaded player from: " + savePath);
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file not found at: " + savePath);
            return null;
        }
    }
}
