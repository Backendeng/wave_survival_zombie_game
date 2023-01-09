using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{

    // Use this for initialization
    [Header("For Values")]
    public string enemyID = "";
    public string weaponName = "Weapon";

    public EnemyDataController.EnemyData data;

    [System.Serializable]
    public class Status
    {
        public float CurrentHealth;
        public int CurrentLane;
        public bool IsCanAttack;
        public bool IsAttacking;
        public bool IsFreezing;
        public bool IsBurning;
        public bool IsElectricShock;
        public bool IsEscaped;
    }
    public Status status;

    [System.Serializable]
    public class Action
    {
        public bool Idle;
        public bool Walk;
        public bool Attack;
        public bool Dead;
    }
    public Action action;

    private const float maxTimeToAttack = 7;
    private const float minTimeToAttack = 0.2f;

    //Components
    [HideInInspector]
    public GameObject general;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public float timeToAttack;

    [HideInInspector]
    public bool stopAllAction;
    [HideInInspector]
    public GameObject weapon;
    [HideInInspector]
    public UISlider healthBar;
    [HideInInspector]
    public GameObject critical;
    [HideInInspector]
    public UI2DSprite freezeEffect;

    //for virtual void awake, start, update, lateupdate
    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }

    private float timeUpdate = 0.2f;

    [HideInInspector]
    public bool isAutoSetSprite = true;
    private bool isWaitForAttack;
    private bool isFirstAttack;

    void Awake()
    {
        AssignObject();
        OnAwake();
    }

    void Start()
    {
        data = Master.EnemyData.GetEnemyDataWithUpgradeByID(enemyID);

        Master.WaitAndDo(2.5f, () =>
        {
            if (Master.EnemyData.CheckAndUnlockEnemy(enemyID))
            {
                Master.UIGameplay.SetNewEnemy(enemyID);
            }
        }, this);

        status.CurrentHealth = data.Health;
        Walk();
        ActionController();
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (action.Walk && !status.IsFreezing && !status.IsAttacking)
        {
            transform.Translate(new Vector3(-data.MoveSpeed * Time.deltaTime * 0.006f, 0, 0));
        }

        if (status.IsFreezing)
        {
            animator.speed = 0;
        }
        //else
        //{
        //    animator.speed = 1;
        //}

        if (action.Dead)
        {
            animator.speed = 1;
        }


        OnUpdate();
    }

    void LateUpdate()
    {
        SpriteController();
        OnLateUpdate();
    }

    void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    void AssignObject()
    {
        GameObject pf_general = Master.GetGameObjectInPrefabs("Characters/Enemies/General");
        general = NGUITools.AddChild(gameObject, pf_general);

        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = Master.GetChildByName(gameObject, "Sprite").GetComponent<SpriteRenderer>();
        weapon = Master.GetChildByName(gameObject, weaponName).gameObject;


        freezeEffect = Master.GetChildByName(gameObject, "FreezeEffect").GetComponent<UI2DSprite>();
        freezeEffect.gameObject.SetActive(false);

        weapon.SetActive(false);
        healthBar = Master.GetChildByName(gameObject, "HealthBar").GetComponent<UISlider>();
        critical = Master.GetGameObjectInPrefabs("Critical");
        healthBar.value = 1;
        healthBar.gameObject.SetActive(false);


    }

    #region Action Controller
    void ActionController()
    {
        SetTimeToAttack();
        InvokeRepeating("WalkController", timeUpdate, timeUpdate);
        InvokeRepeating("AttackController", timeUpdate, timeUpdate);
        InvokeRepeating("HealthController", timeUpdate, timeUpdate / 10);
    }

    public virtual void WalkController()
    {
        if (stopAllAction || status.IsFreezing || status.IsAttacking) return;

        action.Walk = !status.IsCanAttack;
        if (action.Walk)
        {
            Walk();
        }
        if (!action.Walk && !action.Attack && !action.Dead)
        {
            Idle();
        }
    }



    public virtual void AttackController()
    {
        if (stopAllAction || status.IsFreezing) return;

        CheckIsCanAttack();

        if (status.IsCanAttack)
        {
            if (!isFirstAttack)
            {
                Attack();
                isFirstAttack = true;
            }
            else
            {
                timeToAttack -= Time.deltaTime * 15;
                if (timeToAttack <= 0)
                {
                    Attack();
                }
                //if (!isWaitForAttack)
                //{
                //    isWaitForAttack = true;
                //    Master.WaitAndDo(timeToAttack, () =>
                //    {
                //        isWaitForAttack = false;
                //        Attack();
                //    }, this);
                //}
            }
        }
    }

    void CheckIsCanAttack()
    {
        //check unit in lane
        if (!Master.Lane.isExistCharacterByTagInLane(status.CurrentLane, "Unit"))
        {
            status.IsCanAttack = false;
            return;
        }
        //check range
        bool isCanAttack = false;
        foreach (GameObject obj in Master.Lane.GetCharactersInLaneByTag(status.CurrentLane, "Unit"))
        {
            if (transform.position.x < obj.transform.position.x)
            {
                continue;
            }

            float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
            if (data.Range > distance * 100)
            {
                isCanAttack = true;
            }
        }
        status.IsCanAttack = isCanAttack;
    }

    public virtual void SetTimeToAttack()
    {
        timeToAttack = maxTimeToAttack - (data.AttackSpeed * 0.1f);
        if (timeToAttack <= 0) timeToAttack = minTimeToAttack;

        //Debug.Log("Enemy " + enemyID + " AS: " + timeToAttack);
    }

    public virtual void HealthController()
    {
        if (stopAllAction) return;

        float healthValue = status.CurrentHealth / data.Health;

        if (healthBar.value > healthValue)
        {
            healthBar.value -= Time.deltaTime * 2;
        }
        else if (healthBar.value <= healthValue)
        {
            healthBar.value = healthValue;
        }

        if (healthValue == 1)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
        }

        if (status.CurrentHealth <= 0)
        {
            Dead();
        }
    }

    public virtual void GetHit(float damage, bool isCrit = false)
    {
        if (isCrit)
        {
            GameObject obj_crit = NGUITools.AddChild(gameObject, critical);
            obj_crit.transform.localPosition = gameObject.transform.position;
            damage = damage * 2;
        }

        status.CurrentHealth -= damage;

    }

    public virtual void GetFreeze(float timeFreeze)
    {
        Master.Audio.PlaySound("snd_freeze", 0.7f);
        freezeEffect.gameObject.SetActive(true);
        status.IsFreezing = true;
        Master.Effect.Fill(freezeEffect, 0.6f, 0, 1);
        Master.WaitAndDo(timeFreeze, () =>
        {
            status.IsFreezing = false;
            animator.speed = 1;
            freezeEffect.gameObject.SetActive(false);
        }, this);
    }

    void CollisionController(GameObject obj)
    {
        //if (obj.tag == "UnitBullet")
        //{
        //    float damageOfBullet = obj.GetComponent<BulletController>().damage;
        //    GetHit(damageOfBullet);
        //    if (obj.GetComponent<BulletController>().isCrit)
        //    {
        //        GameObject obj_crit = NGUITools.AddChild(gameObject, critical);
        //        obj_crit.transform.localPosition = gameObject.transform.position;
        //    }
        //}

        if (obj.name == "OutOfScreenLeft")
        {
            stopAllAction = true;
            Master.Gameplay.zombiesEscaped++;
            Master.Lane.RemoveCharacterAtLane(status.CurrentLane, gameObject);
            Master.Gameplay.CheckLevelComplete();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        CollisionController(coll.gameObject);
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        CollisionController(coll.gameObject);
    }


    #endregion

    #region Set Action
    public virtual void Idle()
    {
        if (stopAllAction) return;

        animator.speed = 1;
        SetActionStatus("Idle");
        PlayAnimation("Idle");
    }

    public virtual void Walk()
    {
        if (stopAllAction) return;

        animator.speed = 0.8f;
        SetActionStatus("Walk");
        PlayAnimation("Walk");
    }

    public virtual void Attack()
    {
        if (stopAllAction || !status.IsCanAttack) return;

        status.IsAttacking = true;
        animator.speed = 1;
        SetActionStatus("Attack");
        SetTimeToAttack();

        PlayAnimation("Attack", () =>
        {
            status.IsAttacking = false;
            Idle();
        });

        Master.WaitAndDo(0.3f, () =>
        {
            weapon.SetActive(true);
            Master.WaitAndDo(0.2f, () =>
            {
                weapon.SetActive(false);
            }, this);
        }, this);
    }

    public virtual void Dead()
    {
        if (stopAllAction) return;

        stopAllAction = true;
        general.SetActive(false);
        weapon.SetActive(false);
        SetActionStatus("Dead");
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        DropGoldController();

        Master.Audio.PlaySound("snd_zombieDead" + Random.Range(1, 4));
        Master.QuestData.IncreaseProgressValue("01");
        Master.Lane.RemoveCharacterAtLane(status.CurrentLane, gameObject);
        Master.Gameplay.CheckLevelComplete();
        PlayAnimation("Dead", () =>
        {
            DOTween.ToAlpha(() => spriteRenderer.color, x => spriteRenderer.color = x, 0, 1).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
    }

    public virtual void DropGoldController()
    {
        string goldDrop = data.CoinDrop;
        int finalGoldDrop = 0;
        if (goldDrop.Split(',').Length > 0)
        {
            finalGoldDrop = int.Parse(goldDrop.Split(',')[Random.Range(0, goldDrop.Split(',').Length)]);
        }
        else
        {
            finalGoldDrop = int.Parse(goldDrop);
        }

        for (int i = 0; i < finalGoldDrop; i++)
        {
            GameObject pf_goldCollect = Master.GetGameObjectInPrefabs("GoldDrop");
            GameObject go_goldCollect = NGUITools.AddChild(Master.Gameplay.gameplayRoot, pf_goldCollect);
            go_goldCollect.transform.position = transform.position;
        }
    }

    public void RemoveAllEffect()
    {
        freezeEffect.gameObject.SetActive(false);
    }

    #endregion

    public void SetActionStatus(string actionType)
    {
        action.Idle = false;
        action.Walk = false;
        action.Attack = false;
        action.Dead = false;

        if (actionType == "Idle")
        {
            action.Idle = true;
        }
        else if (actionType == "Walk")
        {
            action.Walk = true;
        }
        else if (actionType == "Attack")
        {
            action.Attack = true;
        }
        else if (actionType == "Dead")
        {
            action.Dead = true;
        }
    }

    private void SpriteController()
    {

        if (!isAutoSetSprite) return;

        string statusFolder = "";

        if (action.Idle)
        {
            statusFolder = "Idle";
        }
        else if (action.Walk)
        {
            statusFolder = "Walk";
        }
        else if (action.Attack)
        {
            statusFolder = "Attack";
        }
        else if (action.Dead)
        {
            statusFolder = "Dead";
        }

        string spriteName = spriteRenderer.sprite.name;
        string pathToSprite = "Textures/Characters/Enemies/Enemy_" + data.EnemyID + "/" + statusFolder + "/" + spriteName;
        spriteRenderer.sprite = Resources.Load<Sprite>(pathToSprite);
        spriteRenderer.sortingOrder = -1000 - (int)gameObject.transform.localPosition.y + 10;
    }

    public void OnTouching()
    {

    }

    public void PlayAnimation(string animationClipName, System.Action onAnimationComplete = null)
    {
        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName(animationClipName))
        {
            animator.Play(animationClipName, 0, 0);
            Master.WaitAndDo(0.01f, () =>
            {
                StartCoroutine(DetectAnimationOnComplete(onAnimationComplete));
            }, this);
        }
    }

    IEnumerator DetectAnimationOnComplete(System.Action onComplete = null)
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
        {

            yield return null;
        }

        if (onComplete != null && !animator.IsInTransition(0))
        {
            onComplete();
            onComplete = null;
        }
    }



}
