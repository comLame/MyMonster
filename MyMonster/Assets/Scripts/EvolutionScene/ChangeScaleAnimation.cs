using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScaleAnimation : MonoBehaviour {

	public void StartAnimation(float scale,float time){
		iTween.Stop(gameObject);
		iTween.ScaleTo(gameObject,iTween.Hash("x",scale,"y",scale,"time",time
			,"easeType",iTween.EaseType.linear));
	}
}
