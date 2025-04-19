using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyManager : MonoBehaviour
{
    private List<GameObject> celestialObjects = new List<GameObject>();
    private Dictionary<string, GameObject> celestialObjectMap = new Dictionary<string, GameObject>();

    void FixedUpdate()
    {
        // Gökcisimlerinin hareketlerini uygula
        foreach (var obj in celestialObjects)
        {
            ApplyRotation(obj);
            ApplyOrbitalMotion(obj);
        }
    }

    private void ApplyRotation(GameObject obj)
    {
        // Gökcisminin kendi ekseninde dönüşünü uygula
        var bodyData = CelestialBodyHelper.FindBodyByName(obj.name);
        if (bodyData != null)
        {
            obj.transform.Rotate(Vector3.up, bodyData.rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyOrbitalMotion(GameObject obj)
    {
        // Yörüngesel hareketi uygula
        var bodyData = CelestialBodyHelper.FindBodyByName(obj.name);
        if (bodyData != null && !string.IsNullOrEmpty(bodyData.orbitAround))
        {
            if (celestialObjectMap.TryGetValue(bodyData.orbitAround, out var orbitCenter))
            {
                float distance = Vector3.Distance(obj.transform.position, orbitCenter.transform.position);
                float centralMass = CelestialBodyHelper.FindBodyByName(orbitCenter.name)?.mass ?? 0;

                if (centralMass > 0)
                {
                    float orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant * GravitySettings.OrbitalSpeedMultiplier * centralMass) / distance);
                    obj.transform.RotateAround(orbitCenter.transform.position, Vector3.up, orbitalSpeed * Time.deltaTime);
                }
            }
        }
    }
}
