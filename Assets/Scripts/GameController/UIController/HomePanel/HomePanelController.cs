using UnityEngine;
using System.Collections;

public class HomePanelController : MonoBehaviour
{
    GameObject freeReward;
    GameObject freeRewardTitle;
    private GameObject levelButtons;
    GameObject levelSelectPanel;
    UITexture soundTexture;
    UITexture musicTexture;
    GameObject alertQuestCompleteIcon;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        InvokeRepeating("CheckFreeReward", 0, 2);
        // InvokeRepeating("CheckAlertQuestComplete", 0, 2);
    }

    void Update()
    {
        if (levelSelectPanel.transform.localPosition.x > 0)
        {
            levelSelectPanel.transform.localPosition = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(0, 0, 0);
        }
        if (levelSelectPanel.transform.localPosition.x < -2320)
        {
            levelSelectPanel.transform.localPosition = new Vector3(-2320, 0, 0);

            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-2323, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(2323, 0, 0);

        }
    }

    void AssignObject()
    {
        levelButtons = Master.GetChildByName(gameObject, "LevelButtons");
        levelSelectPanel = Master.GetChildByName(gameObject, "LevelSelect");
        freeReward = Master.GetChildByName(gameObject, "FreeReward");
        freeRewardTitle = Master.GetChildByName(gameObject, "FreeRewardTitle");
        soundTexture = Master.GetChildByName(gameObject, "Sound").GetComponent<UITexture>();
        musicTexture = Master.GetChildByName(gameObject, "Music").GetComponent<UITexture>();
        alertQuestCompleteIcon = Master.GetChildByName(gameObject, "AlertQuestCompleteIcon");
    }

    public void OnOpen()
    {
        SetSettings();
        SetLevelButton();
        CheckAlertQuestComplete();

        if (!Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay))
        {
            if (Master.Stats.Star >= 150)
            {
                Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.UpgradeStatsOfUnit);
            }

            if (!Master.Tutorial.isDoingTutorial)
            {
                if (Master.LevelData.lastLevel >= FreeRewardController.levelCanGetFreeReward)
                {
                    Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.GetFreeReward);
                }
            }

        }

    }

    public void SetLevelButton()
    {
        Vector3 posLastLevelButton = Vector3.zero;
        foreach (Transform level in levelButtons.transform)
        {
            if (level.childCount > 0)
            {
                Destroy(level.GetChild(0).gameObject);
            }
            int levelIndex = int.Parse(level.gameObject.name);
            GameObject pf_levelButton = Master.GetGameObjectInPrefabs("UI/LevelButton");
            GameObject levelButton = NGUITools.AddChild(level.gameObject, pf_levelButton);
            levelButton.name = "LevelSelect_" + levelIndex;
            levelButton.GetComponentInChildren<LevelButton>().SetAttribute(levelIndex);
            if (levelIndex == Master.LevelData.lastLevel + 1)
            {
                posLastLevelButton = level.transform.localPosition;
            }
        }
        Debug.Log(Master.LevelData.lastLevel + " | " + posLastLevelButton.x);
        //set screen center of last level
        if (Master.LevelData.lastLevel >= 12)
        {
            levelSelectPanel.GetComponent<SpringPanel>().enabled = true;
            levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-posLastLevelButton.x, 0, 0);
        }
    }


    void CheckFreeReward()
    {
        if (Master.LevelData.lastLevel >= FreeRewardController.levelCanGetFreeReward)
        {
            freeReward.SetActive(true);
        }
        else
        {
            freeReward.SetActive(false);
        }

        if (FreeRewardController.IsCanGetFreeReward())
        {
            freeRewardTitle.GetComponent<MoveObject>().enabled = true;
        }
        else
        {
            freeRewardTitle.GetComponent<MoveObject>().enabled = false;
        }
    }

    public void FreeRewardButton_OnClick()
    {

        Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.GetFreeReward);

        Master.PlaySoundButtonClick();
        Master.UI.ShowDialog("FreeRewardDialog", 0.5f);
    }


    public void SetSettings()
    {
        if (Master.Audio.isSoundOn)
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(true);
        }

        if (Master.Audio.isBackgroundMusicOn)
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(true);
        }

    }

    public void ToggleAudioSettingButton_OnClick(GameObject go)
    {
        Master.PlaySoundButtonClick();
        string goName = go.name;
        if (goName == "Sound")
        {
            Master.Audio.ToggleSound();
        }
        else if (goName == "Music")
        {
            Master.Audio.ToggleBackgroundMusic();
        }

        SetSettings();
    }

    void CheckAlertQuestComplete()
    {
        if (Master.QuestData.isHaveQuestComplete())
        {
            alertQuestCompleteIcon.SetActive(true);
        }
        else
        {
            alertQuestCompleteIcon.SetActive(false);
        }
    }

}
