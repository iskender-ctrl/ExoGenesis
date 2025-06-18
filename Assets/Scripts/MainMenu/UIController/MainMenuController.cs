using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] panels; // Artık RectTransform değil, GameObject
    public Button[] buttons;
    public float buttonScaleSelected = 1.2f;
    public float buttonScaleDefault = 1.0f;
    public float buttonAnimationDuration = 0.3f;
    public TextMeshProUGUI fuelText, coinsText;

    private int currentPanelIndex = 1;

    void Start()
    {
        // Tüm panellerin aktifliğini ayarla
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentPanelIndex); // Sadece home aktif
        }

        UpdateCoinsUI(PlayerDataManager.GetCoins());
        UpdateFuelUI(PlayerDataManager.GetFuel());

        PlayerDataManager.OnCoinsChanged += UpdateCoinsUI;
        PlayerDataManager.OnFuelChanged += UpdateFuelUI;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => MoveToPanel(index));
        }

        UpdateButtonScales();
    }

    public void MoveToPanel(int targetPanelIndex)
    {
        if (targetPanelIndex < 0 || targetPanelIndex >= panels.Length)
        {
            Debug.LogWarning("Geçersiz panel indexi!");
            return;
        }

        if (targetPanelIndex == currentPanelIndex) return;

        // Panelleri aktif/inaktif yap
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == targetPanelIndex);
        }

        currentPanelIndex = targetPanelIndex;

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
        float maxFuel = 5f; // FuelSystem içinde de bu kullanılıyor
        int percent = Mathf.RoundToInt((fuel / maxFuel) * 100f);
        fuelText.text = fuel.ToString("F1"); 
        //fuelText.text = "%" + percent;
    }
}
