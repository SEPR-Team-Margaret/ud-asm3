﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour {

	void OnMouseEnter() {
		GUI.Button(new Rect(10, 10, 100, 20), new GUIContent("Click me", "This is the tooltip"));
		GUI.Label(new Rect(10, 40, 100, 40), GUI.tooltip);
	}

}
