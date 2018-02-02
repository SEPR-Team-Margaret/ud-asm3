using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeutralAI))]

public class Game : MonoBehaviour {

    private int currentTurn = 1;

    private NeutralAI neutralAI;


    // Use this for initialization
    void Start () {
        neutralAI = GetComponent<NeutralAI>();
    }

    public int GetTurn(){
        return currentTurn;
    }

    public void NextTurn(){
        currentTurn = currentTurn + 1;
        // Loops back to player 1 once player 3 is done
        if (currentTurn > 3){
            currentTurn = 1;
        } else if (currentTurn == 3){
            neutralAI.DecideMove();
        }
    }




}
