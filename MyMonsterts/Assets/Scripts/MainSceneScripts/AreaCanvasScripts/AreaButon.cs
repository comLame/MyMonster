using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaButon : MonoBehaviour {

	[SerializeField] private GameObject questPanel;


	public void ShowQuestPanel(){
		questPanel.SetActive(true);

	}
}
