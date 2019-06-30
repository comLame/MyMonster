using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultManager : MonoBehaviour {

    public ExpTypeData expTypeData;
    public GameObject txtGold;
    public GameObject txtExp;
    public GameObject containerMonster;

    private string strFlag = "initial";
    private int gold = 3461;
    private int exp = 300;

    private List<Monster> ownMonsters = new List<Monster>();
	private Party party = new Party();
	private List<Party> partyList = new List<Party>();

    private void Start(){

        GetMonsterData();
        DisplayMonster();
        GetExpData();

        StartCoroutine(DelayMethod(1,() => {
            txtGold.GetComponent<CountUpAnimation>().StartAnimation(gold);
            txtExp.GetComponent<CountUpAnimation>().StartAnimation(exp);
        }));
    }

    private void GetMonsterData(){
        ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
        partyList = SaveData.GetList<Party>("partyList",partyList);
		party = partyList[0];
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
            int sheetNum = 0;
            switch(exptype){
            case 60:
                sheetNum = 0;
                break;
            case 80:
                sheetNum = 1;
                break;
            case 100:
                sheetNum = 2;
                break;
            case 105:
                sheetNum = 3;
                break;  
            case 125:
                sheetNum = 4;
                break;
            case 164:
                sheetNum = 5;
                break;
            }
            int betweenExp = expTypeData.sheets[sheetNum].list[mons.level - 1].BetweenLevels;
            int nowExp = mons.betweenLevelExp;

            //スライダーの初期値変更
            slider.GetComponent<Slider>().maxValue = betweenExp;
            slider.GetComponent<Slider>().value = nowExp;
        }
    }

    private void GetExpData(){

    }

    public void OnClick(){
        if(strFlag == "initial"){
            //アニメーション完了
            iTween.Stop(txtGold);
            iTween.Stop(txtExp);

            txtGold.GetComponent<Text>().text = gold.ToString();
            txtExp.GetComponent<Text>().text = exp.ToString();

            strFlag = "animationDone";
        }else if(strFlag == "animationDone"){
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
