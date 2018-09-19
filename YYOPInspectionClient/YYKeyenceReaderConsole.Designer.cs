namespace YYOPInspectionClient
{
    partial class YYKeyenceReaderConsole
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Reset = new System.Windows.Forms.Button();
            this.button_Tune = new System.Windows.Forms.Button();
            this.listBox_Reader = new System.Windows.Forms.ListBox();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_FTune = new System.Windows.Forms.Button();
            this.textbox_DataConsole = new System.Windows.Forms.TextBox();
            this.textBox_LogConsole = new System.Windows.Forms.TextBox();
            this.receive = new System.Windows.Forms.Button();
            this.loff = new System.Windows.Forms.Button();
            this.lon = new System.Windows.Forms.Button();
            this.disconnect = new System.Windows.Forms.Button();
            this.connect = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.DataPortInput = new System.Windows.Forms.TextBox();
            this.CommandPortInput = new System.Windows.Forms.TextBox();
            this.btnHide = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 294);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 61;
            this.label5.Text = "数据日志";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 60;
            this.label4.Text = "命令日志";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 59;
            this.label3.Text = "在线Ip";
            // 
            // button_Reset
            // 
            this.button_Reset.Location = new System.Drawing.Point(525, 151);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(138, 61);
            this.button_Reset.TabIndex = 58;
            this.button_Reset.Text = "重置读码器";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            // 
            // button_Tune
            // 
            this.button_Tune.Location = new System.Drawing.Point(827, 151);
            this.button_Tune.Name = "button_Tune";
            this.button_Tune.Size = new System.Drawing.Size(71, 61);
            this.button_Tune.TabIndex = 57;
            this.button_Tune.Text = "学习";
            this.button_Tune.UseVisualStyleBackColor = true;
            this.button_Tune.Click += new System.EventHandler(this.button_Tune_Click);
            // 
            // listBox_Reader
            // 
            this.listBox_Reader.FormattingEnabled = true;
            this.listBox_Reader.ItemHeight = 12;
            this.listBox_Reader.Location = new System.Drawing.Point(253, 40);
            this.listBox_Reader.Name = "listBox_Reader";
            this.listBox_Reader.Size = new System.Drawing.Size(263, 256);
            this.listBox_Reader.TabIndex = 56;
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(525, 233);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(373, 59);
            this.button_Clear.TabIndex = 55;
            this.button_Clear.Text = "清除日志";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_FTune
            // 
            this.button_FTune.Location = new System.Drawing.Point(742, 151);
            this.button_FTune.Name = "button_FTune";
            this.button_FTune.Size = new System.Drawing.Size(71, 61);
            this.button_FTune.TabIndex = 54;
            this.button_FTune.Text = "自动对焦";
            this.button_FTune.UseVisualStyleBackColor = true;
            this.button_FTune.Click += new System.EventHandler(this.button_FTune_Click);
            // 
            // textbox_DataConsole
            // 
            this.textbox_DataConsole.Location = new System.Drawing.Point(22, 316);
            this.textbox_DataConsole.Multiline = true;
            this.textbox_DataConsole.Name = "textbox_DataConsole";
            this.textbox_DataConsole.ReadOnly = true;
            this.textbox_DataConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_DataConsole.Size = new System.Drawing.Size(494, 313);
            this.textbox_DataConsole.TabIndex = 53;
            // 
            // textBox_LogConsole
            // 
            this.textBox_LogConsole.Location = new System.Drawing.Point(22, 40);
            this.textBox_LogConsole.Multiline = true;
            this.textBox_LogConsole.Name = "textBox_LogConsole";
            this.textBox_LogConsole.ReadOnly = true;
            this.textBox_LogConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_LogConsole.Size = new System.Drawing.Size(211, 251);
            this.textBox_LogConsole.TabIndex = 52;
            // 
            // receive
            // 
            this.receive.Location = new System.Drawing.Point(525, 566);
            this.receive.Name = "receive";
            this.receive.Size = new System.Drawing.Size(178, 52);
            this.receive.TabIndex = 51;
            this.receive.Text = "接收数据";
            this.receive.UseVisualStyleBackColor = true;
            this.receive.UseWaitCursor = true;
            this.receive.Visible = false;
            this.receive.Click += new System.EventHandler(this.receive_Click);
            // 
            // loff
            // 
            this.loff.Location = new System.Drawing.Point(720, 316);
            this.loff.Name = "loff";
            this.loff.Size = new System.Drawing.Size(178, 91);
            this.loff.TabIndex = 50;
            this.loff.Text = "结束读码";
            this.loff.UseVisualStyleBackColor = true;
            this.loff.Click += new System.EventHandler(this.loff_Click);
            // 
            // lon
            // 
            this.lon.Location = new System.Drawing.Point(525, 316);
            this.lon.Name = "lon";
            this.lon.Size = new System.Drawing.Size(189, 91);
            this.lon.TabIndex = 49;
            this.lon.Text = "开始读码";
            this.lon.UseVisualStyleBackColor = true;
            this.lon.Click += new System.EventHandler(this.lon_Click);
            // 
            // disconnect
            // 
            this.disconnect.Location = new System.Drawing.Point(742, 83);
            this.disconnect.Name = "disconnect";
            this.disconnect.Size = new System.Drawing.Size(156, 62);
            this.disconnect.TabIndex = 48;
            this.disconnect.Text = "断开读码器";
            this.disconnect.UseVisualStyleBackColor = true;
            this.disconnect.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(525, 83);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(138, 62);
            this.connect.TabIndex = 47;
            this.connect.Text = "连接读码器";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(754, 40);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(53, 12);
            this.Label2.TabIndex = 46;
            this.Label2.Text = "数据端口";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(523, 40);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 12);
            this.Label1.TabIndex = 45;
            this.Label1.Text = "命令端口";
            // 
            // DataPortInput
            // 
            this.DataPortInput.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DataPortInput.Location = new System.Drawing.Point(839, 37);
            this.DataPortInput.MaxLength = 5;
            this.DataPortInput.Name = "DataPortInput";
            this.DataPortInput.Size = new System.Drawing.Size(59, 21);
            this.DataPortInput.TabIndex = 44;
            this.DataPortInput.Text = "9004";
            // 
            // CommandPortInput
            // 
            this.CommandPortInput.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.CommandPortInput.Location = new System.Drawing.Point(646, 37);
            this.CommandPortInput.MaxLength = 5;
            this.CommandPortInput.Name = "CommandPortInput";
            this.CommandPortInput.Size = new System.Drawing.Size(57, 21);
            this.CommandPortInput.TabIndex = 43;
            this.CommandPortInput.Text = "9003";
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(720, 566);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(178, 52);
            this.btnHide.TabIndex = 62;
            this.btnHide.Text = "关闭";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(525, 429);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(120, 16);
            this.checkBox1.TabIndex = 63;
            this.checkBox1.Text = "播放读码完成声音";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // YYKeyenceReaderConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 641);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnHide);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Reset);
            this.Controls.Add(this.button_Tune);
            this.Controls.Add(this.listBox_Reader);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_FTune);
            this.Controls.Add(this.textbox_DataConsole);
            this.Controls.Add(this.textBox_LogConsole);
            this.Controls.Add(this.receive);
            this.Controls.Add(this.loff);
            this.Controls.Add(this.lon);
            this.Controls.Add(this.disconnect);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.DataPortInput);
            this.Controls.Add(this.CommandPortInput);
            this.Name = "YYKeyenceReaderConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "读码器设置";
            this.Load += new System.EventHandler(this.YYKeyenceReaderConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Reset;
        private System.Windows.Forms.Button button_Tune;
        private System.Windows.Forms.ListBox listBox_Reader;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Button button_FTune;
        private System.Windows.Forms.TextBox textbox_DataConsole;
        private System.Windows.Forms.TextBox textBox_LogConsole;
        private System.Windows.Forms.Button receive;
        private System.Windows.Forms.Button lon;
        private System.Windows.Forms.Button disconnect;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label1;
        public System.Windows.Forms.TextBox DataPortInput;
        public System.Windows.Forms.TextBox CommandPortInput;
        private System.Windows.Forms.Button btnHide;
        internal System.Windows.Forms.Button loff;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}