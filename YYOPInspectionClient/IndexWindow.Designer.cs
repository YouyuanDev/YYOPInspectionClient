﻿namespace YYOPInspectionClient
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
            this.couping_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.process_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operator_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.visual_inspection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_tooth_pitch_diameter_max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_tooth_pitch_diameter_avg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_tooth_pitch_diameter_min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_sealing_surface_diameter_max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_sealing_surface_diameter_avg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_sealing_surface_diameter_min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_sealing_surface_ovality = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_pitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_taper = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_height = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_length_min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_bearing_surface_width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.couping_inner_end_depth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_hole_inner_diameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.couping_od = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.couping_length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_tooth_angle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.thread_throug_hole_size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.video_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tool_measuring_record_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inspection_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginTime = new System.Windows.Forms.DateTimePicker();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            this.button1 = new System.Windows.Forms.Button();
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
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.couping_no,
            this.process_no,
            this.operator_no,
            this.visual_inspection,
            this.thread_tooth_pitch_diameter_max,
            this.thread_tooth_pitch_diameter_avg,
            this.thread_tooth_pitch_diameter_min,
            this.thread_sealing_surface_diameter_max,
            this.thread_sealing_surface_diameter_avg,
            this.thread_sealing_surface_diameter_min,
            this.thread_sealing_surface_ovality,
            this.thread_width,
            this.thread_pitch,
            this.thread_taper,
            this.thread_height,
            this.thread_length_min,
            this.thread_bearing_surface_width,
            this.couping_inner_end_depth,
            this.thread_hole_inner_diameter,
            this.couping_od,
            this.couping_length,
            this.thread_tooth_angle,
            this.thread_throug_hole_size,
            this.video_no,
            this.tool_measuring_record_no,
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
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "流水号";
            this.id.Name = "id";
            // 
            // couping_no
            // 
            this.couping_no.DataPropertyName = "couping_no";
            this.couping_no.HeaderText = "接箍编号";
            this.couping_no.Name = "couping_no";
            // 
            // process_no
            // 
            this.process_no.DataPropertyName = "process_no";
            this.process_no.HeaderText = "工位编号";
            this.process_no.Name = "process_no";
            // 
            // operator_no
            // 
            this.operator_no.DataPropertyName = "operator_no";
            this.operator_no.HeaderText = "操作工工号";
            this.operator_no.Name = "operator_no";
            // 
            // visual_inspection
            // 
            this.visual_inspection.DataPropertyName = "visual_inspection";
            this.visual_inspection.HeaderText = "视觉检验";
            this.visual_inspection.Name = "visual_inspection";
            // 
            // thread_tooth_pitch_diameter_max
            // 
            this.thread_tooth_pitch_diameter_max.DataPropertyName = "thread_tooth_pitch_diameter_max";
            this.thread_tooth_pitch_diameter_max.HeaderText = "螺纹齿顶中径最大值";
            this.thread_tooth_pitch_diameter_max.Name = "thread_tooth_pitch_diameter_max";
            // 
            // thread_tooth_pitch_diameter_avg
            // 
            this.thread_tooth_pitch_diameter_avg.DataPropertyName = "thread_tooth_pitch_diameter_avg";
            this.thread_tooth_pitch_diameter_avg.HeaderText = "螺纹齿顶中径平均值";
            this.thread_tooth_pitch_diameter_avg.Name = "thread_tooth_pitch_diameter_avg";
            // 
            // thread_tooth_pitch_diameter_min
            // 
            this.thread_tooth_pitch_diameter_min.DataPropertyName = "thread_tooth_pitch_diameter_min";
            this.thread_tooth_pitch_diameter_min.HeaderText = "螺纹齿顶中径最小值";
            this.thread_tooth_pitch_diameter_min.Name = "thread_tooth_pitch_diameter_min";
            // 
            // thread_sealing_surface_diameter_max
            // 
            this.thread_sealing_surface_diameter_max.DataPropertyName = "thread_sealing_surface_diameter_max";
            this.thread_sealing_surface_diameter_max.HeaderText = "螺纹密封面直径最大值";
            this.thread_sealing_surface_diameter_max.Name = "thread_sealing_surface_diameter_max";
            // 
            // thread_sealing_surface_diameter_avg
            // 
            this.thread_sealing_surface_diameter_avg.DataPropertyName = "thread_sealing_surface_diameter_avg";
            this.thread_sealing_surface_diameter_avg.HeaderText = "螺纹密封面直径平均值";
            this.thread_sealing_surface_diameter_avg.Name = "thread_sealing_surface_diameter_avg";
            // 
            // thread_sealing_surface_diameter_min
            // 
            this.thread_sealing_surface_diameter_min.DataPropertyName = "thread_sealing_surface_diameter_min";
            this.thread_sealing_surface_diameter_min.HeaderText = "螺纹密封面直径最小值";
            this.thread_sealing_surface_diameter_min.Name = "thread_sealing_surface_diameter_min";
            this.thread_sealing_surface_diameter_min.Visible = false;
            // 
            // thread_sealing_surface_ovality
            // 
            this.thread_sealing_surface_ovality.DataPropertyName = "thread_sealing_surface_ovality";
            this.thread_sealing_surface_ovality.HeaderText = "螺纹及密封面椭圆度";
            this.thread_sealing_surface_ovality.Name = "thread_sealing_surface_ovality";
            // 
            // thread_width
            // 
            this.thread_width.DataPropertyName = "thread_width";
            this.thread_width.HeaderText = "螺纹齿宽";
            this.thread_width.Name = "thread_width";
            // 
            // thread_pitch
            // 
            this.thread_pitch.DataPropertyName = "thread_pitch";
            this.thread_pitch.HeaderText = "螺纹螺距";
            this.thread_pitch.Name = "thread_pitch";
            // 
            // thread_taper
            // 
            this.thread_taper.DataPropertyName = "thread_taper";
            this.thread_taper.HeaderText = "螺纹锥度";
            this.thread_taper.Name = "thread_taper";
            // 
            // thread_height
            // 
            this.thread_height.DataPropertyName = "thread_height";
            this.thread_height.HeaderText = "螺纹齿高";
            this.thread_height.Name = "thread_height";
            // 
            // thread_length_min
            // 
            this.thread_length_min.DataPropertyName = "thread_length_min";
            this.thread_length_min.HeaderText = "最小螺纹长度";
            this.thread_length_min.Name = "thread_length_min";
            // 
            // thread_bearing_surface_width
            // 
            this.thread_bearing_surface_width.DataPropertyName = "thread_bearing_surface_width";
            this.thread_bearing_surface_width.HeaderText = "承载面宽度";
            this.thread_bearing_surface_width.Name = "thread_bearing_surface_width";
            // 
            // couping_inner_end_depth
            // 
            this.couping_inner_end_depth.DataPropertyName = "couping_inner_end_depth";
            this.couping_inner_end_depth.HeaderText = "内端面宽度";
            this.couping_inner_end_depth.Name = "couping_inner_end_depth";
            // 
            // thread_hole_inner_diameter
            // 
            this.thread_hole_inner_diameter.DataPropertyName = "thread_hole_inner_diameter";
            this.thread_hole_inner_diameter.HeaderText = "通孔内径";
            this.thread_hole_inner_diameter.Name = "thread_hole_inner_diameter";
            // 
            // couping_od
            // 
            this.couping_od.DataPropertyName = "couping_od";
            this.couping_od.HeaderText = "接箍外径";
            this.couping_od.Name = "couping_od";
            // 
            // couping_length
            // 
            this.couping_length.DataPropertyName = "couping_length";
            this.couping_length.HeaderText = "接箍长度";
            this.couping_length.Name = "couping_length";
            // 
            // thread_tooth_angle
            // 
            this.thread_tooth_angle.DataPropertyName = "thread_tooth_angle";
            this.thread_tooth_angle.HeaderText = "牙型角度";
            this.thread_tooth_angle.Name = "thread_tooth_angle";
            // 
            // thread_throug_hole_size
            // 
            this.thread_throug_hole_size.DataPropertyName = "thread_throug_hole_size";
            this.thread_throug_hole_size.HeaderText = "镗孔尺寸";
            this.thread_throug_hole_size.Name = "thread_throug_hole_size";
            // 
            // video_no
            // 
            this.video_no.DataPropertyName = "video_no";
            this.video_no.HeaderText = "视频编号";
            this.video_no.Name = "video_no";
            // 
            // tool_measuring_record_no
            // 
            this.tool_measuring_record_no.DataPropertyName = "tool_measuring_record_no";
            this.tool_measuring_record_no.HeaderText = "螺纹测量工具编号";
            this.tool_measuring_record_no.Name = "tool_measuring_record_no";
            // 
            // inspection_result
            // 
            this.inspection_result.DataPropertyName = "inspection_result";
            this.inspection_result.HeaderText = "检验结果";
            this.inspection_result.Name = "inspection_result";
            // 
            // inspection_time
            // 
            this.inspection_time.DataPropertyName = "inspection_time";
            this.inspection_time.HeaderText = "检验时间";
            this.inspection_time.Name = "inspection_time";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.dtpEndTime);
            this.panel2.Controls.Add(this.dtpBeginTime);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.textBox1);
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
            // dtpEndTime
            // 
            this.dtpEndTime.CustomFormat = "yyyy-MM-dd";
            this.dtpEndTime.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(610, 23);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(110, 29);
            this.dtpEndTime.TabIndex = 8;
            this.dtpEndTime.Value = new System.DateTime(2018, 3, 30, 0, 0, 0, 0);
            // 
            // dtpBeginTime
            // 
            this.dtpBeginTime.CustomFormat = "yyyy-MM-dd";
            this.dtpBeginTime.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpBeginTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBeginTime.Location = new System.Drawing.Point(411, 22);
            this.dtpBeginTime.Name = "dtpBeginTime";
            this.dtpBeginTime.Size = new System.Drawing.Size(118, 29);
            this.dtpBeginTime.TabIndex = 7;
            this.dtpBeginTime.Value = new System.DateTime(2018, 3, 14, 0, 0, 0, 0);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(250, 21);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(73, 29);
            this.textBox2.TabIndex = 6;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(88, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(78, 29);
            this.textBox1.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearch.Location = new System.Drawing.Point(734, 12);
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
            this.label4.Location = new System.Drawing.Point(527, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 54);
            this.label4.TabIndex = 3;
            this.label4.Text = "结束时间";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(319, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 54);
            this.label3.TabIndex = 2;
            this.label3.Text = "开始时间";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(159, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 54);
            this.label2.TabIndex = 1;
            this.label2.Text = "操作工工号";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 54);
            this.label1.TabIndex = 0;
            this.label1.Text = "接箍编号";
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
            this.新建ToolStripMenuItem.Size = new System.Drawing.Size(172, 62);
            this.新建ToolStripMenuItem.Text = "新建";
            this.新建ToolStripMenuItem.Click += new System.EventHandler(this.新建ToolStripMenuItem_Click);
            // 
            // 未提交ToolStripMenuItem
            // 
            this.未提交ToolStripMenuItem.Name = "未提交ToolStripMenuItem";
            this.未提交ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.未提交ToolStripMenuItem.Size = new System.Drawing.Size(172, 62);
            this.未提交ToolStripMenuItem.Text = "未提交";
            this.未提交ToolStripMenuItem.Click += new System.EventHandler(this.未提交ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.读码器设置ToolStripMenuItem,
            this.录像设置ToolStripMenuItem,
            this.fTP设置ToolStripMenuItem});
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(873, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 38);
            this.button1.TabIndex = 9;
            this.button1.Text = "详细";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.DateTimePicker dtpBeginTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn couping_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn process_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn operator_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn visual_inspection;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_tooth_pitch_diameter_max;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_tooth_pitch_diameter_avg;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_tooth_pitch_diameter_min;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_sealing_surface_diameter_max;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_sealing_surface_diameter_avg;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_sealing_surface_diameter_min;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_sealing_surface_ovality;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_width;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_pitch;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_taper;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_height;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_length_min;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_bearing_surface_width;
        private System.Windows.Forms.DataGridViewTextBoxColumn couping_inner_end_depth;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_hole_inner_diameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn couping_od;
        private System.Windows.Forms.DataGridViewTextBoxColumn couping_length;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_tooth_angle;
        private System.Windows.Forms.DataGridViewTextBoxColumn thread_throug_hole_size;
        private System.Windows.Forms.DataGridViewTextBoxColumn video_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn tool_measuring_record_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_result;
        private System.Windows.Forms.DataGridViewTextBoxColumn inspection_time;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 新建ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 未提交ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 读码器设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 录像设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fTP设置ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}