using UnityEngine;
using System.Collections;

public class FillEnergyDialog : DialogController
{

   // private int gemPerEnergy = 10;
    private int gemRequire = 0;
    private int energyNeedToFill;
    UILabel gemLabel;
    string[] agruments;

    public override void AssignObjects()
    {
        gemLabel = Master.GetChildByName(gameObject, "GemValue").GetComponent<UILabel>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        this.agruments = agruments;
        energyNeedToFill = Master.Stats.MaxEnergy - Master.Stats.Energy;
        gemRequire = energyNeedToFill * Master.Stats.GemPerEnergy;
        gemLabel.text = gemRequire.ToString();
    }

    public void UseGemButton_OnClick()
    {
        if (gemRequire <= Master.Stats.Gem)
        {
            Master.Audio.PlaySound("snd_buy");
            Master.Stats.Energy = Master.Stats.MaxEnergy;
            Master.Stats.Gem -= gemRequire;
            Close(() =>
            {
                if (agruments != null)
                {
                    Debug.Log(agruments[0]);
                    if (agruments[0] == "ReplayScene")
                    {
                        Master.UI.Transition(() =>
                        {
                            Time.timeScale = 1;
                            Application.LoadLevel(Application.loadedLevel);
                        });
                    }
                    else if (agruments[0] == "GoToLevel")
                    {
                        Master.UI.Transition(() =>
                        {
                            Time.timeScale = 1;
                            Master.LevelData.currentLevel = int.Parse(agruments[1]);
                            Application.LoadLevel("Play");
                        });
                    }
                }
            });
        }
        else
        {
            Master.PlaySoundButtonClick();
            Close(() =>
            {
                if (Master.UIMenu != null)
                {
                    Master.UIMenu.OpenPanel(4);
                }
                else
                {
                    Master.defaultPanelMenu = 4;
                    Master.Gameplay.GoToMenu();
                }
            });

        }
    }

    public void InviteFriendButton_OnClick()
    {
        Master.Social.Facebook.AppInvite(() =>
        {
            Master.Audio.PlaySound("snd_buy");
            if (Master.Stats.Energy < Master.Stats.MaxEnergy)
            {
                Master.Stats.Energy++;
            }
            Close();
        });
    }

    public void CloseButton_OnClick()
    {
        Close(() =>
        {
            if (Master.Gameplay != null)
            {
                Master.Gameplay.GoToMenu();
            }
        });
    }
}
