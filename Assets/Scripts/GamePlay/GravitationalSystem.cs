using UnityEngine;

public class GravitationalSystem : MonoBehaviour
{
    void FixedUpdate()
    {
        foreach (var obj in GameManager.Instance.spaceObjectsParent.GetComponentsInChildren<Transform>())
        {
            foreach (var other in GameManager.Instance.spaceObjectsParent.GetComponentsInChildren<Transform>())
            {
                if (obj == other) continue;
                
                ApplyGravitationalForce(obj.gameObject, other.gameObject);
            }
        }
    }

    private void ApplyGravitationalForce(GameObject obj, GameObject other)
    {
        var body1 = CelestialBodyHelper.FindBodyByName(obj.name);
        var body2 = CelestialBodyHelper.FindBodyByName(other.name);

        if (body1 == null || body2 == null) return;

        Vector3 direction = other.transform.position - obj.transform.position;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            float forceMagnitude = (GravitySettings.Instance.gravitationalConstant * body1.mass * body2.mass) / (distance * distance);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction.normalized * forceMagnitude);
            }
        }
    }
}
