using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultManager : MonoBehaviour {

    public SkillData skillData;
    public BaseStatsData baseStatsData;
    public LearnSkillData learnSkillData;
    public ExpTypeData expTypeData;
    public GameObject txtGold;
    public GameObject txtExp;
    public GameObject containerMonster;
    public GameObject containerLearnSkill;

    private float time_levelupSliderAnimation = 0.5f;
    private float time_displayLearnSkill = 1f;
    private string strFlag = "initial";
    private int gold = 3461;
    private int exp =  700;

    private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();
    private List<int> storyProgress = new List<int>();
    private List<int> nowStoryQuest = new List<int>();

    private void Start(){

        GetData();
        DisplayMonster();
        //GetExpData();
        
        txtGold.GetComponent<Text>().text = gold.ToString();
        txtExp.GetComponent<Text>().text = exp.ToString();

        LevelupProcess(0,exp);

        //SaveExpData();
    }

    private void GetData(){
        ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
        partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];

        storyProgress = SaveData.GetList<int>("storyProgress",storyProgress);
        nowStoryQuest = SaveData.GetList<int>("nowStoryQuest",nowStoryQuest);

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

    private void DisplayMonster(){
        for(int i=0;i<5;i++){
            int uid = party.monsters[i];
            Monster mons = GetMonsterFromUID(uid);
            GameObject container = containerMonster.transform.GetChild(i).gameObject;
            GameObject imgMonster = container.transform.GetChild(0).gameObject;
            GameObject txtLevel = container.transform.GetChild(1).GetChild(1).gameObject;
            GameObject slider = container.transform.GetChild(2).gameObject;

            imgMonster.GetComponent<Image>().sprite = Resources.Load<Sprite>("Img_Monster/" + mons.No);
            txtLevel.GetComponent<Text>().text = mons.level.ToString();

            //exp関係
            int exptype = mons.expType;
            int sheetNum = GetExpType(exptype);
            int betweenExp = expTypeData.sheets[sheetNum].list[mons.level - 1].BetweenLevels;
            int nowExp = mons.betweenLevelExp;

            //スライダーの初期値変更
            slider.GetComponent<Slider>().maxValue = betweenExp;
            slider.GetComponent<Slider>().value = nowExp;
        }
    }

    private void LevelupProcess(int num,int getExp){
        int uid = party.monsters[num];
        Monster mons = GetMonsterFromUID(uid);
        GameObject container = containerMonster.transform.GetChild(num).gameObject;
        GameObject imgMonster = container.transform.GetChild(0).gameObject;
        GameObject txtLevel = container.transform.GetChild(1).GetChild(1).gameObject;
        GameObject slider = container.transform.GetChild(2).gameObject;

        //exp関係
        int exptype = mons.expType;
        int sheetNum = GetExpType(exptype);
        int betweenExp = expTypeData.sheets[sheetNum].list[mons.level - 1].BetweenLevels;
        int nowExp = mons.betweenLevelExp;
        //int getExp = exp;
        int gap = betweenExp - (nowExp + getExp); //レベルアップまでの経験値と経験値取得後の経験値の差
        int flag = (gap <= 0) ? 2 : 1 ; //1:levelupなし,2:levelupあり,3:levelupかつ技習得

        //スライダーの初期値変更
        slider.GetComponent<Slider>().maxValue = betweenExp;
        slider.GetComponent<Slider>().value = nowExp;

        if(flag == 1){
            //levelupなし
            slider.GetComponent<LevelUpSliderAnimation>().StartAnimation(nowExp,nowExp+getExp,time_levelupSliderAnimation);
            
            StartCoroutine(DelayMethod(time_levelupSliderAnimation + 0.1f,() => {
                mons.betweenLevelExp = nowExp+getExp;
                SaveExpData(mons);
                if(num == 4){
                    //レベルアップ終了
                    SaveWholeData();
                    strFlag = "canMove";
                    Debug.Log("終了");
                    return;
                }
                LevelupProcess(num + 1,exp);
            }));
        }else if(flag == 2){
            //levelupあり
            slider.GetComponent<LevelUpSliderAnimation>().StartAnimation(nowExp,betweenExp,time_levelupSliderAnimation);
            int level = mons.level + 1;
            int skillNum = 0;
            for(int i=0;i<learnSkillData.sheets[mons.No-1].list.Count;i++){
                if(level == learnSkillData.sheets[mons.No-1].list[i].Level){
                    //技を覚える
                    skillNum = learnSkillData.sheets[mons.No-1].list[i].SkillNum;
                }
            }
            if(skillNum == 0){
                //技覚えない
                StartCoroutine(DelayMethod(time_levelupSliderAnimation + 0.1f,() => {
                    LevelUp(mons,container);
                    if(gap == 0){
                        //ちょうどレベルアップしたら次のモンスター
                        SaveExpData(mons);
                        LevelupProcess(num + 1,exp);
                    }else{
                        //それ以外はもう一回同じモンスター
                        LevelupProcess(num,-1 * gap);
                    }
                }));
            }else{
                //技覚える
                StartCoroutine(DelayMethod(time_levelupSliderAnimation + 0.1f,() => {
                    //技覚えテキストの表示
                    string nameMonster = baseStatsData.sheets[0].list[mons.No-1].Name;
                    string nameSkill = skillData.sheets[0].list[skillNum-1].Name;
                    GameObject txt_nameMonster = containerLearnSkill.transform.GetChild(0).GetChild(0).gameObject;
                    GameObject txt_nameSkill = containerLearnSkill.transform.GetChild(0).GetChild(1).gameObject;
                    txt_nameMonster.GetComponent<Text>().text = nameMonster + "は あたらしく";
                    txt_nameSkill.GetComponent<Text>().text = nameSkill + " をおぼえた！";
                    containerLearnSkill.SetActive(true);
                }));
                StartCoroutine(DelayMethod(time_levelupSliderAnimation + time_displayLearnSkill + 0.1f,() => {
                    containerLearnSkill.SetActive(false);
                    LevelUp(mons,container);
                    if(gap == 0){
                        //ちょうどレベルアップしたら次のモンスター
                        SaveExpData(mons);
                        LevelupProcess(num + 1,exp);
                    }else{
                        //それ以外はもう一回同じモンスター
                        LevelupProcess(num,-1 * gap);
                    }
                }));
            }
        }
    }

    
    private void LevelUp(Monster mons,GameObject container){

        GameObject imgMonster = container.transform.GetChild(0).gameObject;
        GameObject txtLevel = container.transform.GetChild(1).GetChild(1).gameObject;
        GameObject slider = container.transform.GetChild(2).gameObject;

        slider.GetComponent<Slider>().value = 0;
        mons.betweenLevelExp = 0;
        mons.level = mons.level + 1;
        txtLevel.GetComponent<Text>().text = mons.level.ToString();
    }

    private int GetExpType(int expType){
        switch(expType){
        case 60:
            return 0;
        case 80:
            return 1;
        case 100:
            return 2;
        case 105:
            return 3;
        case 125:
            return 4;
        case 164:
            return 5;
        default:
            return 0;
        }
    }

    private void SaveExpData(Monster mons){
        int uid = mons.uniqueID;
        for(int i=0;i<ownMonsters.Count;i++){
            if(ownMonsters[i].uniqueID == uid){
                ownMonsters.RemoveAt(i);
                ownMonsters.Insert(i,mons);
            }
        }
    }

    private void SaveWholeData(){

        //ストーリー進捗チェック
        if(storyProgress[0]==nowStoryQuest[0]&storyProgress[1]==nowStoryQuest[1]){
            //現時点の最高ストーリーと現在のクリアステージが等しい場合、最高ステージの更新
            storyProgress[1] = storyProgress[1]+1;
            Debug.Log("更新後のストーリー番号:" + storyProgress[0]+"-"+storyProgress[1]);
            SaveData.SetList<int>("storyProgress",storyProgress);
        }

        SaveData.SetList<Monster>("ownMonsters",ownMonsters);
		SaveData.Save();
    }

    public void OnClick(){
        if(strFlag == "initial"){
            //アニメーション完了
            iTween.Stop(txtGold);
            iTween.Stop(txtExp);

            txtGold.GetComponent<Text>().text = gold.ToString();
            txtExp.GetComponent<Text>().text = exp.ToString();

            strFlag = "animationDone";
        }else if(strFlag == "canMove"){
            //画面遷移
            FadeManager.Instance.LoadScene ("MainScene", 1.0f);
        }
    }

    //ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}
