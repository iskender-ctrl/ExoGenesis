using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Roket Ayarları")]
    public GameObject rocketPrefab; // Fırlatılacak roket prefabı
    public Transform spawnPoint;   // Roketin çıkış noktası
    public float launchSpeed = 20f; // Roketin fırlatma hızı
    public float rocketLifetime = 10f; // Roketin sahnede kalma süresi (saniye)

    private bool isMouseHeld = false; // Mouse basılı tutuluyor mu?

    void Update()
    {
        // Mouse sol tuşuna basma kontrolü
        if (Input.GetMouseButtonDown(0))
        {
            isMouseHeld = true;
        }

        // Mouse sol tuşunu bırakma kontrolü
        if (Input.GetMouseButtonUp(0) && isMouseHeld)
        {
            FireRocket(); // Roketi fırlat
            isMouseHeld = false;
        }
    }

    void FireRocket()
    {
        // Mouse'un sahnedeki pozisyonunu hesapla
        Vector3 targetPosition = GetMouseWorldPosition();
        Vector3 direction = (targetPosition - spawnPoint.position).normalized;

        // Roketi spawn et ve yönlendirme ver
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject rocket = Instantiate(rocketPrefab, spawnPoint.position, rotation);

        // Rigidbody bileşenine hız uygula
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchSpeed;
        }

        // Roketi belirli bir süre sonra yok et
        Destroy(rocket, rocketLifetime);
    }

    Vector3 GetMouseWorldPosition()
    {
        // Mouse'un dünya uzayındaki pozisyonunu hesapla
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, Vector3.zero); // XZ düzlemi
        if (xzPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance); // Mouse'un dünya koordinatını döndür
        }
        return Vector3.zero;
    }
}
