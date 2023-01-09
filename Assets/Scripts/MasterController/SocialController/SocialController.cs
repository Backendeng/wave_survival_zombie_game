using UnityEngine;
using System.Collections;

public class SocialController : MonoBehaviour
{

    // Use this for initialization


    [Header("Auto Ask Share Setting")]
    public bool isAutoAskShare = true;
    [Space(5)]
    [Tooltip("Auto show ask share dialog when")]
    public AskShareWhen askShareWhen;
    public enum AskShareWhen
    {
        [Tooltip("When complete level and get 3 star")]
        LevelCompleteAndGet3Star,
        [Tooltip("When complete level and get 3 star at level")]
        LevelCompleteAndGet3StarAtLevel,
        [Tooltip("When over times level complete, you must enter 'Times Level Complete To Ask Share'")]
        TimesCompleteLevel,
        [Tooltip("When complete at specify level, you must enter 'List Complete Level To Ask Share'")]
        CompleteAtLevel,
        [Tooltip("When complete at all level")]
        CompleteAllLevel,
    }
    [Header("Value of Condition Ask Share When")]
    [Space(5)]
    [Tooltip("Require 'Times Complete Level' at 'Ask Share When'")]
    public int timesLevelCompleteToAskShare = 3;
    [Space(5)]
    [Tooltip("Require 'Complete At Level' at 'Ask Share When'")]
    public int[] listCompleteLevelToAskShare;
    [Space(5)]
    [Tooltip("Require 'Level Complete And Get 3 Star At Level' at 'Ask Share When'")]
    public int[] listLevelCompleteGet3StarToAskShare;


    GameObject askShareDialog;

    public FacebookController Facebook;
    

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.Social != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Master.Social = this;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool CheckShowAskShareDialog()
    {
        /*
        if (isAutoAskShare)
        {
            bool isShowAskShareDialog = false;
            switch (askShareWhen)
            {
                ////LevelCompleteAndGet3Star
                case AskShareWhen.LevelCompleteAndGet3Star:
                    if (Master.Gameplay.star == 3)
                    {
                        isShowAskShareDialog = true;
                        //ShowAskShareDialog();
                    }
                    break;

                ///LevelCompleteAndGet3StarAtLevel
                case AskShareWhen.LevelCompleteAndGet3StarAtLevel:
                    foreach (int level in listLevelCompleteGet3StarToAskShare)
                    {
                        if (Master.Profile.currentLevel == level)
                        {
                            if (Master.Gameplay.star == 3)
                            {
                                isShowAskShareDialog = true;
                                //ShowAskShareDialog();
                                break;
                            }
                        }
                    }
                    break;

                ///TimesCompleteLevel
                case AskShareWhen.TimesCompleteLevel:
                    if (Stats.timesLevelComplete % timesLevelCompleteToAskShare == 0)
                    {
                        isShowAskShareDialog = true;
                        // ShowAskShareDialog();
                    }
                    break;

                ///CompleteAtLevel
                case AskShareWhen.CompleteAtLevel:
                    foreach (int level in listCompleteLevelToAskShare)
                    {
                        if (Master.Profile.currentLevel == level)
                        {
                            isShowAskShareDialog = true;
                            // ShowAskShareDialog();
                            break;
                        }
                    }
                    break;

                ///CompleteAllLevel
                case AskShareWhen.CompleteAllLevel:
                    isShowAskShareDialog = true;
                    // ShowAskShareDialog();
                    break;
            }

            return isShowAskShareDialog;
        }
        else
        {
            return false;
        }
        */
        return false;
    }

    public void ShareFacebook(bool attackPicture = true)
    {
        Facebook.ShareLink("http://google.com", "Share Facebook Zombie Defense", "Check Share Facebook Zombie Defense", "https://i.imgsafe.org/30aacd1.png");
        Debug.Log("Share Facebook!");
        /*
        Master.Gameplay.DisableTouchGameplay();
        Master.UI.DisableTouchUI();

        if (attackPicture)
        {
            StartCoroutine(TakeScreenShotToShareFacebook());
        }
        else
        {
            UM_ShareUtility.FacebookShare(GetTextShare(), null);
        }

        Master.Gameplay.EnableTouchGameplay();
        Master.UI.EnableTouchUI();
        //StartCoroutine(TakeScreenShotToShareFacebook(content));
        */
    }

    public void ShareTwitter(bool attackPicture = true)
    {
        Debug.Log("Share Twitter!");
        /*
        Master.Gameplay.DisableTouchGameplay();
        Master.UI.DisableTouchUI();

        if (attackPicture)
        {
            StartCoroutine(TakeScreenShotToShareTwitter());
        }
        else
        {
            UM_ShareUtility.TwitterShare(GetTextShare(), null);
        }
        Master.Gameplay.EnableTouchGameplay();
        Master.UI.EnableTouchUI();
        */
    }

    string GetTextShare()
    {
        //return "I got level " + Master.Profile.currentLevel + " at 'Kill The Zombies' - Hot Mobile Game" +
        //    "\n Android: " + Master.instance.linkGooglePlay +
        //   "\n #killthezombies #zombiesgame #puzzlegame";
        return "";
    }

    private IEnumerator TakeScreenShotToShareFacebook()
    {
        yield return new WaitForEndOfFrame();

        Texture2D snap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        snap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        snap.Apply();

        UM_ShareUtility.FacebookShare(GetTextShare(), snap);
       // Master.Gameplay.EnableTouchGameplay();
     //   Master.UI.EnableTouchUI();
    }

    private IEnumerator TakeScreenShotToShareTwitter()
    {
        yield return new WaitForEndOfFrame();
        Texture2D snap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        snap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        snap.Apply();

        UM_ShareUtility.TwitterShare(GetTextShare(), snap);
      //  Master.Gameplay.EnableTouchGameplay();
      //  Master.UI.EnableTouchUI();
    }

}
