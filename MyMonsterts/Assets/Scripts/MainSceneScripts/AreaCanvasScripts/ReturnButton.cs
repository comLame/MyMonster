using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour {
	[SerializeField] private GameObject questPanel;

	public void ReturnToAreaCanvas(){
		questPanel.SetActive(false);
	}

	public void ReturnToMainScene(){
		SceneManager.LoadScene("MainScene");
	}

}
