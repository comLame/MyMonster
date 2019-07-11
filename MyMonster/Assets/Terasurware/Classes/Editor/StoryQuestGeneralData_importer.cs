using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StoryQuestGeneralData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/StoryQuestGeneralData.xls";
	private static readonly string exportPath = "Assets/ExcelData/StoryQuestGeneralData.asset";
	private static readonly string[] sheetNames = { "アフ島", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			StoryQuestGeneralData data = (StoryQuestGeneralData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StoryQuestGeneralData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StoryQuestGeneralData> ();
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

					StoryQuestGeneralData.Sheet s = new StoryQuestGeneralData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						StoryQuestGeneralData.Param p = new StoryQuestGeneralData.Param ();
						
					cell = row.GetCell(0); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Gold = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.Category = (cell == null ? "" : cell.StringCellValue);
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
