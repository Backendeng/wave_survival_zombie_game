using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class QuestDataController : MonoBehaviour
{

    //Unit data
    [XmlRoot("QuestDataCollection")]
    public class QuestDataCollection
    {
        [XmlArray("Quests")]
        [XmlArrayItem("Quest")]
        public List<QuestData> ListQuestData = new List<QuestData>();
    }
    public QuestDataCollection questDataCollection;

    [System.Serializable]
    public class QuestData
    {
        public string QuestID = "";
        public string QuestName = "";
        public RequireValue RequireValue = new RequireValue();
        public Reward Reward = new Reward();
        public int CurrentProgressValue = 0;
        public int CurrentStep = 0;
    }
    [System.Serializable]
    public class RequireValue
    {
        public int Value = 0;
        public float PercentIncreasePerStep = 0;//%
    }
    [System.Serializable]
    public class Reward
    {
        public int Star = 0;
        public int Gem = 0;
        public float PercentIncreasePerStep = 0; //%
    }

    void Awake()
    {
        if (Master.QuestData == null)
        {
            Master.QuestData = this;
        }
        LoadQuestData();
    }

    // Use this for initialization
    void Start()
    {
        Master.QuestData.SetProgressValue("06", Master.LevelData.GetTotalStarAtLevelsGot());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadQuestData()
    {
        TextAsset textAsset = Resources.Load("Data/Quests/QuestData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(QuestDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.questDataCollection = (QuestDataCollection)serializer.Deserialize(reader);
        }

        //set value progress and step
        for (int i = 0; i < questDataCollection.ListQuestData.Count; i++)
        {
            string questID = questDataCollection.ListQuestData[i].QuestID;
            int requireValue = questDataCollection.ListQuestData[i].RequireValue.Value;
            int currentValue = GetCurrentProgressValue(questID);
            int currentStep = GetCurrentStep(questID);
            float increasePercentRequireValue = questDataCollection.ListQuestData[i].RequireValue.PercentIncreasePerStep;
            questDataCollection.ListQuestData[i].RequireValue.Value = (int)Master.IncreaseValues(requireValue, currentStep, increasePercentRequireValue);

            int star = questDataCollection.ListQuestData[i].Reward.Star;
            int gem = questDataCollection.ListQuestData[i].Reward.Gem;
            float increasePercentReward = questDataCollection.ListQuestData[i].Reward.PercentIncreasePerStep;
            questDataCollection.ListQuestData[i].Reward.Star = (int)Master.IncreaseValues(star, currentStep, increasePercentReward);
            questDataCollection.ListQuestData[i].Reward.Gem = (int)Master.IncreaseValues(gem, currentStep, increasePercentReward);
            questDataCollection.ListQuestData[i].CurrentStep = currentStep;
            questDataCollection.ListQuestData[i].CurrentProgressValue = currentValue;
        }
    }

    public QuestData GetQuestDataByID(string id)
    {
        foreach (QuestData questData in questDataCollection.ListQuestData)
        {
            if (id == questData.QuestID)
            {
                return questData;
            }
        }
        return null;
    }

    public void IncreaseProgressValue(string questID, int value = 0, bool isAllowLargerThanRequireValue = true)
    {
        int currentProgressValue = GetCurrentProgressValue(questID);
        if (!isAllowLargerThanRequireValue && currentProgressValue >= GetQuestDataByID(questID).RequireValue.Value)
        {
            return;
        }

        if (value > 0)
        {
            currentProgressValue = currentProgressValue + value;
        }
        else
        {
            currentProgressValue = currentProgressValue + 1;
        }
        ObscuredPrefs.SetInt("CurrentProgressValueQuest_" + questID, currentProgressValue);
        ObscuredPrefs.Save();
        //LoadQuestData();
    }

    public void SetProgressValue(string questID, int value)
    {
        ObscuredPrefs.SetInt("CurrentProgressValueQuest_" + questID, value);
        ObscuredPrefs.Save();
    }

    public void IncreaseStep(string questID)
    {
        int currentStep = GetCurrentStep(questID);
        currentStep = currentStep + 1;
        ObscuredPrefs.SetInt("CurrentStepQuest_" + questID, currentStep);
        ObscuredPrefs.Save();
        // LoadQuestData();
    }

    public void SetStep(string questID, int value)
    {
        ObscuredPrefs.SetInt("CurrentStepQuest_" + questID, value);
        ObscuredPrefs.Save();
    }

    public int GetCurrentProgressValue(string questID)
    {
        return ObscuredPrefs.GetInt("CurrentProgressValueQuest_" + questID, 0);
    }

    public int GetCurrentStep(string questID)
    {
        return ObscuredPrefs.GetInt("CurrentStepQuest_" + questID, 0);
    }

    public bool isHaveQuestComplete()
    {
        Master.QuestData.LoadQuestData();
        foreach (QuestData item in questDataCollection.ListQuestData)
        {
            if (item.CurrentProgressValue >= item.RequireValue.Value)
            {
                return true;
            }
        }
        return false;
    }


    public void Save(QuestDataCollection questDataCollection)
    {
        string path = Application.dataPath + "/Resources/Data/Quests/QuestData.xml";

        var serializer = new XmlSerializer(typeof(QuestDataCollection));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, questDataCollection);
            Debug.Log("Saved XML to " + path);
        }
    }
}
