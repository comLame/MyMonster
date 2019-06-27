using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveScreenButtonManager : MonoBehaviour {

	
	[SerializeField] public GameObject transitionDestinationScreen; //遷移先の画面
	public GameObject hideScreen; //消す
	public bool isTab = false; //押したボタンがタブかどうか
	public bool isTop = false; //遷移先がTopScreenかどうか
	
	private MoveScreenManager MoveScreenManager;

	private void Start(){
		MoveScreenManager = GameObject.Find("MoveScreenManager").GetComponent<MoveScreenManager>();

		//オブジェクトにEventTriggerがない時
		if(gameObject.GetComponent<EventTrigger>() == null){
			gameObject.AddComponent<EventTrigger>();
		}

		var trigger = gameObject.GetComponent<EventTrigger>();
		if(trigger.triggers == null){
			//アクションが設定されていない場合は設定するアクションリストを作成
			trigger.triggers = new List<EventTrigger.Entry>();
		}

        // クリック時のイベントを設定してみる
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick; // 他のイベントを設定したい場合はここを変える
        entry.callback.AddListener( (x) => { 
			MoveScreenManager.MoveScreen(transitionDestinationScreen,hideScreen,isTab,isTop);
		});
        trigger.triggers.Add(entry);
	}

}
