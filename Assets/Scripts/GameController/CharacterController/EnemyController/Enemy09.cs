using UnityEngine;
using System.Collections;

public class Enemy09 : EnemyController
{
    bool isPlayedSound = false;
    AudioSource audioSource;
    //GameObject suggestUsingSkill2;
    public override void OnAwake()
    {
        audioSource = GetComponent<AudioSource>();
        //suggestUsingSkill2 = Master.GetChildByName(gameObject, "SuggestUsingSkill2");
    }

    public override void OnStart()
    {
        //audioSource.volume = Master.Audio.volumne;
        Master.PlayAnimation(weapon.GetComponent<SpriteRenderer>(), "Textures/Characters/Enemies/Enemy_09/Wifi", 0, 9, 0.05f, true);
    }

    public override void OnUpdate()
    {

        if (Master.isLevelComplete || Time.timeScale == 0)
        {
            //weapon.SetActive(false);
            audioSource.Stop();
            isPlayedSound = false;
        }
    }

    public override void AttackController()
    {
        if (stopAllAction) return;

        if (gameObject.transform.localPosition.x <= 490)
        {
            status.IsCanAttack = true;
        }
        else
        {
            status.IsCanAttack = false;
        }

        if (status.IsCanAttack)
        {
            weapon.SetActive(true);
            Master.Gameplay.isBlockUnitSelectByEnemy = true;
        }
        else
        {
            weapon.SetActive(false);
            Master.Gameplay.isBlockUnitSelectByEnemy = false;
        }

        if (status.IsCanAttack)
        {
            if (!isPlayedSound)
            {
                audioSource.Play();
                isPlayedSound = true;
            }
        }
        else
        {
            audioSource.Stop();
            isPlayedSound = false;
        }

        if (status.IsFreezing)
        {
            weapon.SetActive(false);
            audioSource.Stop();
            isPlayedSound = false;
            Master.Gameplay.isBlockUnitSelectByEnemy = false;
        }
    }

    public override void Dead()
    {
        audioSource.Stop();
        base.Dead();
        Master.Gameplay.isBlockUnitSelectByEnemy = false;
    }
}
