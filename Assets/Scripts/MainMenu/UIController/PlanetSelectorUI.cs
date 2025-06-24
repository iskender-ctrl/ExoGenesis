using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlanetSelectorUI : MonoBehaviour,
                                 IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /*───────────────────────────── Inspector Bağlantıları ─────────────────────────────*/
    [Header("References")]
    public RectTransform planetsContainer;          // Gezegen butonlarının parent'ı
    public List<RectTransform> planetButtons;       // Her gezegen butonu (RectTransform)
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

    /*───────────────────────────── Ayarlar ─────────────────────────────*/
    [Header("Layout Settings")]
    public float spacing = 300f;          // Butonlar arası mesafe
    public float selectedScale = 1.2f;
    public float defaultScale = 1f;
    public float selectedY = 233.44f;
    public float defaultY = 0f;
    public float transitionDuration = 0.4f;
    public float rotationSpeed = 7f;

    [Header("Swipe Settings")]
    public float swipeThreshold = 80f;    // Piksel – bu mesafeden büyük sürükleme sayfa çevirir
    public float dragFollowFactor = 0.5f; // Sürüklerken hareket hızı (0–1 arası)

    [Header("Database")]
    public ClickablePlanetDatabase planetDatabase;

    /*───────────────────────────── Runtime ─────────────────────────────*/
    private int currentIndex;
    private Vector2 dragStartPos;
    private float containerStartX;
    private bool isDragging;

    /*───────────────────────────── Unity Life-cycle ─────────────────────────────*/
    private void Start()
    {
        SetupButtonEvents();
        ApplyUnlockStates();

        currentIndex = Mathf.Clamp(currentIndex, 0, planetButtons.Count - 1);
        ScrollToPlanet(currentIndex, 0f);   // Başlangıç yerleşimi
        RefreshUI();

        leftButton.onClick.AddListener(() => MoveByButton(-1));
        rightButton.onClick.AddListener(() => MoveByButton(+1));

        playButton.onClick.AddListener(() =>
        {
            Debug.Log("Play clicked for " + planetDatabase.planets[currentIndex].planetName);
            // SceneManager.LoadScene(...)  // kendi oynanış sahneni burada çağırabilirsin
        });

        decorateButton.onClick.AddListener(() =>
        {
            string planetName = planetDatabase.planets[currentIndex].planetName;
            MapDecorationController.Instance.planetName = planetName;
            SceneManager.LoadSceneAsync(2);  // Decoration sahnesi
        });
    }

    private void Update()
    {
        // SaveSystem DataDirty → bar / ilerleme yeniden çizilsin
        if (SaveSystem.DataDirty)
        {
            SaveSystem.ClearDirty();
            RefreshUI();
        }

        // Bütün gezegen görsellerini kendi ekseninde döndür
        foreach (var pr in planetButtons)
            pr.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    /*───────────────────────────── Swipe (Drag) Kontrolleri ───────────────────────────*/
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPos = eventData.position;
        containerStartX = planetsContainer.anchoredPosition.x;

        if (planetClickPanel) planetClickPanel.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        float deltaX = eventData.position.x - dragStartPos.x;
        planetsContainer.anchoredPosition = new Vector2(
            containerStartX + deltaX * dragFollowFactor,
            planetsContainer.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        float deltaX = eventData.position.x - dragStartPos.x;

        if (Mathf.Abs(deltaX) >= swipeThreshold)
        {
            int dir = deltaX > 0 ? -1 : +1;
            currentIndex = Mathf.Clamp(currentIndex + dir, 0, planetButtons.Count - 1);
        }

        /* ----- Kritik düzeltme: container'ı merkeze çek ------ */
        planetsContainer.anchoredPosition = Vector2.zero;
        /* ----------------------------------------------------- */

        ScrollToPlanet(currentIndex);
        RefreshUI();
    }


    /*───────────────────────────── Ok Düğmeleri ─────────────────────────────*/
    private void MoveByButton(int step)
    {
        int newIndex = Mathf.Clamp(currentIndex + step, 0, planetButtons.Count - 1);
        if (newIndex == currentIndex) return;

        PlayClickFeedback(step < 0 ? leftButton.transform : rightButton.transform);
        currentIndex = newIndex;
        ScrollToPlanet(currentIndex);
        RefreshUI();
    }

    private void PlayClickFeedback(Transform btn)
    {
        btn.DOComplete();
        btn.DOPunchScale(Vector3.one * -0.2f, 0.15f, 1, 0f);
    }

    /*───────────────────────────── Ana Yerleşim / Animasyon ──────────────────────────*/
    private void ScrollToPlanet(int index, float durationOverride = -1f)
    {
        float dur = durationOverride >= 0f ? durationOverride : transitionDuration;

        for (int i = 0; i < planetButtons.Count; i++)
        {
            float offsetX = (i - index) * spacing;
            float targetY = i == index ? selectedY : defaultY;
            float targetScale = i == index ? selectedScale : defaultScale;

            planetButtons[i]
                .DOAnchorPos(new Vector2(offsetX, targetY), dur)
                .SetEase(Ease.OutCubic);

            planetButtons[i]
                .DOScale(targetScale, dur)
                .SetEase(Ease.OutBack);
        }

        DOVirtual.DelayedCall(dur, () =>
        {
            if (planetClickPanel) planetClickPanel.SetActive(true);
        });
        FirebaseEventManager.LogPlanetSelected(planetDatabase.planets[index].planetName);
    }

    /*───────────────────────────── UI Güncelleme ─────────────────────────────*/
    public void RefreshUI()
    {
        var data = planetDatabase.planets[currentIndex];

        planetNameText.text = data.planetName;

        float prog = PlanetProgressUtil.GetProgress(data);
        progressFill.fillAmount = prog;
        progressText.text = Mathf.RoundToInt(prog * 100f) + "%";

        bool interactable = planetButtons[currentIndex].GetComponent<Button>().interactable;
        decorateButton.interactable = interactable;
        playButton.interactable = interactable;
    }

    /*───────────────────────────── Başlangıç Ayarları ───────────────────────────────*/
    private void SetupButtonEvents()
    {
        for (int i = 0; i < planetButtons.Count; i++)
        {
            int idx = i;
            Button b = planetButtons[i].GetComponent<Button>();
            if (b == null) continue;

            b.onClick.AddListener(() =>
            {
                currentIndex = idx;
                ScrollToPlanet(idx);
                RefreshUI();
            });
        }
    }

    private void ApplyUnlockStates()
    {
        int playerLevel = Mathf.Max(PlayerDataManager.GetLevel(), 1);

        for (int i = 0; i < planetButtons.Count; i++)
        {
            Button btn = planetButtons[i].GetComponent<Button>();
            if (btn) btn.interactable = i < playerLevel;
        }

        if (!planetButtons[currentIndex].GetComponent<Button>().interactable)
            SelectPlanetByScroll(playerLevel - 1);
    }

    /*───────────────────────────── Helper Fonksiyonlar ──────────────────────────────*/
    public void SelectPlanetByScroll(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, planetButtons.Count - 1);
        ScrollToPlanet(currentIndex);
        RefreshUI();
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
}
