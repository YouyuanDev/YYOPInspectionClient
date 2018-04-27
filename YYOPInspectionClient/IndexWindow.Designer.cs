namespace YYOPInspectionClient
{
    partial class IndexWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbAcceptanceNo = new System.Windows.Forms.ComboBox();
            this.cmbThreadType = new System.Windows.Forms.ComboBox();
            this.cmbWt = new System.Windows.Forms.ComboBox();
            this.cmbOd = new System.Windows.Forms.ComboBox();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.新建ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.未提交ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.读码器设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.录像设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fTP设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.服务器设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_inspection_record_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.couping_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contract_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machine_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operator_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_crew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_shift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.video_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 50);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1210, 560);
            this.panel1.TabIndex = 4;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.thread_inspection_record_code,
            this.couping_no,
            this.contract_no,
            this.production_line,
            this.machine_no,
            this.operator_no,
            this.production_crew,
            this.production_shift,
            this.video_no,
            this.inspection_result,
            this.inspection_time});
            this.dataGridView1.Location = new System.Drawing.Point(0, 64);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1210, 496);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cmbAcceptanceNo);
            this.panel2.Controls.Add(this.cmbThreadType);
            this.panel2.Controls.Add(this.cmbWt);
            this.panel2.Controls.Add(this.cmbOd);
            this.panel2.Controls.Add(this.btnDetail);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(0, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1212, 64);
            this.panel2.TabIndex = 0;
            // 
            // cmbAcceptanceNo
            // 
            this.cmbAcceptanceNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAcceptanceNo.FormattingEnabled = true;
            this.cmbAcceptanceNo.Location = new System.Drawing.Point(507, 23);
            this.cmbAcceptanceNo.Name = "cmbAcceptanceNo";
            this.cmbAcceptanceNo.Size = new System.Drawing.Size(104, 20);
            this.cmbAcceptanceNo.TabIndex = 13;
            // 
            // cmbThreadType
            // 
            this.cmbThreadType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThreadType.FormattingEnabled = true;
            this.cmbThreadType.Location = new System.Drawing.Point(342, 22);
            this.cmbThreadType.Name = "cmbThreadType";
            this.cmbThreadType.Size = new System.Drawing.Size(79, 20);
            this.cmbThreadType.TabIndex = 12;
            // 
            // cmbWt
            // 
            this.cmbWt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWt.FormattingEnabled = true;
            this.cmbWt.Location = new System.Drawing.Point(176, 22);
            this.cmbWt.Name = "cmbWt";
            this.cmbWt.Size = new System.Drawing.Size(79, 20);
            this.cmbWt.TabIndex = 11;
            // 
            // cmbOd
            // 
            this.cmbOd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOd.FormattingEnabled = true;
            this.cmbOd.Location = new System.Drawing.Point(51, 23);
            this.cmbOd.Name = "cmbOd";
            this.cmbOd.Size = new System.Drawing.Size(79, 20);
            this.cmbOd.TabIndex = 10;
            // 
            // btnDetail
            // 
            this.btnDetail.Location = new System.Drawing.Point(724, 12);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(90, 38);
            this.btnDetail.TabIndex = 9;
            this.btnDetail.Text = "详细";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearch.Location = new System.Drawing.Point(617, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 38);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(420, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 54);
            this.label4.TabIndex = 3;
            this.label4.Text = "标准编号";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(251, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 54);
            this.label3.TabIndex = 2;
            this.label3.Text = "螺纹类型";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(125, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 54);
            this.label2.TabIndex = 1;
            this.label2.Text = "壁厚";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 54);
            this.label1.TabIndex = 0;
            this.label1.Text = "外径";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(20, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1211, 50);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建ToolStripMenuItem,
            this.未提交ToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.toolStripMenuItem1.Size = new System.Drawing.Size(93, 46);
            this.toolStripMenuItem1.Text = "表单";
            // 
            // 新建ToolStripMenuItem
            // 
            this.新建ToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.新建ToolStripMenuItem.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.新建ToolStripMenuItem.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.新建ToolStripMenuItem.Name = "新建ToolStripMenuItem";
            this.新建ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.新建ToolStripMenuItem.Size = new System.Drawing.Size(158, 62);
            this.新建ToolStripMenuItem.Text = "新建";
            this.新建ToolStripMenuItem.Click += new System.EventHandler(this.新建ToolStripMenuItem_Click);
            // 
            // 未提交ToolStripMenuItem
            // 
            this.未提交ToolStripMenuItem.Name = "未提交ToolStripMenuItem";
            this.未提交ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.未提交ToolStripMenuItem.Size = new System.Drawing.Size(158, 62);
            this.未提交ToolStripMenuItem.Text = "未提交";
            this.未提交ToolStripMenuItem.Click += new System.EventHandler(this.未提交ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.读码器设置ToolStripMenuItem,
            this.录像设置ToolStripMenuItem,
            this.fTP设置ToolStripMenuItem,
            this.服务器设置ToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.toolStripMenuItem2.Size = new System.Drawing.Size(93, 46);
            this.toolStripMenuItem2.Text = "设置";
            // 
            // 读码器设置ToolStripMenuItem
            // 
            this.读码器设置ToolStripMenuItem.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.读码器设置ToolStripMenuItem.Name = "读码器设置ToolStripMenuItem";
            this.读码器设置ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.读码器设置ToolStripMenuItem.Size = new System.Drawing.Size(198, 62);
            this.读码器设置ToolStripMenuItem.Text = "读码器设置";
            this.读码器设置ToolStripMenuItem.Click += new System.EventHandler(this.读码器设置ToolStripMenuItem_Click);
            // 
            // 录像设置ToolStripMenuItem
            // 
            this.录像设置ToolStripMenuItem.Name = "录像设置ToolStripMenuItem";
            this.录像设置ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.录像设置ToolStripMenuItem.Size = new System.Drawing.Size(198, 62);
            this.录像设置ToolStripMenuItem.Text = "录像设置";
            this.录像设置ToolStripMenuItem.Click += new System.EventHandler(this.录像设置ToolStripMenuItem_Click);
            // 
            // fTP设置ToolStripMenuItem
            // 
            this.fTP设置ToolStripMenuItem.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.fTP设置ToolStripMenuItem.Name = "fTP设置ToolStripMenuItem";
            this.fTP设置ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.fTP设置ToolStripMenuItem.Size = new System.Drawing.Size(198, 62);
            this.fTP设置ToolStripMenuItem.Text = "FTP设置";
            this.fTP设置ToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.fTP设置ToolStripMenuItem.Click += new System.EventHandler(this.fTP设置ToolStripMenuItem_Click);
            // 
            // 服务器设置ToolStripMenuItem
            // 
            this.服务器设置ToolStripMenuItem.Name = "服务器设置ToolStripMenuItem";
            this.服务器设置ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.服务器设置ToolStripMenuItem.Size = new System.Drawing.Size(198, 62);
            this.服务器设置ToolStripMenuItem.Text = "服务器设置";
            this.服务器设置ToolStripMenuItem.Click += new System.EventHandler(this.服务器设置ToolStripMenuItem_Click);
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "流水号";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            // 
            // thread_inspection_record_code
            // 
            this.thread_inspection_record_code.DataPropertyName = "thread_inspection_record_code";
            this.thread_inspection_record_code.HeaderText = "接箍检验编号";
            this.thread_inspection_record_code.Name = "thread_inspection_record_code";
            this.thread_inspection_record_code.ReadOnly = true;
            // 
            // couping_no
            // 
            this.couping_no.DataPropertyName = "couping_no";
            this.couping_no.HeaderText = "接箍编号";
            this.couping_no.Name = "couping_no";
            this.couping_no.ReadOnly = true;
            // 
            // contract_no
            // 
            this.contract_no.DataPropertyName = "contract_no";
            this.contract_no.HeaderText = "合同号";
            this.contract_no.Name = "contract_no";
            this.contract_no.ReadOnly = true;
            // 
            // production_line
            // 
            this.production_line.DataPropertyName = "production_line";
            this.production_line.HeaderText = "生产区域";
            this.production_line.Name = "production_line";
            this.production_line.ReadOnly = true;
            // 
            // machine_no
            // 
            this.machine_no.DataPropertyName = "machine_no";
            this.machine_no.HeaderText = "机床号";
            this.machine_no.Name = "machine_no";
            this.machine_no.ReadOnly = true;
            // 
            // operator_no
            // 
            this.operator_no.DataPropertyName = "operator_no";
            this.operator_no.HeaderText = "操作工工号";
            this.operator_no.Name = "operator_no";
            this.operator_no.ReadOnly = true;
            // 
            // production_crew
            // 
            this.production_crew.DataPropertyName = "production_crew";
            this.production_crew.HeaderText = "班别";
            this.production_crew.Name = "production_crew";
            this.production_crew.ReadOnly = true;
            // 
            // production_shift
            // 
            this.production_shift.DataPropertyName = "production_shift";
            this.production_shift.HeaderText = "班次";
            this.production_shift.Name = "production_shift";
            this.production_shift.ReadOnly = true;
            // 
            // video_no
            // 
            this.video_no.DataPropertyName = "video_no";
            this.video_no.HeaderText = "视频编号";
            this.video_no.Name = "video_no";
            this.video_no.ReadOnly = true;
            // 
            // inspection_result
            // 
            this.inspection_result.DataPropertyName = "inspection_result";
            this.inspection_result.HeaderText = "检验结果";
            this.inspection_result.Name = "inspection_result";
            this.inspection_result.ReadOnly = true;
            // 
            // inspection_time
            // 
            this.inspection_time.DataPropertyName = "inspection_time";
            this.inspection_time.HeaderText = "检验时间";
            this.inspection_time.Name = "inspection_time";
            this.inspection_time.ReadOnly = true;
            // 
            // IndexWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1211, 614);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "IndexWindow";
            this.Text = "主页";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndexWindow_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 新建ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 未提交ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读码器设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 录像设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fTP设置ToolStripMenuItem;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.ComboBox cmbAcceptanceNo;
        private System.Windows.Forms.ComboBox cmbThreadType;
        private System.Windows.Forms.ComboBox cmbWt;
        private System.Windows.Forms.ComboBox cmbOd;
        private System.Windows.Forms.ToolStripMenuItem 服务器设置ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_inspection_record_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn couping_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn contract_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_line;
        private System.Windows.Forms.DataGridViewTextBoxColumn machine_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn operator_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_crew;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_shift;
        private System.Windows.Forms.DataGridViewTextBoxColumn video_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_result;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_time;
    }
}