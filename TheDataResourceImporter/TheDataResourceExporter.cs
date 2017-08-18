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
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;

namespace TheDataResourceExporter
{
    public class ExportManager
    {
        public static string currentHDFile = "";
        public static DateTime exportStartTime = System.DateTime.Now;

        public static bool forcedStop = false;
        public static string contextInfo = "";
        public static string fileType = "";


        public static string errorMessageTopScope = "";

        public static void resetCounter()
        {
            currentHDFile = "";
            //清空进度信息
            MessageUtil.DoupdateProgressIndicator(0, 0, 0, 0, "");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="AllHDFilePaths">号单存储位置: 可能是实际的号单路径，也可能是号单所在的文件夹路径（1个）</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="storagePaths">提供的存储位置（多个）</param>
        /// <param name="retrivedFileSavePath">提取文件的保存路径</param>
        /// <returns></returns>
        public static bool BeginExport(string[] AllHDFilePaths, string fileType, string[] storagePaths, string retrivedFileSavePath)
        {
            //重置计时
            exportStartTime = System.DateTime.Now;

            contextInfo = $@"上下文信息：
                                {Environment.NewLine}指定的号单路径：{MiscUtil.jsonSerilizeObject(AllHDFilePaths)}
                                {Environment.NewLine}指定的文档类型：{fileType}
                                {Environment.NewLine}数据存储路径：{MiscUtil.jsonSerilizeObject(storagePaths)}
                                {Environment.NewLine}指定的号单路径：{MiscUtil.jsonSerilizeObject(AllHDFilePaths)}                                
                                ";

            ExportManager.fileType = fileType;

            try
            {
                errorMessageTopScope = "";
                MessageUtil.DoAppendTBDetail("开始处理：");
                #region 文件夹模式 查找文件夹下所有的号单文件
                if (!Main.showFileDialog)//文件夹模式
                {
                    if (AllHDFilePaths.Length != 1) //文件夹模式只有一个文件夹路径
                    {
                        var message = $"{MiscUtil.jsonSerilizeObject(AllHDFilePaths)}号单文件夹路径不正确!{contextInfo}";
                        MessageUtil.DoAppendTBDetail(message);
                        LogHelper.WriteExportErrorLog(message);
                        return true;
                    }

                    string dirPath = AllHDFilePaths[0];

                    if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))//路径为空, 路径对应的文件夹不存在
                    {
                        var message = $"号单文件夹路径{dirPath}不正确!{contextInfo}";
                        MessageUtil.showMessageBoxWithErrorLog(message);
                        return true;
                    }

                    string suffixFilter = "txt";

                    List<FileInfo> fileInfos = MiscUtil.getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(dirPath, suffixFilter);
                    var allFoundFilePaths = (from fileTemp in fileInfos
                                             select fileTemp.FullName).Distinct().ToArray();

                    if (allFoundFilePaths.Count() == 0)
                    {
                        var message = $"指定的路径不正确{dirPath}，请选择正确的路径！{contextInfo}";
                        MessageUtil.showMessageBoxWithErrorLog(message);
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
                    //指定的路径即号单路径不需要处理
                }
                #endregion

                #region 对指定的或发现的路径进行处理

                using (DataSourceEntities dataSourceEntites = new DataSourceEntities())
                {
                    dataSourceEntites.Configuration.AutoDetectChangesEnabled = false;
                    dataSourceEntites.Configuration.ProxyCreationEnabled = false;

                    foreach (string HDPath in AllHDFilePaths)//遍历处理需要处理的路径
                    {
                        //强制终止 只有导出完当前号单后才会终止
                        if (forcedStop)
                        {
                            MessageUtil.DoAppendTBDetail("强制终止了导出");
                            break;
                        }

                        currentHDFile = HDPath.Substring(HDPath.LastIndexOf('\\') + 1);

                        try
                        {
                            if (File.Exists(HDPath))
                            {
                                ExportByHDPath(HDPath, fileType, storagePaths, retrivedFileSavePath, dataSourceEntites);
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

                            var errorMsg = $"处理号单{currentHDFile}时发生错误{ex.ToString()}，{Environment.NewLine}错误消息:{ex.Message}详细信息{ex.StackTrace}" + $"{Environment.NewLine}当前文件:{HDPath}，{contextInfo}";
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
                    MessageBox.Show("导出完成，导出发生错误：" + errorMessageTopScope);
                }
                else
                {
                    MessageUtil.DoAppendTBDetail("导出完成，没有错误！");
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

        /// <summary>
        /// 根据号单提取数据
        /// </summary>
        /// <param name="HDPath">号单文件路径</param>
        /// <param name="fileType">数据类型</param>
        /// <param name="storagePaths">存储路径</param>
        /// <param name="retrievedFileSavePath">提取文件保存路径</param>
        /// <param name="dataSourceEntites"></param>
        /// <returns></returns>
        public static bool ExportByHDPath(string HDPath, string fileType, String[] storagePaths, String retrievedFileSavePath, DataSourceEntities dataSourceEntites)
        {
            MessageUtil.DoAppendTBDetail("您选择的资源类型为：" + fileType);
            MessageUtil.DoAppendTBDetail("当前号单文件：" + HDPath);

            FileInfo numFileInfo = new FileInfo(HDPath);

            if (!numFileInfo.Exists)
            {
                MessageUtil.showMessageBoxWithErrorLog($"指定的号单文件{HDPath}错误，文件不存在!");
                return true;
            }


            var parsedResult = parseHaoDanFile(HDPath);
            //号单解析到的 单值列表
            var haoDanFieldDistinctValues = parsedResult.Item1;
            //号单的行数
            var HDFileLineCount = parsedResult.Item2;

            if (null == haoDanFieldDistinctValues || 0 == haoDanFieldDistinctValues.Count())
            {
                MessageUtil.showMessageBoxWithErrorLog($"指定的号单文件{HDPath}有误，没解析到字段值！");
                return true;
            }

            var haoDanDistinctValueCount = haoDanFieldDistinctValues.Count();

            //号单有重复值, 或者有空行
            if (HDFileLineCount != haoDanDistinctValueCount)
            {
                LogHelper.WriteExportErrorLog($"WARN：号单文件{HDPath}行数{HDFileLineCount}和实际解析到的号单值数量{haoDanDistinctValueCount}不一致，号单文件中可能有空行或者重复值");
            }


            //获取号单字段 对应数据库字段
            //数据库字段名
            var haoDanFieldNameStr = "";
            string[] haoDanFieldsArray = null;
            var currentDataResDtl = dataSourceEntites.S_DATA_RESOURCE_TYPES_DETAIL.Where(r => fileType.Equals(r.CHINESE_NAME)).FirstOrDefault();

            if (null != currentDataResDtl)
            {
                haoDanFieldNameStr = currentDataResDtl.HD_FIELD_NAME;
                if (!string.IsNullOrWhiteSpace(haoDanFieldNameStr))
                {
                    haoDanFieldsArray = haoDanFieldNameStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            #region 132 zip
            if ("中国商标".Equals(fileType))
            {
                if (!string.IsNullOrWhiteSpace(haoDanFieldNameStr))
                {
                    MessageUtil.showMessageBoxWithErrorLog("请指定");
                    return true;
                }

                haoDanFieldNameStr = "MARK_CN_ID";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_BRAND, "S_CHINA_BRAND", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
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
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion

            #region 136 zip
            else if ("马德里商标进入中国".Equals(fileType))
            {
                haoDanFieldNameStr = "MARK_CN_ID";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_MADRID_BRAND_ENTER_CHINA, "S_MADRID_BRAND_ENTER_CHINA", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
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
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);

            }
            #endregion


            #region 138 zip
            else if ("美国申请商标".Equals(fileType)) //寻找同目录下
            {
                haoDanFieldNameStr = "SERIAL_NUMBER";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_AMERICA_APPLY_BRAND, "S_AMERICA_APPLY_BRAND", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {

                    if (!String.IsNullOrEmpty(entity.PATH_XML))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PATH_XML);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion


            #region 139 *** 双列，特殊处理
            else if ("美国转让商标".Equals(fileType))
            {
                //var haoDanFieldName1 = "ASSIGNMENT_REEL_NO";
                //var haoDanFieldName2 = "ASSIGNMENT_FRAME_NO";

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                List<S_AMERICA_TRANSFER_BRAND> entityLst = new List<S_AMERICA_TRANSFER_BRAND>();

                foreach (var entityValue in haoDanFieldDistinctValues)
                {
                    var haoDanValueList = entityValue.Split("\\".ToArray());
                    if (2 == haoDanValueList.Count())
                    {
                        var ASSIGNMENT_REEL_NO = haoDanValueList[0];
                        var ASSIGNMENT_FRAME_NO = haoDanValueList[1];
                        var resultRecord = (from entity in dataSourceEntites.S_AMERICA_TRANSFER_BRAND
                                            where ASSIGNMENT_REEL_NO == entity.ASSIGNMENT_REEL_NO && ASSIGNMENT_FRAME_NO == entity.ASSIGNMENT_FRAME_NO
                                            select entity).FirstOrDefault();

                        if (null != resultRecord)
                        {
                            entityLst.Add(resultRecord);
                        }
                    }
                }




                if (null == entityLst || 0 == entityLst.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }


                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in entityLst)
                {

                    if (!String.IsNullOrEmpty(entity.PATH_XML))//XML路径
                    {
                        allRelativePaths.Add(entity.PATH_XML);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{entityLst.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);

            }
            #endregion


            #region 140 未入库
            else if ("美国审判商标".Equals(fileType))
            {

            }
            #endregion 


            #region 147 未入库
            else if ("社内外知识产权图书题录数据".Equals(fileType))
            {

            }
            #endregion


            #region 162 zip
            else if ("中国法院判例初加工数据".Equals(fileType))
            {
                haoDanFieldNameStr = "PN";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_COURTCASE_PROCESS, "S_CHINA_COURTCASE_PROCESS", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {

                    if (!String.IsNullOrEmpty(entity.PATH_XML))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PATH_XML);
                    }

                    if (!String.IsNullOrEmpty(entity.PATH_PDF))
                    {
                        allRelativePaths.Add(entity.PATH_PDF);
                    }

                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion


            #region 172 zip
            else if ("马德里商标购买数据".Equals(fileType))
            {
                haoDanFieldNameStr = "INTREGN";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_MADRID_BRAND_PURCHASE, "S_MADRID_BRAND_PURCHASE", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {

                    if (!String.IsNullOrEmpty(entity.PATH_PIC))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PATH_PIC);
                    }

                    if (!String.IsNullOrEmpty(entity.PATH_XML))
                    {
                        allRelativePaths.Add(entity.PATH_XML);
                    }

                    if (!String.IsNullOrEmpty(entity.PATH_PIC_SF))
                    {
                        allRelativePaths.Add(entity.PATH_PIC_SF);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);

            }
            #endregion


            #region 180 zip
            else if ("中国专利代理知识产权法律法规加工数据".Equals(fileType))
            {
                haoDanFieldNameStr = "LAW_NO";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_PATENT_LAWSPROCESS, "S_CHINA_PATENT_LAWSPROCESS", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {

                    if (!String.IsNullOrEmpty(entity.PATH_XML))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PATH_XML);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);

            }
            #endregion


            #region 183 未入库
            else if ("中国集成电路布图公告及事务数据".Equals(fileType))
            {

            }
            #endregion


            #region 184 未入库
            else if ("中国知识产权海关备案数据".Equals(fileType))
            {

            }
            #endregion


            #region 194 特殊处理 非ZIP  多个记录值
            else if ("中国专利复审（无效）数据".Equals(fileType))
            {
                haoDanFieldNameStr = "APPLICATION_NUMBER";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_PATENT_REVIEW, "S_CHINA_PATENT_REVIEW", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot, true);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {
                    if (!String.IsNullOrEmpty(entity.PATH_XML))//忽略为空的路径
                    {
                        allRelativePaths.Add(CompressUtil.ensureUseBackSlash(entity.PATH_XML));
                    }
                }

                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                //根据查找到的XML相对路径，查找对应的原文 相对路径

                List<String> allDocFilesRelativeFiles = new List<string>();

                MessageUtil.DoSetTBDetail("正在根据XML路径查找原文(*.doc)文件");
                foreach (var xmlRelativePath in allDocFilesRelativeFiles)
                {
                    var filePathParts = xmlRelativePath.Split("\\".ToArray());
                    var xmlFileName = filePathParts.LastOrDefault();
                    var docFileName = xmlFileName.Substring(0, xmlFileName.Length - "xml".Length) + "doc";
                    var parentRelativePath = String.Join("\\", filePathParts.Take(filePathParts.Count() - 1));
                    var DocParentRelativePath = parentRelativePath.Replace("XML", "原文");
                    var DocRelativePath = DocParentRelativePath + "\\" + docFileName;

                    allDocFilesRelativeFiles.Add(DocRelativePath);
                }

                allRelativePaths.AddRange(allDocFilesRelativeFiles.Distinct());

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件。");

                //找寻需要解析的文件并保存到用户指定的位置
                saveRetrivedFilesDirectly(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion


            #region 196 特殊处理 非Zip 多条记录
            else if ("中国专利的判决书数据".Equals(fileType))
            {
                haoDanFieldNameStr = "PATENT_APPLICATION_NUMBER";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_CHINA_PATENT_JUDGMENT, "S_CHINA_PATENT_JUDGMENT", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot, true);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {
                    if (!String.IsNullOrEmpty(entity.PATH_XML))//忽略为空的路径
                    {
                        allRelativePaths.Add(CompressUtil.ensureUseBackSlash(entity.PATH_XML));
                    }
                }

                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }


                //根据查找到的XML相对路径，查找对应的原文 相对路径
                List<String> allDocFilesRelativeFiles = new List<string>();

                MessageUtil.DoSetTBDetail("正在根据XML路径查找原文(*.doc)文件");
                foreach (var xmlRelativePath in allDocFilesRelativeFiles)
                {
                    var filePathParts = xmlRelativePath.Split("\\".ToArray());
                    var xmlFileName = filePathParts.LastOrDefault();
                    var docFileName = xmlFileName.Substring(0, xmlFileName.Length - "xml".Length) + "doc";
                    var parentRelativePath = String.Join("\\", filePathParts.Take(filePathParts.Count() - 1));
                    var DocParentRelativePath = parentRelativePath.Replace("XML", "原文");
                    var DocRelativePath = DocParentRelativePath + "\\" + docFileName;

                    allDocFilesRelativeFiles.Add(DocRelativePath);
                }

                allRelativePaths.AddRange(allDocFilesRelativeFiles.Distinct());

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件。");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesInArchive(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);

            }
            #endregion


            #region 209.1 非zip
            else if ("中国生物序列深加工数据-中文".Equals(fileType))
            {

                haoDanFieldNameStr = "CURRENT_APPLICATION_NUMBER";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_T_BIOLOGICAL_CN, "S_T_BIOLOGICAL_CN", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {

                    if (!String.IsNullOrEmpty(entity.PROJECT_PATH))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PROJECT_PATH);
                    }

                    if (!String.IsNullOrEmpty(entity.SEQUENCE_FILE_PATH))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.SEQUENCE_FILE_PATH);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesDirectly(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion

            #region 209.2 非zip
            else if ("中国生物序列深加工数据-翻译".Equals(fileType))
            {
                haoDanFieldNameStr = "CURRENT_APPLICATION_NUMBER";
                //处理号单
                var HaoDanFieldValuesWithSingleQuot = (from orginValue in haoDanFieldDistinctValues
                                                       select "'" + orginValue + "'").ToList();

                MessageUtil.DoSetTBDetail("正在查询符合条件的记录，请稍候……");

                var resultRecord = queryRecords(dataSourceEntites, dataSourceEntites.S_T_BIOLOGICAL_FY, "S_T_BIOLOGICAL_FY", haoDanFieldNameStr, HaoDanFieldValuesWithSingleQuot);

                if (null == resultRecord || 0 == resultRecord.Count())
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有查询到记录，请核实号单内容!");
                    return true;
                }

                //获取需要提取的文件的相对路径
                List<string> allRelativePaths = new List<string>();

                foreach (var entity in resultRecord)
                {
                    if (!String.IsNullOrEmpty(entity.PROJECT_PATH))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.PROJECT_PATH);
                    }

                    if (!String.IsNullOrEmpty(entity.SEQUENCE_FILE_PATH))//忽略为空的路径
                    {
                        allRelativePaths.Add(entity.SEQUENCE_FILE_PATH);
                    }
                }
                //剔除可能重复的记录
                allRelativePaths = allRelativePaths.Distinct().ToList();

                if (0 == allRelativePaths.Count)
                {
                    MessageUtil.showMessageBoxWithErrorLog("没有解析到需要提取的路径");
                    return true;
                }

                MessageUtil.DoSetTBDetail($"找到{resultRecord.Count}条符合条件的记录，发现{allRelativePaths.Count}个需要提取的文件!");

                //找寻需要解析的文件并保存到用户指定的位置

                saveRetrivedFilesDirectly(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion

            #region 210 中国中药专利翻译数据
            else if ("中国中药专利翻译数据".Equals(fileType))
            {

            }
            #endregion

            #region 211 mdb 直接根据号单内的AP提取，不需要查库
            else if ("中国化学药物专利深加工数据".Equals(fileType))
            {
                //找寻需要解析的文件并保存到用户指定的位置
                List<string> allRelativePaths = (from ap in haoDanFieldDistinctValues
                                                 select $"t_abImage(摘要附图)\\{ap}.gif").ToList();

                saveRetrivedFilesDirectly(storagePaths.ToList(), retrievedFileSavePath, allRelativePaths, HDPath);
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 根据号单文件查询数据库中对应记录
        /// 如果数据库有多条记录，默认全部返回
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entiesContext"></param>
        /// <param name="dbSet">要查询的数据库对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">号单对应的字段名</param>
        /// <param name="HaoDanFieldValuesLstWithSingleQuotesSurrounded">号单值列表</param>
        /// <param name="multiRecs">多条匹配是否全部返回</param>
        /// <returns></returns>
        private static List<TEntity> queryRecords<TEntity>(DataSourceEntities entiesContext, DbSet<TEntity> dbSet, string tableName, string fieldName, List<String> HaoDanFieldValuesLstWithSingleQuotesConclude, bool multiRecs = true) where TEntity : class
        {
            List<TEntity> result = new List<TEntity>();

            foreach (var haoDanFieldValue in HaoDanFieldValuesLstWithSingleQuotesConclude)
            {
                string whereStr = $"where {fieldName} = {haoDanFieldValue}";
                //查询字段值
                string esqlQuery = $"select * from {tableName} {whereStr}";

                if (!multiRecs)//查询记录唯一
                {
                    var targetEntity = dbSet.SqlQuery(esqlQuery).AsNoTracking().ToList().FirstOrDefault();
                    if (null != targetEntity)
                    {
                        result.Add(targetEntity);
                    }
                }
                else //查询结果记录可能有多条
                {
                    var targetEntityEnum = dbSet.SqlQuery(esqlQuery).AsNoTracking().ToList();
                    if (null != targetEntityEnum)
                    {
                        result.AddRange(targetEntityEnum);
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 非压缩包文件的保存
        /// </summary>
        /// <param name="storagePaths"></param>
        /// <param name="retrievedFileSavePath"></param>
        /// <param name="allRelativePaths"></param>
        private static void saveRetrivedFilesDirectly(List<String> storagePaths, String retrievedFileSavePath, List<string> allRelativePaths, String HDPath)
        {
            bool ignoreFileNotFoundError = false;
            int handledCount = 0;
            int totalCount = allRelativePaths.Count();
            foreach (var relativePathDirect in allRelativePaths)
            {
                if (forcedStop)
                {
                    MessageUtil.DoAppendTBDetail("强制终止了导出");
                    break;
                }

                List<String> absoluteFilesPaths = new List<string>();
                //相对路径就是绝对路径 处理样例路径长度不够的情况
                if (File.Exists(relativePathDirect))
                {
                    absoluteFilesPaths.Add(relativePathDirect);
                }

                foreach (var storagePath in storagePaths)
                {
                    string absoluteZipFullPath = getProperAbsPath(storagePath, relativePathDirect);
                    //获取的绝对路径能找到zip文件
                    if (File.Exists(absoluteZipFullPath))
                    {
                        absoluteFilesPaths.Add(absoluteZipFullPath);
                    }
                }

                if (0 == absoluteFilesPaths.Count())//没有找到匹配的文件
                {
                    string message = "没有找到可提取的文件，请检查文件存储位置和文件相对路径组合后的绝对文件路径是否正确!";

                    message = message + Environment.NewLine + $"选择终止(Abort)终止程序运行。{Environment.NewLine}选择重试（Retry）继续寻找下一个文件，找不到要提取的文件继续弹框。{Environment.NewLine}选择忽略(Ignore)，将会遍历处理所有查询到的文件，如果遇到提取不到文件的情况，不再弹出此对话框，可到错误日志里查看遇到的错误";
                    message = message + Environment.NewLine + "请根据提取文件相对路径调整您提供的存储路径，参考信息如下：";
                    message = message + Environment.NewLine + "当前待提取文件相对路径：" + relativePathDirect;
                    message = message + Environment.NewLine + "您提供的存储路径：" + string.Join(Environment.NewLine, storagePaths);

                    LogHelper.WriteExportErrorLog(message);

                    if (!ignoreFileNotFoundError)
                    {
                        var result = MessageBox.Show(message, "", MessageBoxButtons.AbortRetryIgnore);
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
                var firstAbsoluteZipFullPath = absoluteFilesPaths.First();

                String fileName = firstAbsoluteZipFullPath.Split("\\".ToArray()).Last();

                retrievedFileSavePath = ensureNotEndWithBackSlash(retrievedFileSavePath);

                String fullSavePath = "";

                fullSavePath = retrievedFileSavePath + "\\" + relativePathDirect;

                retriveAndSaveFile(firstAbsoluteZipFullPath, fullSavePath);

                handledCount++;

                //更新进度
                MessageUtil.DoupdateProgressIndicator(totalCount, handledCount, 0, 0, HDPath);

                System.GC.Collect();
            }
        }


        /// <summary>
        /// 找到根据号单查询到的需要提取的文件保存到指定的路径
        /// </summary>
        /// <param name="storagePaths"></param>
        /// <param name="retrievedFileSavePath"></param>
        /// <param name="allRelativePaths"></param>
        private static void saveRetrivedFilesInArchive(List<String> storagePaths, String retrievedFileSavePath, List<string> allRelativePaths, String HDPath)
        {
            bool ignoreFileNotFoundError = false;
            int handledCount = 0;
            int totalCount = allRelativePaths.Count();
            foreach (var relativePathWithArchiveInnerPath in allRelativePaths)
            {
                if (forcedStop)
                {
                    MessageUtil.DoAppendTBDetail("强制终止了导出");
                    break;
                }

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
                //相对路径就是绝对路径 处理样例路径长度不够的情况
                if (File.Exists(zipFileRelativeFullName))
                {
                    absoluteZipFilesPaths.Add(zipFileRelativeFullName);
                }

                foreach (var storagePath in storagePaths)
                {
                    string absoluteZipFullPath = getProperAbsPath(storagePath, zipFileRelativeFullName);
                    //获取的绝对路径能找到zip文件
                    if (File.Exists(absoluteZipFullPath))
                    {
                        absoluteZipFilesPaths.Add(absoluteZipFullPath);
                    }
                }

                if (0 == absoluteZipFilesPaths.Count())//没有找到匹配的文件
                {
                    string message = "没有找到可提取的文件，请检查文件存储位置和文件相对路径组合后的绝对文件路径是否正确!";

                    message = message + Environment.NewLine + $"选择终止(Abort)终止程序运行。{Environment.NewLine}选择重试（Retry）继续寻找下一个文件，找不到要提取的文件继续弹框。{Environment.NewLine}选择忽略(Ignore)，将会遍历处理所有查询到的文件，如果遇到提取不到文件的情况，不再弹出此对话框，可到错误日志里查看遇到的错误";
                    message = message + Environment.NewLine + "请根据提取文件相对路径调整您提供的存储路径，参考信息如下：";
                    message = message + Environment.NewLine + "当前待提取文件相对路径：" + relativePathWithArchiveInnerPath;
                    message = message + Environment.NewLine + "您提供的存储路径：" + string.Join(Environment.NewLine, storagePaths);

                    LogHelper.WriteExportErrorLog(message);

                    if (!ignoreFileNotFoundError)
                    {
                        var result = MessageBox.Show(message, "", MessageBoxButtons.AbortRetryIgnore);
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

                handledCount++;

                //更新进度
                MessageUtil.DoupdateProgressIndicator(totalCount, handledCount, 0, 0, HDPath);

                System.GC.Collect();
            }
        }

        /// <summary>
        /// 保存文件 文件保存在压缩包中
        /// </summary>
        /// <param name="absoluteFullRetrivePath"></param>
        /// <param name="absoluteFullSavePath"></param>
        /// <param name="achiveInnerPath"></param>
        private static void retriveAndSaveFile(String absoluteFullRetrivePath, String absoluteFullSavePath, String achiveInnerPath)
        {
            FileInfo retrivedFile = new FileInfo(absoluteFullRetrivePath);

            try
            {
                using (IArchive archive = SharpCompress.Archive.ArchiveFactory.Open(absoluteFullRetrivePath))
                {
                    var targetArchiveEntry = CompressUtil.getEntryByKey(archive, achiveInnerPath);
                    FileInfo targetFileInfo = new FileInfo(absoluteFullSavePath);
                    Directory.CreateDirectory(targetFileInfo.Directory.FullName);
                    targetArchiveEntry.WriteToFile(absoluteFullSavePath);
                }
            }
            catch (Exception ex)
            {
                //提取文件失败
                var message = $"提取文件{absoluteFullRetrivePath}失败：{Environment.NewLine}zip包：{absoluteFullRetrivePath}{Environment.NewLine}保存路径：{absoluteFullSavePath}{Environment.NewLine}包内路径：{achiveInnerPath}{Environment.NewLine}错误信息：{ex.Message}{Environment.NewLine}错误详情：{ex.StackTrace}";
                LogHelper.WriteExportErrorLog(message);
                MessageUtil.DoSetTBDetail(message);
            }

        }


        /// <summary>
        /// 保存文件  目标文件直接存在，不在压缩包中
        /// </summary>
        /// <param name="absoluteFullRetrivePath"></param>
        /// <param name="absoluteFullSavePath"></param>
        /// <param name="achiveInnerPath"></param>
        private static void retriveAndSaveFile(String absoluteFullRetrivePath, String absoluteFullSavePath)
        {
            FileInfo retrivedFile = new FileInfo(absoluteFullRetrivePath);

            try
            {
                FileInfo targetFileInfo = new FileInfo(absoluteFullSavePath);
                Directory.CreateDirectory(targetFileInfo.Directory.FullName);
                targetFileInfo.CopyTo(absoluteFullSavePath);
            }
            catch (Exception ex)
            {
                //提取文件失败
                var message = $"提取文件{absoluteFullRetrivePath}失败：{Environment.NewLine}文件：{absoluteFullRetrivePath}{Environment.NewLine}保存路径：{absoluteFullSavePath}{Environment.NewLine}错误信息：{ex.Message}{Environment.NewLine}错误详情：{ex.StackTrace}";
                LogHelper.WriteExportErrorLog(message);
                MessageUtil.DoSetTBDetail(message);
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
        /// 获取压缩包等文件绝对路径
        /// </summary>
        /// <param name="storagePath"></param>
        /// <param name="fileFullRelativeName"></param>
        /// <returns></returns>
        private static String getProperAbsPath(String storagePath, String fileFullRelativeName)
        {
            storagePath = ensureNotEndWithBackSlash(storagePath);
            fileFullRelativeName = ensureNotStartWithBackSlash(fileFullRelativeName);
            return storagePath + "\\" + fileFullRelativeName;
        }

        /// <summary>
        /// 解析号单
        /// </summary>
        /// <param name="fileHaoDanPath"></param>
        /// <returns></returns>
        private static Tuple<List<String>, int> parseHaoDanFile(string fileHaoDanPath)
        {
            try
            {
                //解析号单
                StreamReader sReader = new StreamReader(new FileStream(fileHaoDanPath, FileMode.Open));

                var haoDanFieldValues = new List<string>();

                int lineCount = 0;
                //解析号单字段值
                while (!sReader.EndOfStream)
                {                    
                    var currentLine = sReader.ReadLine();
                    lineCount++;
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        haoDanFieldValues.Add(currentLine.Trim());
                    }
                }
                //返回元组, 去重后的元素 及去重前数量
                return Tuple.Create(haoDanFieldValues.Distinct().ToList(), lineCount) ;
            }
            catch (Exception ex)
            {
                MessageUtil.showMessageBoxWithErrorLog($"解析号单文件{fileHaoDanPath}发生错误，{ex.Message}{ex.StackTrace}");
                throw;
            }
        }
    }
}