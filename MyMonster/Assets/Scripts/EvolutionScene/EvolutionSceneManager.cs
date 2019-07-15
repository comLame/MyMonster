using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EvolutionSceneManager : MonoBehaviour {

	public EvolutionData evolutionData;
	public GameObject img_monster; //モンスター画像
	public GameObject img_afterEvolutionMons; //進化後のモンスター画像
	public GameObject txt_messageWindow; 

	private float time_whitenAnimation = 1.5f; //徐々に白くするアニメーションの時間
	private float minTime_changeScaleAnimation = 0.8f;
	private int count_changeScaleAnimation = 1;
	private int maxCount_changeScaleAnimation = 6;
	private float time = 0;
	private float time_oneRotation = 2; //一回転（一往復）するのに必要な時間
	private bool canAnimate = false;
	private bool animationFinishFlag = false; //終了させる回かどうか

	private List<Monster> ownMonsters = new List<Monster>();
	private Monster mons;
	private List<int> evolutionMonsterList = new List<int>(); //モンスターのuidが入る
	private int afterEvolutionMonsterNum;
	private int evolutionMonsterIndex;
	private Sprite monsSprite;
	private Sprite afterEvolutionMonsSprite;

	void Start () {
		SetData();
		StartCoroutine(DelayMethod(1,() => {
			AnimationStart();
		}));
		
	}

	void Update(){

	}

	void FixedUpdate(){
		if(!canAnimate)return;
		time += Time.deltaTime;
		float angle = (time/time_oneRotation)*2*Mathf.PI;

		img_monster.GetComponent<RectTransform>().localScale = new Vector3(
			Mathf.Abs(Mathf.Cos(angle)),Mathf.Abs(Mathf.Cos(angle)),Mathf.Abs(Mathf.Cos(angle)));
		img_afterEvolutionMons.GetComponent<RectTransform>().localScale = new Vector3(
			Mathf.Abs(Mathf.Sin(angle)),Mathf.Abs(Mathf.Sin(angle)),Mathf.Abs(Mathf.Sin(angle)));

		if(time >= time_oneRotation){
			//1回転したらここに入る
			count_changeScaleAnimation++;
			if(count_changeScaleAnimation > maxCount_changeScaleAnimation){
				//アニメーション終了
				//canAnimate = false;
				animationFinishFlag = true;
			}
			time -= time_oneRotation;
			//1回転にかかる時間を短くする
			time_oneRotation = time_oneRotation/(count_changeScaleAnimation);
			if(time_oneRotation <= minTime_changeScaleAnimation)time_oneRotation = minTime_changeScaleAnimation;
		}

		if(animationFinishFlag && angle >= Mathf.PI){
			FinishedAnimation();
		}

	}

	//アニメーションが終了した後の処理
	private void FinishedAnimation(){
		canAnimate = false;
		img_afterEvolutionMons.gameObject.GetComponent<RectTransform>().localScale
			= new Vector3(1,1,1);
		img_monster.SetActive(false);

		StartCoroutine(DelayMethod(1,() => {
			iTween.ValueTo(img_afterEvolutionMons,iTween.Hash("from",1,"to",0,"time",time_whitenAnimation,
				"onupdate","WhitenAnimation","onupdatetarget",gameObject));
		}));

		StartCoroutine(DelayMethod(3,() => {
			txt_messageWindow.GetComponent<Text>().text = "おめでとう！　ドルンは" + "\n"
				+ "ドルルンに　しんかした！";
		}));

		StartCoroutine(DelayMethod(5,() => {

			if(evolutionMonsterList.Count > evolutionMonsterIndex){
				//まだ進化するモンスターがいるならもう一度進化画面に遷移
				SaveData.SetInt(SaveDataKeys.evolutionMonsterIndex,evolutionMonsterIndex+1);
				SaveData.Save();

				FadeManager.Instance.LoadScene ("EvolutionScene", 1.0f);
			}else{
				//もう進化するモンスターが以内ならMain画面に遷移
				FadeManager.Instance.LoadScene ("MainScene", 1.0f);
			}
		}));


	}

	private void AnimationStart(){
		//白くする
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"time",time_whitenAnimation,
			"onupdate","WhitenAnimation","onupdatetarget",gameObject));
		
		StartCoroutine(DelayMethod(time_whitenAnimation + 1,() => {
			canAnimate = true;
		}));
		
	}

	private void WhitenAnimation(float alfa){
		img_monster.transform.GetChild(0).gameObject.GetComponent<Image>().color
			= new Color(1,1,1,alfa);
		img_afterEvolutionMons.transform.GetChild(0).gameObject.GetComponent<Image>().color
			= new Color(1,1,1,alfa);
	}


	private void SetData(){
		ownMonsters = SaveData.GetList<Monster>(SaveDataKeys.ownMonsters,ownMonsters);
		evolutionMonsterList = SaveData.GetList<int>(SaveDataKeys.evolutionMonsterList,evolutionMonsterList);
		evolutionMonsterIndex = SaveData.GetInt(SaveDataKeys.evolutionMonsterIndex,evolutionMonsterIndex);

		//進化するモンスターの特定
		for(int i=0;i<ownMonsters.Count;i++){
			if(ownMonsters[i].uniqueID == evolutionMonsterList[evolutionMonsterIndex-1]){
				mons = ownMonsters[i];
			}
		}

		//進化先モンスターのNo.を取得
		afterEvolutionMonsterNum = evolutionData.sheets[mons.No-1].list[1].MonsterNum;

		//画像を設定
		monsSprite = Resources.Load<Sprite>("Img_Monster/" + mons.No);
		afterEvolutionMonsSprite = Resources.Load<Sprite>("Img_Monster/" + afterEvolutionMonsterNum);
		img_monster.GetComponent<Image>().sprite = monsSprite;
		img_afterEvolutionMons.GetComponent<Image>().sprite = afterEvolutionMonsSprite;

		//進化情報を保存
		for(int i=0;i<ownMonsters.Count;i++){
			if(ownMonsters[i].uniqueID == evolutionMonsterList[evolutionMonsterIndex-1]){
				mons.No = afterEvolutionMonsterNum;
				ownMonsters.RemoveAt(i);
                ownMonsters.Insert(i,mons);
			}
		}
		SaveData.SetList<Monster>(SaveDataKeys.ownMonsters,ownMonsters);
		SaveData.Save();
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
