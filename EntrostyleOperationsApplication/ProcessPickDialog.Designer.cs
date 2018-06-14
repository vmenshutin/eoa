﻿namespace EntrostyleOperationsApplication
{
    partial class ProcessPickDialog
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
            this.printLabelBtn = new System.Windows.Forms.Button();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.continueBtn = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.printOnlyBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.carrierTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TntRadioButton = new System.Windows.Forms.RadioButton();
            this.CustomRadioButton = new System.Windows.Forms.RadioButton();
            this.startTrackRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // printLabelBtn
            // 
            this.printLabelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printLabelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printLabelBtn.ForeColor = System.Drawing.Color.OrangeRed;
            this.printLabelBtn.Location = new System.Drawing.Point(483, 156);
            this.printLabelBtn.Name = "printLabelBtn";
            this.printLabelBtn.Size = new System.Drawing.Size(143, 28);
            this.printLabelBtn.TabIndex = 27;
            this.printLabelBtn.TabStop = false;
            this.printLabelBtn.Text = "Print # labels";
            this.printLabelBtn.UseVisualStyleBackColor = true;
            this.printLabelBtn.Click += new System.EventHandler(this.PrintLabelBtn_Click);
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.reportViewer1.DocumentMapWidth = 1;
            this.reportViewer1.LocalReport.ReportPath = "./carrier_label.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.ServerReport.ReportPath = "./carrier_label.rdlc";
            this.reportViewer1.ShowBackButton = false;
            this.reportViewer1.ShowContextMenu = false;
            this.reportViewer1.ShowCredentialPrompts = false;
            this.reportViewer1.ShowDocumentMapButton = false;
            this.reportViewer1.ShowExportButton = false;
            this.reportViewer1.ShowFindControls = false;
            this.reportViewer1.ShowPageNavigationControls = false;
            this.reportViewer1.ShowParameterPrompts = false;
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowProgress = false;
            this.reportViewer1.ShowPromptAreaButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowToolBar = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(463, 238);
            this.reportViewer1.TabIndex = 100;
            this.reportViewer1.TabStop = false;
            this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
            // 
            // continueBtn
            // 
            this.continueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.continueBtn.ForeColor = System.Drawing.Color.Green;
            this.continueBtn.Location = new System.Drawing.Point(483, 122);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(143, 28);
            this.continueBtn.TabIndex = 32;
            this.continueBtn.TabStop = false;
            this.continueBtn.Text = "Continue";
            this.continueBtn.UseVisualStyleBackColor = true;
            this.continueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.OrangeRed;
            this.label22.Location = new System.Drawing.Point(635, 165);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(19, 13);
            this.label22.TabIndex = 29;
            this.label22.Text = "#:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.Color.OrangeRed;
            this.numericUpDown1.Location = new System.Drawing.Point(655, 162);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Enter += new System.EventHandler(this.NumericUpDown1_Enter);
            // 
            // printOnlyBtn
            // 
            this.printOnlyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printOnlyBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printOnlyBtn.ForeColor = System.Drawing.Color.SteelBlue;
            this.printOnlyBtn.Location = new System.Drawing.Point(483, 190);
            this.printOnlyBtn.Name = "printOnlyBtn";
            this.printOnlyBtn.Size = new System.Drawing.Size(143, 28);
            this.printOnlyBtn.TabIndex = 33;
            this.printOnlyBtn.TabStop = false;
            this.printOnlyBtn.Text = "Print Only";
            this.printOnlyBtn.UseVisualStyleBackColor = true;
            this.printOnlyBtn.Click += new System.EventHandler(this.PrintOnlyBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DarkOrchid;
            this.label1.Location = new System.Drawing.Point(480, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Carrier#";
            // 
            // carrierTextBox
            // 
            this.carrierTextBox.ForeColor = System.Drawing.Color.DarkOrchid;
            this.carrierTextBox.Location = new System.Drawing.Point(483, 79);
            this.carrierTextBox.Name = "carrierTextBox";
            this.carrierTextBox.Size = new System.Drawing.Size(219, 20);
            this.carrierTextBox.TabIndex = 0;
            this.carrierTextBox.TextChanged += new System.EventHandler(this.CarrierTextBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.startTrackRadioButton);
            this.groupBox1.Controls.Add(this.TntRadioButton);
            this.groupBox1.Controls.Add(this.CustomRadioButton);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(483, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 39);
            this.groupBox1.TabIndex = 101;
            this.groupBox1.TabStop = false;
            // 
            // TntRadioButton
            // 
            this.TntRadioButton.AutoSize = true;
            this.TntRadioButton.Location = new System.Drawing.Point(74, 13);
            this.TntRadioButton.Name = "TntRadioButton";
            this.TntRadioButton.Size = new System.Drawing.Size(47, 17);
            this.TntRadioButton.TabIndex = 1;
            this.TntRadioButton.TabStop = true;
            this.TntRadioButton.Text = "TNT";
            this.TntRadioButton.UseVisualStyleBackColor = true;
            // 
            // CustomRadioButton
            // 
            this.CustomRadioButton.AutoSize = true;
            this.CustomRadioButton.Checked = true;
            this.CustomRadioButton.Location = new System.Drawing.Point(8, 13);
            this.CustomRadioButton.Name = "CustomRadioButton";
            this.CustomRadioButton.Size = new System.Drawing.Size(60, 17);
            this.CustomRadioButton.TabIndex = 0;
            this.CustomRadioButton.TabStop = true;
            this.CustomRadioButton.Text = "Custom";
            this.CustomRadioButton.UseVisualStyleBackColor = true;
            // 
            // startTrackRadioButton
            // 
            this.startTrackRadioButton.AutoSize = true;
            this.startTrackRadioButton.Location = new System.Drawing.Point(127, 13);
            this.startTrackRadioButton.Name = "startTrackRadioButton";
            this.startTrackRadioButton.Size = new System.Drawing.Size(72, 17);
            this.startTrackRadioButton.TabIndex = 2;
            this.startTrackRadioButton.TabStop = true;
            this.startTrackRadioButton.Text = "StarTrack";
            this.startTrackRadioButton.UseVisualStyleBackColor = true;
            // 
            // ProcessPickDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 238);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.carrierTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.printOnlyBtn);
            this.Controls.Add(this.continueBtn);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.printLabelBtn);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.reportViewer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ProcessPickDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pick";
            this.Load += new System.EventHandler(this.PrintDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button printLabelBtn;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Button continueBtn;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button printOnlyBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox carrierTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton TntRadioButton;
        private System.Windows.Forms.RadioButton CustomRadioButton;
        private System.Windows.Forms.RadioButton startTrackRadioButton;
    }
}