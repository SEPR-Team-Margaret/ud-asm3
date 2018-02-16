using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NeutralAI))]

public class Game : MonoBehaviour {

    private int currentTurn = 1;
	private AssignUnits assignUnits;
    private NeutralAI neutralAI;
    private SuddenDeath suddenDeath;
	private ConflictResolution conflictResolution;
	public bool spawnNewUnitsEachTurn = true;
	private float turnTimerLength = 30.0f;
    private bool hadUpdate = false;
	public Text timerText;

    // Use this for initialization
    void Start () {
        neutralAI = GetComponent<NeutralAI>();
		assignUnits = GetComponent<AssignUnits> ();
        suddenDeath = GetComponent<SuddenDeath>();
		conflictResolution = GetComponent<ConflictResolution>();
		timerText = GameObject.Find("TurnTimerText").GetComponent<Text>();
        if (Data.IsDemo){
            StartCoroutine("DemoModeRoutine");
        }
    }

	void Update() {
		turnTimerLength -= Time.deltaTime; //decrements timer on each update
		updateTimerText(); //calls method to update counter in game UI
		if (turnTimerLength < 0.0f) { //timer ran out
			Debug.Log("TURN OVER - TIME RAN OUT");
			NextTurn (); //move game to next player
			ResetTimer (); //reset timer for next player
			conflictResolution.UndoPress(); //resets UI ready for the next player to select a sector
		}			
	}

    public int GetTurn(){
        return currentTurn;
    }

    public void SetTurn(int turn) {
        currentTurn = turn;
    }

    public void NextTurn(){
        currentTurn = currentTurn + 1;
		SpawnNewUnits ();
        // Loops back to player 1 once last player is done
        if (currentTurn > Data.RealPlayers + 1){
            currentTurn = 1;
        } 
		else if (currentTurn == Data.RealPlayers + 1) {
            neutralAI.DecideMove();
        }
		ResetTimer ();
        // if sudden death mode is active, destroy some units along borders
        suddenDeath.KillUnitsOnBorderSectors();
    }

	//spawning new units for each turn - REPLACING IN GAME BUTTONS
	public void SpawnNewUnits(){
		if (currentTurn == 1) {
			assignUnits.AllocatePlayer2NewUnits();
		} else if (currentTurn == 2) {
			assignUnits.AllocatePlayer1NewUnits();
		} else if (currentTurn == 3 && Data.RealPlayers == 2) {
            assignUnits.AllocatePlayer1NewUnits();
        }
    }

	public void ResetTimer() { //resets timer
		turnTimerLength = 30.0f;
	}

	public void updateTimerText(){
		int iTurnTimerLength = (int)(turnTimerLength); //turns the float turnTimerLength to int to be displayed
		timerText.text = "Turn time remaining: " + iTurnTimerLength.ToString(); //display timer text in game UI
	}

    public void PassHadUpdate(){
        hadUpdate = true;
    }

    public void SaveGame() {
        SaveGameHandler.SaveGame();
    }

    public void LoadGame() {

    }

    // On input, break routine and restart itself
    IEnumerator DemoModeRoutine (){
        int timeoutTimer = 0;
        while (!hadUpdate && timeoutTimer < (60 * 5))
        {
            yield return new WaitForSeconds(1);
            timeoutTimer += 1;
        }
        if (hadUpdate)
        {
            hadUpdate = false;
            StartCoroutine("DemoModeRoutine");
            yield break;
        }
        SceneManager.LoadScene(0);

    }
}
