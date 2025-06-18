using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    /*──────────── Dosya Yol / Dirty Bayrağı ───────────*/
    private const string FileName = "saveData.json";
    private static string PathFile => Path.Combine(Application.persistentDataPath, FileName);

    /// <summary>Menü sahnesi Update’inde bakmak için</summary>
    public static bool DataDirty { get; private set; }
    public static void ClearDirty() => DataDirty = false;

    /*──────────── Genel Yükle / Kaydet ───────────*/
    public static SaveData Load()
    {
        if (File.Exists(PathFile))
        {
            string json = File.ReadAllText(PathFile);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();  // hiç dosya yoksa boş model
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathFile, json);
        DataDirty = true;        // her kayıt → verinin güncellendiği sinyali
    }

    /*──────────── N Ü F U S ───────────*/
    public static int GetPopulation(string planet, int fallback = 0)
    {
        var data  = Load();
        var entry = data.planetPopulations.Find(p => p.planetName == planet);
        return entry != null ? entry.population : fallback;
    }

    public static void SetPopulation(string planet, int newValue)
    {
        var data  = Load();
        var entry = data.planetPopulations.Find(p => p.planetName == planet);
        if (entry == null)
        {
            entry = new PlanetPopulation { planetName = planet };
            data.planetPopulations.Add(entry);
        }
        entry.population = newValue;
        Save(data);
    }

    /*──────────── E V R İ M ───────────*/
    public static int GetEvolutionStage(string planet)
    {
        var data  = Load();
        var entry = data.evolutions.Find(e => e.planetName == planet);
        return entry != null ? entry.stage : 0;
    }

    public static void SetEvolutionStage(string planet, int stage)
    {
        var data  = Load();
        var entry = data.evolutions.Find(e => e.planetName == planet);
        if (entry == null)
        {
            entry = new PlanetEvolution { planetName = planet };
            data.evolutions.Add(entry);
        }
        entry.stage = stage;
        Save(data);
    }

    /*──────────── D E K O R A S Y O N ───────────*/
    public static bool IsObjectActive(string obj)   => Load().activeObjects.Contains(obj);
    public static bool IsItemRemoved (string item)  => Load().removedItems.Contains(item);

    public static void AddActiveObject(string obj)
    {
        var data = Load();
        if (!data.activeObjects.Contains(obj))
        {
            data.activeObjects.Add(obj);
            Save(data);
        }
    }

    public static void AddRemovedItem(string item)
    {
        var data = Load();
        if (!data.removedItems.Contains(item))
        {
            data.removedItems.Add(item);
            Save(data);
        }
    }
}