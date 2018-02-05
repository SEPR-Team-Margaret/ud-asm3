using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Section : MonoBehaviour {

    private GameObject gameManager;         //Define a gameobject which is used to controll general events
    public Section[] adjacentSectors;       //Publicly declare the sections this current section can attack, so they can be defined in the editor
    private int owner;                      //Define a owner of the current setion.

    private int units;                      //Define the units on the current section.
    public Text unitsText;                  //Publicly declare a text object, so a label can be assigned to each section in the editor

    private int flashCounter;               //Define a counter used to control flashing, Flash Controller (FC).
    private bool flashEnable;               //Define a private boolean used for enabling/disabling flasher

    public Sprite landmarkImage;            //Publicly define a sprite which can be attached to any sector with a landmark
    private GameObject sectorImage;         //Define a object used for displaying sector specific images (like landmarks)
    private Image im;                       //Define the image component for the game object above

    private GameObject sectorNameObject;    //Define an object to display sector specific text (like sector name)
    private Text sectorNameText;            //Define the text component for the game object above
    public string sectorNameString;         //Publicly define the name of the current sector so it can be assigned in editor

    public string landmarkName;             //Publicly define the name of the landmark in current sector (so it can be assigned in editor if there is one)
    public string landmarkEffect;           //Publicly define the effect of the landmark in current sector (so it can be assigned in editor if there is one)
    private GameObject landmarkNameObject;  //Define an object to display landmark specific text (like landmark name)
    private Text landmarkNameText;          //Define the text component for the game object above

    // Use this for initialization
    void Start() {

        //GameObject gameObject = gameObject; 		//Get the gameobject, the renderer, and the colour of the section.
        Renderer renderer = gameObject.GetComponent<SpriteRenderer>();
        Color color = renderer.material.color;
        color.a = 0.5f;                     //Change the opacity of the section so it is 50% transparent.
        renderer.material.color = color;

        flashCounter = 15;                          //Update the value of the flash controller so the section does not flash.
        flashEnable = true;                         //enable flashing

        gameManager = GameObject.Find("EventManager");  //Find the gameobject used to manage events 

        sectorImage = GameObject.Find("LandmarkImage");         //Find the gameobject used to display images
        im = GameObject.Find("LandmarkImage").GetComponent<Image>();        //Find the image component of the above 

        sectorNameObject = GameObject.Find("SectorTitle");  //Find the gameobject used to display sector specific text
        sectorNameText = GameObject.Find("SectorTitle").GetComponent<Text>();   //Find the text component of the above

        landmarkNameObject = GameObject.Find("LandmarkDescription");        //Find the gameobject used to display landmark specific text
        landmarkNameText = GameObject.Find("LandmarkDescription").GetComponent<Text>();         //Find the text component of the above

        unitsText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update() {

        ColorByOwner();                     //Update the colour of the section to represent the owner.

        unitsText.text = units.ToString();  //Update the label of the section to show the units.

        if (flashCounter < 10) {                //If the flash controller has been set to less than 10, 
            Flash();                        //change the opacity (giving a flashing effect) 
            flashCounter = flashCounter + 1;                    //increment the flash controller.

        }                                   //EXPLANATION: By setting the flash controller to 0 the opacity will change and 
    }                                       //the controller incremented every frame for 10 frames where the controller now  
                                            //exceeds 10 and the flashing effect stops.	

    void Flash() {

        foreach (Section sector in adjacentSectors) {       //This function iteratively takes every object that this section can attack,

            Renderer renderer = sector.GetComponent<SpriteRenderer>(); //gets its renderer, 
            Color color;                            //and colour.
            color = renderer.material.color;

            if (color.a == 1) {                     //It swaps the opacity between 1 and 0.5.
                color.a = 0.5f;
            }
            else {
                color.a = 1;
            }

            renderer.material.color = color;        //Updates the colour.
        }

        System.Threading.Thread.Sleep(100);         //And adds in a little wait so the user can see the change.
    }

    void OnMouseDown() {	//This function is called whenever the sector is clicked
        Game game = gameManager.GetComponent<Game>();
        ConflictResolution conRes = gameManager.GetComponent<ConflictResolution>();

        //Detect activity for timeout timer
        if (Data.IsDemo) {
            game.PassHadUpdate();
        }

        if (game.GetTurn() == GetOwner() || conRes.GetMode() == 3) {  //Only run this if a section belonging to the current player is clicked
            if (landmarkImage != null) {                       //If this sector has a sprite attached to it for the landmark it contains
                sectorImage.SetActive(true);                //Enable the image game object (make it visible and editable)
                landmarkNameObject.SetActive(true);             //Enable the text game object used for landmark information(make it visible and editable)
                im.sprite = landmarkImage;                      //Set the image to the sprite attached to the sector
                landmarkNameText.text = "Landmark: " + landmarkName + "\n Effect: " + landmarkEffect;   //Format and display the relevant land mark information in the text box

            }
            else {                                   //if the sector does not contain a landmark
                sectorImage.SetActive(false);           //make sure the landmark image, 
                landmarkNameObject.SetActive(false);                //and text is disabled
            }

            sectorNameText.text = sectorNameString;
            if (flashEnable) {                       //If flashing is enabled set flash controller back to 0 when it is clicked	
                flashCounter = 0;                               //so that the section begins flashing.				
            }

            gameManager.BroadcastMessage("SetUnits", this.units);               //Send the relevant information of the sector to the conflict resolution script
            gameManager.BroadcastMessage("SetPlayer", this.owner);              //for use when both an attacking and defending sector have been selected
            gameManager.BroadcastMessage("SetAdjacentSectors", this.adjacentSectors);
            gameManager.BroadcastMessage("SelectSector", this);
        }
    }


    void SetOwner(int x) {
        owner = x;              //When this function is called it sets the owner of the section to whatever it is passed
    }

    public int GetOwner() {
        return owner;
    }

    public void SetUnits(int x) {
        units = x;              //When this function is called it sets the units on this section to whatever it is passed
    }

    public int GetUnits() {
        return units;
    }

    public void AddUnits(int x) {
        units = units + x;      //When this function is called it adds the number it was passed to the units to the section
        if (units < 1) {
            units = 1;
        }
    }

    void TakeUnits(int x) {
        units = units - x;      //When this function is called it subtracts the number it was passed from the units to the section
    }

    void AllocateNewUnits(int player) {     //When this function is called it adds one unit to the current sector units for every object that can attack it (and hence be attacked by it)	

        if (owner == player) {      //if the number it was passed is the same as its current owner
            AddUnits(adjacentSectors.GetLength(0));
        }
    }

    void ColorByOwner() {
        switch (this.owner) {  //When this function is called it updates the colour of the section to represent the current owner.
            case (1):            //If the owner is player 1 the colour is set to red.
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case (2):           //If the owner is player 2 the colour is set to blue.
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case (3):           //If the owner is player 3 the colour is set to green.
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            default:            //If the owner is no-one the colour is set to white.
                GetComponent<SpriteRenderer>().color = Color.clear;
                break;
        }

    }

    void setFlash(bool flash) {     //simply used to enable/disable flashing
        flashEnable = flash;
    }

    void startFlash() {     //used to begin flashing
        flashCounter = 0;
    }

}
