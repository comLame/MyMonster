using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestNameViewAnimation : MonoBehaviour
{
    
    public void StartAnimation(float time_fadein,float time_stay,float time_fadeout){
        iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"time",time_fadein
            ,"onupdate","Fade","onupdatetarget",gameObject));

        StartCoroutine(DelayMethod(time_fadein+time_stay,() => {
            iTween.ValueTo(gameObject,iTween.Hash("from",1,"to",0,"time",time_fadeout
                ,"onupdate","Fade","onupdatetarget",gameObject));
        }));
    }

    private void Fade(float alfa){
        gameObject.GetComponent<CanvasGroup>().alpha = alfa;
    }

    //ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
