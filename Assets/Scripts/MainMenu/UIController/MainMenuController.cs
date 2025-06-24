using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panel-Buton Dizileri")]
    public GameObject[] panels;      // Her panel bir GameObject
    public Button[] buttons;     // Aynı sırada, bir buton = bir panel

    [Header("Buton Ölçek")]
    public float buttonScaleSelected = 1.2f;
    public float buttonScaleDefault = 1.0f;
    public float animDuration = 0.3f;

    [Header("BG Genişlik")]
    public float selectedWidth = 250f;      // Seçili butonun BG genişliği (px)
    public Ease widthEase = Ease.OutBack;

    [Header("Bilgi Metinleri")]
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI coinsText;

    /* ─────────── dahili ─────────── */
    int currentPanelIndex = 1;
    RectTransform[] parentRTs;          // her butonun parent RectTransform’i
    float[] originalWidths;    // parent’in ilk genişliği

    /* ─────────── Unity ─────────── */
    void Start()
    {
        /* 1) paneller – sadece current açık */
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(i == currentPanelIndex);

        /* 2) UI dinleyiciler */
        UpdateCoinsUI(PlayerDataManager.GetCoins());
        UpdateFuelUI(PlayerDataManager.GetFuel());
        PlayerDataManager.OnCoinsChanged += UpdateCoinsUI;
        PlayerDataManager.OnFuelChanged += UpdateFuelUI;

        /* 3) parent genişliklerini sakla + click ekle */
        parentRTs = new RectTransform[buttons.Length];
        originalWidths = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int idx = i;

            parentRTs[i] = buttons[i].transform.parent.GetComponent<RectTransform>();
            originalWidths[i] = parentRTs[i].sizeDelta.x;

            buttons[i].onClick.AddListener(() => MoveToPanel(idx));
        }

        UpdateButtonVisuals();   // ilk vurgulama
    }

    /* ─────────── Panel Geçişi ─────────── */
    public void MoveToPanel(int target)
    {
        if (target == currentPanelIndex || target < 0 || target >= panels.Length) return;

        panels[currentPanelIndex].SetActive(false);
        panels[target].SetActive(true);

        currentPanelIndex = target;
        UpdateButtonVisuals();
    }

    /* ─────────── Ölçek + BG genişliği ─────────── */
    void UpdateButtonVisuals()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            /* 1️⃣ BG genişliği sadece parent’ta */
            float targetW = (i == currentPanelIndex) ? selectedWidth : originalWidths[i];
            Vector2 size = parentRTs[i].sizeDelta; size.x = targetW;

            parentRTs[i].DOSizeDelta(size, animDuration).SetEase(widthEase);

            /* 2️⃣ Buton ölçeği (eski davranış) */
            float scale = (i == currentPanelIndex) ? buttonScaleSelected : buttonScaleDefault;
            buttons[i].transform.DOScale(scale, animDuration).SetEase(Ease.InOutCubic);
        }
    }

    private void UpdateCoinsUI(int coins)
    {
        coinsText.text = coins.ToString();
    }

    private void UpdateFuelUI(float fuel)
    {
        float maxFuel = 5f; // FuelSystem içinde de bu kullanılıyor
        int percent = Mathf.RoundToInt((fuel / maxFuel) * 100f);
        fuelText.text = fuel.ToString("F1");
        //fuelText.text = "%" + percent;
    }
}
