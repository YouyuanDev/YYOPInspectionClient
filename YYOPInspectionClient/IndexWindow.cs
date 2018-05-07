using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
   
    public partial class IndexWindow : Form
    {
        private static Thread thread = null;
        private YYKeyenceReaderConsole readerCodeWindow = null;
        private MainWindow videoWindow=null;
        private ThreadingForm threadFrom = null;
       
        public IndexWindow()
        {
            InitializeComponent();
            this.Font = new Font("宋体", 12, FontStyle.Bold);
            AutoSize autoSize= new AutoSize();
            autoSize.controllInitializeSize(this);
            getSearchParam();
            getThreadingProcessData();
            this.dataGridView1.RowsDefaultCellStyle.Font = new Font("宋体",18, FontStyle.Bold);
            try
            {
                //thread = new Thread(UploadVideo);
                //thread.Start();
                //thread.IsBackground = true;
            }
            catch (Exception e)
            {
                thread.Abort();
            }
        }

        #region 获取检验记录的查询条件参数
        private void getSearchParam()
        {
            
                try
                {
                    string od = this.cmbOd.Text;
                    string wt = this.cmbWt.Text;
                    string thread_type = this.cmbThreadType.Text;
                    string acceptance_no = this.cmbAcceptanceNo.Text;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"od\"" + ":" + "\"" + od + "\",");
                    sb.Append("\"wt\"" + ":" + "\"" + wt + "\",");
                    sb.Append("\"thread_type\"" + ":" + "\"" + thread_type + "\",");
                    sb.Append("\"acceptance_no\"" + ":" + "\"" + acceptance_no + "\"");
                    sb.Append("}");
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    String content = "";
                    JObject o = JObject.Parse(sb.ToString());
                    String param = o.ToString();
                    byte[] data = encoding.GetBytes(param);
                    string url = CommonUtil.getServerIpAndPort() + "Contract/getAllContractNoOfWinform.action";
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.KeepAlive = false;
                    request.Method = "POST";
                    request.ContentType = "application/json;characterSet:UTF-8";
                    request.ContentLength = data.Length;
                    using (Stream sm = request.GetRequestStream())
                    {
                        sm.Write(data, 0, data.Length);
                    }
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream streamResponse = response.GetResponseStream();
                    StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                    Char[] readBuff = new Char[1024];
                    int count = streamRead.Read(readBuff, 0, 1024);
                    while (count > 0)
                    {
                        String outputData = new String(readBuff, 0, count);
                        content += outputData;
                        count = streamRead.Read(readBuff, 0, 1024);
                    }
                    response.Close();
                    string jsons = content;
                    if (jsons != null)
                    {
                        JObject jobject = JObject.Parse(jsons);
                        string rowsJson = jobject["rowsData"].ToString();
                        List<ContractInfo> list = JsonConvert.DeserializeObject<List<ContractInfo>>(rowsJson);
                        List<ComboxItem> odList = new List<ComboxItem>();
                        List<ComboxItem> wtList = new List<ComboxItem>();
                        List<ComboxItem> threadTypeList = new List<ComboxItem>();
                        List<ComboxItem> acceptanceNoList = new List<ComboxItem>();
                        odList.Add(new ComboxItem() { Id="",Text=""});
                        wtList.Add(new ComboxItem() { Id = "", Text = "" });
                        threadTypeList.Add(new ComboxItem() { Id = "", Text = "" });
                       acceptanceNoList.Add(new ComboxItem() { Id = "", Text = "" });
                    foreach (ContractInfo item in list)
                        {
                            if(!string.IsNullOrWhiteSpace(item.Od))
                                odList.Add(new ComboxItem() { Id = item.Od, Text =item.Od});
                            if(!string.IsNullOrWhiteSpace(item.Wt))
                                wtList.Add(new ComboxItem() { Id =item.Wt, Text = item.Wt });
                            if(!string.IsNullOrWhiteSpace(item.Threading_type))
                                threadTypeList.Add(new ComboxItem() { Id = item.Threading_type, Text = item.Threading_type });
                            if(!string.IsNullOrWhiteSpace(item.Thread_acceptance_criteria_no))
                                acceptanceNoList.Add(new ComboxItem() { Id = item.Thread_acceptance_criteria_no, Text = item.Thread_acceptance_criteria_no });
                        }
                        if (odList.Count > 0)
                        {
                            this.cmbOd.DataSource = odList;
                            this.cmbOd.ValueMember = "id";
                            this.cmbOd.DisplayMember = "text";
                            this.cmbOd.SelectedIndex = 0;
                        }
                        if (wtList.Count > 0)
                        {
                            this.cmbWt.DataSource = wtList;
                            this.cmbWt.ValueMember = "id";
                            this.cmbWt.DisplayMember = "text";
                            this.cmbWt.SelectedIndex = 0;
                        }
                        if (threadTypeList.Count > 0)
                        {
                            this.cmbThreadType.DataSource = threadTypeList;
                            this.cmbThreadType.ValueMember = "id";
                            this.cmbThreadType.DisplayMember = "text";
                            this.cmbThreadType.SelectedIndex = 0;
                        }
                        if (acceptanceNoList.Count > 0)
                        {
                            this.cmbAcceptanceNo.DataSource = acceptanceNoList;
                            this.cmbAcceptanceNo.ValueMember = "id";
                            this.cmbAcceptanceNo.DisplayMember = "text";
                            this.cmbAcceptanceNo.SelectedIndex = 0;
                        }
                    }
            }
            catch (Exception e) {
                Console.WriteLine("获取查询条件时失败......");
            }
        }
        #endregion

        #region 多线程上传视频
        private static void UploadVideo()
        {
            try
            {
                string baPath= Application.StartupPath + "\\";
                string ffmpegPath = baPath+ "ffmpeg.exe";
                string path = Application.StartupPath + "\\draft";
                while (true)
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo folder = new DirectoryInfo(path);
                        foreach (DirectoryInfo sonFolder in folder.GetDirectories())
                        {
                            FileInfo[] files = sonFolder.GetFiles("*.mp4");
                            if (files.Length > 0)
                            {
                                foreach (FileInfo file in files)
                                {
                                    if (file.Length > 0)
                                    {
                                        if (!CommonUtil.JudgeFileIsUsing(file.FullName))
                                        {
                                            //视频格式转换，上传
                                            CommonUtil.FormatVideo(ffmpegPath, sonFolder.FullName, file.FullName);
                                        }
                                    }
                                    else {
                                        File.Delete(file.FullName);
                                    }
                                    
                                }

                            }
                            else {
                                Directory.Delete(sonFolder.FullName, true);
                            }
                               
                        }
                    }
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                thread.Abort();
            }

        } 
        #endregion

        #region 分页查询获取数据

        public void getThreadingProcessData()
        {
            try
            {
                string od = this.cmbOd.SelectedValue.ToString();
                string wt = this.cmbWt.SelectedValue.ToString();
                string thread_type = this.cmbThreadType.SelectedValue.ToString();
                string acceptance_no = this.cmbAcceptanceNo.SelectedValue.ToString();
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"od\"" + ":" + "\"" + od + "\",");
                sb.Append("\"wt\"" + ":" + "\"" + wt + "\",");
                sb.Append("\"thread_type\"" + ":" + "\"" + thread_type + "\",");
                sb.Append("\"acceptance_no\"" + ":" + "\"" + acceptance_no + "\"");
                sb.Append("}");
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject o = JObject.Parse(sb.ToString());
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
                string url = CommonUtil.getServerIpAndPort() + "ThreadingOperation/getThreadInspectionRecordOfWinform.action";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json;characterSet:UTF-8";
                request.ContentLength = data.Length;
                using (Stream sm = request.GetRequestStream()) {
                    sm.Write(data, 0, data.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                Char[] readBuff = new Char[1024];
                int count = streamRead.Read(readBuff, 0, 1024);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    content += outputData;
                    count = streamRead.Read(readBuff, 0, 1024);
                }
                response.Close();
                string jsons = content;
                if (jsons != null) {
                    JObject jobject = JObject.Parse(jsons);
                    string rowsJson = jobject["rowsData"].ToString();
                    if (!rowsJson.Trim().Equals("{}"))
                    {
                        List<ThreadInspectionRecord> list = JsonConvert.DeserializeObject<List<ThreadInspectionRecord>>(rowsJson);
                        foreach (ThreadInspectionRecord item in list) {
                            item.Inspection_time = CommonUtil.ConvertTimeStamp(item.Inspection_time);
                        }
                        this.dataGridView1.DataSource = list;
                    }
                    else {
                        this.dataGridView1.DataSource = null;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("获取检验记录时失败......");
            }
           
        }
        #endregion

        #region 点击搜索事件
        private void btnSearch_Click(object sender, EventArgs e)
        {
            getThreadingProcessData();
        }
        #endregion

        #region 菜单栏新建表单事件
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            //ThreadingProcessForm form = new ThreadingProcessForm(this, mainWindow);
            if (threadFrom != null)
            {
                threadFrom.Show();
            }
            else
            {
                threadFrom = new ThreadingForm(this, mainWindow);
                threadFrom.Show();
            }
            //form.Show();
        }
        #endregion

        #region 菜单栏未提交事件
        private void 未提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnSubmitForm form = new UnSubmitForm();
            form.Show();
        }
        #endregion

        #region 菜单栏读码器设置事件
        private void 读码器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (readerCodeWindow == null)
            {
                readerCodeWindow = new YYKeyenceReaderConsole();
            }
            readerCodeWindow.Show();

        }
        #endregion

        #region 菜单栏录像设置事件
        private void 录像设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videoWindow == null)
            {
                videoWindow = new MainWindow();
            }
            videoWindow.Show();
        }
        #endregion

        #region 菜单栏FTP设置事件
        private void fTP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPSetting setting = new FTPSetting();
            setting.Show();
        } 
        #endregion

        private void 开启读码器ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关闭读码器ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void IndexWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 判断读码器和录像机是否关闭
            DateTime start = DateTime.Now;
            DateTime now = DateTime.Now;
            TimeSpan ts = now - start;
            while (true)
            {
                ts = now - start;
                if (ts.TotalSeconds > 1)
                {
                    if (YYKeyenceReaderConsole.myselfForm != null) {
                        YYKeyenceReaderConsole.codeReaderOff();
                    }
                    break;
                }
                else
                {
                    now = DateTime.Now;
                }
            }
            if (MainWindow.mainWindowForm != null) {
                MainWindow.stopRecordVideo();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataShowForm form = new DataShowForm();
                form.Show();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Selected == true)
                    {
                        if (dataGridView1.Rows[i].Cells["thread_pitch_gauge_no"].Value != null) {
                            form.textBox1.Text = dataGridView1.Rows[i].Cells["thread_pitch_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_pitch_calibration_framwork"].Value != null) {
                            form.textBox2.Text =dataGridView1.Rows[i].Cells["thread_pitch_calibration_framwork"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["sealing_surface_gauge_no"].Value != null) {
                            form.textBox3.Text = dataGridView1.Rows[i].Cells["sealing_surface_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["sealing_surface_calibration_ring_no"].Value != null)
                        {
                            form.textBox4.Text = dataGridView1.Rows[i].Cells["sealing_surface_calibration_ring_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["depth_caliper_no"].Value != null)
                        {
                            form.textBox5.Text = dataGridView1.Rows[i].Cells["depth_caliper_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["threading_distance_gauge_no"].Value != null)
                        {
                            form.textBox6.Text = dataGridView1.Rows[i].Cells["threading_distance_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_distance_calibration_sample_no"].Value != null)
                        {
                            form.textBox7.Text = dataGridView1.Rows[i].Cells["thread_distance_calibration_sample_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["taper_gauge_no"].Value != null)
                        {
                            form.textBox8.Text = dataGridView1.Rows[i].Cells["taper_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["tooth_height_gauge_no"].Value != null)
                        {
                            form.textBox9.Text = dataGridView1.Rows[i].Cells["tooth_height_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["tooth_height_calibration_sample_no"].Value != null)
                        {
                            form.textBox10.Text = dataGridView1.Rows[i].Cells["tooth_height_calibration_sample_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["tooth_width_stop_gauge_no"].Value != null)
                        {
                            form.textBox11.Text = dataGridView1.Rows[i].Cells["tooth_width_stop_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_min_length_sample_no"].Value != null) {
                            form.textBox12.Text = dataGridView1.Rows[i].Cells["thread_min_length_sample_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["coupling_length_sample_no"].Value != null)
                        {
                            form.textBox13.Text = dataGridView1.Rows[i].Cells["coupling_length_sample_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["caliper_no"].Value != null)
                        {
                            form.textBox14.Text = dataGridView1.Rows[i].Cells["caliper_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["caliper_tolerance"].Value != null)
                        {
                            form.textBox15.Text = dataGridView1.Rows[i].Cells["caliper_tolerance"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["collar_gauge_no"].Value != null)
                        {
                            form.textBox16.Text = dataGridView1.Rows[i].Cells["collar_gauge_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["couping_no"].Value != null)
                        {
                            form.textBox17.Text = dataGridView1.Rows[i].Cells["couping_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["id"].Value != null)
                        {
                            form.textBox18.Text = dataGridView1.Rows[i].Cells["id"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["operator_no"].Value != null)
                        {
                            form.textBox19.Text = dataGridView1.Rows[i].Cells["operator_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["process_no"].Value != null)
                        {
                            form.textBox20.Text = dataGridView1.Rows[i].Cells["process_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["visual_inspection"].Value != null) {
                            form.textBox21.Text = dataGridView1.Rows[i].Cells["visual_inspection"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_max"].Value != null)
                        {
                            form.textBox22.Text = dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_max"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_avg"].Value != null)
                        {
                            form.textBox23.Text = dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_avg"].Value.ToString();

                        }
                        if (dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_min"].Value != null)
                        {
                            form.textBox24.Text = dataGridView1.Rows[i].Cells["thread_tooth_pitch_diameter_min"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_max"].Value != null)
                        {
                            form.textBox25.Text = dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_max"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_avg"].Value != null)
                        {
                            form.textBox26.Text = dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_avg"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_min"].Value != null)
                        {
                            form.textBox27.Text = dataGridView1.Rows[i].Cells["thread_sealing_surface_diameter_min"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_sealing_surface_ovality"].Value != null)
                        {
                            form.textBox28.Text = dataGridView1.Rows[i].Cells["thread_sealing_surface_ovality"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_width"].Value != null)
                        {
                            form.textBox29.Text = dataGridView1.Rows[i].Cells["thread_width"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_pitch"].Value != null) {
                            form.textBox30.Text = dataGridView1.Rows[i].Cells["thread_pitch"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_taper"].Value != null)
                        {
                            form.textBox31.Text = dataGridView1.Rows[i].Cells["thread_taper"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_height"].Value != null)
                        {
                            form.textBox32.Text = dataGridView1.Rows[i].Cells["thread_height"].Value.ToString();
                        }

                        if (dataGridView1.Rows[i].Cells["inspection_result"].Value != null)
                        {
                            string res= dataGridView1.Rows[i].Cells["inspection_result"].Value.ToString();
                            int result = Convert.ToInt32(res);
                            if (result == 0) {
                                form.textBox33.Text = "合格";
                            }
                            else if (result == 1) {
                                form.textBox33.Text = "不合格";
                            }
                            else {
                                form.textBox33.Text = "待定";
                            }
                        }
                        else
                        {
                            form.textBox33.Text = "";
                        }
                        if (dataGridView1.Rows[i].Cells["thread_length_min"].Value != null) {
                            form.textBox34.Text = dataGridView1.Rows[i].Cells["thread_length_min"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_bearing_surface_width"].Value != null)
                        {
                            form.textBox35.Text = dataGridView1.Rows[i].Cells["thread_bearing_surface_width"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["couping_inner_end_depth"].Value != null)
                        {
                            form.textBox36.Text = dataGridView1.Rows[i].Cells["couping_inner_end_depth"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_hole_inner_diameter"].Value != null)
                        {
                            form.textBox37.Text = dataGridView1.Rows[i].Cells["thread_hole_inner_diameter"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["couping_od"].Value != null)
                        {
                            form.textBox38.Text = dataGridView1.Rows[i].Cells["couping_od"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["couping_length"].Value != null) {
                            form.textBox39.Text = dataGridView1.Rows[i].Cells["couping_length"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_tooth_angle"].Value != null)
                        {
                            form.textBox40.Text = dataGridView1.Rows[i].Cells["thread_tooth_angle"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_throug_hole_size"].Value != null)
                        {
                            form.textBox41.Text = dataGridView1.Rows[i].Cells["thread_throug_hole_size"].Value.ToString();
                        }


                        //新增
                        if (dataGridView1.Rows[i].Cells["contract_no"].Value != null)
                        {
                            form.textBox48.Text = dataGridView1.Rows[i].Cells["contract_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["heat_no"].Value != null)
                        {
                            form.textBox47.Text = dataGridView1.Rows[i].Cells["heat_no"].Value.ToString();
                        }

                        if (dataGridView1.Rows[i].Cells["test_batch_no"].Value != null)
                        {
                            form.textBox42.Text = dataGridView1.Rows[i].Cells["test_batch_no"].Value.ToString();
                        }

                        if (dataGridView1.Rows[i].Cells["steel_grade"].Value != null)
                        {
                            form.textBox43.Text = dataGridView1.Rows[i].Cells["steel_grade"].Value.ToString();
                        }

                        if (dataGridView1.Rows[i].Cells["texture"].Value != null)
                        {
                            form.textBox44.Text = dataGridView1.Rows[i].Cells["texture"].Value.ToString();
                        }

                        if (dataGridView1.Rows[i].Cells["production_area"].Value != null)
                        {
                            form.textBox45.Text = dataGridView1.Rows[i].Cells["production_area"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["machine_no"].Value != null)
                        {
                            form.textBox46.Text = dataGridView1.Rows[i].Cells["machine_no"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["thread_acceptance_criteria_no"].Value != null)
                        {
                            form.textBox49.Text = dataGridView1.Rows[i].Cells["thread_acceptance_criteria_no"].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex) {
                throw ex;
            }
           
        }

        private void 服务器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ServerSetting().Show();
        }

        #region 详细信息
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.dataGridView1.CurrentRow.Index;
                string inspection_no =Convert.ToString(this.dataGridView1.Rows[index].Cells["thread_inspection_record_code"].Value);
                string operator_no =Convert.ToString(this.dataGridView1.Rows[index].Cells["operator_no"].Value);
                string thread_inspection_record_code = Convert.ToString(this.dataGridView1.Rows[index].Cells["thread_inspection_record_code"].Value);
                DetailForm form = new DetailForm(operator_no,inspection_no, thread_inspection_record_code);
                form.txtProductionArea.Text= Convert.ToString(this.dataGridView1.Rows[index].Cells["production_line"].Value);
                form.txtMachineNo.Text = Convert.ToString(this.dataGridView1.Rows[index].Cells["machine_no"].Value);
                form.txtOperatorNo.Text = operator_no;
                form.txtCoupingNo.Text = Convert.ToString(this.dataGridView1.Rows[index].Cells["couping_no"].Value);
                form.cmbProductionCrew.SelectedIndex = form.cmbProductionCrew.Items.IndexOf(this.dataGridView1.Rows[index].Cells["production_crew"].Value);
                form.cmbProductionShift.SelectedIndex = form.cmbProductionShift.Items.IndexOf(this.dataGridView1.Rows[index].Cells["production_shift"].Value);
                form.cmbContractNo.SelectedValue=Convert.ToString(this.dataGridView1.Rows[index].Cells["contract_no"].Value);
                form.Show();
            }
            catch (Exception ex) {
                Console.WriteLine("获取选中的接箍检验记录编号时出错......");
            }
            
        }
        #endregion
    }
}
