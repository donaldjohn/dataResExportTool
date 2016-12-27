using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using TheDataResourceExporter.Models;
using TheDataResourceExporter.Utils;
using System.Data.OleDb;
using SharpCompress.Archive;
using SharpCompress.Common;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading.Tasks;
using System.Xml;
using UpdateDataFromExcel.Utils;
using System.Windows.Forms;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using System.Text.RegularExpressions;
using TheDataResourceImporter;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;

namespace TheDataResourceExporter
{
    public class ExportManger
    {
        public static string currentFile = "";
        //totalCount, handledCount, handledXMLCount, handledDirCount
        public static int totalCount = 0;
        public static int handledCount = 0;
        public static int handledXMLCount = 0;
        public static int withExceptionButExtracted = 0;
        public static int withExcepthonAndFiled2Exracted = 0;
        public static int fileCount = 0;
        public static DateTime importStartTime = System.DateTime.Now;
        public static DateTime bathStartTime = System.DateTime.Now;
        public static bool forcedStop = false;

        public static string bathId = "";

        public static string errorMessageTopScope = "";


        public static int dealCount = 0;
        public static int lostCount = 0;

        public static void resetCounter()
        {
            currentFile = "";
            totalCount = 0;
            handledCount = 0;
            withExceptionButExtracted = 0;
            withExcepthonAndFiled2Exracted = 0;
            fileCount = 0;
            ExportManger.importStartTime = System.DateTime.Now;
            //清空进度信息
            MessageUtil.DoupdateProgressIndicator(0, 0, 0, 0, "");
        }

        public static bool BeginExport(string[] AllHDFilePaths, string fileType, string[] storagePaths, string retrivedFileSavePath)
        {
            try
            {
                errorMessageTopScope = "";
                fileCount = AllHDFilePaths.Length;

                MessageUtil.DoAppendTBDetail("开始处理：");
                #region 文件夹模式 查找文件夹下所有的号单文件
                if (!Main.showFileDialog)//文件夹模式
                {
                    if (AllHDFilePaths.Length != 1) //文件夹模式只有一个文件夹路径
                    {
                        var message = $"{MiscUtil.jsonSerilizeObject(AllHDFilePaths)}文件夹路径不正确";
                        MessageUtil.DoAppendTBDetail(message);
                        LogHelper.WriteExportErrorLog(message);
                        return true;
                    }

                    string dirPath = AllHDFilePaths[0];

                    if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))//路径为空, 路径对应的文件夹不存在
                    {
                        var message = $"文件夹路径{dirPath}不正确";
                        MessageUtil.showMessageBoxWithErrorLog(message);
                        return true;
                    }

                    string suffixFilter = "txt";

                    List<FileInfo> fileInfos = MiscUtil.getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(dirPath, suffixFilter);
                    var allFoundFilePaths = (from fileTemp in fileInfos
                                             select fileTemp.FullName).Distinct().ToArray();

                    if (allFoundFilePaths.Count() == 0)
                    {
                        MessageBox.Show("没有找到指定的文件，请选择正确的路径！");
                        LogHelper.WriteExportErrorLog("没有找到指定的文件");


                        MessageUtil.showMessageBoxWithErrorLog($"指定的路径不正确{dirPath}，请选择正确的路径！");
                        return true;
                    }
                    else
                    {
                        MessageUtil.DoAppendTBDetail($"发现{allFoundFilePaths.Count()}个号单文件,它们是{Environment.NewLine + string.Join(Environment.NewLine, allFoundFilePaths)}");
                        AllHDFilePaths = allFoundFilePaths;
                    }
                }
                else//文件模式 只允许单选 2016年12月15日15:41:54
                {

                }
                #endregion

                #region 对指定的或发现的路径进行处理

                using (DataSourceEntities dataSourceEntites = new DataSourceEntities())
                {
                    dataSourceEntites.Configuration.AutoDetectChangesEnabled = false;
                    dataSourceEntites.Configuration.ProxyCreationEnabled = false;

                    foreach (string HDPath in AllHDFilePaths)//遍历处理需要处理的路径
                    {
                        //强制终止
                        if (forcedStop)
                        {
                            MessageUtil.DoAppendTBDetail("强制终止了导出");
                            break;
                        }
                        currentFile = HDPath.Substring(HDPath.LastIndexOf('\\') + 1);
                        try
                        {
                            if (File.Exists(HDPath))
                            {
                                ExportByPath(HDPath, fileType, storagePaths, retrivedFileSavePath, dataSourceEntites);
                            }
                            else
                            {
                                MessageUtil.showMessageBoxWithErrorLog($"指定的号单文件不存在{HDPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("对象名:“Main”"))
                            {
                                continue;
                            }

                            var errorMsg = $"导出文件{currentFile}时发生错误{ex.ToString()}，{Environment.NewLine}错误消息:{ex.Message}详细信息{ex.StackTrace}" + $"{Environment.NewLine}当前文件:{HDPath}";
                            MessageUtil.DoSetTBDetail($"发生异常:{errorMsg}");
                            LogHelper.WriteExportErrorLog(errorMsg);
                            errorMessageTopScope += errorMsg;
                            continue;
                        }
                    }
                }
                System.GC.Collect();
                GC.WaitForPendingFinalizers();
                #endregion


                if (!string.IsNullOrEmpty(errorMessageTopScope))
                {
                    MessageBox.Show("导出发生错误：" + errorMessageTopScope);
                }
                else
                {
                    MessageUtil.DoAppendTBDetail("导出完成, 没有错误");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"{Environment.NewLine}错误信息{ex.Message}：{ex.StackTrace}";
                MessageUtil.DoSetTBDetail($"发生异常:{errorMsg}");
                LogHelper.WriteExportErrorLog(errorMsg);
                errorMessageTopScope += errorMsg;
                MessageBox.Show(errorMessageTopScope);
            }
            return true;
        }


        public static bool ExportByPath(string HDPath, string fileType, String[] storagePaths, String retrievedFileSavePath, DataSourceEntities dataSourceEntites)
        {
            currentFile = HDPath;

            MessageUtil.DoAppendTBDetail("您选择的资源类型为：" + fileType);
            MessageUtil.DoAppendTBDetail("当前号单文件：" + HDPath);

            FileInfo numFileInfo = new FileInfo(HDPath);

            if (!numFileInfo.Exists)
            {
                MessageUtil.showMessageBoxWithErrorLog("指定的号单文件有误，号单文件不存在");
                return true;
            }

            //数据库字段名
            var haoDanFieldName = "";
            var haoDanFieldValues = parseHaoDanFile(HDPath);

            if (null == haoDanFieldValues || 0 == haoDanFieldValues.Count())
            {
                MessageUtil.showMessageBoxWithErrorLog("指定的号单文件有误，没解析到字段值");
                return true;
            }

            //132
            if (fileType == "中国商标")
            {
                haoDanFieldName = "MARK_CN_ID";
                var whereStr = "";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var result = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_BRAND, "S_CHINA_BRAND", haoDanFieldName, HaoDanFieldValuesWithSingleQuot);

                if (null == result || 0 == result.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in result)
                {
                    if (!String.IsNullOrEmpty(entity.PATH_FILE))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PATH_FILE);
                    }

                    if (!String.IsNullOrEmpty(entity.PATH_JPG))
                    {
                        allRelativePaths.Add(entity.PATH_JPG);
                    }

                    if (!String.IsNullOrEmpty(entity.PATH_JPG_SF))
                    {
                        allRelativePaths.Add(entity.PATH_JPG_SF);
                    }
                }


                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{result.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths);


            }

            return true;
        }

        /// <summary>
        /// 查询号单文件中指定的记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entiesContext"></param>
        /// <param name="dbSet"></param>
        /// <param name="tableName"></param>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        private static List<TEntity> queryRecords<TEntity>(DataSourceEntities entiesContext, DbSet<TEntity> dbSet, string tableName, string fieldName, List<String> HaoDanFieldValuesLst) where TEntity : class
        {
            List<TEntity> result = new List<TEntity>();

            foreach (var haoDanFieldValue in HaoDanFieldValuesLst)
            {
                string whereStr = $"where {fieldName} = {haoDanFieldValue}";
                //查询字段值
                string esqlQuery = $"select * from {tableName} {whereStr}";
                var targetEntity = dbSet.SqlQuery(esqlQuery).AsNoTracking().ToList().FirstOrDefault();
                if (null != targetEntity)
                {
                    result.Add(targetEntity);
                }
            }
            return result;
        }

        /// <summary>
        /// 找到根据号单查询到的需要提取的文件保存到指定的路径
        /// </summary>
        /// <param name="storagePaths"></param>
        /// <param name="retrievedFileSavePath"></param>
        /// <param name="allRelativePaths"></param>
        private static void saveRetrivedFilesInArchive(List<String> storagePaths, String retrievedFileSavePath, List<string> allRelativePaths)
        {
            bool ignoreFileNotFoundError = false;
            foreach (var relativePathWithArchiveInnerPath in allRelativePaths)
            {
                var index = relativePathWithArchiveInnerPath.IndexOf(".zip");
                if (-1 == index)
                {
                    index = relativePathWithArchiveInnerPath.IndexOf(".ZIP");
                }
                if (-1 == index)
                {
                    LogHelper.WriteExportErrorLog($"相对路径{relativePathWithArchiveInnerPath}错误");
                    continue;
                }

                var zipFileFullNameLength = index + 4;

                //zip包完整相对路径
                var zipFileRelativeFullName = relativePathWithArchiveInnerPath.Substring(0, zipFileFullNameLength);

                //压缩包内路径
                var achiveInnerPath = relativePathWithArchiveInnerPath.Substring(zipFileFullNameLength);

                List<String> absoluteZipFilesPaths = new List<string>();
                //相对路径就是绝对路径 处理样例那种路径长度不够的情况
                if (File.Exists(zipFileRelativeFullName))
                {
                    absoluteZipFilesPaths.Add(zipFileRelativeFullName);
                }

                foreach (var storagePath in storagePaths)
                {
                    string absoluteZipFullPath = getProperZipAbsPath(storagePath, zipFileRelativeFullName);
                    //获取的绝对路径能找到zip文件
                    if (File.Exists(absoluteZipFullPath))
                    {
                        absoluteZipFilesPaths.Add(absoluteZipFullPath);
                    }
                }

                if (0 == absoluteZipFilesPaths.Count())//没有找到匹配的文件
                {
                    string message = "没有找到可提取的文件，请检查文件存储位置和文件相对路径组合后的绝对文件路径是否正确!";

                    message = message + Environment.NewLine + "选择终止(Abort)终止程序运行，重试（Retry）继续寻找下一个文件，找不到要提取的文件继续弹框,忽略(Ignore)如果依然提取不到文件，不再弹出此对话框";
                    message = message + Environment.NewLine + "当前待提取文件相对路径：" + achiveInnerPath;
                    message = message + Environment.NewLine + "您提供的存储路径：" + string.Join(Environment.NewLine, storagePaths);

                    LogHelper.WriteExportErrorLog(message);

                    if (!ignoreFileNotFoundError)
                    {
                        var result = MessageBox.Show(message, "没找到文件", MessageBoxButtons.AbortRetryIgnore);
                        if (result == DialogResult.Retry)
                        {
                            continue;
                        }
                        if (result == DialogResult.Ignore)//如果选择忽略
                        {
                            ignoreFileNotFoundError = true;
                        }
                        if (result == DialogResult.Abort)//如果选择放弃 终止程序运行
                        {
                            return;
                        }
                    }
                }


                //如果找到多个文件，只取第一个文件
                var firstAbsoluteZipFullPath = absoluteZipFilesPaths.First();
                achiveInnerPath = ensureNotStartWithBackSlash(achiveInnerPath);
                String zipFileName = firstAbsoluteZipFullPath.Split("\\".ToArray()).Last();

                retrievedFileSavePath = ensureNotEndWithBackSlash(retrievedFileSavePath);

                Directory.CreateDirectory(retrievedFileSavePath);

                String fullSavePath = "";
                if (Regex.IsMatch(relativePathWithArchiveInnerPath, "[A-Za-z]+:\\.*"))
                {
                    fullSavePath = retrievedFileSavePath + "\\" + zipFileName + "\\" + achiveInnerPath;
                }
                else
                {
                    fullSavePath = retrievedFileSavePath + "\\" + relativePathWithArchiveInnerPath;
                }


                retriveAndSaveFile(firstAbsoluteZipFullPath, fullSavePath, achiveInnerPath);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="absoluteFullRetrivePath"></param>
        /// <param name="absoluteFullSavePath"></param>
        /// <param name="achiveInnerPath"></param>
        private static void retriveAndSaveFile(String absoluteFullRetrivePath, String absoluteFullSavePath, String achiveInnerPath)
        {
            FileInfo retrivedFile = new FileInfo(absoluteFullRetrivePath);

            using (IArchive archive = SharpCompress.Archive.ArchiveFactory.Open(absoluteFullRetrivePath))
            {
                var targetArchiveEntry = CompressUtil.getEntryByKey(archive, achiveInnerPath);
                FileInfo targetFileInfo = new FileInfo(absoluteFullSavePath);
                Directory.CreateDirectory(targetFileInfo.Directory.FullName);
                //targetFileInfo.Create();
                targetArchiveEntry.WriteToFile(absoluteFullSavePath);
            }
        }

        /// <summary>
        /// 确保路径不以反斜杠结尾
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ensureNotEndWithBackSlash(String path)
        {
            path = CompressUtil.ensureUseBackSlash(path);
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        /// <summary>
        /// 确保路径不以反斜杠开头
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static String ensureNotStartWithBackSlash(String path)
        {
            path = CompressUtil.ensureUseBackSlash(path);
            if (path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }
            return path;
        }

        /// <summary>
        /// 获取压缩包绝对路径
        /// </summary>
        /// <param name="storagePath"></param>
        /// <param name="zipFileFullName"></param>
        /// <returns></returns>
        private static String getProperZipAbsPath(String storagePath, String zipFileFullName)
        {
            storagePath = ensureNotEndWithBackSlash(storagePath);
            zipFileFullName = ensureNotStartWithBackSlash(zipFileFullName);
            return storagePath + "\\" + zipFileFullName;
        }



        /// <summary>
        /// 解析号单
        /// </summary>
        /// <param name="fileHaoDanPath"></param>
        /// <returns></returns>
        private static List<String> parseHaoDanFile(string fileHaoDanPath)
        {
            try
            {
                //解析号单
                StreamReader sReader = new StreamReader(new FileStream(fileHaoDanPath, FileMode.Open));

                var haoDanFieldValues = new List<string>();

                //解析号单字段值
                while (!sReader.EndOfStream)
                {
                    var currentLine = sReader.ReadLine();
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        haoDanFieldValues.Add(currentLine.Trim());
                    }
                }
                //去重
                return haoDanFieldValues.Distinct().ToList();
            }
            catch (Exception ex)
            {
                MessageUtil.showMessageBoxWithErrorLog($"解析号单文件{fileHaoDanPath}发生错误，{ex.Message}{ex.StackTrace}");
                throw;
            }
        }
    }
}