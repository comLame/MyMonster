using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour {

	//オブジェクト参照
	public GameObject canvasHome;
	public GameObject canvasMonster;
	public GameObject canvasShop;
	public GameObject canvasGatya;
	public GameObject canvasSetting;

	private List<GameObject> canvasArray = new List<GameObject>();

	private void Start(){
		canvasArray.Add(canvasHome);
		canvasArray.Add(canvasMonster);
		canvasArray.Add(canvasShop);
		canvasArray.Add(canvasGatya);
		canvasArray.Add(canvasSetting);

		int num_canvas = canvasArray.Count;
		for(int i=0;i<num_canvas;i++){
			canvasArray[i].SetActive(true);
			if(i != 0)canvasArray[i].SetActive(false);
		}
	}
}
