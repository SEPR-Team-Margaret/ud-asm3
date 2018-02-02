using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeutralAI))]

public class Game : MonoBehaviour {

    private int currentTurn = 1;
	public bool spawnNewUnitsEachTurn = true;
	private AssignUnits assignUnits;
    private NeutralAI neutralAI;


    // Use this for initialization
    void Start () {
        neutralAI = GetComponent<NeutralAI>();
		assignUnits = GetComponent<AssignUnits> ();
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
