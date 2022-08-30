using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class BuildInfoEditor : EditorWindow
    {
        private string m_BuildInfoFilePath = "Assets/Game/Configs/Runtime/BuildInfo.txt";
        private BuildInfo m_BuildInfo;

        private string m_GameVersion;
        private int m_InternalGameVersion;
        private string m_CheckVersionUrl;
        private string m_WindowsAppUrl;
        private string m_MacOSAppUrl;
        private string m_IOSAppUrl;
        private string m_AndroidAppUrl;

        [MenuItem("Game/BuildInfo Editor", false, 0)]
        private static void ShowWindow()
        {
            BuildInfoEditor window = GetWindow<BuildInfoEditor>("BuildInfo Editor", true);
            window.minSize = new Vector2(400f, 200f);
        }

        private void OnEnable()
        {
            LoadBuildInfo();
        }

        private void OnGUI()
        {
            m_GameVersion = EditorGUILayout.TextField("GameVersion", m_GameVersion);
            m_InternalGameVersion = EditorGUILayout.IntField("InternalGameVersion", m_InternalGameVersion);
            m_CheckVersionUrl = EditorGUILayout.TextField("CheckVersionUrl", m_CheckVersionUrl);
            m_WindowsAppUrl = EditorGUILayout.TextField("WindowsAppUrl", m_WindowsAppUrl);
            m_MacOSAppUrl = EditorGUILayout.TextField("MacOSAppUrl", m_MacOSAppUrl);
            m_IOSAppUrl = EditorGUILayout.TextField("IOSAppUrl", m_IOSAppUrl);
            m_AndroidAppUrl = EditorGUILayout.TextField("AndroidAppUrl", m_AndroidAppUrl);

            if (GUILayout.Button("Generate"))
            {
                SaveBuildInfo();
            }

            if (GUILayout.Button("Select"))
            {
                PingBuildInfoFile();
            }
        }

        private void LoadBuildInfo()
        {
            if (!File.Exists(m_BuildInfoFilePath))
            {
                return;
            }

            m_BuildInfo = JsonUtility.ToObject<BuildInfo>(File.ReadAllText(m_BuildInfoFilePath));
            if (m_BuildInfo == null)
            {
                return;
            }

            m_GameVersion = m_BuildInfo.GameVersion;
            m_InternalGameVersion = m_BuildInfo.InternalGameVersion;
            m_CheckVersionUrl = m_BuildInfo.CheckVersionUrl;
            m_WindowsAppUrl = m_BuildInfo.WindowsAppUrl;
            m_MacOSAppUrl = m_BuildInfo.MacOSAppUrl;
            m_IOSAppUrl = m_BuildInfo.IOSAppUrl;
            m_AndroidAppUrl = m_BuildInfo.AndroidAppUrl;
        }

        private void SaveBuildInfo()
        {
            m_BuildInfo = new BuildInfo
            {
                GameVersion = m_GameVersion,
                InternalGameVersion = m_InternalGameVersion,
                CheckVersionUrl = m_CheckVersionUrl,
                WindowsAppUrl = m_WindowsAppUrl,
                MacOSAppUrl = m_MacOSAppUrl,
                IOSAppUrl = m_IOSAppUrl,
                AndroidAppUrl = m_AndroidAppUrl
            };

            string json = JsonUtility.ToJson(m_BuildInfo);
            IOUtility.SaveFileSafe(m_BuildInfoFilePath, json);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BuildInfo Generated : " + m_BuildInfoFilePath);
        }

        private void PingBuildInfoFile()
        {
            if (File.Exists(m_BuildInfoFilePath))
            {
                string path = m_BuildInfoFilePath.Substring(m_BuildInfoFilePath.IndexOf("Asset", StringComparison.Ordinal));
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                EditorGUIUtility.PingObject(textAsset);
            }
            else
            {
                Debug.LogWarning("BuildInfo file not exist!");
            }
        }
    }
}
