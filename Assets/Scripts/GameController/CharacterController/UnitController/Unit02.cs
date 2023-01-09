using UnityEngine;
using System.Collections;

public class Unit02 : UnitController
{

    // Use this for initialization

    public override void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        status.CurrentShoot++;
        //  unitStatus.CurrentAmmo--;
        SetStatus("Shoot");
        animator.speed = 0.5f;
        SetTimeToShoot();

        PlayAnimation("Shoot", () =>
        {
            Idle();
        });
        CreateBullet(0.5f);


    }

}
