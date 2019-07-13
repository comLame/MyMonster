using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island01ConversationData : ScriptableObject
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
		public string Position;
		public string Selif;
	}
}

