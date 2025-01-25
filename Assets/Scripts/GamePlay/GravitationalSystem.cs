using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalSystem : MonoBehaviour
{
    public CelestialBodyManager manager; // Merkezi sistem referansı

    void Start()
    {
        if (manager == null)
        {
            manager = FindFirstObjectByType<CelestialBodyManager>();
        }
    }

    void FixedUpdate()
    {
        var celestialObjects = manager.GetCelestialObjects();
        if (celestialObjects == null || celestialObjects.Count == 0)
        {
            return;
        }
        foreach (var obj in celestialObjects)
        {
            ApplyGravitationalForce(obj);
        }
    }

    private void ApplyGravitationalForce(GameObject obj)
    {
        var bodyData = manager.data.celestialBodies.Find(b => b.bodyName == obj.name);
        if (bodyData == null) return;

        foreach (var otherObj in manager.GetCelestialObjects())
        {
            if (obj == otherObj) continue;

            var otherBodyData = manager.data.celestialBodies.Find(b => b.bodyName == otherObj.name);
            if (otherBodyData == null) continue;

            Vector3 direction = otherObj.transform.position - obj.transform.position;
            float distance = direction.magnitude;

            if (distance > 0)
            {
                float forceMagnitude = (GravitySettings.GravitationalConstant * bodyData.mass * otherBodyData.mass) / (distance * distance);
                Vector3 force = direction.normalized * forceMagnitude;

                Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
                if (objRigidbody != null)
                {
                    objRigidbody.AddForce(force);
                }
            }
        }
    }
}