using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpotButton : MonoBehaviour {

	private GameObject islandScreen;

	// Use this for initialization
	void Start () {

		islandScreen = GameObject.Find("CanvasHome").transform.Find("IslandScreen").gameObject;

		//EventTrigger
		var trigger = gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();

        // クリック時のイベントを設定してみる
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick; // 他のイベントを設定したい場合はここを変える
        entry.callback.AddListener( (x) => { 
			islandScreen.GetComponent<IslandScreenManager>().DisplayDeckSelectionView();
		});
        trigger.triggers.Add(entry);
		
	}
}
