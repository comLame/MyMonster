using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LandingButton : MonoBehaviour {

	public GameObject img_explain;
	public GameObject sv_island;
	public GameObject content;

	private Vector2 size;
	private float scale = 3;
	private float time_animation = 1f;
	private bool isMoving = false;

	private void Start(){
		content = sv_island.transform.GetChild(0).GetChild(0).gameObject;

		float width = content.GetComponent<RectTransform>().rect.width;
		float height = content.GetComponent<RectTransform>().rect.height;
		size = new Vector2(width,height);
	}

	public void OnClick(){
		img_explain.SetActive(false);

		isMoving = true;
		iTween.ScaleTo(content,iTween.Hash("x",scale,"y",scale,"time",time_animation));
		StartCoroutine(DelayMethod(time_animation,() => { isMoving = false; } ));
	}

	private void Update(){
		if(!isMoving)return;
		content.GetComponent<RectTransform>().localPosition = CalculatePos(new Vector3(-267.7f,1624.6f,0));
		
	}

	private Vector3 CalculatePos(Vector3 targetPos){
		float ratioX = targetPos.x/( size.x * (scale - 1) );
		float ratioY = targetPos.y/( size.y * (scale - 1) );

		Vector2 localscale = content.GetComponent<RectTransform>().localScale;
		Vector3 pos = new Vector3(
			size.x * (localscale.x - 1) * ratioX,
			size.y * (localscale.y - 1) * ratioY,
			0
		);

		return pos;
	}

	//ディレイメソッド
	private static IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}
