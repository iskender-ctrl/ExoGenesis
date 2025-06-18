using UnityEngine;

[CreateAssetMenu(menuName = "Planet/Progress Weights")]
public class ProgressWeightsSO : ScriptableObject
{
    [Range(0,1)] public float population  = .5f;
    [Range(0,1)] public float decoration  = .3f;
    [Range(0,1)] public float evolution   = .2f;

    private void OnValidate()             // Toplamı 1’e zorlarsan denge kolay olur
    {
        float sum = population + decoration + evolution;
        if (Mathf.Approximately(sum, 1f) || sum == 0) return;

        population  /= sum;
        decoration  /= sum;
        evolution   /= sum;
    }
}
