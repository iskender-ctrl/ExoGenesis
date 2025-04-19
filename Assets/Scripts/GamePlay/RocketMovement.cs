using UnityEngine;

public class RocketMovement
{
    private Rigidbody rocketRigidbody;
    public RocketMovement(Rigidbody rocketRigidbody)
    {
        this.rocketRigidbody = rocketRigidbody;
    }
    public void ApplyPlanetGravity (CelestialBodyData.CelestialBody body, Vector3 gravityCenter, float intensityMultiplier = 1f)
    {
        Vector3 direction = (gravityCenter - rocketRigidbody.position).normalized;
        float distance = Vector3.Distance(gravityCenter, rocketRigidbody.position);
        float forceMagnitude = (GravitySettings.GravitationalConstant * body.mass) / (distance * distance);

        rocketRigidbody.AddForce(direction * forceMagnitude * intensityMultiplier);
        
    }
}