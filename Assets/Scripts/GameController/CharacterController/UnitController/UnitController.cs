using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UnitController : MonoBehaviour
{
    #region Declare
    [Header("For Values")]
    public string unitID;
    public UnitDataController.UnitData data;

    //Unit Status
    [System.Serializable]
    public class Status
    {
        public float CurrentHealth;
        public int CurrentShoot;
        public int CurrentAmmo;
        public int CurrentLane;
        public bool IsActive;
        public bool IsUpgraded;
        public bool IsCanUpgrade;
        public bool IsUpgrading;
        public bool IsFreezing;
        public bool IsCanShoot;
        public bool IsOutOfAmmo;
    }
    public Status status;

    [System.Serializable]
    public class Action
    {
        public bool Idle;
        public bool Shoot;
        public bool Reload;
        public bool Dead;
    }
    public Action action;

    public float timeToShoot;
    private bool isUpgrading;

    //Components
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public BoxCollider2D boxCollider2D;

    [Header("For assign object")]

    [HideInInspector]
    public GameObject general;
    [HideInInspector]
    public GameObject pf_bullet;
    [HideInInspector]
    public Transform normalSpawnBullet;
    [HideInInspector]
    public Transform upgradeSpawnBullet;
    [HideInInspector]
    public GameObject healthShootBar;
    [HideInInspector]
    public UISlider healthBar;
    [HideInInspector]
    public UISlider shootBar;
    [HideInInspector]
    public GameObject reloadAlert;
    [HideInInspector]
    public GameObject upgradeAlert;
    [HideInInspector]
    public UISlider upgradeBar;
    [HideInInspector]
    public UILabel priceUpgrade;
    [HideInInspector]
    public UI2DSprite freezeEffect;

    //for virtual void awake, start, update, lateupdate
    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }

    private float timeUpdate = 0.2f;
    [HideInInspector]
    public float freezeTime = 2;
    const float percentUpgradePrice = 200; //%
    bool isEnoughGoldToUpgarde;

    //Shoot Controller
    private bool isWaitForShoot;
    private bool isFirstShoot;

    #endregion

    #region System Void
    void Awake()
    {
        AssignObject();
        OnAwake();
    }

    void Start()
    {
        data = Master.UnitData.GetUnitDataWithUpgradeByID(unitID);
        status.CurrentHealth = data.Health;
        status.CurrentAmmo = data.NumberOfAmmoToReload;
        Idle();
        GetBulletPrefabs();

        OnStart();

        Master.Touch.AddTouchEvent(TouchController.TouchType.TouchUp, OnTouchUp);

    }

    void Update()
    {
        doUpgrade();
        if (status.IsFreezing && !action.Dead)
        {
            animator.speed = 0;
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
        Master.Touch.RemoveTouchEvent(TouchController.TouchType.TouchUp, OnTouchUp);
        CancelInvoke();
        StopAllCoroutines();
    }


    private void AssignObject()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = Master.GetChildByName(gameObject, "Sprite").GetComponent<SpriteRenderer>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        GameObject pf_general = Master.GetGameObjectInPrefabs("Characters/Units/General");
        general = NGUITools.AddChild(gameObject, pf_general);

        normalSpawnBullet = Master.GetChildByName(gameObject, "NormalSpawnBullet").transform;
        upgradeSpawnBullet = Master.GetChildByName(gameObject, "UpgradeSpawnBullet").transform;
        healthShootBar = Master.GetChildByName(gameObject, "HealthShootBar");
        healthBar = Master.GetChildByName(gameObject, "HealthBar").GetComponent<UISlider>();
        shootBar = Master.GetChildByName(gameObject, "ShootBar").GetComponent<UISlider>();
        reloadAlert = Master.GetChildByName(gameObject, "ReloadAlert");
        upgradeAlert = Master.GetChildByName(gameObject, "UpgradeAlert");
        upgradeBar = Master.GetChildByName(gameObject, "UpgradeBar").GetComponent<UISlider>();
        priceUpgrade = Master.GetChildByName(gameObject, "PriceLabelUpgrade").GetComponent<UILabel>();
        freezeEffect = Master.GetChildByName(gameObject, "FreezeEffect").GetComponent<UI2DSprite>();

        healthShootBar.gameObject.SetActive(false);
        reloadAlert.SetActive(false);
        upgradeAlert.SetActive(false);
        upgradeBar.gameObject.SetActive(false);
        freezeEffect.gameObject.SetActive(false);

    }


    #endregion

    #region Action Controller
    public void SetActive(bool isActive)
    {
        status.IsActive = isActive;
        if (status.IsActive)
        {
            SetTimeToShoot();
            Idle();
            ActionController();
            priceUpgrade.text = (data.Price * (percentUpgradePrice / 100)).ToString();
        }
    }

    private void ActionController()
    {
        InvokeRepeating("ShootController", timeUpdate, timeUpdate);
        InvokeRepeating("HealthController", timeUpdate / 10, timeUpdate / 10);
        InvokeRepeating("ReloadController", timeUpdate, timeUpdate);
        InvokeRepeating("UpgradeController", timeUpdate, timeUpdate / 10);
    }


    public virtual void ShootController()
    {

        if (action.Dead || status.IsFreezing) return;

        CheckIsCanShoot();

        if (status.IsCanShoot)
        {
            if (!isFirstShoot)
            {
                Shoot();
                isFirstShoot = true;
            }
            else
            {
                timeToShoot -= Time.deltaTime * 15;
                if (timeToShoot <= 0)
                {
                    Shoot();
                }
                //if (!isWaitForShoot)
                //{
                //    isWaitForShoot = true;
                //    Master.WaitAndDo(timeToShoot, () =>
                //    {
                //        isWaitForShoot = false;
                //        Shoot();
                //    }, this);
                //}
            }
        }
        else
        {
            isFirstShoot = false;
        }

    }

    public virtual void CheckIsCanShoot()
    {
        if (action.Dead || status.IsFreezing) return;

        //check condition
        if ((status.CurrentAmmo <= 0 && data.NumberOfAmmoToReload != -1) || action.Reload || action.Dead)
        {
            status.IsCanShoot = false;
            return;
        }
        //check exist enemy in lane

        if (!Master.Lane.isExistCharacterByTagInLane(status.CurrentLane, "Enemy"))
        {
            status.IsCanShoot = false;
            return;
        }

        //check range
        //get list distace of each enemy
        bool isCanShoot = false;
        foreach (GameObject obj in Master.Lane.GetCharactersInLaneByTag(status.CurrentLane, "Enemy"))
        {
            if (transform.position.x > obj.transform.position.x)
            {
                continue;
            }
            float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
            if (data.Range > distance * 100)
            {
                isCanShoot = true;
            }
        }
        status.IsCanShoot = isCanShoot;
    }

    public virtual void SetTimeToShoot()
    {
        timeToShoot = Master.UnitData.maxTimeToShoot - (data.AttackSpeed * 0.1f);
        if (timeToShoot <= 0) timeToShoot = Master.UnitData.minTimeToShoot;
    }

    //Health Controller
    public virtual void HealthController()
    {

        if (action.Dead) return;

        float healthValue = status.CurrentHealth / data.Health;

        if (healthBar.value > healthValue)
        {
            healthBar.value -= Time.deltaTime;
        }
        else if (healthBar.value <= healthValue)
        {
            healthBar.value = healthValue;
        }

        //shoot bar
        float shootValue = (float)status.CurrentShoot / data.NumberOfShootToUpgrade;
        if (shootBar.value < shootValue)
        {
            shootBar.value += Time.deltaTime;
        }
        else if (shootBar.value >= shootValue)
        {
            shootBar.value = shootValue;
        }

        if (!isHoldingTouch)
        {
            if (healthValue == 1 || healthBar.value <= 0)
            {
                healthShootBar.gameObject.SetActive(false);
            }
            else
            {
                healthShootBar.gameObject.SetActive(true);
            }
        }

        if (action.Dead) healthBar.gameObject.SetActive(false);

        if (status.CurrentHealth <= 0)
        {
            Dead();
        }
    }

    //Reload Controller
    private bool isPlaySoundReload = false;
    public virtual void ReloadController()
    {
        if (action.Dead || status.IsFreezing) return;

        if (status.CurrentAmmo <= 0 && !action.Reload && data.NumberOfAmmoToReload != -1)
        {
            reloadAlert.SetActive(true);
            if (!isPlaySoundReload)
            {
                Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.ReloadUnit, gameObject.transform.position - new Vector3(0, 0.13f, 0));
                Master.Audio.PlaySound("snd_unitNeedReload");
                isPlaySoundReload = true;
            }
        }
        else
        {
            reloadAlert.SetActive(false);
            isPlaySoundReload = false;
        }
    }

    //Upgrade Controller
    public virtual void UpgradeController()
    {
        if (action.Dead || status.IsFreezing) return;

        if (status.CurrentShoot >= data.NumberOfShootToUpgrade && !status.IsUpgraded)
        {
            status.IsCanUpgrade = true;
        }
        else
        {
            status.IsCanUpgrade = false;
        }

        if (status.IsCanUpgrade && !status.IsUpgraded && !status.IsUpgrading)
        {
            if (!reloadAlert.activeSelf)
            {
                upgradeAlert.SetActive(true);
                shootBar.gameObject.SetActive(false);

                if (!Master.Tutorial.isDoingTutorial)
                {
                    Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.UpgradeUnitInGameplay, gameObject.transform.position - new Vector3(0, 0.13f, 0), true, 5);
                }
            }
            else
            {
                upgradeAlert.SetActive(false);
                shootBar.gameObject.SetActive(true);
            }
        }
        else
        {
            upgradeAlert.SetActive(false);
        }

        if (status.IsUpgraded)
        {
            shootBar.gameObject.SetActive(false);
        }

        if (isHoldingTouch && status.IsCanUpgrade && !status.IsUpgrading)
        {
            if (!isStartedUpgrade)
            {
                isStartedUpgrade = true;
                Master.WaitAndDo(0.1f, () =>
                {
                    status.IsUpgrading = true;
                    upgradeBar.gameObject.SetActive(true);
                    upgradeAlert.SetActive(false);
                }, this);
            }
        }

        if (!isHoldingTouch)
        {
            status.IsUpgrading = false;
            upgradeBar.gameObject.SetActive(false);
            upgradeBar.value = 0;
        }

        if (Master.Gameplay.gold >= (data.Price * (percentUpgradePrice / 100)))
        {
            priceUpgrade.color = Color.yellow;
            isEnoughGoldToUpgarde = true;
        }
        else
        {
            priceUpgrade.color = Color.red;
            isEnoughGoldToUpgarde = false;
        }
    }

    private bool isStartedUpgrade;
    private void doUpgrade()
    {
        if (status.IsUpgrading && isHoldingTouch)
        {
            if (!isEnoughGoldToUpgarde)
            {
                upgradeBar.GetComponent<AudioSource>().Stop();
                return;
            }

            if (!upgradeBar.GetComponent<AudioSource>().isPlaying)
            {
                upgradeBar.GetComponent<AudioSource>().Play();
            }

            upgradeBar.value += Time.deltaTime;
            if (upgradeBar.value >= 1)
            {
                status.IsUpgraded = true;
                status.IsUpgrading = false;
                status.IsCanUpgrade = false;
                upgradeBar.gameObject.SetActive(false);
                upgradeAlert.SetActive(false);
                isStartedUpgrade = false;
                healthBar.transform.localPosition = new Vector3(healthBar.transform.localPosition.x, healthBar.transform.localPosition.y + 11, healthBar.transform.localPosition.z);
                Master.Audio.PlaySound("snd_buy");
                Master.Gameplay.gold -= (int)(data.Price * (percentUpgradePrice / 100));

                Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.UpgradeUnitInGameplay);


                IncreaseStats();
            }
        }
        else
        {
            upgradeBar.GetComponent<AudioSource>().Stop();
        }
    }

    private void IncreaseStats()
    {
        data = Master.UnitData.GetUnitDataAfterUpgradeInGameplay(data);
        status.CurrentHealth = data.Health;
    }


    private bool isHoldingTouch;
    public virtual void OnTouchIn()
    {
        if (status.CurrentAmmo <= 0 && data.NumberOfAmmoToReload != -1)
        {

            if (Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.ReloadUnit))
            {
                Master.WaitAndDo(5, () =>
                {
                    if (Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.UnitAllowedDead))
                    {
                        Master.WaitAndDo(5, () =>
                        {
                            Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.UnitAllowedDead);
                        }, this);
                    }
                }, this);
            }

            Reload();
            return;
        }
        isHoldingTouch = true;
    }
    public void OnTouchUp()
    {
        isStartedUpgrade = false;
        isHoldingTouch = false;
        if (status.CurrentHealth == data.Health)
        {
            healthShootBar.SetActive(false);
        }
    }
    public virtual void OnTouching()
    {
        if (status.IsActive)
        {
            healthShootBar.SetActive(true);
        }
    }

    void CollisionController(GameObject obj)
    {
        if (obj.tag == "WeaponEnemy")
        {
            GetHit(obj.transform.parent.gameObject.GetComponent<EnemyController>().data.Damage);
            if (obj.name == "WeaponEnemyFreeze")
            {
                GetFreeze(freezeTime);
            }
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
        if (action.Dead || status.IsFreezing) return;

        animator.speed = 1;
        SetStatus("Idle");
        PlayAnimation("Idle");

    }

    public virtual void Shoot()
    {
        if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing) return;

        animator.speed = 1;
        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetTimeToShoot();
        SetStatus("Shoot");
        PlayAnimation("Shoot", () =>
        {
            Idle();
        });
        CreateBullet();

    }

    public virtual void Reload()
    {
        if (action.Dead || action.Reload || status.IsFreezing) return;
        Master.Audio.PlaySound("snd_unitReload");
        animator.speed = 0.6f;
        SetStatus("Reload");
        PlayAnimation("Reload", () =>
        {
            status.CurrentAmmo = data.NumberOfAmmoToReload;
            Idle();
            isFirstShoot = false;
        });
    }

    public virtual void Dead()
    {
        if (action.Dead) return;

        status.IsFreezing = false;

        SetStatus("Dead");

        animator.speed = 1f;
        general.SetActive(false);
        Master.Lane.RemoveCharacterAtLane(status.CurrentLane, gameObject);
        Master.Gameplay.unitsDead++;
        Master.Gameplay.CheckLevelComplete();
        Master.Audio.PlaySound("snd_unitDead");
        Master.UIGameplay.HightlightUnitDeadLabel();
        PlayAnimation("Dead", () =>
        {
            DOTween.ToAlpha(() => spriteRenderer.color, x => spriteRenderer.color = x, 0, 1).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
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

    public virtual void GetHit(float damage)
    {
        status.CurrentHealth -= damage;
    }

    #endregion

    public void SetStatus(string actionType)
    {
        action.Idle = false;
        action.Shoot = false;
        action.Reload = false;
        action.Dead = false;

        if (actionType == "Idle")
        {
            action.Idle = true;
        }
        else if (actionType == "Shoot")
        {
            action.Shoot = true;
        }
        else if (actionType == "Reload")
        {
            action.Reload = true;
        }
        else if (actionType == "Dead")
        {
            action.Dead = true;
        }

    }

    public void CreateBullet(Vector3 spawnPos, float timeToDestroy = -1)
    {
        GameObject obj_bullet = NGUITools.AddChild(Master.Gameplay.gameplayRoot, pf_bullet);
        obj_bullet.GetComponent<BulletController>().damage = data.Damage;
        obj_bullet.transform.localPosition = spawnPos;

        if (timeToDestroy != -1)
        {
            Destroy(obj_bullet, timeToDestroy);
        }

    }

    private void GetBulletPrefabs()
    {
        string bulletName = "";
        if (!status.IsUpgraded)
        {
            bulletName = "Bullet_" + data.UnitID;
        }
        else
        {
            bulletName = "Bullet_" + data.UnitID + "_Upgrade";
        }
        GameObject pf_bullet = Resources.Load<GameObject>("Prefabs/Bullets/" + bulletName);

        if (pf_bullet == null)
        {
            pf_bullet = Resources.Load<GameObject>("Prefabs/Bullets/Bullet_" + data.UnitID);
        }
    }

    public void CreateBullet(float timeToDestroy = -1)
    {
        string bulletName = "";
        PlayShootSound();
        if (!status.IsUpgraded)
        {
            bulletName = "Bullet_" + data.UnitID;
        }
        else
        {
            bulletName = "Bullet_" + data.UnitID + "_Upgrade";
        }

        GameObject pf_bullet = Resources.Load<GameObject>("Prefabs/Bullets/" + bulletName);

        if (pf_bullet == null)
        {
            pf_bullet = Resources.Load<GameObject>("Prefabs/Bullets/Bullet_" + data.UnitID);
        }

        GameObject obj_bullet = NGUITools.AddChild(Master.Gameplay.gameplayRoot, pf_bullet);
        obj_bullet.GetComponent<BulletController>().damage = data.Damage;
        //crit
        int rNumer = Random.Range(0, 101);
        if (rNumer <= data.Critical)
        {
            obj_bullet.GetComponent<BulletController>().isCrit = true;
        }

        if (!status.IsUpgraded)
        {
            obj_bullet.transform.position = normalSpawnBullet.position;
        }
        else
        {
            obj_bullet.transform.position = upgradeSpawnBullet.position;
        }

        if (timeToDestroy != -1)
        {
            Destroy(obj_bullet, timeToDestroy);
        }
    }

    public virtual void PlayShootSound()
    {
        string soundName = "snd_unitShoot_" + unitID;
        if (status.IsUpgraded)
        {
            AudioClip clip = Resources.Load<AudioClip>("Audios/Sounds/UnitShoots/" + soundName + "_upgraded");
            if (clip != null)
            {
                soundName += "_upgraded";
            }
        }
        //volume
        float volume = 1;
        switch (unitID)
        {
            case "03":
                volume = 0.5f;
                break;
            case "04":
                volume = 0.1f;
                break;
            case "07":
                volume = 0.3f;
                break;
            case "08":
                volume = 0.4f;
                break;
        }
        Master.Audio.PlaySound("UnitShoots/" + soundName, volume);
    }

    private void SpriteController()
    {
        string statusFolder = "";
        if (status.IsUpgraded)
        {
            statusFolder = "Upgrade/";
        }
        else
        {
            statusFolder = "Normal/";
        }

        if (action.Idle)
        {
            statusFolder += "Idle";
        }
        else if (action.Shoot)
        {
            statusFolder += "Shoot";
        }
        else if (action.Reload)
        {
            statusFolder += "Reload";
        }
        else if (action.Dead)
        {
            statusFolder += "Dead";
        }

        string spriteName = spriteRenderer.sprite.name;
        string pathToSprite = "Textures/Characters/Units/Unit_" + data.UnitID + "/" + statusFolder + "/" + spriteName;
        spriteRenderer.sprite = Resources.Load<Sprite>(pathToSprite);
        if (status.IsActive)
        {
            spriteRenderer.sortingOrder = -1000 - (int)gameObject.transform.localPosition.y;
        }
        else
        {
            spriteRenderer.sortingOrder = 1000;
        }
    }

    public void SetColliderSize()
    {
        if (status.IsUpgraded)
        {
            boxCollider2D.size = new Vector2(boxCollider2D.size.x * 1.2f, boxCollider2D.size.y);
        }
    }

    public void PlayAnimation(string animationClipName, System.Action onAnimationComplete = null)
    {
        animator.Play(animationClipName, 0, 0);
        Master.WaitAndDo(0.01f, () =>
        {
            StartCoroutine(DetectAnimationOnComplete(onAnimationComplete));
        }, this);
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
