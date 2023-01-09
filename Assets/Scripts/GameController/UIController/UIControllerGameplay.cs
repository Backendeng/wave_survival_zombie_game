using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;

public class UIControllerGameplay : UIController
{

    [HideInInspector]
    public UILabel totalGoldLabel;
    [HideInInspector]
    public UISlider zombieProgressSlider;
    [HideInInspector]
    public GameObject zombieIconProgress;
    [HideInInspector]
    public UILabel unitsDeadLabel;
    [HideInInspector]
    public UILabel levelIndexLabel;
    [HideInInspector]
    public GameObject newEnemy;
    private string newEnemyID;

    public enum Positions
    {
        Left, Top, Right, Bottom, Center
    }
    public Dictionary<Positions, Transform> positionsOnScreen = new Dictionary<Positions, Transform>();

    public override void OnAwake()
    {
        if (Master.UI == null)
        {
            Master.UI = this;
        }

        Master.UIGameplay = this;
        Master.UIMenu = null;


        totalGoldLabel = Master.GetChildByName(uiRoot, "TotalGoldValue").GetComponent<UILabel>();
        zombieProgressSlider = Master.GetChildByName(uiRoot, "ZombieProgressSlider").GetComponent<UISlider>();
        zombieIconProgress = Master.GetChildByName(uiRoot, "ZombieIcon");
        unitsDeadLabel = Master.GetChildByName(uiRoot, "UnitsDeadValue").GetComponent<UILabel>();
        levelIndexLabel = Master.GetChildByName(uiRoot, "LevelIndexLabel").GetComponent<UILabel>();
        newEnemy = Master.GetChildByName(uiRoot, "NewEnemy");
        newEnemy.SetActive(false);
        unitsDeadLabel.GetComponent<TweenColor>().enabled = false;
        unitsDeadLabel.GetComponent<TweenScale>().enabled = false;

        positionsOnScreen.Add(Positions.Left, Master.GetChildByName(uiRoot, "Pos_Left").transform);
        positionsOnScreen.Add(Positions.Top, Master.GetChildByName(uiRoot, "Pos_Top").transform);
        positionsOnScreen.Add(Positions.Right, Master.GetChildByName(uiRoot, "Pos_Right").transform);
        positionsOnScreen.Add(Positions.Bottom, Master.GetChildByName(uiRoot, "Pos_Bottom").transform);
        positionsOnScreen.Add(Positions.Center, Master.GetChildByName(uiRoot, "Pos_Center").transform);
    }

    public override void OnUpdate()
    {
        SetUI();
    }

    public override void OnStart()
    {
        Master.Audio.PlayBackgroundMusic("bg_gameplay_" + Random.Range(1, 3));
    }

    void SetUI()
    {
        totalGoldLabel.text = Master.Gameplay.gold + "";
        unitsDeadLabel.text = Master.Gameplay.unitsDead + "/" + Master.Level.currentLevelData.NumberOfUnitsAllowedDead;
        levelIndexLabel.text = "Level " + Master.LevelData.currentLevel;
        SetZombieProgress();
    }

    void SetZombieProgress()
    {
        float value = (float)Master.Level.totalSequenceIndex / Master.Level.totalSequences;
        if (zombieProgressSlider.value < value)
        {
            zombieProgressSlider.value += Time.deltaTime / 2;
        }
        float xChangeForZombieIcon = 165 - (zombieProgressSlider.value * 330);

        zombieIconProgress.transform.localPosition = new Vector3(xChangeForZombieIcon, zombieIconProgress.transform.localPosition.y, zombieIconProgress.transform.localPosition.z);
    }

    public void PauseButton_OnClick()
    {
        //SceneManager.LoadScene("scene");
        Master.PlaySoundButtonClick();
        Time.timeScale = 0;
        Master.UIGameplay.ShowDialog("PauseGame", 0.4f);
    }

    public void SetNewEnemy(string enemyID)
    {
        newEnemyID = enemyID;
        newEnemy.SetActive(true);
        Master.WaitAndDo(10, () =>
        {
            newEnemy.SetActive(false);
        }, this);

        Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.ViewZombieInfo, Vector3.zero, true, 10);

    }

    public void NewEnemyButton_OnClick()
    {
        Master.PlaySoundButtonClick();
        Master.UI.ShowDialog(UIController.Dialog.ListDialogs.EnemyInfoDialog, 0.4f, new string[] { newEnemyID, "Gameplay" });
        newEnemy.SetActive(false);
        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.ViewZombieInfo);
    }


    public void ShowLevelTitle(System.Action onComplete = null)
    {
        Master.Audio.PlaySound("snd_showLevelTitle", 0.7f);
        GameObject levelTitle = NGUITools.AddChild(uiRoot, Master.GetGameObjectInPrefabs("UI/LevelTitle"));
        levelTitle.GetComponent<UILabel>().text = "Level " + Master.LevelData.currentLevel;
        levelTitle.transform.position = positionsOnScreen[Positions.Right].position;
        float xChange = 0.2f;
        levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x - xChange, 0.27f).OnComplete(() =>
        {
            Master.Audio.PlaySound("snd_ready");

            levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x, 0.15f).OnComplete(() =>
            {
                Master.WaitAndDo(0.8f, () =>
                {
                    Master.Audio.PlaySound("snd_go");
                    Master.WaitAndDo(0.1f, () =>
                    {
                        levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x + xChange, 0.15f).OnComplete(() =>
                        {
                            Master.Audio.PlaySound("snd_showLevelTitle", 0.7f);

                            levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Left].position.x - 0.5f, 0.3f).OnComplete(() =>
                            {
                                if (onComplete != null)
                                {
                                    onComplete();
                                }
                                Destroy(levelTitle);
                            });
                        });
                    }, this);

                }, this);
            });
        });
    }

    public void HightlightUnitDeadLabel()
    {
        unitsDeadLabel.GetComponent<TweenColor>().enabled = true;
        unitsDeadLabel.GetComponent<TweenScale>().enabled = true;

        Master.WaitAndDo(1f, () =>
        {
            unitsDeadLabel.color = Color.white;
            unitsDeadLabel.transform.localScale = new Vector3(1, 1, 1);
            unitsDeadLabel.GetComponent<TweenColor>().enabled = false;
            unitsDeadLabel.GetComponent<TweenScale>().enabled = false;

        }, this);

    }


}
