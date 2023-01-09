using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using System;

public class FacebookController : MonoBehaviour
{

    private System.Action onLoginComplete;
    private System.Action onLoginFail;

    private System.Action onShareLinkComplete;
    private System.Action onShareLinkFail;

    private System.Action onShareScreenshotComplete;
    private System.Action onShareScreenshotFail;

    private System.Action onAppInviteComplete;
    private System.Action onAppInviteFail;

    public string linkImageShare = "";
    public string linkImageShareLevelComplete = "";
    public string linkImageAppInvite = "";
    public string linkAppInvite = "https://fb.me/799383216863366";

    public string titleShareLink = "";
    public string descriptionShareLink = "";

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    void Start()
    {
        Master.Social.Facebook = this;
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login(System.Action onComplete = null, System.Action onFail = null)
    {
        onLoginComplete = onComplete;
        onLoginFail = onFail;
        var perms = new List<string>() { "public_profile", "email", "user_friends"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            Debug.Log("User login successfull!");

            if (onLoginComplete != null)
            {
                onLoginComplete();
                onLoginComplete = null;
            }
        }
        else
        {
            Debug.Log("User cancelled login");

            if (onLoginFail != null)
            {
                onLoginFail();
                onLoginFail = null;
            }
        }
    }

    //share screenshoot
    public void ShareScreenShot(string caption, System.Action onComplete = null, System.Action onFail = null)
    {
        onShareScreenshotComplete = onComplete;
        onShareScreenshotFail = onFail;
        StartCoroutine(TakeScreenshotAndShare(caption));
    }

    private IEnumerator TakeScreenshotAndShare(string caption)
    {
        yield return new WaitForEndOfFrame();

        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        byte[] screenshot = tex.EncodeToPNG();
        var wwwForm = new WWWForm();
        wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
        wwwForm.AddField("name", caption);
        Debug.Log("Finish Capture and upload screenshoot");
        //FB.API("me/photos", Facebook.Unity.HttpMethod.POST, ShareScreenShootCallback, wwwForm);


        if (FB.IsLoggedIn)
        {
            //AndroidNotificationManager.Instance.ShowToastNotification("Start Share ScreenShot", 3);
            FB.API("me/photos", Facebook.Unity.HttpMethod.POST, ShareScreenShootCallback, wwwForm);
        }
        else

        {
            Login(() =>
            {
                FB.API("me/photos", Facebook.Unity.HttpMethod.POST, ShareScreenShootCallback, wwwForm);
            });
        }
    }

    private void ShareScreenShootCallback(IGraphResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Share screenshot Error: " + result.Error);
            if (onShareScreenshotFail != null)
            {
                onShareScreenshotFail();
                onShareScreenshotFail = null;
            }
        }
        else
        {
            // Share succeeded without postID

            if (onShareScreenshotComplete != null)
            {
                onShareScreenshotComplete();
            }
            Debug.Log("Share screenshot success!");
        }
    }

    public void ShareLink(string link = "", string title = "", string description = "", string linkImage = "", System.Action onComplete = null, System.Action onFail = null)
    {
        onShareLinkComplete = onComplete;
        onShareLinkFail = onFail;

        if(link == "")
        {
            link = Master.instance.linkInMarket;
        }

        if (title == "")
        {
            title = titleShareLink;
        }

        if (linkImage == "")
        {
            linkImage = linkImageShare;
        }

        if(description == "")
        {
            description = descriptionShareLink;
        }

        FB.ShareLink(
            new Uri(link),
            title,
            description,
            new Uri(linkImage),
            callback: ShareLinkCallback
        );
    }


    private void ShareLinkCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
            if (onShareLinkFail != null)
            {
                onShareLinkFail();
            }
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            if (onShareLinkComplete != null)
            {
                onShareLinkComplete();
            }
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            if (onShareLinkComplete != null)
            {
                onShareLinkComplete();
            }
            Debug.Log("ShareLink success!");
        }
    }

    public void AppInvite(System.Action onComplete = null, System.Action onFail = null)
    {
        this.onAppInviteComplete = onComplete;
        this.onAppInviteFail = onFail;
        FB.Mobile.AppInvite(
            new Uri(linkAppInvite),
            new Uri(linkImageAppInvite),
            AppInviteCallback
            );
    }

    void AppInviteCallback(IAppInviteResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {

            if (onAppInviteFail != null)
            {
                onAppInviteFail();
                onAppInviteFail = null;
            }
        }
        else
        {
            if (onAppInviteComplete != null)
            {
                onAppInviteComplete();
                onAppInviteComplete = null;
            }
        }
    }

}
