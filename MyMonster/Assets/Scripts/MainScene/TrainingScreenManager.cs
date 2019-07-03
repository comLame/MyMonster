using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrainingScreenManager : MonoBehaviour {

	public GameObject monsterSelectedFramePrefab;
	public GameObject containerTargetMonster;
	public GameObject containerBox;
	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;

	public GameObject monsterspriteObj;
	public GameObject txt_name;
	public GameObject txt_level;
	public GameObject txt_hp;
	public GameObject txt_attack;
	public GameObject txt_defense;
	public GameObject txt_speed;
	public GameObject containerMonsterInfo;
	public GameObject containerSkill;
	public GameObject btn_monsterInfo;
	public GameObject btn_skill;
	public GameObject btn_evolution;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();
	private float xPos_selected;
	private float xPos_unSelected;

	private GameObject targetMonsterObj;
	
	private void OnEnable(){
		xPos_selected = btn_monsterInfo.GetComponent<RectTransform>().localPosition.x;
		xPos_unSelected = btn_skill.GetComponent<RectTransform>().localPosition.x;

		GetOwnMonstersData();
		//GetPartyList();

		DisplayBoxMonster();
		DisplayTargetMonster();

	}

	private void OnDisable(){
		for(int i=0;i<containerBox.transform.childCount;i++){
			Destroy(containerBox.transform.GetChild(i).gameObject);
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

	private void DisplayTargetMonster(){
		//int uid = targetMonsterObj.GetComponent<IconInformation>().uniqueId;
		//変数宣言
		Monster monsterInfo = targetMonsterObj.GetComponent<IconInformation>().monsterInfo;
		int No = monsterInfo.No;
		BaseStatsData.Param baseStats = baseStatsData.sheets[0].list[No-1];
		Sprite monsterSprite = Resources.Load<Sprite>("Img_Monster/" + No);;
		string name = baseStats.Name;
		int level = monsterInfo.level;
		int hp = baseStats.Hp;
		int attack = baseStats.Attack;
		int defense = baseStats.Defense;
		int speed = baseStats.Speed;
		//変数代入
		monsterspriteObj.GetComponent<Image>().sprite = monsterSprite;
		txt_name.GetComponent<Text>().text = name;
		txt_level.GetComponent<Text>().text = "LV:" + level.ToString() + "/100";
		txt_hp.GetComponent<Text>().text = hp.ToString();
		txt_attack.GetComponent<Text>().text = attack.ToString();
		txt_defense.GetComponent<Text>().text = defense.ToString();
		txt_speed.GetComponent<Text>().text = speed.ToString();
		
	}

	private void DisplayBoxMonster(){

		for(int i=0;i<ownMonsters.Count;i++){
			Monster mons = ownMonsters[i];
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = iconCreaterManager.GetComponent<IconCreaterManager>().Create(mons,type);
			GameObject monsObj = (GameObject)Instantiate(monsterSelectedFramePrefab);
			if(i==0){
				//ターゲットモンスター
				targetMonsterObj = icon;
			}else{
				//それ以外
				//ターゲットフレームを消す
				monsObj.transform.GetChild(0).gameObject.SetActive(false);
			}
			icon.transform.SetParent(monsObj.transform,false);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			icon.GetComponent<IconInformation>().monsterInfo = mons;
			monsObj.transform.SetParent(containerBox.transform);
			monsObj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			/* 
			if(i==0)targetMonsterObj = icon;
			icon.transform.SetParent(containerBox.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			icon.GetComponent<IconInformation>().isPartyMonster = false;
			icon.GetComponent<IconInformation>().monsterInfo = mons;
			*/

			//コンポーネントがついてなければアタッチ
			if(icon.GetComponent<EventTrigger>() == null){
				var trigger = icon.AddComponent<EventTrigger>();
				trigger.triggers = new List<EventTrigger.Entry>();

				// クリック時のイベントを設定してみる
				var entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerClick; // 他のイベントを設定したい場合はここを変える
				entry.callback.AddListener( (x) => { 
					OnClickIcon(icon);
				});
				trigger.triggers.Add(entry);
			}
			
		}
	}

	public void ChangeView(string view){
		if(view == "info"){
			DisplayMonsterInfoView();
		}else if(view == "skill"){
			DisplaySkillView();
		}
	}

	private void DisplayMonsterInfoView(){
		containerMonsterInfo.SetActive(true);
		containerSkill.SetActive(false);

		//ボタン
		btn_monsterInfo.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_selected,
			btn_monsterInfo.GetComponent<RectTransform>().localPosition.y,
			btn_monsterInfo.GetComponent<RectTransform>().localPosition.z
		);
		btn_skill.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_unSelected,
			btn_skill.GetComponent<RectTransform>().localPosition.y,
			btn_skill.GetComponent<RectTransform>().localPosition.z
		);
		btn_evolution.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_unSelected,
			btn_evolution.GetComponent<RectTransform>().localPosition.y,
			btn_evolution.GetComponent<RectTransform>().localPosition.z
		);
		btn_monsterInfo.transform.GetChild(2).gameObject.SetActive(false);
		btn_skill.transform.GetChild(2).gameObject.SetActive(true);
		btn_evolution.transform.GetChild(2).gameObject.SetActive(true);
	}

	private void DisplaySkillView(){
		containerMonsterInfo.SetActive(false);
		containerSkill.SetActive(true);

		//ボタン
		btn_monsterInfo.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_unSelected,
			btn_monsterInfo.GetComponent<RectTransform>().localPosition.y,
			btn_monsterInfo.GetComponent<RectTransform>().localPosition.z
		);
		btn_skill.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_selected,
			btn_skill.GetComponent<RectTransform>().localPosition.y,
			btn_skill.GetComponent<RectTransform>().localPosition.z
		);
		btn_evolution.GetComponent<RectTransform>().localPosition = new Vector3(
			xPos_unSelected,
			btn_evolution.GetComponent<RectTransform>().localPosition.y,
			btn_evolution.GetComponent<RectTransform>().localPosition.z
		);
		btn_monsterInfo.transform.GetChild(2).gameObject.SetActive(true);
		btn_skill.transform.GetChild(2).gameObject.SetActive(false);
		btn_evolution.transform.GetChild(2).gameObject.SetActive(true);
	}

	private void OnClickIcon(GameObject monsObj){
		//前のターゲットフレームを消す
		targetMonsterObj.transform.parent.GetChild(0).gameObject.SetActive(false);
		targetMonsterObj = monsObj;
		//新しいターゲットフレームを表示
		targetMonsterObj.transform.parent.GetChild(0).gameObject.SetActive(true);
		DisplayTargetMonster();
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
		Debug.Log(partyList);

	}

}
