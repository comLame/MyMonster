using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtons : MonoBehaviour {
	public void SkillButtonsPushed(){
		GameObject.Find("BattleManager").GetComponent<BattleManager>().Attack(transform.GetChild(0).transform.GetComponent<Text>().text);
	}
}
