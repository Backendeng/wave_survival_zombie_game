using UnityEngine;
using System.Collections;

public class Unit08 : UnitController
{
    // Use this for initialization
    public override void Shoot()
    {
        if (action.Dead || !status.IsCanShoot || status.IsFreezing) return;

        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetTimeToShoot();
        if (!status.IsUpgraded)
        {
            CreateBullet(0.3f);
        }
        else
        {
            CreateBullet(0.3f);
            Master.WaitAndDo(0.25f, () =>
            {
                CreateBullet(0.3f);
            }, this);
        }
        
    }

}
