using UnityEngine;

public class RocketCollisionHandler
{
    private Rigidbody rocketRigidbody;
    private RocketBehavior rocketBehavior;
    private RocketMovement rocketMovement;

    public RocketCollisionHandler(RocketBehavior rocketBehavior, RocketMovement rocketMovement)
    {
        this.rocketBehavior = rocketBehavior;
        this.rocketRigidbody = rocketBehavior.GetComponent<Rigidbody>();
        this.rocketMovement = rocketMovement;
    }

    public void HandleTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "TargetField":
                //
                break;
            case "CelestialBody":
                rocketBehavior.InstantiateExplosion(rocketRigidbody.position); // Patlama efekti
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
        else if (other.CompareTag("TargetField"))
        {
            // Hedef alanı işlemleri
        }
    }

    public void HandleCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Target"))
    {
        //
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