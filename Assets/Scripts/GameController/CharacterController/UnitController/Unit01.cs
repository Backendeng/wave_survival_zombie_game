using UnityEngine;
using System.Collections;

public class Unit01 : UnitController
{

    // Use this for initialization

    public override void OnStart()
    {
        data.Damage = data.Damage / 2;
    }

    public override void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetTimeToShoot();
        SetStatus("Shoot");
        if (!status.IsUpgraded)
        {
            animator.speed = 0.28f;
        }
        else
        {
            animator.speed = 0.22f;
        }

        PlayAnimation("Shoot", () =>
        {
            Idle();
        });

        Master.WaitAndDo(0.15f, () =>
        {
            CreateBullet(3);
            Master.WaitAndDo(0.3f, () =>
            {
                CreateBullet(3);
            }, this);
        }, this);
    }


}
