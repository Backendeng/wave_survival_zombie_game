using UnityEngine;
using System.Collections;

public class EnemyCard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        Master.PlaySoundButtonClick();
        FindObjectOfType<LibraryPanelController>().Card_OnClick(gameObject);
    }

}
