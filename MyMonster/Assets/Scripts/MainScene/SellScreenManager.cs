using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellScreenManager : MonoBehaviour {

	public BaseStatsData baseStatsData;
	public GameObject scrollContainer;
	public GameObject containerInfo; //売却情報のコンテナ
	public GameObject iconPrefab;
	public GameObject iconCreaterManager;
	public GameObject bg_popup; //確認用のポップアップの背景

	private int selectCount = 0;
	private int totalMoney = 0;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();
	private List<int> sellMonsterList = new List<int>(); //売却するモンスターのuidリスト

	private void OnEnable(){
		GetData();
		DisplayMonster();
	}

	//モンスターアイコンの表示
	private void DisplayMonster(){
		//一旦削除
		for(int i=0;i<scrollContainer.transform.childCount;i++){
			Destroy(scrollContainer.transform.GetChild(i).gameObject);
		}
		sellMonsterList.Clear();
		totalMoney = 0;
		DisplaySellInfo();

		//アイコンの表示
		for(int i=0;i<ownMonsters.Count;i++){
			int uid = ownMonsters[i].uniqueID;

			Monster mons = GetMonsterFromUID(uid);
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = iconCreaterManager.GetComponent<IconCreaterManager>().Create(mons,type);
			icon.transform.SetParent(scrollContainer.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<IconInformation>().isPartyMonster = true;

			GameObject black = icon.transform.GetChild(2).gameObject;
			GameObject txt_isOrganize = icon.transform.GetChild(4).gameObject;

			//編成中なら編成中テキストを表示
			for(int j=0;j<5;j++){
				if(party.monsters[j] == uid){
					//パーティ編成中のモンスター
					black.SetActive(true);
					txt_isOrganize.SetActive(true);
				}
			}

			//オブジェクトにButtonInScrollViewがない時はアタッチしてあげる
			if(icon.GetComponent<ButtonInScrollView>() == null){
				icon.AddComponent<ButtonInScrollView>();
			}
			icon.GetComponent<ButtonInScrollView>().onclick= () => {
				OnClickIcon(icon,mons);
			};
			
		}
	}

	private void OnClickIcon(GameObject icon,Monster mons){
		GameObject black = icon.transform.GetChild(2).gameObject;
		GameObject txt_isOrganize = icon.transform.GetChild(4).gameObject;
		GameObject img_check = icon.transform.GetChild(5).gameObject;

		//もし編成中のモンスターならリターン
		if(txt_isOrganize.activeSelf)return;

		
		if(!img_check.activeSelf){
			//追加
			black.SetActive(true);
			img_check.SetActive(true);
			sellMonsterList.Add(mons.uniqueID);
			totalMoney += 400; //お金追加
			DisplaySellInfo();
		}else{
			//削除
			black.SetActive(false);
			img_check.SetActive(false);
			sellMonsterList.RemoveAt(sellMonsterList.IndexOf(mons.uniqueID));
			totalMoney -= 400;
			DisplaySellInfo();
		}
	}

	private void DisplaySellInfo(){
		GameObject txt_count = containerInfo.transform.GetChild(0).GetChild(1).gameObject;
		GameObject txt_money = containerInfo.transform.GetChild(1).GetChild(1).gameObject;

		txt_count.GetComponent<Text>().text = sellMonsterList.Count.ToString();
		txt_money.GetComponent<Text>().text = totalMoney.ToString();
	}
	
	public void DisplayPopUp(){
		GameObject txt = bg_popup.transform.GetChild(0).GetChild(0).gameObject;
		txt.GetComponent<Text>().text = ownMonsters.Count.ToString() + "体のモンスターを"
			+ totalMoney.ToString() + "コインで売却しますか？";
		bg_popup.SetActive(true);
	}

	public void OnClickCancel(){
		bg_popup.SetActive(false);
	}

	public void OnClickSell(){
		//所持モンスターに対して１体ずつ確認
		for(int i=0;i<ownMonsters.Count;i++){
			int uid = ownMonsters[i].uniqueID;
			//売却モンスターの中にあるかどうか見る
			for(int j=0;j<sellMonsterList.Count;j++){
				if(uid == sellMonsterList[j]){
					//一致したらそのモンスターを削除
					ownMonsters.RemoveAt(i);
					sellMonsterList.RemoveAt(j);
				}
			}
		}
		SaveData.SetList<Monster>("ownMonsters",ownMonsters);
		SaveData.Save();

		bg_popup.SetActive(false);
		DisplayMonster();
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

	//データの取得
	private void GetData(){
		partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
	}
}
