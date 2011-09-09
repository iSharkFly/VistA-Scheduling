namespace PrintPreviewDemo {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnClassic = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.chkPrinterSettingBeforePrint = new System.Windows.Forms.CheckBox();
            this.chkShowPrinterSettings = new System.Windows.Forms.CheckBox();
            this.chkShowPageSettings = new System.Windows.Forms.CheckBox();
            this.btnNewAdditionalText = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClassic
            // 
            this.btnClassic.Location = new System.Drawing.Point(12, 12);
            this.btnClassic.Name = "btnClassic";
            this.btnClassic.Size = new System.Drawing.Size(280, 23);
            this.btnClassic.TabIndex = 1;
            this.btnClassic.Text = "Classic Print Preview";
            this.btnClassic.UseVisualStyleBackColor = true;
            this.btnClassic.Click += new System.EventHandler(this.btnClassic_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(12, 41);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(280, 23);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "New Preview (default options)";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(211, 281);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(57, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Controls.Add(this.chkPrinterSettingBeforePrint);
            this.groupBox1.Controls.Add(this.chkShowPrinterSettings);
            this.groupBox1.Controls.Add(this.chkShowPageSettings);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Location = new System.Drawing.Point(12, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 313);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Options";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(6, 121);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(262, 154);
            this.propertyGrid1.TabIndex = 11;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // chkPrinterSettingBeforePrint
            // 
            this.chkPrinterSettingBeforePrint.AutoSize = true;
            this.chkPrinterSettingBeforePrint.Checked = true;
            this.chkPrinterSettingBeforePrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrinterSettingBeforePrint.Location = new System.Drawing.Point(6, 65);
            this.chkPrinterSettingBeforePrint.Name = "chkPrinterSettingBeforePrint";
            this.chkPrinterSettingBeforePrint.Size = new System.Drawing.Size(224, 17);
            this.chkPrinterSettingBeforePrint.TabIndex = 9;
            this.chkPrinterSettingBeforePrint.Text = "Show \"printer settings \" dialog before print";
            this.chkPrinterSettingBeforePrint.UseVisualStyleBackColor = true;
            // 
            // chkShowPrinterSettings
            // 
            this.chkShowPrinterSettings.AutoSize = true;
            this.chkShowPrinterSettings.Location = new System.Drawing.Point(6, 42);
            this.chkShowPrinterSettings.Name = "chkShowPrinterSettings";
            this.chkShowPrinterSettings.Size = new System.Drawing.Size(170, 17);
            this.chkShowPrinterSettings.TabIndex = 8;
            this.chkShowPrinterSettings.Text = "Show \"printer settings \" button";
            this.chkShowPrinterSettings.UseVisualStyleBackColor = true;
            // 
            // chkShowPageSettings
            // 
            this.chkShowPageSettings.AutoSize = true;
            this.chkShowPageSettings.Checked = true;
            this.chkShowPageSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowPageSettings.Location = new System.Drawing.Point(6, 19);
            this.chkShowPageSettings.Name = "chkShowPageSettings";
            this.chkShowPageSettings.Size = new System.Drawing.Size(165, 17);
            this.chkShowPageSettings.TabIndex = 7;
            this.chkShowPageSettings.Text = "Show \"page settings \" button";
            this.chkShowPageSettings.UseVisualStyleBackColor = true;
            // 
            // btnNewAdditionalText
            // 
            this.btnNewAdditionalText.Location = new System.Drawing.Point(12, 70);
            this.btnNewAdditionalText.Name = "btnNewAdditionalText";
            this.btnNewAdditionalText.Size = new System.Drawing.Size(280, 23);
            this.btnNewAdditionalText.TabIndex = 6;
            this.btnNewAdditionalText.Text = "New Preview (Additional text demo)";
            this.btnNewAdditionalText.UseVisualStyleBackColor = true;
            this.btnNewAdditionalText.Click += new System.EventHandler(this.btnNewAdditionalText_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Text Options";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 452);
            this.Controls.Add(this.btnNewAdditionalText);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClassic);
            this.Controls.Add(this.btnNew);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClassic;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkShowPageSettings;
        private System.Windows.Forms.CheckBox chkShowPrinterSettings;
        private System.Windows.Forms.CheckBox chkPrinterSettingBeforePrint;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btnNewAdditionalText;
        private System.Windows.Forms.Label label1;
    }
}

