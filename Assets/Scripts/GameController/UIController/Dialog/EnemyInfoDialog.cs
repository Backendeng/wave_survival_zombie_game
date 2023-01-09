using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyInfoDialog : DialogController
{

    string enemyID;
    EnemyDataController.EnemyData enemyData;
    GameObject enemyCard;
    GameObject enemyInfo;

    UITexture card;
    UILabel enemyName;

    UISlider healthValueSlider;
    UISlider damageValueSlider;
    UISlider moveSpeedValueSlider;
    UISlider attackSpeedValueSlider;

    UILabel ability;

    Transform[] pos = new Transform[2];

    bool isOpenInGameplay = false;

    public override void AssignObjects()
    {
        enemyCard = Master.GetChildByName(gameObject, "EnemyCard");
        enemyInfo = Master.GetChildByName(gameObject, "EnemyInfo");

        card = Master.GetChildByName(gameObject, "Card").GetComponent<UITexture>();
        enemyName = Master.GetChildByName(gameObject, "EnemyNameLabel").GetComponent<UILabel>();
        healthValueSlider = Master.GetChildByName(gameObject, "ValueHealth").GetComponent<UISlider>();
        damageValueSlider = Master.GetChildByName(gameObject, "ValueDamage").GetComponent<UISlider>();
        moveSpeedValueSlider = Master.GetChildByName(gameObject, "ValueMoveSpeed").GetComponent<UISlider>();
        attackSpeedValueSlider = Master.GetChildByName(gameObject, "ValueAttackSpeed").GetComponent<UISlider>();
        ability = Master.GetChildByName(gameObject, "AbilityLabel").GetComponent<UILabel>();

        pos[0] = Master.GetChildByName(gameObject, "SecondPosEnemyCard").transform;
        pos[1] = Master.GetChildByName(gameObject, "SecondPosEnemyInfo").transform;

    }

    public override void OnOpen(string[] agruments = null, Action onCloseComplete = null)
    {
        enemyID = agruments[0];

        if (agruments.Length > 1)
        {
            isOpenInGameplay = true;
        }

        if (isOpenInGameplay)
        {
            Time.timeScale = 0;
            this.onCloseComplete = () =>
            {
                Time.timeScale = 1;
            };
        }

        enemyCard.SetActive(true);
        enemyInfo.SetActive(false);

        enemyData = Master.EnemyData.GetEnemyDataByID(enemyID);

        card.mainTexture = Resources.Load<Texture2D>("Textures/Characters/Enemies/Enemy_" + enemyID + "/Card");
        enemyName.text = enemyData.EnemyName;
        ability.text = enemyData.Ability;
        SetStatsSlider(EnemyDataController.StatsType.Health);
        SetStatsSlider(EnemyDataController.StatsType.Damage);
        SetStatsSlider(EnemyDataController.StatsType.MoveSpeed);
        SetStatsSlider(EnemyDataController.StatsType.AttackSpeed);


    }

    public override void OnShowComplete()
    {
        float time = 0.4f;
        enemyInfo.SetActive(true);
        enemyCard.transform.DOMove(pos[0].position, time).SetUpdate(true);
        enemyInfo.transform.DOMove(pos[1].position, time).SetUpdate(true);
        TweenAlpha.Begin(enemyInfo, time, 1).ignoreTimeScale = true;
    }

    void SetStatsSlider(EnemyDataController.StatsType statsStype)
    {
        UISlider slider = healthValueSlider;
        float percent = 0;
        switch (statsStype)
        {
            case EnemyDataController.StatsType.Health:
                slider = healthValueSlider;
                percent = Master.EnemyData.CalcPercentStatsValue(EnemyDataController.StatsType.Health, enemyData.Health);
                break;
            case EnemyDataController.StatsType.Damage:
                slider = damageValueSlider;
                percent = Master.EnemyData.CalcPercentStatsValue(EnemyDataController.StatsType.Damage, enemyData.Damage);

                break;
            case EnemyDataController.StatsType.MoveSpeed:
                slider = moveSpeedValueSlider;
                percent = Master.EnemyData.CalcPercentStatsValue(EnemyDataController.StatsType.MoveSpeed, enemyData.MoveSpeed);

                break;
            case EnemyDataController.StatsType.AttackSpeed:
                slider = attackSpeedValueSlider;
                percent = Master.EnemyData.CalcPercentStatsValue(EnemyDataController.StatsType.AttackSpeed, enemyData.AttackSpeed);

                break;
        }

        float value = (percent / 100) * 1;
        slider.value = value;

    }


}
