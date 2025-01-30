using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Rigidbody rocketRigidbody;
    private bool isInTargetField;
    private Transform currentTarget;
    private float initialDistance;
    private float spiralSpeed = 4f;
    private Vector3 orbitDirection; // Yörünge yönü için

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
        
        if(isInTargetField && currentTarget != null)
        {
            ApplySpiralMotion();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetField"))
        {
            isInTargetField = true;
            currentTarget = other.transform.parent;
            initialDistance = Vector3.Distance(transform.position, currentTarget.position);
            CalculateOrbitDirection(); // Yörünge yönü hesapla
        }
        if (other.CompareTag("CelestialBody"))
        {
            DestroyRocket();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TargetField"))
        {
            isInTargetField = false;
            currentTarget = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            HandleStandardGravity(other);
        }
        else if (other.CompareTag("TargetField"))
        {
            HandleTargetGravity(other);
        }
    }

    private void CalculateOrbitDirection()
    {
        if(currentTarget == null) return;

        Vector3 relativePosition = transform.position - currentTarget.position;
        Vector3 relativeVelocity = rocketRigidbody.linearVelocity;
        
        // Hız vektörüne göre yön belirleme
        float directionSign = Mathf.Sign(Vector3.Cross(relativePosition, relativeVelocity).y);
        orbitDirection = (directionSign <= 0) ? Vector3.up : -Vector3.up;
    }

    private void HandleStandardGravity(Collider other)
    {
        var body = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
        if (body != null)
        {
            ApplyGravitationalForce(body, other.transform.position);
        }
    }

    private void HandleTargetGravity(Collider other)
    {
        var targetBody = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
        if (targetBody != null)
        {
            ApplyGravitationalForce(targetBody, other.transform.position, 0.5f);
            StabilizeOrbit(targetBody, other.transform.position);
        }
    }

    private void ApplyGravitationalForce(CelestialBodyData.CelestialBody body, Vector3 gravityCenter, float intensityMultiplier = 1f)
    {
        Vector3 direction = (gravityCenter - transform.position).normalized;
        float distance = Vector3.Distance(gravityCenter, transform.position);
        float forceMagnitude = (GravitySettings.Instance.gravitationalConstant * body.mass) / (distance * distance);
        Vector3 force = direction * forceMagnitude * intensityMultiplier;
        rocketRigidbody.AddForce(force);
    }

    private void StabilizeOrbit(CelestialBodyData.CelestialBody body, Vector3 gravityCenter)
    {
        float distance = Vector3.Distance(gravityCenter, transform.position);
        float orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant * body.mass) / distance);

        // Dinamik yön kullanımı
        Vector3 tangentDirection = Vector3.Cross(transform.position - gravityCenter, orbitDirection).normalized;
        rocketRigidbody.linearVelocity = Vector3.Lerp(
            rocketRigidbody.linearVelocity,
            tangentDirection * orbitalSpeed,
            Time.deltaTime * 2f
        );
    }

    private void ApplySpiralMotion()
    {
        float currentDistance = Vector3.Distance(transform.position, currentTarget.position);
        float spiralProgress = 1 - (currentDistance / initialDistance);

        Vector3 spiralDirection = (currentTarget.position - transform.position).normalized;
        rocketRigidbody.AddForce(spiralDirection * spiralSpeed * spiralProgress, ForceMode.Acceleration);
        rocketRigidbody.linearVelocity *= 1 + (0.1f * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        if (rocketRigidbody.linearVelocity.magnitude > 0.1f)
        {
            float rotationSpeed = isInTargetField ? 2f : 10f;
            Quaternion targetRotation = Quaternion.LookRotation(rocketRigidbody.linearVelocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed
            );
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("CelestialBody"))
        {
            DestroyRocket();
        }
    }

    private void DestroyRocket()
    {
        if (rocketRigidbody != null)
        {
            rocketRigidbody.linearVelocity = Vector3.zero;
            rocketRigidbody.angularVelocity = Vector3.zero;
            rocketRigidbody.isKinematic = true;
        }
        Destroy(gameObject, 0.02f);
    }
}