using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class PlanetSelectorUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform planetsContainer;
    public List<RectTransform> planetButtons = new List<RectTransform>();
    public Button leftButton, rightButton;

    [Header("Popup")]
    public GameObject popupPanel;
    public Button playButton, decorateButton;
    public TextMeshProUGUI planetNameText;

    [Header("Settings")]
    public float spacing = 300f;
    public float selectedScale = 1.2f;
    public float defaultScale = 1f;
    public float transitionDuration = 0.3f;

    private int currentIndex = 0;

    private void Start()
    {
        leftButton.onClick.AddListener(() => Move(-1));
        rightButton.onClick.AddListener(() => Move(1));

        ApplyUnlockStates();
        UpdateUI();
        SetupButtonEvents();
    }

    private void SetupButtonEvents()
    {
        for (int i = 0; i < planetButtons.Count; i++)
        {
            int index = i;
            Button b = planetButtons[i].GetComponent<Button>();
            if (b != null)
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => OnPlanetClicked(index));
            }
        }
    }

    private void OnPlanetClicked(int index)
    {
        if (!planetButtons[index].GetComponent<Button>().interactable)
            return;

        currentIndex = index;
        UpdateUI();

        // Popup içeriğini güncelle
        popupPanel.SetActive(true);
        string planetName = planetButtons[index].name;
        planetNameText.text = planetName;

        playButton.onClick.RemoveAllListeners();
        decorateButton.onClick.RemoveAllListeners();

        playButton.onClick.AddListener(() => Debug.Log("Play clicked for " + planetName));
        decorateButton.onClick.AddListener(() =>
        {
            MapDecorationController.Instance.planetName = planetName;
            SceneManager.LoadSceneAsync(2);
        });
    }

    private void Move(int direction)
    {
        int newIndex = currentIndex + direction;
        if (newIndex < 0 || newIndex >= planetButtons.Count)
            return;

        currentIndex = newIndex;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < planetButtons.Count; i++)
        {
            float targetScale = (i == currentIndex) ? selectedScale : defaultScale;
            planetButtons[i].DOScale(targetScale, transitionDuration).SetEase(Ease.OutBack);
        }

        float targetX = -currentIndex * spacing;
        for (int i = 0; i < planetButtons.Count; i++)
        {
            float targetPosX = targetX + i * spacing;
            planetButtons[i].DOAnchorPosX(targetPosX, transitionDuration).SetEase(Ease.InOutCubic);
        }
    }

    private void ApplyUnlockStates()
    {
        int unlockedUpTo = PlayerPrefs.GetInt("UnlockedPlanetIndex", 0);

        for (int i = 0; i < planetButtons.Count; i++)
        {
            Button btn = planetButtons[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = i <= unlockedUpTo;
            }
        }
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
