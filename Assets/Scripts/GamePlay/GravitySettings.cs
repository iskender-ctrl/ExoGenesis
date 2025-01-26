using UnityEngine;

public class GravitySettings : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravitationalConstant = 0.1f;
    public float radiusMultiplier = 1f;
    public float orbitalSpeedMultiplier = 1f;

    // Singleton instance
    public static GravitySettings Instance;

    private void Awake()
    {
        // Singleton yapı
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Global erişim için sabitlere kolay erişim sağlayan property'ler
    public static float GravitationalConstant => Instance.gravitationalConstant;
    public static float RadiusMultiplier => Instance.radiusMultiplier;
    public static float OrbitalSpeedMultiplier => Instance.orbitalSpeedMultiplier;
}
