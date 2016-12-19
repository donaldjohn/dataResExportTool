using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TheDataResourceExporter.Utils
{
    public class LogHelper
    {
        #region 导入日志输出
        /// <summary>
        /// 改成一批次一日志文件
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="txtName"></param>
        /// <param name="text"></param>
        public static void WriteImportLog(string dest, string txtName, string text)
        {

            string dateStr = ExportManger.bathStartTime.ToString("[yyyy年MM月dd日 HH：mm：ss FF]");

            if (string.IsNullOrEmpty(ExportManger.bathId))
            {
                ExportManger.bathId = "";
            }


            if (!Directory.Exists(dest))
            {
                try
                {
                    Directory.CreateDirectory(dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"创建日志文件失败，请检查指定的日志目录是否存在！！！错误详情：{ex.Message} \n {MiscUtil.jsonSerilizeObject(ex)}");
                    throw ex;
                }
            }
            try
            {
                dest = dest + Path.DirectorySeparatorChar + dateStr + "-[" + ExportManger.bathId + "]-" + txtName + ".log";
                using (StreamWriter sw = new StreamWriter(dest, true, Encoding.Default))
                {
                    DateTime current = System.DateTime.Now;
                    string timeStamp = "[" + current.ToLocalTime().ToString() + " " + current.Millisecond + "]";
                    //添加消息换行
                    text = Environment.NewLine + timeStamp + text;

                    sw.WriteLine(text);
                }
            }
            catch (Exception ec)
            {
                MessageBox.Show("写日志出错！" + ec.Message);
            }
        }


        public static void WriteImportLog(string msg)
        {
            string logDir = ConfigurationManager.AppSettings["logDir"];
            WriteImportLog(logDir, "Import_Info", msg);
        }


        public static void WriteImportErrorLog(string msg)
        {
            //添加时间标识
            //DateTime now = System.DateTime.Now;
            string logDir = ConfigurationManager.AppSettings["logDir"];
            var task = new Task(() =>
            {
                lock (typeof(LogHelper))
                {
                    WriteImportLog(logDir, "Import_Error", msg);
                }
            });
            task.Start();
        }
        #endregion
    }
}
