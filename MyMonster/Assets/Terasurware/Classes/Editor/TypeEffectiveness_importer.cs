using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class TypeEffectiveness_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/TypeEffectiveness.xls";
	private static readonly string exportPath = "Assets/ExcelData/TypeEffectiveness.asset";
	private static readonly string[] sheetNames = { "TypeEffecetiveness", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			TypeEffecetiveness data = (TypeEffecetiveness)AssetDatabase.LoadAssetAtPath (exportPath, typeof(TypeEffecetiveness));
			if (data == null) {
				data = ScriptableObject.CreateInstance<TypeEffecetiveness> ();
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

					TypeEffecetiveness.Sheet s = new TypeEffecetiveness.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						TypeEffecetiveness.Param p = new TypeEffecetiveness.Param ();
						
					cell = row.GetCell(0); p.Attack = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Fire = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Water = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.Grass = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Lightning = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Darkness = (int)(cell == null ? 0 : cell.NumericCellValue);
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
