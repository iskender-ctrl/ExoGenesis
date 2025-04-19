using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // Dotween library

public class ProfileUI : MonoBehaviour
{
    [Header("Profile Information")]
    public TMP_InputField nameInput; // Name input (TextMeshPro)
    public TextMeshProUGUI nameText; // Name display (TextMeshPro)
    public TMP_Text levelsCompletedText; // Completed levels (TextMeshPro)
    public TMP_Text planetsUnlockedText; // Unlocked planets (TextMeshPro)

    [Header("Avatar Selection")]
    public Image avatarDisplay; // Image to display the selected avatar
    public GameObject[] avatarObjects; // List of avatar objects (GameObject)
    public Button nextAvatarButton; // Next avatar button
    public Button previousAvatarButton; // Previous avatar button

    private ProfileManager profileManager;
    private int currentAvatarIndex = 0;

    void Start()
    {
        // Instantiate ProfileManager
        profileManager = new ProfileManager();

        // Load the profile
        profileManager.LoadProfile();

        // Update UI elements
        nameText.text = profileManager.currentProfile.playerName;
        levelsCompletedText.text = profileManager.currentProfile.levelsCompleted.ToString();
        planetsUnlockedText.text = profileManager.currentProfile.planetsUnlocked.ToString();

        // Set up avatar selection
        currentAvatarIndex = profileManager.currentProfile.avatarIndex;
        UpdateAvatarDisplay();

        // Add click events to buttons
        nameInput.onValueChanged.AddListener(OnNameChanged);
        nextAvatarButton.onClick.AddListener(OnNextAvatar);
        previousAvatarButton.onClick.AddListener(OnPreviousAvatar);
    }

    // When the name changes
    private void OnNameChanged(string newName)
    {
        profileManager.currentProfile.playerName = newName;
        nameText.text = newName; // Update the name display in real-time
        profileManager.SaveProfile();
    }

    // When the next avatar button is clicked
    private void OnNextAvatar()
    {
        currentAvatarIndex = (currentAvatarIndex + 1) % avatarObjects.Length;
        UpdateAvatarDisplay();
    }

    // When the previous avatar button is clicked
    private void OnPreviousAvatar()
    {
        currentAvatarIndex = (currentAvatarIndex - 1 + avatarObjects.Length) % avatarObjects.Length;
        UpdateAvatarDisplay();
    }

    // Update the avatar display
    private void UpdateAvatarDisplay()
    {
        // Deactivate all avatars
        foreach (GameObject avatar in avatarObjects)
        {
            avatar.SetActive(false);
        }

        // Activate the selected avatar
        avatarObjects[currentAvatarIndex].SetActive(true);

        // Update the avatar display image
        avatarDisplay.sprite = avatarObjects[currentAvatarIndex].GetComponent<Image>().sprite;

        // Save the selected avatar index
        profileManager.currentProfile.avatarIndex = currentAvatarIndex;
        profileManager.SaveProfile();
    }

    // Update stats (e.g., call when a level is completed)
    public void UpdateStats(int levelsCompleted, int planetsUnlocked)
    {
        profileManager.currentProfile.levelsCompleted = levelsCompleted;
        profileManager.currentProfile.planetsUnlocked = planetsUnlocked;
        levelsCompletedText.text = "Completed Levels: " + levelsCompleted;
        planetsUnlockedText.text = "Unlocked Planets: " + planetsUnlocked;
        profileManager.SaveProfile();
    }
}