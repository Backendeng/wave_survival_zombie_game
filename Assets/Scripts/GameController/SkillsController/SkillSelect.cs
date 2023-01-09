using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkillSelect : MonoBehaviour
{

    // Use this for initialization
    public SkillDataController.SkillData skillData;
    private SpriteRenderer icon;
    private UITexture countdown;
    private bool isCountingdown;
    private bool isSelected;
    private bool isCanSelect;
    public bool isLock;
    private GameObject pf_skill;
    private GameObject skill;

    void Start()
    {
        isCanSelect = true;
        pf_skill = Master.GetSkillPrefabByID(skillData.SkillID);
        

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetInfo()
    {
        icon = Master.GetChildByName(gameObject, "Icon").GetComponent<SpriteRenderer>();
        countdown = Master.GetChildByName(gameObject, "Countdown").GetComponent<UITexture>();
        if (!isLock)
        {
            icon.sprite = Resources.Load<Sprite>("Textures/Skills/Skill_" + skillData.SkillID + "/Skill_" + skillData.SkillID + "_Icon");
        }
        else
        {
            icon.sprite = Resources.Load<Sprite>("Textures/Skills/skill_locked");
            icon.color = new Color(1, 1, 1, 0.7f);
            Destroy(icon.gameObject.GetComponent<UIButton>());
        }
        countdown.gameObject.SetActive(false);
        AddEvent();
    }

    public void AddEvent()
    {
        if (isLock) return;

        Master.Touch.AddTouchEvent(TouchController.TouchType.Touching, () =>
        {
            if (isSelected)
            {
                skill.GetComponentInChildren<SkillController>().OnChoosingPosition();
            }
        });

        Master.Touch.AddTouchEvent(TouchController.TouchType.TouchUp, () =>
        {
            if (isSelected)
            {
                if (skill.GetComponentInChildren<SkillController>().Set())
                {
                    StartCountdown();
                }
                isSelected = false;
            }
        });
    }

    void StartCountdown()
    {
        isCountingdown = true;
        countdown.gameObject.SetActive(true);
        Master.Effect.Fill(countdown, skillData.TimeCountdown, 1, 0, () =>
          {
              isCountingdown = false;
              countdown.fillAmount = 1;
              countdown.gameObject.SetActive(false);
          });

    }

    public void OnTouchIn()
    {
        if (!isCanSelect || isCountingdown || isLock || !Master.isGameStart) return;

        isSelected = true;
        Master.Audio.PlaySound("snd_click");

        if (skillData.SkillID == "01")
        {
            Master.Lane.ShowUnitPositionsAvailable();
           // Master.Touch.GetMousePosition();
        }

        skill = NGUITools.AddChild(Master.Gameplay.skillsRoot, pf_skill);
        skill.transform.position = Master.Gameplay.camera.ScreenToWorldPoint(Input.mousePosition);
    }

}
