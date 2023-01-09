using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnitsEditor : EditorWindow
{
    static UnitDataController unitDataController;
    float space = 10;
    static bool isShowWindow = false;
    bool[] listUnitSelect = new bool[200];
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Game Editor/Units Editor")]
    public static void ShowWindow()
    {
        isShowWindow = true;
        EditorWindow.GetWindow(typeof(UnitsEditor));
        LoadUnitsData();
    }
    static void LoadUnitsData()
    {
        unitDataController = new UnitDataController();
        unitDataController.LoadUnitData();

    }
    void OnGUI()
    {
        if (!isShowWindow)
        {
            isShowWindow = true;
            //EditorWindow.GetWindow(typeof(UnitsEditor));
            LoadUnitsData();
        }

        //header
        float space = 15;
        GUILayout.Space(space * 0.5f);
        EditorGUILayout.LabelField("Units Data Manager!");

        GUILayout.Space(space * 0.5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Unit"))
        {
            UnitDataController.UnitData newUnitData = new UnitDataController.UnitData();
            newUnitData.UnitID = (unitDataController.unitDataCollection.ListUnitsData.Count < 10 ? "0" + (unitDataController.unitDataCollection.ListUnitsData.Count + 1).ToString() : (unitDataController.unitDataCollection.ListUnitsData.Count + 1).ToString());
            unitDataController.unitDataCollection.ListUnitsData.Add(newUnitData);
        }

        if (GUILayout.Button("Save Data"))
        {
            unitDataController.Save(unitDataController.unitDataCollection);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(450), GUILayout.Height(490));

        ///load list
        for (int i = 0; i < unitDataController.unitDataCollection.ListUnitsData.Count; i++)
        {
            listUnitSelect[i] = EditorGUILayout.Foldout(listUnitSelect[i], "Unit " + unitDataController.unitDataCollection.ListUnitsData[i].UnitID);
            GUILayout.Space(space * 0.3f);
            if (listUnitSelect[i])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].UnitID = EditorGUILayout.TextField("ID", unitDataController.unitDataCollection.ListUnitsData[i].UnitID);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].UnitName = EditorGUILayout.TextField("Name", unitDataController.unitDataCollection.ListUnitsData[i].UnitName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].Health = float.Parse(EditorGUILayout.TextField("Health", unitDataController.unitDataCollection.ListUnitsData[i].Health.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].Damage = float.Parse(EditorGUILayout.TextField("Damage", unitDataController.unitDataCollection.ListUnitsData[i].Damage.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].AttackSpeed = float.Parse(EditorGUILayout.TextField("Attack Speed", unitDataController.unitDataCollection.ListUnitsData[i].AttackSpeed.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].Range = float.Parse(EditorGUILayout.TextField("Range", unitDataController.unitDataCollection.ListUnitsData[i].Range.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].Critical = float.Parse(EditorGUILayout.TextField("Critical", unitDataController.unitDataCollection.ListUnitsData[i].Critical.ToString()));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].NumberOfAmmoToReload = int.Parse(EditorGUILayout.TextField("Number of Ammo To Reload", unitDataController.unitDataCollection.ListUnitsData[i].NumberOfAmmoToReload.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].NumberOfShootToUpgrade = int.Parse(EditorGUILayout.TextField("Number of Shoot To Upgrade", unitDataController.unitDataCollection.ListUnitsData[i].NumberOfShootToUpgrade.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].Price = int.Parse(EditorGUILayout.TextField("Price", unitDataController.unitDataCollection.ListUnitsData[i].Price.ToString()));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].TimeCountdownSelect = int.Parse(EditorGUILayout.TextField("Time Countdown", unitDataController.unitDataCollection.ListUnitsData[i].TimeCountdownSelect.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                unitDataController.unitDataCollection.ListUnitsData[i].UnlockAtLevel = int.Parse(EditorGUILayout.TextField("Unlock At Level", unitDataController.unitDataCollection.ListUnitsData[i].UnlockAtLevel.ToString()));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                if (GUILayout.Button("Remove Unit"))
                {
                    unitDataController.unitDataCollection.ListUnitsData.Remove(unitDataController.unitDataCollection.ListUnitsData[i]);
                }
                EditorGUILayout.EndHorizontal();

            }

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}


