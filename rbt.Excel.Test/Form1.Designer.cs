namespace rbt.Excel.Test
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.process = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_exportPath = new System.Windows.Forms.TextBox();
            this.txt_configID = new System.Windows.Forms.TextBox();
            this.txt_configFile = new System.Windows.Forms.TextBox();
            this.console = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // process
            // 
            this.process.Location = new System.Drawing.Point(708, 30);
            this.process.Margin = new System.Windows.Forms.Padding(4);
            this.process.Name = "process";
            this.process.Size = new System.Drawing.Size(137, 186);
            this.process.TabIndex = 0;
            this.process.Text = "執行";
            this.process.UseVisualStyleBackColor = true;
            this.process.Click += new System.EventHandler(this.process_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(33, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "設定檔案路徑";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("華康粗圓體(P)", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(33, 77);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "設定資料 ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(32, 117);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "輸出路徑";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.panel1.Controls.Add(this.txt_exportPath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_configID);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txt_configFile);
            this.panel1.Controls.Add(this.label3);
            this.panel1.ForeColor = System.Drawing.SystemColors.Control;
            this.panel1.Location = new System.Drawing.Point(34, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(625, 186);
            this.panel1.TabIndex = 4;
            // 
            // txt_exportPath
            // 
            this.txt_exportPath.Location = new System.Drawing.Point(191, 110);
            this.txt_exportPath.Name = "txt_exportPath";
            this.txt_exportPath.Size = new System.Drawing.Size(349, 27);
            this.txt_exportPath.TabIndex = 7;
            // 
            // txt_configID
            // 
            this.txt_configID.Location = new System.Drawing.Point(191, 70);
            this.txt_configID.Name = "txt_configID";
            this.txt_configID.Size = new System.Drawing.Size(349, 27);
            this.txt_configID.TabIndex = 6;
            // 
            // txt_configFile
            // 
            this.txt_configFile.Location = new System.Drawing.Point(191, 33);
            this.txt_configFile.Name = "txt_configFile";
            this.txt_configFile.Size = new System.Drawing.Size(349, 27);
            this.txt_configFile.TabIndex = 5;
            // 
            // console
            // 
            this.console.Location = new System.Drawing.Point(34, 244);
            this.console.Multiline = true;
            this.console.Name = "console";
            this.console.Size = new System.Drawing.Size(811, 297);
            this.console.TabIndex = 5;
            // 
            // ExportFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 569);
            this.Controls.Add(this.console);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.process);
            this.Font = new System.Drawing.Font("華康粗圓體(P)", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ExportFile";
            this.Text = "Excel 產生測試程式";
            this.Load += new System.EventHandler(this.exportFile_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button process;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_configFile;
        private System.Windows.Forms.TextBox txt_configID;
        private System.Windows.Forms.TextBox txt_exportPath;
        private System.Windows.Forms.TextBox console;


    }
}

