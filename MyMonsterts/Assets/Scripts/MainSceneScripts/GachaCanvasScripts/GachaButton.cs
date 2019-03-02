using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


[Serializable] 
public class MonsterSaveData{
	public int monsterID;
	public int monsterEXP;
	public MonsterSaveData(){
		monsterEXP = 0;
		monsterID = 0;
	}
}

public class GachaButton : MonoBehaviour {
	int num;
	string monsterKey;
	// Update is called once per frame

	public void GachaButtonClicked () {

		int orbCount = SaveData.GetInt("orbCount",0);
		GameObject.Find("OrbCountText").GetComponent<Text>().text  =	orbCount.ToString();

		List<MonsterSaveData> monsterSaveData = new List<MonsterSaveData>();
		monsterSaveData = SaveData.GetClassList("MonsterList",new List<MonsterSaveData>());
		
		MonsterSaveData newMonsterData = new MonsterSaveData(); 
		System.Random rnd = new System.Random();    // インスタンスを生成
		newMonsterData.monsterID =  rnd.Next(1,11);
		newMonsterData.monsterEXP = 0;

		monsterSaveData.Add(newMonsterData);

		SaveData.SetClassList("MonsterList",monsterSaveData);
		SaveData.Save();

	}
}
