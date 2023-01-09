using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Skill_03_Rocket : SkillController
{

    // Use this for initialization
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed = 5;
    private float damage;

    public override void OnStart()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().enabled = false;
        Master.PlayAnimation(spriteRenderer, "Textures/Skills/Skill_03/SkillAnimation", 0, 3, 0.1f, true, null, this);
    }

    public void SetAttribute(Vector3 startPos, Vector3 endPos, float damage)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.damage = damage;
        transform.parent.localPosition = startPos;
    }
    bool isCanMove = true;
    public override void OnUpdate()
    {
        if (isCanMove)
        {
            transform.parent.localPosition = Vector3.MoveTowards(transform.parent.localPosition, endPos, speed);
            if (Vector3.Distance(transform.parent.localPosition, endPos) < 0.1f)
            {
                isCanMove = false;
                GetComponent<BoxCollider2D>().enabled = true;
                Master.Audio.PlaySound("snd_explosion");
                Master.Effect.ShakeCamera(5);
                Master.PlayAnimation(spriteRenderer, "Textures/Skills/Skill_03/SkillAnimation", 5, 16, 0.01f, false, () =>
                {
                    Destroy(transform.parent.gameObject);
                }, this);
            }
        }
    }

    public override void CollisionController(GameObject obj)
    {
        if (obj.tag == "Enemy")
        {
            obj.GetComponent<EnemyController>().GetHit(damage);
           // obj.SendMessage("GetHit", new object[] { (object)damage, (object) false}, SendMessageOptions.DontRequireReceiver);
        }
    }
}
