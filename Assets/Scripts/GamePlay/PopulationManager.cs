using UnityEngine;
using TMPro;

public class PopulationManager : MonoBehaviour
{
    [Header("UI Referansı")]
    public TextMeshProUGUI populationText; // Popülasyonu gösterecek UI elementi

    private float maxPopulation; // Maksimum popülasyon kapasitesi (CelestialBodyData'dan alınacak)
    private float currentPopulation; // Mevcut popülasyon

    private void Start()
    {
        // Hedef gezegenin CelestialBody verisini al
        var targetBody = CelestialBodyHelper.FindBodyByName(gameObject.name);
        if (targetBody != null)
        {
            // maxPopulation değerini CelestialBodyData'dan al
            maxPopulation = targetBody.maxPopulation;

            // Başlangıçta UI'ı güncelle
            UpdatePopulationUI();
        }
    }

    // Popülasyonu artırma fonksiyonu
    public void IncreasePopulation(float amount)
    {
        // Popülasyonu artır
        currentPopulation += amount;

        // Maksimum popülasyonu aşma
        if (currentPopulation > maxPopulation)
        {
            currentPopulation = maxPopulation;
        }

        // UI'ı güncelle
        UpdatePopulationUI();

        // Level tamamlandı mı kontrol et
        if (currentPopulation >= maxPopulation)
        {
            //Debug.Log($"{gameObject.name} için Level Tamamlandı!");
        }
    }

    // UI'ı güncelleme fonksiyonu
    private void UpdatePopulationUI()
    {
        if (populationText != null)
        {
            // Popülasyon değerini UI'da güncelle
            
        }
    }
}