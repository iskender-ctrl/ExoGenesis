using System.Linq;
using UnityEngine;

public static class PlanetProgressUtil
{
    private static ProgressWeightsSO _weights;

    private static ProgressWeightsSO Weights
    {
        get
        {
            if (_weights == null)
                _weights = Resources.Load<ProgressWeightsSO>("ProgressWeights");
            return _weights;
        }
    }

    public static float GetProgress(ClickablePlanetDatabase.PlanetData pd)
    {
        if (pd == null)
        {
            Debug.LogWarning("PlanetData is null!");
            return 0f;
        }

        // --- 1. Nüfus faktörü
        float popScore = Mathf.Clamp01(
            pd.targetPopulation == 0 ? 0f :
            (float)pd.currentPopulation / pd.targetPopulation);

        // --- 2. Dekorasyon faktörü
        int totalItems = pd.items?.Count ?? 0;
        int activeCount = 0;
        if (totalItems > 0)
        {
            foreach (var item in pd.items)
            {
                if (SaveSystem.IsObjectActive(item.decorationName))
                    activeCount++;
            }
        }
        float decoScore = (totalItems == 0) ? 0f : (float)activeCount / totalItems;

        // --- 3. Evrim faktörü
        float evoScore = Mathf.Clamp01(
            pd.maxEvolutionStage == 0 ? 0f :
            (float)pd.currentEvolutionStage / pd.maxEvolutionStage);

        // --- 4. Ağırlıklı ortalama
        float w1 = Weights.population;
        float w2 = Weights.decoration;
        float w3 = Weights.evolution;

        float totalScore = popScore * w1 + decoScore * w2 + evoScore * w3;

        return Mathf.Clamp01(totalScore);
    }
}
