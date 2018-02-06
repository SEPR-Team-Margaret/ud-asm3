using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NeutralAI))]

public class Game : MonoBehaviour {

    private int currentTurn = 1;
	private AssignUnits assignUnits;
    private NeutralAI neutralAI;
    private SuddenDeath suddenDeath;
	public bool spawnNewUnitsEachTurn = true;
	private float turnTimerLength = 30.0f;
	public Text timerText;

    // Use this for initialization
    void Start () {
        neutralAI = GetComponent<NeutralAI>();
		assignUnits = GetComponent<AssignUnits> ();
        suddenDeath = GetComponent<SuddenDeath>();
		timerText = GameObject.Find("TurnTimerText").GetComponent<Text>();
    }

	void Update() {
		turnTimerLength -= Time.deltaTime;
		//Debug.Log("Timer remaining: ");
		//Debug.Log(turnTimerLength);
		updateTimerText();
		if (turnTimerLength < 0.0f) {
			Debug.Log("TURN OVER - TIME RAN OUT");
			NextTurn ();
			Debug.Log(currentTurn);
			ResetTimer ();
		}			
	}

	public void ResetTimer() {
		turnTimerLength = 30.0f;
	}

	public void updateTimerText(){
		int iTurnTimerLength = (int)(turnTimerLength);
		timerText.text = "Turn time remaining: " + iTurnTimerLength.ToString();
	}

    public int GetTurn(){
        return currentTurn;
    }

    public void NextTurn(){
        currentTurn = currentTurn + 1;
		SpawnNewUnits ();
        // Loops back to player 1 once player 3 is done
        if (currentTurn > 3){
            currentTurn = 1;
        } else if (currentTurn == 3){
            neutralAI.DecideMove();
        }

        // if sudden death mode is active, destroy some units along borders
        suddenDeath.KillUnitsOnBorderSectors();
    }

	//spawning new units for each turn - REPLACING IN GAME BUTTONS
	public void SpawnNewUnits(){
		if (currentTurn == 1) {
			assignUnits.AllocatePlayer2NewUnits();
		} else if (currentTurn == 2) {
			assignUnits.AllocatePlayer1NewUnits();
		}
	}
}
