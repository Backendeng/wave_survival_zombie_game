using UnityEngine;
using System.Collections;
using System;

public class RatingDialog : DialogController
{
    public override void OnAwake()
    {
        isAutoPlaySound = false;
    }

    public override void OnOpen(string[] agruments = null, Action onCloseComplete = null)
    {
        this.onCloseComplete = onCloseComplete;
        Master.Audio.PlaySound("snd_specialDialog");

    }

    public void YesButton_OnClick()
    {
        Master.Stats.isClickedRating = true;
        PlayerPrefs.SetInt("isClickedRating", 1);
        PlayerPrefs.Save();

        Close();
    }

}
