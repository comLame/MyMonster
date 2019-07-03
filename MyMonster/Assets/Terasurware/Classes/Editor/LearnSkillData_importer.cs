using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class LearnSkillData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/LearnSkillData.xls";
	private static readonly string exportPath = "Assets/ExcelData/LearnSkillData.asset";
	private static readonly string[] sheetNames = { "ヒコシバ","ドルン","バンプー","トリッピー", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			LearnSkillData data = (LearnSkillData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(LearnSkillData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<LearnSkillData> ();
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

					LearnSkillData.Sheet s = new LearnSkillData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						LearnSkillData.Param p = new LearnSkillData.Param ();
						
					cell = row.GetCell(0); p.Level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.SkillNum = (int)(cell == null ? 0 : cell.NumericCellValue);
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
