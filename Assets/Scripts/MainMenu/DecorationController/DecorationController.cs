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
    public TextMeshProUGUI warningText;
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
        }
    }

    private void SpawnItems(List<ClickablePlanetDatabase.DecorationItem> items)
    {
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

                Image itemIcon = newItem.transform.Find("Icon")?.GetComponent<Image>();
                TextMeshProUGUI itemNameText = newItem.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();

                if (itemIcon != null) itemIcon.sprite = item.icon;
                if (itemNameText != null) itemNameText.text = item.decorationName;

                PlanetButton relatedButton = newItem.GetComponentInChildren<PlanetButton>();
                if (relatedButton != null)
                {
                    relatedButton.childName = item.decorationName;
                }

                spawnedItems.Add(newItem);
            }
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

        // 🌟 İlgili PlanetData ve DecorationItem'ı bul
        var planetData = MapDecorationController.Instance.planetDatabase.planets
            .Find(p => p.planetName == MapDecorationController.Instance.planetName);

        var itemData = planetData.items.Find(item => item.decorationName == childName);

        if (itemData == null)
        {
            Debug.LogWarning("İlgili DecorationItem bulunamadı.");
            return;
        }

        int playerCoins = PlayerDataManager.GetCoins();

        // 🌟 Coin kontrolü
        if (playerCoins < itemData.cost)
        {
            ShowWarning("Yetersiz coin! Gerekli: " + itemData.cost);
            return;
        }

        // 🌟 Coin düş ve kaydet
        PlayerDataManager.SpendCoins(itemData.cost);
        Debug.Log(itemData.cost + " coin harcandı. Kalan coin: " + PlayerDataManager.GetCoins());

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
                return;
            }
        }

        Debug.LogWarning("Child obje bulunamadı: " + childName);
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
