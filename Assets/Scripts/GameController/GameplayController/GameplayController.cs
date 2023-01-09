using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameplayController : MonoBehaviour
{
    [HideInInspector]
    public GameObject gameplayRoot;

    [HideInInspector]
    public Camera camera;

    // Use this for initialization
    [HideInInspector]
    public GameObject unitsRoot;
    [HideInInspector]
    public GameObject enemiesRoot;
    [HideInInspector]
    public GameObject skillsRoot;

    public Transform[] outOfScreenPos = new Transform[4]; //0: left, 1:top, 2:right, 3:bottom

    public ObscuredInt gold;
    public ObscuredInt zombiesEscaped = 0;
    public ObscuredInt unitsDead = 0;
    public ObscuredBool isBlockUnitSelectByEnemy;
    ObscuredFloat timePerGold = 1.2f;

    void Awake()
    {
        if (Master.Gameplay == null)
        {
            Master.Gameplay = this;
        }

        gameplayRoot = GameObject.Find("Gameplay Root");
        camera = Master.GetChildByName(gameplayRoot, "Camera").GetComponent<Camera>();
        unitsRoot = Master.GetChildByName(gameplayRoot, "Units");
        enemiesRoot = Master.GetChildByName(gameplayRoot, "Enemies");
        skillsRoot = Master.GetChildByName(gameplayRoot, "Skills");

        Master.UnitData.LoadUnitData();
        Master.SkillData.LoadSkillData();
        Master.UnitData.LoadUnitAvaiable();
        Master.SkillData.LoadSkillsAvaiable();

    }

    void Start()
    {
        Time.timeScale = 1;
        Master.Ad.Admob.HideBanner();
        SetUnitSelect();
        SetSkillSelect();
        gold = Master.Level.currentLevelData.InitialGold;
        Master.Stats.Energy--;
        Master.Stats.TimesPlay++;
        Master.isLevelComplete = false;
        Master.isGameStart = false;
        GetOutOfScreenPos();

        if (FindObjectOfType<Transition>() != null)
        {
            FindObjectOfType<Transition>().tempOnComplete = FirstLoadLevel;
        }
        else
        {
            FirstLoadLevel();
        }

        InvokeRepeating("GoldController", timePerGold, timePerGold);
        InvokeRepeating("CheckLevelComplete", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    public void FirstLoadLevel()
    {
        Master.UIGameplay.ShowLevelTitle(() =>
        {
            if (Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay, 2))
            {
                return;
            }

            Master.isLevelComplete = false;
            Master.isGameStart = true;
            Master.Level.StartInitEnenmy();
        });
    }

    public void SetUnitSelect()
    {
        GameObject unitSelectGrid = Master.GetChildByName(gameplayRoot, "UnitSelectGrid");

        //not show lock

        //foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitAvailable)
        //{
        //    GameObject pf_unitSelect = Master.GetGameObjectInPrefabs("Characters/Units/UnitSelect");
        //    GameObject obj_unitSelect = NGUITools.AddChild(unitSelectGrid, pf_unitSelect);
        //    obj_unitSelect.GetComponentInChildren<UnitSelect>().unitData = unitData;
        //    obj_unitSelect.GetComponentInChildren<UnitSelect>().SetInfo();
        //}

        //show lock

        foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitData)
        {
            GameObject pf_unitSelect = Master.GetGameObjectInPrefabs("Characters/Units/UnitSelect");
            GameObject obj_unitSelect = NGUITools.AddChild(unitSelectGrid, pf_unitSelect);
            obj_unitSelect.GetComponentInChildren<UnitSelect>().unitData = unitData;
            if (unitData.UnlockAtLevel <= Master.LevelData.lastLevel + 1)
            {
                obj_unitSelect.GetComponentInChildren<UnitSelect>().isLock = false;
            }
            else
            {
                obj_unitSelect.GetComponentInChildren<UnitSelect>().isLock = true;
            }
            obj_unitSelect.GetComponentInChildren<UnitSelect>().SetInfo();
        }

        unitSelectGrid.GetComponent<UIGrid>().Reposition();
    }

    public void SetSkillSelect()
    {
        GameObject skillSelectGrid = Master.GetChildByName(gameplayRoot, "SkillSelectGrid");

        //not show lock
        //foreach (SkillDataController.SkillData skillData in Master.SkillData.listSkillsAvaiable)
        //{
        //    GameObject pf_skillSelect = Master.GetGameObjectInPrefabs("Skills/SkillSelect");
        //    GameObject obj_skillSelect = NGUITools.AddChild(skillSelectGrid, pf_skillSelect);
        //    obj_skillSelect.GetComponentInChildren<SkillSelect>().skillData = skillData;
        //    obj_skillSelect.GetComponentInChildren<SkillSelect>().SetInfo();
        //}


        ////show lock
        foreach (SkillDataController.SkillData skillData in Master.SkillData.listSkillsData)
        {
            GameObject pf_skillSelect = Master.GetGameObjectInPrefabs("Skills/SkillSelect");
            GameObject obj_skillSelect = NGUITools.AddChild(skillSelectGrid, pf_skillSelect);
            obj_skillSelect.GetComponentInChildren<SkillSelect>().skillData = skillData;
            if (skillData.UnlockAtLevel <= Master.LevelData.lastLevel + 1)
            {
                obj_skillSelect.GetComponentInChildren<SkillSelect>().isLock = false;
            }
            else
            {
                obj_skillSelect.GetComponentInChildren<SkillSelect>().isLock = true;
            }
            obj_skillSelect.GetComponentInChildren<SkillSelect>().SetInfo();
        }

        skillSelectGrid.GetComponent<UIGrid>().Reposition();
    }

    void GetOutOfScreenPos()
    {
        outOfScreenPos[0] = Master.GetChildByName(gameplayRoot, "OutOfScreenLeft").transform;
        outOfScreenPos[1] = Master.GetChildByName(gameplayRoot, "OutOfScreenTop").transform;
        outOfScreenPos[2] = Master.GetChildByName(gameplayRoot, "OutOfScreenRight").transform;
        outOfScreenPos[3] = Master.GetChildByName(gameplayRoot, "OutOfScreenBottom").transform;
    }

    public void CheckLevelComplete()
    {

        if (!Master.isGameStart || Master.isLevelComplete) return;

        if ((Master.Level.totalSequenceIndex >= Master.Level.totalSequences && !Master.Lane.isExistCharacterByTagInAllLane("Enemy"))
            || (unitsDead >= Master.Level.currentLevelData.NumberOfUnitsAllowedDead) || (zombiesEscaped > 0))
        {

            Master.isLevelComplete = true;
            Master.isGameStart = false;
            //Master.Stats.TimesLevelComplete++;
            Master.WaitAndDo(2f, () =>
            {
                if (Master.Level.totalSequenceIndex >= Master.Level.totalSequences && !Master.Lane.isExistCharacterByTagInAllLane("Enemy"))
                {
                    if (Master.LevelData.currentLevel >= Master.LevelData.totalLevel)
                    {
                        EndingController.StartEnding();
                        return;
                    }
                }

                Master.UI.ShowDialog("LevelCompleteDialog", 0.6f);

            }, this);
        }
    }




    void GoldController()
    {
        if (Master.isGameStart)
        {
            gold += 1;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void GoToMenu()
    {
        Master.UI.Transition(() =>
        {
            //SceneManager.LoadScene("Menu");
            Time.timeScale = 1;
            Application.LoadLevel("Menu");
            //Time.timeScale = 1;
        });
    }

    public void GoToNextLevel()
    {
        Master.UI.Transition(() =>
        {
            Master.LevelData.currentLevel++;
            //SceneManager.LoadScene("Play");
            Time.timeScale = 1;
            Application.LoadLevel("Play");
            //Time.timeScale = 1;
        });
    }

    public void ReplayGame()
    {
        if (Master.Stats.Energy > 0)
        {
            Master.UI.Transition(() =>
            {
                // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Time.timeScale = 1;
                Application.LoadLevel(Application.loadedLevel);
                //Time.timeScale = 1;
            });
        }
        else
        {
            Master.UI.ShowDialog(UIController.Dialog.ListDialogs.FillEnergyDialog, 0.5f, new string[] { "ReplayScene" });
        }
    }

    public bool CheckEnergy()
    {
        if (Master.Stats.Energy > 0)
        {
            return true;
        }
        else
        {
            Master.UI.ShowDialog(UIController.Dialog.ListDialogs.FillEnergyDialog, 0.5f, new string[] { "ReplayScene" });
            return false;
        }
    }


}

