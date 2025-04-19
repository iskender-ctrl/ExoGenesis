using UnityEngine;

public class ProfileManager
{
    private static ProfileManager instance;
    public PlayerProfile currentProfile;

    // Singleton örneği
    public static ProfileManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ProfileManager();
            }
            return instance;
        }
    }

    // Profili kaydet
    public void SaveProfile()
    {
        string json = JsonUtility.ToJson(currentProfile);
        PlayerPrefs.SetString("PlayerProfile", json);
        PlayerPrefs.Save();
        Debug.Log("Profil kaydedildi!");
    }

    // Profili yükle
    public void LoadProfile()
    {
        if (PlayerPrefs.HasKey("PlayerProfile"))
        {
            string json = PlayerPrefs.GetString("PlayerProfile");
            currentProfile = JsonUtility.FromJson<PlayerProfile>(json);
            Debug.Log("Profil yüklendi!");
        }
        else
        {
            currentProfile = new PlayerProfile();
            Debug.Log("Yeni profil oluşturuldu!");
        }
    }
}