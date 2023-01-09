using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class UIControllerMenu : UIController
{

    // Use this for initialization

    private UILabel starLabel;
    private UILabel gemLabel;
    private UILabel energyLabel;

    public GameObject[] panels = new GameObject[2];
    private GameObject backButton;
    [HideInInspector]
    public UILabel titlePanel;
    [HideInInspector]
    public UILabel timeCountdownEnergyLabel;
    GameObject plusEnergyButton;
    public int currentPanel;



    Dictionary<int, string> titleOfPanel = new Dictionary<int, string>()
    {
        { 0, "Town Center"},
        { 1, "Units"},
        { 2, "Skills"},
        { 3, "Quests"},
        { 4, "Shop"},
        { 5, "Library"},
    };


    public override void OnAwake()
    {
        if (Master.UI == null)
        {
            Master.UI = this;
        }

        Master.UIMenu = this;
        Master.UIGameplay = null;
    }

    public override void AssignObject()
    {
        starLabel = Master.GetChildByName(uiRoot, "StarLabel").GetComponent<UILabel>();
        gemLabel = Master.GetChildByName(uiRoot, "GemLabel").GetComponent<UILabel>();
        energyLabel = Master.GetChildByName(uiRoot, "EnergyLabel").GetComponent<UILabel>();
        timeCountdownEnergyLabel = Master.GetChildByName(uiRoot, "TimeCountdownEnergy").GetComponent<UILabel>();

        backButton = Master.GetChildByName(uiRoot, "BackButton");
        titlePanel = Master.GetChildByName(uiRoot, "TitleUpperBar").GetComponent<UILabel>();
        plusEnergyButton = Master.GetChildByName(uiRoot, "PlusEnergyButton");
        //assign panel
        //panels[0] = Master.GetChildByName(uiRoot, "HomePanel");
        //panels[1] = Master.GetChildByName(uiRoot, "UnitPanel");
    }

    public override void OnStart()
    {
        Time.timeScale = 1;
        foreach (var item in panels)
        {
            item.SetActive(true);
        }

        OpenPanel(Master.defaultPanelMenu, "", false);
        Master.defaultPanelMenu = 0;
        Master.Audio.PlayBackgroundMusic("bg_menu", 1, false);
        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.UpgradeUnitInGameplay);
    }

    public override void OnUpdate()
    {
        starLabel.text = Master.Stats.Star.ToString();
        gemLabel.text = Master.Stats.Gem.ToString();
        energyLabel.text = Master.Stats.Energy + "/" + Master.Stats.MaxEnergy;
        //countdown energy
        if (Master.Stats.Energy < Master.Stats.MaxEnergy)
        {
            timeCountdownEnergyLabel.gameObject.SetActive(true);
            plusEnergyButton.SetActive(true);
            int totalSecondRemain = Master.Stats.timeRemainingCountdownEnergy;
            int minute = totalSecondRemain / 60;
            int second = totalSecondRemain % 60;
            timeCountdownEnergyLabel.text = (minute < 10 ? "0" + minute.ToString() : minute.ToString()) + ":" + (second < 10 ? "0" + second.ToString() : second.ToString());
        }
        else
        {
            timeCountdownEnergyLabel.gameObject.SetActive(false);
            plusEnergyButton.SetActive(false);
        }
    }


    public void OpenPanel_OnClick(GameObject button)
    {
        Master.PlaySoundButtonClick();
        switch (button.name)
        {
            case "UnitButton":
                if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.UpgradeStatsOfUnit)
                {
                    Master.Tutorial.GoToNextStep();
                }
                OpenPanel(1);
                break;
            case "SkillButton":
                OpenPanel(2);
                break;
            case "QuestButton":
                OpenPanel(3);
                break;
            case "ShopButton":
                OpenPanel(4);
                break;
            case "LibraryButton":
                OpenPanel(5);
                break;
            case "BackButton":
                if (currentPanel != 0)
                {
                    OpenPanel(0);
                }
                else
                {
                    GoToFirstMenu();
                }
                break;
        }
    }

    public void OpenPanel(int index, string titlePanelStr = "", bool isTransition = true)
    {
        if (isTransition)
        {
            Transition(() =>
            {

                foreach (GameObject panel in panels)
                {
                    panel.SetActive(false);
                }

                panels[index].SetActive(true);
                panels[index].SendMessage("OnOpen", SendMessageOptions.DontRequireReceiver);
                if (titlePanelStr == "")
                {
                    titlePanel.text = titleOfPanel[index];
                }

            }, null);
        }
        else
        {

            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            }

            panels[index].SetActive(true);
            panels[index].SendMessage("OnOpen", SendMessageOptions.DontRequireReceiver);
            if (titlePanelStr == "")
            {
                titlePanel.text = titleOfPanel[index];
            }
        }

        currentPanel = index;
    }



    public void GoToShopButton_OnClick()
    {
        Master.PlaySoundButtonClick();
        OpenPanel(4, "Shop");
    }

    public void FillFullEnergyButton_OnClick()
    {
        Master.PlaySoundButtonClick();
        ShowDialog(UIController.Dialog.ListDialogs.FillEnergyDialog);
    }

    public void GoToFirstMenu()
    {
        Transition(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("FirstMenu");
        });
    }

}
