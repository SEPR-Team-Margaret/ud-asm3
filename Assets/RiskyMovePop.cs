﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskyMovePop : MonoBehaviour {
	//Define 200x130 px window will apear in the center of the screen.
	private Rect windowRect = new Rect ((Screen.width - 200)/2, (Screen.height - 130)/2, 200, 130);
	//Define a bool that will show/hide the popup
	private bool show = false;			
	private int attu; 					//Define an integer that wild hold the number of attacking units
	private int defu; 					//Define an integer that wild hold the number of defending units
	public GameObject EventManager;		//Publicly define a gameobject that will manage general events (like conflict resolution)

	void OnGUI () 
	{
		if(show)																	//Whenever show is set to true make the window appear, when false hide it
			windowRect = GUI.Window (0, windowRect, DialogWindow, "Risky Attack");	//Set title of box
	}
	

	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label (new Rect (5, y, windowRect.width, 20), "You are outmanned " + attu +  " to " + defu); //Set first line of description
		GUI.Label (new Rect (5, y+20, windowRect.width,20), "do you want to continue?");				//Set second line of description

		if(GUI.Button(new Rect(5,y+55, windowRect.width - 10, 20), "Continue anyway"))					//Define one button that says "Continue anyway"
		{
			EventManager.BroadcastMessage ("ConfRes");													//Calls conflict resolution
			show = false;																				//And closes popup when clicked	
		}

		if(GUI.Button(new Rect(5,y+80, windowRect.width - 10, 20), "Select new move"))					//Define one button that says "Select new move"
		{																								//And closes popup when clicked	
			show = false;
		}
	}
	
	// To open the dialogue from outside of the script.
	public void Open()
	{ 
		show = true;
	} 

	public void Setattu(int AU)
	{ 
		attu = AU;
	} 
	public void Setdefu(int DU)
	{ 
		defu = DU;
	}
}
