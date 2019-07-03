using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

	public List<Monster> ownMonsters = new List<Monster>();

	private void Start(){
		//DataSet();
	}

	private void CreateMonsterData(int No,int uid){
		Monster mons = new Monster();
		mons.No = No;
		mons.uniqueID = uid;
		//mons.exp = 100;
		mons.level = 10;
		for(int k=0;k<4;k++){
			mons.skills[k] = Random.Range(1,10);
		}
		mons.totalExp = 1800; //総経験値
		mons.betweenLevelExp = 0; //レベルアップしてから獲得した経験値
		mons.expType = 60; //経験値テーブル　
		ownMonsters.Add(mons);
	}

	public void DataSet(){
		for(int j=0;j<3;j++){
			for(int i=0;i<5;i++){
				CreateMonsterData(j+1,(j * 5) + i + 1);
			}
		}

		Debug.Log("要素数:"+ownMonsters.Count);

		Party party = new Party();
		party.monsters[0] = 1;
		party.monsters[1] = 2;
		party.monsters[2] = 3;
		party.monsters[3] = 4;
		party.monsters[4] = 5;
		List<Party> partyList = new List<Party>(){party};

		SaveData.SetList<Monster>("ownMonsters",ownMonsters);
		SaveData.SetList<Party>("partyList",partyList);
		SaveData.Save();
	}
}
