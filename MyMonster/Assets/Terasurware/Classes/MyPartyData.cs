using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyPartyData : ScriptableObject
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
		
		public int No;
		public string Name;
		public int Lv;
		public int No_Skill1;
		public string Name_Skill1;
		public int No_Skill2;
		public string Name_Skill2;
		public int No_Skill3;
		public string Name_Skill3;
		public int No_Skill4;
		public string Name_Skill4;
	}
}

