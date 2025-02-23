using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private Image fuelBar;
    
    private int maxFuel = 1;
    private int currentFuel;
    void Start()
    {
        currentFuel = maxFuel * 5;
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

    private void UpdateFuelBar()
    {
        fuelBar.fillAmount = (float)currentFuel / 5;
    }

    public bool HasFuel()
    {
        return currentFuel > 0;
    }
}
