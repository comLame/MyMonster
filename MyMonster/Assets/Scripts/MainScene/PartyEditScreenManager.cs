using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PartyEditScreenManager : MonoBehaviour {

	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;
	public GameObject container;
	public GameObject containerParty;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();

	//private int maxPartyMonsterNum = 5; //パーティに編成できる最大の体数
	private bool isMax = false;

	private void OnEnable(){
		GetOwnMonstersData();
		GetPartyList();

		DisplayBoxMonster();
		DisplayPartyMonster();

		//Check();
		
	}

	private void OnDisable(){
		for(int i=0;i<container.transform.childCount;i++){
			Debug.Log(i + ":destroy");
			Destroy(container.transform.GetChild(i).gameObject);
		}
	}

	//色々なチェック
	private void Check(){
		//Debug.Log("["+party.monsters[0]+","+party.monsters[1]+","+party.monsters[2]+","
		//+party.monsters[3]+","+party.monsters[4]+"]");
		//編成体数がマックスかどうか
		//0があればtrue
		if(Array.IndexOf(party.monsters,0) == -1)isMax = true;
		else isMax = false;
;
		//全モンスターに対して処理を行う
		int allMonsterNum = container.transform.childCount;
		for(int i=0;i<allMonsterNum;i++){
			GameObject monsObj = container.transform.GetChild(i).gameObject;
			GameObject img_black = monsObj.transform.GetChild(2).gameObject;
			int uniqueID = monsObj.GetComponent<IconInformation>().uniqueId;

			//パーティに編成中かどうか
			int index = Array.IndexOf(party.monsters, uniqueID);
			GameObject txt_state = monsObj.transform.GetChild(4).gameObject;
			if(index != -1){
				Debug.Log(i+":編成中");
				//編成中
				txt_state.SetActive(true);
				img_black.SetActive(true);
			}else{
				Debug.Log(i+":Box");
				txt_state.SetActive(false);
				img_black.SetActive(false);
			}

			//maxなら全部のimg_blackをtrue
			if(isMax)img_black.SetActive(true);
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

	private void DisplayPartyMonster(){
		int partyCount = partyList.Count;

		for(int i=0;i<containerParty.transform.childCount;i++){
			Destroy(containerParty.transform.GetChild(i).gameObject);
		}

		//party = partyList[0];
		int monsterCount = party.monsters.Length;
		for(int i=0;i<monsterCount;i++){
			int uid = party.monsters[i];
			if(uid == 0){
				//空欄
				GameObject blank = new GameObject("blank");
				blank.AddComponent<Image>();
				blank.transform.SetParent(containerParty.transform);
				blank.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
				blank.GetComponent<RectTransform>().sizeDelta = new Vector2(180,180);
				blank.GetComponent<Image>().color = new Color(1,1,1,0);

				continue;
			}
			Monster mons = GetMonsterFromUID(uid);
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = iconCreaterManager.GetComponent<IconCreaterManager>().Create(mons,type);
			icon.transform.SetParent(containerParty.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<IconInformation>().isPartyMonster = true;
			
			//オブジェクトにButtonInScrollViewがない時はアタッチしてあげる
			if(icon.GetComponent<ButtonInScrollView>() == null){
				icon.AddComponent<ButtonInScrollView>();
			}
			icon.GetComponent<ButtonInScrollView>().onclick= () => {
				OnClickIcon(icon);
			};

		}

	}

	private void DisplayBoxMonster(){

		for(int i=0;i<ownMonsters.Count;i++){
			Monster mons = ownMonsters[i];
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = iconCreaterManager.GetComponent<IconCreaterManager>().Create(mons,type);
			icon.transform.SetParent(container.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<IconInformation>().isPartyMonster = false;

			//オブジェクトにButtonInScrollViewがない時はアタッチしてあげる
			if(icon.GetComponent<ButtonInScrollView>() == null){
				icon.AddComponent<ButtonInScrollView>();
			}
			icon.GetComponent<ButtonInScrollView>().onclick= () => {
				OnClickIcon(icon);
			};
			
		}

		Check();
	}

	private void OnClickIcon(GameObject obj){
		bool isPartyMonster = obj.GetComponent<IconInformation>().isPartyMonster;
		int uniqueID = obj.GetComponent<IconInformation>().uniqueId;
		if(isPartyMonster){
			Debug.Log("party");
			//パーティ編成中のモンスターなら
			int index = Array.IndexOf(party.monsters,uniqueID);
			Array.Clear(party.monsters,index,1);
		}else{
			Debug.Log("Box");
			//Boxにいるモンスターなら
			if(isMax)return;
			int index = Array.IndexOf(party.monsters,0);
			party.monsters[index] = uniqueID;
		}

		//再描画
		DisplayPartyMonster();
		Check();
	}

	//パーティを確定する
	public void Confirm(){
		partyList[0] = party;
		SaveData.SetList<Party>("partyList",partyList);
		SaveData.Save();
	}

	private void GetOwnMonstersData(){
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
	}

	private void GetPartyList(){
		partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];

	}
}
