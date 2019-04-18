using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {

	public string name;
	public int battleId; //バトルの時にそのキャラに振られる番号 自分のキャラの上から1,2,3,4,5 相手のキャラの上から6,7,8,9,10
	public int hp;
	public int attack;
	public int defense;
	public int speed;
	public bool deathFlag = false; //死んだかどうか

}
