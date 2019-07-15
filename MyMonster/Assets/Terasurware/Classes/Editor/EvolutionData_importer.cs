using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EvolutionData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/EvolutionData.xlsx";
	private static readonly string exportPath = "Assets/ExcelData/EvolutionData.asset";
	private static readonly string[] sheetNames = { "Sheet1","Sheet2","Sheet3","Sheet4","Sheet5", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EvolutionData data = (EvolutionData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EvolutionData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EvolutionData> ();
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

					EvolutionData.Sheet s = new EvolutionData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EvolutionData.Param p = new EvolutionData.Param ();
						
					cell = row.GetCell(0); p.Lv = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.MonsterNum = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Name = (cell == null ? "" : cell.StringCellValue);
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
