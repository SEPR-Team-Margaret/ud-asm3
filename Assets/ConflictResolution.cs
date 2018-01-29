using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using System;   


public class ConflictResolution : MonoBehaviour {
	
	private GameObject IF; private GameObject SB; private GameObject UD;  //Define gameobjects for the input field, submit button and undo button
	public Text CmbDsc;  								//Publicly define a text component used for combat description
	public Text GmInstrctns;							//Publicly define a text component used for game instructions 
	private Text IFT;									//Publicly define the text component of the input field
	private int mode;  									//define a integer used for controlling the current mode conflict resolution is in 
														//this is used for differentiating if user is picking a sector to move units to or from
	private GameObject AttSec; 							//Define a variable that will hold the game object of the attacking sector
	private GameObject DefSec; 							//Define a variable that will hold the game object of the defending sector
	private GameObject[] AttOps; 						//Define a variable that will hold the neighbouring sectors of the attacking sector
	private int AttOwn; 								//Define a variable that will hold the owner of the attacking sector 								
	private int defOwn;									//Define a variable that will hold the owner of the defending sector
	private int attUnits;  			 					//Define a variable that will hold the number of units being used to attack
	private int defUnits; 								//Define a variable that will hold the number of units on the defending sector
	private int orgUnits; 								//Define a variable that will hold the original number of units on the attacking sector
	private GameObject Sectors;							//Define a variable that will hold the gameobject of every sector on the map
	public GameObject InvUnitNumGO;						//Publicly define a variable that is attached to the script generating a popup for if an invalid number of units is selected
	public GameObject RiskyMoveGO;						//Publicly define a variable that is attached to the script generating a popup for if a user selects an attack with low chance of success

	// Use this for initialization
	void Start () { 
		Sectors = GameObject.FindGameObjectWithTag ("Sector"); 	//Find all gameobjects of sectors
		IF = GameObject.Find("UnitField");  					//Find the gameobject of the input field
		SB = GameObject.Find("Submit");   						//Find the gameobject of the submit button
		UD = GameObject.Find ("Undo");							//Find the gameobject of the submit button
		IFT = IF.GetComponentInChildren<UnityEngine.UI.Text> ();//Find the text compontnt of the input field
		mode = 1;  												//Set the mode to 1...where the attacking sector is picked (where units are moved from)
		IF.SetActive(false); 									//Disable (and hide) the input field
		SB.SetActive(false); 									//submit button 
		UD.SetActive (false);									//and undo button
		GmInstrctns.text =  "Pick a sector to attack with..."; 	//Set the game instructions to tell user next action they should take
	}
	
	void SetUnits(int U){ 
		if (mode == 1) { 										//If this is the first sector that has been picked
			orgUnits = U;										//set original units to the number passed (number of units on sector clicked)
			GmInstrctns.text =  "How many units? Max: " + (U-1).ToString();	//Set the game instructions to tell user next action they should take
		}
		if (mode == 3) { 								//If this is the second sector to be picked 
			defUnits = U;								//set defending units to the number passed (number of units on sector clicked)
		}
	} 

	void SetOwn(int Owner){ 
		if (mode == 1) { 								//If this is the first sector that has been picked
			AttOwn = Owner;								//Set the attacking owner to the owner of the sector picked
		}
		if (mode == 3) { 								//If this is the second sector that has been picked
			defOwn = Owner;								//Set the defending owner to the owner of the sector picked
		}
	}

	void SetAttOps(GameObject[] Ops){ 
		if (mode == 1) {								//If this is the first sector that has been picked
			AttOps = Ops;								//set attacking options to all the neighbouring sectors of the clicked one
		}
	}

	void SelectSector(GameObject Sector){ 
		switch (mode) {
		case 1:												//If this is the first sector that has been picked
			AttSec = Sector; 								//store the gameobject in attacking sector
			mode = 2; 										//update the mode to 2...where users should enter number of units they intend to move/attack with
			Sectors.BroadcastMessage ("setFlash", false);  	//disable flashing of all sectors
			IF.SetActive (true); 							//Enable (and show) the input field
			SB.SetActive (true);  							//submit button 
			UD.SetActive (true);							//and undo button
			break;
		case 3:   											//If this is the second sector that has been picked
			bool ValidOp = false;							//Check if the second sector clicked is neighbouring the first sector clicked
			foreach (GameObject G in AttOps) {
				if (Sector == G) { 						
					ValidOp = true;
				}
			}
			if (ValidOp) { 								//If it is 
				DefSec = Sector;						//update defending sector to the second sector clicked
				if (defOwn != AttOwn) {					//check if it is a friendly sector
					if (attUnits < (defUnits * 0.66)) {  			//check if attacking units are less than 2/3 of the defending (unfriendly) units and if they are
						RiskyMoveGO.BroadcastMessage ("Setattu", attUnits);	//send details to a popup checking this is the move users meant to make
						RiskyMoveGO.BroadcastMessage ("Setdefu", defUnits);
						RiskyMoveGO.BroadcastMessage ("Open");	//and open the popup
					} else {  								//If the attacking units are more than 2/3 of the defending units 
						ConfRes (); 							//call the function to resolve the conflict
					} 
				}else {  								//If the attacking units are less than 2/3 of the defending units 
					ConfRes (); 							//but its a friendly sector, move anyway
				} 
			} else { 									//If the second sector is not a neighbour of the first
				AttSec.BroadcastMessage("startFlash"); 	//make the neighbouring sectors of the first sector flash, to remind the users where they can move units to
			}
			break;
		default: 
			break;
		}
	}

	void UndoPress(){ 
		switch(mode){
		case 2:												//When undo is clicked in mode 2
			mode = 1; 										//set mode to 1
			Sectors.BroadcastMessage ("setFlash", true);	//enable the flashing of sectors again
			IF.SetActive(false); 									//Disable (and hide) the input field
			SB.SetActive(false); 									//submit button 
			UD.SetActive (false);									//and undo button
			GmInstrctns.text =  "Pick a sector to attack with..."; 	//Set the game instructions to tell user next action they should take   
			break;
		case 3: 											//When undo is clicked in mode 3
			mode = 2; 										//set mode to 2
			Sectors.BroadcastMessage ("setFlash", false);  	//disable flashing of sectors
			IF.SetActive (true); 							//Enable (and show) the input field
			SB.SetActive (true);  							//submit button 
			UD.SetActive (true);							//and undo button
			GmInstrctns.text =  "How many units? Max: " + (orgUnits-1).ToString();   
			Renderer MyRenderer = AttSec.GetComponent<SpriteRenderer> (); //gets its renderer, 
			Color color; 													//and colour.
			color = MyRenderer.material.color; 
			color.a = 0.5f; 								//ensure that the attacking sector is semi-transparent
			MyRenderer.material.color = color; 
			break;
		}
	}
		
	void SubmitPress(){ 									//When submit is pressed
		attUnits = Convert.ToInt32(IFT.text);  				//set the number of attacking units to the text the user has just entered and submitted
		if ((attUnits < orgUnits)&(attUnits > 0)) {  		//Check that the number of units the user is attacking with is positive but less than the number of units on the sector
			mode = 3;										//move the mode to 3...where users pick the sector they wish to attack/move units to 
			AttSec.BroadcastMessage ("startFlash");			//make the neighbouring sectors of the first sector flash...so user knows where they cna move units to
			Renderer MyRenderer = AttSec.GetComponent<SpriteRenderer> (); 
			Color color; 									//Change transparency of attacking sector so that it is fully opaque
			color = MyRenderer.material.color; 				//so that user knows what sector they are moving units from
			color.a = 1; 
			MyRenderer.material.color = color; 	 
			IF.SetActive(false); 							//Disable (and hide) the input field
			SB.SetActive(false); 							//and the submit button 
			GmInstrctns.text = "Pick a sector to move units to..."; //Update instructions to show user next step
		} else {  
			InvUnitNumGO.BroadcastMessage ("Open");			//If the user has selected an invalid number bring up a popup to give them information
		}
	}  

	void ConfRes(){ 										//When it is time for the conflict to be resolved
		UD.SetActive (false);								//Disable (and hide) the undo button
		System.Random R = new System.Random ();	 			
		String CombDesc;
		AttSec.BroadcastMessage ("TakeUnits", attUnits);	//Subtract the units attacking from the original number of units on the attacking sector
		if (defOwn != AttOwn) {								//If the owners of the two sectors are different the resolution is needed 
										//rather than just subtracting the units from one and adding to the other
			CombDesc = "ATT   DEF\n " + attUnits.ToString ().PadRight(2) + " vs " + defUnits.ToString (); //Update combat description to show initial state of conflict
			while ((attUnits > 0) && (defUnits > 0)) {   	//Keep iterating until either the attacking units or defending units are all gone
				int ARI = R.Next (0,	attUnits+1); 		//Pick a random number between 0 and the current number of attacking units (call this attacking random integer)
				int DRI = R.Next (0,	defUnits+1); 		//Pick a random number between 0 and the current number of defending units (call this defending random integer)
				CombDesc += "\n -" + DRI.ToString().PadRight(2) + " || -" + ARI.ToString(); //Update the combat description to show the random numbers selected
				attUnits = attUnits - DRI; 					//take the defending random integer from the attacking units
				defUnits = defUnits - ARI;  				//take the attacking random integer from the defending units
				CombDesc += "\n  " + Math.Max(attUnits,0).ToString ().PadRight(2) + " vs " + Math.Max(defUnits,0).ToString (); //Update combat description to show remaining units
			} 												//When conflict is resolved
			DefSec.BroadcastMessage ("SetUnits", defUnits); //Update units on defending sector to the remaining number of defending units
			if (attUnits > 0) { 							//If there are any attacking units left
				DefSec.BroadcastMessage ("SetUnits", attUnits); 	//Update units on defending sector to the remaining number of attacking units
				DefSec.BroadcastMessage ("SetOwner", AttOwn);		//Update owner on defending sector to the owner of the attacking sector
			}
			CmbDsc.text = CombDesc;							//Update the actual text box to show the combat description
		} else { 									//If the owners of the two sectors are the same 
			DefSec.BroadcastMessage ("AddUnits", attUnits); //Just add the number of units taken from the first sector to the second
		}
		mode=1; 											//After sorting out units and owners
		Sectors.BroadcastMessage ("setFlash", true);		//Enabling flashing on all sectors
		Renderer MyRenderer = AttSec.GetComponent<SpriteRenderer> (); 
		Color color; 													
		color = MyRenderer.material.color; 					//Update the transparency of the attacking sector to be semi-transparent
		color.a = 0.5f; 
		MyRenderer.material.color = color; 	  
		GmInstrctns.text = "Pick a sector to attack with..."; //Update instructions to show user next step
	} 
}
	

