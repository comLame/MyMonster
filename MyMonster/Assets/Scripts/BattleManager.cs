using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

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
	private List<GameObject> myPartyList = new List<GameObject>();
	private List<GameObject> enemyPartyList = new List<GameObject>();
	private Slider animateSlider; //アニメーションするスライダー

	// Use this for initialization
	void Start () {
		InputDate();
		InitializeSlider(myPartyList);
		InitializeSlider(enemyPartyList);
		SortMonsterOrder();
		DebugMonsterOrderList();
		CheckNextAction();
	}
	
	//monsterOrderList,myPartyList,enemyPartyListにオブジェクトを代入
	private void InputDate(){
		for(int i=0;i<5;i++){
			GameObject monsterObj = myParty.transform.GetChild(i).gameObject;
			monsterOrderList.Add(monsterObj);
			myPartyList.Add(monsterObj);
		}
		for(int i=0;i<5;i++){
			GameObject monsterObj = enemyParty.transform.GetChild(i).gameObject;
			monsterOrderList.Add(monsterObj);
			enemyPartyList.Add(monsterObj);
		}
	}

	//Sliderの初期化
	private void InitializeSlider(List<GameObject> monsterList){
		for(int i=0;i<monsterList.Count;i++){
			GameObject monster =monsterList[i];
			GameObject slider = monster.transform.Find("Slider").gameObject;
			int hp = monster.GetComponent<CharacterStatus>().hp;
			slider.GetComponent<Slider>().maxValue = hp;
			slider.GetComponent<Slider>().value = hp;
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
		//次行動するモンスター
		GameObject monster = monsterOrderList[actionCount-1];
		if(!monster.GetComponent<CharacterStatus>().deathFlag){
			//生きていたら
			int btlId = monsterOrderList[actionCount-1].GetComponent<CharacterStatus>().battleId;
			if(btlId <= 5)AllyAction();
			else EnemyAction();
		}else{
			//死んでいたら
			CheckNextAction();
		}
	}

	//味方の行動
	private void AllyAction(){
		commandArea.SetActive(true);
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(monster);
	}

	//敵の行動
	private void EnemyAction(){
		//動くモンスターの特定
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(monster);

		//攻撃する相手の決定
		List<GameObject> remainMyPartyList = new List<GameObject>(); //生きている味方のリスト
		for(int i=0;i<myPartyList.Count;i++){
			if(!myPartyList[i].GetComponent<CharacterStatus>().deathFlag){
				//生きていればListに追加
				remainMyPartyList.Add(myPartyList[i]);
			}
		}
		remainMyPartyList = remainMyPartyList.OrderBy(a => Guid.NewGuid()).ToList(); //シャッフル
		
		GameObject targetMonster = remainMyPartyList[0];

		//攻撃
		Attack(monster,targetMonster);

		/*
		StartCoroutine(DelayMethod(1.0f,() => {
			CheckNextAction();
		}));
		*/
	}

	//攻撃モーション
	private void AttackMotion(GameObject monster){
		float diff = 150;
		Vector3 originalPos = monster.GetComponent<RectTransform>().localPosition;
		//移動
		if(monster.GetComponent<CharacterStatus>().battleId <= 5){
			//味方モンスター
			monster.GetComponent<RectTransform>().localPosition
				= new Vector3(originalPos.x - diff,originalPos.y,originalPos.z);
		}else{
			monster.GetComponent<RectTransform>().localPosition
				= new Vector3(originalPos.x + diff,originalPos.y,originalPos.z);
		}
		//戻る
		StartCoroutine(DelayMethod(0.8f,() => {
			monster.GetComponent<RectTransform>().localPosition
		 = new Vector3(originalPos.x,originalPos.y,originalPos.z);
		}));
	}

	//被弾モーション
	private void AmmunitionMotion(GameObject monster){
		StartCoroutine("Blink",monster);
	}

	//点滅
	IEnumerator Blink(GameObject monster) {
		int count = 0;
        while ( count <= 3 ) {
			count++;
            //renderer.enabled = !renderer.enabled;
			Color c = monster.GetComponent<Image>().color;
			if(c.a == 1){
				//透明じゃない
				monster.GetComponent<Image>().color = new Color(1,1,1,0);
			}else{
				//透明
				monster.GetComponent<Image>().color = new Color(1,1,1,1);
			}
            yield return new WaitForSeconds(0.2f);
            }
    }

	//攻撃
	private void Attack(GameObject attackMonster,GameObject attackedMonster){
		float skillDamage = 100; //技のダメージ
		float attack = attackMonster.GetComponent<CharacterStatus>().attack; //攻撃するモンスターの攻撃力
		float defence = attackedMonster.GetComponent<CharacterStatus>().defense; //攻撃されるモンスターの防御力
		float attributeMatch = 1.0f; //タイプ一致
		float attributeAffinity = 1.0f; //タイプ相性

		float damage = 22 * skillDamage * attack / defence / 50 * attributeMatch * attributeAffinity;

		//攻撃
		int originalHp = attackedMonster.GetComponent<CharacterStatus>().hp;
		attackedMonster.GetComponent<CharacterStatus>().hp -= (int)damage;
		int hp = attackedMonster.GetComponent<CharacterStatus>().hp;
		//攻撃モーション
		AttackMotion(attackMonster);

		//スライダー
		//attackedMonster.transform.Find("Slider").gameObject.GetComponent<Slider>().value = hp;
		AnimationSlider(attackedMonster.transform.Find("Slider").gameObject.GetComponent<Slider>(),originalHp,hp);

		//被弾モーション
		AmmunitionMotion(attackedMonster);

		//死亡判定
		if(hp <= 0){
			//死亡
			attackedMonster.GetComponent<CharacterStatus>().deathFlag = true;
			StartCoroutine(DelayMethod(1.0f,() => {
				attackedMonster.SetActive(false);
			}));

			//ターゲットマーカー
			int battleId = attackedMonster.GetComponent<CharacterStatus>().battleId;
			if(battleId <= 5 && battleId == targetMonsterId_ally){
				//味方が死んだ　かつ　ターゲットになってた
				targetMonsterId_ally = 0;
				StartCoroutine(DelayMethod(1.0f,() => {
					targetMarker_ally.SetActive(false);
				}));
			}else if(battleId > 5 && battleId == targetMonsterId_enemy){
				//敵が死んだ　かつ　ターゲットになってた
				targetMonsterId_enemy = 0;
				StartCoroutine(DelayMethod(1.0f,() => {
					targetMarker_enemy.SetActive(false);
				}));
			}
		}

		StartCoroutine(DelayMethod(1.0f,() => {
			CheckNextAction();
		}));

	}

	//スライダーアニメーション
	private void AnimationSlider(Slider slider,int originalHp,int currentHp){
		animateSlider = slider;
		iTween.ValueTo(gameObject,iTween.Hash("from",originalHp,"to",currentHp,"onupdatetarget",gameObject,
			"onupdate","UpdateSlider"));
	}

	private void UpdateSlider(float hp){
		animateSlider.value = hp;
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
		//動くモンスターの特定
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(monster);

		//攻撃する相手の決定
		GameObject targetMonster;
		if(targetMonsterId_enemy == 0){
			//ターゲットしていない
			List<GameObject> remainEnemyPartyList = new List<GameObject>(); //生きている味方のリスト
			for(int i=0;i<enemyPartyList.Count;i++){
				if(!enemyPartyList[i].GetComponent<CharacterStatus>().deathFlag){
					//生きていればListに追加
					remainEnemyPartyList.Add(enemyPartyList[i]);
				}
			}
			remainEnemyPartyList = remainEnemyPartyList.OrderBy(a => Guid.NewGuid()).ToList(); //シャッフル
			
			targetMonster = remainEnemyPartyList[0];
		}else{
			//ターゲットしている
			targetMonster = enemyPartyList[targetMonsterId_enemy - 6];
		}

		//攻撃
		Attack(monster,targetMonster);
		/*
		StartCoroutine(DelayMethod(1.0f,() => {
			CheckNextAction();
		}));
		*/
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
