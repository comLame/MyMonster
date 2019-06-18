using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBattleScene : MonoBehaviour {

	public GameObject fadeCanvas;

	private float time_fade = 1f; //fadeの時間
	private Fade fade; //fade

	public void onClick(){
		FadeManager.Instance.LoadScene ("BattleScene", 2.0f);
		/* 
		//SceneManager.LoadScene("BattleScene");
		fade = fadeCanvas.transform.GetChild(0).GetComponent<Fade>();
		fade.FadeIn (time_fade, () =>
		{
			//fade.FadeOut(time_fade);
			StartCoroutine(DelayMethod(0.1f,()=>{
				GameObject text = fadeCanvas.transform.GetChild(1).gameObject;
				//text.SetActive(true);
				StartCoroutine(DelayMethod(0.5f,()=>{
					SceneManager.LoadScene("BattleScene");
					//titleScene.SetActive(false);
					//battleScene.SetActive(true);
				}));
			}));
		});
		*/
		//FadeManager.Instance.LoadScene ("BattleScene", 2.0f);
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

	
}
