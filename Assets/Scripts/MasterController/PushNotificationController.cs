using UnityEngine;
using System.Collections;

public class PushNotificationController : MonoBehaviour
{

    // Use this for initialization
    public class NotificationID
    {
        public static int FullEnergy = 0;
        public static int GetFreeReward = 1;
        public static int LongTimeNoLogin = 2;
    }

    void Awake()
    {
        if (Master.PushNotification == null)
        {
            Master.PushNotification = this;
        }
    }

    void Start()
    {
        SetAllNotification();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SetAllNotification();
        }
    }

    public void SetAllNotification()
    {
        SetEnergyNotification();
        SetGetFreeRewardNotification();
        SetLongTimeNoLogin();
    }

    public void CancelAllNotification()
    {
        AndroidNotificationManager.Instance.CancelAllLocalNotifications();
    }

    public void SetEnergyNotification()
    {
        string title = "Special Squad vs Zombies";
        string content = "Your Energies are fully charged. Let kill all the zombies!";
        int secondDelay = 0;
        int remainEnergy = Master.Stats.MaxEnergy - Master.Stats.Energy;
        if (remainEnergy > 0)
        {
            secondDelay = (Master.Stats.minuteFillPerEnergy * (remainEnergy - 1)) * 60;
            secondDelay += Master.Stats.timeRemainingCountdownEnergy;
        }
        else
        {
            secondDelay = (Master.Stats.minuteFillPerEnergy * Master.Stats.MaxEnergy) * 60;
        }
        SetNotification(NotificationID.FullEnergy, secondDelay, title, content, "snd_fullenergynotification");
    }

    public void SetGetFreeRewardNotification()
    {
        if (Master.LevelData.lastLevel < FreeRewardController.levelCanGetFreeReward) return;

        string title = "Special Squad vs Zombies";
        string content = "Hurry up! Your free rewards are ready. Let get its now!";

        int secondDelay = 0;
        if (FreeRewardController.TimeRemainingFreeReward() > 0)
        {
            secondDelay = FreeRewardController.TimeRemainingFreeReward();
        }
        else
        {
            secondDelay = FreeRewardController.minuteGetRandomReward * 60;
        }

        SetNotification(NotificationID.GetFreeReward, secondDelay, title, content, "snd_getrewardnotification");
    }

    public void SetLongTimeNoLogin()
    {
        int secondDelay = 48 * 60 * 60;
        string title = "Special Squad vs Zombies";
        string content = "The Zombies are destroying the City, let defeat them now!";
 
        SetNotification(NotificationID.LongTimeNoLogin, secondDelay, title, content, "snd_fullenergynotification");
    }

    void SetNotification(int id, int secondDelay, string title, string content, string soundName, string icon = "notificationsmallicon", string largeIcon = "notificationicon")
    {

#if UNITY_EDITOR
        return;
#endif

        Debug.Log("Set Notification: " + id + " | " + secondDelay + " | " + title + " | " + content);
#if UNITY_ANDROID
        AndroidNotificationManager.Instance.CancelLocalNotification(id);
        AndroidNotificationBuilder builder = new AndroidNotificationBuilder(id, title, content, secondDelay);

        builder.SetSoundName(soundName);
        builder.SetIconName(icon);
        builder.SetLargeIcon(largeIcon);
        builder.SetVibration(true);
        builder.ShowIfAppIsForeground(false);
        AndroidNotificationManager.Instance.ScheduleLocalNotification(builder);


#elif UNITY_IOS
        var notif = new UnityEngine.iOS.LocalNotification();
        notif.fireDate = System.DateTime.Now.AddSeconds(secondDelay);
        notif.alertAction = title;
        notif.alertBody = content;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
    }

}
