using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChanceCards : MonoBehaviour {

	private int playerOneChanceCards = 5;							//Define a variable that will count player one's chance cards
	private int playerTwoChanceCards = 5;							//Define a variable that will count player two's chance cards
    private int playerThreeChanceCards = 5;                           //Define a variable that will count player two's chance cards

    private Game game;
	private Section section;
	private GameObject gameManager;
	
	public Text chanceCardsText;									//Define the text variable to display the chance card counter
    private GameObject cardImage;
    public Sprite[] cardSprites;
	
	void Start(){
		gameManager = GameObject.Find("EventManager"); 				//Find the gameobject used to manage events
		game = gameManager.GetComponent<Game>();
		section = gameManager.GetComponent<Section>();
		chanceCardsText = GameObject.Find("CardText").GetComponent<Text>();
        cardImage = GameObject.Find("CardImage");
	}
	
	void Update(){
		UpdateChanceCardText();
	}
	
	private void UpdateChanceCardText(){
		if (game.GetTurn() == 1) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[0];
            chanceCardsText.text = GetPlayerOneChance().ToString();
		} else if (game.GetTurn() == 2) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[1];
            chanceCardsText.text = GetPlayerTwoChance().ToString();
		} else if (game.GetTurn() == 3 && Data.RealPlayers == 3) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[2];
            chanceCardsText.text = GetPlayerThreeChance().ToString();
        }
    }
	
	public int GetPlayerOneChance(){
		return playerOneChanceCards;
	}
	
	public int GetPlayerTwoChance(){
		return playerTwoChanceCards;
	}

    public int GetPlayerThreeChance() {
        return playerThreeChanceCards;
    }

    public void SetPlayerOneChance(int val){
		playerOneChanceCards = val;
	}
	
	public void SetPlayerTwoChance(int val){
		playerTwoChanceCards = val;
	}

    public void SetPlayerThreeChance(int val) {
        playerThreeChanceCards = val;
    }

    public void OnClick(){
		if (game.GetTurn() == 1)  {
			if (playerOneChanceCards  > 0) {
				playerOneChanceCards -= 1;
				float rand = Random.Range(0f,3f);
				if (rand < 1) {
					FriendlySectors();
				} else if (1 < rand && rand < 2) {
					AllSectors();
				} else {
					EnemySectors();
				}
			}
		}
		if (game.GetTurn() == 2)  {
			if (playerTwoChanceCards  > 0) {
				playerTwoChanceCards -= 1;
				float rand = Random.Range(0f,3f);
				if (rand < 1) {
					FriendlySectors();
				} else if (1 < rand && rand < 2) {
					AllSectors();
				} else {
					EnemySectors();
				}
			}
		}
        if (game.GetTurn() == 3 && Data.RealPlayers == 3) {
            if (playerThreeChanceCards > 0) {
                playerThreeChanceCards -= 1;
                float rand = Random.Range(0f, 3f);
                if (rand < 1) {
                    FriendlySectors();
                }
                else if (1 < rand && rand < 2) {
                    AllSectors();
                }
                else {
                    EnemySectors();
                }
            }
        }
    }
	
	void FriendlySectors(){
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = 4;
		float rand = Random.Range(0f, 1f);
		float negChance = 0.1f;
		
		if (rand < negChance ) {
			units *= -1;
		}
		foreach (var sect in sections) {
			if (sect.GetOwner() == game.GetTurn()) {
				sect.AddUnits(units);
			}
		}
	}
	
	void AllSectors(){
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = 2;
		float rand = Random.Range(0f, 1f);
		float negChance = 0.1f;
		
		if (rand < negChance ) {
			units *= -1;
		}
		foreach (var sect in sections) {
				sect.AddUnits(units);
		}
	}
	
	void EnemySectors(){
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = -4;
		float rand = Random.Range(0f, 1f);
		float negChance = 0.1f;
		
		if (rand < negChance ) {
			units *= -1;
		}
		foreach (var sect in sections) {
			if (sect.GetOwner() != game.GetTurn()) {
				sect.AddUnits(units);
			}
		}
	}
}

