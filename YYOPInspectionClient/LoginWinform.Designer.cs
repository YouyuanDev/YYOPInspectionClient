namespace YYOPInspectionClient
{
    partial class LoginWinform
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
            this.txtLoginName = new System.Windows.Forms.TextBox();
            this.txtLoginPwd = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pnlLoginTitle = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblLoginTitle = new System.Windows.Forms.Label();
            this.btnLoginOut = new System.Windows.Forms.Button();
            this.pingLbl = new System.Windows.Forms.Label();
            this.pnlLoginTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(108, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(108, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "密  码：";
            // 
            // txtLoginName
            // 
            this.txtLoginName.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLoginName.Location = new System.Drawing.Point(211, 59);
            this.txtLoginName.Name = "txtLoginName";
            this.txtLoginName.Size = new System.Drawing.Size(100, 32);
            this.txtLoginName.TabIndex = 2;
            this.txtLoginName.Tag = "English";
            this.txtLoginName.Text = "1111";
            this.txtLoginName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtLoginName_MouseDown);
            // 
            // txtLoginPwd
            // 
            this.txtLoginPwd.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLoginPwd.Location = new System.Drawing.Point(210, 121);
            this.txtLoginPwd.Name = "txtLoginPwd";
            this.txtLoginPwd.PasswordChar = '*';
            this.txtLoginPwd.Size = new System.Drawing.Size(100, 32);
            this.txtLoginPwd.TabIndex = 3;
            this.txtLoginPwd.Tag = "English";
            this.txtLoginPwd.Text = "123456";
            this.txtLoginPwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtLoginPwd_MouseDown);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(63, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 52);
            this.button1.TabIndex = 4;
            this.button1.Text = "登录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pnlLoginTitle
            // 
            this.pnlLoginTitle.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlLoginTitle.Controls.Add(this.btnClose);
            this.pnlLoginTitle.Controls.Add(this.lblLoginTitle);
            this.pnlLoginTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLoginTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlLoginTitle.Name = "pnlLoginTitle";
            this.pnlLoginTitle.Size = new System.Drawing.Size(410, 39);
            this.pnlLoginTitle.TabIndex = 5;
            this.pnlLoginTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlLoginTitle_MouseDown);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnClose.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(367, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(43, 39);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "×";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblLoginTitle
            // 
            this.lblLoginTitle.AutoSize = true;
            this.lblLoginTitle.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLoginTitle.Location = new System.Drawing.Point(4, 11);
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Size = new System.Drawing.Size(40, 16);
            this.lblLoginTitle.TabIndex = 0;
            this.lblLoginTitle.Text = "登录";
            this.lblLoginTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblLoginTitle_MouseDown);
            // 
            // btnLoginOut
            // 
            this.btnLoginOut.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLoginOut.Location = new System.Drawing.Point(229, 185);
            this.btnLoginOut.Name = "btnLoginOut";
            this.btnLoginOut.Size = new System.Drawing.Size(143, 52);
            this.btnLoginOut.TabIndex = 6;
            this.btnLoginOut.Text = "退出";
            this.btnLoginOut.UseVisualStyleBackColor = true;
            this.btnLoginOut.Click += new System.EventHandler(this.btnLoginOut_Click);
            // 
            // pingLbl
            // 
            this.pingLbl.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pingLbl.ForeColor = System.Drawing.Color.IndianRed;
            this.pingLbl.Location = new System.Drawing.Point(105, 248);
            this.pingLbl.Name = "pingLbl";
            this.pingLbl.Size = new System.Drawing.Size(212, 22);
            this.pingLbl.TabIndex = 7;
            this.pingLbl.Text = " ";
            this.pingLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginWinform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 275);
            this.Controls.Add(this.pingLbl);
            this.Controls.Add(this.btnLoginOut);
            this.Controls.Add(this.pnlLoginTitle);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtLoginPwd);
            this.Controls.Add(this.txtLoginName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginWinform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginWinform_MouseDown);
            this.pnlLoginTitle.ResumeLayout(false);
            this.pnlLoginTitle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLoginName;
        private System.Windows.Forms.TextBox txtLoginPwd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel pnlLoginTitle;
        private System.Windows.Forms.Label lblLoginTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnLoginOut;
        internal System.Windows.Forms.Label pingLbl;
    }
}