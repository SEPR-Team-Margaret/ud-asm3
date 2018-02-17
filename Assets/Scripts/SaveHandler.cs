using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public static class SaveGameHandler {

	public static void SaveGame() {
        // Serializes the game state and writes it to a file

        string saveName = Application.persistentDataPath + "/" + GetNextSaveGameName();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveName);

        SaveGame saveGame = new SaveGame();
        saveGame.IsDemo = Data.IsDemo;
        saveGame.RealPlayers = Data.RealPlayers;
        saveGame.Sections = MakeSerialSections(GameObject.Find("EventManager").GetComponent<AssignUnits>().sectors);
        saveGame.CurrentTurn = GameObject.Find("EventManager").GetComponent<Game>().GetTurn();

        bf.Serialize(file, saveGame);

        file.Close();

        Debug.Log("Saved file: " + saveName);

    }

    public static void LoadGame(int slotID) {
        // Reads a serialized game state file and restores the game state

        string fileName = Application.persistentDataPath + "/saveGame" + PadZeros(slotID) + slotID + ".ud.bin";

        // Verify that the requested save game exists
        if (!File.Exists(fileName)) InitializePersistentData();
        else {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);

            System.Object data = bf.Deserialize(file);

            if (data is SaveGame) {

                SaveGame saveGame = (SaveGame)data;

                // Restore global data
                Data.RealPlayers = saveGame.RealPlayers;
                Data.IsDemo = saveGame.IsDemo;

                // Open game scene
                SceneManager.LoadScene("Mappit.unity");

                Section[] sections = GameObject.Find("EventManager").GetComponent<AssignUnits>().sectors;


                // Restore section data
                foreach (Section liveSection in sections) {
                    foreach (SerialSection savedSection in saveGame.Sections) {
                        if (savedSection.landmarkNameString == liveSection.landmarkNameString) {
                            liveSection.SetUnits(savedSection.units);
                            liveSection.SetOwner(savedSection.owner);
                            liveSection.PVCHere = savedSection.PVCHere;
                        }
                    }
                }


                Game game = GameObject.Find("EventManager").GetComponent<Game>();

                // Restores the current turn value
                game.SetTurn(saveGame.CurrentTurn);

            } else {
                Debug.Log("Invalid Savegame");
            }
        }
    }

    private static void InitializePersistentData() {
        // Set defaults for if file loading fails
        Data.RealPlayers = 2;
        Data.IsDemo = false;

        SceneManager.LoadScene("Mappit.unity");
    }

    private static string GetNextSaveGameName() {
        // Generate the file name for the next save game 'slot'
        string[] filePaths = Directory.GetFiles(@Application.persistentDataPath + "/", "*.ud.bin", SearchOption.TopDirectoryOnly);
        foreach(string file in filePaths){
        	 Debug.Log(file);
        }
        Debug.Log(@Application.persistentDataPath);
        int lastSlotUsed = 0;
        foreach (string file in filePaths) {
            try {
                string fileName = file.Split('.')[0];
                string slotNumberStr = fileName.Substring(8, 3);
                int slotNumber = int.Parse(slotNumberStr);
                Debug.Log(slotNumber);
                if (slotNumber > lastSlotUsed) {
                    lastSlotUsed = slotNumber + 1;
                }
            } catch {
                //NOP
                Debug.Log("Test");
            }
        }

        return "saveGame" + PadZeros(lastSlotUsed) + lastSlotUsed.ToString() + ".ud.bin";

    }

    private static string PadZeros(int number) { 
        if (number >= 100) {
            return "";
        } else if (number >= 10) {
            return "0";
        } else {
            return "00";
        }
    }

    private static SerialSection[] MakeSerialSections(Section[] sections) {
        // Creates a serializable data structure which holds key data about sections

        SerialSection[] sSections = new SerialSection[sections.Length];
        int index = 0;
        foreach (Section section in sections) {
            sSections[index] = new SerialSection();
            sSections[index].owner = section.GetOwner();
            sSections[index].landmarkNameString = section.landmarkNameString;
            sSections[index].PVCHere = section.PVCHere;
            sSections[index].units = section.GetUnits();
            index++;
        }
        return sSections;
    }
}

[Serializable]
class SaveGame {
    
    [SerializeField] private bool isDemo;
    [SerializeField] private int realPlayers;
    [SerializeField] private SerialSection[] sections;
    [SerializeField] private int currentTurn;

    public bool IsDemo {
        get {
            return isDemo;
        }
        set {
            isDemo = value;
        }
    }

    public int RealPlayers {
        get {
            return realPlayers;
        }
        set {
            realPlayers = value;
        }
    }

    public SerialSection[] Sections {
        get {
            return sections;
        }
        set {
            sections = value;
        }
    }

    public int CurrentTurn {
        get {
            return currentTurn;
        }
        set {
            currentTurn = value;
        }
    }
}

[Serializable]
class SerialSection {
    [SerializeField] public int owner;
    [SerializeField] public int units;
    [SerializeField] public string landmarkNameString;
    [SerializeField] public bool PVCHere;
}
