using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

public class StatsController : MonoBehaviour
{

    // Use this for initialization


    [Header("Global Value")]
    private ObscuredInt _gem;
    public ObscuredInt Gem
    {
        get
        {
            return _gem;
        }
        set
        {
            if (value > _gem)
            {
                ObscuredInt changeValue = value - _gem;
                Master.QuestData.IncreaseProgressValue("08", changeValue);
            }
            _gem = value;

            SaveData();
        }
    }

    private ObscuredInt _star;
    public ObscuredInt Star
    {
        get
        {
            return _star;
        }
        set
        {
            if (value > _star)
            {
                ObscuredInt changeValue = value - _star;
                Master.QuestData.IncreaseProgressValue("09", changeValue);
            }

            _star = value;

            SaveData();
        }
    }

    private ObscuredInt _maxEnergy;
    public ObscuredInt MaxEnergy
    {
        get
        {
            return _maxEnergy;
        }
        set
        {
            _maxEnergy = value;
            SaveData();
        }
    }

    private ObscuredInt _energy;
    public ObscuredInt Energy
    {
        get
        {
            return _energy;
        }
        set
        {
            if (value < _energy)
            {
                ObscuredInt changeValue = _energy - value;
                Master.QuestData.IncreaseProgressValue("10", changeValue);
            }

            _energy = value;

            if (_energy > _maxEnergy) _energy = _maxEnergy;

            if (_energy < 0) _energy = 0;

            SaveData();

            Master.PushNotification.SetEnergyNotification();

        }
    }
    private ObscuredInt firstGemPerEnergy = 10;
    private ObscuredFloat increasePercentGemPerEnergyPerLevel = 3.5f;

    public ObscuredInt GemPerEnergy
    {
        get
        {
            return (ObscuredInt)Master.IncreaseValues(firstGemPerEnergy, Master.LevelData.lastLevel, increasePercentGemPerEnergyPerLevel);
        }
    }

    //public int minuteFillPerEnergy = 5;
    public ObscuredInt minuteFillPerEnergy
    {
        get
        {
            return (ObscuredInt)Master.IncreaseValues(5, Master.LevelData.lastLevel, increasePercentGemPerEnergyPerLevel);
        }
    }

    public ObscuredInt timeRemainingCountdownEnergy = 0;

    public ObscuredInt _timesPlay = 0;
    public ObscuredInt TimesPlay
    {
        get
        {
            return _timesPlay;
        }
        set
        {
            _timesPlay = value;
            SaveData();
        }
    }

    private ObscuredInt _timesLevelComplete = 0;
    public ObscuredInt TimesLevelComplete
    {
        get
        {
            return _timesLevelComplete;
        }
        set
        {
            _timesLevelComplete = value;
            SaveData();
        }
    }

    public static ObscuredInt GoldPerCoin = 15;

    [Space(15)]
    //public int timesLevelComplete = 0;
    [HideInInspector]
    public ObscuredInt timesVictory = 0;
    [HideInInspector]
    public ObscuredInt timesFail = 0;
    [HideInInspector]
    public ObscuredInt timesShowInterAd = 0;
    [HideInInspector]
    public ObscuredInt timesShowUnityAd = 0;


    public bool isTest;
    public bool isClickedRating;

    void Awake()
    {
        if (Master.Stats == null)
        {
            Master.Stats = this;
        }

        ReadData();
    }

    void Start()
    {
        //ObscuredPrefs.DeleteAll();
        CheckEnergyGot();
        CountdownTimeEnergy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        SetLastTimeInGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SetLastTimeInGame();
        }
        else
        {
            CheckEnergyGot();
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    void SetLastTimeInGame()
    {
        string lastTimeInGame = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ObscuredPrefs.SetString("lastTimeInGame", lastTimeInGame);
        ObscuredPrefs.SetInt("lastTimeRemainingEnergy", timeRemainingCountdownEnergy);
        ObscuredPrefs.Save();
    }

    public void SaveData()
    {
        ObscuredPrefs.SetInt("Gem", _gem);
        ObscuredPrefs.SetInt("Star", _star);
        ObscuredPrefs.SetInt("Energy", _energy);
        ObscuredPrefs.SetInt("MaxEnergy", _maxEnergy);
        ObscuredPrefs.SetInt("TimesPlay", _timesPlay);
        ObscuredPrefs.SetInt("TimesLevelComplete", _timesLevelComplete);
        ObscuredPrefs.Save();
    }

    public void ReadData()
    {
        _gem = ObscuredPrefs.GetInt("Gem", 0);
        _star = ObscuredPrefs.GetInt("Star", 0);
        _energy = ObscuredPrefs.GetInt("Energy", 0);
        _timesPlay = ObscuredPrefs.GetInt("TimesPlay", 0);
        _timesLevelComplete = ObscuredPrefs.GetInt("TimesLevelComplete", 0);
        _maxEnergy = 5;

        isClickedRating = ObscuredPrefs.GetInt("isClickedRating", 0) == 0 ? false : true;
    }

    public void CheckEnergyGot()
    {
        string lastTimeInGameStr = ObscuredPrefs.GetString("lastTimeInGame", "");
        if (lastTimeInGameStr == "")
        {
            Energy = MaxEnergy;
        }
        else
        {
            int lastTimeRemaining = ObscuredPrefs.GetInt("lastTimeRemainingEnergy", 0);

            DateTime lastTimeInGame = DateTime.ParseExact(lastTimeInGameStr, "yyyy-MM-dd HH:mm:ss", null);
            DateTime currentDateTime = DateTime.Now;
            int totalSeconds = (int)(currentDateTime - lastTimeInGame).TotalSeconds;
            int minute = totalSeconds / 60;
            int second = totalSeconds % 60;
            int minuteReduntant = 0;
            int energyGot = 0;
            if (minute > 0)
            {
                energyGot = minute / minuteFillPerEnergy;
                minuteReduntant = minute % minuteFillPerEnergy;
            }
            Energy += energyGot;
            timeRemainingCountdownEnergy = (minuteFillPerEnergy * 60) - (minuteReduntant * 60 + second) - ((minuteFillPerEnergy * 60) - lastTimeRemaining);
        }
    }

    public void CountdownTimeEnergy()
    {
        Master.WaitAndDo(1, () =>
        {
            if (Energy >= MaxEnergy)
            {
                timeRemainingCountdownEnergy = minuteFillPerEnergy * 60;
                CountdownTimeEnergy();
                return;
            }

            timeRemainingCountdownEnergy -= 1;
            if (timeRemainingCountdownEnergy <= 0)
            {
                Energy++;
                timeRemainingCountdownEnergy = (minuteFillPerEnergy * 60);
            }
            CountdownTimeEnergy();
        }, this, true);

    }

}
