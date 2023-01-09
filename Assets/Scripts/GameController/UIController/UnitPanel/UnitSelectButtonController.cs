using UnityEngine;
using System.Collections;

public class UnitSelectButtonController : MonoBehaviour {

	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnClick()
    {
        Master.PlaySoundButtonClick();
        Master.UIMenu.panels[1].GetComponent<UnitPanelController>().UnitSelect_OnClick(gameObject);
    }
}
