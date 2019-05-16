using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMonster : MonoBehaviour {

	public void OnClick(){
		Debug.Log("click");
		GameObject battleManager = GameObject.Find("BattleManager");
		battleManager.GetComponent<BattleManager>().TouchMonster(this.gameObject);
	}
}
