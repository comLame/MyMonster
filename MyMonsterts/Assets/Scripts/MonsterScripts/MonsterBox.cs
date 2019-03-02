using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		List<MonsterSaveData> monsterList = new List<MonsterSaveData>();
		monsterList = SaveData.GetClassList("MonsterList",new List<MonsterSaveData>());

		//int num = SaveData.GetInt("getNumber",0);

		for(int i=0;i<monsterList.Count;i++){
			//MonsterSaveData msd = SaveData.GetClass<MonsterSaveData>(num.ToString(),new MonsterSaveData());
			this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
			this.gameObject.transform.GetChild(i)
			.transform.GetChild(0).GetComponent<Text>().text
			= monsterList[i].monsterID.ToString();
		}
	}

}
