using UnityEngine;
using System.Collections;
using System;

public class FreeRewardDialog : DialogController
{
    UIButton getRandomRewardButton;
    UILabel getRandomRewardStatusLabel;

    UIButton shareFacebookButton;
    private float timePerGetRandomReward;
    GameObject randomRewardRoot;
    GameObject shareFacebookRewardRoot;
    UILabel randomRewardGemLabel;
    UILabel randomRewardStarLabel;

    UILabel shareFacebookRewardGemLabel;
    UILabel shareFacebookRewardStarLabel;

    GameObject valuesRandomReward;
    GameObject valuesShareFacebookReward;

    UILabel titleRandomReward;
    UILabel titleShareFacbookReward;

    int[] currentRandomReward = new int[2];
    int[] currentShareFacebookReward = new int[2];

    public override void AssignObjects()
    {
        //PlayerPrefs.DeleteAll();

        getRandomRewardButton = Master.GetChildByName(gameObject, "Button_GetRandomReward").GetComponent<UIButton>();
        getRandomRewardStatusLabel = Master.GetChildByName(gameObject, "ButtonGetStatusLabel").GetComponent<UILabel>();
        shareFacebookButton = Master.GetChildByName(gameObject, "Button_ShareFacebook").GetComponent<UIButton>();
        randomRewardRoot = Master.GetChildByName(gameObject, "RandomReward");
        shareFacebookRewardRoot = Master.GetChildByName(gameObject, "ShareFacebookReward");
        randomRewardGemLabel = Master.GetChildByName(randomRewardRoot, "GemValueLabel").GetComponent<UILabel>();
        randomRewardStarLabel = Master.GetChildByName(randomRewardRoot, "StarValueLabel").GetComponent<UILabel>();

        shareFacebookRewardGemLabel = Master.GetChildByName(shareFacebookRewardRoot, "GemValueLabel").GetComponent<UILabel>();
        shareFacebookRewardStarLabel = Master.GetChildByName(shareFacebookRewardRoot, "StarValueLabel").GetComponent<UILabel>();

        valuesRandomReward = Master.GetChildByName(randomRewardRoot, "Values");
        valuesShareFacebookReward = Master.GetChildByName(shareFacebookRewardRoot, "Values");

        titleRandomReward = Master.GetChildByName(randomRewardRoot, "Title").GetComponent<UILabel>();
        titleShareFacbookReward = Master.GetChildByName(shareFacebookRewardRoot, "Title").GetComponent<UILabel>();

    }

    public override void OnStart()
    {
        SetRandomReward();
        SetShareFacebookReward();
        InvokeRepeating("CheckTime", 0, 1);
    }

    void SetRandomReward()
    {
        currentRandomReward = FreeRewardController.GetReward(0);
        randomRewardGemLabel.text = currentRandomReward[0].ToString();
        randomRewardStarLabel.text = currentRandomReward[1].ToString();
    }

    void SetShareFacebookReward()
    {
        if (!FreeRewardController.IsSharedFacebook())
        {
            currentShareFacebookReward = FreeRewardController.GetReward(1);
            shareFacebookRewardGemLabel.text = currentShareFacebookReward[0].ToString();
            shareFacebookRewardStarLabel.text = currentShareFacebookReward[1].ToString();
        }
    }

    void CheckTime()
    {
        //for time get reward
        valuesRandomReward.SetActive(true);
        titleRandomReward.gameObject.SetActive(false);
        if (FreeRewardController.TimeRemainingFreeReward() <= 0)
        {
            
            getRandomRewardButton.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            getRandomRewardButton.SetState(UIButtonColor.State.Normal, true);
            getRandomRewardStatusLabel.text = "Get";
        }
        else
        {
           // valuesRandomReward.SetActive(false);
            //titleRandomReward.gameObject.SetActive(true);

            getRandomRewardButton.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            getRandomRewardButton.SetState(UIButtonColor.State.Disabled, true);

            int secondRemaining = FreeRewardController.TimeRemainingFreeReward();
            int minute = secondRemaining / 60;
            int second = secondRemaining % 60;
            getRandomRewardStatusLabel.text = ((minute < 10) ? "0" + minute.ToString() : minute.ToString()) + ":" + ((second < 10) ? "0" + second.ToString() : second.ToString());
        }

        //for shared facebook
        if (FreeRewardController.IsSharedFacebook())
        {
            valuesShareFacebookReward.SetActive(false);
            titleShareFacbookReward.gameObject.SetActive(true);
            shareFacebookButton.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            shareFacebookButton.SetState(UIButtonColor.State.Disabled, true);
        }
        else
        {
            valuesShareFacebookReward.SetActive(true);
            titleShareFacbookReward.gameObject.SetActive(false);
            shareFacebookButton.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            getRandomRewardButton.SetState(UIButtonColor.State.Normal, true);
        }
    }

    public void GetRewardButton_OnClick()
    {
        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.GetFreeReward);

        Master.PlaySoundButtonClick();
        Close(() =>
        {
            Master.Ad.UnityAd.ShowAd(() =>
            {
                Master.Stats.Gem += currentRandomReward[0];
                Master.Stats.Star += currentRandomReward[1];
                FreeRewardController.SetDateTimeGetReward();
                Master.QuestData.IncreaseProgressValue("07");
                Master.PushNotification.SetGetFreeRewardNotification();
                // SetRandomReward();
                Master.UIMenu.ShowDialog("GotRewardDialog", 0.3f, new string[] { currentRandomReward[0].ToString(), currentRandomReward[1].ToString() });
            });
        });

    }

    public void GetFacebookShareRewardButton_OnClick()
    {
        Master.PlaySoundButtonClick();
        Close(() =>
        {
            Master.Social.Facebook.ShareLink("", "", "", "", () =>
            {
                Master.Stats.Gem += currentShareFacebookReward[0];
                Master.Stats.Star += currentShareFacebookReward[1];
                Master.QuestData.IncreaseProgressValue("07");
                FreeRewardController.SetDatTimeGetShareFacebookReward();
                Master.UIMenu.ShowDialog("GotRewardDialog", 0.3f, new string[] { currentShareFacebookReward[0].ToString(), currentShareFacebookReward[1].ToString() });
            });
        });
    }


}
