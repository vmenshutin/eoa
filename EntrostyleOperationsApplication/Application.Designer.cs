namespace EntrostyleOperationsApplication
{
    partial class Application
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ScheduleTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SOMain = new System.Windows.Forms.DataGridView();
            this.clearSearchBtn = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.SOSecondary = new System.Windows.Forms.DataGridView();
            this.pickAllBtn = new System.Windows.Forms.Button();
            this.processPickBtn = new System.Windows.Forms.Button();
            this.printPickingBtn = new System.Windows.Forms.Button();
            this.refreshF10 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.SOItemDetails = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.refreshF5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.DifotTab = new System.Windows.Forms.TabPage();
            this.refreshDifot = new System.Windows.Forms.Button();
            this.difotPattern = new System.Windows.Forms.TextBox();
            this.difotTo = new System.Windows.Forms.DateTimePicker();
            this.difotFrom = new System.Windows.Forms.DateTimePicker();
            this.SODifot = new System.Windows.Forms.DataGridView();
            this.SettingsTab = new System.Windows.Forms.TabPage();
            this.label67 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.settings_Save = new System.Windows.Forms.Button();
            this.label105 = new System.Windows.Forms.Label();
            this.settings_printerName = new System.Windows.Forms.TextBox();
            this.label104 = new System.Windows.Forms.Label();
            this.settings_password = new System.Windows.Forms.TextBox();
            this.label103 = new System.Windows.Forms.Label();
            this.settings_login = new System.Windows.Forms.TextBox();
            this.label102 = new System.Windows.Forms.Label();
            this.settings_dbName = new System.Windows.Forms.TextBox();
            this.label101 = new System.Windows.Forms.Label();
            this.settings_installationAddress = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.settings_ClarityFileName = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.ScheduleTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SOMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SOSecondary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SOItemDetails)).BeginInit();
            this.DifotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SODifot)).BeginInit();
            this.SettingsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ScheduleTab);
            this.tabControl1.Controls.Add(this.DifotTab);
            this.tabControl1.Controls.Add(this.SettingsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1425, 734);
            this.tabControl1.TabIndex = 0;
            // 
            // ScheduleTab
            // 
            this.ScheduleTab.Controls.Add(this.splitContainer1);
            this.ScheduleTab.Location = new System.Drawing.Point(4, 22);
            this.ScheduleTab.Name = "ScheduleTab";
            this.ScheduleTab.Padding = new System.Windows.Forms.Padding(3);
            this.ScheduleTab.Size = new System.Drawing.Size(1417, 708);
            this.ScheduleTab.TabIndex = 0;
            this.ScheduleTab.Text = "Schedule";
            this.ScheduleTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pickAllBtn);
            this.splitContainer1.Panel2.Controls.Add(this.processPickBtn);
            this.splitContainer1.Panel2.Controls.Add(this.printPickingBtn);
            this.splitContainer1.Panel2.Controls.Add(this.refreshF10);
            this.splitContainer1.Panel2.Controls.Add(this.label12);
            this.splitContainer1.Panel2.Controls.Add(this.SOItemDetails);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.refreshF5);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label10);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Size = new System.Drawing.Size(1411, 702);
            this.splitContainer1.SplitterDistance = 781;
            this.splitContainer1.TabIndex = 14;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.SOMain);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.clearSearchBtn);
            this.splitContainer2.Panel2.Controls.Add(this.searchBox);
            this.splitContainer2.Panel2.Controls.Add(this.SOSecondary);
            this.splitContainer2.Size = new System.Drawing.Size(781, 702);
            this.splitContainer2.SplitterDistance = 386;
            this.splitContainer2.TabIndex = 0;
            // 
            // SOMain
            // 
            this.SOMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SOMain.BackgroundColor = System.Drawing.SystemColors.Control;
            this.SOMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SOMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SOMain.Location = new System.Drawing.Point(0, 0);
            this.SOMain.Name = "SOMain";
            this.SOMain.Size = new System.Drawing.Size(781, 386);
            this.SOMain.TabIndex = 0;
            // 
            // clearSearchBtn
            // 
            this.clearSearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearSearchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearSearchBtn.ForeColor = System.Drawing.Color.Firebrick;
            this.clearSearchBtn.Location = new System.Drawing.Point(690, 3);
            this.clearSearchBtn.Name = "clearSearchBtn";
            this.clearSearchBtn.Size = new System.Drawing.Size(91, 22);
            this.clearSearchBtn.TabIndex = 3;
            this.clearSearchBtn.Text = "Clear";
            this.clearSearchBtn.UseVisualStyleBackColor = true;
            this.clearSearchBtn.Click += new System.EventHandler(this.clearSearchBtn_Click);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(0, 4);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(686, 20);
            this.searchBox.TabIndex = 2;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // SOSecondary
            // 
            this.SOSecondary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SOSecondary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SOSecondary.BackgroundColor = System.Drawing.SystemColors.Control;
            this.SOSecondary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SOSecondary.Location = new System.Drawing.Point(0, 28);
            this.SOSecondary.Name = "SOSecondary";
            this.SOSecondary.Size = new System.Drawing.Size(781, 284);
            this.SOSecondary.TabIndex = 1;
            // 
            // pickAllBtn
            // 
            this.pickAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pickAllBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickAllBtn.ForeColor = System.Drawing.Color.MediumPurple;
            this.pickAllBtn.Location = new System.Drawing.Point(277, 183);
            this.pickAllBtn.Name = "pickAllBtn";
            this.pickAllBtn.Size = new System.Drawing.Size(134, 28);
            this.pickAllBtn.TabIndex = 19;
            this.pickAllBtn.Text = "Pick All";
            this.pickAllBtn.UseVisualStyleBackColor = true;
            this.pickAllBtn.Click += new System.EventHandler(this.pickAllBtn_Click);
            // 
            // processPickBtn
            // 
            this.processPickBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.processPickBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processPickBtn.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.processPickBtn.Location = new System.Drawing.Point(141, 183);
            this.processPickBtn.Name = "processPickBtn";
            this.processPickBtn.Size = new System.Drawing.Size(130, 28);
            this.processPickBtn.TabIndex = 18;
            this.processPickBtn.Text = "Process Pick";
            this.processPickBtn.UseVisualStyleBackColor = true;
            this.processPickBtn.Click += new System.EventHandler(this.processPickBtn_Click);
            // 
            // printPickingBtn
            // 
            this.printPickingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printPickingBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printPickingBtn.ForeColor = System.Drawing.Color.DarkOrange;
            this.printPickingBtn.Location = new System.Drawing.Point(3, 183);
            this.printPickingBtn.Name = "printPickingBtn";
            this.printPickingBtn.Size = new System.Drawing.Size(131, 28);
            this.printPickingBtn.TabIndex = 17;
            this.printPickingBtn.Text = "Print + Picking";
            this.printPickingBtn.UseVisualStyleBackColor = true;
            this.printPickingBtn.Click += new System.EventHandler(this.printPickingBtn_Click);
            // 
            // refreshF10
            // 
            this.refreshF10.BackColor = System.Drawing.Color.Gainsboro;
            this.refreshF10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshF10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshF10.ForeColor = System.Drawing.Color.OliveDrab;
            this.refreshF10.Location = new System.Drawing.Point(210, 215);
            this.refreshF10.Name = "refreshF10";
            this.refreshF10.Size = new System.Drawing.Size(201, 35);
            this.refreshF10.TabIndex = 16;
            this.refreshF10.Text = "Refresh + Keep Selected (F10)";
            this.refreshF10.UseVisualStyleBackColor = false;
            this.refreshF10.Click += new System.EventHandler(this.refreshF10_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(0, 80);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Address 2:";
            // 
            // SOItemDetails
            // 
            this.SOItemDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SOItemDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SOItemDetails.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SOItemDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.SOItemDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.SOItemDetails.DefaultCellStyle = dataGridViewCellStyle5;
            this.SOItemDetails.Location = new System.Drawing.Point(3, 254);
            this.SOItemDetails.Name = "SOItemDetails";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SOItemDetails.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.SOItemDetails.Size = new System.Drawing.Size(623, 448);
            this.SOItemDetails.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Account:";
            // 
            // refreshF5
            // 
            this.refreshF5.BackColor = System.Drawing.Color.Gainsboro;
            this.refreshF5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshF5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshF5.ForeColor = System.Drawing.Color.Green;
            this.refreshF5.Location = new System.Drawing.Point(3, 215);
            this.refreshF5.Name = "refreshF5";
            this.refreshF5.Size = new System.Drawing.Size(201, 35);
            this.refreshF5.TabIndex = 13;
            this.refreshF5.Text = "Refresh (F5)";
            this.refreshF5.UseVisualStyleBackColor = false;
            this.refreshF5.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "#:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label11.Location = new System.Drawing.Point(110, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "DIFOT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Address 1:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label10.Location = new System.Drawing.Point(110, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Last Scheduled";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Last Scheduled:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label9.Location = new System.Drawing.Point(110, 80);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Address2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(0, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "DIFOT:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label8.Location = new System.Drawing.Point(110, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Address1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label6.Location = new System.Drawing.Point(110, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "#";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label7.Location = new System.Drawing.Point(110, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Account";
            // 
            // DifotTab
            // 
            this.DifotTab.Controls.Add(this.refreshDifot);
            this.DifotTab.Controls.Add(this.difotPattern);
            this.DifotTab.Controls.Add(this.difotTo);
            this.DifotTab.Controls.Add(this.difotFrom);
            this.DifotTab.Controls.Add(this.SODifot);
            this.DifotTab.Location = new System.Drawing.Point(4, 22);
            this.DifotTab.Name = "DifotTab";
            this.DifotTab.Padding = new System.Windows.Forms.Padding(3);
            this.DifotTab.Size = new System.Drawing.Size(1417, 708);
            this.DifotTab.TabIndex = 1;
            this.DifotTab.Text = "DIFOT";
            this.DifotTab.UseVisualStyleBackColor = true;
            // 
            // refreshDifot
            // 
            this.refreshDifot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshDifot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshDifot.ForeColor = System.Drawing.Color.Green;
            this.refreshDifot.Location = new System.Drawing.Point(1264, 4);
            this.refreshDifot.Name = "refreshDifot";
            this.refreshDifot.Size = new System.Drawing.Size(150, 23);
            this.refreshDifot.TabIndex = 4;
            this.refreshDifot.Text = "Apply / Refresh";
            this.refreshDifot.UseVisualStyleBackColor = true;
            this.refreshDifot.Click += new System.EventHandler(this.refreshDifot_Click);
            // 
            // difotPattern
            // 
            this.difotPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.difotPattern.Location = new System.Drawing.Point(3, 6);
            this.difotPattern.Name = "difotPattern";
            this.difotPattern.Size = new System.Drawing.Size(979, 20);
            this.difotPattern.TabIndex = 3;
            // 
            // difotTo
            // 
            this.difotTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.difotTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.difotTo.Location = new System.Drawing.Point(1126, 6);
            this.difotTo.Name = "difotTo";
            this.difotTo.Size = new System.Drawing.Size(132, 20);
            this.difotTo.TabIndex = 2;
            // 
            // difotFrom
            // 
            this.difotFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.difotFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.difotFrom.Location = new System.Drawing.Point(988, 6);
            this.difotFrom.Name = "difotFrom";
            this.difotFrom.Size = new System.Drawing.Size(132, 20);
            this.difotFrom.TabIndex = 1;
            // 
            // SODifot
            // 
            this.SODifot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SODifot.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SODifot.BackgroundColor = System.Drawing.SystemColors.Control;
            this.SODifot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SODifot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SODifot.Location = new System.Drawing.Point(3, 31);
            this.SODifot.Name = "SODifot";
            this.SODifot.Size = new System.Drawing.Size(1411, 677);
            this.SODifot.TabIndex = 0;
            // 
            // SettingsTab
            // 
            this.SettingsTab.BackColor = System.Drawing.Color.Gainsboro;
            this.SettingsTab.Controls.Add(this.label67);
            this.SettingsTab.Controls.Add(this.label68);
            this.SettingsTab.Controls.Add(this.settings_Save);
            this.SettingsTab.Controls.Add(this.label105);
            this.SettingsTab.Controls.Add(this.settings_printerName);
            this.SettingsTab.Controls.Add(this.label104);
            this.SettingsTab.Controls.Add(this.settings_password);
            this.SettingsTab.Controls.Add(this.label103);
            this.SettingsTab.Controls.Add(this.settings_login);
            this.SettingsTab.Controls.Add(this.label102);
            this.SettingsTab.Controls.Add(this.settings_dbName);
            this.SettingsTab.Controls.Add(this.label101);
            this.SettingsTab.Controls.Add(this.settings_installationAddress);
            this.SettingsTab.Controls.Add(this.label88);
            this.SettingsTab.Controls.Add(this.settings_ClarityFileName);
            this.SettingsTab.ForeColor = System.Drawing.Color.RoyalBlue;
            this.SettingsTab.Location = new System.Drawing.Point(4, 22);
            this.SettingsTab.Name = "SettingsTab";
            this.SettingsTab.Size = new System.Drawing.Size(1417, 708);
            this.SettingsTab.TabIndex = 2;
            this.SettingsTab.Text = "Settings";
            // 
            // label67
            // 
            this.label67.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label67.AutoSize = true;
            this.label67.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label67.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label67.Location = new System.Drawing.Point(447, 178);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(89, 15);
            this.label67.TabIndex = 52;
            this.label67.Text = "Process Pick";
            // 
            // label68
            // 
            this.label68.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label68.AutoSize = true;
            this.label68.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label68.ForeColor = System.Drawing.Color.Black;
            this.label68.Location = new System.Drawing.Point(585, 178);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(54, 15);
            this.label68.TabIndex = 51;
            this.label68.Text = "Ctrl + P";
            // 
            // settings_Save
            // 
            this.settings_Save.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_Save.BackColor = System.Drawing.Color.Gainsboro;
            this.settings_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_Save.ForeColor = System.Drawing.Color.Green;
            this.settings_Save.Location = new System.Drawing.Point(450, 387);
            this.settings_Save.Name = "settings_Save";
            this.settings_Save.Size = new System.Drawing.Size(201, 35);
            this.settings_Save.TabIndex = 50;
            this.settings_Save.Text = "Save Settings";
            this.settings_Save.UseVisualStyleBackColor = false;
            this.settings_Save.Click += new System.EventHandler(this.settings_Save_Click);
            // 
            // label105
            // 
            this.label105.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label105.AutoSize = true;
            this.label105.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label105.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label105.Location = new System.Drawing.Point(447, 334);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(94, 15);
            this.label105.TabIndex = 49;
            this.label105.Text = "Printer name:";
            // 
            // settings_printerName
            // 
            this.settings_printerName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_printerName.BackColor = System.Drawing.Color.LightGray;
            this.settings_printerName.Location = new System.Drawing.Point(588, 332);
            this.settings_printerName.Name = "settings_printerName";
            this.settings_printerName.Size = new System.Drawing.Size(426, 20);
            this.settings_printerName.TabIndex = 48;
            this.settings_printerName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label104
            // 
            this.label104.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label104.AutoSize = true;
            this.label104.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label104.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label104.Location = new System.Drawing.Point(447, 308);
            this.label104.Name = "label104";
            this.label104.Size = new System.Drawing.Size(73, 15);
            this.label104.TabIndex = 47;
            this.label104.Text = "Password:";
            // 
            // settings_password
            // 
            this.settings_password.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_password.BackColor = System.Drawing.Color.LightGray;
            this.settings_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.settings_password.Location = new System.Drawing.Point(588, 306);
            this.settings_password.Name = "settings_password";
            this.settings_password.PasswordChar = '●';
            this.settings_password.Size = new System.Drawing.Size(148, 20);
            this.settings_password.TabIndex = 46;
            this.settings_password.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label103
            // 
            this.label103.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label103.AutoSize = true;
            this.label103.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label103.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label103.Location = new System.Drawing.Point(447, 282);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(47, 15);
            this.label103.TabIndex = 45;
            this.label103.Text = "Login:";
            // 
            // settings_login
            // 
            this.settings_login.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_login.BackColor = System.Drawing.Color.LightGray;
            this.settings_login.Location = new System.Drawing.Point(588, 280);
            this.settings_login.Name = "settings_login";
            this.settings_login.Size = new System.Drawing.Size(148, 20);
            this.settings_login.TabIndex = 44;
            this.settings_login.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label102
            // 
            this.label102.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label102.AutoSize = true;
            this.label102.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label102.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label102.Location = new System.Drawing.Point(447, 256);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(70, 15);
            this.label102.TabIndex = 43;
            this.label102.Text = "DB name:";
            // 
            // settings_dbName
            // 
            this.settings_dbName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_dbName.BackColor = System.Drawing.Color.LightGray;
            this.settings_dbName.Location = new System.Drawing.Point(588, 254);
            this.settings_dbName.Name = "settings_dbName";
            this.settings_dbName.Size = new System.Drawing.Size(148, 20);
            this.settings_dbName.TabIndex = 42;
            this.settings_dbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label101
            // 
            this.label101.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label101.AutoSize = true;
            this.label101.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label101.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label101.Location = new System.Drawing.Point(447, 230);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(137, 15);
            this.label101.TabIndex = 41;
            this.label101.Text = "Installation address:";
            // 
            // settings_installationAddress
            // 
            this.settings_installationAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_installationAddress.BackColor = System.Drawing.Color.LightGray;
            this.settings_installationAddress.Location = new System.Drawing.Point(588, 228);
            this.settings_installationAddress.Name = "settings_installationAddress";
            this.settings_installationAddress.Size = new System.Drawing.Size(426, 20);
            this.settings_installationAddress.TabIndex = 40;
            this.settings_installationAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label88
            // 
            this.label88.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label88.AutoSize = true;
            this.label88.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label88.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label88.Location = new System.Drawing.Point(447, 204);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(115, 15);
            this.label88.TabIndex = 39;
            this.label88.Text = "Clarity file name:";
            // 
            // settings_ClarityFileName
            // 
            this.settings_ClarityFileName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.settings_ClarityFileName.BackColor = System.Drawing.Color.LightGray;
            this.settings_ClarityFileName.Location = new System.Drawing.Point(588, 202);
            this.settings_ClarityFileName.Name = "settings_ClarityFileName";
            this.settings_ClarityFileName.Size = new System.Drawing.Size(148, 20);
            this.settings_ClarityFileName.TabIndex = 38;
            this.settings_ClarityFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1425, 734);
            this.Controls.Add(this.tabControl1);
            this.Name = "Application";
            this.Text = "EntroStyle Operations Application";
            this.tabControl1.ResumeLayout(false);
            this.ScheduleTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SOMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SOSecondary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SOItemDetails)).EndInit();
            this.DifotTab.ResumeLayout(false);
            this.DifotTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SODifot)).EndInit();
            this.SettingsTab.ResumeLayout(false);
            this.SettingsTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ScheduleTab;
        private System.Windows.Forms.DataGridView SOMain;
        private System.Windows.Forms.TabPage DifotTab;
        private System.Windows.Forms.TabPage SettingsTab;
        private System.Windows.Forms.DataGridView SOSecondary;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button refreshF5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button clearSearchBtn;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.DataGridView SOItemDetails;
        private System.Windows.Forms.DateTimePicker difotTo;
        private System.Windows.Forms.DateTimePicker difotFrom;
        private System.Windows.Forms.DataGridView SODifot;
        private System.Windows.Forms.TextBox difotPattern;
        private System.Windows.Forms.Button refreshDifot;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button refreshF10;
        private System.Windows.Forms.Button printPickingBtn;
        private System.Windows.Forms.Button settings_Save;
        private System.Windows.Forms.Label label105;
        private System.Windows.Forms.TextBox settings_printerName;
        private System.Windows.Forms.Label label104;
        private System.Windows.Forms.TextBox settings_password;
        private System.Windows.Forms.Label label103;
        private System.Windows.Forms.TextBox settings_login;
        private System.Windows.Forms.Label label102;
        private System.Windows.Forms.TextBox settings_dbName;
        private System.Windows.Forms.Label label101;
        private System.Windows.Forms.TextBox settings_installationAddress;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.TextBox settings_ClarityFileName;
        private System.Windows.Forms.Button processPickBtn;
        private System.Windows.Forms.Button pickAllBtn;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Label label68;
    }
}

