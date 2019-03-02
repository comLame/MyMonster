using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

[SerializeField] private GameObject areaCanvas;

	public void HideAreaCanvas(){
		areaCanvas.SetActive(false);
	}
}
