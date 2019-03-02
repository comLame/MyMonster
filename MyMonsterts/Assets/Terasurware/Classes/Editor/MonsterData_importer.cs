using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class MonsterData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/MonsterData.xlsx";
	private static readonly string exportPath = "Assets/ExcelData/MonsterData.asset";
	private static readonly string[] sheetNames = { "MonsterData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_MonsterData data = (Entity_MonsterData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_MonsterData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_MonsterData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_MonsterData.Sheet s = new Entity_MonsterData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_MonsterData.Param p = new Entity_MonsterData.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.HP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.ATK = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.DEF = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.SPD = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.EvoLv = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.EXP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.ExpGroup = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
