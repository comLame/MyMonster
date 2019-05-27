using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FooterManager : MonoBehaviour {

	public GameObject canvasHome;
	public GameObject canvasMonster;
	public GameObject canvasShop;
	public GameObject canvasGatya;
	public GameObject canvasSetting;

	private List<GameObject> canvasArray = new List<GameObject>();
	private GameObject fadeinCanvas;
	private GameObject fadeoutCanvas;
	private float time_delay = 0.1f;
	private float distance = 4.5f;
	private int nowSceneNum = 0;
	private bool isMoving = false;

	//time
	private float time_animation = 0.4f;

	void Start(){
		canvasArray.Add(canvasHome);
		canvasArray.Add(canvasMonster);
		canvasArray.Add(canvasShop);
		canvasArray.Add(canvasGatya);
		canvasArray.Add(canvasSetting);

		HiddenCommonProcess(canvasMonster);
		HiddenCommonProcess(canvasShop);
		HiddenCommonProcess(canvasGatya);
		HiddenCommonProcess(canvasSetting);
	}

	public void MoveScene(int sceneNum){
		if(isMoving)return;
		isMoving = true;
		HiddenCommonProcess(canvasArray[nowSceneNum]);
		/* 
		int len = canvasArray.Count;
		for(int i=0;i<len;i++){
			canvasArray[i].SetActive(false);
		}

		canvasArray[nowSceneNum].SetActive(false);
		*/
		//canvasArray[sceneNum].SetActive(true);
		DisplayCommonProcess(canvasArray[sceneNum]);
		nowSceneNum = sceneNum;
	}

	//表示の際の共通処理
	private void DisplayCommonProcess(GameObject scene){
		scene.GetComponent<CanvasGroup>().alpha = 0;
		scene.SetActive(true);

		List<GameObject> left = scene.GetComponent<MoveCategorize>().left;
		List<GameObject> right = scene.GetComponent<MoveCategorize>().right;

		//FadeIn
		fadeinCanvas = scene;
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"time",0.2f,"delay",time_animation/2,
			"onupdate","FadeIn","onupdatetarget",gameObject));

		//right
		for(int i=0;i<right.Count;i++){
			GameObject obj = right[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2 ));
		}
		//left
		for(int i=0;i<left.Count;i++){
			GameObject obj = left[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",-distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2 ));
		}
	}

	//非表示の際の共通処理
	private void HiddenCommonProcess(GameObject scene){
		
		List<GameObject> left = scene.GetComponent<MoveCategorize>().left;
		List<GameObject> right = scene.GetComponent<MoveCategorize>().right;

		//FadeOut
		fadeoutCanvas = scene;
		iTween.ValueTo(gameObject,iTween.Hash("from",1,"to",0,"time",0.2f,
			"onupdate","FadeOut","onupdatetarget",gameObject));

		//right
		for(int i=0;i<right.Count;i++){
			GameObject obj = right[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2 ));
		}
		//left
		for(int i=0;i<left.Count;i++){
			GameObject obj = left[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",-distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2 ));
		}

		StartCoroutine(DelayMethod(time_animation + time_delay,()=> {
			scene.SetActive(false);
			isMoving = false;
		}));
	}

	private void FadeIn(float a){
		fadeinCanvas.GetComponent<CanvasGroup>().alpha = a;
	}

	private void FadeOut(float a){
		fadeoutCanvas.GetComponent<CanvasGroup>().alpha = a;
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}


}
