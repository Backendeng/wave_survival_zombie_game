////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsExample : MonoBehaviour {

	public Texture2D bigPicture;

	private int LastNotificationId = 0;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		GoogleCloudMessageService.ActionCMDRegistrationResult += HandleActionCMDRegistrationResult;
		GoogleCloudMessageService.ActionCouldMessageLoaded += OnMessageLoaded;
		GoogleCloudMessageService.Instance.Init();
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	private void Toast() {
		AndroidToast.ShowToastNotification ("Hello Toast", AndroidToast.LENGTH_LONG);
	}

	private void Local() {
		//LastNotificationId = AndroidNotificationManager.instance.ScheduleLocalNotification("Hello", "This is local notification", 5);

		AndroidNotificationBuilder builder = new AndroidNotificationBuilder(SA_IdFactory.NextId,
		                                                                    "Local Notification Title",
		                                                                    "Big Picture Style Notification for AndroidNative Preview",
		                                                                    3);
		builder.SetBigPicture (bigPicture);
		AndroidNotificationManager.Instance.ScheduleLocalNotification(builder);
	}

	private void LoadLaunchNotification (){
		AndroidNotificationManager.instance.OnNotificationIdLoaded += OnNotificationIdLoaded;
		AndroidNotificationManager.instance.LocadAppLaunchNotificationId();
	}

	private void CanselLocal() {
		AndroidNotificationManager.instance.CancelLocalNotification(LastNotificationId);
	}

	private void CancelAll() {
		AndroidNotificationManager.instance.CancelAllLocalNotifications();
	}


	private void Reg() {
		GoogleCloudMessageService.instance.RgisterDevice();
	}

	private void LoadLastMessage() {
		GoogleCloudMessageService.instance.LoadLastMessage();
	}


	private void LocalNitificationsListExample() {
//		List<LocalNotificationTemplate> PendingNotofications;
	//	PendingNotofications = AndroidNotificationManager.instance.LoadPendingNotifications();
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	void HandleActionCMDRegistrationResult (GP_GCM_RegistrationResult res) {
		if(res.IsSucceeded) {
			AN_PoupsProxy.showMessage ("Regstred", "GCM REG ID: " + GoogleCloudMessageService.instance.registrationId);
		} else {
			AN_PoupsProxy.showMessage ("Reg Failed", "GCM Registration failed :(");
		}
	}



	private void OnNotificationIdLoaded (int notificationid){
		AN_PoupsProxy.showMessage ("Loaded", "App was laucnhed with notification id: " + notificationid);
	}

	
	private void OnMessageLoaded(string msg) {
		AN_PoupsProxy.showMessage ("Message Loaded", "Last GCM Message: " + GoogleCloudMessageService.instance.lastMessage);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
