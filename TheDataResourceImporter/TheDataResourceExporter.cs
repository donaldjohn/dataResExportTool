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

        public static bool BeginExport(string[] AllFilePaths, string fileType)
        {
            try
            {
                errorMessageTopScope = "";
                //importStartTime = System.DateTime.Now;
                fileCount = AllFilePaths.Length;

                MessageUtil.DoAppendTBDetail("开始处理：");
                #region 文件夹模式 解析符合条件的文件

                if (!Main.showFileDialog)//文件夹模式
                {
                    if (AllFilePaths.Length != 1) //文件夹模式只有一个文件夹路径
                    {
                        var message = $"{MiscUtil.jsonSerilizeObject(AllFilePaths)}文件夹路径不正确";
                        MessageUtil.DoAppendTBDetail(message);
                        LogHelper.WriteImportErrorLog(message);
                        return true;
                    }

                    string dirPath = AllFilePaths[0];

                    if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))//路径为空, 路径对应的文件夹不存在
                    {
                        var message = $"文件夹路径{dirPath}不正确";
                        MessageUtil.DoAppendTBDetail(message);
                        LogHelper.WriteImportErrorLog(message);
                        return true;
                    }

                    string suffixFilter = "txt";

                    List<FileInfo> fileInfos = MiscUtil.getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(dirPath, suffixFilter);
                    var allFoundFilePaths = (from fileTemp in fileInfos
                                             select fileTemp.FullName).Distinct().ToArray();

                    if (allFoundFilePaths.Count() == 0)
                    {
                        MessageBox.Show("没有找到指定的文件，请选择正确的路径！");
                        LogHelper.WriteImportErrorLog("没有找到指定的文件");
                        return true;
                    }
                    else
                    {
                        MessageUtil.DoAppendTBDetail($"发现{allFoundFilePaths.Count()}个符合条件的文件,它们是{Environment.NewLine + string.Join(Environment.NewLine, allFoundFilePaths)}");
                        AllFilePaths = allFoundFilePaths;
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

                    foreach (string path in AllFilePaths)//遍历处理需要处理的路径
                    {
                        //强制终止
                        if (forcedStop)
                        {
                            MessageUtil.DoAppendTBDetail("强制终止了插入");
                            break;
                        }
                        currentFile = path.Substring(path.LastIndexOf('\\') + 1);
                        try
                        {
                            if (File.Exists(path))
                            {
                                ExportByPath(path, fileType, dataSourceEntites);
                            }
                            else
                            {
                                MessageBox.Show($"指定的文件不存在{path}");
                            }
                            MessageUtil.DoAppendTBDetail("正在写库，请稍候……");

                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("对象名:“Main”"))
                            {
                                continue;
                            }

                            var errorMsg = $"导出文件{currentFile}时发生错误{ex.ToString()}，{Environment.NewLine}错误消息:{ex.Message}详细信息{ex.StackTrace}" + $"{Environment.NewLine}当前文件:{path}";
                            MessageUtil.DoSetTBDetail($"发生异常:{errorMsg}");
                            LogHelper.WriteImportErrorLog(errorMsg);
                            errorMessageTopScope += errorMsg;
                            continue;
                        }
                    }
                }
                System.GC.Collect();
                GC.WaitForPendingFinalizers();
                #endregion


                //MessageUtil.DoAppendTBDetail($"当前批次运行完毕，处理了{bath.FILECOUNT}个文件，入库了{bath.HANDLED_ITEM_COUNT}条目，总耗时{bath.LAST_TIME}秒， 入库速度{bath.HANDLED_ITEM_COUNT / bath.LAST_TIME}件/秒");

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
                LogHelper.WriteImportErrorLog(errorMsg);
                errorMessageTopScope += errorMsg;
                MessageBox.Show(errorMessageTopScope);
            }
            return true;
        }


        public static bool ExportByPath(string filePath, string fileType, DataSourceEntities dataSourceEntites)
        {
            currentFile = filePath;
            MessageUtil.DoAppendTBDetail("您选择的资源类型为：" + fileType);
            MessageUtil.DoAppendTBDetail("当前文件：" + filePath);

            if (fileType == "中国商标")
            {
                export132(filePath, dataSourceEntites, "S_CHINA_BRAND");
            }
            return true;
        }

        private static void export132(string filePath, DataSourceEntities entiesContext, string tableName)
        {
            handledCount = 0;

            FileInfo numFileInfo = new FileInfo(filePath);

            if (!numFileInfo.Exists)
            {
                MessageBox.Show("指定的号单文件有误，号单文件不存在");
                return;
            }

            //解析号单

        }
    }
}