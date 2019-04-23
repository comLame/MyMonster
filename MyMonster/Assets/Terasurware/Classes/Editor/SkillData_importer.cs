using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class SkillData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/SkillData.xls";
	private static readonly string exportPath = "Assets/ExcelData/SkillData.asset";
	private static readonly string[] sheetNames = { "SkillData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			SkillData data = (SkillData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(SkillData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<SkillData> ();
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

					SkillData.Sheet s = new SkillData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						SkillData.Param p = new SkillData.Param ();
						
					cell = row.GetCell(0); p.No = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Species = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.Target = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.Type = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.Damage = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.HealAmount = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.ChangeAmount = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.ChangeStatus = (cell == null ? "" : cell.StringCellValue);
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
