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

    //playerとenemyのList
    public List<GameObject> player;
    public List<GameObject> enemy;

    //勝利敗北UI
    [SerializeField] private GameObject WinCanvas;
    [SerializeField] private GameObject LoseCanvas;

    //透明色と不透明色定義
    private Color Col0 = new Color(255,255,255,0);
    private Color Col255 = new Color(255,255,255,255);

    //メッセージボックステキスト
    public Text CommandText;
    //勝利時獲得アイテム
    public Text GetItemText;

    //コマンドボタン（はたく）
    public GameObject[] commandButton;

    //ステート管理用
    private GameState battleGameState = GameState.TurnStart;

    //初期の敵の数
    private int firstEnemyNum;

    //技の威力（仮）
    private int skillPower = 80; 

    //今攻撃しているモンスターのインデックス
    private int attackNum = 0;

    //ターゲットされている敵のインデックス
    private int selectedEnemyNum = 0;

    //攻撃のコルーチン中であるかどうか
    private bool Attacking = false;

    private string SkillName;

    //速度順に並べ替えられたモンスターのリスト
    List<GameObject> sortedMonsters = new List<GameObject>();

    // Use this for initialization
    void Start () {

        //最初にいた敵の数
        firstEnemyNum = enemy.Count;
        Debug.Log("firstEnemyNum:"+firstEnemyNum);

        //速度順にならべかえる
        Sort();
        /*
        //最初の攻撃が敵か味方か
        if(sortedMonsters[0].GetComponent<Monster>().isEnemy){
            //敵の攻撃を開始
            battleGameState = GameState.EnemyAttack;
            StartCoroutine("EnemyAttack");
            Debug.Log("EnemyAttack");

        }else{
            //バトルコマンド表示
            battleGameState = GameState.PlayerAttack;
            commandButton.SetActive(true);

            //攻撃しているモンスターの可視化
            for(int i = 0;i<sortedMonsters.Count;i++){
                sortedMonsters[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col0;
            }
            sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col255;

        }*/
        NextAttackMonster();
    }
    // Update is called once per frame
    void Update () {
        /*
        //勝ち負けが決まっていない
        if(battleGameState != GameState.Win && battleGameState != GameState.Lose && battleGameState != GameState.TurnStart){
            if(battleGameState == GameState.EnemyAttack && !Attacking){
                Attacking = true;

                for(int i = 0;i<sortedMonsters.Count;i++){
                    sortedMonsters[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col0;
                }
                sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col255;

                StartCoroutine ("EnemyAttack",attackNum);
                Debug.Log("EnemyAttack");
                
            }else if(battleGameState == GameState.Command && !Attacking){
                Attacking = true;

                for(int i = 0;i<sortedMonsters.Count;i++){
                    sortedMonsters[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col0;
                }
                sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col255;

                commandButton.SetActive(true);
                CommandText.text  = "";
                battleGameState = GameState.PlayerAttack;
                
            }
        }*/

        if (Input.GetMouseButtonDown(0)) {
            Vector2 tapPoint  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
            if (collition2d) {
                RaycastHit2D hitObject = Physics2D.Raycast(tapPoint,-Vector2.up);
                if (hitObject) {
                for(int i = 0;i<enemy.Count;i++){
                    enemy[i].transform.GetChild(4).GetComponent<SpriteRenderer>().color = Col0;
                    if(hitObject.collider==enemy[i].GetComponent<Collider2D>()){
                        selectedEnemyNum = i+1;
                        //Debug.Log("selectedEnemyNum:"+selectedEnemyNum);
                    }
                }
                hitObject.collider.gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Col255;
                }
            }
        }
    }

    //素早さ順にモンスター並べ替え
    void Sort(){
        sortedMonsters = new List<GameObject>();
        Dictionary<GameObject, int> monsterSortDic = new Dictionary<GameObject, int>();
        for(int i = 0;i<player.Count;i++){
            monsterSortDic.Add(player[i],player[i].GetComponent<Monster>().Spd); 
        }
        for(int i = 0;i<enemy.Count;i++){
            monsterSortDic.Add(enemy[i],enemy[i].GetComponent<Monster>().Spd);
        }
        var sortedMonstersDic = monsterSortDic.OrderByDescending((x) => x.Value);
        foreach(var v in sortedMonstersDic){
            sortedMonsters.Add(v.Key);
        }
        Debug.Log("sortedMonstercount"+sortedMonsters.Count);
    }

    //[{(攻撃側のレベル × 2 ÷ 5 + 2) × (威力 × 攻撃側の攻撃or特攻 ÷ 防御側の防御or特防) ÷ 50 + 2} × 乱数幅(0.85,0.86,...0.99,1.00)] × 一致補正など
    private IEnumerator EnemyAttack() {
        CommandText.text = sortedMonsters[attackNum].transform.name+"の攻撃！";
        yield return new WaitForSeconds (1f);  

        Debug.Log(sortedMonsters[attackNum].transform.name+"の攻撃！");

        //ダメージ計算
        int rand = Random.Range(1, player.Count+1)-1;
        int damage = (int)(((sortedMonsters[attackNum].GetComponent<Monster>().Level*2/5)+2)*
        ((skillPower*sortedMonsters[attackNum].GetComponent<Monster>().Atk/player[rand].GetComponent<Monster>().Def/50)+2));
        CommandText.text = player[rand].name+"に"+damage+"ダメージ！";
        player[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;

        //攻撃ターン管理
        if(attackNum == player.Count+enemy.Count-1){
            attackNum = 0;
        }else{
            attackNum++;
        }

        //敗北判定
        for(int i = 0; i<player.Count;i++){
            if(player[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
                player[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col0;
                player.RemoveAt(i);
                Sort();
            }
        }
        
        //Debug.Log("playerCount:"+player.Count);
        if(player.Count==0){
            battleGameState = GameState.Lose;
            LoseCanvas.SetActive(true);
        }

        yield return new WaitForSeconds (1f);  
        NextAttackMonster();
        /* 
        //コマンド表示
        if(sortedMonsters[attackNum].GetComponent<Monster>().isEnemy){
            battleGameState = GameState.EnemyAttack;
        }else{
            battleGameState = GameState.Command;
        }

        yield return new WaitForSeconds (1f);  
        Attacking = false;
        */

    }

    public void NextAttackMonster(){
        if(sortedMonsters[attackNum].GetComponent<Monster>().isEnemy){
            StartCoroutine("EnemyAttack");
        }else{
            for(int i = 0;i<commandButton.Length;i++){
                commandButton[i].SetActive(true);
            }

            //攻撃しているモンスターの可視化
            for(int i = 0;i<sortedMonsters.Count;i++){
                sortedMonsters[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col0;
            }
            sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = Col255;
            //StartCoroutine("PlayerAttack");

            CommandText.text  = "";
        }
    }

    public void Attack(string skillName){
        SkillName = skillName;
        StartCoroutine ("PlayerAttack",attackNum);
        Debug.Log("PlayerAttack");
    }

    private IEnumerator PlayerAttack() { 
        
        CommandText.text = sortedMonsters[attackNum].transform.name+"の"+SkillName+"!";
        for(int i = 0;i<commandButton.Length;i++){
            commandButton[i].SetActive(false);
        }

        yield return new WaitForSeconds (1f);

        int rand = Random.Range(1, enemy.Count+1)-1;

        //ダメージ計算
        if(selectedEnemyNum != 0){
            int damage = (int)(((sortedMonsters[attackNum].GetComponent<Monster>().Level*2/5)+2)*
                ((skillPower*sortedMonsters[attackNum].GetComponent<Monster>().Atk/enemy[selectedEnemyNum-1].GetComponent<Monster>().Def/50)+2));
            CommandText.text = enemy[selectedEnemyNum-1].name+"に"+damage+"ダメージ！";
            enemy[selectedEnemyNum-1].transform.GetChild(1).GetComponent<Slider>().value -= damage;
        }else{
            int damage = (int)(((sortedMonsters[attackNum].GetComponent<Monster>().Level*2/5)+2)*
              ((skillPower*sortedMonsters[attackNum].GetComponent<Monster>().Atk/enemy[rand].GetComponent<Monster>().Def/50)+2));
            CommandText.text =enemy[rand].name+"に"+damage+"ダメージ！";
            enemy[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;
        }

        yield return new WaitForSeconds (1f);

        selectedEnemyNum = 0;
    
        for(int i = 0;i<enemy.Count;i++){
            enemy[i].transform.GetChild(4).GetComponent<SpriteRenderer>().color = Col0;
        }

        //勝利判定
        for(int i = 0; i<enemy.Count;i++){
            if(enemy[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
                enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col0;
                enemy[i].GetComponent<CircleCollider2D>().enabled = false;
                enemy.RemoveAt(i);
                Sort();
            }
        }

        //Debug.Log("enemyCount"+ enemy.Count);

        if(enemy.Count == 0){
            battleGameState = GameState.Win;
            WinCanvas.SetActive(true);
            GetItemText.text = "経験値のかけら:"+firstEnemyNum+"個";
            int experienceItem = SaveData.GetInt("ExperienceItem",0);
            Debug.Log("experienceItem:"+experienceItem);
            experienceItem += firstEnemyNum;
            SaveData.SetInt("ExperienceItem",experienceItem);
            SaveData.Save();
        }

        //攻撃ターン管理
        Debug.Log("attackNum"+attackNum);
        if(attackNum == player.Count+enemy.Count-1){
            attackNum = 0;
        }else{
            attackNum++;
        }
        /*
        if(sortedMonsters[attackNum].GetComponent<Monster>().isEnemy){
            battleGameState = GameState.EnemyAttack;
        }else if(!sortedMonsters[attackNum].GetComponent<Monster>().isEnemy){
            battleGameState = GameState.Command;
        }

        Debug.Log(sortedMonsters[attackNum].transform.name+"の攻撃！");
        yield return new WaitForSeconds (1f);

        CommandText.text  = "";
        Attacking = false;*/

        CommandText.text  = "";
        NextAttackMonster();
    }

}

