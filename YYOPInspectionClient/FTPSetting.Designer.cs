namespace YYOPInspectionClient
{
    partial class FTPSetting
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFtpName = new System.Windows.Forms.TextBox();
            this.txtFtpPwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "FTP用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "FTP密码";
            // 
            // txtFtpName
            // 
            this.txtFtpName.Location = new System.Drawing.Point(121, 74);
            this.txtFtpName.Name = "txtFtpName";
            this.txtFtpName.Size = new System.Drawing.Size(100, 21);
            this.txtFtpName.TabIndex = 2;
            this.txtFtpName.Text = "ftpadmin";
            // 
            // txtFtpPwd
            // 
            this.txtFtpPwd.Location = new System.Drawing.Point(121, 126);
            this.txtFtpPwd.Name = "txtFtpPwd";
            this.txtFtpPwd.PasswordChar = '*';
            this.txtFtpPwd.Size = new System.Drawing.Size(100, 21);
            this.txtFtpPwd.TabIndex = 3;
            this.txtFtpPwd.Text = "123456";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "FTP（IP）";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(121, 23);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 21);
            this.txtIP.TabIndex = 5;
            this.txtIP.Text = "192.168.0.200";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(228, 165);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 39);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FTPSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 221);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFtpPwd);
            this.Controls.Add(this.txtFtpName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FTPSetting";
            this.Text = "FTP设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtFtpName;
        public System.Windows.Forms.TextBox txtFtpPwd;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button btnClose;
    }
}