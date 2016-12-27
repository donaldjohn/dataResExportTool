﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpCompress.Archive;
using System.Configuration;
using SharpCompress.Common;
using System.IO;
using SharpCompress.Archive.Zip;
using SharpCompress.Archive.SevenZip;
using SharpCompress.Archive.GZip;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Tar;

namespace TheDataResourceExporter.Utils
{
    class CompressUtil
    {
        public  static string tempDir = ConfigurationManager.AppSettings["tempDir"];
        /***
         * 将当前的entry写到指定目录,保留文件信息,如果文件存在, 覆盖文件
         * 返回临时目录
         * */ 
        public static string writeEntryToTemp(IArchiveEntry entry)
        {
            //string tempDir = ConfigurationManager.AppSettings["tempDir"];
            bool successed = false;
            try
            {
                entry.WriteToDirectory(tempDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                successed = true;
            }
            catch(Exception ex)
            {
                if(File.Exists(Path.Combine(tempDir, entry.Key)))
                {
                    string msg = "!!!!!!!!!!发生异常, 但是文件解压成功：" + ex.Message + ex.StackTrace;
                    MessageUtil.DoAppendTBDetail(msg);
                    LogHelper.WriteExportErrorLog(msg);
                    successed = true;
                }
                else
                {
                    string msg = "!!!!!!!!!!发生异常, 解压失败：" + ex.Message + ex.StackTrace;
                    MessageUtil.DoAppendTBDetail(msg);
                    LogHelper.WriteExportErrorLog(msg);
                    successed = false;   
                }
            }
                if(successed)
            {
               return Path.Combine(tempDir, entry.Key);
            }
                else
            {
                return "";
            }
        }
        
        /***
         * 解压压缩包的所有条目到指定目录, 返回临时目录
         **/
        public static string extractAllEntiresInArchive(IArchive archive)
        {
            archive.WriteToDirectory(tempDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite | ExtractOptions.PreserveAttributes | ExtractOptions.PreserveFileTime);
            return tempDir;
        }
        
        /**
         * 返回当前entry解压后的路径
         * */
        public static string getExtractedFullPath(IArchiveEntry entry)
        {
            return Path.Combine(tempDir, entry.Key);
        }

        /**
         * 删除指定Entry对象的临时文件 只删除文件, 不操作目录
         * */
        public static bool removeEntryTempFile(IArchiveEntry entry)
        {
            bool deleted = true;
            try
            {
                string fullPath = Path.Combine(tempDir, entry.Key);
                FileInfo entryFile = new FileInfo(fullPath);
                if(entryFile.Exists)
                {
                    entryFile.Delete();
                }
            }
            catch(Exception ex)
            {
                deleted = false;
            }
            return deleted;
        }

        public static bool isSupportedArchive(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            try
            {
                using (var stream = fileInfo.OpenRead())
                {
                    if (ZipArchive.IsZipFile(stream, null))
                    {
                        stream.Dispose();
                        return true;
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    if (SevenZipArchive.IsSevenZipFile(stream))
                    {
                        stream.Dispose();
                        return true;

                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    if (GZipArchive.IsGZipFile(stream))
                    {
                        stream.Dispose();
                        return true;
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    if (RarArchive.IsRarFile(stream, Options.None))
                    {
                        stream.Dispose();
                        return true;
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    if (TarArchive.IsTarFile(stream))
                    {
                        stream.Dispose();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 压缩包内目录类型 移除多余的/, 统一使用\\做分隔符
        /// </summary>
        /// <param name="entryKey"></param>
        /// <returns></returns>
        public static string ensureUseBackSlash(string entryKey)
        {
            if (entryKey.EndsWith("/"))
            {
                entryKey = entryKey.Substring(0, entryKey.Length - 1);
            }
            if (entryKey.Contains("/"))
            {
                entryKey = entryKey.Replace('/', '\\');
            }
            return entryKey;
        }

        //获取条目的深度
        public static int getEntryDepth(string originalEntryKey)
        {
            string neatedPath = ensureUseBackSlash(originalEntryKey);
            return neatedPath.Split('\\').Length;
        }

        //获取文件类条目的父目录
        public static string getFileEntryParentPath(string fileEntryKey)
        {
            if (fileEntryKey.Contains("/"))
            {
                fileEntryKey = fileEntryKey.Replace('/', '\\');
            }

            string[] pathParts = fileEntryKey.Split('\\');

            //拼接
            string parentFullPath = string.Join("\\", pathParts, 0, pathParts.Length - 1);
            return parentFullPath;
        }

        //返回条目名称, 不包含父条目信息
        public static string getEntryShortName(string entryKey)
        {
            entryKey = ensureUseBackSlash(entryKey);
            return entryKey.Split('\\').LastOrDefault();
        }

        /// <summary>
        /// 返回符合条件的同目录的entry
        /// </summary>
        /// <param name="achive"></param>
        /// <param name="entry"></param>
        /// <param name="suffix">查询条件, 以suffix结尾，大小写不敏感</param>
        /// <returns></returns>
        public static IArchiveEntry getSpecifiedSiblingEntry(IArchive achive, string entryKey, string suffix)
        {
            var target = (from entryTemp in achive.Entries
                         where (!entryTemp.Key.Equals(entryKey)) && (getFileEntryParentPath(entryTemp.Key).Equals(getFileEntryParentPath(entryKey))) && entryTemp.Key.ToUpper().EndsWith(suffix.ToUpper())
                         select entryTemp).FirstOrDefault();
            return target;
        }


        /// <summary>
        /// 通过key查找entry
        /// </summary>
        /// <param name="achive"></param>
        /// <param name="entryKey"></param>
        /// <returns></returns>
        public static IArchiveEntry getEntryByKey(IArchive achive, string entryKey)
        {
            var target = (from entryTemp in achive.Entries
                          where entryTemp.Key.Equals(entryKey)
                          select entryTemp).FirstOrDefault();
            return target;
        }

        /// <summary>
        /// 查找符合条件的子entry
        /// </summary>
        /// <param name="achive"></param>
        /// <param name="entryKey"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static IArchiveEntry getChildEntryWhithSuffix(IArchive achive, string entryKey, string suffix)
        {
            var target = (from entryTemp in achive.Entries
                          where ensureUseBackSlash(entryTemp.Key).Contains(ensureUseBackSlash(entryKey)) && getEntryDepth(entryTemp.Key) == (getEntryDepth(entryKey) + 1) /*确保是子目录*/ && entryTemp.Key.ToUpper().EndsWith(suffix.ToUpper())
                          select entryTemp).FirstOrDefault();
            return target;
        }

        /// <summary>
        /// 查找符合条件的所有后代条目
        /// </summary>
        /// <param name="achive"></param>
        /// <param name="entryKey"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static IArchiveEntry getDescendantEntryWhithSuffix(IArchive achive, string entryKey, string suffix)
        {
            var target = (from entryTemp in achive.Entries
                          where ensureUseBackSlash(entryTemp.Key).Contains(ensureUseBackSlash(entryKey))  &&  entryTemp.Key.ToUpper().EndsWith(suffix.ToUpper())
                          select entryTemp).FirstOrDefault();
            return target;
        }
    }
}
