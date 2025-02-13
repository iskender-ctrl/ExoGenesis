using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private Slider fuelBar; // UI'deki Slider bileşeni

    private int maxFuel = 5; // Toplam 5 birim yakıt
    public int currentFuel; // Mevcut yakıt seviyesi

    void Start()
    {
        currentFuel = maxFuel; // Başlangıçta full yakıt
        fuelBar.maxValue = maxFuel; // Slider'ın max değeri
        fuelBar.value = currentFuel; // Başlangıçta tam dolu göster
    }

    public bool UseFuel()
    {
        if (currentFuel > 0)
        {
            currentFuel--; // Yakıtı azalt
            fuelBar.value = currentFuel; // Progress barı güncelle
            return true; // Roket fırlatılabilir
        }
        else
        {
            Debug.Log("Yakıt tükendi! Roket fırlatılamaz.");
            return false; // Yakıt bitmiş, fırlatma yapılmaz
        }
    }
}
