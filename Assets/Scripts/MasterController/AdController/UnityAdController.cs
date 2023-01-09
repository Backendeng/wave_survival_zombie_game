using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class UnityAdController : MonoBehaviour
{
    // Use this for initialization
    private System.Action onComplete;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //if (Master.Ad.UnityAd != null)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    Master.Ad.UnityAd = this;
        //}
    }

    void Start()
    {
        if (Master.Ad.isShowUnityAd)
        {
#if UNITY_ANDROID
            Advertisement.Initialize(Master.Ad.androidGameID, false);
#endif

#if UNITY_IOS
            Advertisement.Initialize(GameMaster.Ad.iOsGameID, false);
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowAd(System.Action onComplete = null)
    {

        if (onComplete != null)
        {
            this.onComplete = onComplete;
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = AdCallbackhandler;
        
        if (Advertisement.IsReady(Master.Ad.zone))
        {
            Debug.Log("Show Unity Ad");
            Advertisement.Show(Master.Ad.zone, options);
        }
        else
        {
            if (this.onComplete != null)
            {
                onComplete();
                onComplete = null;
            }
        }
    }

    void AdCallbackhandler(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                EventAd();
                break;
            case ShowResult.Skipped:
                EventAd();
                break;
            case ShowResult.Failed:
                EventAd();
                break;
        }
    }

    void EventAd()
    {
        if (this.onComplete != null)
        {
            onComplete();
            onComplete = null;
        }
        Master.Ad.CheckShowRemoveAd("unityad");
    }

    public bool isReady()
    {
        return Advertisement.IsReady(Master.Ad.zone);
    }
}
