using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveSystem
{
    public static void SavePlayer(GameObject player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "D:/licenta/what_lies_within_the_tower/Assets/WhatLiesWithinTheTower/SavedProgress/player.miau";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = "D:/licenta/what_lies_within_the_tower/Assets/WhatLiesWithinTheTower/SavedProgress/player.miau";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
