﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public void Play(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
	}

	public void Quit(){
		Debug.Log("Application Quit");
		Application.Quit();
	}

    public void DemoMode(bool value)
    {
        Data.IsDemo = value;
    }
}
