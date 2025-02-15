    using UnityEngine;
    using DG.Tweening; // DoTween kütüphanesi için gerekli
    using UnityEngine.UI;
    using TMPro;

    public class MainMenuController : MonoBehaviour
    {
        public RectTransform[] panels; // UI panelleri
        public float transitionDuration = 0.5f; // Sayfa geçiş süresi
        public float panelSpacing = 1920f; // Panellerin birbirine göre yatay mesafesi
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
                    .SetEase(Ease.InOutCubic); // Kayma animasyonu
            }

            currentPanelIndex = targetPanelIndex; // Şu anki paneli güncelle
        }

        private void UpdateCoinsUI(int coins)
        {
            coinsText.text = "Coins: " + coins;
        }

        private void UpdateFuelUI(float fuel)
        {
            fuelText.text = "Fuel: " + fuel.ToString("F1");
        }
    }
