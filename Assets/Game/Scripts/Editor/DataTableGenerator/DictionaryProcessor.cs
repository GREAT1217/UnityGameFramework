using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework;
using UnityEngine;

namespace Game.Editor
{
    public sealed class DictionaryProcessor
    {
        /// <summary>
        /// 注释行分隔符。
        /// </summary>
        private const string CommentLineSeparator = "#";

        /// <summary>
        /// 数据分隔符。
        /// </summary>
        private static readonly char[] DataSplitSeparators = new char[] { '\t' };

        /// <summary>
        /// 数据修剪符。
        /// </summary>
        private static readonly char[] DataTrimSeparators = new char[] { '\"' };

        /// <summary>
        /// 名称行字符数组。
        /// </summary>
        private readonly string[] m_NameRow;

        /// <summary>
        /// 内容起始行索引。
        /// </summary>
        private readonly int m_ContentStartRow;

        /// <summary>
        /// 数据表的所有原始值。
        /// </summary>
        private readonly string[][] m_RawValues;

        public DictionaryProcessor(string fileName, Encoding encoding, int nameRow, int contentStartRow)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new GameFrameworkException("File name is invalid.");
            }

            // if (!fileName.EndsWith(".txt", StringComparison.Ordinal))
            // {
            //     throw new GameFrameworkException(Utility.Text.Format("File '{0}' is not a txt.", fileName));
            // }

            if (!File.Exists(fileName))
            {
                throw new GameFrameworkException(Utility.Text.Format("File '{0}' is not exist.", fileName));
            }

            string[] lines = File.ReadAllLines(fileName, encoding);
            int rawRowCount = lines.Length; // 原始行数

            int rawColumnCount = 0; // 原始列数
            List<string[]> rawValues = new List<string[]>(); // 原始值
            for (int i = 0; i < lines.Length; i++)
            {
                string[] rawValue = lines[i].Split(DataSplitSeparators);
                for (int j = 0; j < rawValue.Length; j++)
                {
                    rawValue[j] = rawValue[j].Trim(DataTrimSeparators);
                }

                if (i == 0)
                {
                    rawColumnCount = rawValue.Length;
                }
                else if (rawValue.Length != rawColumnCount)
                {
                    throw new GameFrameworkException(Utility.Text.Format("File '{0}', raw Column is '{2}', but line '{1}' column is '{3}'.", fileName, i, rawColumnCount, rawValue.Length));
                }

                rawValues.Add(rawValue);
            }

            m_RawValues = rawValues.ToArray();

            if (nameRow < 0)
            {
                throw new GameFrameworkException(Utility.Text.Format("Name row '{0}' is invalid.", nameRow));
            }

            if (contentStartRow < 0)
            {
                throw new GameFrameworkException(Utility.Text.Format("Content start row '{0}' is invalid.", contentStartRow.ToString()));
            }

            if (nameRow >= rawRowCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Name row '{0}' >= raw row count '{1}' is not allow.", nameRow, rawRowCount));
            }

            if (contentStartRow > rawRowCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Content start row '{0}' > raw row count '{1}' is not allow.", contentStartRow, rawRowCount));
            }

            m_NameRow = m_RawValues[nameRow];
            m_ContentStartRow = contentStartRow;
        }

        /// <summary>
        /// 原始行数。
        /// </summary>
        public int RawRowCount
        {
            get
            {
                return m_RawValues.Length;
            }
        }

        /// <summary>
        /// 原始列数。
        /// </summary>
        public int RawColumnCount
        {
            get
            {
                return m_RawValues.Length > 0 ? m_RawValues[0].Length : 0;
            }
        }

        /// <summary>
        /// 内容起始行索引。
        /// </summary>
        public int ContentStartRow
        {
            get
            {
                return m_ContentStartRow;
            }
        }

        /// <summary>
        /// 是否是注释行。
        /// </summary>
        /// <param name="rawRow"></param>
        /// <returns></returns>
        /// <exception cref="GameFrameworkException"></exception>
        public bool IsCommentRow(int rawRow)
        {
            if (rawRow < 0 || rawRow >= RawRowCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Raw row '{0}' is out of range.", rawRow));
            }

            return GetValue(rawRow, 0).StartsWith(CommentLineSeparator, StringComparison.Ordinal);
        }

        /// <summary>
        /// 是否是注释列。
        /// </summary>
        /// <param name="rawColumn"></param>
        /// <returns></returns>
        /// <exception cref="GameFrameworkException"></exception>
        public bool IsCommentColumn(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn));
            }

            string columnName = GetName(rawColumn);
            return string.IsNullOrEmpty(columnName) || columnName.Contains(CommentLineSeparator);
        }

        /// <summary>
        /// 获取列的名称。
        /// </summary>
        /// <param name="rawColumn"></param>
        /// <returns></returns>
        /// <exception cref="GameFrameworkException"></exception>
        public string GetName(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn));
            }

            return m_NameRow[rawColumn];
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="rawRow">原始行索引。</param>
        /// <param name="rawColumn">原始列索引。</param>
        /// <returns>值。</returns>
        /// <exception cref="GameFrameworkException"></exception>
        public string GetValue(int rawRow, int rawColumn)
        {
            if (rawRow < 0 || rawRow >= RawRowCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Raw row '{0}' is out of range.", rawRow));
            }

            if (rawColumn < 0 || rawColumn >= RawColumnCount)
            {
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn));
            }

            return m_RawValues[rawRow][rawColumn];
        }

        public bool GenerateDataFile(string outputFileName)
        {
            if (string.IsNullOrEmpty(outputFileName))
            {
                throw new GameFrameworkException("Output file name is invalid.");
            }

            try
            {
                using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8))
                    {
                        for (int rawRow = ContentStartRow; rawRow < RawRowCount; rawRow++)
                        {
                            if (IsCommentRow(rawRow))
                            {
                                continue;
                            }

                            for (int rawColumn = 0; rawColumn < RawColumnCount; rawColumn++)
                            {
                                if (IsCommentColumn(rawColumn))
                                {
                                    continue;
                                }

                                try
                                {
                                    string value = GetValue(rawRow, rawColumn);
                                    binaryWriter.Write(value);
                                }
                                catch
                                {
                                    Debug.LogError(Utility.Text.Format("Generate config file failure. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}'.", outputFileName, rawRow.ToString(), rawColumn.ToString()));
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(Utility.Text.Format("Parse '{0}' failure, exception is '{1}'.", outputFileName, exception));
                return false;
            }
        }
    }
}
