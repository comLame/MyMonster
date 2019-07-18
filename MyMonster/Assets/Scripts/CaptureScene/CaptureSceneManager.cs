using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CaptureSceneManager : MonoBehaviour {

	public GameObject img_monster;
	public GameObject img_gauge;
	public GameObject messageWindow;
	public GameObject statusWindow;

	public float probability; //ゲットできる確率

	private bool canGet; //ゲットできるかどうか 
	private bool canTap = false; //タップを受け付けるかどうか
	private bool canMove = false; //画面遷移していいかどうか
	private float time_captureAnimation = 2f; 

	private List<Monster> ownMonsters = new List<Monster>();
	private int nextUID; //次（今回捕まえるモンスター）のUID

	private void Start(){
		probability = 80;

		GetData();

		float fProbabilityRate = UnityEngine.Random.value * 100.0f;

		if(fProbabilityRate < probability){
			//成功
			canGet = true;
		}else{
			//失敗
			canGet = false;
		}
	}

	private void Update(){
		if(!canTap)return;

		if(Input.GetMouseButtonDown(0) && !canMove){
			canTap = false;
			if(canGet){
				DisplayStatusWindow();
				StartCoroutine(DelayMethod(2,() => {
					canMove = true;
					canTap = true;
				}));
			}else{
				//画面遷移
        		FadeManager.Instance.LoadScene ("ResultScene", 1.0f);
			}
		}else if(Input.GetMouseButtonDown(0) && canMove){
			//画面遷移
        	FadeManager.Instance.LoadScene ("ResultScene", 1.0f);
		}
	}

	private void DisplayStatusWindow(){
		messageWindow.SetActive(false);

		statusWindow.SetActive(true);
		iTween.MoveFrom(statusWindow,iTween.Hash("x",-5,"time",0.5f,"easetype",iTween.EaseType.easeInSine));
	}

	//
	public void OnClickCaptureButton(){
		GameObject txt_window = messageWindow.transform.GetChild(0).gameObject;
		GameObject btn_cancel = messageWindow.transform.GetChild(1).gameObject;
		GameObject btn_ok = messageWindow.transform.GetChild(2).gameObject;

		btn_cancel.SetActive(false);
		btn_ok.SetActive(false);
		txt_window.GetComponent<Text>().text = "・・・・・・";

		StartCoroutine(DelayMethod(time_captureAnimation/2,() => {
			if(!canGet)img_monster.SetActive(false);
		}));

		StartCoroutine(DelayMethod(time_captureAnimation,() => {
			ResultCapture();
		}));
	}

	private void ResultCapture(){
		canTap = true;
		GameObject txt_window = messageWindow.transform.GetChild(0).gameObject;
		if(canGet){
			//捕まえられたなら
			txt_window.GetComponent<Text>().text = 
				"やった！　" + "トリッピー" + "が" + "\n"
				+"なかまに　なったよ！";
			
			//データ保存
			Monster mons = new Monster();
			mons.No = 4;
			mons.uniqueID = nextUID;
			mons.level = 10;
			mons.skills = new int[]{1,2,3,4};
			mons.totalExp = 1800; //総経験値
			mons.betweenLevelExp = 0; //レベルアップしてから獲得した経験値
			mons.expType = 60; //経験値テーブル　

			ownMonsters.Add(mons); //追加

			SaveData.SetList<Monster>(SaveDataKeys.ownMonsters,ownMonsters);
			SaveData.SetInt(SaveDataKeys.uid,nextUID);
			SaveData.Save();

		}else{
			//失敗
			txt_window.GetComponent<Text>().text = 
				"ざんねん…　" + "トリッピー" + "は" + "\n"
				+"とおくに　いってしまったみたい。";
		}

	}

	private void GetData(){
		ownMonsters = SaveData.GetList<Monster>(SaveDataKeys.ownMonsters,ownMonsters);
		nextUID = SaveData.GetInt(SaveDataKeys.uid,0) + 1;
	}

	public void OnClickCancelCaptureButton(){
		//画面遷移
        FadeManager.Instance.LoadScene ("ResultScene", 1.0f);
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
