using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Transform rocketContainer;
    public Transform collectionContainer;
    public GameObject itemPrefab;
    public RocketData rocketData;
    private string selectedRocket; // Seçilen roketin ismini saklar

    private void Start()
    {
        LoadInventory();
        LoadSelectedRocket();
    }

    public void LoadInventory()
    {
        foreach (Transform child in rocketContainer) Destroy(child.gameObject);
        foreach (Transform child in collectionContainer) Destroy(child.gameObject);

        List<string> rockets = InventoryManager.Instance.GetRockets();
        List<string> collections = InventoryManager.Instance.GetCollectionItems();

        foreach (string rocket in rockets)
        {
            RocketData.Rocket rocketInfo = rocketData.rockets.Find(r => r.rocketName == rocket);

            if (rocketInfo != null)
            {
                GameObject newItem = Instantiate(itemPrefab, rocketContainer);
                newItem.transform.Find("RocketName").GetComponent<TextMeshProUGUI>().text = rocketInfo.rocketName;
                newItem.transform.Find("RocketIcon").GetComponent<Image>().sprite = rocketInfo.icon;

                // 🔹 "Use" Butonuna eriş ve fonksiyon bağla
                Button useButton = newItem.transform.Find("UseButton").GetComponent<Button>();
                useButton.onClick.AddListener(() => SelectRocket(rocketInfo.rocketName, newItem));
                
                // 🔹 Seçili roketi vurgula
                if (rocketInfo.rocketName == selectedRocket)
                {
                    HighlightSelectedRocket(newItem);
                }
            }
        }

        foreach (string item in collections)
        {
            GameObject newItem = Instantiate(itemPrefab, collectionContainer);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = item;
        }
    }

    private void SelectRocket(string rocketName, GameObject selectedItem)
    {
        // 🔹 Seçili roketi kaydet
        PlayerPrefs.SetString("SelectedRocket", rocketName);
        PlayerPrefs.Save();

        selectedRocket = rocketName;

        Debug.Log("🚀 Seçilen Roket: " + rocketName);

        // 🔹 Tüm roketleri resetle
        foreach (Transform child in rocketContainer)
        {
            ResetRocketUI(child.gameObject);
        }

        // 🔹 Seçilen roketi vurgula
        HighlightSelectedRocket(selectedItem);
    }

    private void LoadSelectedRocket()
    {
        selectedRocket = PlayerPrefs.GetString("SelectedRocket", "");
    }

    private void HighlightSelectedRocket(GameObject rocketItem)
    {
        rocketItem.GetComponent<Image>().color = Color.green; // Vurgulamak için yeşil yap
    }

    private void ResetRocketUI(GameObject rocketItem)
    {
        rocketItem.GetComponent<Image>().color = Color.white; // Varsayılan beyaz
    }
}
