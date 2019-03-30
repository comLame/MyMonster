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

    public Text CommandText;
    public Text GetItemText;

    public GameObject enemyImage;
    public GameObject myImage;

    public GameObject commandButton;

    private GameState battleGameState = GameState.TurnStart;
    //public Slider playerSlider;
    //public Slider enemySlider;

    private int firstEnemyNum;

    private int skillPower = 80; 
    private int attackNum = 0;
    private int enemyAttackNum = 0;
    private int playerAttackNum = 0;
    private int selectedEnemyNum = 0;

    List<GameObject> sortedMonsters = new List<GameObject>();

    // Use this for initialization
    void Start () {

        firstEnemyNum = enemy.Count;
        Debug.Log("firstEnemyNum:"+firstEnemyNum);

        Sort();

        if(sortedMonsters[0].GetComponent<Monster>().isEnemy){
            StartCoroutine("EnemyAttack",0);
        }else{
            //コマンド表示
            battleGameState = GameState.PlayerAttack;
            commandButton.SetActive(true);

            Color col1 = sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
            col1.a = 255;
            sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = col1;

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
                    if(hitObject.collider==enemy[i].GetComponent<Collider2D>()){
                        selectedEnemyNum = i+1;
                        //Debug.Log("selectedEnemyNum:"+selectedEnemyNum);
                    }
                }
                Color col = hitObject.collider.gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().color;
                col.a = 255;
                hitObject.collider.gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().color = col;
                }
            }
        }
    }

    //素早さ順にモンスター並べ替え
    void Sort(){
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
    }

    //[{(攻撃側のレベル × 2 ÷ 5 + 2) × (威力 × 攻撃側の攻撃or特攻 ÷ 防御側の防御or特防) ÷ 50 + 2} × 乱数幅(0.85,0.86,...0.99,1.00)] × 一致補正など
    private IEnumerator EnemyAttack(int order) {  
        CommandText.text =sortedMonsters[order].transform.name+"の攻撃！";
        yield return new WaitForSeconds (1f);  

        //ダメージ計算
        int rand = Random.Range(1, player.Count+1)-1;
        int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
        ((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/player[rand].GetComponent<Monster>().Def/50)+2));
        CommandText.text = player[rand].name+"に"+damage+"ダメージ！";
        player[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;

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

        Color col1 = sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
        col1.a = 255;
        sortedMonsters[attackNum].transform.GetChild(3).GetComponent<SpriteRenderer>().color = col1;

        //敗北判定
        for(int i = 0; i<player.Count;i++){
            if(player[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
                Color color = enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                color.a = 0;
                player[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
                player.RemoveAt(i);
            }
        }
        
        Debug.Log("playerCount:"+player.Count);
        if(player.Count==0){
            battleGameState = GameState.Lose;
            LoseCanvas.SetActive(true);
        }
        yield return new WaitForSeconds (1f);  
        //コマンド表示
        commandButton.SetActive(true);

        enemyImage.SetActive(false);
        CommandText.text  = "";
    }

    public void Attack(){
        StartCoroutine ("PlayerAttack",attackNum);
    }

    private IEnumerator PlayerAttack(int order) {  
        CommandText.text = sortedMonsters[order].transform.name+"の攻撃！";
        commandButton.SetActive(false);

        yield return new WaitForSeconds (1f);

        int rand = Random.Range(1, enemy.Count+1)-1;

        //ダメージ計算
        if(selectedEnemyNum != 0){
            int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
                ((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/enemy[selectedEnemyNum-1].GetComponent<Monster>().Def/50)+2));
            CommandText.text = enemy[selectedEnemyNum-1].name+"に"+damage+"ダメージ！";
            enemy[selectedEnemyNum-1].transform.GetChild(1).GetComponent<Slider>().value -= damage;
        }else{
            int damage = (int)(((sortedMonsters[order].GetComponent<Monster>().Level*2/5)+2)*
              ((skillPower*sortedMonsters[order].GetComponent<Monster>().Atk/enemy[rand].GetComponent<Monster>().Def/50)+2));
            CommandText.text =enemy[rand].name+"に"+damage+"ダメージ！";
            enemy[rand].transform.GetChild(1).GetComponent<Slider>().value -= damage;
        }

        yield return new WaitForSeconds (1f);

        selectedEnemyNum = 0;
    
        for(int i = 0;i<enemy.Count;i++){
            Color col = enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
            col.a = 0;
            enemy[i].transform.GetChild(3).GetComponent<SpriteRenderer>().color = col;
        }

        
        battleGameState = GameState.EnemyAttack;
        Color col2 = sortedMonsters[order].transform.GetChild(3).GetComponent<SpriteRenderer>().color;
        col2.a = 0;
        sortedMonsters[order].transform.GetChild(3).GetComponent<SpriteRenderer>().color = col2;

        //勝利判定
        for(int i = 0; i<enemy.Count;i++){
            if(enemy[i].transform.GetChild(1).GetComponent<Slider>().value<=0){
                Color color = enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                color.a = 0;
                enemy[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
                enemy[i].GetComponent<CircleCollider2D>().enabled = false;
                enemy.RemoveAt(i);
            }
        }

        Debug.Log("enemyCount"+ enemy.Count);
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

        CommandText.text  = "";
    }

}

