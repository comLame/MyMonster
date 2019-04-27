using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchCommandButton : MonoBehaviour {

	float maxScale = 1.1f;
	float time_animation = 0.05f;
	float time = 0;
	bool canClick = true;
	float time_canClick = 0.3f; //これ以上タップしてるとクリック判定にならない
	private bool isPush = false;
	public string name_skill = ""; //技の名前
	public string explain = ""; //技の説明文
	public GameObject skillExplainArea;

	public void OnClick(int num){
		if(!canClick)return;
		GameObject battleManager = GameObject.Find("BattleManager");
		battleManager.GetComponent<BattleManager>().AllyAttack(num);
	}

	private void FixedUpdate(){
		if(!isPush)return;
		time += Time.deltaTime;
		if(time >= time_canClick){
			canClick = false;
			skillExplainArea.SetActive(true);
		}
	}

	public void PointerDown(){
		isPush = true;
		//スキル説明文
		GameObject txt_skillName = skillExplainArea.transform.GetChild(0).gameObject;
		GameObject txt_explain = skillExplainArea.transform.GetChild(1).gameObject;
		txt_skillName.GetComponent<Text>().text = name_skill;
		txt_explain.GetComponent<Text>().text = explain;

		time = 0;
		canClick = true;
		iTween.ValueTo(gameObject, iTween.Hash("from",1,"to",maxScale,"time",time_animation,
			"onupdate","UpdateScale","onupdatetarget",gameObject));
	}

	public void PointerUp(){
		isPush = false;
		skillExplainArea.SetActive(false);
		iTween.ValueTo(gameObject, iTween.Hash("from",maxScale,"to",1,"time",time_animation,
			"onupdate","UpdateScale","onupdatetarget",gameObject,
			"oncomplete","CompleteScale","oncompletetarget",gameObject));
	}

	private void UpdateScale(float scale){
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale,scale,1);
	}

	private void CompleteScale(){
		if(canClick){
			GameObject commandArea = gameObject.transform.parent.gameObject;
			commandArea.SetActive(false);
		}
	}
}
