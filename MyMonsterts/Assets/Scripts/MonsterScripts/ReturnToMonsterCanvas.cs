using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMonsterCanvas : MonoBehaviour {
	public void ToMonsterCanvas(){
		SceneManager.LoadScene("MainScene");
	}
}
