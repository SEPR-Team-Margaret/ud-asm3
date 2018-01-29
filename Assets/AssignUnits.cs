using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignUnits : MonoBehaviour {
	public GameObject[] Sections; 					//Declare Sections publicly so they can be assigned in editor 
	private List<int> picked = new List<int>();		//Define a list of picked sections so that the same section will 
	// Use this for initialization					//not be assigned twice.
	void Start () {
		System.Random R2 = new System.Random ();		//To start with every, section is assigned to Player 3, 
		foreach (GameObject Sect in Sections) { 		// the unbiased AI, with a random number of units between 1 and 10.
			Sect.BroadcastMessage ("SetOwner", 3); 		
			int RUnits = R2.Next (1,11);
			Sect.BroadcastMessage ("SetUnits",RUnits);
		}  											//After all sections are under player 3's contol,
		AssignPlayer (1, 3); 						//Player 1 is assigned 3 sections, with 25 units each.
		AssignPlayer (2, 3);						//Player 2 is also assigned 3 sections, with 25 units each.
	}

	void AssignPlayer(int Player,int sections){ 			//This is the function that takes an integer representing the player
		System.Random Rand = new System.Random (); 			//and a number of sections to assign to that player.
		int i = 0;
		while (i <sections) { 								//Then iteratively...
			bool pickedB = true;
			int RSect = Rand.Next(Sections.Length); 		//picks a random section
			foreach (int p in picked) { 					//checks if it has been picked
				if (p == RSect){ 
					pickedB = false;
				}
			}
			if (pickedB){ 									//and if it has not been picked yet
				Sections[RSect].BroadcastMessage ("SetOwner", Player); 	//Assigns it to the player,
				Sections[RSect].BroadcastMessage ("SetUnits", 25);		//Sets the number of units on that point to 25,
				i = i + 1; 									//updates the counter to show one section has been assigned,
				picked.Add(RSect);							//and updates the list of picked sections to show this section has
			}												//now been assigned to a player.
		}
	} 
	void P1TurnUnits()
	{ 					
		foreach (GameObject Sect in Sections) {				//Call a function in every sector that will add units to each one based on   
			Sect.BroadcastMessage ("TurnUnits", 1);			//the number of boundries it has...if owned by player one 
		}
	}
	void P2TurnUnits()
	{ 
		foreach (GameObject Sect in Sections) {				//Call a function in every sector that will add units to each one based on
			Sect.BroadcastMessage ("TurnUnits", 2);			//the number of boundries it has...if owned by player two 
		}
	}
}
