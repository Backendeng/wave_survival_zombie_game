using UnityEngine;
using System.Collections;

public class NewUnitUnlockDialog : DialogController
{
    private UITexture unitTexture;
    public override void AssignObjects()
    {
        isAutoPlaySound = false;
        unitTexture = Master.GetChildByName(gameObject, "Unit").GetComponent<UITexture>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        //base.OnOpen();
        this.onCloseComplete = onCloseComplete;
        Debug.Log("UnitUnlock "+ agruments[0]);
        Master.Audio.PlaySound("snd_unlockUnit");

        string unitID = agruments[0];
        unitTexture.mainTexture = Resources.Load<Texture2D>("Textures/Characters/Units/Unit_" + unitID.Split('-')[0] + "/Card");
    }

}
	
