using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataKeys{

	static public string vod = "vod"; //バトルの勝敗 1:勝利,2:敗北
	static public string ownMonsters = "ownMonsters"; //所持モンスターのMonsterクラスのリスト
	static public string evolutionMonsterList = "evolutionMonsterList"; //進化するモンスターUID配列
	static public string evolutionMonsterIndex = "evolutionMonsterIndex"; //進化するモンスターUID配列の何番目か 1~
}
