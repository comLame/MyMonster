using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour {

	//エクセルデータ関係
	public BaseStatsData baseStatsData;
	public EnemyPartyData enemyPartyData;
	public MyPartyData myPartyData;
	public SkillData skillData;
	//ここまで
	public GameObject myParty;
	public GameObject enemyParty;
	public GameObject commandArea;
	public GameObject selectMarker;
	public GameObject canvas;
	public GameObject targetMarker_ally;
	public GameObject targetMarker_enemy;
	public GameObject bg_VOD;
	public GameObject atkFX_x8;
	public GameObject atkFX_x10;
	public GameObject skillEffectData;

	private int vod = 0; //勝敗 1->勝ち -1->敗け
	private int targetMonsterId_ally = 0;
	private int targetMonsterId_enemy = 0;
	private int actionCount = 0; //1だったら1体目の行動 10が終わったら次のターン
	private float time_break = 0.5f; //行動と行動の間のブレイク
	private float time_stepFront = 0.12f; //前にちょこっと進む時間
	private float time_stepBack = 0.12f; //後ろに戻る時間
	private float time_attackAnimation = 1.0f; //攻撃モーションのアニメーション時間
	private float time_hitAnimation = 1.0f; //被弾モーションのアニメーション時間
	private float time_deathAnimation = 1.0f; //死亡アニメーション時間
	private float time_frame = 0.2f; //エフェクトの１枚ずつの表示時間
	private int num_frame = 5; //１エフェクトのフレーム数
	private float speed_attackAnimation = 1f; //攻撃アニメーションのスピード
	private float diff_step = 0.3f; //ステップの距離
	private float zPos_attackEffect = -3f; //攻撃エフェクトのz座標
	private List<GameObject> monsterOrderList = new List<GameObject>();
	private List<GameObject> myPartyList = new List<GameObject>();
	private List<GameObject> enemyPartyList = new List<GameObject>();
	private Slider animateSlider; //アニメーションするスライダー
	private GameObject privateGameObject; //一時的に使いたい時のGameObject

	// Use this for initialization
	void Start () {
		InputMonsterList();
		GetMyPartyData();
		GetEnemyPartyData();
		InitializeSlider(myPartyList);
		InitializeSlider(enemyPartyList);
		SortMonsterOrder();
		DebugMonsterOrderList();
		CheckNextAction();
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
		monsterOrderList.Sort((a,b) => b.GetComponent<CharacterStatus>().speed - a.GetComponent<CharacterStatus>().speed  );
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
			int defence = monsterOrderList[i].GetComponent<CharacterStatus>().defense;
			int speed = monsterOrderList[i].GetComponent<CharacterStatus>().speed;
			//Debug.Log("B_ID:" + battleId + " Lv."+ level + " " + name + " " + hp + " "+ attack + " "+ defence + " " + speed + " ");
		}
	}

	/*=====================================
	 *
	 * データ入力
	 *
	 ======================================*/

	//マイパーティデータを読み込む
	private void GetMyPartyData(){
		int num_total = myPartyData.sheets[0].list.Count; //総数取得
		
		for(int i=0;i<num_total;i++){
			GameObject monster = myPartyList[i];
			int num_pb = myPartyData.sheets[0].list[i].No; //図鑑番号
			GetStatus(monster,num_pb,50); //ステータス代入
			GetSkill(monster,i);
		}
	}

	//エネミーパーティデータを読み込む
	private void GetEnemyPartyData(){
		int num_total = enemyPartyData.sheets[0].list.Count; //総数取得
		
		for(int i=0;i<num_total;i++){
			GameObject monster = enemyPartyList[i];	
			int num_pb = enemyPartyData.sheets[0].list[i].No; //図鑑番号
			GetStatus(monster,num_pb,50); 
			GetSkill(monster,i,true);
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
			no_skill1 = myPartyData.sheets[0].list[num].No_Skill1;
			no_skill2 = myPartyData.sheets[0].list[num].No_Skill2;
			no_skill3 = myPartyData.sheets[0].list[num].No_Skill3;
			no_skill4 = myPartyData.sheets[0].list[num].No_Skill4;
		}else{
			//敵
			no_skill1 = enemyPartyData.sheets[0].list[num].No_Skill1;
			no_skill2 = enemyPartyData.sheets[0].list[num].No_Skill2;
			no_skill3 = enemyPartyData.sheets[0].list[num].No_Skill3;
			no_skill4 = enemyPartyData.sheets[0].list[num].No_Skill4;
		}

		monster.GetComponent<CharacterStatus>().skills[0] = no_skill1;
		monster.GetComponent<CharacterStatus>().skills[1] = no_skill2;
		monster.GetComponent<CharacterStatus>().skills[2]= no_skill3;
		monster.GetComponent<CharacterStatus>().skills[3] = no_skill4;
	}
	
	//ステータス
	private void GetStatus(GameObject monster,int num_pictureBook,int level){
		//種族値
		int bs_hp = baseStatsData.sheets[0].list[num_pictureBook-1].Hp;
		int bs_attack = baseStatsData.sheets[0].list[num_pictureBook-1].Attack;
		int bs_defence = baseStatsData.sheets[0].list[num_pictureBook-1].Defence;
		int bs_speed = baseStatsData.sheets[0].list[num_pictureBook-1].Speed;
		//Debug.Log(bs_hp + " " + bs_attack + " "+ bs_defence + " "+ bs_speed);
		//データ代入
		monster.GetComponent<CharacterStatus>().level = level;
		monster.GetComponent<CharacterStatus>().hp = GetActualValue(bs_hp,level,true);
		monster.GetComponent<CharacterStatus>().attack = GetActualValue(bs_attack,level);
		monster.GetComponent<CharacterStatus>().defense= GetActualValue(bs_defence,level);
		monster.GetComponent<CharacterStatus>().speed = GetActualValue(bs_speed,level);
		//Debug.Log(GetActualValue(bs_hp,level,true) + " " + GetActualValue(bs_attack,level) + " "+ 
		//	GetActualValue(bs_defence,level) + " "+ GetActualValue(bs_speed,level));
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

	/*=====================================
	 *
	 * バトルの流れ
	 *
	 ======================================*/
	
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
			string type = skillData.sheets[0].list[no_skill-1].Type;
			Debug.Log(type);
			//名前変更
			btn_command.transform.GetChild(0).gameObject.GetComponent<Text>().text = name_skill;
			//色変更
			Image img = btn_command.GetComponent<Image>();
			switch(type){
				case "Normal":
					img.color = new Color(0.736f,0.736f,0.736f);
				break;
				case "Fire":
					img.color = new Color(0.925f,0.280f,0.214f);
				break;
				case "Water":
					img.color = new Color(0.212f,0.368f,0.925f);
				break;
				case "Grass":
					img.color = new Color(0.338f,0.868f,0.168f);
				break;
				case "Lightning":
					img.color = new Color(0.900f,0.915f,0.324f);
				break;
				case "Darkness":
					img.color = new Color(0.858f,0.150f,0.722f);
				break;
				default:
				break;
			}
		}
	}

	//技の選択待ち......
	//味方の攻撃
	public void AllyAttack(int num_skill){
		commandArea.SetActive(false);
		//動くモンスターの特定
		GameObject monster = monsterOrderList[actionCount-1].gameObject;
		//技を設定
		monster.GetComponent<CharacterStatus>().num_selectedSkill = num_skill;
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
		StartCoroutine(DelayMethod(time_break,() => {
			Attack(monster,targetMonster);
		}));
	}

	private void SetSelectMarker(GameObject monster){
		Vector3 pos = canvas.transform.InverseTransformPoint(monster.GetComponent<RectTransform>().position);
		selectMarker.SetActive(true);
		selectMarker.GetComponent<RectTransform>().localPosition 
			= new Vector3(pos.x,pos.y-135,pos.z);
	}

	//攻撃
	private void Attack(GameObject attackMonster,GameObject attackedMonster){
		if(vod != 0)return; //もし勝敗がついてたらreturn

		//前に出る
		StepFront(attackMonster,attackedMonster);

	}

	//前に出る
	private void StepFront(GameObject attackMonster,GameObject attackedMonster){
		//移動
		iTween.MoveAdd(attackMonster,iTween.Hash("x",diff_step,"time",time_stepFront,"easeType",iTween.EaseType.linear));

		//攻撃
		StartCoroutine(DelayMethod(time_stepFront,() => {
			AttackAnimation(attackMonster,attackedMonster);
		}));
	}

	//攻撃アニメーション
	private void AttackAnimation(GameObject attackMonster,GameObject attackedMonster){

		//技の取得
		int num_selectedSkill = attackMonster.GetComponent<CharacterStatus>().num_selectedSkill;
		int no_skill = attackMonster.GetComponent<CharacterStatus>().skills[num_selectedSkill-1]; //１~>

		//ParticleSystemの設定
		GameObject atkFX;
		//ParticleSystem ps = attackEffect.GetComponent<ParticleSystem>();
		Material m = skillEffectData.GetComponent<SkillEffectData>().material;
		Texture texture = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].texture;
		int x = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].x;
		int y = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].y;
		float scale = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].scale;
		float speed = skillEffectData.GetComponent<SkillEffectData>()._valueListList[no_skill-1].speed;
		//マテリアルの画像
		m.mainTexture = texture;
		//atkFXの決定
		if(x == 8){
			atkFX = atkFX_x8;
		}else if(x == 10){
			atkFX = atkFX_x10;
		}else{
			atkFX = atkFX_x10;
		}
		//particlesystemの決定
		ParticleSystem ps = atkFX.GetComponent<ParticleSystem>();
		//ps.duration

		Vector3 mPos = attackedMonster.GetComponent<RectTransform>().position;
		//位置
		atkFX.GetComponent<Transform>().position 
			= new Vector3(mPos.x,mPos.y,zPos_attackEffect);
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
		float skillDamage = skillData.sheets[0].list[no_skill-1].Damage; //技のダメージ
		float attack = attackMonster.GetComponent<CharacterStatus>().attack; //攻撃するモンスターの攻撃力
		float defence = attackedMonster.GetComponent<CharacterStatus>().defense; //攻撃されるモンスターの防御力
		float attributeMatch = 1.0f; //タイプ一致
		float attributeAffinity = 1.0f; //タイプ相性

		float damage = 22 * skillDamage * attack / defence / 50 * attributeMatch * attributeAffinity;

		//hp計算
		int originalHp = attackedMonster.GetComponent<CharacterStatus>().hp;
		attackedMonster.GetComponent<CharacterStatus>().hp -= (int)damage;
		int hp = attackedMonster.GetComponent<CharacterStatus>().hp;

		//スライダー
		//attackedMonster.transform.Find("Slider").gameObject.GetComponent<Slider>().value = hp;
		SliderAnimation(attackedMonster.transform.Find("Slider").gameObject.GetComponent<Slider>(),originalHp,hp);

		//被弾モーション
		StartCoroutine("Blink",attackedMonster);

		//ステップバック
		StartCoroutine(DelayMethod(time_hitAnimation,() => {
			JudgeDeath(attackMonster,attackedMonster,hp);
		}));
	}

	//スライダーアニメーション
	private void SliderAnimation(Slider slider,int originalHp,int currentHp){
		animateSlider = slider;
		iTween.ValueTo(gameObject,iTween.Hash("from",originalHp,"to",currentHp,"onupdatetarget",gameObject,
			"onupdate","UpdateSlider","time",time_hitAnimation));
	}

	private void UpdateSlider(float hp){
		animateSlider.value = hp;
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
			DeathAnimation(attackedMonster);
			//ステップバック
			StartCoroutine(DelayMethod(time_deathAnimation,() => {
				StepBack(attackMonster);
			}));
		}else{
			//ステップバック
			StepBack(attackMonster);
		}
	}

	//死亡アニメーション
	private void DeathAnimation(GameObject attackedMonster){
		privateGameObject = attackedMonster;
		iTween.ValueTo(gameObject, iTween.Hash("from",1,"to",0,"time",time_deathAnimation,
			"onupdate","UpdateDeathAnimation","onupdatetarget",gameObject,"easetype",iTween.EaseType.linear));
		StartCoroutine(DelayMethod(time_deathAnimation,() => {
			attackedMonster.SetActive(false);
		}));
	}

	//死亡アニメーションのUpdate
	private void UpdateDeathAnimation(float alfa){
		Color c = privateGameObject.GetComponent<Image>().color;
		privateGameObject.GetComponent<Image>().color = new Color(c.r,c.g,c.b,alfa);
	}

	//ステップバック
	private void StepBack(GameObject attackMonster){
		//移動
		iTween.MoveAdd(attackMonster,iTween.Hash("x",-1*diff_step,"time",time_stepBack,"easeType",iTween.EaseType.linear));

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
			vod = 1;
			GameSet(); //バトル終了
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
		CheckNextAction();
	}

	//ゲーム終了
	private void GameSet(){
		commandArea.SetActive(false);
		bg_VOD.SetActive(true);
		iTween.MoveFrom(bg_VOD,iTween.Hash("x",-10,"easytype",iTween.EaseType.easeInQuint,"time",0.8f,
			"oncompletetarget",gameObject,"oncomplete","DisplayVOD"));
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
	 * バトル以外
	 *
	 ======================================*/
	
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

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
