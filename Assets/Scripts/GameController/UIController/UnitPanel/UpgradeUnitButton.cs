using UnityEngine;
using System.Collections;

public class UpgradeUnitButton : MonoBehaviour {

    // Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnClick()
    {
        Master.Audio.PlaySound("snd_buy",0.8f);
        Master.UIMenu.panels[1].GetComponent<UnitPanelController>().UpgradeButton_OnClick(gameObject);
    }
}
