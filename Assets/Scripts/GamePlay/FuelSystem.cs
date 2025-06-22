using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FuelSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fuelBar;
    [SerializeField] private TextMeshProUGUI fuelText; // Yüzdelik gösterim

    [Header("Settings")]
    [SerializeField] private int maxFuel;
    public System.Action OnFuelDepleted;
    public float CurrentFuel => PlayerDataManager.GetFuel();

    public static FuelSystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton & DontDestroyOnLoad
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
        maxFuel = (int)PlayerDataManager.GetFuel();
        UpdateFuelBar(immediate: true); // Başlangıçta hızlı yüklensin
    }

    public void UseFuel(float amount = 1f)
    {
        if (PlayerDataManager.GetFuel() > 0f)
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
        float current = PlayerDataManager.GetFuel();
        float targetFill = current / maxFuel;

        if (immediate)
        {
            fuelBar.fillAmount = targetFill;
        }
        else
        {
            fuelBar.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutCubic);
        }

        if (fuelText)
        {
            int percent = Mathf.RoundToInt(targetFill * 100f);
            fuelText.text = "%" + percent;
        }

        if (current <= 0)
        {
            OnFuelDepleted?.Invoke();
        }
    }

    public bool HasFuel()
    {
        return PlayerDataManager.GetFuel() > 0f;
    }
}
