using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

public class EnemyDataController : MonoBehaviour
{
    //Unit data
    [XmlRoot("EnemyDataCollection")]
    public class EnemyDataCollection
    {
        [XmlArray("Enemies")]
        [XmlArrayItem("Enemy")]
        public List<EnemyData> ListEnemiesData = new List<EnemyData>();
    }
    public EnemyDataCollection enemyDataCollection;

    [System.Serializable]
    public class EnemyData
    {
        public string EnemyID = "";
        public string EnemyName = "";
        public float Health;
        public float Damage;
        public float MoveSpeed;
        public float AttackSpeed;
        public float Range;
        public string Ability = "";
        public string CoinDrop = "";
    }

    public EnemyData enemyData;
    public List<EnemyData> listEnemyData = new List<EnemyData>();

    public class IncreaseStatsPerLevel
    {
        //%
        public static float damage = 1.5f;
        public static float health = 5f;
        public static float moveSpeed = 0.1f;
        public static float attackSpeed = 0.1f;
    }

    private float increasePercentPerLevel = 4f; //%

    public enum StatsType
    {
        Health, Damage, MoveSpeed, AttackSpeed, Range
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.EnemyData == null)
        {
            Master.EnemyData = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadEnemyData();
    }


    public void LoadEnemyData()
    {
        listEnemyData.Clear();
        TextAsset textAsset = Resources.Load("Data/Characters/EnemyData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EnemyDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.enemyDataCollection = (EnemyDataCollection)serializer.Deserialize(reader);
        }

        listEnemyData = enemyDataCollection.ListEnemiesData;
    }



    public EnemyData GetEnemyDataByID(string id)
    {
        foreach (EnemyData enemyData in listEnemyData)
        {
            if (id == enemyData.EnemyID)
            {
                return enemyData;
            }
        }
        return null;
    }


    public EnemyData GetEnemyDataWithUpgradeByID(string id)
    {
        return UpgradeEnemy(id, Master.LevelData.currentLevel - 1);
    }

    public EnemyData UpgradeEnemy(string enemyID, int times)
    {
        EnemyData enemyData = GetEnemyDataByID(enemyID);
        EnemyData enemyDataWithUpgrade = new EnemyData();
        enemyDataWithUpgrade.EnemyID = enemyData.EnemyID;
        enemyDataWithUpgrade.EnemyName = enemyData.EnemyName;
        enemyDataWithUpgrade.Health = Master.IncreaseValues(enemyData.Health, times, IncreaseStatsPerLevel.health);
        enemyDataWithUpgrade.Damage = Master.IncreaseValues(enemyData.Damage, times, IncreaseStatsPerLevel.damage);
        enemyDataWithUpgrade.MoveSpeed = Master.IncreaseValues(enemyData.MoveSpeed, times, IncreaseStatsPerLevel.moveSpeed);
        //enemyDataWithUpgrade.MoveSpeed = enemyData.MoveSpeed;
        enemyDataWithUpgrade.AttackSpeed = Master.IncreaseValues(enemyData.AttackSpeed, times, IncreaseStatsPerLevel.attackSpeed);
        enemyDataWithUpgrade.Range = enemyData.Range;
        enemyDataWithUpgrade.CoinDrop = enemyData.CoinDrop;
        return enemyDataWithUpgrade;
    }

    public bool IsEnemyUnlock(string enemyID)
    {
        return ObscuredPrefs.GetInt("EnemyUnlock_" + enemyID, 0) == 0 ? false : true;
    }

    public void UnlockEnemy(string enemyID)
    {
        ObscuredPrefs.SetInt("EnemyUnlock_" + enemyID, 1);
        ObscuredPrefs.Save();
    }

    public bool CheckAndUnlockEnemy(string enemyID)
    {
        if (!IsEnemyUnlock(enemyID))
        {
            UnlockEnemy(enemyID);
            return true;
        }
        return false;
    }

    public float CalcPercentStatsValue(StatsType statsType, float value)
    {
        float biggestValue = GetBiggestValueStats(statsType);

        return (value / biggestValue) * 100;
    }
    public float GetBiggestValueStats(StatsType statsType)
    {
        float value = 0;
        switch (statsType)
        {
            case StatsType.Health:
                value = listEnemyData[0].Health;
                break;
            case StatsType.Damage:
                value = listEnemyData[0].Damage;
                break;
            case StatsType.MoveSpeed:
                value = listEnemyData[0].MoveSpeed;
                break;
            case StatsType.AttackSpeed:
                value = listEnemyData[0].AttackSpeed;
                break;
        }

        foreach (EnemyData enemyData in listEnemyData)
        {
            float newValue = 0;
            switch (statsType)
            {
                case StatsType.Health:
                    newValue = enemyData.Health;
                    break;
                case StatsType.Damage:
                    newValue = enemyData.Damage;
                    break;
                case StatsType.MoveSpeed:
                    newValue = enemyData.MoveSpeed;
                    break;
                case StatsType.AttackSpeed:
                    newValue = enemyData.AttackSpeed;
                    break;
            }

            if (newValue > value)
            {
                value = newValue;
            }
        }
        return value;
    }

    public void Save(EnemyDataCollection enemyDataCollection)
    {
        string path = Application.dataPath + "/Resources/Data/Characters/EnemyData.xml";
        var serializer = new XmlSerializer(typeof(EnemyDataCollection));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, enemyDataCollection);
            Debug.Log("Saved XML to " + path);
        }
    }
}
