using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UnitSelect : MonoBehaviour
{

    // Use this for initialization
    public UnitDataController.UnitData unitData;
    private bool isSelected;
    public bool isLock;
    private GameObject unit;
    private GameObject pf_unit;
    private bool isCanSelect;
    private bool isCountingdown;

    private SpriteRenderer[] listSprite;
    private SpriteRenderer iconSprite;
    private GameObject priceRoot;
    private UILabel priceLabel;
    private UITexture countdown;
    GameObject blockByZombie;

    bool isDoingTutorialBuildUnit = false;
    void Start()
    {
        isDoingTutorialBuildUnit = Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.BuildUnitInGameplay;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
        Master.Touch.RemoveTouchEvent(TouchController.TouchType.Touching, OnTouching);
        Master.Touch.RemoveTouchEvent(TouchController.TouchType.TouchUp, OnTouchUp);
    }

    void AddEvent()
    {
        if (isLock) return;

        Master.Touch.AddTouchEvent(TouchController.TouchType.Touching, OnTouching);
        Master.Touch.AddTouchEvent(TouchController.TouchType.TouchUp, OnTouchUp);
    }

    public void SetInfo()
    {
        listSprite = GetComponentsInChildren<SpriteRenderer>();
        iconSprite = Master.GetChildByName(gameObject, "Icon").gameObject.GetComponent<SpriteRenderer>();
        priceLabel = Master.GetChildByName(gameObject, "PriceLabel").GetComponent<UILabel>();
        priceRoot = Master.GetChildByName(gameObject, "Price");
        countdown = Master.GetChildByName(gameObject, "BGCountdown").GetComponent<UITexture>();
        blockByZombie = Master.GetChildByName(gameObject, "BlockByEnemy");
        blockByZombie.SetActive(false);
        pf_unit = Master.GetUnitPrefabByID(unitData.UnitID);

        if (!isLock)
        {
            iconSprite.sprite = Resources.Load<Sprite>("Textures/Characters/Units/Unit_" + unitData.UnitID + "/Icon");

        }
        else
        {
            iconSprite.sprite = Resources.Load<Sprite>("Textures/Characters/Units/UnitIconLock");
            iconSprite.color = new Color(1, 1, 1, 0.7f);
            Destroy(iconSprite.gameObject.GetComponent<UIButton>());
        }

        if (!isLock)
        {
            priceLabel.text = unitData.Price.ToString();
            priceRoot.SetActive(true);
        }
        else
        {
            priceRoot.SetActive(false);
        }
        countdown.gameObject.SetActive(false);
        InvokeRepeating("CheckCanSelect", 0.2f, 0.2f);
        AddEvent();
    }

    void OnTouching()
    {
        if (isSelected)
        {
            unit.transform.position = Master.Touch.mousePositionGameplay;
            Master.Lane.ChangeColorPosition();
        }
    }

    void OnTouchUp()
    {
        if (isSelected)
        {
            bool isBuildInPosition = false;
            foreach (GameObject go in Master.Touch.listGameObjectsAtMousePosition)
            {
                if (go.tag == "UnitPosition")
                {
                    isBuildInPosition = true;
                    int laneIndex = int.Parse(go.transform.parent.name.Split('_')[1]);
                    Master.Lane.SetUnitAtPosition(go, unit);
                    Master.Lane.SetCharacterAtLane(unit, laneIndex);
                    unit.GetComponent<UnitController>().status.CurrentLane = laneIndex;
                    unit.GetComponent<UnitController>().SetActive(true);
                    unit.transform.DOMove(go.transform.position, 0.3f);
                    Master.Gameplay.gold -= unitData.Price;
                    Master.Audio.PlaySound("snd_unitSet" + Random.Range(1, 3));
                    StartCountdown();

                    if (Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay))
                    {
                        isDoingTutorialBuildUnit = false;
                        Master.isLevelComplete = false;
                        Master.isGameStart = true;
                        Master.Level.StartInitEnenmy();
                    }

                }
            }

            if (!isBuildInPosition)
            {
                Destroy(unit);
                if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.BuildUnitInGameplay)
                {
                    Master.Tutorial.GoToPreviousStep(2);
                }
            }

            isSelected = false;
            unit = null;
            Master.Lane.HideUnitPosition();
        }
    }

    void CheckCanSelect()
    {
        if (Master.Gameplay.isBlockUnitSelectByEnemy)
        {
            blockByZombie.SetActive(true);
        }
        else
        {
            blockByZombie.SetActive(false);
        }

        if (isLock || Master.Gameplay.isBlockUnitSelectByEnemy) { isCanSelect = false; return; }

        if (Master.Gameplay.gold >= unitData.Price)
        {
            isCanSelect = true;
            foreach (SpriteRenderer sprRenderer in listSprite)
            {
                if (sprRenderer.gameObject != countdown)
                {
                    sprRenderer.color = new Color(1, 1, 1f, 1);
                }
            }
            priceLabel.color = new Color(255 / 255f, 237 / 255f, 0, 1);
        }
        else
        {
            isCanSelect = false;
            foreach (SpriteRenderer sprRenderer in listSprite)
            {
                if (sprRenderer.gameObject != countdown)
                {
                    sprRenderer.color = new Color(159 / 255f, 159 / 255f, 159 / 255f, 1);
                }
            }
            priceLabel.color = new Color(1, 0, 0, 1);
        }

        if (isCountingdown)
        {
            isCanSelect = false;
        }
    }

    void StartCountdown()
    {
        isCountingdown = true;
        countdown.gameObject.SetActive(true);
        Master.Effect.Fill(countdown, unitData.TimeCountdownSelect, 1, 0, () =>
          {
              isCountingdown = false;
              countdown.fillAmount = 1;
              countdown.gameObject.SetActive(false);
          });
    }

    public void OnTouchIn()
    {
        if (isLock) return;

        if (isDoingTutorialBuildUnit)
        {
            Master.Tutorial.GoToNextStep(3);
            Master.WaitAndDo(0.7f, () =>
            {
                if (Master.Touch.isTouching)
                {
                    Master.Tutorial.GoToNextStep(4);
                }
            }, this);

            Master.PlaySoundButtonClick();
            CreateUnit();
            return;
        }

        if (!isCanSelect || !Master.isGameStart) return;

        Master.PlaySoundButtonClick();
        CreateUnit();
    }

    void CreateUnit()
    {
        Master.Lane.ShowUnitPositionsAvailable();
        unit = NGUITools.AddChild(Master.Gameplay.unitsRoot, pf_unit);
        unit.transform.position = Master.Gameplay.camera.ScreenToWorldPoint(Input.mousePosition);
        isSelected = true;
    }

}
