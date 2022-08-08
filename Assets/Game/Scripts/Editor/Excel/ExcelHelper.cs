using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class ExcelHelper
    {
        private static readonly string TextExtension = ".txt";
        private static readonly string ExcelExtension = ".xlsx";

        public static void BatchExcelToText(string excelDirectory, string textDirectory, string collectionFileFullPath)
        {
            if (string.IsNullOrEmpty(excelDirectory) || string.IsNullOrEmpty(textDirectory))
            {
                return;
            }

            IOUtility.CreateDirectoryIfNotExists(textDirectory);

            List<string> names = new List<string>();
            List<FileInfo> fileInfos = IOUtility.GetFilesWithExtension(excelDirectory, ExcelExtension);
            for (int i = 0; i < fileInfos.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Excel to text", Utility.Text.Format("Converting {0}", fileInfos[i].Name), (float)i / fileInfos.Count);
                FileInfo fileInfo = fileInfos[i];
                if (fileInfo.Name.StartsWith("~$"))
                {
                    continue;
                }
                string excelFile = Utility.Path.GetRegularPath(fileInfo.FullName);
                string textFile = Utility.Path.GetRegularPath(Path.Combine(textDirectory, fileInfo.Name.Replace(ExcelExtension, TextExtension)));
                ExcelToText(excelFile, textFile);
                names.Add(fileInfo.Name.Split('.')[0]);
            }

            string json = JsonUtility.ToJson(names);
            IOUtility.SaveFileSafe(collectionFileFullPath, json);

            EditorUtility.ClearProgressBar();
        }

        private static void ExcelToText(string excelFileFullName, string textFileFullName)
        {
            if (!File.Exists(excelFileFullName))
            {
                Debug.LogError("File Not Exits : " + excelFileFullName);
                return;
            }

            if (File.Exists(textFileFullName))
            {
                File.Delete(textFileFullName);
            }

            try
            {
                using (FileStream excelStream = File.Open(excelFileFullName, FileMode.Open, FileAccess.Read))
                {
                    StringBuilder text = new StringBuilder();

                    using (IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(excelStream))
                    {
                        DataSet data = reader.AsDataSet();
                        if (data.Tables.Count < 1)
                        {
                            Debug.LogError("Excel Not Exit Any Table : " + excelFileFullName);
                            return;
                        }

                        var sheet = data.Tables[0];
                        int rowCount = sheet.Rows.Count;
                        int columnCount = sheet.Columns.Count;
                        for (int row = 0; row < rowCount; row++)
                        {
                            if (row != 0)
                            {
                                text.Append("\r\n");
                            }
                            for (int column = 0; column < columnCount; column++)
                            {
                                if (column != 0)
                                {
                                    text.Append("\t");
                                }
                                text.Append(sheet.Rows[row][column].ToString());
                            }
                        }
                    }

                    IOUtility.SaveFileSafe(textFileFullName, text.ToString());
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
            }
        }
    }
}
