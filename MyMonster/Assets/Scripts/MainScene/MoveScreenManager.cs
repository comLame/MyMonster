using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MoveScreenManager : MonoBehaviour{
	
	public GameObject tabInitializeManager;

	private GameObject nowScreen;//現在表示している画面
	private GameObject nowTabScreen; //現在表示しているタブのトップ画面
	private GameObject fadeinScreen;
	private GameObject fadeoutScreen;
	private float time_delay = 0.5f;
	private float distance = 4.5f;
	private int nowTabNum = 1; //現在表示している画面のタブ
	private int nowScreenNum = 0; //現在遷移しようとしている画面の種類 0:タブのトップじゃない,数字（n）:左からn番目のタブのトップ画面
	private bool isMoving = false;
	private int moveComponentCount = 0; //動くものの数
	private int moveComponentCounter = 0;

	//time
	private float time_animation = 0.4f;

	private void Start(){
		GameObject canvasHome = GameObject.Find("CanvasHome");
		GameObject topScreen = canvasHome.GetComponent<CanvasManager>().topScreen;
		nowScreen = topScreen;
		nowTabScreen = topScreen;
	}

	public void MoveScreen(GameObject displayScreen,GameObject hideScreen,int tabNum){
		//遷移中は入力不可
		if(isMoving)return;
		nowScreenNum = tabNum;

		if(tabNum != 0){
			if(nowTabNum == tabNum){
				//現在表示しているタブと同じタブボタンが押された
				if(nowTabScreen == displayScreen){
					//トップ画面の表示中にそのタブボタンを押しても画面遷移なし
					return;
				}else{
					//同じタブ内の違う画面にいるのなら、トップ画面に遷移
					Debug.Log("ここにきた");
					hideScreen = nowScreen;
				}
			}else{
				hideScreen = nowTabScreen;
				nowTabScreen = displayScreen;
			}
			nowTabNum = tabNum;
		}

		//コンポーネントのアタッチ
		SetComponents(hideScreen);
		SetComponents(displayScreen);
		//遷移実行
		HiddenScreen(hideScreen);
		DisplayScreen(displayScreen);
		//変数変更
		nowScreen = displayScreen;

		/* 
		if(isTab){
			//遷移先が今と同じ場合は入力不可
			if(nowScreen == screen)return;
			//遷移開始
			isMoving = true;
			moveComponentCount = 0;
			moveComponentCounter = 0;
			//コンポーネントのアタッチ
			SetComponents(nowScreen);
			SetComponents(screen);
			//遷移実行
			HiddenScreen(nowScreen);
			DisplayScreen(screen);
			//変数変更
			canInitialize = true;
			currentScreen = nowScreen;
			nowScreen = screen;
		}else{
			//タブ以外
			//遷移開始
			isMoving = true;
			moveComponentCount = 0;
			moveComponentCounter = 0;
			//コンポーネントのアタッチ
			SetComponents(screen);
			SetComponents(hideScreen);

			//遷移実行
			HiddenScreen(hideScreen);
			DisplayScreen(screen);

			//変数変更
			//currentScreen = nowScreen;
			//nowScreen = screen;
		}
		*/
	}

	private void HiddenScreen(GameObject screen){
		List<GameObject> left = screen.GetComponent<MoveCategorize>().left;
		List<GameObject> right = screen.GetComponent<MoveCategorize>().right;

		//moveComponentCoutn
		moveComponentCount += left.Count;
		moveComponentCount += right.Count;

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
				,"time",time_animation/2,"delay",time_animation/2,"oncomplete","CheckMoveFinish","oncompletetarget",gameObject ));
		}
		//left
		for(int i=0;i<left.Count;i++){
			GameObject obj = left[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",-distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2,"oncomplete","CheckMoveFinish","oncompletetarget",gameObject ));
		}

	}

	//表示の際の共通処理
	private void DisplayScreen(GameObject screen){
		screen.GetComponent<CanvasGroup>().alpha = 0;
		//scene.transform.parent.gameObject.SetActive(true);
		screen.SetActive(true);

		List<GameObject> left = screen.GetComponent<MoveCategorize>().left;
		List<GameObject> right = screen.GetComponent<MoveCategorize>().right;

		//moveComponentCoutn
		moveComponentCount += left.Count;
		moveComponentCount += right.Count;

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
				,"time",time_animation/2,"delay",time_animation/2,"oncomplete","CheckMoveFinish","oncompletetarget",gameObject ));
		}
		//left
		for(int i=0;i<left.Count;i++){
			GameObject obj = left[i];

			Vector3 nowPos = obj.GetComponent<RectTransform>().localPosition;
			iTween.MoveBy(obj,iTween.Hash("x",-distance,"easetype",iTween.EaseType.easeOutSine,"time",time_animation/2));
			iTween.MoveTo(obj,iTween.Hash("position",nowPos,"easetype",iTween.EaseType.easeInSine,"isLocal",true
				,"time",time_animation/2,"delay",time_animation/2,"oncomplete","CheckMoveFinish","oncompletetarget",gameObject ));
		}
	}

	//タブを初期状態に戻す
	private void InitializeTab(){
		

		switch(nowScreenNum){
		case 0:
			return;
		case 1:
			tabInitializeManager.GetComponent<TabInitializeManager>().MonsterTab();
			break;
		case 2:
			tabInitializeManager.GetComponent<TabInitializeManager>().HomeTab();
			break;
		case 3:
		case 4:
		case 5:
		default:
			break;
		}

	}

	//遷移アニメーションが終わったかどうか確認
	private void CheckMoveFinish(){
		moveComponentCounter++;
		Debug.Log("moveComponentCount:" + moveComponentCount);
		Debug.Log("moveComponentCounter:" + moveComponentCounter);
		if(moveComponentCounter >= moveComponentCount){
			//全コンポーネントのアニメーション終了
			isMoving = false;
			fadeoutScreen.SetActive(false);

			//タブを初期状態に戻す
			InitializeTab();
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
