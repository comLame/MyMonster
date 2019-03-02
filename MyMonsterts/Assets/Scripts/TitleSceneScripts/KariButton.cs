using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KariButton : MonoBehaviour {

	public void ToHomeScene(){
		SceneManager.LoadScene("MainScene");
	}
}
