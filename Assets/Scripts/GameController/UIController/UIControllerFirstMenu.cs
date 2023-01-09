using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIControllerFirstMenu : UIController
{
    UITexture soundTexture;
    UITexture musicTexture;

    public override void OnAwake()
    {
        if (Master.UI == null)
        {
            Master.UI = this;
        }
    }

    // Use this for initialization
    public override void AssignObject()
    {
        soundTexture = Master.GetChildByName(uiRoot, "Sound").GetComponent<UITexture>();
        musicTexture = Master.GetChildByName(uiRoot, "Music").GetComponent<UITexture>();
    }


    public override void OnStart()
    {
        Master.Audio.PlayBackgroundMusic("bg_menu", 1, false);
        SetSettings();
    }


    public void Button_OnClick(GameObject go)
    {
        Master.PlaySoundButtonClick();
        switch (go.name)
        {
            case "Button_Play":
                Transition(() =>
                {
                    SceneManager.LoadScene("Menu");
                });
                break;
            case "Button_Rate":
                if (Master.instance.platform == Master.Platform.Android)
                {
                    Application.OpenURL(Master.instance.linkGooglePlay);
                }
                else
                {
                    Application.OpenURL(Master.instance.linkAppStore);
                }
                break;
            case "Button_More":
                if (Master.instance.platform == Master.Platform.Android)
                {
                    Application.OpenURL(Master.instance.linkGooglePlayDeveloperStore);
                }
                else
                {
                    Application.OpenURL(Master.instance.linkAppsStoreDeveloperStore);
                }
                break;

            case "Button_Exit":
                ShowDialog(UIController.Dialog.ListDialogs.ExitGameDialog);
                break;
        }

    }


    public void SetSettings()
    {
        if (Master.Audio.isSoundOn)
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(true);
        }

        if (Master.Audio.isBackgroundMusicOn)
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(true);
        }

    }

    public void ToggleAudioSettingButton_OnClick(GameObject go)
    {
        Master.PlaySoundButtonClick();
        string goName = go.name;
        if (goName == "Sound")
        {
            Master.Audio.ToggleSound();
        }
        else if (goName == "Music")
        {
            Master.Audio.ToggleBackgroundMusic();
        }

        SetSettings();
    }

}
