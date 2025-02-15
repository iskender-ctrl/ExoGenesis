using UnityEngine;
using UnityEngine.UI;

public class PlanetButton : MonoBehaviour
{
    public string childName;        // Aktif edilecek child objenin adı
    public Button button;           // Buton referansı

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Direkt ilgili child objeyi aktif etme çağrısı
        DecorationController.Instance.ActivateChild(childName);
    }
}
