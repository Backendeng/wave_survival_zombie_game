using UnityEngine;
using System.Collections;

public class Bullet03Upgrade : BulletController
{

    float decreasePercentDamageAfterEnemy = 70;
    public override void OnStart()
    {
        Master.WaitAndDo(0.15f, () =>
        {
            SendHitToEnemy();
        }, this);
    }

    public override void CollisionController(GameObject obj)
    {
        //do nothing
    }

    public void SendHitToEnemy()
    {
        bool isFirstHit = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, Vector3.right, 3000);
        float currentDamage = damage;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    if (!isFirstHit)
                    {
                        currentDamage = currentDamage * ((100-decreasePercentDamageAfterEnemy)/100);
                    }
                    hit.collider.gameObject.GetComponent<EnemyController>().GetHit(currentDamage, isCrit);
                    isFirstHit = false;
                }
            }
        }
    }

}
