using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }
    public CelestialBodyData celestialBodyData;
    private LevelDatabase levelDatabase;
    public ClickablePlanetDatabase planetDatabase;
    private string saveFilePath;
    public Transform spawnParent;

    [Header("UI")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private FuelSystem fuelSystem;

    private bool _panelReachedTarget = false;
    private bool _alreadyScored = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (successPanel != null)
            successPanel.SetActive(false);

        RocketLauncher.IsPanelOpen = false;
        saveFilePath = Application.persistentDataPath + "/saveData.json";
        LoadLevelData();
        LoadLevel(PlayerDataManager.GetLevel());

        fuelSystem = FindObjectOfType<FuelSystem>();
        if (fuelSystem != null)
            fuelSystem.OnFuelDepleted += OnFuelDepleted;

        foreach (var planet in planetDatabase.planets)
        {
            int currentPopulation = LoadPlanetPopulation(planet.planetName, planet.defaultPopulation);
            planet.currentPopulation = currentPopulation;
        }
    }

    public void OnSuccessfulShot()
    {
        if (_panelReachedTarget || _alreadyScored || RocketLauncher.IsPanelOpen) return;

        string targetPlanetName = GetCurrentPlanetName();
        if (!string.IsNullOrEmpty(targetPlanetName))
        {
            IncreasePlanetPopulation(targetPlanetName, 10);
            Debug.Log($"🎯 Başarılı atış! {targetPlanetName} +10 nüfus");

            int currentPopulation = LoadPlanetPopulation(targetPlanetName, 0);
            int currentLevel = PlayerDataManager.GetLevel();
            LevelData currentLevelData = levelDatabase.levels.Find(l => l.level == currentLevel);

            if (currentLevelData != null)
            {
                bool reachedTarget = currentPopulation >= currentLevelData.targetPopulation;

                if (reachedTarget)
                {
                    int nextLevel = currentLevel + 1;
                    PlayerDataManager.SetLevel(nextLevel);
                    Debug.Log("🎉 Yeni level: " + nextLevel);
                }

                _alreadyScored = true;
                ShowSuccessPanel(reachedTarget, 2f);
            }
        }
    }

    private void ShowSuccessPanel(bool reachedTarget, float delay)
    {
        if (_panelReachedTarget && !reachedTarget)
        {
            Debug.Log("⚠️ Panel zaten başarıyla açılmıştı, başarısız çağrı iptal.");
            return;
        }

        _panelReachedTarget = reachedTarget;
        CancelInvoke(nameof(DelayedShowSuccessPanel));
        Invoke(nameof(DelayedShowSuccessPanel), delay);

        continueButton.onClick.RemoveAllListeners();
        tryAgainButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();

        continueButton.gameObject.SetActive(reachedTarget);
        tryAgainButton.gameObject.SetActive(!reachedTarget);

        continueButton.onClick.AddListener(LoadNextLevel);
        tryAgainButton.onClick.AddListener(OnTryAgain);
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    private void DelayedShowSuccessPanel()
    {
        RocketLauncher.IsPanelOpen = true;
        successPanel.SetActive(true);
    }

    private void OnTryAgain()
    {
        if (fuelSystem != null)
            fuelSystem.AddFuel(1);

        successPanel.SetActive(false);
        RocketLauncher.IsPanelOpen = false;
        _panelReachedTarget = false;
        _alreadyScored = false;
    }

    public void OnFuelDepleted()
    {
        if (!_panelReachedTarget)
        {
            Debug.Log("⛽ Yakıt bitti!");
            ShowSuccessPanel(false, 1f);
        }
    }

    public void OnRocketCrashed()
    {
        if (fuelSystem != null)
            fuelSystem.UseFuel();
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
            levelDatabase = JsonUtility.FromJson<LevelDatabase>(jsonFile.text);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

                    newPlanet.tag = planet.name == levelData.targetPlanet ? "Target" : "CelestialBody";

                    var planetData = planetDatabase.planets.Find(p => p.planetName == planet.name);
                    if (planetData != null)
                    {
                        int savedPopulation = LoadPlanetPopulation(planetData.planetName, planetData.defaultPopulation);
                        planetData.currentPopulation = savedPopulation;
                    }

                    if (planet.name == levelData.targetPlanet)
                        newPlanet.GetComponent<Renderer>().material.color = Color.green;
                }
            }
        }
    }

    GameObject FindPlanetPrefab(string name)
    {
        foreach (var body in celestialBodyData.celestialBodies)
        {
            if (body.bodyName == name && body.prefab != null)
                return body.prefab;
        }
        Debug.LogWarning($"Prefab bulunamadı: {name}");
        return null;
    }

    void SavePlanetPopulation(string planetName, int population)
    {
        SaveData saveData = LoadSaveData();
        PlanetPopulation planetPopulation = saveData.planetPopulations.Find(p => p.planetName == planetName);

        if (planetPopulation != null)
            planetPopulation.population = population;
        else
            saveData.planetPopulations.Add(new PlanetPopulation { planetName = planetName, population = population });

        SaveDataToFile(saveData);
    }

    int LoadPlanetPopulation(string planetName, int defaultPopulation)
    {
        SaveData saveData = LoadSaveData();
        PlanetPopulation planetPopulation = saveData.planetPopulations.Find(p => p.planetName == planetName);
        return planetPopulation != null ? planetPopulation.population : defaultPopulation;
    }

    public void IncreasePlanetPopulation(string planetName, int amount)
    {
        int currentPopulation = LoadPlanetPopulation(planetName, 0);
        int newPopulation = currentPopulation + amount;
        SavePlanetPopulation(planetName, newPopulation);

        var updated = planetDatabase.planets.Find(p => p.planetName == planetName);
        if (updated != null)
            updated.currentPopulation = newPopulation;

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
