using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public Transform shopContainer;
    public GameObject shopItemPrefab;
    public RocketData rocketData;
    public InventoryUI inventoryUI; // Envanteri güncellemek için referans

    private void Start()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (var rocket in rocketData.rockets)
        {
            if (InventoryManager.Instance.HasRocket(rocket.rocketName)) continue; // Eğer roket zaten satın alındıysa atla

            GameObject item = Instantiate(shopItemPrefab, shopContainer);

            item.transform.Find("RocketName").GetComponent<TextMeshProUGUI>().text = rocket.rocketName;

            TextMeshProUGUI priceText = item.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            priceText.text = rocket.price.ToString();
            priceText.raycastTarget = false;

            item.transform.Find("Icon").GetComponent<Image>().sprite = rocket.icon;

            Button buyButton = item.transform.Find("BuyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyRocket(rocket, item));
        }
    }

    private void BuyRocket(RocketData.Rocket rocket, GameObject shopItem)
    {
        if (PlayerDataManager.GetCoins() >= rocket.price)
        {
            PlayerDataManager.RemoveCoins(rocket.price);
            InventoryManager.Instance.AddRocket(rocket.rocketName);
            Debug.Log(rocket.rocketName + " satın alındı!");

            Destroy(shopItem); // Satın alındıktan sonra itemi Shop listesinden kaldır

            // 🔥 Envanteri anında güncelle!
            if (inventoryUI != null)
            {
                inventoryUI.LoadInventory();
            }
        }
        else
        {
            Debug.Log("Yetersiz coin!");
        }
    }
}
