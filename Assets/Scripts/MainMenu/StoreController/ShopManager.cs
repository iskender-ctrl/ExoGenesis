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

    private void Start()
    {
        defaultRocketName = rocketData.DefaultRocket != null
                            ? rocketData.DefaultRocket.rocketName
                            : "DefaultRocket";
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

        // ► StoreItem'ı ayarla; elle assign gerekmez
        StoreItem storeItem = item.GetComponent<StoreItem>();
        storeItem.Initialize(rocket);   // rocketInfo set + fiyat güncelle

        // Satın alma butonu
        Button buyBtn = item.transform.Find("BuyButton").GetComponent<Button>();
        if (rocket.payment == RocketData.PaymentType.Gold)
            buyBtn.onClick.AddListener(() => BuyWithGold(rocket, item));
        else
            buyBtn.onClick.AddListener(() => BuyWithIAP(rocket, item));
    }
}


    /* ---------- GOLD ---------- */
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

    /* ---------- IAP ---------- */
    private void BuyWithIAP(RocketData.Rocket rocket, GameObject shopItem)
    {
        // Burada kendi IAP sisteminizi çağırın:
        IAPManager.Instance.BuyProduct(rocket.iapProductId,
            onSuccess: () => GrantRocket(rocket, shopItem),
            onFail: () => Debug.Log("Satın alma iptal/başarısız"));
    }

    /* ---------- Ortak ödül verme ---------- */
    private void GrantRocket(RocketData.Rocket rocket, GameObject shopItem)
    {
        InventoryManager.Instance.AddRocket(rocket.rocketName);
        Debug.Log($"{rocket.rocketName} envantere eklendi");
        Destroy(shopItem);

        if (inventoryUI != null) inventoryUI.LoadInventory();
    }
}
