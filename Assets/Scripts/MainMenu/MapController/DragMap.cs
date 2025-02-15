using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMap : MonoBehaviour
{
    public float dragSpeed = 1.0f; // Hareket hızını ayarla

    private Camera mainCamera;
    private Vector3 dragOrigin;
    private bool isDragging;
    public bool isDecorateCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Eğer dokunulan bir UI elemanı varsa, geri dön
        if (IsPointerOverUI()) return;

        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = touch.position;
                isDragging = true;
            }

            if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 difference = (Vector3)touch.position - dragOrigin;

                if (!isDecorateCamera)
                {
                    // Kamerayı X ve Y ekseninde hareket ettir
                    mainCamera.transform.position -= new Vector3(difference.x, difference.y, 0) * dragSpeed * Time.deltaTime;
                }
                else
                {
                    mainCamera.transform.position -= new Vector3(difference.x, 0, difference.y) * dragSpeed * Time.deltaTime;
                }

                // Yeni drag başlangıç noktasını güncelle
                dragOrigin = touch.position;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Canvas üzerindeki Graphic Raycaster'ı bul
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        if (raycaster == null)
        {
            return false;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (Input.touchCount > 0)
        {
            eventData.position = Input.GetTouch(0).position;
        }
        else
        {
            eventData.position = Input.mousePosition;
        }

        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        return results.Count > 0;
    }
}
