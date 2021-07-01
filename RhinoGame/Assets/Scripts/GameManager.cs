using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData playerData;
    public string FilePath;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                playerData = new PlayerData();
                SavePlayerData();
            }

            var fileContents = File.ReadAllText(FilePath);
            playerData = JsonConvert.DeserializeObject<PlayerData>(fileContents);
            
        }
        catch (System.Exception e)
        {
            Debug.Log("Error has occured whilst reading data: " + e.Message);
        }
    }

    public void SavePlayerData()
    {
        try
        {
            var serializedData = JsonConvert.SerializeObject(playerData);
            File.WriteAllText(FilePath, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error has occured whilst saving data: " + e.Message);
        }
        
    }
}
