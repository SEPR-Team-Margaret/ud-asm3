using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour {
	private GameObject SettingPop; 				//Define a gameobject that will be popped up

	// Use this for initialization
	void Start () {
		SettingPop = GameObject.Find("Popup");   //Find the gameobject that holds all the objects that will appear in the settings popup
		SettingPop.SetActive (false);			//Hide this object

	}
	
	void SettingsClick(){ 					//When settings is clicked
		SettingPop.SetActive (true);		//Show settings popup
	} 

	void CloseClick(){ 						//When close is clicked
		SettingPop.SetActive (false);		//hide settings popup
	}
}
