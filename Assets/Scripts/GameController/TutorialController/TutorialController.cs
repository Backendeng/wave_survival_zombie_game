using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialController : MonoBehaviour
{

    public GameObject pf_tutorialRoot;
    public GameObject tutorialRoot;

    public class TutorialsIndex
    {
        public static int BuildUnitInGameplay = 0;
        public static int ReloadUnit = 1;
        public static int UnitAllowedDead = 2;
        public static int UpgradeUnitInGameplay = 3;
        public static int UpgradeStatsOfUnit = 4;
        public static int GetFreeReward = 5;
        public static int ViewZombieInfo = 6;
        public static int ReturnToMenu = 7;

    }

    public List<GameObject> listTutorialsGO = new List<GameObject>();
    public GameObject currentTutorialGO;
    public int currentTutorialIndex = -1;

    public List<GameObject> steps = new List<GameObject>();
    public GameObject currentStepGO;
    public int currentStepIndex = 0;

    const int totalTutorial = 7;
    public bool isDoingTutorial = false;

    void Awake()
    {
        if (Master.Tutorial == null)
        {
            Master.Tutorial = this;
        }
        AssignObjects();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void AssignObjects()
    {
        pf_tutorialRoot = Master.GetGameObjectInPrefabs("Tutorials/Tutorial Root");
    }

    public void SetListTutorials()
    {
        listTutorialsGO.Clear();
        foreach (Transform item in tutorialRoot.transform.Find("Tutorials").transform)
        {
            listTutorialsGO.Add(item.gameObject);
        }
    }

    public bool CheckAndStartTutorial(int tutorialIndex)
    {
        if (isDoingTutorial) return false;

        if (!IsTutorialDone(tutorialIndex))
        {
            StartTutorial(tutorialIndex);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckAndStartTutorial(int tutorialIndex, Vector3 arrowPosition, bool isAutoFinish = false, float timeAutoFinish = -1)
    {
        if (isDoingTutorial) return false;

        if (!IsTutorialDone(tutorialIndex))
        {
            StartTutorial(tutorialIndex);
            if (arrowPosition != Vector3.zero)
            {
                GameObject arrow = Master.GetChildByName(Master.Tutorial.currentStepGO, "Arrows");
                arrow.transform.position = arrowPosition;
            }
            if (isAutoFinish)
            {
                Master.WaitAndDo(timeAutoFinish, () =>
                {
                    CheckAndFinishTutorial(tutorialIndex);
                }, this);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsTutorialDone(int tutorialIndex)
    {
        return PlayerPrefs.GetInt("isTutorialDone_" + tutorialIndex, 0) == 0 ? false : true;
    }

    public void StartTutorial(int tutorialIndex)
    {
        isDoingTutorial = true;
        currentTutorialIndex = tutorialIndex;

        if (tutorialRoot == null)
        {
            tutorialRoot = Instantiate(pf_tutorialRoot, Vector3.zero, Quaternion.identity) as GameObject;
            tutorialRoot.name = "Tutorial Root";
        }
        SetListTutorials();
        foreach (GameObject item in listTutorialsGO)
        {
            item.SetActive(false);
        }

        currentTutorialGO = Master.GetChildByName(tutorialRoot, "Tutorial_" + tutorialIndex);
        currentTutorialGO.SetActive(true);
        steps.Clear();
        foreach (Transform step in currentTutorialGO.transform)
        {
            steps.Add(step.gameObject);
        }
        SetActiveStep(0);
    }

    public bool CheckAndContinueNextStepTutorial(int tutorialIndex, int stepIndex = -1)
    {
        if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == tutorialIndex)
        {
            GoToNextStep(stepIndex);
            return true;
        }
        return false;
    }

    public void GoToNextStep(int stepIndex = -1)
    {
        if (stepIndex == -1)
        {
            currentStepIndex++;
        }
        else
        {
            currentStepIndex = stepIndex;
        }
        SetActiveStep(currentStepIndex);
    }

    public bool CheckAndFinishTutorial(int tutorialIndex = -1)
    {
        if (tutorialIndex == -1)
        {
            tutorialIndex = currentTutorialIndex;
        }

        if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == tutorialIndex)
        {
            PlayerPrefs.SetInt("isTutorialDone_" + tutorialIndex, 1);
            PlayerPrefs.Save();
            Destroy(tutorialRoot);
            tutorialRoot = null;
            isDoingTutorial = false;
            currentStepGO = null;
            currentStepIndex = 0;
            currentTutorialGO = null;
            currentTutorialIndex = -1;
            return true;
        }
        else
        {
            return false;
        }


    }

    public void SetVisibleTutorial(bool isVisble, int tutorialIndex = -1)
    {
        if (tutorialIndex == -1)
        {
            if (currentTutorialGO != null)
            {
                currentTutorialGO.SetActive(isVisble);
            }
            else
            {
                listTutorialsGO[tutorialIndex].SetActive(isVisble);
            }

        }
        else
        {
            listTutorialsGO[tutorialIndex].SetActive(isVisble);
        }
    }



    public void GoToPreviousStep(int stepIndex = -1)
    {
        if (stepIndex == -1)
        {
            currentStepIndex--;
        }
        else
        {
            currentStepIndex = stepIndex;
        }
        SetActiveStep(currentStepIndex);
    }


    public void SetActiveStep(int stepIndex)
    {
        currentStepGO = steps[stepIndex];
        foreach (GameObject item in steps)
        {
            item.SetActive(false);
        }
        currentStepGO.SetActive(true);

        if (currentTutorialIndex == TutorialsIndex.BuildUnitInGameplay && currentStepIndex == 3)
        {
            MoveArrowTutBuildUnitStep3();
        }
    }



    void MoveArrowTutBuildUnitStep3()
    {
        GameObject arrow = Master.GetChildByName(currentStepGO, "Arrows");
        Transform moveToPosition = Master.GetChildByName(currentStepGO, "ToUnitBuildPos").transform;
        Vector3 firstPosArrow = arrow.transform.position;
        arrow.transform.DOMove(moveToPosition.position, 0.8f).OnComplete(() =>
        {
            arrow.transform.position = firstPosArrow;
        });
    }

}
