using UnityEngine;
using DG.Tweening; // DoTween kütüphanesi için gerekli
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public int coins = 100;
    public float fuel = 5.0f;
    public int level = 1;
}

public static class PlayerDataManager
{
    private static string savePath = Application.persistentDataPath + "/playerdata.json";
    private static PlayerData playerData;

    public static event System.Action<int> OnCoinsChanged;
    public static event System.Action<float> OnFuelChanged;

    static PlayerDataManager()
    {
        LoadData();
    }

    public static void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            ResetToDefault();
        }
    }

    public static void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, json);
    }
    public static void SetLevel(int newLevel)
    {
        playerData.level = newLevel;
        SaveData();
    }

    public static int GetLevel() => playerData.level;

    public static void ResetToDefault()
    {
        // 1. Oyuncu verilerini sıfırla
        playerData = new PlayerData();
        SaveData();
        OnCoinsChanged?.Invoke(playerData.coins);
        OnFuelChanged?.Invoke(playerData.fuel);

        // 2. Gezegen populasyonlarını sıfırla
        string planetSavePath = Application.persistentDataPath + "/saveData.json";

        if (File.Exists(planetSavePath))
        {
            string json = File.ReadAllText(planetSavePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            saveData.planetPopulations.Clear(); // Tüm gezegen populasyon verilerini temizle

            string newJson = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(planetSavePath, newJson);

            Debug.Log("🌍 Tüm gezegen popülasyon verileri sıfırlandı.");
        }
        else
        {
            Debug.Log("ℹ️ Gezegen verisi zaten yoktu (saveData.json bulunamadı).");
        }
    }


    public static void AddCoins(int amount)
    {
        playerData.coins += amount;
        OnCoinsChanged?.Invoke(playerData.coins);
        SaveData();
    }

    public static void SpendCoins(int amount)
    {
        if (playerData.coins >= amount)
        {
            playerData.coins -= amount;
            OnCoinsChanged?.Invoke(playerData.coins);
            SaveData();
        }
    }

    public static void AddFuel(float amount)
    {
        playerData.fuel += amount;
        OnFuelChanged?.Invoke(playerData.fuel);
        SaveData();
    }

    public static void UseFuel(float amount)
    {
        if (playerData.fuel >= amount)
        {
            playerData.fuel -= amount;
            OnFuelChanged?.Invoke(playerData.fuel);
            SaveData();
        }
    }
    public static void RemoveCoins(int amount)
    {
        if (playerData.coins >= amount)
        {
            playerData.coins -= amount;
            OnCoinsChanged?.Invoke(playerData.coins);
            SaveData();
        }
    }
    public static int GetCoins() => playerData.coins;
    public static float GetFuel() => playerData.fuel;
}
