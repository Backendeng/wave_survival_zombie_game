using UnityEngine;
using System.Collections;

public class PauseGameDialog : DialogController
{
    UILabel statusSoundLabel;
    UILabel statusMusicLabel;

    public override void AssignObjects()
    {
        statusSoundLabel = Master.GetChildByName(gameObject, "StatusSoundLabel").GetComponent<UILabel>();
        statusMusicLabel = Master.GetChildByName(gameObject, "StatusMusicLabel").GetComponent<UILabel>();
    }

    public override void OnStart()
    {
        SetSetting();
    }

    void SetSetting()
    {
        if (Master.Audio.isSoundOn)
        {
            statusSoundLabel.gameObject.SetActive(true);
        }
        else
        {
            statusSoundLabel.gameObject.SetActive(false);
        }

        if (Master.Audio.isBackgroundMusicOn)
        {
            statusMusicLabel.gameObject.SetActive(true);
        }
        else
        {
            statusMusicLabel.gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    public void ResumeButton_OnClick()
    {
        Master.PlaySoundButtonClick();

        Close(() =>
        {
            Master.Gameplay.ResumeGame();
        });
    }
    public void MenuButton_OnClick()
    {
        Master.PlaySoundButtonClick();

        Close(() =>
        {
            Master.Gameplay.GoToMenu();
        });
    }

    public void ReplayButton_OnClick()
    {
        Master.PlaySoundButtonClick();

        Close(() =>
        {
            Master.Gameplay.ReplayGame();
        });
    }

    public void ChangeStatusAudioButton_OnClick(GameObject go)
    {
        Master.PlaySoundButtonClick();

        if (go.name == "BGSoundButton")
        {
            Master.Audio.ToggleSound();
        }
        else if (go.name == "BGMusicButton")
        {
            Master.Audio.ToggleBackgroundMusic();
        }

        SetSetting();
    }

}
