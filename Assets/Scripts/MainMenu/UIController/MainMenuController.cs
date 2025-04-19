using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public RectTransform[] panels; // UI panelleri
    public Button[] buttons; // Panel butonları
    public float transitionDuration = 0.5f; // Sayfa geçiş süresi
    public float panelSpacing = 1920f; // Panellerin birbirine göre yatay mesafesi
    public float buttonScaleSelected = 1.2f; // Seçili buton büyüklüğü
    public float buttonScaleDefault = 1.0f; // Varsayılan buton büyüklüğü
    public float buttonAnimationDuration = 0.3f; // Buton büyüme/küçülme süresi
    public TextMeshProUGUI fuelText, coinsText;

    private int currentPanelIndex = 0; // Hangi panelde olduğumuzu takip ederiz

    void Start()
    {
        // Tüm panelleri doğru pozisyona ayarla
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].anchoredPosition = new Vector2(i * panelSpacing, 0);
        }

        // UI'yı PlayerDataManager'dan güncelle
        UpdateCoinsUI(PlayerDataManager.GetCoins());
        UpdateFuelUI(PlayerDataManager.GetFuel());

        // Eventleri bağla
        PlayerDataManager.OnCoinsChanged += UpdateCoinsUI;
        PlayerDataManager.OnFuelChanged += UpdateFuelUI;

        // Butonlara tıklanınca ilgili sayfaya geçmeleri için event ekle
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Lambda expression için index'i saklıyoruz
            buttons[i].onClick.AddListener(() => MoveToPanel(index));
        }

        // İlk butonu seçili olarak büyüt
        UpdateButtonScales();
    }

    public void MoveToPanel(int targetPanelIndex)
    {
        if (targetPanelIndex < 0 || targetPanelIndex >= panels.Length)
        {
            Debug.LogWarning("Geçersiz panel indexi!");
            return;
        }

        if (targetPanelIndex == currentPanelIndex) return; // Zaten o paneldeyse hiçbir şey yapma

        // Hedef pozisyonu hesapla
        float targetX = -targetPanelIndex * panelSpacing;

        // Tüm panelleri kaydır
        foreach (RectTransform panel in panels)
        {
            panel.DOAnchorPosX(panel.anchoredPosition.x + (currentPanelIndex - targetPanelIndex) * panelSpacing, transitionDuration)
                .SetEase(Ease.InOutCubic);
        }

        currentPanelIndex = targetPanelIndex; // Şu anki paneli güncelle

        // Buton animasyonlarını güncelle
        UpdateButtonScales();
    }

    private void UpdateButtonScales()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == currentPanelIndex)
            {
                buttons[i].transform.DOScale(buttonScaleSelected, buttonAnimationDuration).SetEase(Ease.OutBack);
            }
            else
            {
                buttons[i].transform.DOScale(buttonScaleDefault, buttonAnimationDuration).SetEase(Ease.InOutCubic);
            }
        }
    }

    private void UpdateCoinsUI(int coins)
    {
        coinsText.text = coins.ToString();
    }

    private void UpdateFuelUI(float fuel)
    {
        fuelText.text = fuel.ToString("F1");
    }
}
