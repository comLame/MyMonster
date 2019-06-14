using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDeckSelectView : MonoBehaviour {

	public GameObject stageSelectView;
	public GameObject deckSelectView;

	public void OnClick(){
		deckSelectView.SetActive(true);
		stageSelectView.SetActive(false);
	}
}
