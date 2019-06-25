using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyEditScreenManager : MonoBehaviour {

	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;
	public GameObject container;
	public GameObject containerParty;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();

	private void Awake(){
		GetOwnMonstersData();
		GetPartyList();

		DisplayBoxMonster();
		DisplayPartyMonster();
		
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

	private void DisplayPartyMonster(){
		int partyCount = partyList.Count;

		party = partyList[0];
		int monsterCount = party.monsters.Length;
		for(int i=0;i<monsterCount;i++){
			int uid = party.monsters[i];
			Monster mons = GetMonsterFromUID(uid);
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = IconCreate(mons.No,type,mons.level);
			icon.transform.SetParent(containerParty.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

		}

	}

	private void DisplayBoxMonster(){

		for(int i=0;i<ownMonsters.Count;i++){
			Monster mons = ownMonsters[i];
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = IconCreate(mons.No,type,mons.level);
			icon.transform.SetParent(container.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		}
	}

	private GameObject IconCreate(int No,string type,int level){
		//必要要素
		/*
			図鑑No、
			属性、
			レベル、
		 */
		return iconCreaterManager.GetComponent<IconCreaterManager>().Create(No,type,level);
	}

	private void GetOwnMonstersData(){
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
	}

	private void GetPartyList(){
		partyList = SaveData.GetList<Party>("partyList",partyList);
		Debug.Log(partyList);

	}
}
