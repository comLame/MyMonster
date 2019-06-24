using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyEditScreenManager : MonoBehaviour {

	public GameObject iconCreaterManager;
	public BaseStatsData baseStatsData;
	public GameObject container;

	private List<Monster> ownMonsters = new List<Monster>();

	private void Awake(){
		GetOwnMonstersData();

		for(int i=0;i<ownMonsters.Count;i++){
			Monster mons = ownMonsters[i];
		}

		for(int i=0;i<ownMonsters.Count;i++){
			Monster mons = ownMonsters[i];
			string type = baseStatsData.sheets[0].list[mons.No-1].Type;

			GameObject icon = IconCreate(mons.No,type,mons.level);
			icon.transform.SetParent(container.transform);
			icon.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		}
	}

	private GameObject IconCreate(int No,string type,int level){
		//必要要素
		/*
			図鑑No、
			属性、
			レベル、
		 */
		return iconCreaterManager.GetComponent<IconCreaterManager>().Create(No,type,level);
	}

	private void GetOwnMonstersData(){
		ownMonsters = SaveData.GetList<Monster>("ownMonsters",ownMonsters);
	}
}
