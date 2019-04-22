using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyAtkFX : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(DelayMethod(5.0f,()=>{
			Destroy(gameObject);
		}));
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
