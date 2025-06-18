using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Toggle notificationToggle;
    public Toggle vibrationToggle;
    private void Start()
    {
        // Kaydedilmiş ayarları yükle
        soundToggle.isOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1;
        notificationToggle.isOn = PlayerPrefs.GetInt("Notification", 1) == 1;
        notificationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;

        // Toggle değişimlerini dinle
        soundToggle.onValueChanged.AddListener(delegate { ToggleSound(soundToggle.isOn); });
        musicToggle.onValueChanged.AddListener(delegate { ToggleMusic(musicToggle.isOn); });
        notificationToggle.onValueChanged.AddListener(delegate { ToggleNotification(notificationToggle.isOn); });
        notificationToggle.onValueChanged.AddListener(delegate { ToggleVibration(notificationToggle.isOn); });
    }

    private void ToggleSound(bool isOn)
    {
        PlayerPrefs.SetInt("Sound", isOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Sound: " + (isOn ? "Açık" : "Kapalı"));
    }

    private void ToggleMusic(bool isOn)
    {
        PlayerPrefs.SetInt("Music", isOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Music: " + (isOn ? "Açık" : "Kapalı"));
    }

    private void ToggleNotification(bool isOn)
    {
        PlayerPrefs.SetInt("Notification", isOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Notification: " + (isOn ? "Açık" : "Kapalı"));
    }
    private void ToggleVibration(bool isOn)
    {
        PlayerPrefs.SetInt("Vibration", isOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Notification: " + (isOn ? "Açık" : "Kapalı"));
    }
}
