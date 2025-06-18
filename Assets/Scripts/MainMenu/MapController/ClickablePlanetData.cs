using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetDatabase", menuName = "Planet/Create Planet Database")]
public class ClickablePlanetDatabase : ScriptableObject
{
    // ────────────────────────────────── Dekorasyon ──────────────────────────────────
    [System.Serializable]
    public class DecorationItem
    {
        public string decorationName;
        public Sprite icon;
        public int requiredPopulation;          // Dekorasyonun açılması için min. nüfus
    }

    // ────────────────────────────────── Gezegen Verisi ──────────────────────────────
    [System.Serializable]
    public class PlanetData
    {
        [Header("Basic")]
        public string planetName;
        public Sprite bG;

        [Header("Population")]
        public int defaultPopulation = 0;       // Oyuna ilk girişteki nüfus
        public int currentPopulation = 0;       // Runtime’da SaveSystem’den yüklenir
        public int targetPopulation  = 100;     // 🌟 Level atlamak / % hesabı için üst sınır

        [Header("Evolution")]
        public int currentEvolutionStage = 0;   // 🌟 Gezegenin yeşillenme seviyesi
        public int maxEvolutionStage     = 5;   // 🌟 Maksimum evrim basamağı (0-max)

        [Header("Decorations")]
        public List<DecorationItem> items = new(); // Dekorasyon listesi
    }

    // ────────────────────────────────────────────────────────────────────────────────
    public List<PlanetData> planets = new();
}
