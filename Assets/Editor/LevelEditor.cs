using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class LevelEditor : EditorWindow
{
    public static LevelDataController levelDataController;
    bool isReadLevelData;
    bool[] listLevelsSelect = new bool[200];
    bool[] listWavesSelectOfLevel = new bool[200];
    bool[] listWavesSelect = new bool[200];
    bool[] listSequencesSelectOfWave = new bool[200];
    bool[] listSequencesSelect = new bool[200];

    bool[] listEnemiesSelectOfSequence = new bool[200];
    bool[] listEnemiesSelect = new bool[200];

    Vector2 scrollPos = Vector2.zero;

    LevelDataController.LevelDataCollection newLevelDataCollection;

    static bool isShowWindow = false;

    [MenuItem("Game Editor/Level Editor")]
    public static void ShowWindow()
    {
        isShowWindow = true;
        EditorWindow.GetWindow(typeof(LevelEditor));
        LoadLevelData();
    }

    static void LoadLevelData()
    {
        levelDataController = new LevelDataController();
        levelDataController.LoadLevelData();
    }

    void OnGUI()
    {
        if (!isShowWindow)
        {
            isShowWindow = true;
            LoadLevelData();
        }

        float space = 15;
        GUILayout.Space(space * 0.5f);
        EditorGUILayout.LabelField("Level Data Manager!");

        GUILayout.Space(space * 0.5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Level"))
        {
            LevelDataController.LevelData newLevelData = new LevelDataController.LevelData();
            newLevelData.LevelIndex = levelDataController.levelDataCollection.ListLevelData.Count + 1;
            levelDataController.levelDataCollection.ListLevelData.Add(newLevelData);
        }

        if (GUILayout.Button("Save Data"))
        {
            SaveLevelData();
        }
        EditorGUILayout.EndHorizontal();

        /////////////Total Level/////////////////////
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Total Level: " + levelDataController.levelDataCollection.ListLevelData.Count.ToString(), EditorStyles.boldLabel);

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        GUILayoutOption[] optionsScrollView = new GUILayoutOption[] { GUILayout.Width(450), GUILayout.Height(490) };

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, optionsScrollView);
        for (int i = 0; i < levelDataController.levelDataCollection.ListLevelData.Count; i++)
        {
            LevelDataController.LevelData levelData = levelDataController.levelDataCollection.ListLevelData[i];

            /////////////Level/////////////////////
            listLevelsSelect[i] = EditorGUILayout.Foldout(listLevelsSelect[i], "Level " + levelDataController.levelDataCollection.ListLevelData[i].LevelIndex);
            GUILayout.Space(space * 0.3f);
            if (listLevelsSelect[i])
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(500) };

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                levelDataController.levelDataCollection.ListLevelData[i].InitialGold = int.Parse(EditorGUILayout.TextField("Initial Gold", levelDataController.levelDataCollection.ListLevelData[i].InitialGold.ToString(), options));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                levelDataController.levelDataCollection.ListLevelData[i].NumberOfLanes = int.Parse(EditorGUILayout.TextField("Number of lane", levelDataController.levelDataCollection.ListLevelData[i].NumberOfLanes.ToString(), options));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                levelDataController.levelDataCollection.ListLevelData[i].NumberOfPositionsCanBuildUnitInLane = int.Parse(EditorGUILayout.TextField("Number of position can build Unit in lane", levelDataController.levelDataCollection.ListLevelData[i].NumberOfPositionsCanBuildUnitInLane.ToString(), options));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                levelDataController.levelDataCollection.ListLevelData[i].NumberOfUnitsAllowedDead = int.Parse(EditorGUILayout.TextField("Number of Units Allowed Dead", levelDataController.levelDataCollection.ListLevelData[i].NumberOfUnitsAllowedDead.ToString(), options));
                EditorGUILayout.EndHorizontal();


                /////////////Waves/////////////////////
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                listWavesSelectOfLevel[i] = EditorGUILayout.Foldout(listWavesSelectOfLevel[i], "Waves");
                EditorGUILayout.EndHorizontal();
                if (listWavesSelectOfLevel[i])
                {
                    for (int y = 0; y < levelDataController.levelDataCollection.ListLevelData[i].ListWaves.Count; y++)
                    {
                        //LevelDataController.Waves wave = levelData.ListWaves[y];
                        GUILayout.Space(space * 0.3f);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(space * 2);
                        listWavesSelect[y] = EditorGUILayout.Foldout(listWavesSelect[y], "Wave " + y);
                        EditorGUILayout.EndHorizontal();
                        if (listWavesSelect[y])
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(space * 4);
                            listSequencesSelectOfWave[y] = EditorGUILayout.Foldout(listSequencesSelectOfWave[y], "Sequences");
                            EditorGUILayout.EndHorizontal();
                            if (listSequencesSelectOfWave[y])
                            {
                                /////////////List Sequences/////////////////////

                                for (int z = 0; z < levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences.Count; z++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(space * 6);
                                    listSequencesSelect[z] = EditorGUILayout.Foldout(listSequencesSelect[z], "Sequence " + z);
                                    EditorGUILayout.EndHorizontal();
                                    if (listSequencesSelect[z])
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(space * 8);
                                        levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].Time = int.Parse(EditorGUILayout.TextField("Time (second)", levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].Time.ToString(), options));
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(space * 8);
                                        listEnemiesSelectOfSequence[z] = EditorGUILayout.Foldout(listEnemiesSelectOfSequence[z], "Enemies");
                                        EditorGUILayout.EndHorizontal();

                                        if (listEnemiesSelectOfSequence[z])
                                        {
                                            for (int x = 0; x < levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence.Count; x++)
                                            {
                                                EditorGUILayout.BeginHorizontal();
                                                GUILayout.Space(space * 10);
                                                listEnemiesSelect[x] = EditorGUILayout.Foldout(listEnemiesSelect[x], "Enemy " + x);
                                                EditorGUILayout.EndHorizontal();
                                                if (listEnemiesSelect[x])
                                                {
                                                    EditorGUILayout.BeginHorizontal();
                                                    GUILayout.Space(space * 12);
                                                    levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence[x].EnemyID = EditorGUILayout.TextField("Enemy ID", levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence[x].EnemyID.ToString(), options);
                                                    EditorGUILayout.EndHorizontal();

                                                    EditorGUILayout.BeginHorizontal();
                                                    GUILayout.Space(space * 12);
                                                    levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence[x].Lane = int.Parse(EditorGUILayout.TextField("At Lane", levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence[x].Lane.ToString(), options));
                                                    EditorGUILayout.EndHorizontal();

                                                    //button
                                                    GUILayout.Space(space * 0.3f);
                                                    EditorGUILayout.BeginHorizontal();
                                                    GUILayout.Space(space * 12);
                                                    if (GUILayout.Button("Remove Enemy"))
                                                    {
                                                        levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence.Remove(levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence[x]);
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }
                                            }
                                            //button
                                            GUILayout.Space(space * 0.3f);
                                            EditorGUILayout.BeginHorizontal();
                                            GUILayout.Space(space * 10);
                                            if (GUILayout.Button("Add Enemy"))
                                            {
                                                LevelDataController.EnemyAtSequence enemyAtSequence = new LevelDataController.EnemyAtSequence();
                                                enemyAtSequence.EnemyID = "";
                                                enemyAtSequence.Lane = 0;
                                                levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z].ListEnemyAtSequence.Add(enemyAtSequence);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }

                                        //button
                                        GUILayout.Space(space * 0.3f);
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(space * 8);
                                        if (GUILayout.Button("Remove Sequence"))
                                        {
                                            levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences.Remove(levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences[z]);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                //button
                                GUILayout.Space(space * 0.3f);
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(space * 6);
                                if (GUILayout.Button("Add Sequence"))
                                {
                                    LevelDataController.Sequences sequence = new LevelDataController.Sequences();
                                    levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y].ListSequences.Add(sequence);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            GUILayout.Space(space * 0.3f);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(space * 4);
                            if (GUILayout.Button("Remove Wave"))
                            {
                                levelDataController.levelDataCollection.ListLevelData[i].ListWaves.Remove(levelDataController.levelDataCollection.ListLevelData[i].ListWaves[y]);
                                // Debug.Log(levelData.InitialGold.ToString());
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.Space(space * 0.3f);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space * 2);
                    if (GUILayout.Button("Add Wave"))
                    {
                        LevelDataController.Waves wave = new LevelDataController.Waves();
                        levelDataController.levelDataCollection.ListLevelData[i].ListWaves.Add(wave);
                        // Debug.Log(levelData.InitialGold.ToString());
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(space * 0.5f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                if (GUILayout.Button("Remove Level"))
                {
                    levelDataController.levelDataCollection.ListLevelData.Remove(levelDataController.levelDataCollection.ListLevelData[i]);
                    //SaveLevelData();
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

    }

    void SaveLevelData()
    {
        bool isCanSave = true;
        foreach (LevelDataController.LevelData levelData in levelDataController.levelDataCollection.ListLevelData)
        {
            //validation
            string alert = "";
            string first = "\nLevel " + levelData.LevelIndex + "->";
            bool isValid = true;
            if (levelData.InitialGold <= 1)
            {
                isValid = false;
                alert += first + "Initial Gold must be large than 0";
            }
            if (levelData.NumberOfLanes < 1 || levelData.NumberOfLanes > 3)
            {
                isValid = false;
                alert += first + "'Number of lanes' must be in range 1-3";
            }
            if (levelData.NumberOfPositionsCanBuildUnitInLane < 1 || levelData.NumberOfPositionsCanBuildUnitInLane > 7)
            {
                isValid = false;
                alert += first + "'Number of position can build Unit in lane' must be in range 1-7";
            }
            if (levelData.NumberOfUnitsAllowedDead < 1)
            {
                isValid = false;
                alert += first + "'Number of Zombies Allowed Escape' must be large than 0";
            }
            if (levelData.ListWaves.Count <= 0)
            {
                isValid = false;
                alert += first + "You must have at least one wave";
            }
            else
            {
                for (int i = 0; i < levelData.ListWaves.Count; i++)
                {
                    if (levelData.ListWaves[i].ListSequences.Count <= 0)
                    {
                        isValid = false;
                        alert += first + "Wave " + i + ": You must have at least one sequence";
                    }
                    else
                    {
                        for (int y = 0; y < levelData.ListWaves[i].ListSequences.Count; y++)
                        {
                            if (levelData.ListWaves[i].ListSequences[y].Time <= 0)
                            {
                                isValid = false;
                                alert += first + "Wave " + i + "->Sequence " + y + ": Time to appear enemy must larger than 0";
                            }

                            if (levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence.Count <= 0)
                            {
                                isValid = false;
                                alert += first + "Wave " + i + "->Sequence " + y + ": You must have at least one enemy";
                            }
                            else
                            {
                                for (int x = 0; x < levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence.Count; x++)
                                {
                                    if (levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence[x].EnemyID == "")
                                    {
                                        isValid = false;
                                        alert += first + "Wave " + i + "->Sequence " + y + "->Enemy " + x + ": Enemy ID must not be blank";
                                    }
                                    if (levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence[x].Lane < 1 || levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence[x].Lane > 3)
                                    {
                                        isValid = false;
                                        alert += first + "Wave " + i + "->Sequence " + y + "->Enemy " + x + ": Lane of Enemy must be in range 1-3";
                                    }
                                    if (levelData.ListWaves[i].ListSequences[y].ListEnemyAtSequence[x].Lane > levelData.NumberOfLanes)
                                    {
                                        isValid = false;
                                        alert += first + "Wave " + i + "->Sequence " + y + "->Enemy " + x + ": Lane of Enemy can not be larger than 'Number of Lanes (" + levelData.NumberOfLanes + ")'";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!isValid)
            {
                EditorUtility.DisplayDialog("Can not save!", "Data input is not valid!" + alert, "OK");
                isCanSave = false;
            }
        }

        if (isCanSave)
        {
            levelDataController.Save(levelDataController.levelDataCollection);
            //   EditorUtility.DisplayDialog("Save successfull!", "Save level data successfull!", "OK");
        }
    }
}