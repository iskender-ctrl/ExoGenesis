using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMain : MonoBehaviour
{
    /*────────────────── Inspector ──────────────────*/
    [Header("Yakıt uyarı paneli")]
    [SerializeField] private GameObject noFuelPanel;   // içerik
    [SerializeField] private GameObject popUPBG;       // karartma / BG

    [Header("Yüklenecek sahne (Index ya da İsim)")]
    [SerializeField] private string sceneToLoad = "Level1";
    [SerializeField] private int sceneIndexToLoad = -1;
    [SerializeField] PopupManager popupManager;
    /*────────────────── FPS sabitle ────────────────*/
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    /*────────────────── Public API ────────────────*/
    public void LoadScene()
    {
        if (PlayerDataManager.GetFuel() <= 0f)
        {
            Debug.LogWarning("⛽ Yakıt yok! Oyun başlatılamaz.");
            ShowNoFuelPopup();
            return;
        }

        LoadTargetScene();
    }

    public void WatchAdForFuel()
    {
        Debug.Log("🎬 Reklam izleniyor, yakıt kazanılacak…");

        AdManager.Instance.ShowRewardedForFuel(() =>
        {
            PlayerDataManager.AddFuel(1f);
            Debug.Log("🎁 Reklam tamamlandı, 1 yakıt verildi!");
            HideNoFuelPopup();
            LoadTargetScene();
            FirebaseEventManager.LogAdWatched("rewarded");
        });
    }

    /*────────────────── Popup helper’ları ─────────*/
    private void ShowNoFuelPopup()
    {
        // Her ihtimale karşı önce kapat, sonra aç
        if (noFuelPanel) popupManager.OpenPopup(noFuelPanel.transform);
        if (popUPBG) popUPBG.SetActive(true);
    }

    /// <summary>“X / Kapat” butonuna bunu bağla</summary>
    public void HideNoFuelPopup()
    {
        if (noFuelPanel) popupManager.ClosePopup(noFuelPanel.transform);
        if (popUPBG) popUPBG.SetActive(false);
    }

    /*────────────────── Sahne helper’ı ────────────*/
    private void LoadTargetScene()
    {
        if (sceneIndexToLoad >= 0)
            SceneManager.LoadSceneAsync(sceneIndexToLoad);
        else if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadSceneAsync(sceneToLoad);
        else
            Debug.LogError("Yüklenecek sahne belirtilmemiş!");
    }
}
