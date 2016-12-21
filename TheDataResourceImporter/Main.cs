﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheDataResourceExporter.Utils;
using TheDataResourceImporter;

namespace TheDataResourceExporter
{
    public partial class Main : Form
    {

        public static bool showFileDialog = true;

        public Main()
        {
            try
            {
                InitializeComponent();

                DataSourceEntities entitiesDataSource = new DataSourceEntities();
                //绑定数据类型 下拉列表(查询条件：支持导出工具且已实现入库)
                var availableDataTypes = from dataType in entitiesDataSource.S_DATA_RESOURCE_TYPES_DETAIL.Where(
                    dataType => "Y".Equals(dataType.HASEXPORTER)
                    &&
                    "Y".Equals(dataType.IMPLEMENTED_IMPORT_LOGIC)
                    ).ToList()
                                         orderby dataType.ID ascending
                                         select
                                         new
                                         {
                                             diplayName = dataType.ID + "-" + dataType.CHINESE_NAME,
                                             selectedValue = dataType.CHINESE_NAME
                                         };

                cbFileType.DisplayMember = "diplayName";
                cbFileType.ValueMember = "selectedValue";
                cbFileType.DataSource = availableDataTypes.ToList();

                MessageUtil.setTbDetail = SetTextBoxDetail;
                MessageUtil.appendTbDetail = appendTextBoxDetail;

                //添加进度输出
                MessageUtil.updateProgressIndicator = updateProgressIndicator;
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.  
                SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动失败，请确保安装了必备包内软件！！！\n错误详情：“{ex.Message}”");
                throw;
            }
        }

        private void getAndSetDataStoragePath(DataSourceEntities entitiesDataSource, string dataResName)
        {
            string addr1 = "", addr2 = "", recAddr = "";

            var queriedBianMuEnumerator = entitiesDataSource.W_SJZYZTSXXX.Where(rec => (!string.IsNullOrEmpty(dataResName) && dataResName.Equals(rec.F_DATANAME)));

            var targetBianMu = queriedBianMuEnumerator.FirstOrDefault();
            if (null == targetBianMu)
            {
                MessageBox.Show("没有查询到指定类型的编目信息，请联系开发人员或手工指定提取数据位置");
            }
            else
            {
                addr1 = targetBianMu.F_ONEADDRESS;
                if (!string.IsNullOrEmpty(addr1))
                {
                    addr1 = addr1.Trim();
                }

                addr2 = targetBianMu.F_TWOADDRESS;
                if (!string.IsNullOrEmpty(addr1))
                {
                    addr2 = addr2.Trim();
                }

                recAddr = targetBianMu.F_RECOVERYADDRESS;
                if (!string.IsNullOrEmpty(recAddr))
                {
                    recAddr = recAddr.Trim();
                }
            }

            tbAddr1.Text = addr1;
            tbAddr2.Text = addr2;
            tbRecAddr.Text = recAddr;
        }


        string[] filePaths = null;
        private void btn_Choose_Click(object sender, EventArgs e)
        {
            if (showFileDialog) //展示文件选择器
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "任意文件(*.*)|*.txt";
                dialog.Multiselect = false;



                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    filePaths = null;
                    tbHDFilePath.Text = string.Empty;


                    filePaths = new string[] { dialog.FileName };

                    tbHDFilePath.Text = dialog.FileName;
                    //foreach (string filePath in filePaths)
                    //{
                    //    tb_FilePath.Text += (filePath + ";");
                    //}
                }
            }
            else //文件夹模式
            {
                //FolderBrowserDialog dialog = new FolderBrowserDialog();
                //dialog.ShowNewFolderButton = false;
                //dialog.Description = "请选择文件路径";
                //dialog.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑
                //var dataRootDirStr = System.Configuration.ConfigurationManager.AppSettings["dataRootDir"];

                ////获取数据资源默认路径
                //if (Directory.Exists(dataRootDirStr))
                //{
                //    dialog.SelectedPath = dataRootDirStr;
                //}

                //if (dialog.ShowDialog() == DialogResult.OK)
                //{
                //    string foldPath = dialog.SelectedPath;

                //    tb_FilePath.Text = foldPath;

                //    filePaths = new string[] { foldPath };
                //}

                FolderBrowserDialogEx folderDialogEx = new FolderBrowserDialogEx();

                folderDialogEx.ShowNewFolderButton = false;
                folderDialogEx.Description = "请选择文件路径";
                folderDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑

                if (folderDialogEx.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = folderDialogEx.SelectedPath;

                    tbHDFilePath.Text = foldPath;

                    filePaths = new string[] { foldPath };
                }



            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            //清空进度信息
            ExportManger.resetCounter();

            //清空强制终止标识
            ExportManger.forcedStop = false;

            MessageUtil.setTbDetail("");

            if (string.IsNullOrEmpty(tbHDFilePath.Text) || null == filePaths || filePaths.Length == 0)
            {
                MessageBox.Show("请选择至少选择一个文件！");
                return;
            }

            var fileType = cbFileType.SelectedValue.ToString();

            //未选中文件类型
            if (String.IsNullOrEmpty(fileType))
            {
                MessageBox.Show("请选择数据类型！");
                return;
            }


            SetEnabled(btn_ChooseHD, false);
            SetEnabled(btnStart, false);

            Func<string[], string, bool> func = TheDataResourceExporter.ExportManger.BeginImport;

            ExportManger.importStartTime = System.DateTime.Now;

            func.BeginInvoke(filePaths, fileType.Trim(),
                delegate (IAsyncResult ia)
                {
                    try
                    {
                        bool result = func.EndInvoke(ia);
                        if (result)
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    SetEnabled(btn_ChooseHD, true);
                    SetEnabled(btnStart, true);
                }, null);
        }



        delegate void SetTextBoxDetailHander(string msg);
        public void SetTextBoxDetail(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetTextBoxDetailHander(SetTextBoxDetail), msg);
            }
            else
            {
                //lock (textBoxDetail)
                {
                    textBoxDetail.Text = msg;
                }
            }
        }



        delegate void updateProgressIndicatorHander(int totalCount, int handledCount, int handledXMLCount, int handledDirCount, string achievePath);
        public void updateProgressIndicator(int totalCount, int handledCount, int handledXMLCount, int handledDirCount, string achievePath)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new updateProgressIndicatorHander(updateProgressIndicator), totalCount, handledCount, handledXMLCount, handledDirCount, achievePath);
            }
            else
            {
                {
                    labelcurrentArchive.Text = achievePath;
                    string status = handledCount + "/" + totalCount;

                    labelStatus.Text = status;

                    string progressMsg = $"发现需导出{totalCount}条记录，已导出{handledCount}条，剩余{totalCount - handledCount}天";

                    labelProgressMsg.Text = progressMsg;

                    int currentPercentage = 0;
                    if (totalCount > 0)
                    {
                        //计算百分比
                        currentPercentage = (Int32)Math.Floor((double)handledCount * 100 / totalCount);
                    }

                    //当前进度百分比
                    importProgressBar.Value = currentPercentage;

                    //更新耗时信息
                    double totalSecCount = System.DateTime.Now.Subtract(ExportManger.importStartTime).TotalSeconds;

                    double averageTime = totalSecCount / ExportManger.handledCount;

                    double importCountPerSec = ExportManger.handledCount / totalSecCount;

                    labelelapsedTotalTime.Text = totalSecCount.ToString("0.####") + "S";

                    labelAverageElapsedTime.Text = averageTime.ToString("0.####") + "S";

                    labelImportCountPerSec.Text = importCountPerSec.ToString("0.####" + "件/S");
                }
            }
        }



        delegate void appendTextBoxDetailHander(string msg);
        public void appendTextBoxDetail(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new appendTextBoxDetailHander(appendTextBoxDetail), msg);
            }
            else
            {
                //textBoxDetail.Text = textBoxDetail.Text + msg;
                //lock (textBoxDetail)
                {
                    textBoxDetail.SelectionStart = textBoxDetail.Text.Length;
                    //textBoxDetail.AppendText(msg);
                    textBoxDetail.Text = msg;
                    textBoxDetail.ScrollToCaret();
                }
            }
        }



        delegate void SetEnabledHander(Control control, bool flag);

        public void SetEnabled(Control control, bool flag)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new SetEnabledHander(SetEnabled), control, flag);
            }
            else
            {
                control.Enabled = flag;
            }
        }


        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确定退出？", "退出确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }


        private void btnAbort_Click(object sender, EventArgs e)
        {
            //强制终止
            ExportManger.forcedStop = true;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            //重置
            ExportManger.resetCounter();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            new AboutBoxUS().ShowDialog();
        }

        private void menuShowImportHistory_Click(object sender, EventArgs e)
        {
            var sessionHistoryForm = new ImportHistoryForm();
            sessionHistoryForm.WindowState = FormWindowState.Maximized;
            sessionHistoryForm.Show();
        }

        private void cbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 == cbFileType.SelectedIndex)
            {
                return;
            }


            var fileType = cbFileType.SelectedValue;

            var fileTypeInBianMu = fileType;//如果和编目的数据资源类型不同,需要转换




            using (DataSourceEntities dataSourceEntities = new DataSourceEntities())
            {
                getAndSetDataStoragePath(dataSourceEntities, fileTypeInBianMu.ToString());
            }

            //清空文件路径
            filePaths = null;
            tbHDFilePath.Text = "";
        }

        private void checkBoxIsDir_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIsDir.Checked)
            {
                showFileDialog = false;//文件夹模式
            }
            else
            {
                showFileDialog = true;//文件模式
            }
        }

        private void BathHistoryMenu_Click(object sender, EventArgs e)
        {
            var bathHistoryForm = new ImportBatchHistoryForm();
            bathHistoryForm.WindowState = FormWindowState.Maximized;
            bathHistoryForm.Show();
        }

        private void tb_FilePath_TextChanged(object sender, EventArgs e)
        {
            var inputedpath = tbHDFilePath.Text;
            filePaths = new string[] { inputedpath };
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void btnGetDataStoragePath_Click(object sender, EventArgs e)
        {
            cbFileType_SelectedIndexChanged(sender, e);
        }
    }
}
