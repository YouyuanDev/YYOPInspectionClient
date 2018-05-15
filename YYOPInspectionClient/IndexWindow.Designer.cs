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
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_inspection_record_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coupling_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contract_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machine_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.process_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operator_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_crew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.production_shift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coupling_heat_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coupling_lot_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.video_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
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
            this.服务器设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblIndexFormTitle = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 65);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1210, 498);
            this.panel1.TabIndex = 4;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeight = 50;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.thread_inspection_record_code,
            this.coupling_no,
            this.contract_no,
            this.production_line,
            this.machine_no,
            this.process_no,
            this.operator_no,
            this.production_crew,
            this.production_shift,
            this.coupling_heat_no,
            this.coupling_lot_no,
            this.video_no,
            this.inspection_result,
            this.inspection_time});
            this.dataGridView1.Location = new System.Drawing.Point(0, 63);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 50;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1210, 435);
            this.dataGridView1.TabIndex = 1;
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
            // coupling_no
            // 
            this.coupling_no.DataPropertyName = "coupling_no";
            this.coupling_no.HeaderText = "接箍编号";
            this.coupling_no.Name = "coupling_no";
            this.coupling_no.ReadOnly = true;
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
            // process_no
            // 
            this.process_no.DataPropertyName = "process_no";
            this.process_no.HeaderText = "工位编号";
            this.process_no.Name = "process_no";
            this.process_no.ReadOnly = true;
            this.process_no.Visible = false;
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
            // coupling_heat_no
            // 
            this.coupling_heat_no.DataPropertyName = "coupling_heat_no";
            this.coupling_heat_no.HeaderText = "接箍炉号";
            this.coupling_heat_no.Name = "coupling_heat_no";
            this.coupling_heat_no.ReadOnly = true;
            // 
            // coupling_lot_no
            // 
            this.coupling_lot_no.DataPropertyName = "coupling_lot_no";
            this.coupling_lot_no.HeaderText = "接箍批号";
            this.coupling_lot_no.Name = "coupling_lot_no";
            this.coupling_lot_no.ReadOnly = true;
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
            this.inspection_result.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // inspection_time
            // 
            this.inspection_time.DataPropertyName = "inspection_time";
            this.inspection_time.HeaderText = "检验时间";
            this.inspection_time.Name = "inspection_time";
            this.inspection_time.ReadOnly = true;
            this.inspection_time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnExit);
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
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1212, 69);
            this.panel2.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.Location = new System.Drawing.Point(1028, 9);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 46);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "登出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cmbAcceptanceNo
            // 
            this.cmbAcceptanceNo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbAcceptanceNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAcceptanceNo.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbAcceptanceNo.FormattingEnabled = true;
            this.cmbAcceptanceNo.ItemHeight = 40;
            this.cmbAcceptanceNo.Location = new System.Drawing.Point(658, 9);
            this.cmbAcceptanceNo.Name = "cmbAcceptanceNo";
            this.cmbAcceptanceNo.Size = new System.Drawing.Size(150, 46);
            this.cmbAcceptanceNo.TabIndex = 13;
            this.cmbAcceptanceNo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbAcceptanceNo_DrawItem);
            // 
            // cmbThreadType
            // 
            this.cmbThreadType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbThreadType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThreadType.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbThreadType.FormattingEnabled = true;
            this.cmbThreadType.ItemHeight = 40;
            this.cmbThreadType.Location = new System.Drawing.Point(449, 9);
            this.cmbThreadType.Name = "cmbThreadType";
            this.cmbThreadType.Size = new System.Drawing.Size(118, 46);
            this.cmbThreadType.TabIndex = 12;
            this.cmbThreadType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbThreadType_DrawItem);
            // 
            // cmbWt
            // 
            this.cmbWt.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbWt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWt.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbWt.FormattingEnabled = true;
            this.cmbWt.ItemHeight = 40;
            this.cmbWt.Location = new System.Drawing.Point(226, 9);
            this.cmbWt.Name = "cmbWt";
            this.cmbWt.Size = new System.Drawing.Size(118, 46);
            this.cmbWt.TabIndex = 11;
            this.cmbWt.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbWt_DrawItem);
            // 
            // cmbOd
            // 
            this.cmbOd.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbOd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOd.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbOd.FormattingEnabled = true;
            this.cmbOd.ItemHeight = 40;
            this.cmbOd.Location = new System.Drawing.Point(51, 9);
            this.cmbOd.Name = "cmbOd";
            this.cmbOd.Size = new System.Drawing.Size(118, 46);
            this.cmbOd.TabIndex = 10;
            this.cmbOd.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbOd_DrawItem);
            // 
            // btnDetail
            // 
            this.btnDetail.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDetail.Location = new System.Drawing.Point(921, 9);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(90, 46);
            this.btnDetail.TabIndex = 9;
            this.btnDetail.Text = "详细";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearch.Location = new System.Drawing.Point(814, 9);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 46);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(570, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 54);
            this.label4.TabIndex = 3;
            this.label4.Text = "标准编号";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(352, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 54);
            this.label3.TabIndex = 2;
            this.label3.Text = "螺纹类型";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(175, 5);
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
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(4, 7);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(20, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(605, 50);
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
            // 服务器设置ToolStripMenuItem
            // 
            this.服务器设置ToolStripMenuItem.Name = "服务器设置ToolStripMenuItem";
            this.服务器设置ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.服务器设置ToolStripMenuItem.Size = new System.Drawing.Size(198, 62);
            this.服务器设置ToolStripMenuItem.Text = "服务器设置";
            this.服务器设置ToolStripMenuItem.Click += new System.EventHandler(this.服务器设置ToolStripMenuItem_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.Controls.Add(this.lblIndexFormTitle);
            this.panel3.Controls.Add(this.menuStrip1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1211, 62);
            this.panel3.TabIndex = 6;
            // 
            // lblIndexFormTitle
            // 
            this.lblIndexFormTitle.AutoSize = true;
            this.lblIndexFormTitle.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblIndexFormTitle.Location = new System.Drawing.Point(914, 23);
            this.lblIndexFormTitle.Name = "lblIndexFormTitle";
            this.lblIndexFormTitle.Size = new System.Drawing.Size(82, 24);
            this.lblIndexFormTitle.TabIndex = 0;
            this.lblIndexFormTitle.Text = "label5";
            // 
            // IndexWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1211, 572);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IndexWindow";
            this.Text = "主页";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndexWindow_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.IndexWindow_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
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
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.ComboBox cmbAcceptanceNo;
        private System.Windows.Forms.ComboBox cmbThreadType;
        private System.Windows.Forms.ComboBox cmbWt;
        private System.Windows.Forms.ComboBox cmbOd;
        private System.Windows.Forms.ToolStripMenuItem 服务器设置ToolStripMenuItem;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_inspection_record_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn coupling_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn contract_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_line;
        private System.Windows.Forms.DataGridViewTextBoxColumn machine_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn process_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn operator_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_crew;
        private System.Windows.Forms.DataGridViewTextBoxColumn production_shift;
        private System.Windows.Forms.DataGridViewTextBoxColumn coupling_heat_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn coupling_lot_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn video_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_result;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_time;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label lblIndexFormTitle;
    }
}