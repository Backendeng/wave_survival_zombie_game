using UnityEngine;
using System.Collections;

public class Bullet07 : BulletController
{
    public float speed;
   
    // Update is called once per frame

    public override void OnUpdate()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }


    public override void CollisionController(GameObject obj)
    {   
        if (obj.tag == "Zombie")
        {
            obj.SendMessage("GetHit", damage);
            animator.Play("Bullet_07_Broken");
            Destroy(gameObject);
        }
    }


}
