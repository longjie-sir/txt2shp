namespace readData
{
    partial class Form1
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
            this.lab1 = new System.Windows.Forms.Label();
            this.lab2 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtOutPath = new System.Windows.Forms.TextBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnStart = new System.Windows.Forms.Button();
            this.ckFile = new System.Windows.Forms.CheckBox();
            this.ckDir = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.chkTxt = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.logTxt = new System.Windows.Forms.TextBox();
            this.btnLog = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lab1
            // 
            this.lab1.AutoSize = true;
            this.lab1.Location = new System.Drawing.Point(25, 84);
            this.lab1.Name = "lab1";
            this.lab1.Size = new System.Drawing.Size(67, 15);
            this.lab1.TabIndex = 0;
            this.lab1.Text = "文件地址";
            // 
            // lab2
            // 
            this.lab2.AutoSize = true;
            this.lab2.Location = new System.Drawing.Point(25, 145);
            this.lab2.Name = "lab2";
            this.lab2.Size = new System.Drawing.Size(67, 15);
            this.lab2.TabIndex = 1;
            this.lab2.Text = "存放地址";
            // 
            // txtPath
            // 
            this.txtPath.Enabled = false;
            this.txtPath.Location = new System.Drawing.Point(115, 81);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(537, 25);
            this.txtPath.TabIndex = 2;
            // 
            // txtOutPath
            // 
            this.txtOutPath.Location = new System.Drawing.Point(115, 142);
            this.txtOutPath.Name = "txtOutPath";
            this.txtOutPath.Size = new System.Drawing.Size(537, 25);
            this.txtOutPath.TabIndex = 3;
            // 
            // btnInput
            // 
            this.btnInput.Enabled = false;
            this.btnInput.Location = new System.Drawing.Point(683, 73);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(95, 37);
            this.btnInput.TabIndex = 4;
            this.btnInput.Text = "选择文件";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(683, 133);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(95, 39);
            this.btnOutput.TabIndex = 5;
            this.btnOutput.Text = "导出路径";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.SystemColors.Control;
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnStart.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStart.Location = new System.Drawing.Point(644, 245);
            this.btnStart.Margin = new System.Windows.Forms.Padding(0);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 32);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "运  行";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.Paint += new System.Windows.Forms.PaintEventHandler(this.btnStart_Paint);
            // 
            // ckFile
            // 
            this.ckFile.AutoSize = true;
            this.ckFile.Location = new System.Drawing.Point(28, 30);
            this.ckFile.Name = "ckFile";
            this.ckFile.Size = new System.Drawing.Size(197, 19);
            this.ckFile.TabIndex = 7;
            this.ckFile.Text = "txt转shp按带号聚合文件";
            this.ckFile.UseVisualStyleBackColor = true;
            this.ckFile.CheckedChanged += new System.EventHandler(this.ckFile_CheckedChanged);
            // 
            // ckDir
            // 
            this.ckDir.AutoSize = true;
            this.ckDir.Location = new System.Drawing.Point(309, 30);
            this.ckDir.Name = "ckDir";
            this.ckDir.Size = new System.Drawing.Size(182, 19);
            this.ckDir.TabIndex = 8;
            this.ckDir.Text = "txt转shp生成单个文件";
            this.ckDir.UseVisualStyleBackColor = true;
            this.ckDir.CheckedChanged += new System.EventHandler(this.ckDir_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Enabled = false;
            this.progressBar.Location = new System.Drawing.Point(6, 625);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(261, 23);
            this.progressBar.TabIndex = 9;
            // 
            // chkTxt
            // 
            this.chkTxt.AutoSize = true;
            this.chkTxt.Location = new System.Drawing.Point(620, 30);
            this.chkTxt.Name = "chkTxt";
            this.chkTxt.Size = new System.Drawing.Size(92, 19);
            this.chkTxt.TabIndex = 10;
            this.chkTxt.Text = "shp转txt";
            this.chkTxt.UseVisualStyleBackColor = true;
            this.chkTxt.CheckedChanged += new System.EventHandler(this.chkTxt_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLog);
            this.groupBox1.Location = new System.Drawing.Point(12, 289);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(817, 330);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "过程日志";
            // 
            // txtLog
            // 
            this.txtLog.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.Location = new System.Drawing.Point(16, 25);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(795, 299);
            this.txtLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "日志存放地址";
            // 
            // logTxt
            // 
            this.logTxt.Location = new System.Drawing.Point(115, 192);
            this.logTxt.Name = "logTxt";
            this.logTxt.Size = new System.Drawing.Size(537, 25);
            this.logTxt.TabIndex = 13;
            // 
            // btnLog
            // 
            this.btnLog.Location = new System.Drawing.Point(683, 183);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(95, 39);
            this.btnLog.TabIndex = 14;
            this.btnLog.Text = "日志路径";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbCount.Location = new System.Drawing.Point(750, 628);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(69, 19);
            this.lbCount.TabIndex = 16;
            this.lbCount.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 660);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.logTxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkTxt);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ckDir);
            this.Controls.Add(this.ckFile);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.txtOutPath);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lab2);
            this.Controls.Add(this.lab1);
            this.Name = "Form1";
            this.Text = "FormLJ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lab1;
        private System.Windows.Forms.Label lab2;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtOutPath;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnOutput;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox ckFile;
        private System.Windows.Forms.CheckBox ckDir;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox chkTxt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox logTxt;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Label lbCount;
    }
}

