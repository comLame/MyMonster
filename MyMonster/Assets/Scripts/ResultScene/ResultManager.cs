using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResultManager : MonoBehaviour {

    public GameObject txtGold;
    public GameObject txtExp;

    private void Start(){
        StartCoroutine(DelayMethod(1,() => {
            txtGold.GetComponent<CountUpAnimation>().StartAnimation();
            txtExp.GetComponent<CountUpAnimation>().StartAnimation();
        }));
    }

    //ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
