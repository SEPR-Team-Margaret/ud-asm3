using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;

public class Section : MonoBehaviour {
	public GameObject[] AttOptions;  		//Publicly declare the sections this current section can attack, so they can be defined in the editor
	private int FC;							//Define a counter used to control flashing, Flash Controller (FC).
	private int Owner;						//Define a owner of the current setion.
	private int Units;						//Define the units on the current section.
	public Text Label;						//Publicly declare a text object, so a label can be assigned to each section in the editor
	private GameObject manager; 			//Define a gameobject which is used to controll general events
	private bool flashEnable; 				//Define a private boolean used for enabling/disabling flasher
	public Sprite landm; 					//Publicly define a sprite which can be attached to any sector with a landmark
	private GameObject sectorim; 			//Define a object used for displaying sector specific images (like landmarks)
	private Image im; 						//Define the image component for the game object above
	private GameObject sectortext; 			//Define an object to display sector specific text (like sector name)
	private Text sectxt; 					//Define the text component for the game object above
	public string secname;					//Publicly define the name of the current sector so it can be assigned in editor
	public string LMName; 					//Publicly define the name of the landmark in current sector (so it can be assigned in editor if there is one)
	public string LMEff;					//Publicly define the effect of the landmark in current sector (so it can be assigned in editor if there is one)
	private GameObject LMtxtGO; 			//Define an object to display landmark specific text (like landmark name)
	private Text LMtxt;						//Define the text component for the game object above

	// Use this for initialization
	void Start () {
		GameObject me = gameObject; 		//Get the gameobject, the renderer, and the colour of the section.
		Renderer MyRenderer = me.GetComponent<SpriteRenderer>();
		Color color;
		color = MyRenderer.material.color; 
		color.a = 0.5f; 					//Change the opacity of the section so it is 50% transparent.
		MyRenderer.material.color = color;	
		FC = 15; 							//Update the value of the flash controller so the section does not flash.
		manager = GameObject.Find("EventManager"); 	//Find the gameobject used to manage events 
		sectorim = GameObject.Find("Image"); 		//Find the gameobject used to display images
		im = sectorim.GetComponent<Image>(); 		//Find the image component of the above 
		sectortext = GameObject.Find ("Sector"); 	//Find the gameobject used to display sector specific text
		sectxt = sectortext.GetComponent<Text> (); 	//Find the text component of the above
		LMtxtGO = GameObject.Find ("LMDesc");		//Find the gameobject used to display landmark specific text
		LMtxt = LMtxtGO.GetComponent<Text> (); 		//Find the text component of the above
		flashEnable = true; 						//enable flashing
		}
	
	// Update is called once per frame
	void Update () {
		OwnerColour ();						//Update the colour of the section to represent the owner.
		Label.text = Units.ToString();		//Update the label of the section to show the units.
		if (FC<10) {						//If the flash controller has been set to less than 10, 
			Flash ();						//change the opacity (giving a flashing effect) 
			FC = FC + 1;					//increment the flash controller.
		}									//EXPLANATION: By setting the flash contoller to 0 the opacity will change and 
	}										//the controller incremented every frame for 10 frames where the controller now  
											//exceeds 10 and the flashing effect stops.	

	void Flash(){ 
		foreach (GameObject G in AttOptions){		//This function iteratively takes every object that this section can attack,
			Renderer MyRenderer = G.GetComponent<SpriteRenderer> (); //gets its renderer, 
			Color color; 							//and colour.
			color = MyRenderer.material.color; 
			if (color.a == 1) {						//It swaps the opacity between 1 and 0.5.
				color.a = 0.5f; 
			} else { 
				color.a = 1; 
			}
			MyRenderer.material.color = color; 		//Updates the colour.
		}
		System.Threading.Thread.Sleep (100); 		//And adds in a little wait so the user can see the change.
	}

	void OnMouseDown()	//This function is called whenever the sector is clicked
	{   
		if (landm != null) { 						//If this sector has a sprite attached to it for the landmark it contains
			sectorim.SetActive (true); 				//Enable the image game object (make it visible and editable)
			LMtxtGO.SetActive (true);				//Enable the text game object used for landmark information(make it visible and editable)
			im.sprite = landm;   					//Set the image to the sprite attached to the sector
			LMtxt.text = "Landmark: " + LMName + "\n Effect: " + LMEff;	//Format and display the relevant land mark information in the text box
		} else { 									//if the sector does not contain a landmark
			sectorim.SetActive (false); 			//make sure the landmark image, 
			LMtxtGO.SetActive (false);				//and text is disabled
		} 
		sectxt.text = secname;
		if (flashEnable){						//If flashing is enabled set flash controller back to 0 when it is clicked	
			FC = 0;								//so that the section begins flashing.				
		}							
		manager.BroadcastMessage("SetUnits", this.Units);				//Send the relevant information of the sector to the conflict resolution script
		manager.BroadcastMessage("SetOwn", this.Owner); 				//for use when both an attacking and defending sector have been selected
		manager.BroadcastMessage("SetAttOps", this.AttOptions);
		manager.BroadcastMessage("SelectSector", this.gameObject);
	}  	


	void SetOwner(int x){ 
		Owner = x;				//When this function is called it sets the owner of the section to whatever it is passed
	}
	void SetUnits(int x){ 
		Units = x;				//When this function is called it sets the units on this section to whatever it is passed
	}
	void AddUnits(int x){ 		
		Units = Units + x; 		//When this function is called it adds the number it was passed to the units to the section
	}
	void TakeUnits(int x){ 
		Units = Units - x;		//When this function is called it subtracts the number it was passed from the units to the section
	}
	void TurnUnits(int p){  	//When this function is called it adds one unit to the current sector units for every object that can attack it (and hence be attacked by it)	
		if (Owner == p) {		//if the number it was passed is the same as its current owner
			AddUnits (AttOptions.GetLength(0)); 
				
		}
	}

	void OwnerColour(){ 
		switch(this.Owner){  //When this function is called it updates the colour of the section to represent the current owner.
		case(1): 			//If the owner is player 1 the colour is set to red.
			GetComponent<SpriteRenderer> ().color = Color.red;
			break;
		case(2): 			//If the owner is player 2 the colour is set to blue.
			GetComponent<SpriteRenderer>().color = Color.blue;
			break;
		case(3): 			//If the owner is player 3 the colour is set to green.
			GetComponent<SpriteRenderer>().color = Color.green; 
			break;
		default: 			//If the owner is no-one the colour is set to white.
			GetComponent<SpriteRenderer> ().color = Color.clear;
			break;
		}

	} 

	void setFlash(bool x){ 	//simply used to enable/disable flashing
		flashEnable = x;
	}
	void startFlash(){  	//used to begin flashing
		FC = 0;
	} 

}

