using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

public class UnitDataController : MonoBehaviour
{

    //Unit data
    [XmlRoot("UnitDataCollection")]
    public class UnitDataCollection
    {
        [XmlArray("Units")]
        [XmlArrayItem("Unit")]
        public List<UnitData> ListUnitsData = new List<UnitData>();
    }
    public UnitDataCollection unitDataCollection;

    [System.Serializable]
    public class UnitData
    {
        public string UnitID = "";
        public string UnitName = "";
        public float Health;
        public float Damage;
        public float AttackSpeed;
        public float Range;
        public float Critical;
        public int NumberOfAmmoToReload; //-1: no need reload
        public int NumberOfShootToUpgrade;
        public int Price;
        public float TimeCountdownSelect;
        public int UnlockAtLevel;
    }
    [HideInInspector]
    public UnitData unitData;
    public List<UnitData> listUnitData = new List<UnitData>();

    [System.Serializable]
    public class UnitUpgradeData
    {
        public string UnitID = "";
        public int Health;
        public int Damage;
        public int AttackSpeed;
        public int Range;
        public int Critical;
    }
    [HideInInspector]
    public UnitUpgradeData unitUpgradeData;
    [HideInInspector]
    public List<UnitUpgradeData> listUnitUpgradeData = new List<UnitUpgradeData>();

    public class UnitUpgradeType
    {
        public static string Health = "Health";
        public static string Damage = "Damage";
        public static string AttackSpeed = "AttackSpeed";
        public static string Range = "Range";
        public static string Critical = "Critical";
    }
    [HideInInspector]
    public List<string> listStatsItem = new List<string> { "Damage", "Health", "AttackSpeed", "Range", "Critical" };

    [HideInInspector]
    public List<UnitData> listUnitChoose = new List<UnitData>();
    //  [HideInInspector]
    public List<UnitData> listUnitAvailable = new List<UnitData>();

    public int totalUnit;

    //for upgrade setting
   // private float increasePercentPerUpgrade = 15;

    private int firstStarRequireUpgrade = 150;
    private float increaseStarPercentPerUpgrade = 100; //%

    [HideInInspector]
    public float maxTimeToShoot = 7; //second
    [HideInInspector]
    public float minTimeToShoot = 0.2f; //second

    Dictionary<string, float> increaseValuePercentPerUpgrade = new Dictionary<string, float>()
    {
        { UnitUpgradeType.Damage, 20},
        { UnitUpgradeType.Health, 20},
    //    { UnitUpgradeType.AttackSpeed, 8}, get another logic
        { UnitUpgradeType.Range, 8},
        { UnitUpgradeType.Critical, 10},
    };

    // private florat increasePercentInvolUpgradeStats = 15;

    void Awake()
    {
        if (Master.UnitData == null)
        {
            Master.UnitData = this;
        }
    }

    void Start()
    {
        LoadUnitData();
        LoadUnitAvaiable();
        LoadUnitUpgradeData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadUnitData()
    {
        listUnitData.Clear();
        TextAsset textAsset = Resources.Load("Data/Characters/UnitData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UnitDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.unitDataCollection = (UnitDataCollection)serializer.Deserialize(reader);
        }
        listUnitData = unitDataCollection.ListUnitsData;
        totalUnit = listUnitData.Count;
    }

    public void LoadUnitAvaiable()
    {
        listUnitAvailable.Clear();
        foreach (UnitData unitData in unitDataCollection.ListUnitsData)
        {
            if (unitData.UnlockAtLevel <= Master.LevelData.lastLevel + 1)
            {
                listUnitAvailable.Add(unitData);
            }
        }
    }

    public void LoadUnitUpgradeData()
    {
        listUnitUpgradeData.Clear();
        for (int i = 0; i < unitDataCollection.ListUnitsData.Count; i++)
        {
            string unitID = unitDataCollection.ListUnitsData[i].UnitID;
            UnitUpgradeData unitUpgradeData = new UnitUpgradeData();
            string firstParam = "Unit_" + unitID + "_Upgrade_";
            unitUpgradeData.UnitID = unitID;
            unitUpgradeData.Health = ObscuredPrefs.GetInt(firstParam + UnitUpgradeType.Health, 0);
            unitUpgradeData.Damage = ObscuredPrefs.GetInt(firstParam + UnitUpgradeType.Damage, 0);
            unitUpgradeData.AttackSpeed = ObscuredPrefs.GetInt(firstParam + UnitUpgradeType.AttackSpeed, 0);
            unitUpgradeData.Range = ObscuredPrefs.GetInt(firstParam + UnitUpgradeType.Range, 0);
            unitUpgradeData.Critical = ObscuredPrefs.GetInt(firstParam + UnitUpgradeType.Critical, 0);
            listUnitUpgradeData.Add(unitUpgradeData);
        }
    }

    public UnitData GetUnitDataByID(string id)
    {
        foreach (UnitData unitData in listUnitData)
        {
            if (id == unitData.UnitID)
            {
                return unitData;
            }
        }
        return null;
    }

    public UnitUpgradeData GetUnitUpgradeDataByID(string id)
    {
        foreach (UnitUpgradeData unitUpgradeData in listUnitUpgradeData)
        {
            if (id == unitUpgradeData.UnitID)
            {
                return unitUpgradeData;
            }
        }
        return null;
    }

    public UnitData GetUnitDataWithUpgradeByID(string unitID)
    {
        UnitData unitData = GetUnitDataByID(unitID);
        UnitData unitDataWithUpgrade = new UnitData();
        // withUpgrade = unitData;
        if (unitData == null) return null;

        UnitUpgradeData unitUpgradeData = GetUnitUpgradeDataByID(unitID);
        unitDataWithUpgrade.UnitID = unitData.UnitID;
        unitDataWithUpgrade.UnitName = unitData.UnitName;
        unitDataWithUpgrade.Health = Master.IncreaseValues(unitData.Health, unitUpgradeData.Health, increaseValuePercentPerUpgrade[UnitUpgradeType.Health]);
        unitDataWithUpgrade.Damage = Master.IncreaseValues(unitData.Damage, unitUpgradeData.Damage, increaseValuePercentPerUpgrade[UnitUpgradeType.Damage]);
        unitDataWithUpgrade.AttackSpeed = unitData.AttackSpeed + unitUpgradeData.AttackSpeed * CalculateAttackSpeedPerUpgrade(unitData.AttackSpeed);
        //unitDataWithUpgrade.AttackSpeed = Master.IncreaseValues(unitData.AttackSpeed, unitUpgradeData.AttackSpeed, increaseValuePercentPerUpgrade[UnitUpgradeType.AttackSpeed]);
        unitDataWithUpgrade.Range = Master.IncreaseValues(unitData.Range, unitUpgradeData.Range, increaseValuePercentPerUpgrade[UnitUpgradeType.Range]);
        unitDataWithUpgrade.Critical = Master.IncreaseValues(unitData.Critical, unitUpgradeData.Critical, increaseValuePercentPerUpgrade[UnitUpgradeType.Critical]);
        unitDataWithUpgrade.NumberOfAmmoToReload = unitData.NumberOfAmmoToReload;
        unitDataWithUpgrade.NumberOfShootToUpgrade = unitData.NumberOfShootToUpgrade;
        unitDataWithUpgrade.Price = unitData.Price;
        unitDataWithUpgrade.TimeCountdownSelect = unitData.TimeCountdownSelect;
        unitDataWithUpgrade.UnlockAtLevel = unitData.UnlockAtLevel;

        return unitDataWithUpgrade;
    }


    public UnitData GetUnitDataAfterUpgradeInGameplay(UnitData unitData, int times = 1)
    {
        unitData.Health = Master.IncreaseValues(unitData.Health, times, increaseValuePercentPerUpgrade[UnitUpgradeType.Health] * 0.6f);
        unitData.Damage = Master.IncreaseValues(unitData.Damage, times, increaseValuePercentPerUpgrade[UnitUpgradeType.Damage] * 0.6f);
        unitData.AttackSpeed = unitData.AttackSpeed + CalculateAttackSpeedPerUpgrade(unitData.AttackSpeed);
        //unitData.AttackSpeed = Master.IncreaseValues(unitData.AttackSpeed, times, increaseValuePercentPerUpgrade[UnitUpgradeType.AttackSpeed]);
        unitData.Range = Master.IncreaseValues(unitData.Range, times, increaseValuePercentPerUpgrade[UnitUpgradeType.Range]);
        unitData.Critical = Master.IncreaseValues(unitData.Critical, times, increaseValuePercentPerUpgrade[UnitUpgradeType.Critical]);
        return unitData;
    }



    public int GetCurrentUpgradedStats(string unitID, string upgradeType)
    {
        int currentUpgardeValue = 0;
        UnitUpgradeData currentUnitUpgradedData = GetUnitUpgradeDataByID(unitID);

        if (upgradeType == UnitUpgradeType.Health)
        {
            currentUpgardeValue = currentUnitUpgradedData.Health;
        }

        if (upgradeType == UnitUpgradeType.Damage)
        {
            currentUpgardeValue = currentUnitUpgradedData.Damage;
        }

        if (upgradeType == UnitUpgradeType.AttackSpeed)
        {
            currentUpgardeValue = currentUnitUpgradedData.AttackSpeed;
        }

        if (upgradeType == UnitUpgradeType.Range)
        {
            currentUpgardeValue = currentUnitUpgradedData.Range;
        }

        if (upgradeType == UnitUpgradeType.Critical)
        {
            currentUpgardeValue = currentUnitUpgradedData.Critical;
        }
        return currentUpgardeValue;
    }


    //float GetValueWithUpgrade(float value, int upgraded)
    //{
    //    for (int i = 0; i < upgraded; i++)
    //    {
    //        value += (float)(value * increasePercentPerUpgrade) / 100;
    //    }
    //    value = Mathf.Round(value * 10f) / 10f;
    //    return value;
    //}

    public UnitData doUpgradeUnitData(string unitID, string upgradeType)
    {
        string firstParam = "Unit_" + unitID + "_Upgrade_";
        int currentUpgardeValue = GetCurrentUpgradedStats(unitID, upgradeType) + 1;

        ObscuredPrefs.SetInt(firstParam + upgradeType, currentUpgardeValue);
        ObscuredPrefs.Save();

        Master.Stats.Star -= GetStarRequireUpgrade(unitID, upgradeType);

        LoadUnitUpgradeData();

        return GetUnitDataWithUpgradeByID(unitID);
    }

    public int GetStarRequireUpgrade(string unitID, string upgradeType)
    {
        //get current upgarde
        float value = (float)firstStarRequireUpgrade;
        int currentUpgradedValue = GetCurrentUpgradedStats(unitID, upgradeType);

        for (int i = 0; i < currentUpgradedValue; i++)
        {
            value += (float)(value * increaseStarPercentPerUpgrade) / 100;
        }
        return (int)value;
    }

    public UnitData[] GetUnlockUnitAtLevel(int level)
    {
        List<UnitData> listUnitUnlock = new List<UnitData>();
        UnitData[] unitsUnlock;
        foreach (UnitData unitData in unitDataCollection.ListUnitsData)
        {
            if (unitData.UnlockAtLevel == level)
            {
                listUnitUnlock.Add(unitData);
            }
        }

        if (listUnitUnlock.Count > 0)
        {
            unitsUnlock = new UnitData[listUnitUnlock.Count];
            for (int i = 0; i < listUnitUnlock.Count; i++)
            {
                unitsUnlock[i] = listUnitUnlock[i];
            }
            return unitsUnlock;
        }
        else
        {
            return null;
        }
    }

    public void CheckUnitUnlock(System.Action actionAfterClose = null)
    {
        UnitDataController.UnitData[] listUnitUnlock = Master.UnitData.GetUnlockUnitAtLevel(Master.LevelData.lastLevel + 1);
        if (listUnitUnlock == null)
        {
            if (actionAfterClose != null)
            {
                actionAfterClose();
            }
        }
        else
        {
            string listUnitUnlockStr = "";
            foreach (UnitDataController.UnitData unitData in listUnitUnlock)
            {
                listUnitUnlockStr += unitData.UnitID + "-";
            }
            Master.UIGameplay.ShowDialog(UIController.Dialog.ListDialogs.NewUnitUnlockDialog, 0.3f, new string[] { listUnitUnlockStr }, null, actionAfterClose);
        }
    }

    float CalculateAttackSpeedPerUpgrade(float currentAttackSpeed)
    {
        float valueNeedToMax = (maxTimeToShoot * 10) - currentAttackSpeed;
        return valueNeedToMax / 10;
    }

    public void Save(UnitDataCollection unitDataCollection)
    {
        string path = Application.dataPath + "/Resources/Data/Characters/UnitData.xml";
        var serializer = new XmlSerializer(typeof(UnitDataCollection));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, unitDataCollection);
            Debug.Log("Saved XML to " + path);
        }
    }

}
