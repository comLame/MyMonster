using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbPurchaseButton : MonoBehaviour {

	private Text orbText;

	void Start(){
		orbText = GameObject.Find("OrbCountText").GetComponent<Text>();
	}

	public void OrbPurchase10(){
		int orb = SaveData.GetInt("OrbCount",0);

		orb += 10;
		orbText.text = orb.ToString();
		SaveData.SetInt("OrbCount",orb);
		SaveData.Save();

	}
	public void OrbPurchase30(){
		int orb = SaveData.GetInt("OrbCount",0);

		orb += 30;
		orbText.text = orb.ToString();
		SaveData.SetInt("OrbCount",orb);
		SaveData.Save();

	}
	public void OrbPurchase50(){
		int orb = SaveData.GetInt("OrbCount",0);

		orb += 50;
		orbText.text = orb.ToString();
		SaveData.SetInt("OrbCount",orb);
		SaveData.Save();

	}
	public void OrbPurchase80(){
		int orb = SaveData.GetInt("OrbCount",0);

		orb += 80;
		orbText.text = orb.ToString();
		SaveData.SetInt("OrbCount",orb);
		SaveData.Save();

	}public void OrbPurchase100(){
		int orb = SaveData.GetInt("OrbCount",0);

		orb += 100;
		orbText.text = orb.ToString();
		SaveData.SetInt("OrbCount",orb);
		SaveData.Save();

	}
}
