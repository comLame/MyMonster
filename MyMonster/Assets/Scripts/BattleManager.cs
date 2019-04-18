using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour {

	public GameObject myParty;
	public GameObject enemyParty;
	public GameObject commandArea;
	public GameObject selectMarker;
	public GameObject canvas;
	public GameObject targetMarker_ally;
	public GameObject targetMarker_enemy;

	private int targetMonsterId_ally = 0;
	private int targetMonsterId_enemy = 0;
	private int actionCount = 0; //1だったら1体目の行動 10が終わったら次のターン
	private List<GameObject> monsterOrderList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		InputDate();
		SortMonsterOrder();
		DebugMonsterOrderList();
		CheckNextAction();
	}
	
	//monsterOrderリストにオブジェクトを代入
	private void InputDate(){
		for(int i=0;i<5;i++){
			GameObject monsterObj = myParty.transform.GetChild(i).gameObject;
			monsterOrderList.Add(monsterObj);
		}
		for(int i=0;i<5;i++){
			GameObject monsterObj = enemyParty.transform.GetChild(i).gameObject;
			monsterOrderList.Add(monsterObj);
		}
	}

	//monsterOrderリスト内のオブジェクトをスピード順に並び替える
	private void SortMonsterOrder(){
		monsterOrderList.Sort((a,b) => b.GetComponent<CharacterStatus>().speed - a.GetComponent<CharacterStatus>().speed  );
	}
	
	//monsterOrderListの中身をDebug.Logする
	private void DebugMonsterOrderList(){
		Debug.Log("start");
		for(int i=0;i<monsterOrderList.Count;i++){
			Debug.Log(monsterOrderList[i].GetComponent<CharacterStatus>().battleId);
		}
	}

	//次の行動が味方なのか敵なのか判定
	private void CheckNextAction(){
		actionCount++;
		if(actionCount >= 11){
			actionCount = 1;
			SortMonsterOrder();
		}
		int btlId = monsterOrderList[actionCount-1].GetComponent<CharacterStatus>().battleId;
		if(btlId <= 5)AllyAction();
		else EnemyAction();
	}

	private void AllyAction(){
		commandArea.SetActive(true);
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(monster);
	}

	private void EnemyAction(){
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(monster);
		StartCoroutine(DelayMethod(1.0f,() => {
			CheckNextAction();
		}));
	}

	private void SetSelectMarker(GameObject monster){
		Vector3 pos = canvas.transform.InverseTransformPoint(monster.GetComponent<RectTransform>().position);
		selectMarker.SetActive(true);
		selectMarker.GetComponent<RectTransform>().localPosition 
			= new Vector3(pos.x,pos.y-135,pos.z);
	}

	//ターゲットマーカーの表示非表示
	public void TouchMonster(GameObject monster){
		int battleId = monster.GetComponent<CharacterStatus>().battleId;
		Debug.Log("battleid " + battleId);
		if(battleId <= 5){
			Debug.Log("ally " + targetMonsterId_ally);
			if(battleId == targetMonsterId_ally){
				//同じだったら
				targetMonsterId_ally = 0;
				targetMarker_ally.SetActive(false);
			}else{
				targetMonsterId_ally = battleId;
				Vector3 pos = canvas.transform.InverseTransformPoint(monster.GetComponent<RectTransform>().position);
				targetMarker_ally.SetActive(true);
				targetMarker_ally.GetComponent<RectTransform>().localPosition 
					= new Vector3(pos.x,pos.y-6,pos.z);
			}
		}else {
			Debug.Log("enemy " + targetMonsterId_enemy);
			if(battleId == targetMonsterId_enemy){
				//同じだったら
				targetMonsterId_enemy = 0;
				targetMarker_enemy.SetActive(false);
			}else{
				targetMonsterId_enemy = battleId;
				Vector3 pos = canvas.transform.InverseTransformPoint(monster.GetComponent<RectTransform>().position);
				targetMarker_enemy.SetActive(true);
				targetMarker_enemy.GetComponent<RectTransform>().localPosition 
					= new Vector3(pos.x,pos.y-6,pos.z);
			}
		}
	}

	//味方の攻撃
	public void AllyAttack(){
		commandArea.SetActive(false);
		StartCoroutine(DelayMethod(1.0f,() => {
			CheckNextAction();
		}));
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
