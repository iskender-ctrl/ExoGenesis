using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;

public class StoreItem : MonoBehaviour
{
    [Header("Bağlantılar (boş bırakılabilir)")]
    [SerializeField] private TextMeshProUGUI priceText;   // Inspector’da atıksız olabilir
    [SerializeField] private string priceTextPath = "Price"; // child objenin adı

    [Header("Ürün Bilgisi")]
    [HideInInspector] public RocketData.Rocket rocketInfo;   // ShopManager set edecek

    private void Awake()
    {
        // priceText el ile atanmadıysa bul
        if (priceText == null)
        {
            Transform t = transform.Find(priceTextPath);
            if (t != null) priceText = t.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable() => Refresh();

    /// <summary>ShopManager instantiation sonrası çağırır.</summary>
    public void Initialize(RocketData.Rocket info)
    {
        rocketInfo = info;
        Refresh();
    }

    public void Refresh()
    {
        if (priceText == null || rocketInfo == null) return;

        if (rocketInfo.payment == RocketData.PaymentType.Gold)
        {
            priceText.text = $"{rocketInfo.price:N0} <sprite name=\"Gold\">";
            return;
        }

#if UNITY_EDITOR
        priceText.text = "IAP";
#else
        var prod = IAPManager.Instance?.GetProductById(rocketInfo.iapProductId);
        priceText.text = (prod != null && prod.availableToPurchase)
            ? prod.metadata.localizedPriceString
            : "Yükleniyor…";
#endif
    }
}
