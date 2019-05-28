using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

	public bool isTopScreen = false;
	public GameObject topScreen;

	//topScreenのみをactiveにする
	public void Initialize(){
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
		topScreen.SetActive(true);
		isTopScreen = true;
	}
}
