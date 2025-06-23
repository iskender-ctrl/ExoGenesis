using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FuelSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fuelBar;
    [SerializeField] private TextMeshProUGUI fuelText;

    [Header("Settings")]
    [SerializeField] private int sessionMaxFuel;   // Bu oturumdaki sabit “maksimum”

    public System.Action OnFuelDepleted;
    public float CurrentFuel => PlayerDataManager.GetFuel();

    public static FuelSystem Instance { get; private set; }

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

    void Start()
    {
        // Oyun açıldığında mevcut yakıtı oku; min 5 garanti et
        sessionMaxFuel = Mathf.Max(5, (int)CurrentFuel);
        UpdateFuelBar(immediate: true);
    }

    public void UseFuel(float amount = 1f)
    {
        if (CurrentFuel > 0f)
        {
            PlayerDataManager.UseFuel(amount);
            UpdateFuelBar();
        }
    }

    public void AddFuel(float amount = 1f)
    {
        PlayerDataManager.AddFuel(amount);
        UpdateFuelBar();
    }

    private void UpdateFuelBar(bool immediate = false)
    {
        float current = CurrentFuel;

        // Daima başlangıçta kilitlediğimiz max’e göre oranla
        float targetFill = sessionMaxFuel > 0
            ? Mathf.Clamp01(current / sessionMaxFuel)
            : 0f;

        if (immediate)
            fuelBar.fillAmount = targetFill;
        else
            fuelBar.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutCubic);

        if (fuelText)
            fuelText.text = "%" + Mathf.RoundToInt(targetFill * 100f);

        if (current <= 0)
            OnFuelDepleted?.Invoke();
    }

    public bool HasFuel() => CurrentFuel > 0f;
}
