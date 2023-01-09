using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_03 : SkillController
{
    public Transform[] dropRocketPoints = new Transform[2];

    private GameObject pf_airplane;

    public override void OnStart()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        gameObject.transform.localScale = new Vector3(Master.SkillData.skill_03_data.Range, gameObject.transform.localScale.y, 0);

        pf_airplane = Master.GetSkillPrefabByID("03_Airplane");

        dropRocketPoints[0] = Master.GetChildByName(gameObject,"FirstDropRocketPoint").transform;
        dropRocketPoints[1] = Master.GetChildByName(gameObject, "EndDropRocketPoint").transform;
    }

    public override bool Set()
    {
        Master.QuestData.IncreaseProgressValue("05");

        GameObject airplane = NGUITools.AddChild(Master.Gameplay.skillsRoot, pf_airplane);
        airplane.GetComponentInChildren<Skill_03_Ariplane>().SetAttribute(transform.parent.localPosition, dropRocketPoints, Master.SkillData.skill_03_data.Damage, gameObject);
        spriteRenderer.sprite = null;
        return true;
    }

    public override void OnChoosingPosition()
    {
        transform.parent.position = Master.Touch.mousePositionGameplay;
    }
}
