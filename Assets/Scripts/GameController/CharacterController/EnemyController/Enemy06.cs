using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Enemy06 : EnemyController
{
    public override void OnStart()
    {
        timeToAttack = 0;
    }

    public override void Attack()
    {
        if (stopAllAction && !status.IsCanAttack) return;

        animator.speed = 0.7f;
        SetActionStatus("Attack");
        SetTimeToAttack();
        PlayAnimation("Attack", () =>
        {

            Master.Effect.ShakeCamera(1, 0.2f);
            Master.Audio.PlaySound("snd_explosion");
            weapon.SetActive(true);
            Debug.Log(weapon);
            Dead();
        });

    }

    public override void Dead()
    {
        if (stopAllAction) return;

        stopAllAction = true;
        general.SetActive(false);
        SetActionStatus("Dead");
        Master.Lane.RemoveCharacterAtLane(status.CurrentLane, gameObject);
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        DropGoldController();
        Master.Audio.PlaySound("snd_zombieDead" + Random.Range(1, 4));
        Master.QuestData.IncreaseProgressValue("01");
        Master.Gameplay.CheckLevelComplete();

        PlayAnimation("Dead", () =>
        {
            weapon.SetActive(false);
            DOTween.ToAlpha(() => spriteRenderer.color, x => spriteRenderer.color = x, 0, 1).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
    }

}
