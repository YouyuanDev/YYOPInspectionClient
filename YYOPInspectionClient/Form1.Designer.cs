namespace ReaderMaintenance
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.uploadBtn = new System.Windows.Forms.Button();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.commandResponseText = new System.Windows.Forms.TextBox();
            this.commandTxt = new System.Windows.Forms.ComboBox();
            this.readerSearchGroup = new System.Windows.Forms.GroupBox();
            this.nicComboBox = new System.Windows.Forms.ComboBox();
            this.Search_Btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.AllReadersSelectBtn = new System.Windows.Forms.CheckBox();
            this.SearchedReaders_ChkLstBx = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.downloadPath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maintainSelect = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.liveviewForm1 = new Keyence.AutoID.SDK.LiveviewForm();
            this.LiveViewReaderList = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.readerSearchGroup.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uploadBtn
            // 
            this.uploadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uploadBtn.Enabled = false;
            this.uploadBtn.Location = new System.Drawing.Point(11, 353);
            this.uploadBtn.Name = "uploadBtn";
            this.uploadBtn.Size = new System.Drawing.Size(134, 37);
            this.uploadBtn.TabIndex = 8;
            this.uploadBtn.Text = "SendFiles";
            this.uploadBtn.UseVisualStyleBackColor = true;
            this.uploadBtn.Click += new System.EventHandler(this.upload_Click);
            // 
            // downloadBtn
            // 
            this.downloadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadBtn.Location = new System.Drawing.Point(6, 22);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(99, 30);
            this.downloadBtn.TabIndex = 7;
            this.downloadBtn.Text = "ReceiveFiles";
            this.downloadBtn.UseVisualStyleBackColor = true;
            this.downloadBtn.Click += new System.EventHandler(this.download_Click);
            // 
            // commandResponseText
            // 
            this.commandResponseText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandResponseText.Location = new System.Drawing.Point(3, 53);
            this.commandResponseText.MaxLength = 10;
            this.commandResponseText.Multiline = true;
            this.commandResponseText.Name = "commandResponseText";
            this.commandResponseText.ReadOnly = true;
            this.commandResponseText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.commandResponseText.Size = new System.Drawing.Size(472, 136);
            this.commandResponseText.TabIndex = 6;
            // 
            // commandTxt
            // 
            this.commandTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandTxt.FormattingEnabled = true;
            this.commandTxt.Items.AddRange(new object[] {
            "KEYENCE",
            "LON",
            "LOFF",
            "TUNE,01",
            "FTUNE",
            "BLOAD,01",
            "SAVE",
            "BUSYSTAT"});
            this.commandTxt.Location = new System.Drawing.Point(6, 22);
            this.commandTxt.Name = "commandTxt";
            this.commandTxt.Size = new System.Drawing.Size(310, 23);
            this.commandTxt.TabIndex = 5;
            this.commandTxt.Text = "KEYENCE";
            this.commandTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            // 
            // readerSearchGroup
            // 
            this.readerSearchGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.readerSearchGroup.Controls.Add(this.nicComboBox);
            this.readerSearchGroup.Controls.Add(this.Search_Btn);
            this.readerSearchGroup.Location = new System.Drawing.Point(9, 3);
            this.readerSearchGroup.Name = "readerSearchGroup";
            this.readerSearchGroup.Size = new System.Drawing.Size(488, 49);
            this.readerSearchGroup.TabIndex = 15;
            this.readerSearchGroup.TabStop = false;
            this.readerSearchGroup.Text = "Reader Search";
            // 
            // nicComboBox
            // 
            this.nicComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nicComboBox.FormattingEnabled = true;
            this.nicComboBox.Location = new System.Drawing.Point(7, 19);
            this.nicComboBox.Name = "nicComboBox";
            this.nicComboBox.Size = new System.Drawing.Size(379, 23);
            this.nicComboBox.TabIndex = 18;
            this.nicComboBox.SelectedValueChanged += new System.EventHandler(this.nicComboBox_SelectedIndexChanged);
            // 
            // Search_Btn
            // 
            this.Search_Btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Search_Btn.Location = new System.Drawing.Point(401, 16);
            this.Search_Btn.Name = "Search_Btn";
            this.Search_Btn.Size = new System.Drawing.Size(65, 25);
            this.Search_Btn.TabIndex = 16;
            this.Search_Btn.Text = "Search";
            this.Search_Btn.UseVisualStyleBackColor = true;
            this.Search_Btn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 17;
            // 
            // AllReadersSelectBtn
            // 
            this.AllReadersSelectBtn.AutoSize = true;
            this.AllReadersSelectBtn.Location = new System.Drawing.Point(13, 21);
            this.AllReadersSelectBtn.Name = "AllReadersSelectBtn";
            this.AllReadersSelectBtn.Size = new System.Drawing.Size(131, 19);
            this.AllReadersSelectBtn.TabIndex = 19;
            this.AllReadersSelectBtn.Text = "Select All Readers";
            this.AllReadersSelectBtn.UseVisualStyleBackColor = true;
            this.AllReadersSelectBtn.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SearchedReaders_ChkLstBx
            // 
            this.SearchedReaders_ChkLstBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchedReaders_ChkLstBx.CheckOnClick = true;
            this.SearchedReaders_ChkLstBx.FormattingEnabled = true;
            this.SearchedReaders_ChkLstBx.Location = new System.Drawing.Point(11, 40);
            this.SearchedReaders_ChkLstBx.Name = "SearchedReaders_ChkLstBx";
            this.SearchedReaders_ChkLstBx.Size = new System.Drawing.Size(459, 76);
            this.SearchedReaders_ChkLstBx.TabIndex = 15;
            this.SearchedReaders_ChkLstBx.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SearchedReaders_ChkLstBx_ItemCheck);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.downloadPath);
            this.groupBox3.Controls.Add(this.downloadBtn);
            this.groupBox3.Location = new System.Drawing.Point(12, 326);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 69);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "CONFIG/Script File";
            // 
            // downloadPath
            // 
            this.downloadPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadPath.Location = new System.Drawing.Point(119, 27);
            this.downloadPath.Name = "downloadPath";
            this.downloadPath.ReadOnly = true;
            this.downloadPath.Size = new System.Drawing.Size(269, 23);
            this.downloadPath.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.commandTxt);
            this.groupBox2.Controls.Add(this.commandResponseText);
            this.groupBox2.Location = new System.Drawing.Point(9, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(481, 195);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Command";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(403, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 25);
            this.button1.TabIndex = 8;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(322, 22);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 25);
            this.button4.TabIndex = 7;
            this.button4.Text = "Send";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.maintainSelect);
            this.groupBox1.Controls.Add(this.uploadBtn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.liveviewForm1);
            this.groupBox1.Controls.Add(this.LiveViewReaderList);
            this.groupBox1.Location = new System.Drawing.Point(8, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 399);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SingleReader";
            // 
            // maintainSelect
            // 
            this.maintainSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maintainSelect.Appearance = System.Windows.Forms.Appearance.Button;
            this.maintainSelect.AutoSize = true;
            this.maintainSelect.Location = new System.Drawing.Point(263, 21);
            this.maintainSelect.Name = "maintainSelect";
            this.maintainSelect.Size = new System.Drawing.Size(53, 25);
            this.maintainSelect.TabIndex = 4;
            this.maintainSelect.Text = "Select";
            this.maintainSelect.UseVisualStyleBackColor = true;
            this.maintainSelect.CheckedChanged += new System.EventHandler(this.maintainSelect_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 364);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "--";
            // 
            // liveviewForm1
            // 
            this.liveviewForm1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.liveviewForm1.AutoSize = true;
            this.liveviewForm1.BackColor = System.Drawing.Color.Black;
            this.liveviewForm1.BinningType = Keyence.AutoID.SDK.LiveviewForm.ImageBinningType.OneQuarter;
            this.liveviewForm1.ImageFormat = Keyence.AutoID.SDK.LiveviewForm.ImageFormatType.Jpeg;
            this.liveviewForm1.ImageQuality = 5;
            this.liveviewForm1.IpAddress = "192.168.100.100";
            this.liveviewForm1.Location = new System.Drawing.Point(10, 59);
            this.liveviewForm1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.liveviewForm1.Name = "liveviewForm1";
            this.liveviewForm1.PullTimeSpan = 100;
            this.liveviewForm1.Size = new System.Drawing.Size(287, 274);
            this.liveviewForm1.TabIndex = 0;
            this.liveviewForm1.TimeoutMs = 2000;
            // 
            // LiveViewReaderList
            // 
            this.LiveViewReaderList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LiveViewReaderList.FormattingEnabled = true;
            this.LiveViewReaderList.Location = new System.Drawing.Point(11, 23);
            this.LiveViewReaderList.Name = "LiveViewReaderList";
            this.LiveViewReaderList.Size = new System.Drawing.Size(241, 23);
            this.LiveViewReaderList.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(861, 416);
            this.splitContainer1.SplitterDistance = 500;
            this.splitContainer1.TabIndex = 11;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.AllReadersSelectBtn);
            this.groupBox4.Controls.Add(this.SearchedReaders_ChkLstBx);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(494, 399);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "MultiReaders";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.readerSearchGroup);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(861, 476);
            this.splitContainer2.SplitterDistance = 56;
            this.splitContainer2.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(398, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "--";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 476);
            this.Controls.Add(this.splitContainer2);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "ReaderMaintenance";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.readerSearchGroup.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox commandResponseText;
        private System.Windows.Forms.ComboBox commandTxt;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Button uploadBtn;
        private System.Windows.Forms.ComboBox LiveViewReaderList;
        private Keyence.AutoID.SDK.LiveviewForm liveviewForm1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox readerSearchGroup;
        private System.Windows.Forms.CheckBox AllReadersSelectBtn;
        private System.Windows.Forms.ComboBox nicComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Search_Btn;
        private System.Windows.Forms.CheckedListBox SearchedReaders_ChkLstBx;
        private System.Windows.Forms.CheckBox maintainSelect;
        private System.Windows.Forms.TextBox downloadPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
    }
}

