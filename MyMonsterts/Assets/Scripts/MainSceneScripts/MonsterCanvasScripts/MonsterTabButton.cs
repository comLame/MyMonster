using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterTabButton : MonoBehaviour {
	public void MoveToFormationScene(){
		SceneManager.LoadScene("FormationScene");
	}

	public void MoveToPowerUPScene(){
		SceneManager.LoadScene("PowerUpScene");
	}

	public void MoveToEvolutionScene(){
		SceneManager.LoadScene("EvolutionScene");
	}
}
