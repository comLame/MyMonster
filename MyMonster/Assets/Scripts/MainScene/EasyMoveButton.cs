using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyMoveButton : MonoBehaviour {

	public GameObject hiddenScreen;
	public GameObject displayScreen;

	public void OnClick(){
		displayScreen.SetActive(true);
		hiddenScreen.SetActive(false);
	}
}
