using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandScreenManager : MonoBehaviour {

	public GameObject deckSelectionView; //デッキ選択画面

	public void DisplayDeckSelectionView(){
		deckSelectionView.SetActive(true);
	}
}
