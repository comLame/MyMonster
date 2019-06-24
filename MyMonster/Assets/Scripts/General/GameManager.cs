using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

	public List<Monster> ownMonsters = new List<Monster>();

	private void Start(){
		DataSet();
		
	}

	private void DataSet(){
		for(int j=0;j<3;j++){
			for(int i=0;i<10;i++){
				Monster mons = new Monster();
				mons.No = j+1;
				mons.uniqueID = (j * 10) + i + 1;
				mons.exp = Random.Range(0,51);
				mons.level = Random.Range(5,31);
				ownMonsters.Add(mons);
			}
		}

		SaveData.SetList<Monster>("ownMonsters",ownMonsters);
		SaveData.Save();
	}

	private void Update(){
		if (Input.GetKeyDown(KeyCode.Space)) {
			//セーブデータの取得
			List<Monster> getOwnMonsters = SaveData.GetList<Monster>("ownMonsters",new List<Monster>());
			int test = SaveData.GetInt("test",0);
			Debug.Log(JsonUtility.ToJson(getOwnMonsters[1]));
			//Debug.Log(test);

        }
	}
}
