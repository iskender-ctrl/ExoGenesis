using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public List<string> productIds; // IAP Catalog’daki ürün ID'leri
    public event Action<string> OnRocketPurchased;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized()) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var id in productIds)
        {
            builder.AddProduct(id, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public Product GetProductById(string productId)
    {
        if (IsInitialized())
        {
            return storeController.products.WithID(productId);
        }
        return null;
    }

    // IStoreListener Metotları
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    // **GÜNCELLENEN METOT** (2 parametre alıyor artık)
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //        Debug.LogError($"IAP Başlatılamadı: {error} - {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;
        switch (productId)
        {
            case "removeads":
                Debug.Log("NO ADS PURCHASE");
                break;
            case "buy100coin":
                GrantCoins(100);
                break;
            case "buy200coin":
                GrantCoins(200);
                break;
            case "buy5fuel":
                GrantFuel(5);
                break;
            case "buy10fuel":
                GrantFuel(10);
                break;
            case "buy5fuel100coin":
                GrantFuel(5);
                GrantCoins(100);
                break;
            case "buy5fuel200coin":
                GrantFuel(5);
                GrantCoins(200);
                break;   // ← BU EKSİKTİ!
            case "buy10fuel100coin":
                GrantFuel(10);
                GrantCoins(100);
                break;   // ← BU EKSİKTİ!
            case "buy10fuel200coin":
                GrantFuel(10);
                GrantCoins(200);
                break;
            case "rocketsisko":
                GrantRocket("İskender");
                break;
            default:
                Debug.LogWarning($"Bilinmeyen ürün: {productId}");
                break;
        }


        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Satın alma başarısız: {product.definition.id}, Neden: {failureReason}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }
    public void BuyProduct(string productId, System.Action onSuccess, System.Action onFail)
    {
        Debug.Log($"[IAP-Stub] Satın alma simüle edildi: {productId}");
        onSuccess?.Invoke();   // daima başarı
    }
    private void GrantFuel(int amount)
    {
        PlayerDataManager.AddFuel(amount);
        Debug.Log($"+{amount} yakıt verildi.");
    }
    private void GrantCoins(int amount)
    {
        PlayerDataManager.AddCoins(amount);
        Debug.Log($"+{amount} coin verildi.");
    }
    private void GrantRocket(string rocketName)
    {
        InventoryManager.Instance.AddRocket(rocketName);
        Debug.Log($"{rocketName} roketi satın alındı ve envantere eklendi.");
        OnRocketPurchased?.Invoke(rocketName); // → UI’dan da kaldırmak için ShopManager dinleyecek
    }
}
