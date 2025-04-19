using UnityEngine;

public class RocketCollisionHandler
{
    private Rigidbody rocketRigidbody;
    private RocketBehavior rocketBehavior;
    private RocketMovement rocketMovement;
    private RocketLanding rocketLanding;

    public float populationIncreaseAmount = 10f;



    public RocketCollisionHandler(RocketBehavior rocketBehavior, RocketMovement rocketMovement)
    {
        this.rocketBehavior = rocketBehavior;
        this.rocketRigidbody = rocketBehavior.GetComponent<Rigidbody>();
        this.rocketMovement = rocketMovement;
        this.rocketLanding = rocketBehavior.GetComponent<RocketLanding>();
    }



    public void HandleTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Target":

                break;

            case "TargetField":
                var planetTransform = other.transform.parent;
                if (rocketLanding != null)
                {
                    float planetRadius = planetTransform.localScale.x * 0.5f;
                    float gravityFieldRadius = other.GetComponent<SphereCollider>().radius * planetTransform.localScale.x;
                    rocketLanding.InitializeLanding(planetRadius, gravityFieldRadius, planetTransform);
                    LevelManager.Instance.LevelCompleted();
                }
                break;

            case "CelestialBody":
                rocketBehavior.InstantiateExplosion(rocketRigidbody.position);
                DestroyRocket();
                break;
        }
    }

    public void HandleTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            var body = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
            if (body != null)
            {
                rocketMovement.ApplyPlanetGravity(body, other.transform.position);
            }
        }
    }

    public void HandleCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            // Roket fiziksel olarak hedefe çarptıysa da inişi durdur
            if (rocketLanding != null)
            {
                rocketLanding.StopLanding();
            }
        }
        else if (collision.gameObject.CompareTag("CelestialBody"))
        {
            DestroyRocket();
        }
    }

    private void DestroyRocket()
    {
        if (rocketRigidbody != null)
        {
            rocketRigidbody.isKinematic = false;
            rocketRigidbody.linearVelocity = Vector3.zero;
            rocketRigidbody.angularVelocity = Vector3.zero;
            rocketRigidbody.isKinematic = true;
        }

        rocketBehavior.gameObject.SetActive(false);
    }
}
