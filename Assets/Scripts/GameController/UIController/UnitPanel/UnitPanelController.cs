using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitPanelController : MonoBehaviour
{

    // Use this for initialization
    public string unitSelectedID = "01";
    private UnitDataController.UnitData unitData;
    private GameObject unitShow;

    private GameObject unitPanel;
    private GameObject unitSelectPanel;

    private bool isSetListUnit;

    //for unit preview
    public class UnitPreview
    {
        public static GameObject unitPreviewPanel;
        public static GameObject unitShowPanel;
        public static UILabel unitNameLabel;
        public static UILabel unitPriceLabel;
        public static UILabel unitTimeCountdownLabel;
        public static UILabel unitAmmoLabel;
        public static UILabel unitNumberOfShootToUpgradeLabel;
    }


    public class UpgradeStats
    {
        public static GameObject upgradeStatsPanel;
        public static GameObject upgradeStatsDetailPanel;
        public static Dictionary<string, UpgradeStatsItemDetail> listUpgradeStatsDetail = new Dictionary<string, UpgradeStatsItemDetail>();
    }

    public class UpgradeStatsItemDetail
    {
        public UITexture icon;
        public UILabel title;
        public UILabel statsValueLabel;
        public UILabel starRequire;
        public UIPanel upgradeButtonPanel;
        public UILabel alertLabel;
        public GameObject upgradeProgress;
        public List<UITexture> listUpgradeProgressTexture = new List<UITexture>();
    }

    private bool isAssign;

    private GameObject pf_unitSelectButton;
    private List<GameObject> listUnitSelectButton = new List<GameObject>();

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        //AssignObject();
    }

    public void OnOpen()
    {
        SetListUnit();
        SetInfo();
    }

    void AssignObject()
    {
        if (isAssign) return;
        isAssign = true;

        Debug.Log("Assign Unit Panel");
        //unitPanel = Master.UIMenu.panels[1];
        unitPanel = gameObject;
        //Master.UIMenu.panels[1] = unitPanel;
        unitSelectPanel = Master.GetChildByName(Master.GetChildByName(unitPanel, "UnitSelect"), "Units");
        pf_unitSelectButton = Master.GetGameObjectInPrefabs("UI/UnitSelect_UnitPanel");

        //for unit preview
        UnitPreview.unitPreviewPanel = Master.GetChildByName(unitPanel, "UnitPreviewPanel");
        UnitPreview.unitShowPanel = Master.GetChildByName(unitPanel, "UnitShow");
        UnitPreview.unitNameLabel = Master.GetChildByName(unitPanel, "UnitNameLabel").GetComponent<UILabel>();
        UnitPreview.unitPriceLabel = Master.GetChildByName(unitPanel, "UnitPriceLabel").GetComponent<UILabel>();
        UnitPreview.unitTimeCountdownLabel = Master.GetChildByName(unitPanel, "UnitTimeCountdownLabel").GetComponent<UILabel>();
        UnitPreview.unitAmmoLabel = Master.GetChildByName(unitPanel, "UnitAmmoLabel").GetComponent<UILabel>();
        UnitPreview.unitNumberOfShootToUpgradeLabel = Master.GetChildByName(unitPanel, "NumberOfShootToUpgradeLabel").GetComponent<UILabel>();

        //for upgrade stats
        UpgradeStats.upgradeStatsPanel = Master.GetChildByName(unitPanel, "UpgradeStats");
        UpgradeStats.upgradeStatsDetailPanel = Master.GetChildByName(UpgradeStats.upgradeStatsPanel, "Stats");
        GameObject pf_upgradeItem = Master.GetGameObjectInPrefabs("UI/StatsUpgradeItem");
        UpgradeStats.listUpgradeStatsDetail.Clear();

        foreach (string statsItem in Master.UnitData.listStatsItem)
        {
            GameObject upgradeItem = NGUITools.AddChild(UpgradeStats.upgradeStatsDetailPanel, pf_upgradeItem);
            upgradeItem.name = statsItem;
            UpgradeStatsItemDetail upgradeStatsItem = new UpgradeStatsItemDetail();
            upgradeStatsItem.icon = Master.GetChildByName(upgradeItem.gameObject, "IconStats").GetComponent<UITexture>();
            upgradeStatsItem.icon.mainTexture = Resources.Load<Texture2D>("Textures/UI/UnitStats/" + statsItem);
            upgradeStatsItem.title = Master.GetChildByName(upgradeItem.gameObject, "TitleLabel").GetComponent<UILabel>();
            upgradeStatsItem.title.text = statsItem;
            upgradeStatsItem.statsValueLabel = Master.GetChildByName(upgradeItem.gameObject, "StatsValueLabel").GetComponent<UILabel>();
            upgradeStatsItem.starRequire = Master.GetChildByName(upgradeItem.gameObject, "StarRequireValueLabel").GetComponent<UILabel>();
            upgradeStatsItem.upgradeButtonPanel = Master.GetChildByName(upgradeItem.gameObject, "UpgradeButton").GetComponent<UIPanel>();
            upgradeStatsItem.alertLabel = Master.GetChildByName(upgradeItem.gameObject, "AlertLabel").GetComponent<UILabel>();
            upgradeStatsItem.alertLabel.gameObject.SetActive(false);
            upgradeStatsItem.upgradeProgress = Master.GetChildByName(upgradeItem.gameObject, "UpgradeProgress");

            foreach (Transform itemUpgradeProgress in upgradeStatsItem.upgradeProgress.transform)
            {
                upgradeStatsItem.listUpgradeProgressTexture.Add(itemUpgradeProgress.gameObject.GetComponent<UITexture>());
            }
            UpgradeStats.listUpgradeStatsDetail.Add(upgradeItem.gameObject.name, upgradeStatsItem);
        }
        UpgradeStats.upgradeStatsDetailPanel.GetComponent<UIGrid>().Reposition();

    }

    public void SetInfo()
    {
        SetListUnit();
        SetUnitPreview();
        SetStatsAndUpgradeInfo();
    }

    void SetListUnit()
    {
        if (isSetListUnit) return;

        isSetListUnit = true;
        listUnitSelectButton.Clear();

        //clear gameobject

        if (unitSelectPanel.transform.childCount > 0)
        {
            foreach (Transform item in unitSelectPanel.transform)
            {
                unitSelectPanel.GetComponent<UIGrid>().RemoveChild(item);
                Destroy(item.gameObject);
            }
        }
        unitSelectPanel.GetComponent<UIGrid>().Reposition();

        foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitData)
        {
            GameObject unit = NGUITools.AddChild(unitSelectPanel, pf_unitSelectButton);

            UITexture icon = Master.GetChildByName(unit, "Icon").GetComponent<UITexture>();
            UILabel unlockAtLevel = Master.GetChildByName(unit, "UnlockAtLevelValue").GetComponent<UILabel>();
            GameObject lockIcon = Master.GetChildByName(unit, "LockIcon");

            string name = "";

            if (unitData.UnlockAtLevel <= (Master.LevelData.lastLevel + 1))
            {
                icon.mainTexture = Resources.Load<Texture2D>("Textures/Characters/Units/Unit_" + unitData.UnitID + "/Icon_Upgrade");
                unlockAtLevel.transform.parent.gameObject.SetActive(false);
                lockIcon.SetActive(false);
                name = "Unit_" + unitData.UnitID + "_Unlocked";
                listUnitSelectButton.Add(unit);

            }
            else
            {
                icon.mainTexture = Resources.Load<Texture2D>("Textures/Characters/Units/unit_lock");
                //icon.mainTexture = Resources.Load<Texture2D>("Textures/Characters/Units/Unit_" + unitData.UnitID + "/Icon_Upgrade");
                icon.color = new Color(110 / 255f, 110 / 255f, 110 / 255f, 1f);
                lockIcon.SetActive(false);

                unlockAtLevel.transform.parent.gameObject.SetActive(false);
                unlockAtLevel.text = unitData.UnlockAtLevel.ToString();
                name = "Unit_" + unitData.UnitID + "_Lock";
            }
            unit.name = name;
        }
        unitSelectPanel.GetComponent<UIGrid>().Reposition();
        unitSelectedID = listUnitSelectButton[0].name.Split('_')[1];

        SetUnitSelectButtonTexture();
    }



    private void SetUnitPreview()
    {
        if (unitShow != null)
        {
            Destroy(unitShow);
        }

        this.unitData = Master.UnitData.GetUnitDataWithUpgradeByID(unitSelectedID);
        unitShow = NGUITools.AddChild(UnitPreview.unitShowPanel, Master.GetUnitPrefabByID(unitSelectedID));
        unitShow.transform.Find("Sprite").gameObject.layer = LayerMask.NameToLayer("UI");

        UnitPreview.unitNameLabel.text = unitData.UnitName;
        UnitPreview.unitPriceLabel.text = unitData.Price.ToString();
        UnitPreview.unitTimeCountdownLabel.text = unitData.TimeCountdownSelect.ToString();
        UnitPreview.unitAmmoLabel.text = (unitData.NumberOfAmmoToReload != -1 ? unitData.NumberOfAmmoToReload.ToString() : "No");
        UnitPreview.unitNumberOfShootToUpgradeLabel.text = unitData.NumberOfShootToUpgrade.ToString();
    }


    #region Set Stats And Upgrade Info
    private void SetStatsAndUpgradeInfo()
    {
        UpgradeStats.listUpgradeStatsDetail["Range"].alertLabel.gameObject.SetActive(false);
        UpgradeStats.listUpgradeStatsDetail["Range"].upgradeProgress.SetActive(true);
        UpgradeStats.listUpgradeStatsDetail["Range"].upgradeButtonPanel.alpha = 1f;
        UpgradeStats.listUpgradeStatsDetail["Range"].upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;

        //set stats and upgrade info
        UnitDataController.UnitUpgradeData unitUpgradeData = Master.UnitData.GetUnitUpgradeDataByID(unitSelectedID);
        List<float> listStatsValue = new List<float>
        {
            unitData.Damage, unitData.Health, unitData.AttackSpeed, unitData.Range, unitData.Critical
        };

        List<int> listStatsUpgradeValue = new List<int>
        {
            unitUpgradeData.Damage, unitUpgradeData.Health, unitUpgradeData.AttackSpeed, unitUpgradeData.Range, unitUpgradeData.Critical
        };

        for (int i = 0; i < Master.UnitData.listStatsItem.Count; i++)
        {
            UpgradeStats.listUpgradeStatsDetail[Master.UnitData.listStatsItem[i]].statsValueLabel.text = listStatsValue[i].ToString();
            string starRequire = "";

            if (listStatsUpgradeValue[i] < 10)
            {
                starRequire = Master.UnitData.GetStarRequireUpgrade(unitSelectedID, Master.UnitData.listStatsItem[i]).ToString();
            }
            else
            {
                starRequire = "Max";
            }

            UpgradeStats.listUpgradeStatsDetail[Master.UnitData.listStatsItem[i]].starRequire.text = starRequire;
            SetUpgradeButton(Master.UnitData.GetStarRequireUpgrade(unitSelectedID, Master.UnitData.listStatsItem[i]), UpgradeStats.listUpgradeStatsDetail[Master.UnitData.listStatsItem[i]].upgradeButtonPanel, listStatsUpgradeValue[i]);
            SetProgressUpgrade(listStatsUpgradeValue[i], UpgradeStats.listUpgradeStatsDetail[Master.UnitData.listStatsItem[i]].listUpgradeProgressTexture);
        }

        //some units can not upgrade range

        if (unitData.UnitID == "02" || unitData.UnitID == "06" || unitData.UnitID == "08")
        {
            UpgradeStats.listUpgradeStatsDetail["Range"].alertLabel.text = "This Unit can not upgrade this item";
            UpgradeStats.listUpgradeStatsDetail["Range"].alertLabel.gameObject.SetActive(true);
            UpgradeStats.listUpgradeStatsDetail["Range"].upgradeProgress.SetActive(false);
            UpgradeStats.listUpgradeStatsDetail["Range"].upgradeButtonPanel.alpha = 0.6f;
            UpgradeStats.listUpgradeStatsDetail["Range"].upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            UpgradeStats.listUpgradeStatsDetail["Range"].starRequire.text = "0";
        }


    }


    private void SetProgressUpgrade(int value, List<UITexture> listProgressTextures)
    {
        //clear all progress
        foreach (UITexture item in listProgressTextures)
        {
            item.mainTexture = Resources.Load<Texture2D>("Textures/UI/upgrade_progress_icon_disable");
        }

        for (int i = 0; i < value; i++)
        {
            listProgressTextures[i].mainTexture = Resources.Load<Texture2D>("Textures/UI/upgrade_progress_icon_enable");
        }
    }

    public bool SetUpgradeButton(int requireStar, UIPanel upgradeButtonPanel, int currentUpgradeProgress)
    {
        if (requireStar > Master.Stats.Star || currentUpgradeProgress >= 10)
        {
            upgradeButtonPanel.alpha = 0.6f;
            upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            return false;
        }
        else
        {
            upgradeButtonPanel.alpha = 1f;
            upgradeButtonPanel.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
            return true;
        }
    }

    public void UpgradeButton_OnClick(GameObject button)
    {

        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.UpgradeStatsOfUnit);

        string upgradeType = button.transform.parent.parent.parent.name;
        unitData = Master.UnitData.doUpgradeUnitData(unitSelectedID, upgradeType);
        SetStatsAndUpgradeInfo();
    }

    #endregion

    private void SetUnitSelectButtonTexture()
    {
        foreach (GameObject item in listUnitSelectButton)
        {
            if (item.name.Split('_')[1] == unitSelectedID)
            {
                item.GetComponentInChildren<UITexture>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                item.GetComponentInChildren<UITexture>().color = new Color(146 / 255f, 146 / 255f, 146 / 255f, 1f);
            }
        }
    }

    public void UnitSelect_OnClick(GameObject unitSelectButton)
    {
        string unitID = unitSelectButton.transform.parent.name.Split('_')[1];
        bool isUnlocked = unitSelectButton.transform.parent.name.Split('_')[2] == "Unlocked" ? true : false;
        if (isUnlocked)
        {
            unitSelectedID = unitID;
            SetInfo();
        }

        SetUnitSelectButtonTexture();
    }



}
