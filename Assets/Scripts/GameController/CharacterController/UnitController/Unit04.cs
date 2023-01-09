using UnityEngine;
using System.Collections;

public class Unit04 : UnitController
{
    // Use this for initialization
    public override void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetStatus("Shoot");
        animator.speed = 1f;
        SetTimeToShoot();
        PlayAnimation("Shoot", () =>
        {
            Idle();
        });
        CreateBullet(4);
    }

}
