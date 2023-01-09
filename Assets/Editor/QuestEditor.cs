using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuestEditor : EditorWindow
{
    static QuestDataController questDataController;
    float space = 10;
    static bool isShowWindow = false;
    bool[] listQuestSelect = new bool[200];
    bool[] listRequireValueSelect = new bool[200];
    bool[] listRewardSelect = new bool[200];

    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Game Editor/Quest Editor")]
    public static void ShowWindow()
    {
        isShowWindow = true;
        EditorWindow.GetWindow(typeof(QuestEditor));
        LoadQuestData();
    }
    static void LoadQuestData()
    {
        questDataController = new QuestDataController();
        questDataController.LoadQuestData();
    }

    void OnGUI()
    {
        if (!isShowWindow)
        {
            isShowWindow = true;
           // EditorWindow.GetWindow(typeof(LevelEditor));
            LoadQuestData();
        }

        //header
        float space = 15;
        GUILayout.Space(space * 0.5f);
        EditorGUILayout.LabelField("Quest Data Manager!");

        GUILayout.Space(space * 0.5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Quest"))
        {
            QuestDataController.QuestData newQuestData = new QuestDataController.QuestData();
            newQuestData.QuestID = (questDataController.questDataCollection.ListQuestData.Count < 10 ? "0" + (questDataController.questDataCollection.ListQuestData.Count + 1).ToString() : (questDataController.questDataCollection.ListQuestData.Count + 1).ToString());
            questDataController.questDataCollection.ListQuestData.Add(newQuestData);
        }

        if (GUILayout.Button("Save Data"))
        {
            questDataController.Save(questDataController.questDataCollection);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(450), GUILayout.Height(490));

        ///load list
        for (int i = 0; i < questDataController.questDataCollection.ListQuestData.Count; i++)
        {
            listQuestSelect[i] = EditorGUILayout.Foldout(listQuestSelect[i], "Quest " + questDataController.questDataCollection.ListQuestData[i].QuestID);
            GUILayout.Space(space * 0.3f);
            if (listQuestSelect[i])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                questDataController.questDataCollection.ListQuestData[i].QuestID = EditorGUILayout.TextField("ID", questDataController.questDataCollection.ListQuestData[i].QuestID);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                questDataController.questDataCollection.ListQuestData[i].QuestName = EditorGUILayout.TextField("Name", questDataController.questDataCollection.ListQuestData[i].QuestName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                listRequireValueSelect[i] = EditorGUILayout.Foldout(listRequireValueSelect[i], "Require Value");
                EditorGUILayout.EndHorizontal();

                if (listRequireValueSelect[i])
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space*2);
                    questDataController.questDataCollection.ListQuestData[i].RequireValue.Value = int.Parse(EditorGUILayout.TextField("Value", questDataController.questDataCollection.ListQuestData[i].RequireValue.Value.ToString()));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space * 2);
                    questDataController.questDataCollection.ListQuestData[i].RequireValue.PercentIncreasePerStep = float.Parse(EditorGUILayout.TextField("Percent Increase Per Step", questDataController.questDataCollection.ListQuestData[i].RequireValue.PercentIncreasePerStep.ToString()));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                listRewardSelect[i] = EditorGUILayout.Foldout(listRewardSelect[i], "Reward");
                EditorGUILayout.EndHorizontal();

                if (listRewardSelect[i])
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space * 2);
                    questDataController.questDataCollection.ListQuestData[i].Reward.Star = int.Parse(EditorGUILayout.TextField("Star", questDataController.questDataCollection.ListQuestData[i].Reward.Star.ToString()));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space * 2);
                    questDataController.questDataCollection.ListQuestData[i].Reward.Gem = int.Parse(EditorGUILayout.TextField("Gem", questDataController.questDataCollection.ListQuestData[i].Reward.Gem.ToString()));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space * 2);
                    questDataController.questDataCollection.ListQuestData[i].Reward.PercentIncreasePerStep = float.Parse(EditorGUILayout.TextField("Percent Increase Per Step", questDataController.questDataCollection.ListQuestData[i].Reward.PercentIncreasePerStep.ToString()));
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(space * 0.3f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                if (GUILayout.Button("Remove Quest"))
                {
                    questDataController.questDataCollection.ListQuestData.Remove(questDataController.questDataCollection.ListQuestData[i]);
                }
                EditorGUILayout.EndHorizontal();
            }

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}


