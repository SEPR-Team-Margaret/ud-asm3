using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine.UI;

public class UpdateLoadList : MonoBehaviour {

	public GameObject targetGrid;
	public GameObject gridItemPrefab;
	
	private BinaryFormatter bf;

	private void Start(){
		getSaves();
	}

	public void getSaves(){

		Text playerNumText;
		Text saveSlotText;
		
		string filePath = Application.persistentDataPath + "/";
		BinaryFormatter bf = new BinaryFormatter();
		
		foreach (string file in System.IO.Directory.GetFiles(filePath)){
			
			if (Path.GetExtension(file) == ".bin"){
				Debug.Log("Found Correct Extension"); 
				FileStream currentFile = File.Open(file, FileMode.Open);
				System.Object data = bf.Deserialize(currentFile);
				
				if (data is SaveGame) {

					SaveGame saveGame = (SaveGame)data;
					
					GameObject newGridItem = Instantiate(gridItemPrefab, targetGrid.transform) as GameObject;
					//Edit GridItems Text Child
					playerNumText = newGridItem.transform.GetChild(0).GetComponent<Text>();
					saveSlotText = newGridItem.transform.GetChild(1).GetComponent<Text>();

					playerNumText.text = saveGame.RealPlayers.ToString() + " Player";

					string fileName = Path.GetFileName(file);
                	string slotNumberStr = fileName.Substring(8, 3);
					saveSlotText.text = "Save #" + slotNumberStr;

				}

			}

		}
	}
	
}
