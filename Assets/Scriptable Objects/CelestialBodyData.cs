using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCelestialBodyCollection", menuName = "CelestialBody/Collection")]
public class CelestialBodyData : ScriptableObject
{
    [System.Serializable]
    public class CelestialBody
    {
        public string bodyName; // Gökcisminin adı
        public float mass; // Kütle
        public float rotationSpeed; // Gezegenin kendi etrafındaki dönüş hızı
        public GameObject prefab; // Gökcisminin prefabı
        public string orbitAround; // Gezegenin etrafında döneceği gökcismi adı
    }
    public List<CelestialBody> celestialBodies = new List<CelestialBody>(); // Gök cisimlerinin listesi
}
