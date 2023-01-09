using UnityEngine;
using UnityEditor;
using System.Collections;

public class EnemiesEditor : EditorWindow
{
    static EnemyDataController enemyDataController;
    float space = 10;
    static bool isShowWindow = false;
    bool[] listEnemiesSelect = new bool[200];
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Game Editor/Enemies Editor")]
    public static void ShowWindow()
    {
        isShowWindow = true;
        EditorWindow.GetWindow(typeof(EnemiesEditor));
        LoadEnemiesData();
    }
    static void LoadEnemiesData()
    {
        enemyDataController = new EnemyDataController();
        enemyDataController.LoadEnemyData();

    }
    void OnGUI()
    {
        if (!isShowWindow)
        {
            isShowWindow = true;
            // EditorWindow.GetWindow(typeof(LevelEditor));
            LoadEnemiesData();
        }

        //header
        float space = 15;
        GUILayout.Space(space * 0.5f);
        EditorGUILayout.LabelField("Enemy Data Manager!");

        GUILayout.Space(space * 0.5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Enemy"))
        {
            EnemyDataController.EnemyData newEnemyData = new EnemyDataController.EnemyData();
            newEnemyData.EnemyID = ((enemyDataController.enemyDataCollection.ListEnemiesData.Count + 1) < 10) ? "0" + (enemyDataController.enemyDataCollection.ListEnemiesData.Count + 1).ToString() : (enemyDataController.enemyDataCollection.ListEnemiesData.Count + 1).ToString();
            // newEnemyData.EnemyID = (enemyDataController.enemyDataCollection.ListEnemiesData.Count < 10 ? "0" + (enemyDataController.enemyDataCollection.ListEnemiesData.Count + 1).ToString() : (enemyDataController.enemyDataCollection.ListEnemiesData.Count + 1).ToString());
            enemyDataController.enemyDataCollection.ListEnemiesData.Add(newEnemyData);
        }

        if (GUILayout.Button("Save Data"))
        {
           
            enemyDataController.Save(enemyDataController.enemyDataCollection);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(450), GUILayout.Height(490));

        ///load list
        for (int i = 0; i < enemyDataController.enemyDataCollection.ListEnemiesData.Count; i++)
        {
            listEnemiesSelect[i] = EditorGUILayout.Foldout(listEnemiesSelect[i], "Enemy " + enemyDataController.enemyDataCollection.ListEnemiesData[i].EnemyID);
            GUILayout.Space(space * 0.3f);
            if (listEnemiesSelect[i])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].EnemyID = EditorGUILayout.TextField("ID", enemyDataController.enemyDataCollection.ListEnemiesData[i].EnemyID);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].EnemyName = EditorGUILayout.TextField("Name", enemyDataController.enemyDataCollection.ListEnemiesData[i].EnemyName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].Health = float.Parse(EditorGUILayout.TextField("Health", enemyDataController.enemyDataCollection.ListEnemiesData[i].Health.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].Damage = float.Parse(EditorGUILayout.TextField("Damage", enemyDataController.enemyDataCollection.ListEnemiesData[i].Damage.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].MoveSpeed = float.Parse(EditorGUILayout.TextField("Move Speed", enemyDataController.enemyDataCollection.ListEnemiesData[i].MoveSpeed.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].AttackSpeed = float.Parse(EditorGUILayout.TextField("Attack Speed", enemyDataController.enemyDataCollection.ListEnemiesData[i].AttackSpeed.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].Range = float.Parse(EditorGUILayout.TextField("Range", enemyDataController.enemyDataCollection.ListEnemiesData[i].Range.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].Ability = EditorGUILayout.TextField("Ability", enemyDataController.enemyDataCollection.ListEnemiesData[i].Ability.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                enemyDataController.enemyDataCollection.ListEnemiesData[i].CoinDrop = EditorGUILayout.TextField("Coin Drop (a,b,c...)", enemyDataController.enemyDataCollection.ListEnemiesData[i].CoinDrop.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                if (GUILayout.Button("Remove enemy"))
                {
                    enemyDataController.enemyDataCollection.ListEnemiesData.Remove(enemyDataController.enemyDataCollection.ListEnemiesData[i]);
                }
                EditorGUILayout.EndHorizontal();

            }

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}


