using UnityEngine;

public class WorldSpaceCanvasController : MonoBehaviour
{
    public Transform targetObject; // Canvas'ın bağlanacağı obje
    private Camera mainCamera; // Ana kamera referansı

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object atanmadı! Canvas'ın hangi objeye bağlı olduğunu belirtmelisin.");
        }

        // Ana kamerayı bul
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetObject == null || mainCamera == null) return;

        // Canvas'ı hedef objenin pozisyonuna ayarla
        transform.position = targetObject.position;

        // Canvas'ı kameraya doğru döndür
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}
