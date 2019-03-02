using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour {

	public GameObject mapCanvas;
	private bool canvasMove = false;

	void Update(){
		if(canvasMove){
			mapCanvas.transform.position 
			= new Vector3(mapCanvas.transform.position.x,mapCanvas.transform.position.y-0.01f,mapCanvas.transform.position.z);
		}
	}

	public void mapScrollButtonDown(){
		canvasMove = true;
		Debug.Log("a");
	}

	public void mapScrollButtonUp(){
		canvasMove = false;
	}

}
