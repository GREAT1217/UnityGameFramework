using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Hotfix;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public sealed class DictionaryGenerator
    {
        private static readonly string ExcelPath = Application.dataPath + "/../Excels/Dictionaries";
        private const string CollectionFilePath = "Assets/Game/Configs/Editor/DictionaryCollection.json";

        /// <summary>
        /// 生成字典 Text 文件。
        /// </summary>
        [MenuItem("Game/Generate Dictionary Text File", false, 61)]
        private static void GenerateDictionaryTextFile()
        {
            ExcelUtility.BatchExcelToText(ExcelPath, AssetUtility.DictionaryPath, CollectionFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Dictionary Text File Generated !");
        }

        /// <summary>
        /// 生成字典数据文件。
        /// </summary>
        [MenuItem("Game/Generate Dictionary Data File", false, 62)]
        public static void GenerateDictionaryDataFile()
        {
            string collection = File.ReadAllText(CollectionFilePath);
            if (string.IsNullOrEmpty(collection))
            {
                Debug.LogWarning("Please Generate Text File First.");
                return;
            }

            IOUtility.CreateDirectoryIfNotExists(AssetUtility.DictionaryPath);

            List<string> dictionaryNames = JsonUtility.ToObject<List<string>>(collection);
            for (int i = 0; i < dictionaryNames.Count; i++)
            {
                string dictionaryName = dictionaryNames[i];
                EditorUtility.DisplayProgressBar("Generate Dictionary Data File", Utility.Text.Format("Generate {0}", dictionaryName), (float)i / dictionaryNames.Count);
                try
                {
                    DictionaryProcessor processor = new DictionaryProcessor(Utility.Path.GetRegularPath(AssetUtility.GetEditorDictionaryAsset(dictionaryName, false)), Encoding.UTF8, 0, 1);
                    GenerateDataFile(processor, dictionaryName);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Dictionary Data File Generated !");
        }

        public static void GenerateDataFile(DictionaryProcessor localizationProcessor, string localizationName)
        {
            string binaryDataFileName = Utility.Path.GetRegularPath(AssetUtility.GetEditorDictionaryAsset(localizationName, true));
            if (!localizationProcessor.GenerateDataFile(binaryDataFileName) && File.Exists(binaryDataFileName))
            {
                File.Delete(binaryDataFileName);
            }
        }
    }
}
