using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DamageTextAnimation : MonoBehaviour {

	private float time_animation = 1.0f;
	private float time_fadeout = 0.2f;
	private float time_scaleup = 0.4f;
	private float dis_up = 0.4f;
	private float scale_max = 1.5f;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(true);
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

		//上に移動
		iTween.MoveAdd(gameObject, iTween.Hash("y",dis_up,"time",time_animation
			,"easetype",iTween.EaseType.easeOutExpo));
		
		//Scale
		iTween.ScaleTo(gameObject, iTween.Hash("x",1,"y",1,"time",time_scaleup));

		StartCoroutine(DelayMethod(time_animation,()=> {
			Destroy(gameObject);
		}));

	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}
