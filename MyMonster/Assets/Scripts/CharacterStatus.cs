using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {

	public string name;
	public int battleId; //バトルの時にそのキャラに振られる番号 自分のキャラの上から1,2,3,4,5 相手のキャラの上から6,7,8,9,10
	public int level; //レベル
	public int hp; //HP
	public int attack; //攻撃力
	public int defense; //守備力
	public int speed; //素早さ
	public List<int> skills = new List<int>(){0,0,0,0}; //所持技 技番号が入る1,2,3,4...
	public int num_selectedSkill=1; //選択した技の番号 1→技1 2→技2 ...
	public bool deathFlag = false; //死んだかどうか

}
