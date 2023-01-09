using UnityEngine;
using System.Collections;

public class LevelSelectDialog : DialogController
{

    private UILabel levelTitleLabel;
    private int levelIndex;

    public override void AssignObjects()
    {

        levelTitleLabel = Master.GetChildByName(gameObject, "Title").GetComponent<UILabel>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        levelIndex = int.Parse(agruments[0]);
        levelTitleLabel.text = "Level " + levelIndex;
    }

    public void GoButton_OnClick()
    {
        Master.PlaySoundButtonClick();

        Master.UIMenu.CloseDialog(gameObject, 0.25f, () =>
         {
             Master.LevelData.currentLevel = levelIndex;
             Master.UIMenu.Transition(() =>
             {
                 //Master.Audio.StopBackgroundMusic();
                 Application.LoadLevel("Play");
             });
         });



        //Application.LoadLevel("Play");
    }

}
