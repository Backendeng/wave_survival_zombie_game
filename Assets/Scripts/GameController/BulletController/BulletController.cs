using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{

    // Use this for initialization


    [HideInInspector]
    public float damage;
    [HideInInspector]
    public bool isCrit;
    [HideInInspector]
    public Animator animator;

    public bool isChild;

    [Header("For Animation")]
    public bool autoPlayAnimation;
    public string animationName;
    public bool playAnimationWhenHitZombie;
    public string animationNameWhenHitZombie;

    [Header("For Movement")]
    public Movement movement = Movement.Move;
    public float moveSpeed;

    [Header("For Sound")]
    public bool isPlaySoundWhenHitEnemy;
    public string soundName;

    private bool isHitEnemy;

    public enum Movement
    {
        Fixed, Move
    }


    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        CheckAutoAnimation();

        if (isChild)
        {
            damage = transform.parent.gameObject.GetComponent<BulletController>().damage;
        }
        OnStart();

        //if (isCrit)
        //{
        //    damage = damage * 2;
        //}
    }

    void Update()
    {
        CheckOutOfScreen();
        MovementController();
        OnUpdate();
    }

    public virtual void OnStart()
    {

    }

    public virtual void OnUpdate() { }

    public void MovementController()
    {
        if (movement == Movement.Move && !isHitEnemy)
        {
            transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
        }
    }

    public void CheckAutoAnimation()
    {
        if (autoPlayAnimation)
        {
            animator.Play(animationName);
        }
    }

    public virtual void CollisionController(GameObject obj)
    {
        if (obj.tag == "Enemy")
        {
            isHitEnemy = true;
            Destroy(gameObject.GetComponent<BoxCollider2D>());

            obj.GetComponent<EnemyController>().GetHit(damage, isCrit);

            if (playAnimationWhenHitZombie)
            {
                if (isPlaySoundWhenHitEnemy)
                {
                    Master.Audio.PlaySound(soundName);
                }

                PlayAnimation(animationNameWhenHitZombie, () =>
                {
                    Destroy(gameObject);
                });
            }
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        CollisionController(col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        CollisionController(col.gameObject);
    }

    public void CheckOutOfScreen()
    {
        float x = gameObject.transform.localPosition.x;
        float maxX = 1280;
        if (x > maxX)
        {
            Destroy(gameObject);
        }
    }

    public void PlayAnimation(string animationClipName, System.Action onAnimationComplete = null)
    {
        animator.Play(animationClipName, 0, 0);
        Master.WaitAndDo(0.01f, () =>
        {
            StartCoroutine(DetectAnimationOnComplete(onAnimationComplete));
        });
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
