using UnityEngine;
using System.Collections;

public class Unit07 : UnitController
{
    // Use this for initialization
    public override void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        status.CurrentShoot++;
        status.CurrentAmmo--;
        animator.speed = 0.6f;
        SetStatus("Shoot");
        SetTimeToShoot();
        PlayAnimation("Shoot", () =>
        {
            Idle();
        });
        Master.WaitAndDo(0.1f, () =>
        {
            CreateBullet();
        }, this);


    }

}
