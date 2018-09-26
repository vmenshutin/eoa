namespace EntrostyleOperationsApplication
{
    partial class PickUserControl
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
            this.printOnlyBtn = new System.Windows.Forms.Button();
            this.continueBtn = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.printLabelBtn = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // printOnlyBtn
            // 
            this.printOnlyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printOnlyBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printOnlyBtn.ForeColor = System.Drawing.Color.SteelBlue;
            this.printOnlyBtn.Location = new System.Drawing.Point(12, 72);
            this.printOnlyBtn.Name = "printOnlyBtn";
            this.printOnlyBtn.Size = new System.Drawing.Size(101, 24);
            this.printOnlyBtn.TabIndex = 38;
            this.printOnlyBtn.Text = "Labels";
            this.printOnlyBtn.UseVisualStyleBackColor = true;
            this.printOnlyBtn.Click += new System.EventHandler(this.PrintOnlyBtn_Click);
            // 
            // continueBtn
            // 
            this.continueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.continueBtn.ForeColor = System.Drawing.Color.Green;
            this.continueBtn.Location = new System.Drawing.Point(12, 12);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(101, 24);
            this.continueBtn.TabIndex = 37;
            this.continueBtn.Text = "Picksheet";
            this.continueBtn.UseVisualStyleBackColor = true;
            this.continueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.OrangeRed;
            this.label22.Location = new System.Drawing.Point(121, 48);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(19, 13);
            this.label22.TabIndex = 36;
            this.label22.Text = "#:";
            // 
            // printLabelBtn
            // 
            this.printLabelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printLabelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printLabelBtn.ForeColor = System.Drawing.Color.OrangeRed;
            this.printLabelBtn.Location = new System.Drawing.Point(12, 42);
            this.printLabelBtn.Name = "printLabelBtn";
            this.printLabelBtn.Size = new System.Drawing.Size(101, 24);
            this.printLabelBtn.TabIndex = 35;
            this.printLabelBtn.Text = "Picksheet+Labels";
            this.printLabelBtn.UseVisualStyleBackColor = true;
            this.printLabelBtn.Click += new System.EventHandler(this.PrintLabelBtn_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.Color.OrangeRed;
            this.numericUpDown1.Location = new System.Drawing.Point(141, 45);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown1.TabIndex = 34;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Enter += new System.EventHandler(this.NumericUpDown1_Enter);
            // 
            // PickUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.printOnlyBtn);
            this.Controls.Add(this.continueBtn);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.printLabelBtn);
            this.Controls.Add(this.numericUpDown1);
            this.Name = "PickUserControl";
            this.Size = new System.Drawing.Size(200, 108);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button printOnlyBtn;
        private System.Windows.Forms.Button continueBtn;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button printLabelBtn;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
