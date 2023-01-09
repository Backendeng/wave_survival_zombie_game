using UnityEngine;
using System.Collections;

public class Unit05 : UnitController
{
    // Use this for initialization
    public override void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        CreateBullet();
        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetStatus("Shoot");
        SetTimeToShoot();
        animator.speed = 0.5f;

        PlayAnimation("Shoot", () =>
        {
            Idle();
        });

    }

}
