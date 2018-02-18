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
    [SerializeField] private bool turnTimerPaused = false;
    private bool hadUpdate = false;
	public Text timerText;
	private Section[] sections;

    // Use this for initialization
    void Start () {
        neutralAI = GetComponent<NeutralAI>();
		assignUnits = GetComponent<AssignUnits> ();
        suddenDeath = GetComponent<SuddenDeath>();
		conflictResolution = GetComponent<ConflictResolution>();
		timerText = GameObject.Find("TurnTimerText").GetComponent<Text>();
		sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();

        if (Data.IsDemo){
            StartCoroutine("DemoModeRoutine");
        }
        if (Data.GameFromLoaded) {
            // Restores the current turn value
            SetTurn(SaveGameHandler.loadedGame.CurrentTurn);
        }
    }

	void Update() {

        if (!turnTimerPaused)
        {
            turnTimerLength -= Time.deltaTime; //decrements timer on each update
            updateTimerText(); //calls method to update counter in game UI
            if (turnTimerLength < 0.0f)
            { //timer ran out
                Debug.Log("TURN OVER - TIME RAN OUT");
                NextTurn(); //move game to next player
                ResetTimer(); //reset timer for next player
                conflictResolution.UndoPress(); //resets UI ready for the next player to select a sector
            }	
        }
	}

    public void PauseTurnTimer() {
        turnTimerPaused = true;
    }

    public void UnpauseTurnTimer() {
        turnTimerPaused = false;
    }

    public int GetTurn(){
        return currentTurn;
    }

    public void SetTurn(int turn) {
        currentTurn = turn;
    }

    public void NextTurn(){
        currentTurn = currentTurn + 1;
        // Loops back to player 1 once last player is done
        if (currentTurn > Data.RealPlayers + 1){
            currentTurn = 1;
        } 
		else if (currentTurn == Data.RealPlayers + 1) {
            neutralAI.DecideMove();
        }

        assignUnits.AllocateNewUnits(currentTurn);

		ResetTimer ();
        // if sudden death mode is active, destroy some units along borders
        suddenDeath.KillUnitsOnBorderSectors();
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

	public void DisableRayCast() 
	{
		Button gobutton = GameObject.Find("GoButton").GetComponent<Button>();
		Button backbutton = GameObject.Find("BackButton").GetComponent<Button>();
		Button cardbutton = GameObject.Find("CardButton").GetComponent<Button>();
		Button helpbutton = GameObject.Find("HelpButton").GetComponent<Button>();
		Button settingsbutton = GameObject.Find("SettingsButton").GetComponent<Button>();
		InputField unitstextbox = GameObject.Find("UnitsTextbox").GetComponent<InputField>();

		gobutton.interactable = false;
		backbutton.interactable = false;
		cardbutton.interactable = false;
		helpbutton.interactable = false;
		settingsbutton.interactable = false;
		unitstextbox.interactable = false;

		foreach (Section section in sections)
		{
			section.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		}

			
	}

	public void EnableRayCast ()
	{
		Button gobutton = GameObject.Find("GoButton").GetComponent<Button>();
		Button backbutton = GameObject.Find("BackButton").GetComponent<Button>();
		Button cardbutton = GameObject.Find("CardButton").GetComponent<Button>();
		Button helpbutton = GameObject.Find("HelpButton").GetComponent<Button>();
		Button settingsbutton = GameObject.Find("SettingsButton").GetComponent<Button>();
		InputField unitstextbox = GameObject.Find("UnitsTextbox").GetComponent<InputField>();

		gobutton.interactable = true;
		backbutton.interactable = true;
		cardbutton.interactable = true;
		helpbutton.interactable = true;
		settingsbutton.interactable = true;
		unitstextbox.interactable = true;

		foreach (Section section in sections)
		{
			section.gameObject.layer = LayerMask.NameToLayer ("UI");

		}

	}
    // On input, break routine and restart itself
    IEnumerator DemoModeRoutine (){
        int timeoutTimer = 0;
        while (!hadUpdate && timeoutTimer < (60 * 3))
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);

    }
}
