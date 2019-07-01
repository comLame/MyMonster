using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSliderAnimation : MonoBehaviour {

	public void StartAnimation(int start,int goal,float time){
		iTween.ValueTo(gameObject,iTween.Hash("from",start,"to",goal,"onupdate","UpdateAnimation",
			"onupdatetarget",gameObject,"time",time));
	}

	private void UpdateAnimation(float value){
		gameObject.GetComponent<Slider>().value = value;
	}
}
