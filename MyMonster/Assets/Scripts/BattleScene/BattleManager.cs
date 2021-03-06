﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; 

public class BattleManager : MonoBehaviour {

	//エクセルデータ関係
	public BaseStatsData baseStatsData;
	public EnemyPartyData enemyPartyData;
	public StoryQuestGeneralData storyQuestGeneralData;
	public Island01EnemyData island01EnemyData;
	public MyPartyData myPartyData;
	public SkillData skillData;
	public SkillEffectData skillEffectData;
	public TypeEffecetiveness typeEffectiveness;
	//ここまで
	public GameObject resultScreen; //結果画面
	public GameObject containerBG; //背景のコンテナ
	public GameObject fadeCanvas;
	public GameObject myParty;
	public GameObject enemyParty;
	public GameObject commandArea;
	public GameObject selectMarker;
	public GameObject canvas;
	public GameObject targetMarker_ally;
	public GameObject targetMarker_enemy;
	public GameObject bg_VOD;
	public GameObject atkFX_x8Prefab;
	public GameObject atkFX_x10Prefab;
	public GameObject txt_skillNamePrefab;
	public GameObject txt_damagePrefab;
	public GameObject txt_healPrefab;
	public GameObject txt_changePrefab;
	public Material m_skillEffect;
	public GameObject questNameView;

	private int counter_checkNextAction = 0;
	private int count_targetMonster = 0; //攻撃対象になるモンスターの数
	private int vod = 0; //勝敗 1->勝ち -1->敗け
	private int targetMonsterId_ally = 0;
	private int targetMonsterId_enemy = 0;
	private int actionCount = 0; //1だったら1体目の行動 10が終わったら次のターン
	private int nowWave = 1; //今のWave数 1~
	private int totalWave; //トータルのWave数 1~
	private int num_frame = 5; //１エフェクトのフレーム数
	private float time_break = 0.5f; //行動と行動の間のブレイク
	private float time_stepFront = 0.12f; //前にちょこっと進む時間
	private float time_stepBack = 0.12f; //後ろに戻る時間
	private float time_attackAnimation = 1.0f; //攻撃モーションのアニメーション時間
	private float time_hitAnimation = 1.0f; //被弾モーションのアニメーション時間
	private float time_deathAnimation = 1.0f; //死亡アニメーション時間
	private float time_frame = 0.2f; //エフェクトの１枚ずつの表示時間
	private float time_entranceAnimation = 2f; //入場アニメーションの時間
	private float time_allyEntranceAnimation = 0.5f;
	private float time_nextWaveAnimation = 3f; //次のウェーブにいく時のアニメーション
	private float time_questNameFadeIn = 0.5f; //クエストネームのフェイドイン時間
	private float time_questNameStay = 1.5f; //クエストネームを表示する時間
	private float time_questNameFadeOut = 0.5f; //クエストネームのフェイドアウト時間
	private float speed_attackAnimation = 1f; //攻撃アニメーションのスピード
	private float diff_step = 0.2f; //ステップの距離
	private float zPos_attackEffect = -3f; //攻撃エフェクトのz座標
	private string questName=""; //クエスト名
	private List<GameObject> monsterOrderList = new List<GameObject>();
	private List<GameObject> myPartyList = new List<GameObject>();
	private List<GameObject> enemyPartyList = new List<GameObject>();
	private Fade fade; //fadeオブジェクト

	private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();
	private List<int> nowStoryQuest = new List<int>();
	private int islandNum; //島番号
	private int battleQuestNum; //バトルクエストのなんばんめか

	enum Type {
		Fire,
		Water,
		Grass,
		Lightning,
		Darkness
	}

	// Use this for initialization
	void Start () {
		GetSaveData();
		//NextWaveAnimation();
		//return;
		CountWave();
		InputMonsterList();
		GetMyPartyData();
		GetEnemyPartyData();
		InitializeSlider(myPartyList);
		InitializeSlider(enemyPartyList);
		SortMonsterOrder();
		DebugMonsterOrderList();

		//fade関係
		BattleStart();
	}

	private void BattleStart(){
		StartCoroutine(DelayMethod(0.5f,() => {
			questNameView.SetActive(true);
			questNameView.transform.GetChild(1).gameObject.GetComponent<Text>().text = questName;
			questNameView.GetComponent<QuestNameViewAnimation>().StartAnimation(
				time_questNameFadeIn,time_questNameStay,time_questNameFadeOut
			);
			StartCoroutine(DelayMethod(time_questNameFadeIn+time_questNameStay,() => {
				EntranceAnimation();
			}));
		}));
	}

	//wave数を数える
	private void CountWave(){
		
		totalWave = (int)island01EnemyData.sheets[battleQuestNum-1].list.Count/5;
		//totalWave = 1;
	}

	private void FadeAnimation(){
		//fade関係
		fade = fadeCanvas.transform.GetChild(0).GetComponent<Fade>();
		GameObject text = fadeCanvas.transform.GetChild(1).gameObject;
		//text.SetActive(true);
		fade.FadeIn (0, () => {
			fade.FadeOut(1,() => {
				StartCoroutine(DelayMethod(0.2f,() => {
					EntranceAnimation();
					StartCoroutine(DelayMethod(1f,() => {
						text.SetActive(false);
					}));
				}));
			});
		});
	}
	
	//monsterOrderList,myPartyList,enemyPartyListにオブジェクトを代入
	private void InputMonsterList(){
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
		monsterOrderList.Sort((a,b) => 
			(b.GetComponent<CharacterStatus>().speed * (1 + b.GetComponent<CharacterStatus>().statusRank_speed/2))
			 - (a.GetComponent<CharacterStatus>().speed * (1 + a.GetComponent<CharacterStatus>().statusRank_speed/2))  );
	}
	
	//monsterOrderListの中身をDebug.Logする
	private void DebugMonsterOrderList(){
		Debug.Log("start");
		for(int i=0;i<monsterOrderList.Count;i++){
			string name = monsterOrderList[i].GetComponent<CharacterStatus>().name;
			int battleId = monsterOrderList[i].GetComponent<CharacterStatus>().battleId;
			int level = monsterOrderList[i].GetComponent<CharacterStatus>().level;
			int hp = monsterOrderList[i].GetComponent<CharacterStatus>().hp;
			int attack = monsterOrderList[i].GetComponent<CharacterStatus>().attack;
			int defense = monsterOrderList[i].GetComponent<CharacterStatus>().defense;
			int speed = monsterOrderList[i].GetComponent<CharacterStatus>().speed;
			//Debug.Log("B_ID:" + battleId + " Lv."+ level + " " + name + " " + hp + " "+ attack + " "+ defense + " " + speed + " ");
		}
	}

	/*=====================================
	 *
	 * データ入力
	 *
	 ======================================*/

	//マイパーティデータを読み込む
	private void GetMyPartyData(){
		//int num_total = myPartyData.sheets[0].list.Count; //総数取得
		
		for(int i=0;i<5;i++){
			GameObject monster = myPartyList[i];
			//int num_pb = myPartyData.sheets[0].list[i].No; //図鑑番号
			int uid = party.monsters[i];
			Monster mons = GetMonsterFromUID(uid);
			int num_pb = mons.No;
			int level = mons.level;
			GetStatus(monster,num_pb,level); //ステータス代入
			GetSkill(monster,i);
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

	//エネミーパーティデータを読み込む
	private void GetEnemyPartyData(){
		//int num_total = enemyPartyData.sheets[0].list.Count; //総数取得
		
		for(int i=0;i<5;i++){
			GameObject monster = enemyPartyList[i];	
			int listNum = i + ((nowWave - 1) * 5 );
			int num_pb = island01EnemyData.sheets[battleQuestNum-1].list[listNum].No; //図鑑番号
			GetStatus(monster,num_pb,island01EnemyData.sheets[battleQuestNum-1].list[listNum].Lv); 
			GetSkill(monster,listNum,true);
		}
	}

	//技を読み込む
	private void GetSkill(GameObject monster,int num,bool isEnemy = false){
		int no_skill1;
		int no_skill2;
		int no_skill3;
		int no_skill4;
		if(!isEnemy){
			//味方
			int uid = party.monsters[num];
			Monster mons = GetMonsterFromUID(uid);
			no_skill1 = mons.skills[0];
			no_skill2 = mons.skills[1];
			no_skill3 = mons.skills[2];
			no_skill4 = mons.skills[3];
		}else{
			//敵
			no_skill1 = island01EnemyData.sheets[battleQuestNum-1].list[num].No_Skill1;
			no_skill2 = island01EnemyData.sheets[battleQuestNum-1].list[num].No_Skill2;
			no_skill3 = island01EnemyData.sheets[battleQuestNum-1].list[num].No_Skill3;
			no_skill4 = island01EnemyData.sheets[battleQuestNum-1].list[num].No_Skill4;
		}

		monster.GetComponent<CharacterStatus>().skills[0] = no_skill1;
		monster.GetComponent<CharacterStatus>().skills[1] = no_skill2;
		monster.GetComponent<CharacterStatus>().skills[2]= no_skill3;
		monster.GetComponent<CharacterStatus>().skills[3] = no_skill4;
	}
	
	//ステータス
	private void GetStatus(GameObject monster,int num_pictureBook,int level){

		//0じゃなかったら
		if(num_pictureBook != 0){
		monster.SetActive(true);
		//種族値
		int bs_hp = baseStatsData.sheets[0].list[num_pictureBook-1].Hp;
		int bs_attack = baseStatsData.sheets[0].list[num_pictureBook-1].Attack;
		int bs_defense = baseStatsData.sheets[0].list[num_pictureBook-1].Defense;
		int bs_speed = baseStatsData.sheets[0].list[num_pictureBook-1].Speed;
		//Debug.Log(bs_hp + " " + bs_attack + " "+ bs_defense + " "+ bs_speed);
		//データ代入
		monster.GetComponent<CharacterStatus>().level = level;
		monster.GetComponent<CharacterStatus>().type = baseStatsData.sheets[0].list[num_pictureBook-1].Type;
		monster.GetComponent<CharacterStatus>().maxHp = GetActualValue(bs_hp,level,true);
		monster.GetComponent<CharacterStatus>().hp = GetActualValue(bs_hp,level,true);
		monster.GetComponent<CharacterStatus>().attack = GetActualValue(bs_attack,level);
		monster.GetComponent<CharacterStatus>().defense= GetActualValue(bs_defense,level);
		monster.GetComponent<CharacterStatus>().speed = GetActualValue(bs_speed,level);
		monster.GetComponent<CharacterStatus>().deathFlag = false;
		//Debug.Log(GetActualValue(bs_hp,level,true) + " " + GetActualValue(bs_attack,level) + " "+ 
		//	GetActualValue(bs_defense,level) + " "+ GetActualValue(bs_speed,level));

		//モンスターのイラスト変更
		monster.GetComponent<Image>().sprite = Resources.Load<Sprite>("Img_Monster/" + num_pictureBook);

		//属性の色に変更
		GameObject icon_type = monster.transform.GetChild(0).GetChild(3).gameObject;
		icon_type.GetComponent<Image>().color = GetTypeColor(baseStatsData.sheets[0].list[num_pictureBook-1].Type);

		//レベル表示
		GameObject txt_level = icon_type.transform.GetChild(0).gameObject;
		txt_level.GetComponent<Text>().text = level.ToString();

		} else if(num_pictureBook == 0){
			//図鑑番号が0だったら誰もいないということ
			monster.SetActive(false);
			monster.GetComponent<CharacterStatus>().deathFlag = true;
		}
	}

	//実数値取得 かつ 返す
	private int GetActualValue(int bs,int level,bool isHp = false){

		int iv = 31; //個体値(Indivisual Value)
		int ev = 252; //努力値(Effort Value)

		if(isHp){
			int av = (int)((bs*2 + iv + (ev/4)) * level/100 + 10 + level);
			return av;
		}else{
			int av = (int)((bs*2 + iv + (ev/4)) * level/100 + 5);
			return av;
		}
	}

	//入場アニメーション
	private void EntranceAnimation(){
		
		AllyEntranceAnimation();
		
		EnemyEntranceAnimation();

		//バトル開始
		StartCoroutine(DelayMethod(time_entranceAnimation,() => {
			CheckNextAction();
		}));


	}

	private void AllyEntranceAnimation(){
		//味方
		myParty.SetActive(true);
		//変数宣言
		Vector3 nowPos = myParty.GetComponent<RectTransform>().localPosition;
		float disX = 600;
		float disY = 250;
		float angle = 60 * Mathf.PI / 180;
		Vector3[] _path = new Vector3[3];
		_path[0] = new Vector3(nowPos.x + disX/2 + Mathf.Cos(angle)*disX/2,nowPos.y + Mathf.Sin(angle) * disY,nowPos.z);
		_path[1] = new Vector3(nowPos.x + disX/2,nowPos.y + disY,nowPos.z);
		_path[2] = nowPos;
		myParty.GetComponent<RectTransform>().localPosition = new Vector3(nowPos.x + disX,nowPos.y,nowPos.z);
		//iTween.MoveTo(myParty,iTween.Hash("path",_path,"time",time_allyEntranceAnimation,
		//	"easetype",iTween.EaseType.easeInCubic,"isLocal",true));
		Tweener tween = myParty.GetComponent<RectTransform>().DOLocalPath(_path, time_allyEntranceAnimation, PathType.CatmullRom)
			.SetEase(Ease.InQuad);
	}

	private void EnemyEntranceAnimation(){
		//敵
		enemyParty.SetActive(false);
		StartCoroutine(DelayMethod(time_allyEntranceAnimation,()=> {
			enemyParty.SetActive(true);
			for(int i =0 ;i<enemyPartyList.Count;i++){
				enemyPartyList[i].GetComponent<Image>().color = new Color(1,1,1,0);
				enemyPartyList[i].GetComponent<MonsterAnimation>()
					.FadeInAnimation(time_entranceAnimation -time_allyEntranceAnimation);
			}
		}));
	}

	/*=====================================
	 *
	 * バトルの流れ
	 *
	 ======================================*/
	
	//次の行動が味方なのか敵なのか判定
	private void CheckNextAction(){
		counter_checkNextAction = 0;
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
		SetCommandArea(monster);
		SetSelectMarker(monster);
	}

	//コマンドエリアの設定
	private void SetCommandArea(GameObject monster){
		for(int i=0;i<4;i++){
			//変数代入
			GameObject btn_command = commandArea.transform.GetChild(i).gameObject;
			int no_skill = monster.GetComponent<CharacterStatus>().skills[i];
			string name_skill = skillData.sheets[0].list[no_skill-1].Name;
			btn_command.GetComponent<TouchCommandButton>().name_skill = name_skill;
			string type = skillData.sheets[0].list[no_skill-1].Type;
			//技説明文
			btn_command.GetComponent<TouchCommandButton>().explain 
				= skillData.sheets[0].list[no_skill-1].Explain;
			//名前変更
			btn_command.transform.GetChild(0).gameObject.GetComponent<Text>().text = name_skill;
			//色変更
			Image img = btn_command.GetComponent<Image>();
			//img.color = GetTypeColor(type);
			img.sprite = GetSkillBtn(type);
		}
	}

	//技の選択待ち......
	//味方の攻撃
	public void AllyAttack(int num_skill){
		//commandArea.SetActive(false);
		//動くモンスターの特定
		GameObject attackMonster = monsterOrderList[actionCount-1].gameObject;

		//技を設定
		attackMonster.GetComponent<CharacterStatus>().num_selectedSkill = num_skill;
		int no_skill = attackMonster.GetComponent<CharacterStatus>().skills[num_skill - 1];
		SetSelectMarker(attackMonster);

		//攻撃する相手の決定
		List<GameObject> partyList; //参照するパーティリスト
		List<GameObject> remainTargetPartyList = new List<GameObject>(); //生きているターゲットモンスターのリスト
		int count_target = 1; //ターゲットの体数 1->単体 5->全体
		List<GameObject> attackedMonsters = new List<GameObject>(); //攻撃対象のモンスター

		//技のターゲットによって分類
		switch(skillData.sheets[0].list[no_skill-1].Target){
		case "SingleEnemy":
			count_target = 1;
			partyList = enemyPartyList;
			break;
		case "WholeEnemy":
			count_target = 5;
			partyList = enemyPartyList;
			break;
		case "SingleAlly":
			count_target = 1;
			partyList = myPartyList;
			break;
		case "WholeAlly":
			count_target = 5;
			partyList = myPartyList;
			break;
		default:
			count_target = 1;
			partyList = enemyPartyList;
			break;
		}

		for(int i=0;i<partyList.Count;i++){
			if(!partyList[i].GetComponent<CharacterStatus>().deathFlag){
				//生きていればListに追加
				remainTargetPartyList.Add(partyList[i]);
			}
		}
		remainTargetPartyList = remainTargetPartyList.OrderBy(a => Guid.NewGuid()).ToList(); //シャッフル

		//attackedMonsterの決定
		if(count_target == 1){
			//単体
			count_targetMonster = 1;
			//ターゲットしているかどうか,対象が敵か味方かで分類
			switch(skillData.sheets[0].list[no_skill-1].Target){
			case "SingleEnemy":
				if(targetMonsterId_enemy == 0){
				//誰も選択していない
				attackedMonsters.Add(remainTargetPartyList[0]);
				}else{
					//ターゲットしている
					attackedMonsters.Add(enemyPartyList[targetMonsterId_enemy - 6]);
				}
				break;
			case "SingleAlly":
				if(targetMonsterId_ally == 0){
					//誰も選択していない
					attackedMonsters.Add(remainTargetPartyList[0]);
				}else{
					//ターゲットしている
					attackedMonsters.Add(myPartyList[targetMonsterId_ally-1]);
				}
				break;
			default:
				if(targetMonsterId_enemy == 0){
					//誰も選択していない
					attackedMonsters.Add(remainTargetPartyList[0]);
				}else{
					//ターゲットしている
					attackedMonsters.Add(enemyPartyList[targetMonsterId_enemy - 6]);
				}
				break;
			}
		}else{
			//全体
			count_targetMonster = remainTargetPartyList.Count;
			attackedMonsters = remainTargetPartyList;
		}

		//攻撃
		Attack(attackMonster,attackedMonsters);
	}

	//敵の行動
	private void EnemyAction(){

		Debug.Log("セットする前");

		//動くモンスターの特定
		GameObject attackMonster = monsterOrderList[actionCount-1].gameObject;
		SetSelectMarker(attackMonster);

		Debug.Log("セットした後");

		//技の決定
		List<int> skillList = attackMonster.GetComponent<CharacterStatus>().skills;
		int num_skill = UnityEngine.Random.Range(0,4); //持ち技の中の何番目の技か
		attackMonster.GetComponent<CharacterStatus>().num_selectedSkill = num_skill + 1;
		int no_skill = skillList[num_skill]; //放つ技の技番号

		Debug.Log("1");

		//攻撃する相手の決定
		List<GameObject> partyList; //参照するパーティリスト
		List<GameObject> remainTargetPartyList = new List<GameObject>(); //生きているターゲットモンスターのリスト
		int count_target = 1; //ターゲットの体数 1->単体 5->全体
		List<GameObject> attackedMonsters = new List<GameObject>(); //攻撃対象のモンスター

		Debug.Log("2");

		//技のターゲットによって分類
		switch(skillData.sheets[0].list[no_skill-1].Target){
		case "SingleEnemy":
			count_target = 1;
			partyList = myPartyList;
			break;
		case "WholeEnemy":
			count_target = 5;
			partyList = myPartyList;
			break;
		case "SingleAlly":
			count_target = 1;
			partyList = enemyPartyList;
			break;
		case "WholeAlly":
			count_target = 5;
			partyList = enemyPartyList;
			break;
		default:
			count_target = 1;
			partyList = myPartyList;
			break;
		}

		Debug.Log("3");

		for(int i=0;i<partyList.Count;i++){
			if(!partyList[i].GetComponent<CharacterStatus>().deathFlag){
				//生きていればListに追加
				remainTargetPartyList.Add(partyList[i]);
			}
		}
		remainTargetPartyList = remainTargetPartyList.OrderBy(a => Guid.NewGuid()).ToList(); //シャッフル

		Debug.Log("4");

		//attackedMonsterの決定
		if(count_target == 1){
			//単体
			count_targetMonster = 1;
			attackedMonsters.Add(remainTargetPartyList[0]);
		}else{
			//全体
			count_targetMonster = remainTargetPartyList.Count;
			attackedMonsters = remainTargetPartyList;
		}

		//攻撃
		StartCoroutine(DelayMethod(time_break,() => {
			Debug.Log("攻撃");
			Attack(attackMonster,attackedMonsters);
		}));
	}

	private void SetSelectMarker(GameObject monster){
		Vector3 pos = canvas.transform.InverseTransformPoint(monster.GetComponent<RectTransform>().position);
		selectMarker.SetActive(true);
		selectMarker.GetComponent<RectTransform>().localPosition 
			= new Vector3(pos.x,pos.y-135,pos.z);
	}

	//攻撃
	private void Attack(GameObject attackMonster,List<GameObject> attackedMonsters){
		if(vod != 0)return; //もし勝敗がついてたらreturn

		//スキル名表示
		int num_selectedSkill = attackMonster.GetComponent<CharacterStatus>().num_selectedSkill;
		int no_skill = attackMonster.GetComponent<CharacterStatus>().skills[num_selectedSkill-1];
		DisplaySkillNameTxt(attackMonster,no_skill);

		//前に出る
		StepFront(attackMonster,attackedMonsters);

	}

	//前に出る
	private void StepFront(GameObject attackMonster,List<GameObject> attackedMonsters){
		//移動
		iTween.MoveAdd(attackMonster,iTween.Hash("x",-1 * diff_step,"time",time_stepFront,"easeType",iTween.EaseType.linear));

		//攻撃
		StartCoroutine(DelayMethod(time_stepFront,() => {
			for(int i=0;i<attackedMonsters.Count;i++){
				AttackAnimation(attackMonster,attackedMonsters[i]);
			}
		}));
	}

	//攻撃アニメーション
	private void AttackAnimation(GameObject attackMonster,GameObject attackedMonster){

		//技の取得
		int num_selectedSkill = attackMonster.GetComponent<CharacterStatus>().num_selectedSkill;
		int no_skill = attackMonster.GetComponent<CharacterStatus>().skills[num_selectedSkill-1]; //１~>


		//ParticleSystemの設定
		GameObject atkFXPrefab;
		
		int num_texture = skillEffectData.sheets[0].list[no_skill-1].Texture;
		Texture texture = (Texture)Resources.Load("Img_Effect/pipo-btleffect" + num_texture.ToString("D3"));
		int x = skillEffectData.sheets[0].list[no_skill-1].x;
		int y = skillEffectData.sheets[0].list[no_skill-1].y;
		//float scale = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].scale;
		//float speed = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].speed;
		
		//マテリアルの画像
		m_skillEffect.mainTexture = texture;
		//atkFXの決定
		if(x == 8){
			atkFXPrefab = atkFX_x8Prefab;
		}else if(x == 10){
			atkFXPrefab = atkFX_x10Prefab;
		}else{
			atkFXPrefab = atkFX_x10Prefab;
		}
		
		//particlesystemの決定
		GameObject atkFX = (GameObject)Instantiate(atkFXPrefab);
		ParticleSystem ps = atkFX.GetComponent<ParticleSystem>();
		//位置
		Vector3 mPos = attackedMonster.GetComponent<RectTransform>().position;
		atkFX.GetComponent<Transform>().position 
			= new Vector3(mPos.x,mPos.y,zPos_attackEffect);
		//scale
		//atkFX.GetComponent<Transform>().localScale = new Vector3(scale,scale,1);
		ps.Play();

		StartCoroutine(DelayMethod(time_attackAnimation,() => {
				HitAnimation(attackMonster,attackedMonster);
		}));
		
	}

	//被弾アニメーション
	private void HitAnimation(GameObject attackMonster,GameObject attackedMonster){
		//変数代入
		int num_selectedSkill = attackMonster.GetComponent<CharacterStatus>().num_selectedSkill;
		int no_skill = attackMonster.GetComponent<CharacterStatus>().skills[num_selectedSkill - 1];
		String speceis = skillData.sheets[0].list[no_skill - 1].Species; //技の種類

		//技の種類によって分類
		switch(speceis){
		case "Damage":
			DamageSkill(attackMonster,attackedMonster,no_skill);
			break;
		case "Heal":
			HealSkill(attackMonster,attackedMonster,no_skill);
			break;
		case "Change":
			ChangeSkill(attackMonster,attackedMonster,no_skill);
			break;
		}
	}

	//攻撃技
	private void DamageSkill(GameObject attackMonster,GameObject attackedMonster,int no_skill){
		float skillDamage = skillData.sheets[0].list[no_skill-1].Damage; //技のダメージ
		float attack = attackMonster.GetComponent<CharacterStatus>().attack
			* (1 + attackMonster.GetComponent<CharacterStatus>().statusRank_attack/2.0f); //攻撃するモンスターの攻撃力
		float defense = attackedMonster.GetComponent<CharacterStatus>().defense
			* (1 + attackedMonster.GetComponent<CharacterStatus>().statusRank_defense/2.0f); //攻撃されるモンスターの防御力
		
		string type_attackMonster = attackMonster.GetComponent<CharacterStatus>().type;
		string type_skill = skillData.sheets[0].list[no_skill-1].Type;
		string type_attackedMonster = attackedMonster.GetComponent<CharacterStatus>().type;

		float attributeMatch = (type_attackMonster == type_skill) ? 1.5f : 1.0f; //タイプ一致
		float attributeAffinity = GetTypeAttribute(type_skill,type_attackedMonster); //タイプ相性

		int damage = (int)(22 * skillDamage * attack / defense / 50 * attributeMatch * attributeAffinity);

		//hp計算
		int originalHp = attackedMonster.GetComponent<CharacterStatus>().hp;
		attackedMonster.GetComponent<CharacterStatus>().hp -= damage;
		int hp = attackedMonster.GetComponent<CharacterStatus>().hp;

		//ダメージテキスト表示
		DisplayDamageText(attackedMonster,damage,attributeAffinity);

		//スライダー
		attackedMonster.transform.Find("Slider").gameObject
			.GetComponent<SliderAnimation>().Animation(originalHp,hp,time_hitAnimation);

		//被弾モーション
		StartCoroutine("Blink",attackedMonster);

		//JudgeDeath
		StartCoroutine(DelayMethod(time_hitAnimation,() => {
			JudgeDeath(attackMonster,attackedMonster,hp);
		}));
	}

	private float GetTypeAttribute(string type_attack,string type_attacked){

		int int_attackType = 0;
		int int_attackedType = 0;
		foreach (Type value in Enum.GetValues(typeof(Type)))
		{
			string name = Enum.GetName(typeof(Type), value);
			if(name == type_attack)int_attackType = (int)value;
			if(name == type_attacked)int_attackedType = (int)value;
		}

		int effective = 0;
		switch(int_attackedType){
		case 0:
			effective = typeEffectiveness.sheets[0].list[int_attackType].Fire;
			break;
		case 1:
			effective = typeEffectiveness.sheets[0].list[int_attackType].Water;
			break;
		case 2:
			effective = typeEffectiveness.sheets[0].list[int_attackType].Grass;
			break;
		case 3:
			effective = typeEffectiveness.sheets[0].list[int_attackType].Lightning;
			break;
		case 4:
			effective = typeEffectiveness.sheets[0].list[int_attackType].Darkness;
			break;
		}

		if(effective == 0){
			return 1.0f;
		}else if(effective == 1){
			return 2.0f;
		}else{
			return 0.5f;
		}
	}

	//回復技
	private void HealSkill(GameObject attackMonster,GameObject attackedMonster,int no_skill){
		int maxHp = attackedMonster.GetComponent<CharacterStatus>().maxHp;
		int healAmount = skillData.sheets[0].list[no_skill-1].HealAmount; //回復量（%）
		healAmount = (int)(maxHp * healAmount / 100); //回復量（実数値） 

		//hp計算
		int originalHp = attackedMonster.GetComponent<CharacterStatus>().hp;
		int currentHp = originalHp + healAmount;
		currentHp = (currentHp >= maxHp) ? maxHp : currentHp; //最大値を超えないように
		attackedMonster.GetComponent<CharacterStatus>().hp = currentHp;

		//Healテキスト表示
		DisplayHealText(attackedMonster,healAmount);

		//スライダー
		attackedMonster.transform.Find("Slider").gameObject
			.GetComponent<SliderAnimation>().Animation(originalHp,currentHp,time_hitAnimation);

		//JudgeDeath
		StartCoroutine(DelayMethod(time_hitAnimation,() => {
			JudgeDeath(attackMonster,attackedMonster,currentHp);
		}));
	}

	//変化技
	private void ChangeSkill(GameObject attackMonster,GameObject attackedMonster,int no_skill){
		//変数代入
		int changeAmount = skillData.sheets[0].list[no_skill - 1].ChangeAmount; //変化量
		bool isUp = changeAmount >= 0 ? true : false; //上昇なのか減少なのか
		String changeStatus = skillData.sheets[0].list[no_skill-1].ChangeStatus; //変化するステータス

		//実際にステータス変化
		switch(changeStatus){
		case "Attack":
			attackedMonster.GetComponent<CharacterStatus>().statusRank_attack += changeAmount;
			break;
		case "Defense":
			attackedMonster.GetComponent<CharacterStatus>().statusRank_defense += changeAmount;
			break;
		case "Speed":
			attackedMonster.GetComponent<CharacterStatus>().statusRank_speed += changeAmount;
			break;
		}

		//hp計算
		int hp = attackedMonster.GetComponent<CharacterStatus>().hp;

		//変化技テキスト表示
		DisplayChangeText(attackedMonster,changeStatus,isUp);

		//ステータスアイコンの表示
		DisplayStatusIcon(attackedMonster);

		//JudgeDeath
		StartCoroutine(DelayMethod(time_hitAnimation,() => {
			JudgeDeath(attackMonster,attackedMonster,hp);
		}));
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

	private void JudgeDeath(GameObject attackMonster,GameObject attackedMonster, int hp){
		//死んだモンスターがいるかどうか
		bool existDeathMonster = false;
		counter_checkNextAction++;

		//死亡判定
		if(hp <= 0){
			//死亡
			attackedMonster.GetComponent<CharacterStatus>().deathFlag = true;
			/*
			StartCoroutine(DelayMethod(time_deathAnimation,() => {
				attackedMonster.SetActive(false);
			}));
			*/

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
			//死亡アニメーション 
			attackedMonster.GetComponent<MonsterAnimation>().DeathAnimation(time_deathAnimation);
			existDeathMonster = true;
		}
		
		if(counter_checkNextAction == count_targetMonster){
			if(existDeathMonster){
				//死んだモンスターがいる
				//ステップバック
				StartCoroutine(DelayMethod(time_deathAnimation,() => {
					StepBack(attackMonster);
				}));
			}else{
				//死んだモンスターがいない
				//ステップバック
				StepBack(attackMonster);
			}
		}
	}

	//ステップバック
	private void StepBack(GameObject attackMonster){
		//移動
		iTween.MoveAdd(attackMonster,iTween.Hash("x",diff_step,"time",time_stepBack,"easeType",iTween.EaseType.linear));

		StartCoroutine(DelayMethod(time_stepBack,() => {
			JudgeVOD();
		}));
	}

	//勝敗判定
	private void JudgeVOD(){
		int count = 0;
		for(int i=0;i<enemyPartyList.Count;i++){
			if(!enemyPartyList[i].GetComponent<CharacterStatus>().deathFlag)count++;//いきてたらカウントアップ
		}
		if(count == 0){
			if(nowWave == totalWave){
				//最後のWaveならバトル終了
				vod = 1;
				GameSet(); //バトル終了
			}else{
				//そうじゃないなら次のWave
				GoNextWave();
			}
			return;
		}

		count = 0;
		for(int i=0;i<myPartyList.Count;i++){
			if(!myPartyList[i].GetComponent<CharacterStatus>().deathFlag)count++;//いきてたらカウントアップ
		}
		if(count == 0){
			vod = -1;
			GameSet(); //バトル終了
			return;
		}
		CheckNextAction(); //最後の行動の時に次の行動にいく
	}

	//次のWaveに移動
	private void GoNextWave(){

		nowWave++;

		//InputMonsterList();
		//GetMyPartyData();
		GetEnemyPartyData();
		//InitializeSlider(myPartyList);
		InitializeSlider(enemyPartyList);
		SortMonsterOrder();

		NextWaveAnimation();

		enemyParty.SetActive(false);
		StartCoroutine(DelayMethod(time_nextWaveAnimation,() => {
			enemyParty.SetActive(true);
			EnemyEntranceAnimation();
			//バトル開始
			StartCoroutine(DelayMethod(time_entranceAnimation,() => {
				CheckNextAction();
			}));
		}));
	}

	//次のWaveにいく時のアニメーション
	private void NextWaveAnimation(){
		GameObject bg1 = containerBG.transform.GetChild(0).gameObject;
		GameObject bg2 = containerBG.transform.GetChild(1).gameObject;
		Vector3 pos1 = bg1.GetComponent<RectTransform>().localPosition;
		Vector3 pos2 = bg2.GetComponent<RectTransform>().localPosition;
		float width = bg1.GetComponent<RectTransform>().sizeDelta.x;

		iTween.MoveTo(bg1,iTween.Hash("position",new Vector3(pos1.x + width,pos1.y,pos1.z)
			,"time",time_nextWaveAnimation,"islocal",true,"easetype",iTween.EaseType.linear));
		iTween.MoveTo(bg2,iTween.Hash("position",new Vector3(pos2.x + width,pos2.y,pos2.z)
			,"time",time_nextWaveAnimation,"islocal",true,"easetype",iTween.EaseType.linear));

		//コールバック
		StartCoroutine(DelayMethod(time_nextWaveAnimation + 0.1f,() => {
			bg1.GetComponent<RectTransform>().localPosition = pos2;
			bg1.transform.SetSiblingIndex(1);
		}));

	}

	//ゲーム終了
	private void GameSet(){
		commandArea.SetActive(false);
		bg_VOD.SetActive(true);
		iTween.MoveFrom(bg_VOD,iTween.Hash("x",-10,"easytype",iTween.EaseType.easeInQuint,"time",0.8f,
			"oncompletetarget",gameObject,"oncomplete","DisplayVOD"));
		//勝敗情報を保存
		SaveData.SetInt("vod",vod);
		SaveData.Save();

		StartCoroutine(DelayMethod(3,() => {
			//resultScreen.SetActive(true);
			float fProbabilityRate = UnityEngine.Random.value * 100.0f;
			if(fProbabilityRate < 100){
				FadeManager.Instance.LoadScene ("CaptureScene", 1.0f);
			}else{
				FadeManager.Instance.LoadScene ("ResultScene", 1.0f);
			}
		}));
	}

	private void DisplayVOD(){
		if(vod == 1){
			//勝利
			GameObject txt = bg_VOD.transform.Find("txt_VOD").gameObject;
			txt.SetActive(true);
			txt.GetComponent<TextMeshProUGUI>().text = "WIN!!!";
			txt.GetComponent<TextMeshProUGUI>().color = new Color(0.976f,1,0.1647f);
			//アニメーション

		}else{
			//敗北
			GameObject txt = bg_VOD.transform.Find("txt_VOD").gameObject;
			txt.SetActive(true);
			txt.GetComponent<TextMeshProUGUI>().text = "LOSE...";
			txt.GetComponent<TextMeshProUGUI>().color = new Color(0.49f,0.1f,0.86f);
			//アニメーション
		}
	}

	/*=====================================
	 *
	 * テキスト表示
	 *
	 ======================================*/

	//技名表示
	private void DisplaySkillNameTxt(GameObject attackMonster,int no_skill){
		//変数代入
		Vector3 posMons = canvas.transform.InverseTransformPoint(attackMonster.GetComponent<RectTransform>().position);
		string name_skill = skillData.sheets[0].list[no_skill-1].Name; //技のダメージ
		//txtPrefabの設定
		GameObject txt = (GameObject)Instantiate(txt_skillNamePrefab);
		txt.transform.SetParent(canvas.transform);
		txt.GetComponent<RectTransform>().localPosition 
			= new Vector3(posMons.x,posMons.y - 40,posMons.z);
		//txt.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
		txt.GetComponent<Text>().text = name_skill;
	}

	//ダメージ表示
	private void DisplayDamageText(GameObject attackedMonster,int damage,float typeEffectiveness){
		//変数代入
		Vector3 posMons = canvas.transform.InverseTransformPoint(attackedMonster.GetComponent<RectTransform>().position);
		//txt_damagePrefabの設定
		GameObject txt = (GameObject)Instantiate(txt_damagePrefab);
		txt.transform.SetParent(canvas.transform);
		txt.GetComponent<RectTransform>().localPosition 
			= new Vector3(posMons.x,posMons.y,posMons.z);
		txt.GetComponent<Text>().text = damage.ToString();
		txt.GetComponent<Text>().color = new Color(1,0.75f,0.175f);

		//タイプ相性
		GameObject txt_typeEffectiveness = txt.transform.GetChild(0).gameObject;
		if(typeEffectiveness < 1){
			//抜群
			txt_typeEffectiveness.SetActive(true);
			txt_typeEffectiveness.GetComponent<Text>().text = "イマイチ";
			txt_typeEffectiveness.GetComponent<Text>().color = new Color(0.243f,0.439f,0.773f);

		}else if(typeEffectiveness > 1){
			//抜群
			txt_typeEffectiveness.SetActive(true);
			txt_typeEffectiveness.GetComponent<Text>().text = "弱点！";
			txt_typeEffectiveness.GetComponent<Text>().color = new Color(0.820f,0.365f,0);
		}else{
			//普通
			txt_typeEffectiveness.SetActive(false);
		}
	}

	//回復量表示
	private void DisplayHealText(GameObject attackedMonster,int healAmount){
		//変数代入
		Vector3 posMons = canvas.transform.InverseTransformPoint(attackedMonster.GetComponent<RectTransform>().position);
		//txt_healPrefabの設定
		GameObject txt = (GameObject)Instantiate(txt_healPrefab);
		txt.transform.SetParent(canvas.transform);
		txt.GetComponent<RectTransform>().localPosition 
			= new Vector3(posMons.x,posMons.y,posMons.z);
		txt.GetComponent<Text>().text = healAmount.ToString();
		txt.GetComponent<Text>().color = new Color(0.351f,1,0.176f);
	}

	//変化技テキスト表示
	private void DisplayChangeText(GameObject attackedMonster,string changeStatus,bool isUp){
		//変数代入
		Vector3 posMons = canvas.transform.InverseTransformPoint(attackedMonster.GetComponent<RectTransform>().position);
		String str_changeStatus = "";
		switch(changeStatus){
		case "Hp":
			str_changeStatus = "体力";
			break;
		case "Attack":
			str_changeStatus = "攻撃力";
			break;
		case "Defense":
			str_changeStatus = "防御力";
			break;
		case "Speed":
			str_changeStatus = "スピード";
			break;
		}
		String str_isUp = isUp ? "上昇" : "減少";

		//txt_changePrefabの設定
		GameObject txt = (GameObject)Instantiate(txt_changePrefab);
		txt.transform.SetParent(canvas.transform);
		txt.GetComponent<RectTransform>().localPosition 
			= new Vector3(posMons.x,posMons.y,posMons.z);
		txt.GetComponent<Text>().text = str_changeStatus + str_isUp;
		txt.GetComponent<Text>().color = isUp ? 
			new Color(0.519f,0.85f,0.925f) : new Color(0.726f,0.202f,0.677f);
	}

	void DisplayStatusIcon(GameObject attackedMonster){
		//変数宣言、代入
		GameObject container_statusicon = attackedMonster.transform.Find("container_statusicon").gameObject;
		int rank;
		GameObject icon;
		//Attack
		rank = attackedMonster.GetComponent<CharacterStatus>().statusRank_attack;
		if(rank > 0){
			//正のランク補正
			icon = container_statusicon.transform.GetChild(0).gameObject;
			container_statusicon.transform.GetChild(1).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}else if(rank == 0){
			container_statusicon.transform.GetChild(0).gameObject.SetActive(false);
			container_statusicon.transform.GetChild(1).gameObject.SetActive(false);
		}else {
			//負のランク補正
			icon = container_statusicon.transform.GetChild(1).gameObject;
			container_statusicon.transform.GetChild(0).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}

		//Defense
		rank = attackedMonster.GetComponent<CharacterStatus>().statusRank_defense;
		if(rank > 0){
			//正のランク補正
			icon = container_statusicon.transform.GetChild(2).gameObject;
			container_statusicon.transform.GetChild(3).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}else if(rank == 0){
			container_statusicon.transform.GetChild(2).gameObject.SetActive(false);
			container_statusicon.transform.GetChild(3).gameObject.SetActive(false);
		}else {
			//負のランク補正
			icon = container_statusicon.transform.GetChild(3).gameObject;
			container_statusicon.transform.GetChild(2).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}

		//Speed
		rank = attackedMonster.GetComponent<CharacterStatus>().statusRank_speed;
		if(rank > 0){
			//正のランク補正
			icon = container_statusicon.transform.GetChild(4).gameObject;
			container_statusicon.transform.GetChild(5).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}else if(rank == 0){
			container_statusicon.transform.GetChild(4).gameObject.SetActive(false);
			container_statusicon.transform.GetChild(5).gameObject.SetActive(false);
		}else {
			//負のランク補正
			icon = container_statusicon.transform.GetChild(5).gameObject;
			container_statusicon.transform.GetChild(4).gameObject.SetActive(false);
			icon.SetActive(true);
			icon.transform.GetChild(0).gameObject.GetComponent<Text>().text = rank.ToString();
		}
	}

	/*=====================================
	 *
	 * バトル以外
	 *
	 ======================================*/

	//属性カラー取得
	private Color GetTypeColor(string type){
		switch(type){
		case "Normal":
			return new Color(0.736f,0.736f,0.736f);
		case "Fire":
			return new Color(0.925f,0.280f,0.214f);
		case "Water":
			return new Color(0.212f,0.368f,0.925f);
		case "Grass":
			return new Color(0.338f,0.868f,0.168f);
		case "Lightning":
			return new Color(0.900f,0.915f,0.324f);
		case "Darkness":
			return new Color(0.858f,0.150f,0.722f);
		default:
			return new Color(0.736f,0.736f,0.736f);
		}
	}

	//属性にあったスキルボタンの取得
	private Sprite GetSkillBtn(string type){
		switch(type){
		case "Normal":
			return Resources.Load<Sprite>("Img_SkillButton/" + 6);
		case "Fire":
			return Resources.Load<Sprite>("Img_SkillButton/" + 1);
		case "Water":
			return Resources.Load<Sprite>("Img_SkillButton/" + 2);
		case "Grass":
			return Resources.Load<Sprite>("Img_SkillButton/" + 3);
		case "Lightning":
			return Resources.Load<Sprite>("Img_SkillButton/" + 4);
		case "Darkness":
			return Resources.Load<Sprite>("Img_SkillButton/" + 5);
		default:
			return Resources.Load<Sprite>("Img_SkillButton/" + 6);
		}
	}
	
	//ターゲットマーカーの表示非表示
	public void TouchMonster(GameObject monster){
		int battleId = monster.GetComponent<CharacterStatus>().battleId;
		if(battleId <= 5){
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

	/*=====================================
	 *
	 * セーブデータ
	 *
	 ======================================*/

	private void GetSaveData(){
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
		partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];

		nowStoryQuest = SaveData.GetList<int>("nowStoryQuest",nowStoryQuest);
		islandNum = nowStoryQuest[0];
		int questNum = nowStoryQuest[1];
		battleQuestNum = 0;
		for(int i=0;i<questNum;i++){
			if(storyQuestGeneralData.sheets[islandNum-1].list[i].Category == "Battle"){
				battleQuestNum++;
			}
		}

		questName = storyQuestGeneralData.sheets[islandNum-1].list[questNum-1].Name;

		Debug.Log("battleQuestNum : " + battleQuestNum);
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
