using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCreaterManager : MonoBehaviour {

	public GameObject iconPrefab;

	public List<Sprite> frameList = new List<Sprite>();
	public List<Sprite> typeIconList = new List<Sprite>();

	public GameObject Create(Monster monsterClass,string type){
		GameObject icon = (GameObject)Instantiate(iconPrefab);
		GameObject frame = icon.gameObject;
		GameObject monster = icon.transform.GetChild(0).gameObject;
		GameObject typeIcon = icon.transform.GetChild(1).gameObject;
		int typeNum = 0;
		switch(type.ToString()){
		case "Fire":
			typeNum = 0;
			break;
		case "Water":
			typeNum = 1;
			break;
		case "Grass":
			typeNum = 2;
			break;
		case "Lightning":
			typeNum = 3;
			break;
		case "Darkness":
			typeNum = 4;
			break;
		}
		monster.GetComponent<Image>().sprite = Resources.Load<Sprite>("Img_Monster/" + monsterClass.No);
		frame.GetComponent<Image>().sprite = frameList[typeNum];
		typeIcon.GetComponent<Image>().sprite = typeIconList[typeNum];
		icon.transform.GetChild(3).GetComponent<Text>().text = monsterClass.level.ToString();
		icon.GetComponent<IconInformation>().uniqueId = monsterClass.uniqueID;

		return icon;
	}
}
