//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;

namespace Game.Hotfix
{
    public static class AssetUtility
    {
        public static readonly string ConfigPath = "Assets/Game/Configs/Runtime";
        public static readonly string DataTablePath = "Assets/Game/DataTables";
        public static readonly string DictionaryPath = "Assets/Game/Localization/Dictionaries";

        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("{0}/{1}.{2}", ConfigPath, assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("{0}/{1}.{2}", DataTablePath, assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetEditorDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("{0}/{1}.{2}", DictionaryPath, assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("{0}/{1}_{2}.{3}", DictionaryPath, GameEntry.Localization.Language, assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Game/UI/UISounds/{0}.wav", assetName);
        }
    }
}
