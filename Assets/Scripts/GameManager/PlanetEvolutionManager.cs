using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Seçili gezegenin “yaşam / gelişim” yüzdesini gösterir.
/// Hesap: PlanetProgressUtil (nüfus + dekorasyon + evrim, ağırlıklı).
/// </summary>
public class PlanetEvolutionManager : MonoBehaviour
{
    /* ───────────── UI ───────────── */
    [Header("UI (Inspector’dan atayabilir ya da Tag ile bulunur)")]
    [SerializeField] private Image lifeProgressFill;   // Bar
    [SerializeField] private TextMeshProUGUI lifeProgressText;   // % metin

    /* ───────────── Data ─────────── */
    [Header("Databases")]
    [SerializeField] private ClickablePlanetDatabase planetDatabase;

    /* ─────────── Görsel Root ────── */
    [Header("Scene Ref - Gezegen")]
    [SerializeField] private Transform planetVisualRoot;         // Gezegen prefab kökü

    private int _planetIndex;

    /*────────────── A P I ─────────────*/
    /// <summary>Hangi gezegen gösterilecek?</summary>
    public void SetPlanetIndex(int index)
    {
        _planetIndex = Mathf.Clamp(index, 0, planetDatabase.planets.Count - 1);
        RefreshUI();
    }

    private void Start() => RefreshUI();   // Sahne açıldığında bir kez
    private void Update()
    {
        if (SaveSystem.DataDirty)
        {
            SaveSystem.ClearDirty(); // Flag sıfırla
            RefreshUI();             // UI güncelle
        }
    }


    /*────────────── U I  G Ü N C E L L E ─────────────*/
    public void RefreshUI()
    {
        /* ---- Lazy-find: Inspector’dan atanmamışsa Tag ile bul ---- */
        if (!lifeProgressFill)
        {
            var go = GameObject.FindGameObjectWithTag("DecorationFill");
            if (go) lifeProgressFill = go.GetComponent<Image>();
        }
        if (!lifeProgressText)
        {
            var go = GameObject.FindGameObjectWithTag("DecorationProgressText");
            if (go) lifeProgressText = go.GetComponent<TextMeshProUGUI>();
        }
        /* --------------------------------------------------------- */

        if (planetDatabase == null || planetDatabase.planets.Count == 0) return;

        var pData = planetDatabase.planets[_planetIndex];

        /* 1) Güncel veriyi SaveSystem’den çek (nüfus & evrim) */
        pData.currentPopulation = SaveSystem.GetPopulation(pData.planetName, pData.currentPopulation);
        pData.currentEvolutionStage = SaveSystem.GetEvolutionStage(pData.planetName);

        /* 2) Ağırlıklı ilerleme skoru */
        float score = PlanetProgressUtil.GetProgress(pData);   // 0-1

        /* 3) UI yaz-çiz */
        if (lifeProgressFill) lifeProgressFill.fillAmount = score;
        if (lifeProgressText) lifeProgressText.text = Mathf.RoundToInt(score * 100f) + "%";

        /* 4) Gezegenin görsel evrimini uygula (varsa) */
        /*if (planetVisualRoot && PlanetLifeProgressManager.Instance)
            PlanetLifeProgressManager.Instance.ApplyVisuals(pData.planetName, planetVisualRoot);*/
    }
}
