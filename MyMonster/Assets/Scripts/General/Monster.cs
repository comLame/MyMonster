using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Monster
{
    //キャラクター固有の情報 CharacterDataはScriptableObjectを継承
	public int No; //図鑑番号
	public int uniqueID; //固有ID
	public int level; //レベル
	//public int exp; //現在のレベルになってから獲得した経験値
	public int[] skills = new int[4]{0,0,0,0}; //所持技
	public int totalExp; //総経験値
	public int betweenLevelExp; //レベルアップしてから獲得した経験値
	public int expType; //経験値タイプ
}