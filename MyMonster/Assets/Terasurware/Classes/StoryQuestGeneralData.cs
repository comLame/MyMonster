using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryQuestGeneralData : ScriptableObject
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
		
		public string Name;
		public int Gold;
		public int Exp;
		public string Category;
	}
}

