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
    public PopupManager popupManager;
    public Transform popupUI;
    public TextMeshProUGUI popupTitle;
    public Button popupSelectButton;
    private string selectedRocket;
    public string defaultRocketName;

    private void Start()
    {
        var defaultRocket = rocketData.DefaultRocket;

        // Default roket envantere ekli değilse ekle
        if (!InventoryManager.Instance.HasRocket(defaultRocketName))
        {
            InventoryManager.Instance.AddRocket(defaultRocketName);
        }

        LoadSelectedRocket();

        // Seçili roket yoksa defaultu seç
        if (string.IsNullOrEmpty(selectedRocket))
        {
            selectedRocket = defaultRocketName;
            PlayerPrefs.SetString("SelectedRocket", selectedRocket);
            PlayerPrefs.Save();
        }

        LoadInventory();
    }
    public void LoadInventory()
    {
        foreach (Transform child in rocketContainer) Destroy(child.gameObject);
        foreach (Transform child in collectionContainer) Destroy(child.gameObject);

        // Envanterde tüm roketleri göster, rocketData.rockets içindeki tüm roketler
        foreach (var rocketInfo in rocketData.rockets)
        {
            GameObject newItem = Instantiate(itemPrefab, rocketContainer);
            newItem.transform.Find("RocketName").GetComponent<TextMeshProUGUI>().text = rocketInfo.rocketName;
            newItem.transform.Find("RocketIcon").GetComponent<Image>().sprite = rocketInfo.icon;

            Button btn = newItem.GetComponent<Button>();

            // Roket satın alınmış mı?
            bool hasRocket = InventoryManager.Instance.HasRocket(rocketInfo.rocketName);

            btn.interactable = hasRocket;

            btn.onClick.AddListener(() =>
            {
                if (!btn.interactable)
                    return; // Satın alınmamış rokete tıklanmasın

                popupTitle.text = rocketInfo.rocketName;
                popupSelectButton.onClick.RemoveAllListeners();
                popupSelectButton.onClick.AddListener(() => SelectRocket(rocketInfo.rocketName, newItem));
                popupManager.OpenPopup(popupUI);
            });

            // Seçili roketi vurgula
            if (rocketInfo.rocketName == selectedRocket)
            {
                HighlightSelectedRocket(newItem);
            }
            else
            {
                ResetRocketUI(newItem);
            }
        }

        // Koleksiyon itemlarınız varsa onları da ekleyebilirsiniz
    }

    private void SelectRocket(string rocketName, GameObject selectedItem)
    {
        PlayerPrefs.SetString("SelectedRocket", rocketName);
        PlayerPrefs.Save();
        selectedRocket = rocketName;
        FirebaseEventManager.LogItemPurchased("rocket_selected", 0);
        
        foreach (Transform child in rocketContainer)
        {
            // Rocket adını UI’dan çek
            string name = child.Find("RocketName").GetComponent<TextMeshProUGUI>().text;

            Button btn = child.GetComponent<Button>();
            if (btn == null) continue;

            // Satın alınmışsa aktif bırak, değilse pasifle
            bool hasRocket = InventoryManager.Instance.HasRocket(name);
            btn.interactable = hasRocket;

            ResetRocketUI(child.gameObject);
        }

        // Seçili roketi vurgula
        HighlightSelectedRocket(selectedItem);

        popupManager.ClosePopup(popupUI);
        Debug.Log("🚀 Seçilen Roket: " + rocketName);
    }

    private void LoadSelectedRocket()
    {
        selectedRocket = PlayerPrefs.GetString("SelectedRocket", "");
    }

    private void HighlightSelectedRocket(GameObject rocketItem)
    {
        rocketItem.GetComponent<Image>().color = Color.green;
    }

    private void ResetRocketUI(GameObject rocketItem)
    {
        rocketItem.GetComponent<Image>().color = Color.white;
    }
}
