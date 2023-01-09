using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerStatsEditor : EditorWindow
{

    [MenuItem("Game Editor/Player Stats Editor")]

    public static void ShowWindow()
    {
        PlayerPrefs.DeleteAll();
        // isShowWindow = true;
        EditorWindow.GetWindow(typeof(PlayerStatsEditor));
        // LoadLevelData();
    }
    void OnGUI()
    {

        if (GUILayout.Button("Delete All PlayerPref"))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Master.Stats == null) return;

        EditorGUILayout.BeginHorizontal();
        Master.Stats.Gem = int.Parse(EditorGUILayout.TextField("Gem", Master.Stats.Gem.ToString()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Master.Stats.Star = int.Parse(EditorGUILayout.TextField("Star", Master.Stats.Star.ToString()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Master.Stats.Energy = int.Parse(EditorGUILayout.TextField("Energy", Master.Stats.Energy.ToString()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //Master.LevelData.lastLevel = int.Parse(EditorGUILayout.TextField("Last Level", Master.LevelData.lastLevel.ToString()));
        Master.LevelData.SetLastLevel(int.Parse(EditorGUILayout.TextField("Last Level", Master.LevelData.lastLevel.ToString())));
        EditorGUILayout.EndHorizontal();

    }
}
