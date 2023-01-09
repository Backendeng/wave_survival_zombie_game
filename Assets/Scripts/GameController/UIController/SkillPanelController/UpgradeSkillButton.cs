using UnityEngine;
using System.Collections;

public class UpgradeSkillButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        Master.Audio.PlaySound("snd_buy", 0.8f);
        Master.UIMenu.panels[2].GetComponent<SkillPanelController>().UpgardeSkillButton_OnClick(gameObject);
    }
}
