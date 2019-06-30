using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountUpAnimation : MonoBehaviour {

	//public int num;
	public float time;

	public void StartAnimation(int num){
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",num,"time",time,
			"onupdate","UpdateAnimation","onupdatetarget",gameObject));
	}

	private void UpdateAnimation(float n){
		GetComponent<Text>().text = ((int)n).ToString();
	}
}
