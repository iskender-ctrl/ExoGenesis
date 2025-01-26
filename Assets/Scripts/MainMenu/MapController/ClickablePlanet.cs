using UnityEngine;

public class ClickablePlanet : MonoBehaviour
{
    // Child objenin aktif/pasif durumunu kontrol etmek için değişken
    private GameObject planetChild;

    void Start()
    {
        // İlk çocuğu bul
        if (transform.childCount > 0)
        {
            planetChild = transform.GetChild(0).gameObject;
            planetChild.SetActive(false); // Başlangıçta child pasif olsun
        }
        else
        {
            Debug.LogWarning("Bu gezegenin child objesi yok!", gameObject);
        }
    }

    void OnMouseDown()
    {
        // Gezegen tıklandığında child aktif/pasif hale gelir
        if (planetChild != null)
        {
            bool isActive = planetChild.activeSelf;
            planetChild.SetActive(!isActive);
        }
    }
}
