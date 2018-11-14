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
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class IndexWindow : Form
    {
        #region 全局变量
        //定义上传视频的线程
        private static Thread thread = null;
        //定义主窗体实例变量
        private static IndexWindow myForm = null; 
        #endregion

        #region 获取主窗体实例
        public static IndexWindow getForm()
        {
            if (myForm == null)
            {
                new IndexWindow();
            }

            return myForm;
        } 
        #endregion

        #region 构造函数
        private IndexWindow()
        {
            InitializeComponent();
            try
            {
                //获取模糊查询所需要的参数
                getSearchParam();
                //获取螺纹检验记录
                getThreadingProcessData();
                //设置datagridView字体
                this.dataGridView1.RowsDefaultCellStyle.Font = new Font("宋体", 18, FontStyle.Bold);
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font("宋体", 18, FontStyle.Bold);
                //设置datagridView列和头部样式
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                {
                    col.HeaderCell.Style = style;
                }
                this.dataGridView1.EnableHeadersVisualStyles = false;
                myForm = this;
                //开启上传视频线程
                thread = new Thread(UploadVideo);
                thread.Start();
                thread.IsBackground = true;
            }
            catch (Exception e)
            {
                //thread.Abort();
            }
            finally {
                try
                {
                    //初始化录像窗体
                    MainWindow.getForm();
                    //开启录像机
                    if (MainWindow.recordStatus == 0)
                        MainWindow.getForm().btnLogin_Click(null, null);
                    if (MainWindow.recordStatus == 1)
                        MainWindow.getForm().btnPreview_Click_1(null, null);
                    //初始化读码器窗体
                    YYKeyenceReaderConsole.getForm();
                    //连接读码器
                    if (YYKeyenceReaderConsole.readerStatus == 0)
                        YYKeyenceReaderConsole.codeReaderConnect();
                    YYKeyenceReaderConsole.getForm().Show();
                    YYKeyenceReaderConsole.getForm().Hide();
                    //开启读码器,此处开启无法读码
                    //if (YYKeyenceReaderConsole.readerStatus == 1)
                    //    YYKeyenceReaderConsole.codeReaderLon();
                    //初始化螺纹检验表单窗体
                    ThreadingForm.getMyForm();
                }
                catch (Exception ex) {
                    MessagePrompt.Show("初始化窗体异常,异常信息:"+ex.Message);
                }
            }
        } 
        #endregion

        #region 获取检验记录的查询条件参数
        private void getSearchParam()
        {
            try
            {
                JObject json = new JObject { };
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content =null;
                byte[] data = encoding.GetBytes(json.ToString());
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
                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
                if (content != null)
                {
                    JObject jobject = JObject.Parse(content);
                    string rowsJson = jobject["rowsData"].ToString();
                    //将获取到的json格式的合同信息转成List对象
                    List<ContractInfo> list = JsonConvert.DeserializeObject<List<ContractInfo>>(rowsJson);
                    //添加空白选项
                    this.cmbContractNo.Items.Add("");
                    this.cmbOd.Items.Add("");
                    this.cmbWt.Items.Add("");
                    this.cmbThreadingType.Items.Add("");
                    this.cmbPipeHeatNo.Items.Add("");
                    this.cmbPipeLotNo.Items.Add("");
                    //遍历合同信息集合
                    foreach (ContractInfo item in list)
                    {
                        //如果合同编号不为空
                        if (!string.IsNullOrWhiteSpace(item.Contract_no))
                        {
                            //如果合同编号下拉框没有当前合同编号
                            if (!cmbContractNo.Items.Contains(item.Contract_no))
                            {
                                //添加当前合同编号到合同编号下拉框中
                                this.cmbContractNo.Items.Add(item.Contract_no);
                            }
                        }
                        //追加外径到外径下拉框中
                        if (!string.IsNullOrWhiteSpace(item.Od)) {
                            if (!this.cmbOd.Items.Contains(item.Od))
                                this.cmbOd.Items.Add(item.Od);
                        }
                        //追加壁厚到壁厚下拉框中
                        if (!string.IsNullOrWhiteSpace(item.Wt)) {
                            if (!this.cmbWt.Items.Contains(item.Wt))
                                this.cmbWt.Items.Add(item.Wt);
                        }
                        //追加螺纹类型到螺纹类型下拉框中
                        if (!string.IsNullOrWhiteSpace(item.Threading_type)) {
                            if (!this.cmbThreadingType.Items.Contains(item.Threading_type))
                                this.cmbThreadingType.Items.Add(item.Threading_type);
                        }
                        //追加炉号到炉号下拉框中
                        if (!string.IsNullOrWhiteSpace(item.Pipe_heat_no)) {
                            if (!this.cmbPipeHeatNo.Items.Contains(item.Pipe_heat_no))
                                this.cmbPipeHeatNo.Items.Add(item.Pipe_heat_no);
                        }
                        //追加试批号到试批号下拉框中
                        if (!string.IsNullOrWhiteSpace(item.Pipe_lot_no))
                        {
                            if (!this.cmbPipeLotNo.Items.Contains(item.Pipe_lot_no))
                                this.cmbPipeLotNo.Items.Add(item.Pipe_lot_no);
                        }
                    }
                    //设置主窗体搜索参数下拉框默认选择第一条空白选项
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
                //MessagePrompt.Show("获取查询条件时出错,错误原因:" + e.Message);
            }
        }
        #endregion

        #region 分页查询获取数据

        public void getThreadingProcessData()
        {
            try
            {
                //封装查询螺纹检验记录的参数(参数依次是:操作工工号、班别、班次、合同号、接箍类型、外径、壁厚、炉号、试批号、开始日期、结束日期)
                JObject jsonData = new JObject
                {
                    {"operator_no",this.txtOperatorno.Text.Trim() },
                    { "production_crew",this.cmbProductionCrew.Text.Trim()},
                    { "production_shift",this.cmbProductionShift.Text.Trim()},
                    { "contract_no",this.cmbContractNo.Text.Trim()},
                    { "threading_type",this.cmbThreadingType.Text.Trim()},
                    { "od",this.cmbOd.Text.Trim()},
                    { "wt",this.cmbWt.Text.Trim()},
                    { "pipe_heat_no",this.cmbPipeHeatNo.Text.Trim()},
                    { "pipe_lot_no",this.cmbPipeLotNo.Text.Trim()},
                    { "beginTime",this.dateTimePicker1.Value.ToString("yyyy-MM-dd")},
                    { "endTime",this.dateTimePicker2.Value.ToString("yyyy-MM-dd")}
                };
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content =null;
                byte[] data = encoding.GetBytes(jsonData.ToString());
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
                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
                if (content != null)
                {
                    JObject jobject = JObject.Parse(content);
                    string rowsJson = jobject["rowsData"].ToString();
                    if (!rowsJson.Trim().Contains("{}"))
                    {
                        //将搜索到json格式的螺纹检验数据转换成对象
                        List<ThreadInspectionRecord> list = JsonConvert.DeserializeObject<List<ThreadInspectionRecord>>(rowsJson);
                        foreach (ThreadInspectionRecord item in list)
                        {
                            //此时item.Inspection_time格式如：1531909218000,需转换成用户可读的时间类型
                            item.Inspection_time = CommonUtil.ConvertTimeStamp(item.Inspection_time);
                        }
                        //设置dataGridView数据源为list集合
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
            try
            {
                //打开螺纹检验窗体
                ThreadingForm.getMyForm().Show();            
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("新建表单时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 菜单栏未提交事件
        private void 未提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //打开未提交表单窗体
               new UnSubmitForm().Show();
            }
            catch (Exception ex) {
                MessagePrompt.Show("查看未提交表单时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 菜单栏读码器设置事件
        private void 读码器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //打开读码器设置窗体
                YYKeyenceReaderConsole.getForm().Show();
            }
            catch (Exception ex) {
                MessagePrompt.Show("打开读码器设置页面时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 菜单栏录像设置事件
        private void 录像设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //打开录像设置窗体
                MainWindow.getForm().Show();
                MainWindow.isRecordClick = true;
            }
            catch (Exception ex) {
                MessagePrompt.Show("打开录像设置页面时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 主页关闭事件
        private void IndexWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DateTime start = DateTime.Now;
            DateTime now = DateTime.Now;
            TimeSpan ts = now - start;
            while (true)
            {
                ts = now - start;
                //1秒后
                if (ts.TotalSeconds > 1)
                {
                    if (YYKeyenceReaderConsole.getForm() != null)
                    {
                        //关闭读码器
                        YYKeyenceReaderConsole.codeReaderOff();
                    }
                    break;
                }
                else
                {
                    now = DateTime.Now;
                }
            }
            if (MainWindow.getForm() != null)
            {
                //关闭录像机
                MainWindow.stopRecordVideo();
            }
        }
        #endregion

        #region 服务器设置事件
        private void 服务器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new ServerSetting().Show();
            }
            catch (Exception ex) {
                MessagePrompt.Show("打开服务器设置页面时出错,错误信息:" + ex.Message);
            }
            
        } 
        #endregion

        #region 螺纹检验记录修改事件
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                //当前dataGridView(数据显示控件)选中行的索引
                int index = this.dataGridView1.CurrentRow.Index;
                string operator_no = "",thread_inspection_record_code = "",coupling_heat_no="", coupling_lot_no="", production_line="", machine_no="", coupling_no="",
                    production_crew="", production_shift="", contract_no="", inspection_result="",videoNo="",inspection_time="";
                //获取选中行的数据
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
                object obj11 = this.dataGridView1.Rows[index].Cells["video_no"].Value;
                object obj12 = this.dataGridView1.Rows[index].Cells["inspection_time"].Value;
                object obj13= this.dataGridView1.Rows[index].Cells["inspection_result"].Value;
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
                if (obj11 != null)
                    videoNo = Convert.ToString(obj11);
                if (obj12 != null)
                    inspection_time = Convert.ToString(obj12);
                if (obj13 != null)
                    inspection_result = Convert.ToString(obj13);
                //将选中行的数据填充到DetailForm中对应的控件上
                DetailForm form = new DetailForm(operator_no, thread_inspection_record_code);
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
                form.tbContractNo.Text =contract_no;
                form.videoNoArr = videoNo;
                form.lblresult1.Text = inspection_result;
                form.lblresult2.Text = inspection_result;
                form.lblInspectionTime.Text = inspection_time;
                form.video_url = videoNo;
                form.Show();
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("打开检验记录时出错,错误信息:"+ex.Message);
            }

        }
        #endregion

        #region 螺纹检验记录删除事件
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除吗?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    int index = this.dataGridView1.CurrentRow.Index;
                    object obj0 = this.dataGridView1.Rows[index].Cells["operator_no"].Value;
                    object obj1 = this.dataGridView1.Rows[index].Cells["inspection_time"].Value;
                    string operator_no = null, inspection_time = null;
                    //获取选中行的记录，并通过选中行记录获取此条记录提交的时间和提交此条记录的操作工工号
                    if (obj0 != null)
                        operator_no = obj0.ToString();
                    if (obj1 != null)
                        inspection_time = obj1.ToString();
                    //如果没有查到提交该记录的操作工工号或提交时间
                    if (string.IsNullOrWhiteSpace(operator_no)|| string.IsNullOrWhiteSpace(inspection_time)) {
                        MessagePrompt.Show("系统繁忙,请稍后修改!");
                        return;
                    }
                    DateTime dt = DateTime.Parse(inspection_time);
                    //判断当前时间与螺纹检验记录提交时间间隔
                    if (Convert.ToSingle((DateTime.Now - dt).TotalHours.ToString()) > 12) {
                        MessagePrompt.Show("已超过12小时的记录不能删除!");
                        return;
                    }
                    //判断用户是否已经掉线
                    if (string.IsNullOrWhiteSpace(Person.pname) || string.IsNullOrWhiteSpace(Person.employee_no))
                    {
                        MessagePrompt.Show("您已掉线，请重新登录!");
                        Application.Exit();
                        return;
                    }
                    //判断删除的是否时自己提交的
                    if (!operator_no.Equals(Person.employee_no)) {
                        MessagePrompt.Show("只能删除自己的表单数据!");
                        return;
                    }
                    //删除检验记录
                    excuteDelteInspectionRecord(index);
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("删除失败,失败原因:" + ex.Message);
                }
            }
        }
        #endregion

        #region 螺纹检验记录删除
        public void excuteDelteInspectionRecord(int index)
        {
            try
            {
                //获取删除选中行的检验记录编号
                object obj1 = this.dataGridView1.Rows[index].Cells["thread_inspection_record_code"].Value;
                if (obj1 == null) {
                    MessagePrompt.Show("删除失败!");
                    return;
                }
                string thread_inspection_record_code = obj1.ToString();
                JObject jsonData = new JObject
                         {
                            {"thread_inspection_record_code",thread_inspection_record_code }
                        };
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                byte[] data = encoding.GetBytes(jsonData.ToString());
                string url = CommonUtil.getServerIpAndPort() + "ThreadingOperation/delThreadInspectionRecordOfWinform.action";
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
                if (jsons == null) {
                    MessagePrompt.Show("删除失败!");
                    return;
                }
                JObject jobject = JObject.Parse(jsons);
                string rowsJson = jobject["rowsData"].ToString();
                if (rowsJson.Trim().Contains("{}")) {
                    MessagePrompt.Show("删除失败!");
                    return;
                }
                if (rowsJson.Contains("True"))
                {
                    MessagePrompt.Show("删除成功!");
                    //刷新检验记录列表(dataGridView)
                    getThreadingProcessData();
                }
                else
                {
                    MessagePrompt.Show("删除失败!");
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("删除失败,失败原因:" + ex.Message);
            }
        }
        #endregion

        #region 退出主页面返回到登录页面
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginWinform.getForm().Show();
        }
        #endregion

        #region 下拉框绘制(主要用于绘制下拉框样式和字体大小)
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

        #region  窗体显示状态改变事件
        private void IndexWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Person.pname))
                this.lblIndexFormTitle.Text = "现在登录的是:" + Person.pname + ",工号:" + Person.employee_no;
        }
        #endregion

        #region 多线程上传视频
        private static void UploadVideo()
        {
            try
            {
                //获取程序中的根路径(bin目录,basePath格式如:C:\Workspace\YYOPInspectionClient\YYOPInspectionClient\bin\)
                string basePath = Application.StartupPath + "\\";
                //ffmpeg.exe用于转换视频格式
                string ffmpegPath = basePath + "ffmpeg.exe";
                //bin目录下的done文件下的视频文件为可上传视频文件
                string donePath = basePath + "done";
                //先判断done目录是否存在,不存在则创建文件
                if (!Directory.Exists(donePath))
                {
                    Directory.CreateDirectory(donePath);
                }
                //定义目录对象
                DirectoryInfo folder = new DirectoryInfo(donePath);
                FileInfo[] files = null;
                while (true)
                {
                    //每执行一次循环，线程休眠5秒中
                    Thread.Sleep(5000);
                    try
                    {
                        //获取done目录下所有以.mp4结尾的视频文件
                        files = folder.GetFiles("*.mp4");
                        //如果done目录下有视频文件
                        if (files.Length > 0)
                        {
                            //遍历done目录下所有的视频文件
                            foreach (FileInfo file in files)
                            {
                                //判断当前视频文件是否在另一个线程中在占用(因为有可能视频格式刚转化完，正由其他目录转到done目录中)
                                if (!CommonUtil.FileIsUsed(file.FullName))
                                {
                                    //如果文件大小大于0，则开始编码然后上传到服务器,否则删除文件
                                    if (file.Length > 0)
                                    {
                                        //判断当前视频是否已经转化格式
                                        if (!file.Name.Contains("vcr"))
                                        {
                                            //开始转换视频格式
                                            CommonUtil.FormatAndUploadVideo(ffmpegPath, donePath, file.FullName);
                                        }
                                        else
                                        {
                                            //开始上传转换后的视频文件()
                                            if (CommonUtil.uploadVideoToTomcat(file.FullName))
                                            {
                                                //上传完成删除本地视频文件
                                                File.Delete(file.FullName);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //如果文件大小为空可能为垃圾文件
                                        File.Delete(file.FullName);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exx) { }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("上传视频出错,错误信息:"+e.Message);
            }

        }
        #endregion

    }
}
