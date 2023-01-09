using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;

public class AntiCheatController : MonoBehaviour {

	// Use this for initialization
	private void Start () {
        ObscuredCheatingDetector.StartDetection(OnCheatDetected);
	}
    
	
	private void OnCheatDetected()
    {
        Application.Quit();
    }
}
