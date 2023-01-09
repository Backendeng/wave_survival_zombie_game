using UnityEngine;
using System.Collections;

public class AdController : MonoBehaviour
{

    // Use this for initialization
    public AdmobController Admob;
    public UnityAdController UnityAd;

    [HideInInspector]
    public bool isRemoveAd;

    [Header("Banner Ad Settings")]
    public bool isShowBannerAd = true;
    [Space(5)]
    public string androidBannerAdID = "";
    public string iOsBannerAdID = "";
    public string windowPhoneBannerAdID = "";
    public enum PositionBannerAd
    {
        Top, Bottom, BothTopAndBottom
    }
    public PositionBannerAd positionBannerAd = PositionBannerAd.Top;

    public enum ShowBannerAdWhen
    {
        AllTime, OnlyWhenLevelComplete, OnlyInMenu, OnlyInMenuAndLevelComplete, OnlyInGameplay
    }
    public ShowBannerAdWhen showBannerWhen = ShowBannerAdWhen.AllTime;

    [Space(10)]
    [Header("Interstitial Ad Setting")]
    public bool isShowInterstitialAd = true;
    [Space(5)]
    public string androidInterAdID = "";
    public string iOsInterAdID = "";
    public string windowPhoneInterAdID = "";
    public enum ShowInterstinialAdWhen
    {
        WhenLevelComplete, WhenStartLevel
    }
    [Tooltip("Condition to show Interstinial Ad")]
    public ShowInterstinialAdWhen showInterstinialAdWhen = ShowInterstinialAdWhen.WhenLevelComplete;
    public int timesPlayToShowInterstinialAd = 3;
    public int timesLevelCompleteToShowInterstinialAd = 2;

    [Space(10)]
    [Header("Unity Ad Setting")]
    public bool isShowUnityAd = true;
    [Space(5)]
    public string androidGameID = "";
    public string iOsGameID = "";
    public string zone = "rewardedVideo";

    public enum ShowUnityAdWhen
    {
        WhenLevelComplete, WhenStartLevel
    }

    [Tooltip("Condition to show Unity Ad")]
    public ShowUnityAdWhen showUnityAdWhen = ShowUnityAdWhen.WhenLevelComplete;
    public int timesPlayToShowUnityAd = 5;
    public int timesLevelCompleteToShowUnityAd = 3;

    [Space(10)]
    [Header("For Auto Show Remove Ad")]
    public bool isAutoShowRemoveAd = true;
    public enum ShowRemoveAdWhen
    {
        AfterInterAd,
        AfterUnityAd,
        AfterInterAdOrUnityAd,
        AfterTimesInterAd,
        AfterTimesUnityAd,
    }
    public ShowRemoveAdWhen showRemoveAdWhen = ShowRemoveAdWhen.AfterUnityAd;
    public int timesShowInterAdToShowRemoveAd = 2;
    public int timesShowUnityToShowRemoveAd = 2;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.Ad != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Master.Ad = this;
            Admob = FindObjectOfType<AdmobController>();
            UnityAd = FindObjectOfType<UnityAdController>();
        }
        CheckRemoveAd();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckRemoveAd()
    {
        isRemoveAd = PlayerPrefs.GetInt("isRemoveAd", 0) == 0 ? false : true;
		//isRemoveAd = false;
    }

    public bool CheckAndShowAd(System.Action onCompleteShowAd = null)
    {

        if (!Master.instance.isInternetConnected()) {
            if(onCompleteShowAd != null)
            {
                onCompleteShowAd();
            }
            return false;
        }

        if (!isRemoveAd)
        {
           // BannerAdController();
            if (UnityAdController(onCompleteShowAd))
            {
                return true;
            }
            else if (InterstinialAdController(onCompleteShowAd))
            {
                return true;
            }
            else
            {
                if (onCompleteShowAd != null)
                {
                    onCompleteShowAd();
                }
                return false;
            }
        }
        else
        {
            if (onCompleteShowAd != null)
            {
                onCompleteShowAd();
            }
            return false;
        }
    }

    public bool CheckShowRemoveAd(string type, System.Action onComplete = null)
    {
        if (isAutoShowRemoveAd)
        {
            if (type.Trim().ToLower() == "interad")
            {
                Master.Stats.timesShowInterAd++;
                if (showRemoveAdWhen == ShowRemoveAdWhen.AfterInterAd)
                {
                    return true;
                }
                else if (showRemoveAdWhen == ShowRemoveAdWhen.AfterTimesInterAd)
                {
                    if (Master.Stats.timesShowInterAd % timesShowInterAdToShowRemoveAd == 0)
                    {
                        return true;
                    }
                }
                else if (showRemoveAdWhen == ShowRemoveAdWhen.AfterInterAdOrUnityAd)
                {
                    return true;
                }
            }
            else if (type.Trim().ToLower() == "unityad")
            {
                Master.Stats.timesShowUnityAd++;
                if (showRemoveAdWhen == ShowRemoveAdWhen.AfterUnityAd)
                {
                    return true;
                }
                else if (showRemoveAdWhen == ShowRemoveAdWhen.AfterTimesUnityAd)
                {
                    if (Master.Stats.timesShowUnityAd % timesShowUnityToShowRemoveAd == 0)
                    {
                        return true;
                    }
                }
                else if (showRemoveAdWhen == ShowRemoveAdWhen.AfterInterAdOrUnityAd)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    #region Banner Ad Controller
    void BannerAdController()
    {
        Admob.HideBanner();
        if (showBannerWhen == ShowBannerAdWhen.AllTime)
        {
            ShowBanner();
        }
        else if (showBannerWhen == ShowBannerAdWhen.OnlyInGameplay)
        {
            if (Master.isGameStart)
            {
                ShowBanner();
            }
        }
        else if (showBannerWhen == ShowBannerAdWhen.OnlyInMenu)
        {
            if (Master.isMenu)
            {
                ShowBanner();
            }
        }
        else if (showBannerWhen == ShowBannerAdWhen.OnlyInMenuAndLevelComplete)
        {
            if (Master.isMenu || Master.isLevelComplete)
            {
                ShowBanner();
            }
        }
        else if (showBannerWhen == ShowBannerAdWhen.OnlyWhenLevelComplete)
        {
            if (Master.isLevelComplete)
            {
                ShowBanner();
            }
        }
    }
    private void ShowBanner()
    {
        if (isShowBannerAd)
        {
            if (positionBannerAd == PositionBannerAd.Top)
            {
                Admob.ShowTopBanner();
            }
            else if (positionBannerAd == PositionBannerAd.Bottom)
            {
                Admob.ShowTopBanner();
            }
            else if (positionBannerAd == PositionBannerAd.BothTopAndBottom)
            {
                Admob.ShowTopBanner();
                Admob.ShowBottomBanner();
            }
        }
        else
        {
            Admob.HideBanner();
        }
    }

    #endregion

    #region Interstinial Ad Controller

    bool InterstinialAdController(System.Action onCompleteShowAd = null)
    {
        if (isShowInterstitialAd)
        {
            if (showInterstinialAdWhen == ShowInterstinialAdWhen.WhenLevelComplete && Master.isLevelComplete)
            {
                if (Master.Stats.TimesLevelComplete % timesLevelCompleteToShowInterstinialAd == 0 && Master.Stats.TimesLevelComplete > 0)
                {
                    Admob.ShowInterAd(onCompleteShowAd);
                    return true;
                }
                return false;
            }
            else if (showInterstinialAdWhen == ShowInterstinialAdWhen.WhenStartLevel)
            {
                if (Master.Stats.TimesPlay % timesPlayToShowInterstinialAd == 0 && Master.Stats.TimesPlay > 0)
                {
                    Admob.ShowInterAd(onCompleteShowAd);
                    return true;
                }
                return false;
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    #endregion

    #region Unity Ad Controller

    bool UnityAdController(System.Action onCompleteShowAd = null)
    {
        if (isShowUnityAd)
        {
            if (!UnityAd.isReady())
            {
                return false;
            }

            if (showUnityAdWhen == ShowUnityAdWhen.WhenLevelComplete && Master.isLevelComplete)
            {
                if (Master.Stats.TimesLevelComplete % timesLevelCompleteToShowUnityAd == 0 && Master.Stats.TimesLevelComplete > 0)
                {
                    UnityAd.ShowAd(onCompleteShowAd);
                    return true;
                }
            }
            else if (showUnityAdWhen == ShowUnityAdWhen.WhenStartLevel)
            {
                if (Master.Stats.TimesPlay % timesPlayToShowUnityAd == 0 && Master.Stats.TimesPlay > 0)
                {
                    UnityAd.ShowAd(onCompleteShowAd);
                    return true;
                }
            }
        }
        else
        {
            return false;
        }
        return false;
    }
    #endregion

}