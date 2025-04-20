using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }
    public CelestialBodyData celestialBodyData; // Artık prefablar buradan alınacak
    private LevelDatabase levelDatabase;
    public ClickablePlanetDatabase planetDatabase;
    private string saveFilePath;
    public Transform spawnParent;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/saveData.json";
        LoadLevelData();
        int currentLevel = PlayerDataManager.GetLevel();
        LoadLevel(currentLevel);

        // 🌟 Oyunun başında gezegenlerin mevcut nüfusunu konsola yazdır
        foreach (var planet in planetDatabase.planets)
        {
            int currentPopulation = LoadPlanetPopulation(planet.planetName, planet.defaultPopulation);
            planet.currentPopulation = currentPopulation;
            Debug.Log($"🟢 {planet.planetName} mevcut nüfus: {currentPopulation}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            string currentPlanetName = GetCurrentPlanetName();
            if (!string.IsNullOrEmpty(currentPlanetName))
            {
                IncreasePlanetPopulation(currentPlanetName, 10);
            }
        }
    }

    string GetCurrentPlanetName()
    {
        int currentLevel = PlayerDataManager.GetLevel();
        LevelData levelData = levelDatabase.levels.Find(l => l.level == currentLevel);
        return levelData?.targetPlanet;
    }

    void LoadLevelData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("levelData");
        if (jsonFile != null)
        {
            levelDatabase = JsonUtility.FromJson<LevelDatabase>(jsonFile.text);

        }
    }

    void LoadLevel(int level)
    {
        LevelData levelData = levelDatabase.levels.Find(l => l.level == level);

        if (levelData != null)
        {
            foreach (PlanetDataLevel planet in levelData.planets)
            {
                GameObject prefab = FindPlanetPrefab(planet.name);
                if (prefab != null)
                {
                    Vector3 position = new Vector3(planet.position[0], planet.position[1], planet.position[2]);
                    GameObject newPlanet = Instantiate(prefab, position, Quaternion.identity, spawnParent);
                    newPlanet.name = prefab.name;
                    newPlanet.transform.localScale = Vector3.one * planet.scale;

                    // 🌟 TAG ATAMA
                    if (planet.name == levelData.targetPlanet)
                        newPlanet.tag = "Target";
                    else
                        newPlanet.tag = "CelestialBody";

                    // Nüfus yüklemesi
                    ClickablePlanetDatabase.PlanetData planetData = planetDatabase.planets.Find(p => p.planetName == planet.name);
                    if (planetData != null)
                    {
                        int savedPopulation = LoadPlanetPopulation(planetData.planetName, planetData.defaultPopulation);
                        planetData.currentPopulation = savedPopulation;
                        Debug.Log($"🔵 {planetData.planetName} için yüklenen nüfus: {planetData.currentPopulation}");
                    }

                    // Hedef gezegen rengi
                    if (planet.name == levelData.targetPlanet)
                    {
                        newPlanet.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }
        }
    }


    GameObject FindPlanetPrefab(string name)
    {
        foreach (var body in celestialBodyData.celestialBodies)
        {
            if (body.bodyName == name && body.prefab != null)
            {
                return body.prefab;
            }
        }
        Debug.LogWarning($"Prefab bulunamadı: {name}");
        return null;
    }

    public void LevelCompleted()
    {
        int currentLevel = PlayerDataManager.GetLevel();
        Debug.Log($"🎯 Tamamlanan level: {currentLevel}");

        LevelData currentLevelData = levelDatabase.levels.Find(l => l.level == currentLevel);
        if (currentLevelData != null)
        {
            string targetPlanetName = currentLevelData.targetPlanet;

            ClickablePlanetDatabase.PlanetData targetPlanetData = planetDatabase.planets.Find(p => p.planetName == targetPlanetName);
            if (targetPlanetData != null)
            {
                IncreasePlanetPopulation(targetPlanetName, 50);
                Debug.Log($"🌍 {targetPlanetName} gezegeninin nüfusu +50 yapıldı.");
            }
            else
            {
                Debug.LogWarning($"🎯 {targetPlanetName} veritabanında bulunamadı!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Level {currentLevel} verisi bulunamadı.");
        }

        // 🔁 Şimdi level'i artır (artık bitirdik çünkü)
        int nextLevel = currentLevel + 1;
        PlayerDataManager.SetLevel(nextLevel);
        Debug.Log($"✅ Yeni level: {nextLevel}");

        // Ana menü sahnesine dön
        SceneManager.LoadScene("MainMenu");
    }
    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SavePlanetPopulation(string planetName, int population)
    {
        SaveData saveData = LoadSaveData();
        PlanetPopulation planetPopulation = saveData.planetPopulations.Find(p => p.planetName == planetName);

        if (planetPopulation != null)
        {
            planetPopulation.population = population;
        }
        else
        {
            saveData.planetPopulations.Add(new PlanetPopulation { planetName = planetName, population = population });
        }

        SaveDataToFile(saveData);
    }

    int LoadPlanetPopulation(string planetName, int defaultPopulation)
    {
        SaveData saveData = LoadSaveData();
        PlanetPopulation planetPopulation = saveData.planetPopulations.Find(p => p.planetName == planetName);

        if (planetPopulation != null)
        {
            return planetPopulation.population;
        }

        return defaultPopulation;
    }

    public void IncreasePlanetPopulation(string planetName, int amount)
    {
        int currentPopulation = LoadPlanetPopulation(planetName, 0);
        int newPopulation = currentPopulation + amount;

        SavePlanetPopulation(planetName, newPopulation);

        ClickablePlanetDatabase.PlanetData updatedPlanetData = planetDatabase.planets.Find(p => p.planetName == planetName);
        if (updatedPlanetData != null)
        {
            updatedPlanetData.currentPopulation = newPopulation;
        }

        Debug.Log($"📈 {planetName} nüfusu artırıldı! Yeni nüfus: {newPopulation}");
    }

    void SaveDataToFile(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    SaveData LoadSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();
    }

    [System.Serializable]
    public class SaveData
    {
        public List<PlanetPopulation> planetPopulations = new List<PlanetPopulation>();
    }

    [System.Serializable]
    public class PlanetPopulation
    {
        public string planetName;
        public int population;
    }
}
