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
        private MainWindow videoWindow = null;
        private ThreadingForm threadFrom = null;

        #region 构造函数
        public IndexWindow()
        {
            InitializeComponent();
            this.Font = new Font("宋体", 12, FontStyle.Bold);
            AutoSize autoSize = new AutoSize();
            autoSize.controllInitializeSize(this);
            getSearchParam();
            getThreadingProcessData();
            this.dataGridView1.RowsDefaultCellStyle.Font = new Font("宋体", 18, FontStyle.Bold);
            try
            {
                this.Text ="现在登录的是:"+Person.pname+",工号:"+Person.employee_no;
                thread = new Thread(UploadVideo);
                thread.Start();
                thread.IsBackground = true;
            }
            catch (Exception e)
            {
                thread.Abort();
            }
        } 
        #endregion

        #region 获取检验记录的查询条件参数
        private void getSearchParam()
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
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
                    odList.Add(new ComboxItem() { Id = "", Text = "" });
                    wtList.Add(new ComboxItem() { Id = "", Text = "" });
                    threadTypeList.Add(new ComboxItem() { Id = "", Text = "" });
                    acceptanceNoList.Add(new ComboxItem() { Id = "", Text = "" });
                    foreach (ContractInfo item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Od))
                            odList.Add(new ComboxItem() { Id = item.Od, Text = item.Od });
                        if (!string.IsNullOrWhiteSpace(item.Wt))
                            wtList.Add(new ComboxItem() { Id = item.Wt, Text = item.Wt });
                        if (!string.IsNullOrWhiteSpace(item.Threading_type))
                            threadTypeList.Add(new ComboxItem() { Id = item.Threading_type, Text = item.Threading_type });
                        if (!string.IsNullOrWhiteSpace(item.Thread_acceptance_criteria_no))
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
            catch (Exception e)
            {
                Console.WriteLine("获取查询条件时失败......");
            }
        }
        #endregion

        #region 多线程上传视频
        private static void UploadVideo()
        {
            try
            {
                string basePath = Application.StartupPath + "\\";
                string ffmpegPath = basePath + "ffmpeg.exe";
                string donePath = basePath + "done";
                //先判断done目录是否存在
                if (!Directory.Exists(donePath))
                {
                    Directory.CreateDirectory(donePath);
                }
                DirectoryInfo folder = new DirectoryInfo(donePath);
                FileInfo[] files = null;
                while (true)
                {
                    files = folder.GetFiles("*.mp4");
                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            //如果文件大小大于0，则开始编码然后上传到服务器
                            if (file.Length > 0)
                            {
                                if (!CommonUtil.JudgeFileIsUsing(file.FullName))
                                {
                                    //视频格式转换，上传
                                    if (!file.Name.Contains("vcr")) {
                                        CommonUtil.FormatAndUploadVideo(ffmpegPath, donePath, file.FullName);
                                    }
                                    Thread.Sleep(5000);
                                }
                            }
                            else
                            {
                                File.Delete(file.FullName);
                            }

                        }
                    }
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
                    //Console.WriteLine(rowsJson);
                    if (!rowsJson.Trim().Equals("{}"))
                    {
                        List<ThreadInspectionRecord> list = JsonConvert.DeserializeObject<List<ThreadInspectionRecord>>(rowsJson);
                        foreach (ThreadInspectionRecord item in list)
                        {
                            item.Inspection_time = CommonUtil.ConvertTimeStamp(item.Inspection_time);
                        }
                        this.dataGridView1.DataSource = list;
                    }
                    else
                    {
                        this.dataGridView1.DataSource = null;
                    }
                }
            }
            catch (Exception e)
            {
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
            threadFrom.threadForm = threadFrom;
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

        #region 主页关闭事件
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
                    if (YYKeyenceReaderConsole.myselfForm != null)
                    {
                        YYKeyenceReaderConsole.codeReaderOff();
                    }
                    break;
                }
                else
                {
                    now = DateTime.Now;
                }
            }
            if (MainWindow.mainWindowForm != null)
            {
                MainWindow.stopRecordVideo();
            }
        }
        #endregion

        #region 服务器设置事件
        private void 服务器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ServerSetting().Show();
        } 
        #endregion

        #region 详细信息
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.dataGridView1.CurrentRow.Index;
                //string inspection_no = Convert.ToString(this.dataGridView1.Rows[index].Cells["thread_inspection_record_code"].Value);

                string operator_no = "",thread_inspection_record_code = "",coupling_heat_no="", coupling_lot_no="", production_line="", machine_no="", coupling_no="",
                    production_crew="", production_shift="", contract_no="", inspection_result="";
                object obj0 = this.dataGridView1.Rows[index].Cells["operator_no"].Value;
                object obj1 = this.dataGridView1.Rows[index].Cells["thread_inspection_record_code"].Value;
                object obj2 = this.dataGridView1.Rows[index].Cells["coupling_heat_no"].Value;
                object obj3 = this.dataGridView1.Rows[index].Cells["coupling_lot_no"].Value;
                object obj4 = this.dataGridView1.Rows[index].Cells["production_line"].Value;
                object obj5 = this.dataGridView1.Rows[index].Cells["machine_no"].Value;
                object obj6 = this.dataGridView1.Rows[index].Cells["coupling_no"].Value;
                object obj7 = this.dataGridView1.Rows[index].Cells["production_crew"].Value;
                object obj8 = this.dataGridView1.Rows[index].Cells["production_shift"].Value;
                object obj9 = this.dataGridView1.Rows[index].Cells["contract_no"].Value; 
                object obj10= this.dataGridView1.Rows[index].Cells["inspection_result"].Value;
                if (obj0!=null)
                    operator_no = Convert.ToString(obj0);
                 if(obj1!=null)
                    thread_inspection_record_code = Convert.ToString(obj1);
                if (obj2 != null)
                    coupling_heat_no = Convert.ToString(obj2);
                if (obj3 != null)
                    coupling_lot_no = Convert.ToString(obj3);
                if (obj4 != null)
                    production_line = Convert.ToString(obj4);
                if (obj5 != null)
                    machine_no = Convert.ToString(obj5);
                if (obj6 != null)
                    coupling_no = Convert.ToString(obj6);
                if (obj7 != null)
                    production_crew = Convert.ToString(obj7);
                if (obj8 != null)
                    production_shift = Convert.ToString(obj8);
                if (obj9 != null)
                    contract_no = Convert.ToString(obj9);
                if (obj10!= null)
                    inspection_result = Convert.ToString(obj10);
                DetailForm form = new DetailForm(operator_no, thread_inspection_record_code);
                form.indexWindow = this;
                form.txtProductionArea.Text =production_line;
                form.txtMachineNo.Text = machine_no;
                form.txtOperatorNo.Text = operator_no;
                form.txtCoupingNo.Text = coupling_heat_no;
                form.txtBatchNo.Text = coupling_lot_no;
                form.txtCoupingNo.Text = coupling_no;
                form.cmbProductionCrew.SelectedIndex = form.cmbProductionCrew.Items.IndexOf(production_crew);
                form.cmbProductionShift.SelectedIndex = form.cmbProductionShift.Items.IndexOf(production_shift);
                form.txtHeatNo.Text = coupling_heat_no;
                form.txtBatchNo.Text = coupling_lot_no;
                form.cmbInspectionResutlt.SelectedIndex = form.cmbInspectionResutlt.Items.IndexOf(inspection_result);
                form.cmbContractNo.SelectedValue =contract_no;
                form.Show();
                form.detailForm = form;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取选中的接箍检验记录编号时出错......");
            }

        }
        #endregion

        #region 退出程序
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        } 
        #endregion
    }
}
