using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMain : MonoBehaviour
{
    [Header("Yakıt uyarı paneli")]
    [SerializeField] private GameObject noFuelPanel;

    [Header("Yüklenecek sahne (Index ya da İsim)")]
    [SerializeField] private string sceneToLoad = "Level1"; // ya da "Gameplay"
    [SerializeField] private int sceneIndexToLoad = -1; // istersen index olarak da yükleyebilirsin

    public void LoadScene()
    {
        float fuel = PlayerDataManager.GetFuel();

        if (fuel <= 0f)
        {
            Debug.LogWarning("⛽ Yakıt yok! Oyun başlatılamaz.");
            if (noFuelPanel != null)
                noFuelPanel.SetActive(true); // Uyarı panelini aç
            return;
        }

        LoadTargetScene();
    }

    public void WatchAdForFuel()
    {
        Debug.Log("🎬 Reklam izleniyor, yakıt kazanılacak...");

        AdManager.Instance.ShowRewardedForFuel(() =>
        {
            PlayerDataManager.AddFuel(1f);
            Debug.Log("🎁 Reklam tamamlandı, 1 yakıt verildi!");
            if (noFuelPanel != null)
                noFuelPanel.SetActive(false);

            LoadTargetScene();
        });
    }

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
