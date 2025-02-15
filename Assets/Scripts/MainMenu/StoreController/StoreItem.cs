using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using System.Linq;

[ExecuteAlways]
public class StoreItem : MonoBehaviour
{
    public string productId; 
    public TextMeshProUGUI priceText;

    private void OnEnable()
    {
        UpdatePrice();
    }

    public void UpdatePrice()
    {
#if UNITY_EDITOR
        var catalog = ProductCatalog.LoadDefaultCatalog();
        var product = catalog.allProducts.FirstOrDefault(p => p.id == productId);

        if (product != null)
        {
            // Simülasyon için ürün adını ve varsayılan fiyat göster
            string simulatedPrice = !string.IsNullOrEmpty(product.id) 
                ? $"{product.id} - 9.99₺ (Simülasyon)" 
                : "9.99₺ (Simülasyon)";

            priceText.text = simulatedPrice;
        }
        else
        {
            priceText.text = "Ürün bulunamadı";
        }
#else
        // Oyun çalışırken gerçek fiyatı göster
        if (IAPManager.Instance != null)
        {
            var product = IAPManager.Instance.GetProductById(productId);
            if (product != null && product.availableToPurchase)
            {
                priceText.text = product.metadata.localizedPriceString;
            }
            else
            {
                priceText.text = "Yükleniyor...";
            }
        }
        else
        {
            priceText.text = "IAP Başlatılmadı";
        }
#endif
    }
}
