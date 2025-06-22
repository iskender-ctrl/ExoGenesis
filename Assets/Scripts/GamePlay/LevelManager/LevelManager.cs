using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }
    public CelestialBodyData celestialBodyData;
    private LevelDatabase levelDatabase;
    public ClickablePlanetDatabase planetDatabase;
    private string saveFilePath;
    public Transform spawnParent;
    public CelestialBodyManager celestialBodyManager;
    [SerializeField] PopupManager popupManager;

    [Header("UI")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private FuelSystem fuelSystem;
    [SerializeField] private TextMeshProUGUI statusText; // ← YENİ: Başarı/başarısız yazısı
    [SerializeField] private TextMeshProUGUI planetNameText; // ← YENİ: Başarı/başarısız yazısı
    [SerializeField] private TextMeshProUGUI populationText;

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

        fuelSystem = FindFirstObjectByType<FuelSystem>();
        if (fuelSystem != null)
            fuelSystem.OnFuelDepleted += OnFuelDepleted;

        foreach (var planet in planetDatabase.planets)
        {
            planet.currentPopulation = SaveSystem.GetPopulation(planet.planetName, planet.defaultPopulation);
        }

        // ⬇⬇⬇  START SONUNDA GÜNCEL HEDEF GEZEGENİ YAZDIR ⬇⬇⬇

        string targetPlanetName = GetCurrentPlanetName();
        if (!string.IsNullOrEmpty(targetPlanetName))
        {
            planetNameText.text = targetPlanetName;
            int pop = SaveSystem.GetPopulation(targetPlanetName, 0);
            populationText.text = pop.ToString();
        }
    }

    // (1) ► YENİ: her roket atışında durum bayraklarını sıfırlamak için
    public void ResetShotState()                      // NEW
    {
        _alreadyScored = false;
        // _panelReachedTarget'ı sıfırlamıyoruz; hedefe ulaşıldıysa true kalmalı
    }
    public void OnSuccessfulShot()
    {
        if (_panelReachedTarget || _alreadyScored || RocketLauncher.IsPanelOpen) return;

        string targetPlanetName = GetCurrentPlanetName();
        if (string.IsNullOrEmpty(targetPlanetName)) return;

        IncreasePlanetPopulation(targetPlanetName, 10);
        Debug.Log($"🎯 Başarılı atış! {targetPlanetName} +10 nüfus");

        // Seviye ilerleme hesabı HÂLÂ yapılıyor
        int currentPopulation = LoadPlanetPopulation(targetPlanetName, 0);
        int currentLevel = PlayerDataManager.GetLevel();
        LevelData currentLevelData = levelDatabase.levels.Find(l => l.level == currentLevel);

        if (currentLevelData != null && currentPopulation >= currentLevelData.targetPopulation)
        {
            int nextLevel = currentLevel + 1;
            PlayerDataManager.SetLevel(nextLevel);
            Debug.Log("🎉 Yeni level: " + nextLevel);
        }

        _alreadyScored = true;

        /*  ▼▼  K R İ T İ K  D E Ğ İ Ş İ K L İ K  ▼▼
            Panelde daima Continue + Main Menu gözüksün diye
            reachedTarget parametresini **true** gönderiyoruz.
        */
        ShowSuccessPanel(true, 2f);                          // ← değiştirildi
    }

    // (3) ► BAŞARISIZ ATIŞ: panel AÇMA, sadece bayrağı set et
    public void OnFailedShot(float delay = 0f)        // NEW (tamamını değiştir)
    {
        if (_panelReachedTarget || _alreadyScored) return;
        _alreadyScored = true;                        // aynı roket için tekrar sayma
                                                      // **Panel açılmaz** – yakıt bitince OnFuelDepleted() halledecek
        Invoke(nameof(RespawnRocket), delay + 0.5f);
    }
    private void RespawnRocket()
    {
        RocketLauncher.Instance?.SpawnRocket();
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

        if (statusText != null)
        {
            statusText.text = reachedTarget ? "Level Completed" : "Level Failed";
            statusText.color = reachedTarget ? Color.green : Color.red; // İsteğe bağlı
        }

        continueButton.onClick.RemoveAllListeners();
        tryAgainButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();

        continueButton.gameObject.SetActive(reachedTarget);
        tryAgainButton.gameObject.SetActive(!reachedTarget);

        continueButton.onClick.AddListener(() => OnContinueClicked());
        mainMenuButton.onClick.AddListener(() => OnMainMenuClicked()); ;
        tryAgainButton.onClick.AddListener(OnTryAgain);
    }
    private void OnContinueClicked()
    {
        // Butonları geçici disable et (çift tıklama engeli)
        continueButton.interactable = false;
        mainMenuButton.interactable = false;

        AdManager.Instance.ShowInterstitial(() =>
        {
            continueButton.interactable = true;
            mainMenuButton.interactable = true;
            LoadNextLevel(); // Devam et
        });
    }

    private void OnMainMenuClicked()
    {
        continueButton.interactable = false;
        mainMenuButton.interactable = false;

        AdManager.Instance.ShowInterstitial(() =>
        {
            continueButton.interactable = true;
            mainMenuButton.interactable = true;
            SceneManager.LoadScene("MainMenu"); // Ana menüye dön
        });
    }
    private void DelayedShowSuccessPanel()
    {
        // Artık reklam yok!
        RocketLauncher.IsPanelOpen = true;
        popupManager.OpenPopup(successPanel.transform);
        spawnParent.gameObject.SetActive(false);
    }

    private void OnTryAgain()
    {
        if (fuelSystem != null && fuelSystem.CurrentFuel <= 0f)
        {
            Debug.Log("⛽ Yakıt yok, reklam izletilecek...");

            AdManager.Instance.ShowRewardedForFuel(() =>
            {
                fuelSystem.AddFuel(1); // Reklam izleyince 1 yakıt
                RestartLevel();        // Sahneyi yeniden başlat
            });

            return;
        }

        fuelSystem.AddFuel(1); // Normalde zaten 1 yakıt veriyorduk
        RestartLevel();
    }
    private void RestartLevel()
    {
        successPanel.SetActive(false);
        spawnParent.gameObject.SetActive(true);
        RocketLauncher.IsPanelOpen = false;
        _panelReachedTarget = false;
        _alreadyScored = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                    planetNameText.text = newPlanet.name;

                    var planetData = planetDatabase.planets.Find(p => p.planetName == planet.name);
                    if (planetData != null)
                    {
                        int savedPopulation = LoadPlanetPopulation(planetData.planetName, planetData.defaultPopulation);
                        planetData.currentPopulation = savedPopulation;
                    }

                    // Burada orbitAround ekleniyor
                    var celestial = celestialBodyData.celestialBodies.Find(b => b.bodyName == planet.name);
                    if (celestial != null)
                    {
                        celestial.orbitAround = planet.orbitAround;
                    }
                    if (levelData.gravitySettings != null)
                    {
                        GravitySettings.Instance.gravitationalConstant = levelData.gravitySettings.gravitationalConstant;
                        GravitySettings.Instance.radiusMultiplier = levelData.gravitySettings.radiusMultiplier;
                        GravitySettings.Instance.orbitalSpeedMultiplier = levelData.gravitySettings.orbitalSpeedMultiplier;
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
        int newPop = SaveSystem.GetPopulation(planetName) + amount;
        SaveSystem.SetPopulation(planetName, newPop);

        var p = planetDatabase.planets.Find(pl => pl.planetName == planetName);
        if (p != null) p.currentPopulation = newPop;

        Debug.Log($"📈 {planetName} yeni nüfus: {newPop}");
        populationText.text = newPop.ToString();
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
