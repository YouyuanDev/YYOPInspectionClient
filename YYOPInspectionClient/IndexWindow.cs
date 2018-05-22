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
        public ThreadingForm threadFrom = null;
        public  LoginWinform loginWinform=null;
        #region 构造函数
        public IndexWindow()
        {
            InitializeComponent();
            getSearchParam();
            getThreadingProcessData();
            try
            {
                //---------------设置datagridView字体(开始)
                this.dataGridView1.RowsDefaultCellStyle.Font = new Font("宋体", 18, FontStyle.Bold);
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font("宋体",18,FontStyle.Bold);
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                {
                    col.HeaderCell.Style = style;
                }
                this.dataGridView1.EnableHeadersVisualStyles = false;
                //---------------设置datagridView字体(结束)
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
                    this.cmbContractNo.Items.Add("");
                    this.cmbOd.Items.Add("");
                    this.cmbWt.Items.Add("");
                    this.cmbThreadingType.Items.Add("");
                    this.cmbPipeHeatNo.Items.Add("");
                    this.cmbPipeLotNo.Items.Add("");
                    foreach (ContractInfo item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Contract_no))
                        {
                            if (!cmbContractNo.Items.Contains(item.Contract_no))
                            {
                                this.cmbContractNo.Items.Add(item.Contract_no);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(item.Od)) {
                            if (!this.cmbOd.Items.Contains(item.Od))
                                this.cmbOd.Items.Add(item.Od);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Wt)) {
                            if (!this.cmbWt.Items.Contains(item.Wt))
                                this.cmbWt.Items.Add(item.Wt);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Threading_type)) {
                            if (!this.cmbThreadingType.Items.Contains(item.Threading_type))
                                this.cmbThreadingType.Items.Add(item.Threading_type);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Pipe_heat_no)) {
                            if (!this.cmbPipeHeatNo.Items.Contains(item.Pipe_heat_no))
                                this.cmbPipeHeatNo.Items.Add(item.Pipe_heat_no);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Pipe_lot_no))
                        {
                            if (!this.cmbPipeLotNo.Items.Contains(item.Pipe_lot_no))
                                this.cmbPipeLotNo.Items.Add(item.Pipe_lot_no);
                        }
                    }
                    this.cmbProductionCrew.SelectedIndex = 0;
                    this.cmbProductionShift.SelectedIndex = 0;
                    this.cmbContractNo.SelectedIndex = 0;
                    this.cmbOd.SelectedIndex = 0;
                    this.cmbWt.SelectedIndex = 0;
                    this.cmbThreadingType.SelectedIndex = 0;
                    this.cmbPipeHeatNo.SelectedIndex = 0;
                    this.cmbPipeLotNo.SelectedIndex = 0;
                    this.cmbPipeHeatNo.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取查询条件时出错,错误原因:"+e.Message);
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
                                    if (!file.Name.Contains("vcr"))
                                    {
                                        CommonUtil.FormatAndUploadVideo(ffmpegPath, donePath, file.FullName);
                                    }
                                    else {
                                        //直接上传
                                        if (CommonUtil.uploadVideoToTomcat(file.FullName)) {
                                            File.Delete(file.FullName);
                                        }
                                       
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
                string operator_no = this.txtOperatorno.Text.Trim();
                string production_crew = this.cmbProductionCrew.Text.Trim();
                string production_shift = this.cmbProductionShift.Text.Trim();
                string contract_no = this.cmbContractNo.Text.Trim();
                string threading_type = this.cmbThreadingType.Text.Trim();
                string od = this.cmbOd.Text.Trim();
                string wt = this.cmbWt.Text.Trim();
                string pipe_heat_no = this.cmbPipeHeatNo.Text.Trim();
                string pipe_lot_no = this.cmbPipeLotNo.Text.Trim();
                string beginTime = this.dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string endTime = this.dateTimePicker2.Value.ToString("yyyy-MM-dd");
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"operator_no\"" + ":" + "\"" + operator_no + "\",");
                sb.Append("\"production_crew\"" + ":" + "\"" + production_crew + "\",");
                sb.Append("\"production_shift\"" + ":" + "\"" + production_shift + "\",");
                sb.Append("\"contract_no\"" + ":" + "\"" + contract_no + "\",");
                sb.Append("\"threading_type\"" + ":" + "\"" + threading_type + "\",");
                sb.Append("\"od\"" + ":" + "\"" + od + "\",");
                sb.Append("\"wt\"" + ":" + "\"" + wt + "\",");
                sb.Append("\"pipe_heat_no\"" + ":" + "\"" + pipe_heat_no + "\",");
                sb.Append("\"pipe_lot_no\"" + ":" + "\"" + pipe_lot_no + "\",");
                sb.Append("\"beginTime\"" + ":" + "\"" + beginTime + "\",");
                sb.Append("\"endTime\"" + ":" + "\"" + endTime + "\"");
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
                    MessageBox.Show(rowsJson);
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
                //throw e;
                Console.WriteLine("获取检验记录时失败......" +e.Message);
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
            //Application.Exit();
            if (loginWinform != null)
            {
                this.Hide();
                loginWinform.Show();
            }
            else {
                MessagePrompt.Show("登出失败,请重启系统!");
                Application.Exit();
            }
        }
        #endregion

        #region 下拉框绘制
        private void cmbOd_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbOd.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbWt_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbWt.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbThreadType_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbThreadingType.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbPipeHeatNo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbPipeHeatNo.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbProductionCrew_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbProductionCrew.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbProductionShift_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbProductionShift.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbContractNo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbContractNo.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }

        private void cmbPipeLotNo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.Graphics.DrawString(cmbPipeLotNo.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
            e.DrawFocusRectangle();
        }
        #endregion

        #region  窗体Visible改变事件
        private void IndexWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Person.pname))
                this.lblIndexFormTitle.Text = "现在登录的是:" + Person.pname + ",工号:" + Person.employee_no;
        }
        #endregion

       
    }
}
