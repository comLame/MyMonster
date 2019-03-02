using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MonsterData : MonoBehaviour {

	int num;
	string monsterKey;

	public static void SaveMonsterData(MonsterSaveData newMonsterData){
		List<MonsterSaveData> monsterSaveData = SaveData.GetClassList("MonsterList", new List<MonsterSaveData>());
		if(newMonsterData != null){
			monsterSaveData.Add(newMonsterData);
			SaveData.SetClassList("MonsterList",monsterSaveData);
			SaveData.Save();
		}
	}

	public void  OnClick1(){
		num = SaveData.GetInt("getNumber",0);
		num++;
		SaveData.SetInt("getNumber",num);
		
		monsterKey = num.ToString();

		MonsterSaveData playerData = SaveData.GetClass<MonsterSaveData>(monsterKey,new MonsterSaveData());
		SaveData.SetClass<MonsterSaveData> (monsterKey, playerData);
		SaveData.Save();
	}
}

