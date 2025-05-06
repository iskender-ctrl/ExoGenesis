using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private Image fuelBar;
    [SerializeField] private int maxFuel = 50; // 🔧 Inspector'dan ayarlanabilir hale getirildi
    public System.Action OnFuelDepleted;

    private int currentFuel;

    void Start()
    {
        currentFuel = maxFuel;
        UpdateFuelBar();
    }

    public void UseFuel()
    {
        if (currentFuel > 0)
        {
            currentFuel--;
            UpdateFuelBar();
        }
    }

    public void AddFuel(int amount = 1)
    {
        currentFuel += amount;
        if (currentFuel > maxFuel)
            currentFuel = maxFuel;

        UpdateFuelBar();
    }

    private void UpdateFuelBar()
    {
        fuelBar.fillAmount = (float)currentFuel / maxFuel;

        if (currentFuel <= 0)
        {
            OnFuelDepleted?.Invoke();
        }
    }

    public bool HasFuel()
    {
        return currentFuel > 0;
    }
}
