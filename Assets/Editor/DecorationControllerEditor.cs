using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(DecorationController))]
public class DecorationControllerEditor : Editor
{
    private string saveFilePath;

    private void OnEnable()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public override void OnInspectorGUI()
    {
        // Varsayılan Inspector'ı göster
        DrawDefaultInspector();

        // Özel başlık
        GUILayout.Space(10);
        GUILayout.Label("🗑️ Save Data Manager", EditorStyles.boldLabel);

        // JSON dosyası var mı kontrol et
        if (File.Exists(saveFilePath))
        {
            GUILayout.Label("Save file exists at:");
            GUILayout.Label(saveFilePath, EditorStyles.helpBox);

            if (GUILayout.Button("🗑️ Delete Save Data"))
            {
                if (EditorUtility.DisplayDialog("Confirm Deletion", "Are you sure you want to delete the save data?", "Yes", "No"))
                {
                    DeleteSaveData();
                }
            }
        }
        else
        {
            GUILayout.Label("No save data found.", EditorStyles.miniLabel);
        }

        if (GUILayout.Button("📂 Open Save Folder"))
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }

    private void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data deleted: " + saveFilePath);
            EditorUtility.DisplayDialog("Save Data Manager", "Save data deleted successfully!", "OK");
        }
        else
        {
            Debug.LogWarning("No save data found to delete.");
        }
    }
}
