using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState{
	TurnStart,
	EnemyAttack,
	PlayerAttack,
	Command,
	Win,
	Lose
}

public class BattleManager : MonoBehaviour {

	public List<GameObject> player;
	public List<GameObject> enemy;

	[SerializeField] private GameObject WinCanvas;
	[SerializeField] private GameObject LoseCanvas;

	public GameObject enemyImage;
	public GameObject myImage;

	public GameObject commandButton;

	private GameState battleGameState = GameState.TurnStart;
	//public Slider playerSlider;
	//public Slider enemySlider;

	private int skillPower = 80; 

	private int attackNum = 0;
	private int enemyAttackNum = 0;
	private int playerAttackNum = 0;

	private int selectedEnemyNum = 0;


	List<GameObject> sortedMonsters = new List<GameObject>();
	

	// Use this for initialization
	void Start () {
		for(int i =0;i<enemy.Count;i++){
			enemy[i].transform.GetChild(2).GetComponent<Text>().text = "Lv."+enemy[i].transform.GetChild(0).GetComponent<Monster>().Level;
			enemy[i].transform.GetChild(1).GetComponent<Slider>().maxValue = enemy[i].transform.GetChild(0).GetComponent<Monster>().Hp;
			enemy[i].transform.GetChild(1).GetComponent<Slider>().value = enemy[i].transform.GetChild(0).GetComponent<Monster>().Hp;
		}

		for(int i =0;i<player.Count;i++){
			player[i].transform.GetChild(2).GetComponent<Text>().text = "Lv."+player[i].transform.GetChild(0).GetComponent<Monster>().Level;
			player[i].transform.GetChild(1).GetComponent<Slider>().maxValue = player[i].transform.GetChild(0).GetComponent<Monster>().Hp;
			player[i].transform.GetChild(1).GetComponent<Slider>().value = player[i].transform.GetChild(0).GetComponent<Monster>().Hp;
		}

		Sort();

		if(sortedMonsters[0].GetComponent<Monster>().isEnemy){
			StartCoroutine("EnemyAttack",0);
		}else{
			//コマンド表示
			battleGameState = GameState.PlayerAttack;
			commandButton.SetActive(true);
			enemyImage.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(battleGameState != GameState.Win && battleGameState != GameState.Lose){
			if(battleGameState == GameState.EnemyAttack){
				StartCoroutine ("EnemyAttack",attackNum);
				enemyImage.SetActive(true);
				battleGameState = GameState.PlayerAttack;
			}
		}

		if (Input.GetMouseButtonDown(0)) {
    		Vector2 tapPoint  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    		Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
    		if (collition2d) {
        		RaycastHit2D hitObject = Physics2D.Raycast(tapPoint,-Vector2.up);
        		if (hitObject) {
					for(int i = 0;i<enemy.Count;i++){
						Color color = enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
						color.a = 0;
						enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = color;

						if(hitObject.Equals(enemy[i])){
							selectedEnemyNum = i;
							Debug.Log("selectedEnemyNum:"+selectedEnemyNum);
						}
					}
					Color col = hitObject.collider.gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().color;
					col.a = 255;
					hitObject.collider.gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().color = col;
					
        		}
    		}
		}
	}

	void Sort(){
		Dictionary<GameObject, int> monsterSortDic = new Dictionary<GameObject, int>();
		for(int i = 0;i<player.Count;i++){
			monsterSortDic.Add(player[i].transform.GetChild(0).gameObject,player[i].transform.GetChild(0).GetComponent<Monster>().Spd); 
		}
		for(int i = 0;i<enemy.Count;i++){
			monsterSortDic.Add(enemy[i].transform.GetChild(0).gameObject,enemy[i].transform.GetChild(0).GetComponent<Monster>().Spd);
		}
		var sortedMonstersDic = monsterSortDic.OrderByDescending((x) => x.Value);
		foreach(var v in sortedMonstersDic){
			sortedMonsters.Add(v.Key);
		}
	}

	//[{(攻撃側のレベル × 2 ÷ 5 + 2) × (威力 × 攻撃側の攻撃or特攻 ÷ 防御側の防御or特防) ÷ 50 + 2} × 乱数幅(0.85,0.86,...0.99,1.00)] × 一致補正など

	private IEnumerator EnemyAttack(int order) {  
        yield return new WaitForSeconds (1f);  
		Debug.Log("敵の攻撃！");

		//ダメージ計算
		int rand = Random.Range(1, player.Count+1)-1;
		int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
			((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/player[rand].transform.GetChild(0).GetComponent<Monster>().Def/50)+2));
		Debug.Log("プレイヤーに"+damage+"ダメージ！");
		player[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;

		//コマンド表示
		commandButton.SetActive(true);
		enemyImage.SetActive(false);

		//攻撃ターン管理
		if(attackNum == player.Count+enemy.Count-1){
			attackNum = 0;
		}else{
			attackNum++;
		}
		if(enemyAttackNum == enemy.Count-1){
			enemyAttackNum = 0;
		}else{
			enemyAttackNum++;
		}


		//敗北判定
		int playerDeathCounter = 0;
		for(int i = 0; i<player.Count;i++){
			if(player[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
				playerDeathCounter++;
			}
		}
		Debug.Log("playerDeathCounter:"+playerDeathCounter);
		if(playerDeathCounter==player.Count){
			battleGameState = GameState.Lose;
			LoseCanvas.SetActive(true);
		}
    }

	public void Attack(){
		StartCoroutine ("PlayerAttack",attackNum);
	}

	private IEnumerator PlayerAttack(int order) {  
        yield return new WaitForSeconds (1f);
		Debug.Log("プレイヤーの攻撃！");

		int rand = Random.Range(1, enemy.Count+1)-1;

		//ダメージ計算
	
		
		if(selectedEnemyNum != 0){
			int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
								((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/enemy[selectedEnemyNum].transform.GetChild(0).GetComponent<Monster>().Def/50)+2));
			Debug.Log("敵に"+damage+"ダメージ！");
			enemy[selectedEnemyNum].transform.GetChild(1).GetComponent<Slider>().value -= damage;
		}else{
			int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
							((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/enemy[rand].transform.GetChild(0).GetComponent<Monster>().Def/50)+2));
			Debug.Log("敵に"+damage+"ダメージ！");
			enemy[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;
		}

		selectedEnemyNum = 0;
		
		for(int i = 0;i<enemy.Count;i++){
			Color col = enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
			col.a = 0;
			enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = col;
		}

		//コマンド表示
		commandButton.SetActive(false);
		battleGameState = GameState.EnemyAttack;

		//勝利判定
		int enemyDeathCounter = 0;
		for(int i = 0; i<enemy.Count;i++){
			if(enemy[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
				enemyDeathCounter++;
				Color color = enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color;
				color.a = 0;
				enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
				enemy[i].GetComponent<CircleCollider2D>().enabled = false;
				enemy.RemoveAt(i);
			}
		}

		Debug.Log("enemyDeathCounter:"+enemyDeathCounter);
		if(enemyDeathCounter==enemy.Count){
			battleGameState = GameState.Win;
			WinCanvas.SetActive(true);
		}
		
		//攻撃ターン管理
		if(attackNum == player.Count+enemy.Count-1){
			attackNum = 0;
		}else{
			attackNum++;
		}
		if(playerAttackNum == player.Count-1){
			playerAttackNum = 0;
		}else{
			playerAttackNum++;
		}
    }
}
