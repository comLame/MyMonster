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
				for(int k=0;k<4;k++){
					mons.skills[k] = Random.Range(1,10);
				}
				ownMonsters.Add(mons);
			}
		}

		Party party = new Party();
		party.monsters[0] = 1;
		party.monsters[1] = 7;
		party.monsters[2] = 5;
		party.monsters[3] = 19;
		party.monsters[4] = 21;
		List<Party> partyList = new List<Party>(){party};

		SaveData.SetList<Monster>("ownMonsters",ownMonsters);
		SaveData.SetList<Party>("partyList",partyList);
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
