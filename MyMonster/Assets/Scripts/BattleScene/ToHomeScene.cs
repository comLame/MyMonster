using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToHomeScene : MonoBehaviour {

	public void OnClick(){
		//SceneManager.LoadScene("MainScene");
		FadeManager.Instance.LoadScene ("MainScene", 1.0f);
	}
}
