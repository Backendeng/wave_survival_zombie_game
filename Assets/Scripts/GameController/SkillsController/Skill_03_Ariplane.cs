using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_03_Ariplane : SkillController
{

    // Use this for initialization
    public Vector3 setSkillPosition;
    public float damage;
    public float speedMove = 5;
    private GameObject pf_rocket;
    private GameObject skill_03;
    private bool isStartDropRocket;
    public Transform[] dropRocketPoints = new Transform[2];
    private bool isCanDropRocket;
    private float timePerDropRocket = 0.08f;

    public override void OnStart()
    {
        pf_rocket = Master.GetSkillPrefabByID("03_Rocket");
        Master.Audio.PlaySound("snd_skill03_airplane", 0.8f);
    }

    public void SetAttribute(Vector3 setSkillPosition, Transform[] dropRocketPoints, float damage, GameObject skill_03)
    {
        this.dropRocketPoints = dropRocketPoints;
        this.damage = damage;
        this.setSkillPosition = setSkillPosition;
        this.skill_03 = skill_03;
        transform.parent.localPosition = new Vector3(Master.Gameplay.outOfScreenPos[0].transform.localPosition.x, setSkillPosition.y, 0);
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        transform.parent.transform.Translate(new Vector3(speedMove * Time.deltaTime, 0, 0));
        if (transform.parent.position.x >= (dropRocketPoints[0].position.x - 0.4f) && transform.parent.position.x <= (dropRocketPoints[1].position.x - 0.4f))
        {
            isCanDropRocket = true;
            if (!isStartDropRocket)
            {
                StartDropRocket();
            }
        }
        else
        {
            isCanDropRocket = false;
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    void StartDropRocket()
    {
        isStartDropRocket = true;
        InvokeRepeating("DropRocket", 0, timePerDropRocket);
    }

    void DropRocket()
    {
        if (isCanDropRocket)
        {
            GameObject rocket = NGUITools.AddChild(Master.Gameplay.skillsRoot, pf_rocket);
            Vector3 startPos = new Vector3(transform.parent.localPosition.x, Master.Gameplay.outOfScreenPos[1].localPosition.y, 0);
            Vector3 endPos = new Vector3(transform.parent.localPosition.x + 150, setSkillPosition.y - 20, 0);
            rocket.GetComponentInChildren<Skill_03_Rocket>().SetAttribute(startPos, endPos, damage);
        }
    }

    public override void CollisionController(GameObject obj)
    {
        if (obj == Master.Gameplay.outOfScreenPos[2].gameObject)
        {
            Destroy(skill_03.transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }



}
