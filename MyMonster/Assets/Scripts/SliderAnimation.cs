using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAnimation : MonoBehaviour {

	//スライダーアニメーション
	public void Animation(int originalHp,int currentHp,float time_hitAnimation){
		iTween.ValueTo(gameObject,iTween.Hash("from",originalHp,"to",currentHp,"onupdatetarget",gameObject,
			"onupdate","UpdateSlider","time",time_hitAnimation));
	}

	private void UpdateSlider(float hp){
		gameObject.GetComponent<Slider>().value = hp;
	}
}
