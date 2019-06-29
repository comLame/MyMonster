using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestSelectScreenManager : MonoBehaviour {

	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;
	public GameObject containerParty;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();

	private void OnEnable(){
		GetOwnMonstersData();
		GetPartyList();

		DisplayParty();
	}

	public void DisplayParty(){

		for(int i=0;i<5;i++){
			int uid = party.monsters[i];
			if(uid == 0){
				//空欄

				continue;
			}
			Monster mons = GetMonsterFromUID(uid);
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = iconCreaterManager.GetComponent<IconCreaterManager>().Create(mons,type);
			GameObject parent = containerParty.transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
			icon.transform.SetParent(parent.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

		}
	}

	private Monster GetMonsterFromUID(int uid){
		int totalCount = ownMonsters.Count;
		for(int i=0;i<totalCount;i++){
			if(uid == ownMonsters[i].uniqueID){
				return ownMonsters[i];
			}
		}

		//一応
		return new Monster();
	}

	private void GetOwnMonstersData(){
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
	}

	private void GetPartyList(){
		partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];
		Debug.Log(partyList);

	}

}
