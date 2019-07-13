using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class Island01EnemyData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/Island01EnemyData.xlsx";
	private static readonly string exportPath = "Assets/ExcelData/Island01EnemyData.asset";
	private static readonly string[] sheetNames = { "Sheet1","Sheet2","Sheet3","Sheet4","Sheet5","Sheet6","Sheet7", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Island01EnemyData data = (Island01EnemyData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Island01EnemyData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Island01EnemyData> ();
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

					Island01EnemyData.Sheet s = new Island01EnemyData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Island01EnemyData.Param p = new Island01EnemyData.Param ();
						
					cell = row.GetCell(0); p.No = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Lv = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.No_Skill1 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Name_Skill1 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.No_Skill2 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Name_Skill2 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.No_Skill3 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.Name_Skill3 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(9); p.No_Skill4 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.Name_Skill4 = (cell == null ? "" : cell.StringCellValue);
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
