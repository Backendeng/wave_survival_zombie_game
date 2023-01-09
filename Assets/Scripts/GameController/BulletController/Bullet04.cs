using UnityEngine;
using System.Collections;

public class Bullet04 : BulletController {

    // Use this for initialization
    public override void OnStart()
    {
        SetDamageToEachBullet();
    }

    void SetDamageToEachBullet()
    {
        foreach (Transform bullet in gameObject.transform)
        {
            bullet.gameObject.GetComponent<BulletController>().damage = damage;
        }
    }
}
