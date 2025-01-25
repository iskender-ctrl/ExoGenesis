using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject rocketPrefab; // Fırlatılacak roket prefabı
    public Transform spawnPoint;   // Roketin çıkış noktası
    public float launchSpeed = 20f; // Roketin fırlatma hızı
    public float rocketLifetime = 10f; // Roketin sahnede kalma süresi (saniye)
    private bool isMouseHeld = false; // Mouse basılı tutuluyor mu?

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol tuşa basıldığında
        {
            isMouseHeld = true;
        }

        if (Input.GetMouseButtonUp(0) && isMouseHeld) // Sol tuş bırakıldığında
        {
            FireRocket(); // Roketi fırlat
            isMouseHeld = false; // Mouse basılı değil
        }
    }

    void FireRocket()
    {
        // Mouse'un sahnedeki pozisyonunu hesapla
        Vector3 targetPosition = GetMouseWorldPosition();
        Vector3 direction = (targetPosition - spawnPoint.position).normalized;

        // Roketi spawn et ve doğru rotasyon ver
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject rocket = Instantiate(rocketPrefab, spawnPoint.position, rotation);

        // Rigidbody bileşenini al ve hız uygula
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchSpeed; // Rigidbody'ye hız uygula
        }
        // Roketi belli bir süre sonra yok et
        Destroy(rocket, rocketLifetime);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, Vector3.zero); // XZ düzlemi
        if (xzPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
