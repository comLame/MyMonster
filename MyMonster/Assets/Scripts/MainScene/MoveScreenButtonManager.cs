using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveScreenButtonManager : MonoBehaviour,IPointerClickHandler{

	
	[SerializeField] public GameObject transitionDestinationScreen; //遷移先の画面
	public GameObject hideScreen; //消す
	public int tabNum = 0; //0:タブじゃない ,数字（n）:左からn番目のタブボタン
	
	private MoveScreenManager MoveScreenManager;

	private void Start(){
		MoveScreenManager = GameObject.Find("MoveScreenManager").GetComponent<MoveScreenManager>();
	}

	public void OnPointerClick(PointerEventData eventData){
    	MoveScreenManager.MoveScreen(transitionDestinationScreen,hideScreen,tabNum);
    }

}
