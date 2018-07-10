namespace YYOPInspectionClient
{
    partial class MainWindow
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
            this.RealPlayWnd = new System.Windows.Forms.PictureBox();
            this.TextBoxInfo = new System.Windows.Forms.TextBox();
            this.checkBoxHiDDNS = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnBMP = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnJPEG = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxView = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.listViewIPChannel = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.RealPlayWnd)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // RealPlayWnd
            // 
            this.RealPlayWnd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RealPlayWnd.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.RealPlayWnd.Location = new System.Drawing.Point(223, 132);
            this.RealPlayWnd.Name = "RealPlayWnd";
            this.RealPlayWnd.Size = new System.Drawing.Size(428, 341);
            this.RealPlayWnd.TabIndex = 62;
            this.RealPlayWnd.TabStop = false;
            this.RealPlayWnd.Tag = "normal";
            this.RealPlayWnd.Click += new System.EventHandler(this.RealPlayWnd_Click);
            // 
            // TextBoxInfo
            // 
            this.TextBoxInfo.Location = new System.Drawing.Point(5, 17);
            this.TextBoxInfo.Multiline = true;
            this.TextBoxInfo.Name = "TextBoxInfo";
            this.TextBoxInfo.Size = new System.Drawing.Size(621, 148);
            this.TextBoxInfo.TabIndex = 72;
            // 
            // checkBoxHiDDNS
            // 
            this.checkBoxHiDDNS.AutoSize = true;
            this.checkBoxHiDDNS.Enabled = false;
            this.checkBoxHiDDNS.Location = new System.Drawing.Point(9, 16);
            this.checkBoxHiDDNS.Name = "checkBoxHiDDNS";
            this.checkBoxHiDDNS.Size = new System.Drawing.Size(108, 16);
            this.checkBoxHiDDNS.TabIndex = 29;
            this.checkBoxHiDDNS.Text = "HiDDNS域名登录";
            this.checkBoxHiDDNS.UseVisualStyleBackColor = true;
            this.checkBoxHiDDNS.CheckedChanged += new System.EventHandler(this.checkBoxHiDDNS_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Device IP/Domain";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "User Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "用户名";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxPassword.Location = new System.Drawing.Point(371, 81);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(112, 21);
            this.textBoxPassword.TabIndex = 23;
            this.textBoxPassword.Text = "c12345678";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(285, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 27;
            this.label8.Text = "密码";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(103, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 67;
            this.label14.Text = "BMP抓图";
            this.label14.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 65;
            this.label11.Text = "预览";
            this.label11.Visible = false;
            // 
            // btnBMP
            // 
            this.btnBMP.Location = new System.Drawing.Point(89, 14);
            this.btnBMP.Name = "btnBMP";
            this.btnBMP.Size = new System.Drawing.Size(79, 27);
            this.btnBMP.TabIndex = 66;
            this.btnBMP.Text = "Capture BMP";
            this.btnBMP.UseVisualStyleBackColor = true;
            this.btnBMP.Visible = false;
            this.btnBMP.Click += new System.EventHandler(this.btnBMP_Click_1);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(371, 43);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(112, 21);
            this.textBoxPort.TabIndex = 21;
            this.textBoxPort.Text = "8000";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(121, 43);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(141, 21);
            this.textBoxIP.TabIndex = 20;
            this.textBoxIP.Text = "100.100.0.90";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(283, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "设备端口";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(517, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 28;
            this.label10.Text = "登录";
            this.label10.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(283, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "设备IP或域名";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.btnRecord);
            this.groupBox2.Controls.Add(this.btnBMP);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.btnJPEG);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Location = new System.Drawing.Point(668, 279);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 171);
            this.groupBox2.TabIndex = 68;
            this.groupBox2.TabStop = false;
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox2_Paint);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 12);
            this.label12.TabIndex = 47;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(103, 87);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 43;
            this.label9.Text = "JPEG抓图";
            this.label9.Visible = false;
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(0, 123);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(97, 48);
            this.btnRecord.TabIndex = 44;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Visible = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnJPEG
            // 
            this.btnJPEG.Location = new System.Drawing.Point(0, 72);
            this.btnJPEG.Name = "btnJPEG";
            this.btnJPEG.Size = new System.Drawing.Size(97, 47);
            this.btnJPEG.TabIndex = 70;
            this.btnJPEG.Text = "Capture JPEG";
            this.btnJPEG.UseVisualStyleBackColor = true;
            this.btnJPEG.Visible = false;
            this.btnJPEG.Click += new System.EventHandler(this.btnJPEG_Click_1);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(103, 137);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 74;
            this.label15.Text = "客户端录像";
            this.label15.Visible = false;
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(121, 81);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(141, 21);
            this.textBoxUserName.TabIndex = 22;
            this.textBoxUserName.Text = "admin";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(498, 52);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(78, 50);
            this.btnLogin.TabIndex = 19;
            this.btnLogin.Text = "登录";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxHiDDNS);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.textBoxPassword);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textBoxPort);
            this.groupBox3.Controls.Add(this.textBoxIP);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBoxUserName);
            this.groupBox3.Controls.Add(this.btnLogin);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(31, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(621, 117);
            this.groupBox3.TabIndex = 69;
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(283, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Device Port";
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(645, 30);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(126, 47);
            this.btnHide.TabIndex = 77;
            this.btnHide.Text = "关闭";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(6, 210);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(139, 125);
            this.btnPreview.TabIndex = 64;
            this.btnPreview.Text = "启动录像机";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click_1);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(86, 127);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 75;
            this.label13.Text = "刷新通道列表";
            this.label13.Visible = false;
            // 
            // comboBoxView
            // 
            this.comboBoxView.FormattingEnabled = true;
            this.comboBoxView.Items.AddRange(new object[] {
            "Direct:直接预览 ",
            "Callback:回调解码 "});
            this.comboBoxView.Location = new System.Drawing.Point(6, 164);
            this.comboBoxView.Name = "comboBoxView";
            this.comboBoxView.Size = new System.Drawing.Size(139, 20);
            this.comboBoxView.TabIndex = 76;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(6, 122);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(74, 36);
            this.btnRefresh.TabIndex = 73;
            this.btnRefresh.Text = "刷新通道";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // listViewIPChannel
            // 
            this.listViewIPChannel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2});
            this.listViewIPChannel.FullRowSelect = true;
            this.listViewIPChannel.GridLines = true;
            this.listViewIPChannel.Location = new System.Drawing.Point(6, 20);
            this.listViewIPChannel.MultiSelect = false;
            this.listViewIPChannel.Name = "listViewIPChannel";
            this.listViewIPChannel.Size = new System.Drawing.Size(174, 91);
            this.listViewIPChannel.TabIndex = 63;
            this.listViewIPChannel.UseCompatibleStateImageBehavior = false;
            this.listViewIPChannel.View = System.Windows.Forms.View.Details;
            this.listViewIPChannel.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewIPChannel_ItemSelectionChanged);
            this.listViewIPChannel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewIPChannel_MouseClick);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "通道 Channel";
            this.ColumnHeader1.Width = 90;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "状态 Status";
            this.ColumnHeader2.Width = 90;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.listViewIPChannel);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.comboBoxView);
            this.groupBox1.Location = new System.Drawing.Point(31, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(191, 341);
            this.groupBox1.TabIndex = 78;
            this.groupBox1.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.TextBoxInfo);
            this.groupBox4.Controls.Add(this.btnHide);
            this.groupBox4.Location = new System.Drawing.Point(37, 479);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(787, 165);
            this.groupBox4.TabIndex = 79;
            this.groupBox4.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 656);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RealPlayWnd);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Name = "MainWindow";
            this.ShowInTaskbar = false;
            this.Text = "录像设置";
            ((System.ComponentModel.ISupportInitialize)(this.RealPlayWnd)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox TextBoxInfo;
        private System.Windows.Forms.CheckBox checkBoxHiDDNS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnBMP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnJPEG;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.ComboBox comboBoxView;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ListView listViewIPChannel;
        private System.Windows.Forms.ColumnHeader ColumnHeader1;
        private System.Windows.Forms.ColumnHeader ColumnHeader2;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.PictureBox RealPlayWnd;
    }
}