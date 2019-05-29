using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveScreenButtonManager : MonoBehaviour {

	
	[SerializeField] public GameObject transitionDestinationScreen; //遷移先の画面
	public bool isTab = false; //押したボタンがタブかどうか
	public bool isTop = false; //遷移先がTopScreenかどうか
	
	private MoveScreenManager MoveScreenManager;

	private void Start(){
		MoveScreenManager = GameObject.Find("MoveScreenManager").GetComponent<MoveScreenManager>();

		var trigger = gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();

        // クリック時のイベントを設定してみる
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick; // 他のイベントを設定したい場合はここを変える
        entry.callback.AddListener( (x) => { 
			MoveScreenManager.MoveScreen(transitionDestinationScreen,isTab,isTop);
		});
        trigger.triggers.Add(entry);
	}

}
