using System.Windows.Forms;

namespace TheDataResourceExporter
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.importProgressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.textBoxDetail = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.fileDialogLabel = new System.Windows.Forms.Label();
            this.fileTypeLabel = new System.Windows.Forms.Label();
            this.cbFileType = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btn_ChooseHD = new System.Windows.Forms.Button();
            this.tbHDFilePath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelcurrentArchive = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.labelelapsedTotalTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelAverageElapsedTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelImportCountPerSec = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuShowImportHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.BathHistoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SessionHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCheckHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxIsDir = new System.Windows.Forms.CheckBox();
            this.btnGetDataStoragePath = new System.Windows.Forms.Button();
            this.btnCSRetvedFileSavePath = new System.Windows.Forms.Button();
            this.tbRetrievedFileSavePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelProgressMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAddr1 = new System.Windows.Forms.TextBox();
            this.btnCSAddr1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbRecAddr = new System.Windows.Forms.TextBox();
            this.btnCSRecAddr = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.tbAddr2 = new System.Windows.Forms.TextBox();
            this.btnCSAddr2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAddr2 = new System.Windows.Forms.CheckBox();
            this.cbRecAddr = new System.Windows.Forms.CheckBox();
            this.cbAddr1 = new System.Windows.Forms.CheckBox();
            this.mainMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // importProgressBar
            // 
            this.importProgressBar.Location = new System.Drawing.Point(124, 610);
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(474, 19);
            this.importProgressBar.Step = 1;
            this.importProgressBar.TabIndex = 0;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(57, 617);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(41, 12);
            this.progressLabel.TabIndex = 1;
            this.progressLabel.Text = "进度：";
            // 
            // textBoxDetail
            // 
            this.textBoxDetail.Location = new System.Drawing.Point(113, 424);
            this.textBoxDetail.Multiline = true;
            this.textBoxDetail.Name = "textBoxDetail";
            this.textBoxDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDetail.Size = new System.Drawing.Size(474, 137);
            this.textBoxDetail.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(66, 424);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(41, 12);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "详细：";
            // 
            // fileDialogLabel
            // 
            this.fileDialogLabel.AutoSize = true;
            this.fileDialogLabel.Location = new System.Drawing.Point(23, 275);
            this.fileDialogLabel.Name = "fileDialogLabel";
            this.fileDialogLabel.Size = new System.Drawing.Size(53, 36);
            this.fileDialogLabel.TabIndex = 4;
            this.fileDialogLabel.Text = "号单文\r\n\r\n件路径：";
            // 
            // fileTypeLabel
            // 
            this.fileTypeLabel.AutoSize = true;
            this.fileTypeLabel.Location = new System.Drawing.Point(35, 39);
            this.fileTypeLabel.Name = "fileTypeLabel";
            this.fileTypeLabel.Size = new System.Drawing.Size(65, 12);
            this.fileTypeLabel.TabIndex = 5;
            this.fileTypeLabel.Text = "文档类型：";
            // 
            // cbFileType
            // 
            this.cbFileType.FormattingEnabled = true;
            this.cbFileType.Items.AddRange(new object[] {
            "中国专利全文代码化数据",
            "中国专利全文图像数据",
            "中国专利标准化全文文本数据",
            "中国专利标准化全文图像数据",
            "中国专利公报数据",
            "中国专利著录项目与文摘数据",
            "中国专利法律状态数据",
            "中国专利法律状态变更翻译数据",
            "中国标准化简单引文数据",
            "专利缴费数据",
            "公司代码库",
            "区域代码库",
            "美国专利全文文本数据（标准化）",
            "欧专局专利全文文本数据（标准化）",
            "韩国专利全文代码化数据（标准化）",
            "瑞士专利全文代码化数据（标准化）",
            "英国专利全文代码化数据（标准化）",
            "日本专利全文代码化数据（标准化）",
            "中国发明申请专利数据（DI）",
            "中国发明授权专利数据（DI）",
            "中国实用新型专利数据（DI）",
            "中国外观设计专利数据（DI）",
            "中国专利生物序列数据（DI）",
            "中国专利摘要英文翻译数据（DI）",
            "专利同族数据（DI）",
            "全球专利引文数据（DI）",
            "中国专利费用信息数据（DI）",
            "中国专利通知书数据（DI）",
            "中国法律状态标引库（DI）",
            "专利分类数据(分类号码)（DI）",
            "世界法律状态数据（DI）",
            "DOCDB数据（DI）",
            "美国专利著录项及全文数据（US）（DI）",
            "韩国专利著录项及全文数据（KR）（DI）",
            "欧洲专利局专利著录项及全文数据（EP）（DI）",
            "国际知识产权组织专利著录项及全文数据（WIPO)（DI）",
            "加拿大专利著录项及全文数据（CA）（DI）",
            "俄罗斯专利著录项及全文数据（RU）（DI）",
            "英国专利全文数据（GB）（DI）",
            "瑞士专利全文数据（CH）（DI）",
            "日本专利著录项及全文数据（JP）（DI）",
            "德国专利著录项及全文数据（DE）（DI）",
            "法国专利著录项及全文数据（FR）（DI）",
            "比利时专利全文数据（BE）（标准化）",
            "奥地利专利全文数据（AT）（标准化）",
            "西班牙专利全文数据（ES）（标准化）",
            "波兰专利著录项及全文数据（PL）（标准化）",
            "以色列专利著录项及全文数据（IL）（标准化）",
            "新加坡专利著录项及全文数据（SG）（标准化）",
            "台湾专利著录项及全文数据（TW）（DI）",
            "香港专利著录项数据（HK）（DI）",
            "澳门专利著录项数据（MO）（DI）",
            "欧亚组织专利著录项及全文数据（EA）（DI）",
            "美国外观设计专利数据（DI）",
            "日本外观设计专利数据（DI）",
            "韩国外观设计专利数据（DI）",
            "德国外观设计专利数据（DI）",
            "法国外观设计专利数据（DI）",
            "俄罗斯外观设计专利数据（DI）",
            "中国专利全文数据PDF（DI）",
            "国外专利全文数据PDF（DI）",
            "日本专利文摘英文翻译数据（PAJ)（DI）",
            "韩国专利文摘英文翻译数据(KPA)（DI）",
            "俄罗斯专利文摘英文翻译数据（DI）",
            "中国商标",
            "中国商标许可数据",
            "中国商标转让数据",
            "马德里商标进入中国",
            "中国驰名商标数据",
            "美国申请商标",
            "美国转让商标",
            "美国审判商标",
            "社内外知识产权图书题录数据",
            "民国书",
            "中外期刊的著录项目与文摘数据",
            "中国法院判例初加工数据",
            "中国商标分类数据",
            "美国商标图形分类数据",
            "美国商标美国分类数据",
            "马德里商标购买数据",
            "中国专利代理知识产权法律法规加工数据",
            "中国集成电路布图公告及事务数据",
            "中国知识产权海关备案数据",
            "国外专利生物序列加工成品数据",
            "中国专利复审数据",
            "中国专利无效数据",
            "中国专利的判决书数据",
            "中国生物序列深加工数据",
            "中国中药专利翻译数据",
            "中国化学药物专利深加工数据",
            "中国中药专利深加工数据"});
            this.cbFileType.Location = new System.Drawing.Point(124, 31);
            this.cbFileType.Name = "cbFileType";
            this.cbFileType.Size = new System.Drawing.Size(479, 20);
            this.cbFileType.TabIndex = 6;
            this.cbFileType.SelectedIndexChanged += new System.EventHandler(this.cbFileType_SelectedIndexChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(109, 377);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(219, 377);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 8;
            this.btnAbort.Text = "强制终止";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btn_ChooseHD
            // 
            this.btn_ChooseHD.Location = new System.Drawing.Point(516, 275);
            this.btn_ChooseHD.Name = "btn_ChooseHD";
            this.btn_ChooseHD.Size = new System.Drawing.Size(75, 23);
            this.btn_ChooseHD.TabIndex = 10;
            this.btn_ChooseHD.Text = "选择";
            this.btn_ChooseHD.UseVisualStyleBackColor = true;
            this.btn_ChooseHD.Click += new System.EventHandler(this.btn_Choose_Click);
            // 
            // tbHDFilePath
            // 
            this.tbHDFilePath.BackColor = System.Drawing.SystemColors.Window;
            this.tbHDFilePath.Location = new System.Drawing.Point(117, 276);
            this.tbHDFilePath.Name = "tbHDFilePath";
            this.tbHDFilePath.Size = new System.Drawing.Size(393, 21);
            this.tbHDFilePath.TabIndex = 9;
            this.tbHDFilePath.TextChanged += new System.EventHandler(this.tb_FilePath_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(411, 661);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 12);
            this.label8.TabIndex = 18;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(624, 617);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(23, 12);
            this.labelStatus.TabIndex = 22;
            this.labelStatus.Text = "0/0";
            // 
            // labelcurrentArchive
            // 
            this.labelcurrentArchive.AutoSize = true;
            this.labelcurrentArchive.Location = new System.Drawing.Point(124, 581);
            this.labelcurrentArchive.Name = "labelcurrentArchive";
            this.labelcurrentArchive.Size = new System.Drawing.Size(0, 12);
            this.labelcurrentArchive.TabIndex = 24;
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(342, 377);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 25;
            this.buttonReset.Text = "重置";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 649);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "耗时：";
            // 
            // labelelapsedTotalTime
            // 
            this.labelelapsedTotalTime.AutoSize = true;
            this.labelelapsedTotalTime.Location = new System.Drawing.Point(105, 649);
            this.labelelapsedTotalTime.Name = "labelelapsedTotalTime";
            this.labelelapsedTotalTime.Size = new System.Drawing.Size(0, 12);
            this.labelelapsedTotalTime.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(252, 648);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 28;
            this.label7.Text = "每件耗时：";
            // 
            // labelAverageElapsedTime
            // 
            this.labelAverageElapsedTime.AutoSize = true;
            this.labelAverageElapsedTime.Location = new System.Drawing.Point(327, 648);
            this.labelAverageElapsedTime.Name = "labelAverageElapsedTime";
            this.labelAverageElapsedTime.Size = new System.Drawing.Size(0, 12);
            this.labelAverageElapsedTime.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(392, 648);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "提取速度（件/秒）：";
            // 
            // labelImportCountPerSec
            // 
            this.labelImportCountPerSec.AutoSize = true;
            this.labelImportCountPerSec.Location = new System.Drawing.Point(517, 649);
            this.labelImportCountPerSec.Name = "labelImportCountPerSec";
            this.labelImportCountPerSec.Size = new System.Drawing.Size(0, 12);
            this.labelImportCountPerSec.TabIndex = 31;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShowImportHistory,
            this.menuHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(734, 25);
            this.mainMenu.TabIndex = 32;
            this.mainMenu.Text = "menuStrip1";
            // 
            // menuShowImportHistory
            // 
            this.menuShowImportHistory.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BathHistoryMenu,
            this.SessionHistoryMenuItem});
            this.menuShowImportHistory.Enabled = false;
            this.menuShowImportHistory.Name = "menuShowImportHistory";
            this.menuShowImportHistory.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuShowImportHistory.Size = new System.Drawing.Size(68, 21);
            this.menuShowImportHistory.Text = "导出历史";
            // 
            // BathHistoryMenu
            // 
            this.BathHistoryMenu.Name = "BathHistoryMenu";
            this.BathHistoryMenu.Size = new System.Drawing.Size(112, 22);
            this.BathHistoryMenu.Text = "按批次";
            this.BathHistoryMenu.Click += new System.EventHandler(this.BathHistoryMenu_Click);
            // 
            // SessionHistoryMenuItem
            // 
            this.SessionHistoryMenuItem.Name = "SessionHistoryMenuItem";
            this.SessionHistoryMenuItem.Size = new System.Drawing.Size(112, 22);
            this.SessionHistoryMenuItem.Text = "按文件";
            this.SessionHistoryMenuItem.Click += new System.EventHandler(this.menuShowImportHistory_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCheckHelp,
            this.menuAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 21);
            this.menuHelp.Text = "帮助";
            // 
            // menuCheckHelp
            // 
            this.menuCheckHelp.Enabled = false;
            this.menuCheckHelp.Name = "menuCheckHelp";
            this.menuCheckHelp.Size = new System.Drawing.Size(124, 22);
            this.menuCheckHelp.Text = "查看帮助";
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(124, 22);
            this.menuAbout.Text = "关于";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // checkBoxIsDir
            // 
            this.checkBoxIsDir.AutoSize = true;
            this.checkBoxIsDir.Location = new System.Drawing.Point(615, 279);
            this.checkBoxIsDir.Name = "checkBoxIsDir";
            this.checkBoxIsDir.Size = new System.Drawing.Size(84, 16);
            this.checkBoxIsDir.TabIndex = 34;
            this.checkBoxIsDir.Text = "文件夹模式";
            this.checkBoxIsDir.UseVisualStyleBackColor = true;
            this.checkBoxIsDir.CheckedChanged += new System.EventHandler(this.checkBoxIsDir_CheckedChanged);
            // 
            // btnGetDataStoragePath
            // 
            this.btnGetDataStoragePath.Location = new System.Drawing.Point(622, 89);
            this.btnGetDataStoragePath.Name = "btnGetDataStoragePath";
            this.btnGetDataStoragePath.Size = new System.Drawing.Size(75, 23);
            this.btnGetDataStoragePath.TabIndex = 37;
            this.btnGetDataStoragePath.Text = "从编目获取";
            this.btnGetDataStoragePath.UseVisualStyleBackColor = true;
            this.btnGetDataStoragePath.Click += new System.EventHandler(this.btnGetDataStoragePath_Click);
            // 
            // btnCSRetvedFileSavePath
            // 
            this.btnCSRetvedFileSavePath.Location = new System.Drawing.Point(516, 329);
            this.btnCSRetvedFileSavePath.Name = "btnCSRetvedFileSavePath";
            this.btnCSRetvedFileSavePath.Size = new System.Drawing.Size(75, 23);
            this.btnCSRetvedFileSavePath.TabIndex = 40;
            this.btnCSRetvedFileSavePath.Text = "选择";
            this.btnCSRetvedFileSavePath.UseVisualStyleBackColor = true;
            // 
            // tbRetrievedFileSavePath
            // 
            this.tbRetrievedFileSavePath.BackColor = System.Drawing.SystemColors.Window;
            this.tbRetrievedFileSavePath.Location = new System.Drawing.Point(117, 330);
            this.tbRetrievedFileSavePath.Name = "tbRetrievedFileSavePath";
            this.tbRetrievedFileSavePath.Size = new System.Drawing.Size(393, 21);
            this.tbRetrievedFileSavePath.TabIndex = 39;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 329);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 36);
            this.label3.TabIndex = 38;
            this.label3.Text = "提取文件\r\n\r\n保存位置：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 581);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "当前号单：";
            // 
            // labelProgressMsg
            // 
            this.labelProgressMsg.AutoSize = true;
            this.labelProgressMsg.Location = new System.Drawing.Point(57, 672);
            this.labelProgressMsg.Name = "labelProgressMsg";
            this.labelProgressMsg.Size = new System.Drawing.Size(89, 12);
            this.labelProgressMsg.TabIndex = 11;
            this.labelProgressMsg.Text = "              ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 36);
            this.label1.TabIndex = 4;
            this.label1.Text = "本 地\r\n\r\n备份1：";
            // 
            // tbAddr1
            // 
            this.tbAddr1.BackColor = System.Drawing.SystemColors.Window;
            this.tbAddr1.Location = new System.Drawing.Point(128, 95);
            this.tbAddr1.Name = "tbAddr1";
            this.tbAddr1.Size = new System.Drawing.Size(393, 21);
            this.tbAddr1.TabIndex = 9;
            this.tbAddr1.TextChanged += new System.EventHandler(this.tb_FilePath_TextChanged);
            // 
            // btnCSAddr1
            // 
            this.btnCSAddr1.Location = new System.Drawing.Point(527, 94);
            this.btnCSAddr1.Name = "btnCSAddr1";
            this.btnCSAddr1.Size = new System.Drawing.Size(75, 23);
            this.btnCSAddr1.TabIndex = 10;
            this.btnCSAddr1.Text = "选择";
            this.btnCSAddr1.UseVisualStyleBackColor = true;
            this.btnCSAddr1.Click += new System.EventHandler(this.btn_Choose_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 36);
            this.label6.TabIndex = 4;
            this.label6.Text = "灾备\r\n\r\n备份：";
            // 
            // tbRecAddr
            // 
            this.tbRecAddr.BackColor = System.Drawing.SystemColors.Window;
            this.tbRecAddr.Location = new System.Drawing.Point(129, 152);
            this.tbRecAddr.Name = "tbRecAddr";
            this.tbRecAddr.Size = new System.Drawing.Size(393, 21);
            this.tbRecAddr.TabIndex = 9;
            this.tbRecAddr.TextChanged += new System.EventHandler(this.tb_FilePath_TextChanged);
            // 
            // btnCSRecAddr
            // 
            this.btnCSRecAddr.Location = new System.Drawing.Point(528, 151);
            this.btnCSRecAddr.Name = "btnCSRecAddr";
            this.btnCSRecAddr.Size = new System.Drawing.Size(75, 23);
            this.btnCSRecAddr.TabIndex = 10;
            this.btnCSRecAddr.Text = "选择";
            this.btnCSRecAddr.UseVisualStyleBackColor = true;
            this.btnCSRecAddr.Click += new System.EventHandler(this.btn_Choose_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(41, 139);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 36);
            this.label9.TabIndex = 4;
            this.label9.Text = "本 地\r\n\r\n备份2：";
            // 
            // tbAddr2
            // 
            this.tbAddr2.BackColor = System.Drawing.SystemColors.Window;
            this.tbAddr2.Location = new System.Drawing.Point(129, 210);
            this.tbAddr2.Name = "tbAddr2";
            this.tbAddr2.Size = new System.Drawing.Size(393, 21);
            this.tbAddr2.TabIndex = 9;
            this.tbAddr2.TextChanged += new System.EventHandler(this.tb_FilePath_TextChanged);
            // 
            // btnCSAddr2
            // 
            this.btnCSAddr2.Location = new System.Drawing.Point(528, 208);
            this.btnCSAddr2.Name = "btnCSAddr2";
            this.btnCSAddr2.Size = new System.Drawing.Size(75, 23);
            this.btnCSAddr2.TabIndex = 10;
            this.btnCSAddr2.Text = "选择";
            this.btnCSAddr2.UseVisualStyleBackColor = true;
            this.btnCSAddr2.Click += new System.EventHandler(this.btn_Choose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAddr2);
            this.groupBox1.Controls.Add(this.cbRecAddr);
            this.groupBox1.Controls.Add(this.cbAddr1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(25, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(591, 188);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "提取数据位置";
            // 
            // cbAddr2
            // 
            this.cbAddr2.AutoSize = true;
            this.cbAddr2.Location = new System.Drawing.Point(6, 148);
            this.cbAddr2.Name = "cbAddr2";
            this.cbAddr2.Size = new System.Drawing.Size(15, 14);
            this.cbAddr2.TabIndex = 5;
            this.cbAddr2.UseVisualStyleBackColor = true;
            // 
            // cbRecAddr
            // 
            this.cbRecAddr.AutoSize = true;
            this.cbRecAddr.Location = new System.Drawing.Point(6, 90);
            this.cbRecAddr.Name = "cbRecAddr";
            this.cbRecAddr.Size = new System.Drawing.Size(15, 14);
            this.cbRecAddr.TabIndex = 5;
            this.cbRecAddr.UseVisualStyleBackColor = true;
            // 
            // cbAddr1
            // 
            this.cbAddr1.AutoSize = true;
            this.cbAddr1.Checked = true;
            this.cbAddr1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddr1.Location = new System.Drawing.Point(7, 31);
            this.cbAddr1.Name = "cbAddr1";
            this.cbAddr1.Size = new System.Drawing.Size(15, 14);
            this.cbAddr1.TabIndex = 5;
            this.cbAddr1.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 768);
            this.Controls.Add(this.btnCSRetvedFileSavePath);
            this.Controls.Add(this.tbRetrievedFileSavePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxIsDir);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.btn_ChooseHD);
            this.Controls.Add(this.tbHDFilePath);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.fileDialogLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.textBoxDetail);
            this.Controls.Add(this.btnGetDataStoragePath);
            this.Controls.Add(this.labelImportCountPerSec);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelAverageElapsedTime);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelelapsedTotalTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelcurrentArchive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelProgressMsg);
            this.Controls.Add(this.btnCSAddr2);
            this.Controls.Add(this.btnCSRecAddr);
            this.Controls.Add(this.btnCSAddr1);
            this.Controls.Add(this.tbAddr2);
            this.Controls.Add(this.tbRecAddr);
            this.Controls.Add(this.tbAddr1);
            this.Controls.Add(this.cbFileType);
            this.Controls.Add(this.fileTypeLabel);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.importProgressBar);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Main";
            this.Text = "数据资源导出工具 V0.０.１";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar importProgressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.TextBox textBoxDetail;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label fileTypeLabel;
        private System.Windows.Forms.Label fileDialogLabel;
        private System.Windows.Forms.ComboBox cbFileType;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btn_ChooseHD;
        private System.Windows.Forms.TextBox tbHDFilePath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelcurrentArchive;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelelapsedTotalTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelAverageElapsedTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelImportCountPerSec;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuCheckHelp;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private CheckBox checkBoxIsDir;
        private ToolStripMenuItem menuShowImportHistory;
        private ToolStripMenuItem BathHistoryMenu;
        private ToolStripMenuItem SessionHistoryMenuItem;
        private Button btnGetDataStoragePath;
        private Button btnCSRetvedFileSavePath;
        private TextBox tbRetrievedFileSavePath;
        private Label label3;
        private Label label2;
        private Label labelProgressMsg;
        private Label label1;
        private TextBox tbAddr1;
        private Button btnCSAddr1;
        private Label label6;
        private TextBox tbRecAddr;
        private Button btnCSRecAddr;
        private Label label9;
        private TextBox tbAddr2;
        private Button btnCSAddr2;
        private GroupBox groupBox1;
        private CheckBox cbAddr1;
        private CheckBox cbAddr2;
        private CheckBox cbRecAddr;
    }
}

