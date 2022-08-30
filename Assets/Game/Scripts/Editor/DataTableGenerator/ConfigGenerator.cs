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
    public sealed class ConfigGenerator
    {
        private static readonly string ConfigExcelPath = Application.dataPath + "/../Excels/Configs";
        private const string CollectionFilePath = "Assets/Game/Configs/Editor/ConfigCollection.json";

        /// <summary>
        /// 生成配置 Text 文件。
        /// </summary>
        [MenuItem("Game/Generate Config Text File", false, 21)]
        private static void GenerateConfigTextFile()
        {
            ExcelUtility.BatchExcelToText(ConfigExcelPath, AssetUtility.ConfigPath, CollectionFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Config Text File Generated !");
        }

        /// <summary>
        /// 生成配置数据文件。
        /// </summary>
        [MenuItem("Game/Generate Config Data File", false, 22)]
        public static void GenerateConfigDataFile()
        {
            string collection = File.ReadAllText(CollectionFilePath);
            if (string.IsNullOrEmpty(collection))
            {
                Debug.LogWarning("Please Generate Text File First.");
                return;
            }

            IOUtility.CreateDirectoryIfNotExists(AssetUtility.ConfigPath);

            List<string> configNames = JsonUtility.ToObject<List<string>>(collection);
            for (int i = 0; i < configNames.Count; i++)
            {
                string configName = configNames[i];
                EditorUtility.DisplayProgressBar("Generate Config Data File", Utility.Text.Format("Generate {0}", configName), (float)i / configNames.Count);
                try
                {
                    DictionaryProcessor processor = new DictionaryProcessor(Utility.Path.GetRegularPath(AssetUtility.GetConfigAsset(configName, false)), Encoding.UTF8, 1, 2);
                    GenerateDataFile(processor, configName);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.Message);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Config Data File Generated !");
        }

        public static void GenerateDataFile(DictionaryProcessor configProcessor, string configName)
        {
            string binaryDataFileName = Utility.Path.GetRegularPath(AssetUtility.GetConfigAsset(configName, true));
            if (!configProcessor.GenerateDataFile(binaryDataFileName) && File.Exists(binaryDataFileName))
            {
                File.Delete(binaryDataFileName);
            }
        }
    }
}
