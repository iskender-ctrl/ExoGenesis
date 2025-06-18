#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DecorationController : MonoBehaviour
{
    [Header("Refs")]
    public AstronautManager astronautManager;
    public Image backgroundImage;
    public Transform itemSpawnPoint;          // UI’deki butonların geleceği parent
    public GameObject itemPrefab;             // Tek bir UI item prefab’ı
    public GameObject decorationPopUp;        // Aç/iptal popup’ı
    public Transform planetTargetParent;      // Sahnedeki gezegen (ve child’ları) için parent
    public List<PlanetPrefabMapping> planetPrefabs;

    [Header("UI")]
    public TextMeshProUGUI planetNameText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI populationText;

    [HideInInspector] public static DecorationController Instance;

    // Runtime listeleri
    private readonly List<GameObject> spawnedItems = new();

    [System.Serializable]
    public class PlanetPrefabMapping
    {
        public string planetName;
        public GameObject planetPrefab;
    }

    /*─────────────────────────────── L I F E  C Y C L E ───────────────────────────────*/
    private void Awake()
    {
        Instance = this;

        // Aktif gezegen verisini bul
        var planetData = MapDecorationController.Instance.planetDatabase
            .planets.Find(p => p.planetName == MapDecorationController.Instance.planetName);

        SetPlanetData(planetData);
        LoadDecorationData();          // daha önce açılan dekorasyonları uygula
    }

    /*──────────────────────────── S E T U P  /  S P A W N ─────────────────────────────*/
    public void SetPlanetData(ClickablePlanetDatabase.PlanetData planetData)
    {
        if (planetNameText) planetNameText.text = planetData.planetName;
        if (backgroundImage && planetData.bG) backgroundImage.sprite = planetData.bG;

        UpdatePopulationText(planetData.currentPopulation);

        ClearSpawnedItems();
        SpawnItems(planetData.items);
        SpawnPlanetSpecificPrefab(planetData.planetName);
    }

    private void SpawnPlanetSpecificPrefab(string planetName)
    {
        var mapping = planetPrefabs.Find(p => p.planetName == planetName);
        if (mapping == null || mapping.planetPrefab == null) return;

        GameObject planetObj = Instantiate(mapping.planetPrefab, Vector3.zero,
                                           Quaternion.identity, planetTargetParent);
        planetObj.name = planetName + "_Prefab";
        planetObj.SetActive(true);                          // Gezegen daima aktif

        spawnedItems.Add(planetObj);

        // Astronotları bir frame sonra spawn et
        StartCoroutine(DelayedSpawnAstronauts(planetObj));
    }

    private System.Collections.IEnumerator DelayedSpawnAstronauts(GameObject planetObject)
    {
        yield return null; // 1 frame

        Transform spawnPoint = planetObject.transform.Find("AstronautSpawnPoint");
        var planetData = MapDecorationController.Instance.planetDatabase
            .planets.Find(p => p.planetName == MapDecorationController.Instance.planetName);

        if (spawnPoint && astronautManager && planetData != null)
        {
            astronautManager.spawnAreaCenter = spawnPoint;
            astronautManager.SpawnAstronauts(planetData.currentPopulation);
        }
        else
            Debug.LogWarning("❌ AstronautSpawnPoint bulunamadı veya population verisi yok.");
    }

    /*─────────────────────────── I T E M   (U I)  S P A W N ───────────────────────────*/
    private void SpawnItems(List<ClickablePlanetDatabase.DecorationItem> items)
    {
        var planetData = MapDecorationController.Instance.planetDatabase
            .planets.Find(p => p.planetName == MapDecorationController.Instance.planetName);

        int currentPop = planetData != null ? planetData.currentPopulation : 0;

        foreach (var item in items)
        {
            // Listeden kalıcı silindiyse atla
            if (SaveSystem.IsItemRemoved(item.decorationName))
                continue;

            GameObject uiItem = Instantiate(itemPrefab, itemSpawnPoint.position,
                                            Quaternion.identity, itemSpawnPoint);
            uiItem.name = item.decorationName;

            // Görseller & metin
            Image icon = uiItem.transform.Find("Icon")?.GetComponent<Image>();
            TextMeshProUGUI nameTxt = uiItem.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (icon) icon.sprite = item.icon;
            if (nameTxt) nameTxt.text = item.decorationName;

            // Nüfus kontrolü
            Button btn = uiItem.GetComponent<Button>();
            bool unlocked = currentPop >= item.requiredPopulation;
            if (icon) icon.color = unlocked ? Color.white : Color.gray;
            if (btn) btn.interactable = unlocked;

            // Butonun child’ındaki nüfus metni
            PlanetButton planetBtn = uiItem.GetComponentInChildren<PlanetButton>();
            if (planetBtn) planetBtn.childName = item.decorationName;

            TextMeshProUGUI reqTxt = planetBtn?.transform.GetChild(0)?.GetComponent<TextMeshProUGUI>();
            if (reqTxt) reqTxt.text = $"Nüfus: {item.requiredPopulation}";

            spawnedItems.Add(uiItem);
        }
    }

    /*────────────────────────── D E K O R A S Y O N  A C M A ─────────────────────────*/
    public void ActivateChild(string childName)
    {
        decorationPopUp.SetActive(false);

        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == MapDecorationController.Instance.planetName);
        if (planetData == null) return;

        var itemData = planetData.items.Find(i => i.decorationName == childName);
        if (itemData == null) return;

        if (planetData.currentPopulation < itemData.requiredPopulation)
        {
            ShowWarning($"Bu dekorasyonu almak için en az {itemData.requiredPopulation} nüfusa ihtiyacınız var!");
            return;
        }

        // Çocuğu (Prefab içindeki objeyi) aktif et
        foreach (Transform planet in planetTargetParent)
        {
            Transform target = planet.Find(childName); // child direkt altındaysa
            if (target != null)
            {
                target.gameObject.SetActive(true);
                SaveSystem.AddActiveObject(childName);     // KAYIT
                RemoveItemFromList(childName);             // Butonu listeden sil
                CheckPlanetCollectionComplete(planetData.planetName);
                return;
            }
        }
        Debug.LogWarning($"Child obje bulunamadı: {childName}");
    }
    /// <summary>
    /// Bu gezegendeki TÜM dekorasyonlar açıldıysa koleksiyon ödülünü ekler.
    /// </summary>
    private void CheckPlanetCollectionComplete(string planetName)
    {
        // Gezegen verisini bul
        var planetData = MapDecorationController.Instance.planetDatabase
            .planets.Find(p => p.planetName == planetName);
        if (planetData == null) return;

        // Tüm dekorasyonlar aktif mi?
        bool allActive = true;
        foreach (var item in planetData.items)
        {
            if (!SaveSystem.IsObjectActive(item.decorationName))
            {
                allActive = false;
                break;
            }
        }

        // Koleksiyon ödülünü ver
        if (allActive &&
            !InventoryManager.Instance.GetCollectionItems().Contains(planetName))
        {
            InventoryManager.Instance.AddCollectionItem(planetName);
            Debug.Log($"🌟 Koleksiyon tamamlandı: {planetName}");
        }
    }

    /*─────────────────────────────── L O A D / S A V E ───────────────────────────────*/
    private void LoadDecorationData()
    {
        SaveData data = SaveSystem.Load();

        /* 1) Daha önce aktifleştirilen dekorasyonları sahnede bulup aç */
        Transform[] allTransforms = planetTargetParent.GetComponentsInChildren<Transform>(true);
        foreach (string objName in data.activeObjects)
        {
            foreach (Transform t in allTransforms)
            {
                if (t.name == objName)
                {
                    t.gameObject.SetActive(true);
                    break;
                }
            }
        }

        /* 2) Önceden listeden çıkarılmış (removed) öğeleri UI listenden sil */
        spawnedItems.RemoveAll(item =>
        {
            bool removed = data.removedItems.Contains(item.name);
            if (removed) Destroy(item);
            return removed;
        });
    }

    private void RemoveItemFromList(string childName)
    {
        GameObject uiItem = spawnedItems.Find(go => go.name == childName);
        if (uiItem == null) return;

        spawnedItems.Remove(uiItem);
        Destroy(uiItem);

        SaveSystem.AddRemovedItem(childName);   // KAYIT
    }

    /*──────────────────────────── U I   Y A R D I M C I L A R ─────────────────────────*/
    private void UpdatePopulationText(int pop)
    {
        if (populationText) populationText.text = $"Population: {pop}";
    }

    private void ShowWarning(string msg)
    {
        if (!warningText) return;
        warningText.text = msg;
        warningText.gameObject.SetActive(true);
        Invoke(nameof(HideWarning), 2f);
    }
    private void HideWarning() => warningText?.gameObject.SetActive(false);

    private void ClearSpawnedItems()
    {
        foreach (var go in spawnedItems) Destroy(go);
        spawnedItems.Clear();
    }

    /*───────────────────────────────── U I  N A V I ──────────────────────────────────*/
    public void LoadScene() => SceneManager.LoadSceneAsync(1);

#if UNITY_EDITOR
    [ContextMenu("🗑️ Delete Save Data")]
    private void DeleteSaveData()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "saveData.json");
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Save data deleted: " + path);
            EditorUtility.DisplayDialog("Save Data Manager", "Save data deleted successfully!", "OK");
        }
        else
            Debug.LogWarning("No save data found to delete.");
    }
#endif
}
