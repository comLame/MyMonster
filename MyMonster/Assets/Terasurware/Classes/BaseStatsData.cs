using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseStatsData : ScriptableObject
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
		public string Type;
		public int Hp;
		public int Attack;
		public int Defense;
		public int Speed;
	}
}

