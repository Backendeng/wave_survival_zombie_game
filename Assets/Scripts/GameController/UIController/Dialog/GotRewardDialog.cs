using UnityEngine;
using System.Collections;

public class GotRewardDialog : DialogController
{
    private UILabel gemValueLabel;
    private UILabel starValueLabel;

    public override void AssignObjects()
    {
        isAutoPlaySound = false;
        gemValueLabel = Master.GetChildByName(gameObject, "GemValueLabel").GetComponent<UILabel>();
        starValueLabel = Master.GetChildByName(gameObject, "StarValueLabel").GetComponent<UILabel>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        Master.Audio.PlaySound("snd_getReward");
        Master.Effect.CreateEffect("Effect_Star", gameObject.transform.localPosition);
        gemValueLabel.text = agruments[0];
        starValueLabel.text = agruments[1];
    }


}
