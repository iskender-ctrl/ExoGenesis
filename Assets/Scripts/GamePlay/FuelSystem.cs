using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private Image fuelBar; // UI'deki doluluk göstergesi (Image)
    
    private int maxFuel = 1; // Maksimum 5 birim yakıt
    private int currentFuel; // Mevcut yakıt miktarı

    void Start()
    {
        currentFuel = maxFuel * 5; // Başlangıçta full yakıt
        UpdateFuelBar(); // İlk başta barı güncelle
    }

    public void UseFuel()
    {
        if (currentFuel > 0)
        {
            currentFuel--; // Yakıtı azalt
            UpdateFuelBar(); // Progress barı güncelle
        }
    }

    private void UpdateFuelBar()
    {
        fuelBar.fillAmount = (float)currentFuel / 5; // Doğru float bölme işlemi
        Debug.Log(fuelBar.fillAmount.ToString());
    }

    public bool HasFuel()
    {
        return currentFuel > 0; // Yakıt olup olmadığını kontrol eden fonksiyon
    }
}
