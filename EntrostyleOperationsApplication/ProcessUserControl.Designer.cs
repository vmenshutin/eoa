namespace EntrostyleOperationsApplication
{
    partial class ProcessUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.startTrackRadioButton = new System.Windows.Forms.RadioButton();
            this.TntRadioButton = new System.Windows.Forms.RadioButton();
            this.CustomRadioButton = new System.Windows.Forms.RadioButton();
            this.carrierTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.printOnlyBtn = new System.Windows.Forms.Button();
            this.continueBtn = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.printLabelBtn = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.startTrackRadioButton);
            this.groupBox1.Controls.Add(this.TntRadioButton);
            this.groupBox1.Controls.Add(this.CustomRadioButton);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 39);
            this.groupBox1.TabIndex = 109;
            this.groupBox1.TabStop = false;
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
            // carrierTextBox
            // 
            this.carrierTextBox.ForeColor = System.Drawing.Color.DarkOrchid;
            this.carrierTextBox.Location = new System.Drawing.Point(12, 76);
            this.carrierTextBox.Name = "carrierTextBox";
            this.carrierTextBox.Size = new System.Drawing.Size(219, 20);
            this.carrierTextBox.TabIndex = 102;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DarkOrchid;
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 108;
            this.label1.Text = "Carrier#";
            // 
            // printOnlyBtn
            // 
            this.printOnlyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printOnlyBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printOnlyBtn.ForeColor = System.Drawing.Color.SteelBlue;
            this.printOnlyBtn.Location = new System.Drawing.Point(247, 72);
            this.printOnlyBtn.Name = "printOnlyBtn";
            this.printOnlyBtn.Size = new System.Drawing.Size(146, 24);
            this.printOnlyBtn.TabIndex = 107;
            this.printOnlyBtn.TabStop = false;
            this.printOnlyBtn.Text = "Labels";
            this.printOnlyBtn.UseVisualStyleBackColor = true;
            this.printOnlyBtn.Click += new System.EventHandler(this.PrintOnlyBtn_Click);
            // 
            // continueBtn
            // 
            this.continueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.continueBtn.ForeColor = System.Drawing.Color.Green;
            this.continueBtn.Location = new System.Drawing.Point(247, 12);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(146, 24);
            this.continueBtn.TabIndex = 106;
            this.continueBtn.TabStop = false;
            this.continueBtn.Text = "Process";
            this.continueBtn.UseVisualStyleBackColor = true;
            this.continueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.OrangeRed;
            this.label22.Location = new System.Drawing.Point(399, 48);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(19, 13);
            this.label22.TabIndex = 105;
            this.label22.Text = "#:";
            // 
            // printLabelBtn
            // 
            this.printLabelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printLabelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printLabelBtn.ForeColor = System.Drawing.Color.OrangeRed;
            this.printLabelBtn.Location = new System.Drawing.Point(247, 42);
            this.printLabelBtn.Name = "printLabelBtn";
            this.printLabelBtn.Size = new System.Drawing.Size(146, 24);
            this.printLabelBtn.TabIndex = 104;
            this.printLabelBtn.TabStop = false;
            this.printLabelBtn.Text = "Process + Labels";
            this.printLabelBtn.UseVisualStyleBackColor = true;
            this.printLabelBtn.Click += new System.EventHandler(this.PrintLabelBtn_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.Color.OrangeRed;
            this.numericUpDown1.Location = new System.Drawing.Point(419, 45);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown1.TabIndex = 103;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Enter += new System.EventHandler(this.NumericUpDown1_Enter);
            // 
            // ProcessUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.carrierTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.printOnlyBtn);
            this.Controls.Add(this.continueBtn);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.printLabelBtn);
            this.Controls.Add(this.numericUpDown1);
            this.Name = "ProcessUserControl";
            this.Size = new System.Drawing.Size(479, 108);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton startTrackRadioButton;
        private System.Windows.Forms.RadioButton TntRadioButton;
        private System.Windows.Forms.RadioButton CustomRadioButton;
        private System.Windows.Forms.TextBox carrierTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button printOnlyBtn;
        private System.Windows.Forms.Button continueBtn;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button printLabelBtn;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
