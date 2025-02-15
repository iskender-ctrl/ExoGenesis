using UnityEngine;
using DG.Tweening; // DoTween için gerekli
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ClickablePlanet : MonoBehaviour
{
    public static ClickablePlanet Instance;
    public float cameraMoveDuration = 1f; // Kameranın hareket süresi
    public float distanceFromObject = 5f; // Kameranın objeye olan uzaklığı
    private Camera mainCamera; // Ana kamera
    private Vector3 originalCameraPosition; // Kameranın orijinal pozisyonu
    private Quaternion originalCameraRotation; // Kameranın orijinal rotasyonu
    private bool isCameraFocused = false; // Kamera şu an bir objeye odaklanmış mı?
    private Transform focusedChild; // Şu an odaklanılan child
    public string planetName;

    void Start()
    {
        // Kamerayı bul ve orijinal pozisyonu kaydet
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
        }

        // Başlangıçta tüm child objelerin alt child'larını devre dışı bırak
        foreach (Transform child in transform)
        {
            Button[] buttons = child.GetComponentsInChildren<Button>(true); // Tüm butonları al, devre dışı olanları da dahil et
            foreach (Button button in buttons)
            {
                if (button.name == "Play")
                {
                    button.onClick.RemoveAllListeners(); // Eski listener'ları temizle
                    button.onClick.AddListener(() => Debug.Log("Play"));
                }
                else if (button.name == "Decorate")
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OpenDecorationPanel(child.name));
                }
            }

            if (child.childCount > 0)
            {
                child.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Child objenin ({child.name}) alt child'ı yok!", child.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // PC için tıklama veya mobil için dokunma
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Tıklama/dokunma noktasından bir ray gönder
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Raycast bir nesneye çarptı mı?
            {
                Transform clickedTransform = hit.transform;

                // Eğer tıklanan obje, parent objenin bir child'ıysa
                if (clickedTransform != null && clickedTransform.parent == transform)
                {
                    HandleChildClick(clickedTransform); // Tıklanan child'ı yönet
                }
            }
        }
    }

    private void HandleChildClick(Transform clickedChild)
    {
        // Eğer kamera şu an bir objeye odaklanmışsa
        if (isCameraFocused && focusedChild == clickedChild)
        {
            ResetCameraAndChild(clickedChild); // Kamera ve child sıfırlanır
            return;
        }

        // Diğer tüm child'ların alt child'larını devre dışı bırak
        foreach (Transform child in transform)
        {
            if (child.childCount > 0)
            {
                child.GetChild(0).gameObject.SetActive(false);
            }
        }

        // Tıklanan child'ın alt child'ını aktif hale getir
        if (clickedChild.childCount > 0)
        {
            clickedChild.GetChild(0).gameObject.SetActive(true);
        }

        // Kamerayı tıklanan child'a odakla
        FocusCameraOnObject(clickedChild);
        focusedChild = clickedChild; // Şu an odaklanılan child'ı kaydet
    }

    private void FocusCameraOnObject(Transform target)
    {
        if (mainCamera == null) return;

        // Kameranın hedef pozisyonunu hesapla
        Vector3 directionToTarget = (target.position - mainCamera.transform.position).normalized;
        Vector3 targetPosition = target.position - directionToTarget * distanceFromObject;

        // Kameranın hedef pozisyonunun ekranın merkezine uygun olmasını sağla
        targetPosition.y = target.position.y; // Objeyle aynı yükseklikte olmalı

        // Kameranın rotasyonunu objeye bakacak şekilde hesapla
        Quaternion targetRotation = Quaternion.LookRotation(target.position - targetPosition);

        // Kamerayı smooth bir şekilde hareket ettir
        mainCamera.transform.DOMove(targetPosition, cameraMoveDuration).SetEase(Ease.InOutCubic);
        mainCamera.transform.DORotateQuaternion(targetRotation, cameraMoveDuration).SetEase(Ease.InOutCubic);

        isCameraFocused = true; // Kamera şu an odakta
    }

    private void ResetCameraAndChild(Transform child)
    {
        if (mainCamera == null) return;

        // Kamerayı orijinal pozisyonuna ve rotasyonuna smooth bir şekilde döndür
        mainCamera.transform.DOMove(originalCameraPosition, cameraMoveDuration).SetEase(Ease.InOutCubic);
        mainCamera.transform.DORotateQuaternion(originalCameraRotation, cameraMoveDuration).SetEase(Ease.InOutCubic);

        // Tıklanan child'ın alt child'ını kapat
        if (child.childCount > 0)
        {
            child.GetChild(0).gameObject.SetActive(false);
        }

        isCameraFocused = false; // Kamera odaktan çıktı
        focusedChild = null; // Odaklanan child sıfırlandı
    }

    private void OpenDecorationPanel(string planetNameText)
    {
        planetName = planetNameText;
        MapDecorationController.Instance.planetName = planetName;
        SceneManager.LoadSceneAsync(2);
    }
}
