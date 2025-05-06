#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class DecorationController : MonoBehaviour
{
    public AstronautManager astronautManager;
    public static DecorationController Instance;
    public Image backgroundImage;
    public Transform itemSpawnPoint;
    public GameObject itemPrefab, decorationPopUp;

    public TextMeshProUGUI planetNameText;
    public List<GameObject> spawnedItems = new List<GameObject>();
    public List<PlanetPrefabMapping> planetPrefabs;
    public Transform planetTargetParent;
    private SaveData saveData = new SaveData();
    private string saveFilePath;
    public TextMeshProUGUI warningText, populationText;
    [System.Serializable]
    public class PlanetPrefabMapping
    {
        public string planetName;
        public GameObject planetPrefab;
    }

    private void Awake()
    {
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        SetPlanetData(MapDecorationController.Instance.planetDatabase.planets.Find(p => p.planetName == MapDecorationController.Instance.planetName));
        LoadData();
    }
    public void SetPlanetData(ClickablePlanetDatabase.PlanetData planetData)
    {
        if (planetNameText != null)
            planetNameText.text = planetData.planetName;

        if (backgroundImage != null && planetData.bG != null)
        {
            backgroundImage.sprite = planetData.bG;
        }

        // 🌟 Nüfus metnini güncelle
        UpdatePopulationText(planetData.currentPopulation);
        ClearSpawnedItems();
        SpawnItems(planetData.items);
        SpawnPlanetSpecificPrefab(planetData.planetName);
    }
    private void SpawnPlanetSpecificPrefab(string planetName)
    {
        var mapping = planetPrefabs.Find(p => p.planetName == planetName);
        if (mapping != null && mapping.planetPrefab != null)
        {
            GameObject planetObject = Instantiate(mapping.planetPrefab, Vector3.zero, Quaternion.identity, planetTargetParent);
            planetObject.name = planetName + "_Prefab";
            planetObject.SetActive(true); // 🌟 Daima aktif olacak
            spawnedItems.Add(planetObject);

            // 🚀 Astronot spawn işlemini gecikmeli başlat
            StartCoroutine(DelayedSpawnAstronauts(planetObject));
        }
    }
    private System.Collections.IEnumerator DelayedSpawnAstronauts(GameObject planetObject)
    {
        // 🌙 Bir frame bekle (tüm objeler instantiate olsun)
        yield return null;

        Transform spawnPoint = planetObject.transform.Find("AstronautSpawnPoint");
        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == MapDecorationController.Instance.planetName);

        if (spawnPoint != null && astronautManager != null && planetData != null)
        {
            astronautManager.spawnAreaCenter = spawnPoint;
            astronautManager.SpawnAstronauts(planetData.currentPopulation);
        }
        else
        {
            Debug.LogWarning("❌ AstronautSpawnPoint bulunamadı veya population verisi yok.");
        }
    }

    private void SpawnItems(List<ClickablePlanetDatabase.DecorationItem> items)
    {
        // 🌟 Gezegenin mevcut nüfusunu al
        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == MapDecorationController.Instance.planetName);
        int currentPopulation = planetData != null ? planetData.currentPopulation : 0;

        foreach (var item in items)
        {
            // 🌟 Silinen öğe daha önce kaydedilmiş mi?
            if (saveData.removedItems.Contains(item.decorationName))
            {
                Debug.Log(item.decorationName + " daha önce silinmiş, listelenmiyor.");
                continue;  // Listeleme
            }

            if (itemPrefab != null && itemSpawnPoint != null)
            {
                GameObject newItem = Instantiate(itemPrefab, itemSpawnPoint.position, Quaternion.identity, itemSpawnPoint);
                newItem.name = item.decorationName;

                // 🌟 Item bileşenlerini bul
                Image itemIcon = newItem.transform.Find("Icon")?.GetComponent<Image>();
                TextMeshProUGUI itemNameText = newItem.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                Button itemButton = newItem.GetComponent<Button>();

                // 🌟 Butonun child'ı olan TextMeshProUGUI bileşenini bul

                // 🌟 Item bilgilerini ayarla
                if (itemIcon != null) itemIcon.sprite = item.icon;
                if (itemNameText != null) itemNameText.text = item.decorationName;



                // 🌟 Nüfus yeterli mi?
                if (currentPopulation < item.requiredPopulation)
                {
                    // 🌟 Nüfus yetersizse, itemi grileştir veya pasif yap
                    if (itemIcon != null) itemIcon.color = Color.gray;
                    if (itemButton != null) itemButton.interactable = false;
                }
                else
                {
                    // 🌟 Nüfus yeterliyse, itemi aktif yap
                    if (itemIcon != null) itemIcon.color = Color.white;
                    if (itemButton != null) itemButton.interactable = true;
                }

                // 🌟 Butona tıklama özelliği ekle
                PlanetButton relatedButton = newItem.GetComponentInChildren<PlanetButton>();
                if (relatedButton != null)
                {
                    relatedButton.childName = item.decorationName;
                }
                TextMeshProUGUI requiredPopulationText = relatedButton.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();

                // 🌟 Required population değerini butonun child'ına yazdır
                if (requiredPopulationText != null)
                {
                    requiredPopulationText.text = "Nüfus: " + item.requiredPopulation.ToString();
                }

                spawnedItems.Add(newItem);
            }
        }
    }
    private void CheckPlanetCollectionComplete(string planetName)
    {
        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == planetName);

        if (planetData == null) return;

        // 🔍 Tüm item'ler aktif mi?
        bool allDecorationsActive = true;
        foreach (var item in planetData.items)
        {
            if (!saveData.activeObjects.Contains(item.decorationName))
            {
                allDecorationsActive = false;
                break;
            }
        }

        if (allDecorationsActive)
        {
            if (!InventoryManager.Instance.GetCollectionItems().Contains(planetName))
            {
                InventoryManager.Instance.AddCollectionItem(planetName);
                Debug.Log("🌟 Koleksiyon tamamlandı ve eklendi: " + planetName);
            }
        }
    }

    private void UpdatePopulationText(int currentPopulation)
    {
        if (populationText != null)
        {
            populationText.text = "Population: " + currentPopulation.ToString();
        }
    }

    private void ClearSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ActivateChild(string childName)
    {
        Debug.Log("Aktif edilecek child obje: " + childName);
        decorationPopUp.SetActive(false);

        // 🌟 İlgili gezegenin verisini bul
        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == MapDecorationController.Instance.planetName);

        if (planetData == null)
        {
            Debug.LogWarning("Gezegen verisi bulunamadı.");
            return;
        }

        // 🌟 Dekorasyon öğesini bul
        var itemData = planetData.items.Find(item => item.decorationName == childName);

        if (itemData == null)
        {
            Debug.LogWarning("İlgili DecorationItem bulunamadı.");
            return;
        }

        // 🌟 Nüfus kontrolü
        if (planetData.currentPopulation < itemData.requiredPopulation)
        {
            ShowWarning($"Bu dekorasyonu almak için en az {itemData.requiredPopulation} nüfusa ihtiyacınız var!");
            return;
        }

        Debug.Log($"Dekorasyon açıldı: {childName} (Gereken nüfus: {itemData.requiredPopulation}, Mevcut nüfus: {planetData.currentPopulation})");

        // 🌟 Child objeyi aktif et
        foreach (Transform planet in planetTargetParent)
        {
            Transform targetChild = planet.Find(childName);
            if (targetChild != null)
            {
                targetChild.gameObject.SetActive(true);
                Debug.Log("Aktif edildi: " + targetChild.name);

                if (!saveData.activeObjects.Contains(childName))
                {
                    saveData.activeObjects.Add(childName);
                    SaveData();
                }

                RemoveItemFromList(childName);

                // 🌟 Nüfus metnini güncelle
                UpdatePopulationText(planetData.currentPopulation);
                CheckPlanetCollectionComplete(planetData.planetName);
                return;
            }
        }
    }

    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        Invoke(nameof(HideWarning), 2f); // 2 saniye sonra kapat
    }

    private void HideWarning()
    {
        warningText.gameObject.SetActive(false);
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Veri kaydedildi: " + saveFilePath);
    }
    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Veri yüklendi: " + saveFilePath);

            // 🌟 Kaydedilen aktif objeleri otomatik olarak aktif et
            foreach (string activeObjectName in saveData.activeObjects)
            {
                foreach (Transform planet in planetTargetParent)
                {
                    Transform targetChild = planet.Find(activeObjectName);
                    if (targetChild != null)
                    {
                        targetChild.gameObject.SetActive(true);
                    }
                }
            }

            // 🌟 Silinen liste öğelerini kaldır
            foreach (string removedItem in saveData.removedItems)
            {
                GameObject itemToRemove = spawnedItems.Find(item => item.name == removedItem);
                if (itemToRemove != null)
                {
                    spawnedItems.Remove(itemToRemove);
                    Destroy(itemToRemove);
                    Debug.Log(removedItem + " daha önce silinmişti, tekrar listelenmedi.");
                }
            }

            // 🌟 Nüfus metnini güncelle
            var planetData = MapDecorationController.Instance.planetDatabase.planets
                .Find(p => p.planetName == MapDecorationController.Instance.planetName);
            if (planetData != null)
            {
                UpdatePopulationText(planetData.currentPopulation);
            }
        }
    }

    private void RemoveItemFromList(string childName)
    {
        GameObject itemToRemove = spawnedItems.Find(item => item.name == childName);
        if (itemToRemove != null)
        {
            spawnedItems.Remove(itemToRemove);
            Destroy(itemToRemove);
            Debug.Log(childName + " listeden tamamen silindi.");

            // 🌟 Silinen öğeyi kaydet
            if (!saveData.removedItems.Contains(childName))
            {
                saveData.removedItems.Add(childName);
                SaveData();  // Veriyi güncelledikten sonra kaydet
            }
        }
    }


#if UNITY_EDITOR
    [ContextMenu("🗑️ Delete Save Data")]
    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data deleted: " + saveFilePath);
            EditorUtility.DisplayDialog("Save Data Manager", "Save data deleted successfully!", "OK");
        }
        else
        {
            Debug.LogWarning("No save data found to delete.");
        }
    }
#endif
}
