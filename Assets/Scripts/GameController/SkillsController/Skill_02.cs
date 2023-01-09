using UnityEngine;
using System.Collections;

public class Skill_02 : SkillController
{

    public override void OnStart()
    {
        int getUpgarde = Master.SkillData.GetUpgradeSkillByID(skillID);
        gameObject.transform.localScale = new Vector3(Master.SkillData.skill_02_data.Raridus, Master.SkillData.skill_02_data.Raridus, 0);
    }

    public override bool Set()
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Gameplay");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ((Master.SkillData.skill_02_data.Raridus / 100) - 0.08f), layerMask);
        Master.QuestData.IncreaseProgressValue("04");

        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject.tag == "Enemy")
            {
                col.gameObject.SendMessage("GetFreeze", Master.SkillData.skill_02_data.FreezeTime);
            }
        }
        Destroy(transform.parent.gameObject);
        return true;
    }

    public override void OnChoosingPosition()
    {
        gameObject.transform.parent.transform.position = Master.Touch.mousePositionGameplay;
    }

    public override void CollisionController(GameObject obj)
    {
    }
}
