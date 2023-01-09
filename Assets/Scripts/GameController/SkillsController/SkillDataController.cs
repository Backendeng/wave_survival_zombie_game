using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class SkillDataController : MonoBehaviour
{

    //Unit data
    [XmlRoot("SkillDataCollection")]
    public class SkillDataCollection
    {
        [XmlArray("Skills")]
        [XmlArrayItem("Skill")]
        public List<SkillData> ListSkillsData = new List<SkillData>();
    }
    public SkillDataCollection skillDataCollection;

    [System.Serializable]
    public class SkillData
    {
        public string SkillID = "";
        public string SkillName = "";
        public float TimeCountdown;
        public int UnlockAtLevel;
    }
    [HideInInspector]
    public SkillData skillData;
    public List<SkillData> listSkillsData = new List<SkillData>();

    [System.Serializable]
    public class Skill_01_Data : SkillData
    {
        public float Damage = 30;
    }
    [HideInInspector]
    public Skill_01_Data skill_01_data;

    [System.Serializable]
    public class Skill_02_Data : SkillData
    {
        public float FreezeTime = 2;
        public float Raridus = 25;
    }
    [HideInInspector]
    public Skill_02_Data skill_02_data;

    [System.Serializable]
    public class Skill_03_Data : SkillData
    {
        public float Range = 40;
        public float Damage = 40;
    }
    [HideInInspector]
    public Skill_03_Data skill_03_data;

    public List<SkillData> listSkillsAvaiable = new List<SkillData>();

    public Dictionary<string, int> listUpgradeSkill = new Dictionary<string, int>();


    public int totalSkill;

    //for upgrade
    private float increasePercentPerUpgrade = 20;
    private int firstGemRequireUpgrade = 20;
    private float increasePercentGemPerUpgrade = 90; //%

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.SkillData == null)
        {
            Master.SkillData = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {
        LoadAllSkillData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadAllSkillData()
    {
        LoadSkillData();
        LoadSkillsAvaiable();
        LoadUpgradeSkill();
        SetSkillWithUpgrade();
    }

    public void LoadSkillData()
    {
        listSkillsData.Clear();
        TextAsset textAsset = Resources.Load("Data/Skills/SkillData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SkillDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.skillDataCollection = (SkillDataCollection)serializer.Deserialize(reader);
        }
        listSkillsData = skillDataCollection.ListSkillsData;
        totalSkill = listSkillsData.Count;

        //skill 01
        skill_01_data.SkillID = "01";
        skill_01_data.SkillName = listSkillsData[0].SkillName;
        skill_01_data.TimeCountdown = listSkillsData[0].TimeCountdown;
        skill_01_data.UnlockAtLevel = listSkillsData[0].UnlockAtLevel;

        //skill 02
        skill_02_data.SkillID = "02";
        skill_02_data.SkillName = listSkillsData[1].SkillName;
        skill_02_data.TimeCountdown = listSkillsData[1].TimeCountdown;
        skill_02_data.UnlockAtLevel = listSkillsData[1].UnlockAtLevel;

        //skill 03
        skill_03_data.SkillID = "03";
        skill_03_data.SkillName = listSkillsData[2].SkillName;
        skill_03_data.TimeCountdown = listSkillsData[2].TimeCountdown;
        skill_03_data.UnlockAtLevel = listSkillsData[2].UnlockAtLevel;

    }

    public void LoadSkillsAvaiable()
    {
        listSkillsAvaiable.Clear();
        foreach (SkillData skillData in listSkillsData)
        {
            if (skillData.UnlockAtLevel <= Master.LevelData.lastLevel + 1)
            {
                listSkillsAvaiable.Add(skillData);
            }
        }
    }

    public void LoadUpgradeSkill()
    {
        listUpgradeSkill.Clear();
        foreach (SkillData skillData in listSkillsData)
        {
            int getUpgrade = ObscuredPrefs.GetInt("UpgradeSkill_" + skillData.SkillID, 0);
            listUpgradeSkill.Add(skillData.SkillID, getUpgrade);
        }
    }

    public SkillData GetSkillDataByID(string skillID)
    {
        foreach (var item in listSkillsData)
        {
            if (item.SkillID == skillID)
            {
                return item;
            }
        }
        return null;
    }

    public SkillData[] GetUnlockSkillAtLevel(int level)
    {
        List<SkillData> listSkillsUnlock = new List<SkillData>();
        SkillData[] skillsUnlock;
        foreach (SkillData skillData in skillDataCollection.ListSkillsData)
        {
            if (skillData.UnlockAtLevel == level)
            {
                listSkillsUnlock.Add(skillData);
            }
        }

        if (listSkillsUnlock.Count > 0)
        {
            skillsUnlock = new SkillData[listSkillsUnlock.Count];
            for (int i = 0; i < listSkillsUnlock.Count; i++)
            {
                skillsUnlock[i] = listSkillsUnlock[i];
            }
            return skillsUnlock;
        }
        else
        {
            return null;
        }
    }

    public void CheckSkillUnlock(System.Action actionAfterClose = null)
    {
        SkillData[] listSkillsUnlock = GetUnlockSkillAtLevel(Master.LevelData.lastLevel + 1);
        if (listSkillsUnlock == null)
        {
            if (actionAfterClose != null)
            {
                actionAfterClose();
            }
        }
        else
        {
            string listSkillsUnlockStr = "";
            foreach (SkillData skillData in listSkillsUnlock)
            {
                listSkillsUnlockStr += skillData.SkillID + "-";
            }
            Master.UIGameplay.ShowDialog(UIController.Dialog.ListDialogs.NewSkillUnlockDialog, 0.3f, new string[] { listSkillsUnlockStr }, null, actionAfterClose);
        }
    }

    public int GetUpgradeSkillByID(string skillID)
    {
        return listUpgradeSkill[skillID];
    }

    public void doUpgradeSkill(string skillID)
    {
        int currentUpgarde = listUpgradeSkill[skillID];
        int upgradeValue = currentUpgarde + 1;
        ObscuredPrefs.SetInt("UpgradeSkill_" + skillID, upgradeValue);
        ObscuredPrefs.Save();
        Master.Stats.Gem -= GetGemRequireUpgrade(skillID);
        LoadAllSkillData();
    }

    public void SetSkillWithUpgrade()
    {
        //skill 01
        int upgraded = listUpgradeSkill["01"];
        Skill_01_Data skill01Data = new Skill_01_Data();
        skill_01_data.Damage = GetValueWithUpgrade(skill01Data.Damage, upgraded);

        //skill 02
        upgraded = listUpgradeSkill["02"];
        Skill_02_Data skill02Data = new Skill_02_Data();
        skill_02_data.FreezeTime = GetValueWithUpgrade(skill02Data.FreezeTime, upgraded);
        skill_02_data.Raridus = GetValueWithUpgrade(skill02Data.Raridus, upgraded);

        //skill 02
        upgraded = listUpgradeSkill["03"];
        Skill_03_Data skill03Data = new Skill_03_Data();
        skill_03_data.Damage = GetValueWithUpgrade(skill03Data.Damage, upgraded);
        skill_03_data.Range = GetValueWithUpgrade(skill03Data.Range, upgraded);

    }

    float GetValueWithUpgrade(float value, int upgraded)
    {
        for (int i = 0; i < upgraded; i++)
        {
            value += (float)(value * increasePercentPerUpgrade) / 100;
        }
        value = Mathf.Round(value * 10f) / 10f;
        return value;
    }

    public int GetGemRequireUpgrade(string skillID)
    {
        float value = (float)firstGemRequireUpgrade;
        int currentUpgrade = GetUpgradeSkillByID(skillID);
        for (int i = 0; i < currentUpgrade; i++)
        {
            value += (float)(value * increasePercentGemPerUpgrade) / 100;
        }
        return (int)value;
    }

}
