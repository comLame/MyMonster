using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPauseButton : MonoBehaviour {

	private bool isPause = false;

	public void OnClick(){
		if(isPause){
			//動かす
			//Time.timeScale = 1;
			//iTween.Resume();

			isPause = !isPause;
		}else{
			//止める
			//Time.timeScale = 0;
			//iTween.Pause();

			isPause = !isPause;
		}
	}
}
