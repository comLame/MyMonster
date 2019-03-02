using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_MonsterData : ScriptableObject
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
		
		public int ID;
		public string Name;
		public int HP;
		public int ATK;
		public int DEF;
		public int SPD;
		public int EvoLv;
		public int EXP;
		public int ExpGroup;
	}
}

