using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private int currentTurn = 1;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetTurn(){
        return currentTurn;
    }

    public void NextTurn(){
        currentTurn = currentTurn + 1;
        // Loops back to player 1 once player 3 is done
        if (currentTurn > 3){
            currentTurn = 1;
        }
    }




}
