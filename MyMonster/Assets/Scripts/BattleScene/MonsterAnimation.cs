using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MonsterAnimation : MonoBehaviour {

	//死亡アニメーション
	public void DeathAnimation(float time_deathAnimation){
		iTween.ValueTo(gameObject, iTween.Hash("from",1,"to",0,"time",time_deathAnimation,
			"onupdate","UpdateDeathAnimation","onupdatetarget",gameObject,"easetype",iTween.EaseType.linear));
		StartCoroutine(DelayMethod(time_deathAnimation,() => {
			this.gameObject.SetActive(false);
		}));
	}

	//死亡アニメーションのUpdate
	private void UpdateDeathAnimation(float alfa){
		Color c = this.gameObject.GetComponent<Image>().color;
		gameObject.GetComponent<Image>().color = new Color(c.r,c.g,c.b,alfa);
	}

	//FadeIn
	public void FadeInAnimation(float time_fadeinAnimation){
		iTween.ValueTo(gameObject, iTween.Hash("from",0,"to",1,"time",time_fadeinAnimation,
			"onupdate","UpdateDeathAnimation","onupdatetarget",gameObject,"easetype",iTween.EaseType.linear));
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
