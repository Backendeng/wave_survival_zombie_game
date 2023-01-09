using UnityEngine;
using System.Collections;

public class ExitGameDialog : DialogController
{
    public override void OnStart()
    {
        Master.PushNotification.SetAllNotification();
    }

    public void Button_Onclick(GameObject go)
    {
        Master.PlaySoundButtonClick();
        switch (go.name)
        {
            case "Button_Yes":
                Master.WaitAndDo(1, () =>
                {
                    Application.Quit();
                });
                break;
            default:
                Close();
                break;
        }
    }
}
