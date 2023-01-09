using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Enemy05 : EnemyController
{
    private GameObject weaponCollider;
    private Vector3 firstPosWeaponCollider;

    public override void OnAwake()
    {
        weaponCollider = Master.GetChildByName(gameObject, "WeaponCollier");
        firstPosWeaponCollider = weaponCollider.transform.localPosition;
        weaponCollider.SetActive(false);
    }

    public override void Attack()
    {
        if (stopAllAction || !status.IsCanAttack) return;

        status.IsAttacking = true;
        animator.speed = 1;
        SetActionStatus("Attack");
        SetTimeToAttack();

        PlayAnimation("Attack", () =>
        {
            Idle();
        });

        Master.WaitAndDo(0.3f, () =>
        {
            if (!status.IsCanAttack)
            {
                status.IsAttacking = false;
                return;
            }

            Master.Audio.PlaySound("snd_enemy05_attack", 0.2f);
            weapon.SetActive(true);
            weaponCollider.SetActive(true);
            weaponCollider.transform.localPosition = firstPosWeaponCollider;
            weaponCollider.transform.DOLocalMoveX(firstPosWeaponCollider.x - 200, 0.7f);

            weapon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Characters/Enemies/Enemy_05/Attack/Rocks/1");
            Master.Effect.ShakeCamera(1, 0.17f);
            Master.PlayAnimation(weapon.GetComponent<SpriteRenderer>(), "Textures/Characters/Enemies/Enemy_05/Attack/Rocks", 0, 5, 0.045f, false, () =>
            {
                Master.WaitAndDo(0.2f, () =>
                {
                    weaponCollider.transform.localPosition = firstPosWeaponCollider;
                    weaponCollider.SetActive(false);
                    weapon.SetActive(false);
                    status.IsAttacking = false;
                }, this);
            }, null);
        }, this);
    }

}
