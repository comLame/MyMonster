using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterFirstMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		bool isEnemy = GetComponent<Monster>().isEnemy;
		if(!isEnemy){
			RectTransform rect = GetComponent<RectTransform>();

			Vector3[] path = {
				new Vector3(rect.localPosition.x-150f, rect.localPosition.y+100f, rect.localPosition.z),
				new Vector3(rect.localPosition.x-300f, rect.localPosition.y,rect.localPosition.z)
			};

			rect.DOLocalPath(path,0.5f,PathType.CatmullRom)
				.SetEase(Ease.OutQuad);
		}else{
			RectTransform rect = GetComponent<RectTransform>();

			Vector3[] path = {
				new Vector3(rect.localPosition.x+150f, rect.localPosition.y+100f, rect.localPosition.z),
				new Vector3(rect.localPosition.x+300f, rect.localPosition.y,rect.localPosition.z)
			};

			rect.DOLocalPath(path,0.5f,PathType.CatmullRom)
				.SetEase(Ease.OutQuad);
		}
	}

}
