using UnityEngine;
using System.Collections;

public class NewSkillUnlockDialog : DialogController
{
    private UITexture skillTexture;

    public override void AssignObjects()
    {
        isAutoPlaySound = false;
        skillTexture = Master.GetChildByName(gameObject, "Skill").GetComponent<UITexture>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        //base.OnOpen();
        this.onCloseComplete = onCloseComplete;
        Debug.Log("SkillUnlock " + agruments[0]);
        Master.Audio.PlaySound("snd_unlockUnit");

        string skillID = agruments[0].Split('-')[0];
        skillTexture.mainTexture = Resources.Load<Texture2D>("Textures/Skills/Skill_" + skillID + "/Skill_" + skillID + "_Card");
    }

}

