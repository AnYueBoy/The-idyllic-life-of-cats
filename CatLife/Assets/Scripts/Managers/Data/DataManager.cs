using System.IO;
using LitJson;
using UnityEngine;

public class DataManager : IManager
{
    private PlayerData playerData;

    public void Init()
    {
        DeserializeData();
    }

    private readonly float saveInterval = 5f;
    private float saveTimer;

    public void LocalUpdate(float dt)
    {
        CheckPlayerData(dt);
    }

    private void CheckPlayerData(float dt)
    {
        saveTimer += dt;
        if (saveTimer < saveInterval)
        {
            return;
        }

        saveTimer = 0;
        SerializeData();
    }

    private void SerializeData()
    {
        if (playerData == null)
        {
            return;
        }

        string dataDirPath = Directory.GetParent(Application.dataPath) + "/PersistenceData/";
        if (!Directory.Exists(dataDirPath))
        {
            Directory.CreateDirectory(dataDirPath);
        }

        string dataJson = JsonMapper.ToJson(playerData);
        string dataFilePath = dataDirPath + "PersistenceData.json";
        File.WriteAllText(dataFilePath, dataJson);
    }

    private void DeserializeData()
    {
        string dataFilePath = Directory.GetParent(Application.dataPath) + "/PersistenceData/PersistenceData.json";
        if (!File.Exists(dataFilePath))
        {
            playerData = new PlayerData();
            return;
        }

        string dataJson = File.ReadAllText(dataFilePath);
        playerData = JsonMapper.ToObject<PlayerData>(dataJson);
    }
}