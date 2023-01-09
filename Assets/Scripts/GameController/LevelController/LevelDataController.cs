using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

public class LevelDataController : MonoBehaviour
{

    // Use this for initialization
    //Unit data
    [XmlRoot("LevelDataCollection")]
    public class LevelDataCollection
    {
        [XmlArray("Levels")]
        [XmlArrayItem("Level")]
        public List<LevelData> ListLevelData = new List<LevelData>();
    }
    public LevelDataCollection levelDataCollection;

    [System.Serializable]
    public class LevelData
    {
        public int LevelIndex = 0;
        public int NumberOfLanes;
        public int NumberOfPositionsCanBuildUnitInLane;
        public int NumberOfUnitsAllowedDead;
        public int InitialGold;
        [XmlArray("Waves")]
        [XmlArrayItem("Wave")]
        public List<Waves> ListWaves = new List<Waves>();
    }

    [System.Serializable]
    public class Waves
    {
        //public int WaveIndex;
        [XmlArray("Sequences")]
        [XmlArrayItem("Sequence")]
        public List<Sequences> ListSequences = new List<Sequences>();
    }

    [System.Serializable]
    public class Sequences
    {
        public float Time;
        [XmlArray("Enemies")]
        [XmlArrayItem("Enemy")]
        public List<EnemyAtSequence> ListEnemyAtSequence = new List<EnemyAtSequence>();
    }


    [System.Serializable]
    public class EnemyAtSequence
    {
        public string EnemyID;
        public int Lane;
    }

    private bool startInitEnemy;

    public int totalLevel = 40;
    public int lastLevel = 1;
    [HideInInspector]
    public int currentLevel = 1;
    public Dictionary<int, int> starAtLevel = new Dictionary<int, int>();

    void Awake()
    {
        if (Master.LevelData == null)
        {
            Master.LevelData = this;
        }

        LoadLevelData();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void LoadLevelData()
    {
        TextAsset textAsset = Resources.Load("Data/Levels/LevelData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LevelDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.levelDataCollection = (LevelDataCollection)serializer.Deserialize(reader);
        }

        for (int i = 0; i < levelDataCollection.ListLevelData.Count; i++)
        {
            levelDataCollection.ListLevelData[i].LevelIndex = i + 1;
        }

        lastLevel = ObscuredPrefs.GetInt("LastLevel", 0);
        totalLevel = levelDataCollection.ListLevelData.Count;
        // lastLevel = 2;
    }

    public LevelData GetLevelDataByIndex(int levelIndex)
    {

        //foreach (LevelData levelData in levelDataCollection.ListLevelData)
        // {
        //   if (levelIndex == levelData.LevelIndex) return levelData;
        // }

        return levelDataCollection.ListLevelData[levelIndex - 1];
    }

    public int GetStarAtLevel(int level)
    {
        return ObscuredPrefs.GetInt("StarAtLevel_" + level, 0);
    }

    public void SetStarAtLevel(int level, int star)
    {
        ObscuredPrefs.SetInt("StarAtLevel_" + level, star);
        ObscuredPrefs.Save();
    }

    public void SetLastLevel(int level)
    {
        if (level > lastLevel)
        {
            lastLevel = level;
            ObscuredPrefs.SetInt("LastLevel", level);
            ObscuredPrefs.Save();
            Master.QuestData.SetProgressValue("02", lastLevel);
        }
    }

    public int GetTotalStarAtLevelsGot()
    {
        int totalStarAtLevels = 0;
        for(int i = 1; i <= lastLevel; i++)
        {
            totalStarAtLevels += GetStarAtLevel(i);
        }
        return totalStarAtLevels;
    }

    public void Save(LevelDataCollection levelDataCollection)
    {
        string path = Application.dataPath + "/Resources/Data/Levels/LevelData.xml";

        var serializer = new XmlSerializer(typeof(LevelDataCollection));
        using (var stream = new FileStream(path, FileMode.Create))
        {

            serializer.Serialize(stream, levelDataCollection);
            Debug.Log("Saved XML to " + path);
        }
    }


    //public bool CheckGotGemRewardAtLevelAndStarGot(int level, int starGotAtLevel)
    //{
    //    bool isGotReward = (ObscuredPrefs.GetInt("IsGotGemRewardAtLevelAndStar_" + level + "_" + starGotAtLevel, 0) == 0) ? false : true;
    //    return isGotReward;
    //}

    //public void SetGotGemRewardAtLevelAndStarGot(int level, int starGotAtLevel)
    //{
    //    ObscuredPrefs.SetInt("IsGotGemRewardAtLevelAndStar_" + level + "_" + starGotAtLevel, 1);
    //    ObscuredPrefs.Save();
    //}

}
