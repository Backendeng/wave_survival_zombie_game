using UnityEngine;
using System.Collections;

public class Enemy03 : EnemyController
{
    GameObject canNotEffectSkill2;

    public override void OnAwake()
    {
        canNotEffectSkill2 = Master.GetChildByName(gameObject, "CanNotEffectSkill2");
        canNotEffectSkill2.SetActive(false);
    }

    public override void GetFreeze(float timeFreeze)
    {
        canNotEffectSkill2.SetActive(true);
        Master.WaitAndDo(2, () =>
        {
            canNotEffectSkill2.SetActive(false);
        }, this);
    }

}
