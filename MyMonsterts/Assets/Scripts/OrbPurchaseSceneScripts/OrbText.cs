using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbText : MonoBehaviour {
	// Use this for initialization
	void Start () {
		int orbCount = SaveData.GetInt("OrbCount",0);
		this.gameObject.GetComponent<Text>().text = orbCount.ToString();
	}
}
