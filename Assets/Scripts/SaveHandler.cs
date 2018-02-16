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

        string saveName = Application.persistentDataPath + GetNextSaveGameName();

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

        string fileName = Application.persistentDataPath + "saveGame" + PadZeros(slotID) + slotID + ".ud.bin";

        if (!File.Exists(fileName)) InitializePersistentData();
        else {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);

            System.Object data = bf.Deserialize(file);

            if (data is SaveGame) {

                SaveGame saveGame = (SaveGame)data;

                Data.RealPlayers = saveGame.RealPlayers;
                Data.IsDemo = saveGame.IsDemo;

                SceneManager.LoadScene("Mappit.unity");

                Section[] sections = GameObject.Find("EventManager").GetComponent<AssignUnits>().sectors;

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

                game.SetTurn(saveGame.CurrentTurn);

            } else {
                Debug.Log("Invalid Savegame");
            }
        }
    }

    private static void InitializePersistentData() {
        Data.RealPlayers = 2;
        Data.IsDemo = false;

        SceneManager.LoadScene("Mappit.unity");
    }

    private static string GetNextSaveGameName() {
        string[] filePaths = Directory.GetFiles(@Application.persistentDataPath, "*.ud.bin", SearchOption.TopDirectoryOnly);
        int lastSlotUsed = 0;
        foreach (string file in filePaths) {
            try {
                string fileName = file.Split('.')[0];
                string slotNumberStr = fileName.Substring(8, 3);
                int slotNumber = int.Parse(slotNumberStr);
                if (slotNumber > lastSlotUsed) {
                    lastSlotUsed = slotNumber + 1;
                }
            } catch {
                //NOP
            }
        }

        return "saveGame00" + PadZeros(lastSlotUsed) + lastSlotUsed.ToString() + ".ud.bin";

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
