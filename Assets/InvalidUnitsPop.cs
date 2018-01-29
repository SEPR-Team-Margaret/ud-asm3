using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidUnitsPop : MonoBehaviour
{
	//Define a 200x105 px window that will apear in the center of the screen.
	private Rect windowRect = new Rect ((Screen.width - 200)/2, (Screen.height - 105)/2, 200, 125);
	//Define a bool that will show/hide the popup
	private bool show = false;

	void OnGUI () 
	{
		if(show)			//Whenever show is set to true make the window appear, when false hide it
			windowRect = GUI.Window (0, windowRect, DialogWindow, "Invalid Number of Units");	//Set title of box
	}


	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label (new Rect (5, y, windowRect.width, 20), "You must move at least one unit,"); 	//Set first line of description
		GUI.Label (new Rect (5, y+20, windowRect.width,20), "and leave at least one unit");		//Set second line of desription
		GUI.Label (new Rect (5, y+40, windowRect.width,20), "on attacking sector");				//Set third line of desription 

		if(GUI.Button(new Rect(5,y+75, windowRect.width - 10, 20), "Select New Number"))		//Define one button that says "Select New Number"
		{																						//And closes popup when clicked	
			show = false;
		}
			
	}
		
	public void Open()
	{					//when open is called
		show = true;	//show the popup
	}
}