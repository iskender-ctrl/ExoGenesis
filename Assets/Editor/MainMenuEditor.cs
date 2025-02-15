using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(MainMenuController))]
public class MainMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MainMenuController menuController = (MainMenuController)target;

        GUILayout.Space(10);
        GUILayout.Label("Debugging Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset Player Data"))
        {
            PlayerDataManager.ResetToDefault();
            Debug.Log("Player data reset to default values.");
        }

        if (GUILayout.Button("Simulate Coin Gain (10 Coins)"))
        {
            PlayerDataManager.AddCoins(10);
        }

        if (GUILayout.Button("Simulate Fuel Usage (-5 Fuel)"))
        {
            PlayerDataManager.UseFuel(5);
        }
    }
}
