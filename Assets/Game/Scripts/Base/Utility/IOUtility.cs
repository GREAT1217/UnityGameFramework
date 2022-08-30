using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Game
{
    public static class IOUtility
    {
        public static void CreateDirectoryIfNotExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Directory path is null.");
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void SaveFileSafe(string filePath, string text)
        {
            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            SaveFileSafe(path, fileName, text);
        }

        public static void SaveFileSafe(string path, string fileName, string text)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("File name is null.");
                return;
            }

            CreateDirectoryIfNotExists(path);

            string filePath = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                UTF8Encoding utf8Encoding = new UTF8Encoding(false);
                using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                {
                    writer.Write(text);
                }
            }
        }

        public static List<FileInfo> GetFilesWithExtension(string directoryPath, string fileExtension)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*" + fileExtension, SearchOption.AllDirectories);
            List<FileInfo> result = new List<FileInfo>();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                result.Add(fileInfos[i]);
            }
            return result;
        }
    }
}
