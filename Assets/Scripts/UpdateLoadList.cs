using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class UpdateLoadList : MonoBehaviour {

	public GameObject targetGrid;
	public GameObject gridItemPrefab;
	private BinaryFormatter bf;

	private void Start(){
		getSaves();
	}

	public void getSaves(){
		
		string filePath = Application.persistentDataPath + "/";
		BinaryFormatter bf = new BinaryFormatter();
		
		foreach (string file in System.IO.Directory.GetFiles(filePath)){
			
			if (Path.GetExtension(file) == ".bin"){
				Debug.Log("Found Correct Extension"); 
				FileStream currentFile = File.Open(file, FileMode.Open);
				System.Object data = bf.Deserialize(currentFile);
				
				if (data is SaveGame) {
					GameObject newGridItem = Instantiate(gridItemPrefab) as GameObject; 
					newGridItem.transform.parent = GameObject.Find("Grid").transform;
					//Edit GridItems Text Child
				}

			}

		}
	}
	
}
