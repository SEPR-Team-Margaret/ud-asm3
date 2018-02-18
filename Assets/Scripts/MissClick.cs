using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissClick : MonoBehaviour
{
	public bool win = false;

	public GameObject startMenu;
	public GameObject buffAwarded;
	public GameObject buffNotAwarded;
	public GameObject PVC;


	void OnMouseDown()
	{
		if (startMenu.activeInHierarchy)
		{
			return;
		}

		if (buffAwarded.activeInHierarchy)
		{
			win = true;
			return;
		}

		win = false;
		gameObject.SetActive(false);
		buffNotAwarded.gameObject.SetActive(true);
	}

	public void Initialize() {

		startMenu.SetActive(true);
		buffAwarded.SetActive(false);
		buffNotAwarded.SetActive(false);
		PVC.SetActive(false);

		win = false;

	}

}