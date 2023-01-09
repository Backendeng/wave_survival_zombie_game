using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class LevelCompleteDialog : DialogController
{

    // Use this for initialization
    private UITexture completeStatusTexture;
    private GameObject[] stars = new GameObject[3];
    private GameObject starReward;
    private UILabel starRewardLabel;
    private GameObject gemReward;
    private UILabel gemRewardLabel;
    private GameObject allButtons;
    private UIGrid buttonsGrid;
    private GameObject nextButton;
    private GameObject shareButtons;
    bool isVictory = true;
    bool isPlayedThisLevel = false;
    bool isCanClick = false;

    int star = 1;
    int gemRewardValue = 0;
    int starRewardValue = 0;


    public override void AssignObjects()
    {
        isAutoPlaySound = false;
        completeStatusTexture = Master.GetChildByName(gameObject, "CompleteStatus").GetComponent<UITexture>();
        for (int i = 0; i < 3; i++)
        {
            stars[i] = Master.GetChildByName(gameObject, "Star_" + (i + 1));
            stars[i].SetActive(false);
        }
        starReward = Master.GetChildByName(gameObject, "StarReward");
        starRewardLabel = Master.GetChildByName(gameObject, "StarRewardLabel").GetComponent<UILabel>();
        gemReward = Master.GetChildByName(gameObject, "GemReward");
        gemRewardLabel = Master.GetChildByName(gameObject, "GemRewardLabel").GetComponent<UILabel>();
        allButtons = Master.GetChildByName(gameObject, "AllButtons");

        buttonsGrid = Master.GetChildByName(gameObject, "Buttons").GetComponent<UIGrid>();
        nextButton = Master.GetChildByName(gameObject, "NextButton");
        shareButtons = Master.GetChildByName(gameObject, "ShareButtons");
    }

    public override void OnOpen(string[] agruments = null, Action onCloseComplete = null)
    {
        Master.Tutorial.CheckAndFinishTutorial();

        //stop game
        Time.timeScale = 0;
        starRewardLabel.text = "--";
        gemRewardLabel.text = "--";
        allButtons.transform.localPosition = new Vector3(allButtons.transform.localPosition.x, -500, 0);
        allButtons.SetActive(false);
        //check is victory

        if (Master.Gameplay.unitsDead >= Master.Level.currentLevelData.NumberOfUnitsAllowedDead || Master.Gameplay.zombiesEscaped > 0)
        {
            isVictory = false;
        }

        if (Master.LevelData.currentLevel <= Master.LevelData.lastLevel)
        {
            isPlayedThisLevel = true;
        }

        Master.Audio.StopBackgroundMusic();

        if (isVictory)
        {
            Master.Stats.TimesLevelComplete++;

            Master.Audio.PlaySound("snd_victory_2", 0.8f);

            //calculate star
            float percentUnitDead = ((float)Master.Gameplay.unitsDead / Master.Level.currentLevelData.NumberOfUnitsAllowedDead) * 100;
            if (percentUnitDead < 20)
            {
                star = 3;
            }
            else if (percentUnitDead >= 20 && percentUnitDead < 45)
            {
                star = 2;
            }

            //set Status icon
            completeStatusTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Dialog/LevelComplete/victory");

            //save level data
            gemRewardValue = RewardController.GetGemReward(Master.LevelData.currentLevel, star);
            starRewardValue = RewardController.GetStarReward(Master.LevelData.currentLevel, star);

            Master.LevelData.SetLastLevel(Master.LevelData.currentLevel);
            Master.LevelData.SetStarAtLevel(Master.LevelData.currentLevel, star);

        }
        else
        {
            Master.Audio.PlaySound("snd_defeat", 0.3f);

            star = 0;
            completeStatusTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Dialog/LevelComplete/defeat");
            starRewardLabel.text = "0";
            gemRewardLabel.text = "0";
            NGUITools.Destroy(nextButton);
            buttonsGrid.Reposition();
            shareButtons.SetActive(false);
        }

    }

    public override void OnShowComplete()
    {
        if (isVictory)
        {
            ShowStar(() =>
            {
                Master.WaitAndDo(0.7f, () =>
                {
                    Master.Audio.PlaySound("snd_getReward");
                    Master.Effect.CreateEffect("Effect_Star", gameObject.transform.localPosition);
                    starRewardLabel.text = starRewardValue.ToString();
                    gemRewardLabel.text = gemRewardValue.ToString();

                    Master.WaitAndDo(0.7f, () =>
                    {
                        if (isVictory)
                        {
                            if (!isPlayedThisLevel)
                            {
                                Master.UnitData.CheckUnitUnlock(() =>
                                {
                                    Master.SkillData.CheckSkillUnlock(() =>
                                    {
                                        Master.instance.CheckShowRatingDialog(() =>
                                        {
                                            ShowAllButtons();
                                        });
                                    });
                                });
                            }
                            else
                            {
                                Master.instance.CheckShowRatingDialog(() =>
                                {
                                    ShowAllButtons();
                                });
                            }
                        }
                        else
                        {
                            ShowAllButtons();
                        }
                    }, this, true);
                }, this, true);


            });
        }
        else
        {
            ShowAllButtons();
        }
    }

    void ShowStar(System.Action onComplete = null)
    {
        float timePerShowStar = 0.5f;
        if (star > 0)
        {
            Master.WaitAndDo(0.1f, () =>
            {
                doShowStar(0);
                if (star > 1)
                {
                    Master.WaitAndDo(timePerShowStar, () =>
                    {
                        doShowStar(1);
                        if (star > 2)
                        {
                            Master.WaitAndDo(timePerShowStar, () =>
                            {
                                doShowStar(2);
                                if (onComplete != null)
                                {
                                    onComplete();
                                }
                            }, this, true);
                        }
                        else
                        {
                            if (onComplete != null)
                            {
                                onComplete();
                            }
                        }
                    }, this, true);
                }
                else
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                }
            }, this, true);
        }
    }
    void ShowAllButtons()
    {
        //Master.WaitAndDo(0.2f, () =>
        // {
        PlayShowHideSound();
        Master.Ad.Admob.HideBanner();
        Master.Ad.Admob.ShowInterAd();
       
        allButtons.SetActive(true);
        allButtons.transform.DOLocalMoveY(-240, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            isCanClick = true;
            if (Master.Stats.Star >= 150 && !Master.Tutorial.IsTutorialDone(TutorialController.TutorialsIndex.UpgradeStatsOfUnit))
            {
                Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.ReturnToMenu);
            }
        });
        // }, this, true);
    }
    void doShowStar(int starIndex)
    {
        //Master.Effect.CreateEffect("Effect_Star", stars[starIndex].transform.localPosition);
        stars[starIndex].SetActive(true);
        stars[starIndex].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        stars[starIndex].transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.1f).SetUpdate(true).OnComplete(() =>
        {
            Master.Audio.PlaySound("snd_showStar", 0.5f);
            stars[starIndex].transform.DOScale(new Vector3(1, 1, 1), 0.07f).SetUpdate(true).OnComplete(() =>
            {

            });
        });

        Master.Effect.ShakeCamera(4);
    }

    public void MenuButton_OnClick()
    {
        if (!isCanClick) return;

        Master.PlaySoundButtonClick();
        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.ReturnToMenu);

        Close(() =>
        {
            Master.Gameplay.GoToMenu();
        });
    }

    public void NextButton_OnClick()
    {
        if (!isCanClick) return;

        Master.PlaySoundButtonClick();

        Close(() =>
        {
            Master.Gameplay.GoToNextLevel();
        });
    }

    public void ReplayButton_OnClick()
    {
        if (!isCanClick) return;

        Master.PlaySoundButtonClick();

        Close(() =>
        {
            Master.Gameplay.ReplayGame();
        });
    }

    public void ShareFacebookButton_OnClick()
    {
        if (!isCanClick) return;

        Master.PlaySoundButtonClick();
        Master.Social.Facebook.ShareLink("", "I have completed level " + Master.LevelData.currentLevel + " | Special Squad vs Zombies", "Let's play \"Special Squad vs Zombies\" and lead your Special Squad to defeat the Zombies!!!",
            Master.Social.Facebook.linkImageShareLevelComplete);
    }

    public void ShareTwitterButton_OnClick()
    {
        if (!isCanClick) return;

        Master.PlaySoundButtonClick();
        Master.Social.ShareTwitter(true);
    }


}
