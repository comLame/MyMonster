using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class QuestSelectScreenManager : MonoBehaviour {

	public StoryQuestGeneralData storyQuestGeneralData;
	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;
	public GameObject containerParty;
	public GameObject islandSelectScreen;
	public GameObject stageSelectScreen;
	public GameObject partySelectScreen;
	public GameObject stageButtonPrefab;
	public GameObject stageButtonPrefab_conversation;

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();
	private List<int> storyProgress = new List<int>();
	private List<int> nowStoryQuest = new List<int>();
 
	private void OnEnable(){
		GetOwnMonstersData();
		GetData();

		DisplayIslandSelectView();
		DisplayStageSelectView();
	}

	private void DisplayIslandSelectView(){

	}

	private void DisplayStageSelectView(){
		GameObject txt_islandName = stageSelectScreen.transform.GetChild(0).GetChild(0).gameObject;
		GameObject containerStageButton = stageSelectScreen.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

		int islandNum = storyProgress[0];
		int stageNum = storyProgress[1];

		//一旦削除
		for(int i=0;i<containerStageButton.transform.childCount;i++){
			Destroy(containerStageButton.transform.GetChild(i).gameObject);
		}

		for(int i=0;i<stageNum;i++){
			GameObject stageBtn;
			if(storyQuestGeneralData.sheets[0].list[i].Category == "Battle"){
				stageBtn = (GameObject)Instantiate(stageButtonPrefab);
				stageBtn.GetComponent<MoveScreenButtonManager>().transitionDestinationScreen = partySelectScreen;
				stageBtn.GetComponent<MoveScreenButtonManager>().hideScreen = stageSelectScreen;
			}else{
				stageBtn = (GameObject)Instantiate(stageButtonPrefab_conversation);
			}
			GameObject txt_number = stageBtn.transform.GetChild(0).gameObject;
			GameObject txt_name = stageBtn.transform.GetChild(1).gameObject;

			stageBtn.transform.SetParent(containerStageButton.transform);
			stageBtn.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			

			txt_number.GetComponent<Text>().text = islandNum.ToString() + "-" + (i+1).ToString();

			String stageName = storyQuestGeneralData.sheets[islandNum-1].list[i].Name;
			txt_name.GetComponent<Text>().text = stageName;

			//オブジェクトにEventTriggerがない時
			if(stageBtn.GetComponent<EventTrigger>() == null){
				stageBtn.AddComponent<EventTrigger>();
			}

			var trigger = stageBtn.GetComponent<EventTrigger>();
			if(trigger.triggers == null){
				//アクションが設定されていない場合は設定するアクションリストを作成
				trigger.triggers = new List<EventTrigger.Entry>();
			}

			int nowStageNum = i+1;
			// クリック時のイベントを設定してみる
			var entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick; // 他のイベントを設定したい場合はここを変える
			entry.callback.AddListener( (x) => { 
				if(storyQuestGeneralData.sheets[0].list[nowStageNum-1].Category == "Battle"){
					DisplayParty(islandNum,nowStageNum,stageName);
				}else{
					ToConversationScene(islandNum,nowStageNum);
				}
			});
			trigger.triggers.Add(entry);

		}

	}

	public void DisplayParty(int island,int stage,String name){

		//出撃しようとしているクエスト情報
		nowStoryQuest.Add(island);
		nowStoryQuest.Add(stage);

		GameObject containerSelectedStage = partySelectScreen.transform.GetChild(1).gameObject;
		GameObject txt_stageNum = containerSelectedStage.transform.GetChild(0).gameObject;
		GameObject txt_stageName = containerSelectedStage.transform.GetChild(1).gameObject;
		txt_stageNum.GetComponent<Text>().text = island.ToString() + "-" + stage.ToString();
		txt_stageName.GetComponent<Text>().text = name;

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

	public void ToBattleScene(){
		SaveData.SetList<int>("nowStoryQuest",nowStoryQuest);
		SaveData.Save();

		FadeManager.Instance.LoadScene ("BattleScene", 1.0f);
	}

	private void ToConversationScene(int island,int stage){
		//出撃しようとしているクエスト情報
		nowStoryQuest.Add(island);
		nowStoryQuest.Add(stage);
		SaveData.SetList<int>("nowStoryQuest",nowStoryQuest);
		SaveData.Save();

		FadeManager.Instance.LoadScene ("ConversationScene", 1.0f);
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

	private void GetData(){
		partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];

		storyProgress = SaveData.GetList<int>("storyProgress",new List<int>(){1,1});

	}

}
