using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignUnits : MonoBehaviour {
	
    public GameObject[] sectors; 					//Declare Sections publicly so they can be assigned in editor 
    private List<int> assignedSectors = new List<int>();		//Define a list of picked sections so that the same section will
	private Game game;
	
    // Use this for initialization					//not be assigned twice.
	void Start () {
        
        System.Random random = new System.Random ();	//To start with every, section is assigned to Player 3, 
		
        foreach (GameObject sector in sectors) { 		//the unbiased AI, with a random number of units between 1 and 10.

            int units = random.Next (1,25);

            sector.BroadcastMessage ("SetOwner", 3); 		
            sector.BroadcastMessage ("SetUnits",units);
		}
                                                    //After all sections are under player 3's contol,
		AssignPlayer (1, 3); 						//Player 1 is assigned 3 sections, with 25 units each.
		AssignPlayer (2, 3);						//Player 2 is also assigned 3 sections, with 25 units each.
		SpawnPVC();
	}

    void AssignPlayer(int player, int sectors){ 			//This is the function that takes an integer representing the player
		
        System.Random random = new System.Random (); 			//and a number of sections to assign to that player.
		int i = 0;

        while (i < sectors) { 								//Then iteratively...
			
            int sectorID = random.Next(this.sectors.Length);       //picks a random section
			
            if (!assignedSectors.Contains(sectorID)) { 									//and if it has not been assigned yet
				this.sectors[sectorID].BroadcastMessage ("SetOwner", player); 	//Assigns it to the player,
				this.sectors[sectorID].BroadcastMessage ("SetUnits", 25);		//Sets the number of units on that point to 25,
				i += 1; 									//updates the counter to show one section has been assigned,
				assignedSectors.Add(sectorID);							//and updates the list of picked sections to show this section has
			}												//now been assigned to a player.
		}
	} 
	
    void AllocateNewUnits(int player) {

        foreach (GameObject sector in this.sectors)
        {
            sector.BroadcastMessage("AllocateNewUnits", player);
        }

    }


    public void AllocatePlayer1NewUnits()
	{ 					
        AllocateNewUnits(1);
	}
	
    public void AllocatePlayer2NewUnits()
	{ 
        AllocateNewUnits(2);
	}
     
	public void SpawnPVC(){ 			//This is the function that takes an integer representing the player

		System.Random random = new System.Random (); 			//and a number of sections to assign to that player.
		int i = 0;

		int sectorID = random.Next(this.sectors.Length);       //picks a random section
		this.sectors [sectorID].BroadcastMessage("spawnPVC"); //sets boolean for PVC to true
	}
}
