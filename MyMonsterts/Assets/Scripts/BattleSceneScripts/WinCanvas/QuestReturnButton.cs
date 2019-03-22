using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestReturnButton : MonoBehaviour {

	public void QuestReturnButtonClicked(){
		SceneManager.LoadScene("QuestScene");
	}
}
