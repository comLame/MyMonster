using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TypeEffecetiveness : ScriptableObject
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
		
		public string Attack;
		public int Fire;
		public int Water;
		public int Grass;
		public int Lightning;
		public int Darkness;
	}
}

