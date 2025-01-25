using UnityEngine;

public class GravitySettings : MonoBehaviour
{
    [Header("Yerçekimi Ayarları")]
    [Tooltip("Yerçekimi sabiti. Çekim kuvveti hesaplamaları için kullanılır.")]
    public float gravitationalConstant = 0.1f;

    [Tooltip("Görsel çekim alanlarının boyutunu kontrol eder.")]
    public float radiusMultiplier = 1f;

    [Tooltip("Yörüngesel hız hesaplamaları için çarpan.")]
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
        else
        {
            Destroy(gameObject); // Birden fazla GravitySettings varsa yok et
        }
    }

    // Global erişim için sabitlere kolay erişim sağlayan property'ler
    public static float GravitationalConstant => Instance.gravitationalConstant;
    public static float RadiusMultiplier => Instance.radiusMultiplier;
    public static float OrbitalSpeedMultiplier => Instance.orbitalSpeedMultiplier;
}
