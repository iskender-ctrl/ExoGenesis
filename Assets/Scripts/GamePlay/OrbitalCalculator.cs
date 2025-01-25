using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCalculator : MonoBehaviour
{
    private Transform orbitCenter;
    private float distanceToCenter;
    private float orbitalSpeed;
    private CelestialBodyManager manager;
    private CelestialBodyData.CelestialBody bodyData;

    void Start()
    {
        GameObject centerObject = GameObject.Find(bodyData.orbitAround);
        if (centerObject != null)
        {
            orbitCenter = centerObject.transform;
        }
        if (orbitCenter != null)
        {
            distanceToCenter = Vector3.Distance(transform.position, orbitCenter.position);
            float centralMass = manager.data.celestialBodies.Find(b => b.bodyName == orbitCenter.name)?.mass ?? 0;
            orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant * GravitySettings.OrbitalSpeedMultiplier * centralMass) / distanceToCenter);
        }
    }

    void Update()
    {
        if (orbitCenter != null)
        {
            transform.RotateAround(orbitCenter.position, Vector3.up, orbitalSpeed * Time.deltaTime);
        }
    }

}

