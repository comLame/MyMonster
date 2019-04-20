using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Inspectorに複数データを表示するためのクラス
[System.SerializableAttribute]
public class  ValueList{
  //public List<int>  List = new List<int>();
	/* 
  public ValueList(List<int>  list){
    List = list;
  }*/
	public Texture texture;
	public int x = 1;
	public int y = 1;
	public float scale = 1.5f;
	public float speed = 1.5f;
}

public class SkillEffectData : MonoBehaviour {
	public Material material;
	//Inspectorに表示される
	[SerializeField]
	public List<ValueList> _valueListList = new List<ValueList> ();
	
}
