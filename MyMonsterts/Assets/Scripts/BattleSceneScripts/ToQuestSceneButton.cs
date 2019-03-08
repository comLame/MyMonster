using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToQuestSceneButton : MonoBehaviour {

	public void ToQuestSceneButtonClicked(){
		SceneManager.LoadScene("QuestScene");
	}
}
