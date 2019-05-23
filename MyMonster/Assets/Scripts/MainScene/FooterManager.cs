using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FooterManager : MonoBehaviour {

	public GameObject canvasHome;
	public GameObject canvasMonster;
	public GameObject canvasShop;
	public GameObject canvasGatya;
	public GameObject canvasSetting;

	private List<GameObject> canvasArray = new List<GameObject>();

	void Start(){
		canvasArray.Add(canvasHome);
		canvasArray.Add(canvasMonster);
		canvasArray.Add(canvasShop);
		canvasArray.Add(canvasGatya);
		canvasArray.Add(canvasSetting);
	}

	public void MoveScene(int sceneNum){
		int len = canvasArray.Count;
		for(int i=0;i<len;i++){
			canvasArray[i].SetActive(false);
		}
		canvasArray[sceneNum].SetActive(true);
	}
}
