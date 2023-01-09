using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Enemy07 : EnemyController
{
    GameObject explosion;

    public override void OnAwake()
    {
        explosion = Master.GetChildByName(gameObject, "Explosion");
        explosion.SetActive(false);
    }

    public override void Dead()
    {
        if (stopAllAction) return;

        stopAllAction = true;
        RemoveAllEffect();
        healthBar.gameObject.SetActive(false);
        SetActionStatus("Dead");
        Master.Lane.RemoveCharacterAtLane(status.CurrentLane, gameObject);
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        DropGoldController();
        Master.Audio.PlaySound("snd_zombieDead" + Random.Range(1, 4));
        Master.QuestData.IncreaseProgressValue("01");
        Master.Gameplay.CheckLevelComplete();

        PlayAnimation("Dead", () =>
        {

            Master.Audio.PlaySound("snd_countdownBomb", 0.25f);
            Master.WaitAndDo(0.8f, () =>
            {
                explosion.SetActive(true);
                spriteRenderer.gameObject.SetActive(false);
                Master.Effect.ShakeCamera(1, 0.19f);
                Master.Audio.PlaySound("snd_explosion");
                Master.PlayAnimation(explosion.GetComponent<SpriteRenderer>(), "Textures/Effects/Explosion", 0, 11, 0.05f, false, () =>
                {
                    Destroy(gameObject);
                }, null);
            });
            
        });
    }
}
