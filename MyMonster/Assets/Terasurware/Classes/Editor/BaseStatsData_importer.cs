using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class BaseStatsData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/BaseStatsData.xls";
	private static readonly string exportPath = "Assets/ExcelData/BaseStatsData.asset";
	private static readonly string[] sheetNames = { "BaseStatsData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			BaseStatsData data = (BaseStatsData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(BaseStatsData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<BaseStatsData> ();
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

					BaseStatsData.Sheet s = new BaseStatsData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						BaseStatsData.Param p = new BaseStatsData.Param ();
						
					cell = row.GetCell(0); p.No = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.Attack = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Defense = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Speed = (int)(cell == null ? 0 : cell.NumericCellValue);
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
