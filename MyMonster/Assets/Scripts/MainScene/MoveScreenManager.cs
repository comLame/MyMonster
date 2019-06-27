using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MoveScreenManager : MonoBehaviour{
	
	private GameObject currentScreen;
	private GameObject nowScreen;
	private GameObject fadeinScreen;
	private GameObject fadeoutScreen;
	private float time_delay = 0.5f;
	private float distance = 4.5f;
	private int nowSceneNum = 0;
	private bool isMoving = false;

	//time
	private float time_animation = 0.4f;

	private void Start(){
		GameObject canvasHome = GameObject.Find("CanvasHome");
		GameObject topScreen = canvasHome.GetComponent<CanvasManager>().topScreen;
		currentScreen = topScreen;
	}

	public void MoveScreen(GameObject screen,GameObject hideScreen,bool isTab = false,bool isTop = false){
		//遷移中は入力不可
		if(isMoving)return;
		
		if(isTab){
			//遷移先が今と同じ場合は入力不可
			if(currentScreen == screen)return;
			//遷移開始
			isMoving = true;
			//コンポーネントのアタッチ
			SetComponents(currentScreen);
			SetComponents(screen);
			//遷移実行
			HiddenScreen(currentScreen);
			DisplayScreen(screen);
			//変数変更
			currentScreen = screen;
		}else{
			//タブ以外
			//遷移開始
			isMoving = true;
			//コンポーネントのアタッチ
			SetComponents(screen);
			SetComponents(hideScreen);

			//遷移実行
			HiddenScreen(hideScreen);
			DisplayScreen(screen);
		}
	}

	private void HiddenScreen(GameObject screen){
		List<GameObject> left = screen.GetComponent<MoveCategorize>().left;
		List<GameObject> right = screen.GetComponent<MoveCategorize>().right;

		//FadeOut
		fadeoutScreen = screen;
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
			screen.SetActive(false);
			isMoving = false;
		}));
	}

	//表示の際の共通処理
	private void DisplayScreen(GameObject screen){
		screen.GetComponent<CanvasGroup>().alpha = 0;
		//scene.transform.parent.gameObject.SetActive(true);
		screen.SetActive(true);

		List<GameObject> left = screen.GetComponent<MoveCategorize>().left;
		List<GameObject> right = screen.GetComponent<MoveCategorize>().right;

		//FadeIn
		fadeinScreen = screen;
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

	private void SetComponents(GameObject screen){
		//CanvasGroupのアタッチ
		if(screen.GetComponent<CanvasGroup>() == null){
			screen.AddComponent<CanvasGroup>();
		}

		//MoveCategorizeのアタッチ
		if(screen.GetComponent<MoveCategorize>() == null){
			screen.AddComponent<MoveCategorize>();
		}
	}

	private void FadeIn(float a){
		fadeinScreen.GetComponent<CanvasGroup>().alpha = a;
	}

	private void FadeOut(float a){
		fadeoutScreen.GetComponent<CanvasGroup>().alpha = a;
	}

	//ディレイメソッド
	private static IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
