using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillData : ScriptableObject
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
		public string Species;
		public string Target;
		public string Type;
		public int Damage;
		public int HealAmount;
		public int ChangeAmount;
		public string ChangeStatus;
		public string Explain;
	}
}

