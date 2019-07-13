using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterAnimation : MonoBehaviour {

	private float time_animation = 0.2f;
	private float disY_animation = 0.1f;

	public void AnimationStart(){
		iTween.MoveBy(gameObject,iTween.Hash("y",disY_animation,"time",time_animation/2));
		StartCoroutine(DelayMethod(time_animation/2+0.01f,() => {
			iTween.MoveBy(gameObject,iTween.Hash("y",-1 * disY_animation,"time",time_animation/2));
		}));
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
