using UnityEngine;
using System.Collections;
using System;

public class FreeRewardController : MonoBehaviour
{

    public static int levelCanGetFreeReward = 3;

    static float increaseGemPercentPerLevel = 5; //%
    static float increaseStarPercentPerLevel = 7;

    public static int minuteGetRandomReward = 15;
    static int[] randomRewardGem = new int[2] { 1, 2 };
    static int[] randomRewardStar = new int[2] { 40, 60 };

    static int[] facebookRewardGem = new int[2] { 4, 6 };
    static int[] facebookRewardStar = new int[2] { 100, 150 };

    public static int TimeRemainingFreeReward()
    {
        string dateTimeGetRewarStr = PlayerPrefs.GetString("DateTimeGetReward", "");

        if (dateTimeGetRewarStr == "")
        {
            return 0;
        }
        else
        {
            DateTime dateTimeGetReward = DateTime.ParseExact(dateTimeGetRewarStr, "yyyy-MM-dd HH:mm:ss", null);
            DateTime currentDateTime = DateTime.Now;

            if (currentDateTime.CompareTo(dateTimeGetReward) >= 0)
            {
                return 0;
            }
            else
            {
                return (int)(dateTimeGetReward - currentDateTime).TotalSeconds;
            }
        }
    }

    public static bool IsSharedFacebook()
    {
        string dateTimeSharedFacebookStr = PlayerPrefs.GetString("DateTimeGetShareFacebookReward", "");
        if (dateTimeSharedFacebookStr == "")
        {
            return false;
        }
        else
        {
            DateTime dateTimeSharedFacebook = DateTime.ParseExact(dateTimeSharedFacebookStr, "yyyy-MM-dd", null);
           // DateTime currentDateTime = DateTime.Now;
            DateTime currentDateTime = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);

            if (currentDateTime.CompareTo(dateTimeSharedFacebook) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsCanGetFreeReward()
    {
        if (TimeRemainingFreeReward() == 0 || !IsSharedFacebook())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SetDateTimeGetReward()
    {
        DateTime currentDateTime = DateTime.Now.AddMinutes(minuteGetRandomReward);
        string dateTimeStr = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString("DateTimeGetReward", dateTimeStr);
        PlayerPrefs.SetString("CurrentRandomReward", "");

        PlayerPrefs.Save();

    }

    public static void SetDatTimeGetShareFacebookReward()
    {
        DateTime currentDateTime = DateTime.Now;
        string dateTimeStr = currentDateTime.ToString("yyyy-MM-dd");
        PlayerPrefs.SetString("DateTimeGetShareFacebookReward", dateTimeStr);
        PlayerPrefs.SetString("CurrentShareFacebookReward", "");

        PlayerPrefs.Save();
    }

    public static int[] GetReward(int type) //0: random reward; 1: share facebook reward
    {
        //check current reward
        string rewardStr = "";
        string playerPref = "";
        if (type == 0)
        {
            playerPref = "CurrentRandomReward";
        }
        else
        {
            playerPref = "CurrentShareFacebookReward";
        }

        rewardStr = PlayerPrefs.GetString(playerPref, "");

        int[] reward = new int[2];//1: gem, 2:star
        int gem = 0;
        int star = 0;
        if (rewardStr == "")
        {
            if (type == 0)
            {
                gem = UnityEngine.Random.Range(randomRewardGem[0], randomRewardGem[1] + 1);
                star = UnityEngine.Random.Range(randomRewardStar[0], randomRewardStar[1] + 1);
            }
            else if (type == 1)
            {
                gem = UnityEngine.Random.Range(facebookRewardGem[0], facebookRewardGem[1]+1);
                star = UnityEngine.Random.Range(facebookRewardStar[0], facebookRewardStar[1]+1);
            }

            gem = (int)Master.IncreaseValues(gem, Master.LevelData.lastLevel, increaseGemPercentPerLevel);
            star = (int)Master.IncreaseValues(star, Master.LevelData.lastLevel, increaseStarPercentPerLevel);

            //for (int i = 0; i < Master.LevelData.lastLevel; i++)
            //{
            //    gem += (int)(gem * increaseGemPercentPerLevel) / 100;
            //}

            //for (int i = 0; i < Master.LevelData.lastLevel; i++)
            //{
            //    star += (int)(star * increaseStarPercentPerLevel) / 100;
            //}

            PlayerPrefs.SetString(playerPref, gem + "-" + star);
            PlayerPrefs.Save();

        }
        else
        {
            gem = int.Parse(rewardStr.Split('-')[0]);
            star = int.Parse(rewardStr.Split('-')[1]);

        }
        reward[0] = gem;
        reward[1] = star;

        return reward;
    }

    //public static int[] GetShareFacebookReward()
    //{
    //    int[] reward = new int[2];//1: gem, 2:star

    //    int gem = UnityEngine.Random.Range(facebookRewardGem[0], facebookRewardGem[1]);
    //    int star = UnityEngine.Random.Range(facebookRewardStar[0], facebookRewardStar[1]);

    //    for (int i = 0; i < Master.LevelData.lastLevel; i++)
    //    {
    //        gem += (int)(gem * increaseGemPercentPerLevel) / 100;
    //    }

    //    for (int i = 0; i < Master.LevelData.lastLevel; i++)
    //    {
    //        star += (int)(star * increaseStarPercentPerLevel) / 100;
    //    }

    //    return reward;
    //}




}
