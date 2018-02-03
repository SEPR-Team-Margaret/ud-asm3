using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenDeath : MonoBehaviour {

    [SerializeField] private int countdown = 2;
    [SerializeField] private bool suddenDeathMode = false;
    [SerializeField] private GameObject splash;

    void Start() {
        splash = GameObject.Find("SuddenDeathUI");
        splash.SetActive(false);
    }

    public void DecrementCountdown() {

        countdown -= 1;

        if (countdown == 0) {
            // initiate sudden death mode
            suddenDeathMode = true;
            splash.SetActive(true);
        }
    }

    public void SuddenDeathConflict() {

    }

}
