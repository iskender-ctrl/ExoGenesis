using UnityEngine;

public class AdFrequencyController
{
    const int MAX_PER_SESSION = 5;
    const float MIN_DELAY_SECONDS = 30f;
    const int MIN_LEVELS_GAP = 2;

    int adsShownThisSession = 0;
    float lastAdTime = -999f;
    int lastAdLevel = -999;

    public bool CanShow(int currentLevel)
    {
        if (adsShownThisSession >= MAX_PER_SESSION) return false;
        if (Time.realtimeSinceStartup - lastAdTime < MIN_DELAY_SECONDS) return false;
        if (currentLevel - lastAdLevel < MIN_LEVELS_GAP) return false;
        return true;
    }

    public void MarkShown(int currentLevel)
    {
        adsShownThisSession++;
        lastAdTime = Time.realtimeSinceStartup;
        lastAdLevel = currentLevel;
    }
}
public class AdCounter
{
    const string KEY_CONTINUE_COUNT = "ad_continue_cnt";

    private int continueCount;

    /* ctor sadece sıfırlar */
    public AdCounter() { continueCount = 0; }

    /* Awake/Start’ta çağırılacak */
    public void LoadFromPrefs()
    {
        continueCount = PlayerPrefs.GetInt(KEY_CONTINUE_COUNT, 0);
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetInt(KEY_CONTINUE_COUNT, continueCount);
        PlayerPrefs.Save();
    }

    public void ShowAdImmediately()
    {
        AdManager.Instance.ShowInterstitial(null);
    }

    public bool TryShowAdOnContinue()
    {
        continueCount++;
        SaveToPrefs();

        if (continueCount % 5 == 0)
        {
            AdManager.Instance.ShowInterstitial(null);
            return true;
        }
        return false;
    }
}
