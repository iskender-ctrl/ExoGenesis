using UnityEngine;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public float animationDuration = 0.3f; // Animasyon süresi
    public GameObject blackBG;
    public void OpenPopup(Transform targetPopup)
    {
        if (targetPopup != null)
        {
            targetPopup.localScale = Vector3.zero; // İlk olarak scale'ini 0 yap
            targetPopup.gameObject.SetActive(true); // Sonra aktif et
            targetPopup.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
            blackBG.SetActive(true);
        }
    }

    public void ClosePopup(Transform targetPopup)
    {
        if (targetPopup != null)
        {
            targetPopup.DOScale(Vector3.zero, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => targetPopup.gameObject.SetActive(false)); // Animasyon bitince pasif yap
                blackBG.SetActive(false);
        }
    }
}
