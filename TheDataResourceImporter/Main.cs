using System;
using System.Collections.Generic;
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

        private void getAndSetDataStoragePath(DataSourceEntities entitiesDataSource, string dataNum)
        {
            string addr1 = "", addr2 = "", recAddr = "";

            var queriedBianMuEnumerator = entitiesDataSource.W_SJZYZTSXXX.Where(rec => (!string.IsNullOrEmpty(dataNum) && dataNum.Equals(rec.F_DATANUM)));

            var targetBianMu = queriedBianMuEnumerator.FirstOrDefault();
            if (null == targetBianMu)
            {
                var message = "没有查询到指定类型的编目信息，请联系开发人员或手工指定提取数据位置";
                MessageUtil.DoSetTBDetail(message);
                MessageBox.Show(message);
            }
            else
            {
                addr1 = targetBianMu.F_ONEADDRESS;
                if (!string.IsNullOrEmpty(addr1))
                {
                    addr1 = addr1.Trim();
                }

                addr2 = targetBianMu.F_TWOADDRESS;
                if (!string.IsNullOrEmpty(addr2))
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
                }
            }
            else //文件夹模式
            {
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

        private void btnCSAddr1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx folderDialogEx = new FolderBrowserDialogEx();

            folderDialogEx.ShowNewFolderButton = false;
            folderDialogEx.Description = "请选择本地备份1路径";
            folderDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑

            if (folderDialogEx.ShowDialog() == DialogResult.OK)
            {
                string foldPath = folderDialogEx.SelectedPath;
                tbAddr1.Text = foldPath;
            }
        }

        private void btnCSRecAddr_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx folderDialogEx = new FolderBrowserDialogEx();

            folderDialogEx.ShowNewFolderButton = false;
            folderDialogEx.Description = "请选择灾备备份路径";
            folderDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑

            if (folderDialogEx.ShowDialog() == DialogResult.OK)
            {
                string foldPath = folderDialogEx.SelectedPath;
                tbRecAddr.Text = foldPath;
            }
        }

        private void btnCSAddr2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx folderDialogEx = new FolderBrowserDialogEx();

            folderDialogEx.ShowNewFolderButton = false;
            folderDialogEx.Description = "请选择本地备份2路径";
            folderDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑

            if (folderDialogEx.ShowDialog() == DialogResult.OK)
            {
                string foldPath = folderDialogEx.SelectedPath;

                tbAddr2.Text = foldPath;
            }
        }

        private void btnCSRetvedFileSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx folderDialogEx = new FolderBrowserDialogEx();
            folderDialogEx.ShowNewFolderButton = false;
            folderDialogEx.Description = "请选择文件路径";
            folderDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;//打开我的电脑

            if (folderDialogEx.ShowDialog() == DialogResult.OK)
            {
                string foldPath = folderDialogEx.SelectedPath;
                tbRetrievedFileSavePath.Text = foldPath;
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {

            //清空进度信息
            ExportManger.resetCounter();

            //清空强制终止标识
            ExportManger.forcedStop = false;

            //清空消息
            MessageUtil.setTbDetail("");

            var fileType = cbFileType.SelectedValue.ToString();

            //未选中文件类型
            if (String.IsNullOrEmpty(fileType))
            {
                MessageUtil.showMessageBoxWithErrorLog("请选择数据类型！");
                return;
            }

            List<String> storagePaths = new List<string>();

            var addrStr1 = tbAddr1.Text;
            if (cbAddr1.Checked && !string.IsNullOrEmpty(addrStr1))
            {
                storagePaths.AddRange(addrStr1.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            var addrStr2 = tbAddr2.Text;
            if (cbAddr2.Checked && !string.IsNullOrEmpty(addrStr2))
            {
                storagePaths.AddRange(addrStr2.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            var addrRecStr = tbRecAddr.Text;
            if (cbRecAddr.Checked && !string.IsNullOrEmpty(addrRecStr))
            {
                storagePaths.AddRange(addrRecStr.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            if (0 == storagePaths.Count())
            {
                var message = "请指定至少一个数据位置!";
                MessageUtil.showMessageBoxWithErrorLog(message);
                return;
            }

            var HDFilePath = tbHDFilePath.Text;

            if (string.IsNullOrEmpty(tbHDFilePath.Text) || null == filePaths || filePaths.Length == 0)
            {
                MessageUtil.showMessageBoxWithErrorLog("请选择至少选择一个号单文件！");
                return;
            }


            var retrivedFileSavePath = tbRetrievedFileSavePath.Text;

            if (String.IsNullOrEmpty(retrivedFileSavePath))
            {
                MessageUtil.showMessageBoxWithErrorLog("请指定提取文件保存位置!");
            }
            

            SetEnabled(btn_ChooseHD, false);

            SetEnabled(btnStart, false);

            Func<string[], string, string[], string, bool> func = TheDataResourceExporter.ExportManger.BeginExport;

            ExportManger.exportStartTime = System.DateTime.Now;

            func.BeginInvoke(filePaths, fileType.Trim(), storagePaths.ToArray(), retrivedFileSavePath,
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
                    double totalSecCount = System.DateTime.Now.Subtract(ExportManger.exportStartTime).TotalSeconds;

                    double averageTime = totalSecCount / handledCount;

                    double importCountPerSec = handledCount / totalSecCount;

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
                {
                    textBoxDetail.SelectionStart = textBoxDetail.Text.Length;
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


            var fileType = cbFileType.SelectedValue.ToString();
            if (string.IsNullOrEmpty(fileType))
            {
                return;
            }

            var fDataNum = "";

            switch (fileType)
            {
                case "中国生物序列深加工数据-中文": fDataNum = "045"; break;
                case "中国生物序列深加工数据-翻译": fDataNum = "045"; break;
                case "中国商标": fDataNum = "600"; break;
                case "马德里商标进入中国": fDataNum = "603"; break;
                case "美国申请商标": fDataNum = "605"; break;
                case "美国转让商标": fDataNum = "606"; break;
                case "美国审判商标": fDataNum = "607"; break;
                case "社内外知识产权图书题录数据": fDataNum = "608"; break;
                case "中国法院判例初加工数据": fDataNum = "610"; break;
                case "马德里商标购买数据": fDataNum = "613"; break;
                case "中国专利代理知识产权法律法规加工数据": fDataNum = "614"; break;
                case "中国集成电路布图公告及事务数据": fDataNum = "615"; break;
                case "中国知识产权海关备案数据": fDataNum = "616"; break;
                case "中国专利复审（无效）数据": fDataNum = "046"; break;
                case "中国专利的判决书数据": fDataNum = "506"; break;
                case "中国中药专利翻译数据": fDataNum = "052"; break;
            }

            using (DataSourceEntities dataSourceEntities = new DataSourceEntities())
            {
                getAndSetDataStoragePath(dataSourceEntities, fDataNum.ToString());
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

        private void tbHDFilePath_TextChanged(object sender, EventArgs e)
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
