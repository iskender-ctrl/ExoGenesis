using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Transform shopContainer;
    public GameObject shopItemPrefab;
    public RocketData rocketData;
    public InventoryUI inventoryUI;

    private string defaultRocketName;
    private Dictionary<string, GameObject> rocketShopItems = new Dictionary<string, GameObject>();

    private void Start()
    {
        defaultRocketName = rocketData.DefaultRocket != null
                            ? rocketData.DefaultRocket.rocketName
                            : "DefaultRocket";
        IAPManager.Instance.OnRocketPurchased += OnRocketPurchasedHandler;
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (var rocket in rocketData.rockets)
        {
            if (rocket.rocketName == defaultRocketName) continue;
            if (InventoryManager.Instance.HasRocket(rocket.rocketName)) continue;

            GameObject item = Instantiate(shopItemPrefab, shopContainer);

            // UI alanları
            item.transform.Find("RocketName").GetComponent<TextMeshProUGUI>().text = rocket.rocketName;
            item.transform.GetChild(0).GetComponent<Image>().sprite = rocket.icon;

            StoreItem storeItem = item.GetComponent<StoreItem>();
            storeItem.Initialize(rocket);

            Button buyBtn = item.transform.Find("BuyButton").GetComponent<Button>();
            if (rocket.payment == RocketData.PaymentType.Gold)
                buyBtn.onClick.AddListener(() => BuyWithGold(rocket, item));

            // DICTIONARY'YE EKLE!
            rocketShopItems[rocket.rocketName] = item;
        }
    }

    private void BuyWithGold(RocketData.Rocket rocket, GameObject shopItem)
    {
        if (PlayerDataManager.GetCoins() >= rocket.price)
        {
            PlayerDataManager.RemoveCoins(rocket.price);
            GrantRocket(rocket, shopItem);
        }
        else
        {
            Debug.Log("Yetersiz altın!");
        }
    }

    private void GrantRocket(RocketData.Rocket rocket, GameObject shopItem)
    {
        InventoryManager.Instance.AddRocket(rocket.rocketName);
        Debug.Log($"{rocket.rocketName} envantere eklendi");
        Destroy(shopItem);

        rocketShopItems.Remove(rocket.rocketName);

        if (inventoryUI != null) inventoryUI.LoadInventory();
    }

    // IAP'den tetiklenen event
    private void OnRocketPurchasedHandler(string rocketName)
    {
        if (rocketShopItems.TryGetValue(rocketName, out var shopItem) && shopItem != null)
        {
            Destroy(shopItem);
            rocketShopItems.Remove(rocketName);

            if (inventoryUI != null)
                inventoryUI.LoadInventory();
        }
    }
}
