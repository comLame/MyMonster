using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToAreaSceneButton : MonoBehaviour {

	[SerializeField] private GameObject homeCanvas;
	[SerializeField] private GameObject monsterCanvas;
	[SerializeField] private GameObject shopCanvas;
	[SerializeField] private GameObject settingCanvas;
	[SerializeField] private GameObject areaCanvas;
	// Use this for initialization
	public void ShowAreaScene(){
		//SceneManager.LoadScene("AreaScene");

		homeCanvas.SetActive(false);
		monsterCanvas.SetActive(false);
		shopCanvas.SetActive(false);
		settingCanvas.SetActive(false);
		areaCanvas.SetActive(true);
		
		
	}
}
