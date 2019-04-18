using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCommandButton : MonoBehaviour {

	public void OnClick(){
		GameObject battleManager = GameObject.Find("BattleManager");
		battleManager.GetComponent<BattleManager>().AllyAttack();
	}
}
