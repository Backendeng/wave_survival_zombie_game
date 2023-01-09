using UnityEngine;
using System.Collections;

public class AdmobController : MonoBehaviour
{
    // Use this for initialization
    int topBannerID = 0;
    int bottomBannerID = 0;

    private GoogleMobileAdBanner topBanner;
    private GoogleMobileAdBanner bottomBanner;

    private System.Action onCompleteInterAd;

    public GameObject pf_loadingInterAdPanel;
    private GameObject loadingInterAdPanel;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //if (Master.Ad.Admob != null)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    Master.Ad.Admob = this;
        //}

        if (!UM_AdManager.IsInited)
        {
            UM_AdManager.Init();
        }
    }

    void Start()
    {
        SetIDAd();
        CreateBanner();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetIDAd()
    {
        //banner
        GoogleMobileAdSettings.Instance.Android_BannersUnitId = Master.Ad.androidBannerAdID;
        GoogleMobileAdSettings.Instance.IOS_BannersUnitId = Master.Ad.iOsBannerAdID;
        GoogleMobileAdSettings.Instance.WP8_BannersUnitId = Master.Ad.windowPhoneBannerAdID;

        //interAd
        GoogleMobileAdSettings.Instance.Android_InterstisialsUnitId = Master.Ad.androidInterAdID;
        GoogleMobileAdSettings.Instance.IOS_InterstisialsUnitId = Master.Ad.iOsInterAdID;
        GoogleMobileAdSettings.Instance.WP8_InterstisialsUnitId = Master.Ad.windowPhoneInterAdID;
    }

    void CreateBanner()
    {
        topBanner = GoogleMobileAd.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
        if (topBanner != null)
            topBanner.ShowOnLoad = false;

        bottomBanner = GoogleMobileAd.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
        if (bottomBanner != null)
            bottomBanner.ShowOnLoad = false;
    }

    void InitInterAd()
    {
        //UM_AdManager.ResetActions();
        ShowInterAdLoadingPanel(true);
        UM_AdManager.StartInterstitialAd();
        UM_AdManager.OnInterstitialLoaded += HandleOnInterstitialLoaded;
        UM_AdManager.OnInterstitialLoadFail += HandleOnInterstitialLoadFail;
        UM_AdManager.OnInterstitialClosed += HandleOnInterstitialClosed;
        UM_AdManager.LoadInterstitialAd();
    }

    void ShowInterAdLoadingPanel(bool isShow)
    {
        if (isShow)
        {
            loadingInterAdPanel = (GameObject)Instantiate(pf_loadingInterAdPanel, Vector3.zero, Quaternion.identity);
        }
        else
        {
            if (loadingInterAdPanel != null)
                Destroy(loadingInterAdPanel);
        }
    }

    void HandleOnInterstitialClosed()
    {
        EventInterAd();
        UM_AdManager.OnInterstitialClosed -= HandleOnInterstitialClosed;
    }

    void HandleOnInterstitialLoadFail()
    {
        EventInterAd();
        UM_AdManager.OnInterstitialLoadFail -= HandleOnInterstitialLoadFail;

    }

    void HandleOnInterstitialLoaded()
    {
        UM_AdManager.ShowInterstitialAd();
        UM_AdManager.OnInterstitialLoaded -= HandleOnInterstitialLoaded;
    }

    void EventInterAd()
    {
        ShowInterAdLoadingPanel(false);
        if (onCompleteInterAd != null)
        {
            onCompleteInterAd();
            onCompleteInterAd = null;
        }
        Master.Ad.CheckShowRemoveAd("interad");
    }

    public void ShowTopBanner()
    {
        if (topBanner != null)
        {
            Debug.Log("Show Top Banner");
            topBanner.Show();
        }
        //if (topBannerID == 0)
        //{
        //    topBannerID = UM_AdManager.CreateAdBanner(TextAnchor.UpperCenter);
        //}

        //UM_AdManager.ShowBanner(topBannerID);


    }

    public void ShowBottomBanner()
    {

        if (bottomBanner != null)
        {
            Debug.Log("Show Bottom Banner");
            bottomBanner.Show();
        }

        //if (bottomBannerID == 0)
        //{
        //    bottomBannerID = UM_AdManager.CreateAdBanner(TextAnchor.LowerCenter);
        //}

        //UM_AdManager.ShowBanner(bottomBannerID);

    }

    public void HideBanner()
    {
        //// GoogleMobileAd.GetBanner(0).
        //if (topBannerID != 0)
        //{
        //    UM_AdManager.HideBanner(topBannerID);
        //}

        //if (bottomBannerID != 0)
        //{
        //    UM_AdManager.HideBanner(bottomBannerID);
        //}

        Debug.Log("Hide Banner");
        if (topBanner != null)
        {
            topBanner.Hide();
        }
        if (bottomBanner != null)
        {
            bottomBanner.Hide();
        }
    }

    public void ShowInterAd(System.Action onCompleteInterAd = null)
    {
        if (onCompleteInterAd != null)
        {
            this.onCompleteInterAd = onCompleteInterAd;
        }

        Debug.Log("Show Inter Ad");

#if UNITY_EDITOR
        if (onCompleteInterAd != null)
        {
            onCompleteInterAd();
        }
        return;
#endif
        //UM_AdManager.StartInterstitialAd();
        InitInterAd();
    }


}
