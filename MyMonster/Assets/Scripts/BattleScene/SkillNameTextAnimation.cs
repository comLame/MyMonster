using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SkillNameTextAnimation : MonoBehaviour {

	private float time_scaleUp = 0.2f;
	private float time_animation = 2.0f;
	private float time_fadeout = 0.2f;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(true);
		//拡大
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"onupdate","UpdateAnimation",
			"onupdatetarget",gameObject,"time",time_scaleUp));
		//FadeOut
		StartCoroutine(DelayMethod(time_animation - time_fadeout,() => {
			iTween.ValueTo(gameObject,iTween.Hash("from",1,"to",0,"onupdate","UpdateFadeOut",
				"onupdatetarget",gameObject,"time",time_fadeout));
		}));
		//Destroy
		StartCoroutine(DelayMethod(time_animation,() => {
			Destroy(this.gameObject);
		}));
	}

	private void UpdateAnimation(float scale){
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale,scale,scale);
	}

	private void UpdateFadeOut(float alfa){
		Color c = gameObject.GetComponent<Text>().color;
		Color c_outline = gameObject.GetComponent<Outline>().effectColor;
		gameObject.GetComponent<Text>().color = new Color(c.r,c.g,c.b,alfa);
		gameObject.GetComponent<Outline>().effectColor = new Color(c_outline.r,c_outline.g,c_outline.b,alfa);
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
