using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;

public class PlanetSelectorUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform planetsContainer;
    public List<RectTransform> planetButtons = new List<RectTransform>();
    public Button decorateButton;
    public Button playButton;

    [Header("Progress Info")]
    public TextMeshProUGUI planetNameText;
    public Image progressFill;
    public TextMeshProUGUI progressText;

    [Header("Navigation Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Info Panel")]
    public GameObject planetClickPanel;

    [Header("Settings")]
    public float spacing = 300f;
    public float selectedScale = 1.2f;
    public float defaultScale = 1f;
    public float selectedY = 233.44f;
    public float defaultY = 0f;
    public float transitionDuration = 0.4f;
    public float rotationSpeed = 7f;

    [Header("Database")]
    public ClickablePlanetDatabase planetDatabase;

    private int currentIndex = 0;

    void Start()
    {
        SetupButtonEvents();
        ApplyUnlockStates();
        ScrollToPlanet(currentIndex); // İlk pozisyon
        RefreshUI();

        leftButton.onClick.AddListener(() =>
   {
       PlayClickFeedback(leftButton.transform);          // ✨ efekt
       int newIndex = Mathf.Clamp(currentIndex - 1, 0, planetButtons.Count - 1);
       if (newIndex != currentIndex)
       {
           currentIndex = newIndex;
           ScrollToPlanet(currentIndex);
           RefreshUI();
       }
   });

        rightButton.onClick.AddListener(() =>
        {
            PlayClickFeedback(rightButton.transform);         // ✨ efekt
            int newIndex = Mathf.Clamp(currentIndex + 1, 0, planetButtons.Count - 1);
            if (newIndex != currentIndex)
            {
                currentIndex = newIndex;
                ScrollToPlanet(currentIndex);
                RefreshUI();
            }
        });

        playButton.onClick.AddListener(() =>
        {
            Debug.Log("Play clicked for " + planetDatabase.planets[currentIndex].planetName);
            // SceneManager.LoadScene(...) çağrısı yapılabilir
        });

        decorateButton.onClick.AddListener(() =>
        {
            string selectedPlanet = planetDatabase.planets[currentIndex].planetName;
            MapDecorationController.Instance.planetName = selectedPlanet;
            SceneManager.LoadSceneAsync(2);
        });
    }
    private void Update()
    {
        if (SaveSystem.DataDirty)
        {
            SaveSystem.ClearDirty();  // bayrağı sıfırla
            RefreshUI();              // veriyi tekrar oku, barı güncelle
        }

        // Planets listesindeki tüm objeleri y ekseninde döndür
        foreach (var planetRect in planetButtons)
        {
            if (planetRect != null)
            {
                planetRect.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            }
        }
    }
    private void PlayClickFeedback(Transform btn)
    {
        const float clickScale = 0.8f;   // ne kadar küçülsün?
        const float clickTime = 0.07f;  // in-out süresi

        // Küçül → geri büyü sekansı
        btn.DOScale(clickScale, clickTime)
           .SetEase(Ease.InQuad)
           .OnComplete(() => btn.DOScale(1f, clickTime).SetEase(Ease.OutQuad));
    }
    void SetupButtonEvents()
    {
        for (int i = 0; i < planetButtons.Count; i++)
        {
            int index = i;
            Button b = planetButtons[i].GetComponent<Button>();
            if (b != null)
            {
                b.onClick.AddListener(() =>
                {
                    currentIndex = index;
                    ScrollToPlanet(index);
                    RefreshUI();
                });
            }
        }
    }

    void ScrollToPlanet(int index)
    {
        if (planetClickPanel != null)
            planetClickPanel.SetActive(false);

        for (int i = 0; i < planetButtons.Count; i++)
        {
            float offsetX = (i - index) * spacing;
            float targetY = (i == index) ? selectedY : defaultY;
            float targetScale = (i == index) ? selectedScale : defaultScale;

            planetButtons[i].DOAnchorPos(new Vector2(offsetX, targetY), transitionDuration).SetEase(Ease.OutCubic);
            planetButtons[i].DOScale(targetScale, transitionDuration).SetEase(Ease.OutBack);
        }

        DOVirtual.DelayedCall(transitionDuration, () =>
        {
            if (planetClickPanel != null)
                planetClickPanel.SetActive(true);
        });
    }

    public void RefreshUI()
    {
        var data = planetDatabase.planets[currentIndex];
        planetNameText.text = data.planetName;

        float progress = PlanetProgressUtil.GetProgress(data);
        progressFill.fillAmount = progress;
        progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

        // Yeni kontrol: planet button aktifse, alt butonlar da aktif
        bool planetInteractable = planetButtons[currentIndex].GetComponent<Button>().interactable;

        decorateButton.interactable = planetInteractable;
        playButton.interactable = planetInteractable;
    }

    void ApplyUnlockStates()
    {
        // PlayerDataManager.GetLevel()  => 1, 2, 3 …   (1-den başlıyorsa)
        int playerLevel = Mathf.Max(PlayerDataManager.GetLevel(), 1);

        for (int i = 0; i < planetButtons.Count; i++)
        {
            Button btn = planetButtons[i].GetComponent<Button>();
            if (btn == null) continue;

            // Örnek: level 1 → yalnızca index 0 (ilk gezegen) açık
            //        level 2 → index 0 ve 1 açık
            btn.interactable = (i < playerLevel);
        }

        // Scroll pozisyonu seçili gezegen kilitliyse ilk açık olana dön
        if (!planetButtons[currentIndex].GetComponent<Button>().interactable)
        {
            SelectPlanetByScroll(playerLevel - 1);  // son açık gezegene zıpla
        }
    }


    bool IsPlanetUnlocked(int index)
    {
        int unlockedUpTo = PlayerPrefs.GetInt("UnlockedPlanetIndex", 0);
        return index <= unlockedUpTo;
    }

    public static void UnlockNextPlanet(int currentIndex)
    {
        int saved = PlayerPrefs.GetInt("UnlockedPlanetIndex", 0);
        if (currentIndex >= saved)
        {
            PlayerPrefs.SetInt("UnlockedPlanetIndex", currentIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public void SelectPlanetByScroll(int index)
    {
        if (index >= 0 && index < planetButtons.Count)
        {
            currentIndex = index;
            ScrollToPlanet(index);
            RefreshUI();
        }
    }
}
