using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillPanelController : MonoBehaviour
{


    public class Skill_Assign
    {
        public GameObject skillPanel;
        public UITexture iconTexture;
        public GameObject skillInfo;
        public UILabel unlockAtLevelLabel;
        public UILabel timeCountdownLabel;
        public GameObject upgradeProgress;
        public List<UITexture> listUpgradeProgressTexture = new List<UITexture>();
        public UIPanel upgradeSkillButtonPanel;
        public UILabel gemRequireLabel;
    }

    public class Skill_01_Assign : Skill_Assign
    {
        public static UILabel damageLabel;
    }

    public class Skill_02_Assign : Skill_Assign
    {
        public static UILabel freezeTimeLabel;
        public static UILabel radiusEffectLabel;
    }

    public class Skill_03_Assign : Skill_Assign
    {
        public static UILabel damageLabel;
        public static UILabel rangeLabel;
    }

    public List<Skill_Assign> listSkillAssign = new List<Skill_Assign>()
    {
        new Skill_01_Assign(),
        new Skill_02_Assign(),
        new Skill_03_Assign(),
    };

    public bool isAssign;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        // AssignObject();
    }

    public void OnOpen()
    {
        //AssignObject();
        SetInfo();
    }

    public void AssignObject()
    {
        if (isAssign) return;
        isAssign = true;

        Debug.Log("Assign Skill Panel");

        for (int i = 0; i < listSkillAssign.Count; i++)
        {
            string skillID = "0" + (i + 1);
            listSkillAssign[i].skillPanel = Master.GetChildByName(gameObject, "Skill_" + skillID);
            listSkillAssign[i].iconTexture = Master.GetChildByName(listSkillAssign[i].skillPanel, "Icon").GetComponent<UITexture>();
            listSkillAssign[i].skillInfo = Master.GetChildByName(listSkillAssign[i].skillPanel, "SkillInfo");
            listSkillAssign[i].unlockAtLevelLabel = Master.GetChildByName(listSkillAssign[i].skillPanel, "UnlockAtLevel").GetComponent<UILabel>();
            listSkillAssign[i].timeCountdownLabel = Master.GetChildByName(listSkillAssign[i].skillPanel, "TimeCountdownValueLabel").GetComponent<UILabel>();
            listSkillAssign[i].upgradeProgress = Master.GetChildByName(listSkillAssign[i].skillPanel, "UpgradeProgress");
            listSkillAssign[i].listUpgradeProgressTexture.Clear();
            foreach (Transform item in listSkillAssign[i].upgradeProgress.transform)
            {
                listSkillAssign[i].listUpgradeProgressTexture.Add(item.gameObject.GetComponent<UITexture>());
            }
            listSkillAssign[i].upgradeSkillButtonPanel = Master.GetChildByName(listSkillAssign[i].skillPanel, "UpgradeButton").GetComponent<UIPanel>();
            listSkillAssign[i].gemRequireLabel = Master.GetChildByName(listSkillAssign[i].upgradeSkillButtonPanel.gameObject, "GemRequireValueLabel").GetComponent<UILabel>();
        }

        //skill 01
        Skill_01_Assign.damageLabel = Master.GetChildByName(listSkillAssign[0].skillPanel, "DamageValueLabel").GetComponent<UILabel>();

        //skill 02
        Skill_02_Assign.freezeTimeLabel = Master.GetChildByName(listSkillAssign[1].skillPanel, "FreezeTimeValueLabel").GetComponent<UILabel>();
        Skill_02_Assign.radiusEffectLabel = Master.GetChildByName(listSkillAssign[1].skillPanel, "RadiusEffectValueLabel").GetComponent<UILabel>();

        //skill 03
        Skill_03_Assign.damageLabel = Master.GetChildByName(listSkillAssign[2].skillPanel, "DamageValueLabel").GetComponent<UILabel>();
        Skill_03_Assign.rangeLabel = Master.GetChildByName(listSkillAssign[2].skillPanel, "RangeValueLabel").GetComponent<UILabel>();
    }


    public void SetInfo()
    {

        for (int i = 0; i < listSkillAssign.Count; i++)
        {
            bool isUnlock = false;
            if ((Master.LevelData.lastLevel + 1) >= Master.SkillData.listSkillsData[i].UnlockAtLevel)
            {
                isUnlock = true;
            }

            string skillID = "0" + (i + 1);
            listSkillAssign[i].timeCountdownLabel.text = Master.SkillData.listSkillsData[i].TimeCountdown.ToString();
            SetProgressTexture(Master.SkillData.listUpgradeSkill[skillID], listSkillAssign[i].listUpgradeProgressTexture);
            SetUpgradeButton(Master.SkillData.GetGemRequireUpgrade(skillID), listSkillAssign[i].upgradeSkillButtonPanel, Master.SkillData.listUpgradeSkill[skillID], isUnlock);

            if (isUnlock)
            {
                listSkillAssign[i].iconTexture.mainTexture = Resources.Load<Texture2D>("Textures/Skills/Skill_" + skillID + "/Skill_" + skillID + "_Icon");
                listSkillAssign[i].skillInfo.SetActive(true);
                listSkillAssign[i].unlockAtLevelLabel.gameObject.SetActive(false);
                listSkillAssign[i].gemRequireLabel.text = Master.SkillData.GetGemRequireUpgrade(skillID).ToString();
            }
            else
            {

                listSkillAssign[i].iconTexture.mainTexture = Resources.Load<Texture2D>("Textures/Skills/skill_locked");
                listSkillAssign[i].skillInfo.SetActive(false);
                listSkillAssign[i].unlockAtLevelLabel.gameObject.SetActive(true);
                listSkillAssign[i].unlockAtLevelLabel.text = "Unlock at level " + Master.SkillData.listSkillsData[i].UnlockAtLevel;
                listSkillAssign[i].gemRequireLabel.text = "0";
            }
        }

        //skill 01
        Skill_01_Assign.damageLabel.text = Master.SkillData.skill_01_data.Damage.ToString();

        //skill 02
        Skill_02_Assign.freezeTimeLabel.text = Master.SkillData.skill_02_data.FreezeTime.ToString();
        Skill_02_Assign.radiusEffectLabel.text = Master.SkillData.skill_02_data.Raridus.ToString();

        //skill 03
        Skill_03_Assign.damageLabel.text = Master.SkillData.skill_03_data.Damage.ToString();
        Skill_03_Assign.rangeLabel.text = Master.SkillData.skill_03_data.Range.ToString();

    }


    private void SetProgressTexture(int value, List<UITexture> listTexture)
    {
        //clear all progress
        foreach (UITexture item in listTexture)
        {
            item.mainTexture = Resources.Load<Texture2D>("Textures/UI/upgrade_progress_icon_disable");
        }

        for (int i = 0; i < value; i++)
        {
            listTexture[i].mainTexture = Resources.Load<Texture2D>("Textures/UI/upgrade_progress_icon_enable");
        }
    }

    public bool SetUpgradeButton(int requireGem, UIPanel upgradeButtonPanel, int currentUpgradeProgress, bool isUnlock)
    {
        if (requireGem > Master.Stats.Gem || currentUpgradeProgress >= 15 || !isUnlock)
        {
            upgradeButtonPanel.alpha = 0.6f;
            upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            upgradeButtonPanel.gameObject.GetComponentInChildren<UIButton>().enabled = false;
            return false;
        }
        else
        {
            upgradeButtonPanel.alpha = 1f;
            upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
            upgradeButtonPanel.gameObject.GetComponentInChildren<UIButton>().enabled = false;

            return true;
        }
    }

    public void UpgardeSkillButton_OnClick(GameObject button)
    {
        string skillID = button.transform.parent.parent.name.Split('_')[1];
        Master.SkillData.doUpgradeSkill(skillID);
        SetInfo();
    }

}


