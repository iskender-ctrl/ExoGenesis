using UnityEngine;
using System.Collections.Generic;

public class AstronautManager : MonoBehaviour
{
    [Header("Astronaut Options")]
    public GameObject astronautPrefab;
    public Transform spawnAreaCenter;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f); // X-Z düzleminde

    private List<GameObject> spawnedAstronauts = new();

    public void SpawnAstronauts(int populationCount)
    {
        // Önceki astronotları temizle
        foreach (var astro in spawnedAstronauts)
            Destroy(astro);

        spawnedAstronauts.Clear();

        for (int i = 0; i < populationCount; i++)
        {
            Vector3 randomPos = spawnAreaCenter.position + new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                0,
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
            );

            GameObject astro = Instantiate(astronautPrefab, randomPos, Quaternion.identity, spawnAreaCenter);
            astro.name = $"Astronaut_{i}";
            spawnedAstronauts.Add(astro);
        }
    }

    public void ClearAstronauts()
    {
        foreach (var astro in spawnedAstronauts)
            Destroy(astro);

        spawnedAstronauts.Clear();
    }
}
