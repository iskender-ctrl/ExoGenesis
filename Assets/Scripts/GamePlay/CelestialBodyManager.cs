using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyManager : MonoBehaviour
{
    public CelestialBodyData data;
    private List<GameObject> celestialObjects = new List<GameObject>();
    private Dictionary<string, GameObject> celestialObjectMap = new Dictionary<string, GameObject>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            var bodyData = data.celestialBodies.Find(b => b.bodyName == child.name);
            if (bodyData != null)
            {
                celestialObjects.Add(child.gameObject);
                celestialObjectMap[child.name] = child.gameObject;

                // "CelestialBody" tag'ini ekle
                if (child.tag != "CelestialBody")
                {
                    child.tag = "CelestialBody";
                }

            }
        }
    }

    public List<GameObject> GetCelestialObjects()
    {
        return celestialObjects;
    }

    void FixedUpdate()
    {
        foreach (var obj in celestialObjects)
        {
            ApplyRotation(obj);
            ApplyOrbitalMotion(obj);
        }
    }

    private void ApplyRotation(GameObject obj)
    {
        var bodyData = data.celestialBodies.Find(b => b.bodyName == obj.name);
        if (bodyData != null)
        {
            obj.transform.Rotate(Vector3.up, bodyData.rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyOrbitalMotion(GameObject obj)
    {
        var bodyData = data.celestialBodies.Find(b => b.bodyName == obj.name);
        if (bodyData != null && !string.IsNullOrEmpty(bodyData.orbitAround))
        {
            if (celestialObjectMap.TryGetValue(bodyData.orbitAround, out var orbitCenter))
            {
                float distance = Vector3.Distance(obj.transform.position, orbitCenter.transform.position);
                float centralMass = data.celestialBodies.Find(b => b.bodyName == orbitCenter.name)?.mass ?? 0;

                if (centralMass > 0)
                {
                    float orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant * GravitySettings.OrbitalSpeedMultiplier * centralMass) / distance);
                    obj.transform.RotateAround(orbitCenter.transform.position, Vector3.up, orbitalSpeed * Time.deltaTime);
                }
            }
        }
    }
}
