using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationChangeManager : MonoBehaviour {

	public void FormationDecide(){
		GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationName = this.gameObject.name;
		this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
		GameObject.Find("FormationManager").GetComponent<FormationData>().selected = true;
		GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationNumber = int.Parse(this.gameObject.name.Substring(11));
	}

	public void FormationChange(){
		if(GameObject.Find("FormationManager").GetComponent<FormationData>().selected){
			GameObject.Find(GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationName)
			.transform.GetChild(0).GetComponent<Text>().text = this.gameObject.transform.GetChild(0)
			.GetComponent<Text>().text;

			GameObject.Find(GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationName)
			.transform.GetChild(2).gameObject.SetActive(false);

			List<int> formation = new List<int>(); 
			formation = SaveData.GetList<int>("Formation",new List<int>(){0,0,0,0,0});
			Debug.Log("selectedFormationNum:"+GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationNumber);
			formation[GameObject.Find("FormationManager").GetComponent<FormationData>().selectedFormationNumber-1] 
			= int.Parse(this.gameObject.transform.GetChild(0).GetComponent<Text>().text);
			SaveData.SetList("Formation",formation);
			SaveData.Save();
		}
		GameObject.Find("FormationManager").GetComponent<FormationData>().selected = false;
	}
}
