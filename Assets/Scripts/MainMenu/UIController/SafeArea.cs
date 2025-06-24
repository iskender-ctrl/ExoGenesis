using UnityEngine;

/// <summary>
/// Pins this RectTransform to the safe-area’s top edge while
/// preserving its original size & horizontal stretch.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    RectTransform _rt;
    Rect          _lastSa;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        ApplySafeArea();
    }
    void OnEnable() => ApplySafeArea();

    void Update()
    {
        if (_lastSa != Screen.safeArea)   // orientation / cut-out change
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect sa   = Screen.safeArea;
        _lastSa   = sa;

        // Yukarıdan boşluk miktarı (pozitif ↓, offsetMax.y negatif ↑)
        float topInset = Screen.height - (sa.y + sa.height);

        Vector2 offMin = _rt.offsetMin;   // mevcut offsets
        Vector2 offMax = _rt.offsetMax;

        // *** Sadece üst kenarı ayarla ***
        offMax.y = -topInset;             // negatif değer yukarı iter

        // Eğer barın altını da güvenli alanın altına çekmek istersen
        // offMin.y =  sa.y;

        _rt.offsetMin = offMin;
        _rt.offsetMax = offMax;
    }
}
