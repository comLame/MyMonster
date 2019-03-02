using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_ExpData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int Lv;
		public int Group60;
		public int Group80;
		public int Group100;
		public int Group105;
		public int Group125;
		public int Group160;
	}
}

