using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour
{
    // Use this for initialization
    public string skillID;
    public SkillDataController.SkillData data;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        data = Master.SkillData.GetSkillDataByID(skillID);
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    void LateUpdate()
    {
        OnLateUpdate();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    public virtual bool Set()
    {
        return true;
    }

    public virtual void OnChoosingPosition()
    {

    }

    public virtual void CollisionController(GameObject obj)
    {

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        CollisionController(coll.gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        CollisionController(coll.gameObject);
    }

}
