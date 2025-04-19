using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public List<string> productIds; // IAP Catalog’daki ürün ID'leri

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
        Debug.Log($"Satın alma başarılı: {args.purchasedProduct.definition.id}");
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
}
