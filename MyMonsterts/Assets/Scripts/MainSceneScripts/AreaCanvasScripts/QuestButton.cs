using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestButton : MonoBehaviour {
	public void MoveToQuestScene(){
		SceneManager.LoadScene("QuestScene");
	}
}
