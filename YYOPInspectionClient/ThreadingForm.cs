using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class ThreadingForm : Form
    {
        //防止合同combobox自动执行选中事件
        private bool flag = false;
        public static List<TextBox> flpTabOneTxtList = new List<TextBox>();
        public static List<TextBox> flpTabTwoTxtList = new List<TextBox>();
        private List<Label> flpTabTwoLblList = new List<Label>();
        private List<string> measureItemCodeList = new List<string>();
        //时间戳(视频和form表单保存的目录名)
        private string videosArr = "";
        private string timestamp = null;
        private int countTime = 0;
        private static ThreadingForm myForm = null;
        public System.Timers.Timer timer = null;
        public static bool isQualified =true;
        public delegate void EventHandle(object sender, EventArgs e);
        public static bool isMeasuringToolTabSelected=true;//
        public static string focusTextBoxName = null;
        public static Dictionary<string, TextBox> controlTxtDir = new Dictionary<string, TextBox>();
        public static Dictionary<string, Label> controlLblDir = new Dictionary<string, Label>();
        #region 构造函数
        public static ThreadingForm getMyForm()
        {
            if (myForm == null)
            {
                new ThreadingForm();
            }
            return myForm;
        }

        #endregion
       
        #region 窗体构造函数
        public ThreadingForm()
        {
           
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(Person.pname) && !string.IsNullOrWhiteSpace(Person.employee_no))
                {
                    AlphabetKeyboardForm.getForm().containerControl = this.flpTabOneContent;
                    NumberKeyboardForm.getForm().containerControl = this.flpTabTwoContent;
                    timestamp = CommonUtil.getMesuringRecord();
                    if(YYKeyenceReaderConsole.readerStatus==-1)
                      this.lblReaderStatus.Text = "读码器未连接...";
                    else if(YYKeyenceReaderConsole.readerStatus ==1)
                        this.lblReaderStatus.Text = "读码器已启动...";
                    else if (YYKeyenceReaderConsole.readerStatus ==2)
                        this.lblReaderStatus.Text = "读码器异常...";
                    if(MainWindow.recordStatus==0)
                       this.lblVideoStatus.Text = "录像未登录...";
                    else if (MainWindow.recordStatus == 1)
                        this.lblVideoStatus.Text = "录像已登录...";
                    else if (MainWindow.recordStatus == 2)
                        this.lblVideoStatus.Text = "录像未启动...";
                    else if (MainWindow.recordStatus == 3)
                        this.lblVideoStatus.Text = "录像已启动...";
                    else if (MainWindow.recordStatus == 4)
                        this.lblVideoStatus.Text = "录像中...";
                    else
                        this.lblVideoStatus.Text = "录像异常...";
                    //this.Font = new Font("宋体", 12, FontStyle.Bold);
                    //1------------初始化合同Combobox
                    
                    flag = true;
                    //2------------初始化检验记录表单
                    InitThreadForm();
                    //3------------定时器
                    //System.Timers.Timer t = new System.Timers.Timer();
                    txtProductionArea.MouseDown += new MouseEventHandler(txt_MouseDown);
                    txtMachineNo.MouseDown += new MouseEventHandler(txt_MouseDown);
                    txtCoupingNo.MouseDown += new MouseEventHandler(txt_MouseDown);
                    txtHeatNo.MouseDown += new MouseEventHandler(txt_MouseDown);
                    txtBatchNo.MouseDown += new MouseEventHandler(txt_MouseDown);
                    myForm = this;
                }
                else
                {
                    MessagePrompt.Show("您已掉线,请重新登录!");
                    this.Dispose();
                    Application.Exit();
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine("新建表单时出错!");
            }
           
           
        }
        #endregion

        #region 结束录制视频
        public void EndVideoRecord()
        {
            MainWindow.stopRecordVideo();
            //保存timestamp到fileuploadrecord中
            RestoreSetting();
            this.button2.Text = "开始录制视频";
            this.lblVideoStatus.Text = "录像完成...";
            string sourceFilePath = Application.StartupPath + "\\draft\\" + timestamp + ".mp4";
            string destPath = Application.StartupPath + "\\done\\" + timestamp + ".mp4";
            if (CommonUtil.MoveFile(sourceFilePath, destPath))
            {
                CommonUtil.DeleteFile(sourceFilePath);
            }
            this.lblTimer.Text = "";
        }
        #endregion

        #region 初始化表单合同数组
        public void InitContractList()
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                //JObject o = JObject.Parse(sb.ToString());
                String param = "";
                byte[] data = encoding.GetBytes(param);
                string url = CommonUtil.getServerIpAndPort() + "Contract/getAllDropDownContractNoOfWinform.action";
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
                    List<ComboxItem> list = JsonConvert.DeserializeObject<List<ComboxItem>>(rowsJson);
                    foreach (ComboxItem item in list)
                    {
                        this.cmbContractNo.Items.Add(item.Text);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取下拉合同号时出错......");
            }
        }
        #endregion

        #region 初始化检验记录表单
        public void InitThreadForm()
        {
            if (!string.IsNullOrWhiteSpace( this.cmbContractNo.Text))
            {
                GetThreadFormInitData(this.cmbContractNo.Text);
            }
        }
        #endregion

        #region 根据合同编号初始化检验记录表单
        public void GetThreadFormInitData(string contract_no)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                string json = "";
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"contract_no\"" + ":" + "\"" + contract_no + "\"");
                sb.Append("}");
                json = sb.ToString();
                JObject o = JObject.Parse(json);
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
                string url = CommonUtil.getServerIpAndPort() + "StaticMeasure/getMeasureDataByContractNoOfWinform.action";
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
                    if (rowsJson.Trim().Equals("fail"))
                    {
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        MessagePrompt.Show("初始化表单时出错!");
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(rowsJson);
                        string contractInfo = jo["contractInfo"].ToString();
                        string measureInfo = jo["measureInfo"].ToString();
                        FillFormTitle(contractInfo);//填充表单合同信息
                        JArray measureArr = (JArray)JsonConvert.DeserializeObject(measureInfo);
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        InitMeasureTools(measureArr);
                    }
                }
            }
            catch (Exception e)
            {
                this.flpTabOneContent.Controls.Clear();
                this.flpTabTwoContent.Controls.Clear();
                MessagePrompt.Show("初始化表单出错，错误信息:" + e.Message);
            }
        }
        #endregion

        #region 合同编号选中事件

        private void cmbContractNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag)
            {
                try
                {
                    ClearCntrValue(this);
                    flpTabOneTxtList.Clear();
                    flpTabTwoTxtList.Clear();
                    flpTabTwoLblList.Clear();
                    controlTxtDir.Clear();
                    controlLblDir.Clear();
                    string contract_no = this.cmbContractNo.Text.Trim();
                    GetThreadFormInitData(contract_no);
                    this.lblCountSubmit.Text = "1";
                    ChangeFrequency();
                    GoThroughControls(this.flpTabOneContent, flpTabOneTxtList, null);
                    GoThroughControls(this.flpTabTwoContent, flpTabTwoTxtList, flpTabTwoLblList);
                    AlphabetKeyboardForm.flpTabOneTxtList = flpTabOneTxtList;
                    NumberKeyboardForm.flpTabTwoTxtList = flpTabTwoTxtList;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("查询必填项时失败!");
                }

            }
        }
        #endregion
       
        #region 填充表单合同信息
        private void FillFormTitle(string contractInfo)
        {

            JObject contractObj = (JObject)JsonConvert.DeserializeObject(contractInfo);
            if (!string.IsNullOrWhiteSpace(contractObj["machining_contract_no"].ToString()))
                this.txtMachiningContractNo.Text = contractObj["machining_contract_no"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["threading_type"].ToString()))
            {
                this.txtThreadType.Text = contractObj["threading_type"].ToString();
                this.lblThreadType.Text = "螺纹类型:" + contractObj["threading_type"].ToString();
            }
            else
            {
                this.lblThreadType.Text = "";
            }
            if (!string.IsNullOrWhiteSpace(contractObj["od"].ToString()))
            {
                this.txtOdDiameter.Text = contractObj["od"].ToString();
                this.lblOd.Text = "外径:" + contractObj["od"].ToString();
            }
            else
            {
                this.lblOd.Text = "";
            }
            if (!string.IsNullOrWhiteSpace(contractObj["wt"].ToString()))
            {
                this.txtTreadWt.Text = contractObj["wt"].ToString();
                this.lblWt.Text = "壁厚:" + contractObj["wt"].ToString();
            }
            else
            {
                this.lblWt.Text = "";
            }
            if (!string.IsNullOrWhiteSpace(contractObj["pipe_steel_grade"].ToString()))
                this.lblSteelGrade.Text = "钢级:" + contractObj["wt"].ToString();
            else
                this.lblSteelGrade.Text = "";
            if (!string.IsNullOrWhiteSpace(contractObj["customer_spec"].ToString()))
                this.txtCriteriaNo.Text = contractObj["customer_spec"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["graph_no"].ToString()))
                this.txtDrawingNo.Text = contractObj["graph_no"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["handbook_no"].ToString()))
                this.txtHandbookNo.Text = contractObj["handbook_no"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["seal_sample_graph_no"].ToString()))
                this.txtSealPatternNo.Text = contractObj["seal_sample_graph_no"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["thread_sample_graph_no"].ToString()))
                this.txtThreadDrawingNo.Text = contractObj["thread_sample_graph_no"].ToString();
            //if (!string.IsNullOrWhiteSpace(contractObj["pipe_heat_no"].ToString()))
            //    this.txtHeatNo.Text = contractObj["pipe_heat_no"].ToString();
            //if (!string.IsNullOrWhiteSpace(contractObj["pipe_lot_no"].ToString()))
            //    this.txtBatchNo.Text = contractObj["pipe_lot_no"].ToString();
            if (!string.IsNullOrWhiteSpace(Person.employee_no))
                this.txtOperatorNo.Text = Person.employee_no;
            if (!string.IsNullOrWhiteSpace(Person.pname))
                this.txtOperatorName.Text = Person.pname;
        }
        #endregion

        #region 初始化测量项
        private void InitMeasureTools(JArray measureArr)
        {
            measureItemCodeList.Clear();
            foreach (var item in measureArr)
            {
                JObject obj = (JObject)item;
                measureItemCodeList.Add(obj["measure_item_code"].ToString());
                //初始化测量工具编号表单
                string measure_tool1 = obj["measure_tool1"].ToString();
                string measure_tool2 = obj["measure_tool2"].ToString();
                if (!string.IsNullOrWhiteSpace(measure_tool1) || !string.IsNullOrWhiteSpace(measure_tool1))
                {
                    Panel pnlMeasureTool = new Panel() { Width = 312, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                    Label lbl0_0 = new Label { Text = obj["measure_item_name"].ToString(), Name = obj["measure_item_code"].ToString() + "_lbl_Name", Location = new Point(50, 10), AutoSize = true, TextAlign = ContentAlignment.MiddleCenter };
                    pnlMeasureTool.Controls.Add(lbl0_0);
                    if (!string.IsNullOrWhiteSpace(measure_tool1))
                    {
                        Label lbl0_1 = new Label { Text = obj["measure_tool1"].ToString(), Location = new Point(10, 40), AutoSize = true, MaximumSize = new Size(150, 0), TextAlign = ContentAlignment.MiddleRight };
                        pnlMeasureTool.Controls.Add(lbl0_1);
                        TextBox tbTool1 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool1", Location = new Point(160, 40) };
                        pnlMeasureTool.Controls.Add(tbTool1);
                        controlTxtDir.Add(obj["measure_item_code"].ToString() + "_measure_tool1", tbTool1);
                        tbTool1.Enter += new EventHandler(txt_Enter);
                        tbTool1.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbTool1.Leave += new EventHandler(txt_Leave);
                    }
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        Label lbl0_2 = new Label { Text = obj["measure_tool2"].ToString(), Location = new Point(10, 90), AutoSize = true, MaximumSize = new Size(150, 0), TextAlign = ContentAlignment.MiddleRight };
                        pnlMeasureTool.Controls.Add(lbl0_2);
                        TextBox tbTool2 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool2", Location = new Point(160, 90) };
                        pnlMeasureTool.Controls.Add(tbTool2);
                        controlTxtDir.Add(obj["measure_item_code"].ToString() + "_measure_tool2", tbTool2);
                        tbTool2.Enter += new EventHandler(txt_Enter);
                        tbTool2.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbTool2.Leave += new EventHandler(txt_Leave);
                    }
                    this.flpTabOneContent.Controls.Add(pnlMeasureTool);
                }
                //1.先获取readingTypes(1代表单值,2代表最大值,3代表最小值,4代表均值,5代表椭圆度)
                string[] readtyps = { };
                if (obj["reading_types"] != null)
                    readtyps = obj["reading_types"].ToString().Split(',');
                if (readtyps.Length > 0)
                {
                    //初始化测量值表单
                    //添加测量项的名字和是否必填提示
                    Panel pnlMeasureValue = new Panel { Width = 310, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                    Label lblMeasureName = new Label { Text = obj["measure_item_name"].ToString(), Name = obj["measure_item_code"].ToString() + "_lbl_Name", Location = new Point(10, 10), AutoSize = true };
                    Label lblRequired = new Label { Text = "*必填", Name = obj["measure_item_code"].ToString() + "_lbl_Prompt", Location = new Point(220, 10), Width = 100, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Red };
                    pnlMeasureValue.Controls.Add(lblMeasureName);
                    pnlMeasureValue.Controls.Add(lblRequired);
                    //2.获取是否是两端检验
                    string both_ends = "0";
                    if (obj["both_ends"] != null)
                        both_ends = obj["both_ends"].ToString();
                    string item_min_value = "", item_max_value = "", item_frequency = "", item_pos_deviation_value = "", item_neg_deviation_value = "", ovality_max="",item_std_value="";
                    if (obj["item_min_value"] != null)
                        item_min_value = obj["item_min_value"].ToString();
                    if (obj["item_max_value"] != null)
                        item_max_value = obj["item_max_value"].ToString();
                    if (obj["item_frequency"] != null)
                    {
                        item_frequency = obj["item_frequency"].ToString();
                        lblRequired.Tag = item_frequency;
                    }
                    if (obj["item_pos_deviation_value"] != null)
                        item_pos_deviation_value = obj["item_pos_deviation_value"].ToString();
                    if (obj["item_neg_deviation_value"] != null)
                        item_neg_deviation_value = obj["item_neg_deviation_value"].ToString();
                    if (obj["ovality_max"] != null)
                        ovality_max = obj["ovality_max"].ToString();
                    if (obj["item_std_value"] != null)
                        item_std_value = obj["item_std_value"].ToString();
                    Label lblRangeFrequencyOvality= new Label();
                    float item_max_val = 0, item_min_val = 0, pos_deviation_value = 0, neg_deviation_value = 0;
                    if (!string.IsNullOrWhiteSpace(item_pos_deviation_value))
                        pos_deviation_value = Convert.ToSingle(item_pos_deviation_value);
                    if (!string.IsNullOrWhiteSpace(item_neg_deviation_value))
                        neg_deviation_value = Convert.ToSingle(item_neg_deviation_value);
                    string rangeFrequencyOvalitySdVal = item_max_value + "," + item_min_value + ","+item_frequency+","+ ovality_max+","+ item_std_value;
                    //3.判断该测量项是否有范围
                    if (!string.IsNullOrWhiteSpace(item_min_value) && !string.IsNullOrWhiteSpace(item_max_value) && !string.IsNullOrWhiteSpace(item_frequency))
                    {
                        item_max_val = Convert.ToSingle(item_max_value);
                        item_min_val = Convert.ToSingle(item_min_value);
                        item_frequency = Convert.ToDouble(item_frequency).ToString("00%");
                        lblRangeFrequencyOvality.Name = obj["measure_item_code"].ToString() + "_RangeFrequencyOvality_lbl";
                        lblRangeFrequencyOvality.Width = 310;
                        lblRangeFrequencyOvality.TextAlign = ContentAlignment.MiddleCenter;
                        lblRangeFrequencyOvality.Location = new Point(0, 50);
                        if (item_max_val - item_min_val > 0.00001)
                        {
                            lblRangeFrequencyOvality.Tag = rangeFrequencyOvalitySdVal;
                            if (Math.Abs(pos_deviation_value)- Math.Abs(neg_deviation_value) <= 0.00001)
                                lblRangeFrequencyOvality.Text = "±"+ pos_deviation_value + "/" + item_frequency;
                            else
                                lblRangeFrequencyOvality.Text = neg_deviation_value + "～" + pos_deviation_value + "/" + item_frequency;
                            pnlMeasureValue.Controls.Add(lblRangeFrequencyOvality);
                        }
                        else
                        {
                            lblRangeFrequencyOvality.Tag = rangeFrequencyOvalitySdVal;
                            lblRangeFrequencyOvality.Text = item_frequency;
                            pnlMeasureValue.Controls.Add(lblRangeFrequencyOvality);
                        }
                    }
                    else
                    {
                        //添加频率
                        lblRangeFrequencyOvality.Tag = rangeFrequencyOvalitySdVal;
                        lblRangeFrequencyOvality.Text = item_frequency;
                        pnlMeasureValue.Controls.Add(lblRangeFrequencyOvality);
                    }
                    //代表该测量项只是个单值
                    if (readtyps.Contains("1"))
                    {
                        //判断是否为两端都测量
                        if (both_ends.Contains("1"))
                        {
                            Label lblA = new Label { Text = "A:", Location = new Point(60, 80), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                            TextBox tbA = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(100, 80) };
                            Label lblB = new Label { Text = "B:", Location = new Point(60, 120), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                            TextBox tbB = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_B_Value", Location = new Point(100, 120) };
                            pnlMeasureValue.Controls.Add(lblA);
                            pnlMeasureValue.Controls.Add(lblB);
                            pnlMeasureValue.Controls.Add(tbA);
                            pnlMeasureValue.Controls.Add(tbB);
                            controlTxtDir.Add(obj["measure_item_code"].ToString() + "_A_Value", tbA);
                            controlTxtDir.Add(obj["measure_item_code"].ToString() + "_B_Value", tbB);
                            tbA.Enter += new EventHandler(txt_Enter);
                            tbA.MouseDown += new MouseEventHandler(txt_MouseDown);
                            tbA.Leave += new EventHandler(txt_Leave);
                            tbB.Enter += new EventHandler(txt_Enter);
                            tbB.MouseDown += new MouseEventHandler(txt_MouseDown);
                            tbB.Leave += new EventHandler(txt_Leave);
                        }
                        else
                        {
                            TextBox tbValue = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(90, 80) };
                            pnlMeasureValue.Controls.Add(tbValue);
                            controlTxtDir.Add(obj["measure_item_code"].ToString() + "_A_Value", tbValue);
                            tbValue.Enter += new EventHandler(txt_Enter);
                            tbValue.MouseDown += new MouseEventHandler(txt_MouseDown);
                            tbValue.Leave += new EventHandler(txt_Leave);
                        }
                    }
                    else
                    {
                        lblMeasureName.Location = new Point(10, 10);
                        Label lblMax = new Label { Text = "最大", Location = new Point(40, 50), AutoSize = true };
                        Label lblMin = new Label { Text = "最小", Location = new Point(180, 50) };
                        Label lblMaxA = new Label { Text = "A:", Location = new Point(10, 80), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                        TextBox tbMaxA = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_MaxA_Value", Location = new Point(40, 80) };
                        Label lblMaxB = new Label { Text = "B:", Location = new Point(10, 120), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                        TextBox tbMaxB = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_MaxB_Value", Location = new Point(40, 120) };
                        Label lblMinA = new Label { Text = "A:", Location = new Point(150, 80), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                        TextBox tbMinA = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_MinA_Value", Location = new Point(170, 80) };
                        Label lblMinB = new Label { Text = "B:", Location = new Point(150, 120), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                        TextBox tbMinB = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_MinB_Value", Location = new Point(170, 120) };
                        Label lblAvg = new Label { Text = "均值", Location = new Point(290, 50), AutoSize = true };
                        Label lblOvality = new Label { Text = "椭圆度", Location = new Point(370, 50), AutoSize = true };
                        Label lblAvgA = new Label {Tag="lblVal", Text = "", Location = new Point(290, 80), Name = obj["measure_item_code"].ToString() + "_AvgA" };
                        Label lblAvgB = new Label { Tag = "lblVal", Text = "", Location = new Point(290, 120), Name = obj["measure_item_code"].ToString() + "_AvgB" };
                        Label lblOvalityA = new Label { Tag = "lblVal", Text = "", Location = new Point(390, 80), Name = obj["measure_item_code"].ToString() + "_OvalityA" };
                        Label lblOvalityB = new Label { Tag = "lblVal", Text = "", Location = new Point(390, 120), Name = obj["measure_item_code"].ToString() + "_OvalityB" };
                        tbMaxA.Enter += new EventHandler(txt_Enter);
                        tbMaxA.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbMaxA.Leave += new EventHandler(txt_Leave);
                        tbMaxB.Enter += new EventHandler(txt_Enter);
                        tbMaxB.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbMaxB.Leave += new EventHandler(txt_Leave);
                        tbMinA.Enter += new EventHandler(txt_Enter);
                        tbMinA.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbMinA.Leave += new EventHandler(txt_Leave);
                        tbMinB.Enter += new EventHandler(txt_Enter);
                        tbMinB.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbMinB.Leave += new EventHandler(txt_Leave);
                        //如果包含最大值
                        if (readtyps.Contains("2"))
                        {
                            pnlMeasureValue.Controls.Add(lblMax);
                            pnlMeasureValue.Controls.Add(lblMaxA);
                            pnlMeasureValue.Controls.Add(tbMaxA);
                            controlTxtDir.Add(obj["measure_item_code"].ToString() + "_MaxA_Value", tbMaxA);
                            if (both_ends.Contains("1"))
                            {
                                pnlMeasureValue.Controls.Add(lblMaxB);
                                pnlMeasureValue.Controls.Add(tbMaxB);
                                controlTxtDir.Add(obj["measure_item_code"].ToString() + "_MaxB_Value", tbMaxB);
                            }
                        }
                        //如果包含最小值
                        if (readtyps.Contains("3"))
                        {
                            pnlMeasureValue.Controls.Add(lblMin);
                            pnlMeasureValue.Controls.Add(lblMinA);
                            pnlMeasureValue.Controls.Add(tbMinA);
                            controlTxtDir.Add(obj["measure_item_code"].ToString() + "_MinA_Value", tbMinA);
                            if (both_ends.Contains("1"))
                            {
                                pnlMeasureValue.Controls.Add(lblMinB);
                                pnlMeasureValue.Controls.Add(tbMinB);
                                controlTxtDir.Add(obj["measure_item_code"].ToString() + "_MinB_Value", tbMinB);
                            }
                        }
                        //如果包含均值
                        if (readtyps.Contains("4"))
                        {
                            pnlMeasureValue.Width = 480;
                            lblRangeFrequencyOvality.Location = new Point(130, 10);
                            lblRequired.Location = new Point(380, 10);
                            pnlMeasureValue.Controls.Add(lblAvg);
                            pnlMeasureValue.Controls.Add(lblAvgA);
                            controlLblDir.Add(obj["measure_item_code"].ToString() + "_AvgA", lblAvgA);
                            if (both_ends.Contains("1")) {
                                pnlMeasureValue.Controls.Add(lblAvgB);
                                controlLblDir.Add(obj["measure_item_code"].ToString() + "_AvgB", lblAvgB);
                            }
                                
                        }
                        //如果包含椭圆度
                        if (readtyps.Contains("5"))
                        {
                            pnlMeasureValue.Width = 480;
                            lblRangeFrequencyOvality.Location = new Point(130, 10);
                            lblRequired.Location = new Point(380, 10);
                            pnlMeasureValue.Controls.Add(lblOvality);
                            pnlMeasureValue.Controls.Add(lblOvalityA);
                            controlLblDir.Add(obj["measure_item_code"].ToString() + "_OvalityA", lblOvalityA);
                            if (both_ends.Contains("1")) {
                                pnlMeasureValue.Controls.Add(lblOvalityB);
                                controlLblDir.Add(obj["measure_item_code"].ToString() + "_OvalityB", lblOvalityB);
                            }
                               
                        }
                    }
                    this.flpTabTwoContent.Controls.Add(pnlMeasureValue);
                }
            }
            //flpTabOneTxtList.Clear();
            //flpTabTwoTxtList.Clear();
           
        }
        #endregion

        #region 清理表单头部信息
        public void ClearCntrValue(Control parContainer)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    ClearCntrValue(parContainer.Controls[index]);
                }
                else
                {
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            parContainer.Controls[index].Text = "";
                            break;
                    }
                }
            }
        }
        #endregion

        #region 表单提交事件
        private void btnFormSubmit_Click(object sender, EventArgs e)
        {
            //if (button2.Text.Trim().Contains("结束录制") || button1.Text.Trim().Contains("结束扫码"))
            //{
            //    MessagePrompt.Show("录像机或读码器尚未关闭！");
            //}
            if (!string.IsNullOrWhiteSpace(Person.pname) && !string.IsNullOrWhiteSpace(Person.employee_no))
            {
                //关闭录像
                if(!button2.Text.Contains("开始录制"))
                   button2_Click(null, null);
                ThreadFormSubmit();
            }
            else
            {
                MessagePrompt.Show("您已掉线，请重新登录!");
                Application.Exit();
            }

        }
        #endregion

        #region 表单关闭事件
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            //关闭之前判断是否关闭读码器和结束录像
            if (button2.Text.Trim().Contains("结束录制"))
            {
                MessagePrompt.Show("录像机尚未关闭！");
            }
            else
            {
                RestoreSetting();
                //清空表单测量值
                //ClearForm();
                this.Hide();
            }
        }
        #endregion

        #region 表单提交
        private void ThreadFormSubmit()
        {
            String param = "";
            try
            {
                string videoNo = videosArr;
                //一条item_record检验记录包括(检测项编码、最大值、最小值、均值、椭圆度、量具编号1、量具编号2、检测项值)
                //检测项值、最大值、最小值、均值、椭圆度分为A、B
                string itemvalue = "", reading_max = "",reading_min="",reading_avg="",reading_ovality="",toolcode1="",toolcode2="",measure_sample1="",measure_sample2="";
               
                //获取所有的flpTabOne容器中所有的TextBox控件
                //GoThroughControls(this.flpTabOneContent,flpTabOneTxtList,null);
                //获取所有的flpTabTwo容器中所有的TextBox和Label控件
                //GoThroughControls(this.flpTabTwoContent,flpTabTwoTxtList,flpTabTwoLblList);
                //遍历所有属于某个测量项的值
                JArray jarray = new JArray();
                foreach (string measure_item_code in measureItemCodeList)
                {
                    if(controlTxtDir.ContainsKey(measure_item_code + "_A_Value"))
                        itemvalue += controlTxtDir[measure_item_code + "_A_Value"].Text.Trim() + ",";
                    if (controlTxtDir.ContainsKey(measure_item_code + "_B_Value"))
                        itemvalue += controlTxtDir[measure_item_code + "_B_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MaxA_Value"))
                        reading_max += controlTxtDir[measure_item_code + "_MaxA_Value"].Text.Trim() + ",";
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MaxB_Value"))
                        reading_max += controlTxtDir[measure_item_code + "_MaxB_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MinA_Value"))
                        reading_min += controlTxtDir[measure_item_code + "_MinA_Value"].Text.Trim() + ",";
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MinB_Value"))
                        reading_min += controlTxtDir[measure_item_code + "_MinB_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_measure_tool1"))
                        toolcode1 += controlTxtDir[measure_item_code + "_measure_tool1"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_measure_tool2"))
                        toolcode2 += controlTxtDir[measure_item_code + "_measure_tool2"].Text.Trim();
                    if (controlLblDir.ContainsKey(measure_item_code + "_AvgA"))
                        reading_avg += controlLblDir[measure_item_code + "_AvgA"].Text.Trim()+ ",";
                    if (controlLblDir.ContainsKey(measure_item_code + "_AvgB"))
                        reading_avg += controlLblDir[measure_item_code + "_AvgB"].Text.Trim();
                    if (controlLblDir.ContainsKey(measure_item_code + "_OvalityA"))
                        reading_ovality += controlLblDir[measure_item_code + "_OvalityA"].Text.Trim() + ",";
                    if (controlLblDir.ContainsKey(measure_item_code + "_OvalityB"))
                        reading_ovality += controlLblDir[measure_item_code + "_OvalityB"].Text.Trim();
                     
                    if (itemvalue.Equals(","))
                        itemvalue = itemvalue.Replace(",", "");
                    if (reading_max.Equals(","))
                        reading_max = reading_max.Replace(",", "");
                    if (reading_min.Equals(","))
                        reading_min = reading_min.Replace(",", "");
                    if (reading_avg.Equals(","))
                        reading_avg = reading_avg.Replace(",", "");
                    if (reading_ovality.Equals(","))
                        reading_ovality = reading_ovality.Replace(",", "");
                    JObject jobject = new JObject{
                        {"itemcode", measure_item_code },
                        {"itemvalue",HttpUtility.UrlEncode(itemvalue,Encoding.UTF8)}, {"reading_max",reading_max},
                        {"reading_min",reading_min}, {"reading_avg",reading_avg},
                        {"reading_ovality",reading_ovality}, {"toolcode1",toolcode1},
                         {"toolcode2",toolcode2}, {"measure_sample1",measure_sample1},
                         { "measure_sample2",measure_sample2}
                    };
                    jarray.Add(jobject);
                    itemvalue = "";reading_max = "";reading_min = "";reading_avg = "";reading_ovality = "";
                    toolcode1 = "";toolcode2 = "";measure_sample1 = "";measure_sample2 = "";
                }
                string inspectionResult = "";
                if (isQualified)
                    inspectionResult = "合格";
                else
                    inspectionResult = "不合格";
                JObject dataJson = new JObject {
                    {"isAdd","add" }, {"coupling_no",HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8) },
                    {"contract_no", HttpUtility.UrlEncode(this.cmbContractNo.Text, Encoding.UTF8) },
                    { "production_line", HttpUtility.UrlEncode(txtProductionArea.Text.Trim(), Encoding.UTF8) },
                    {"machine_no", HttpUtility.UrlEncode(txtMachineNo.Text.Trim(), Encoding.UTF8) },
                    { "operator_no", HttpUtility.UrlEncode(txtOperatorNo.Text.Trim(), Encoding.UTF8)},
                    {"production_crew",HttpUtility.UrlEncode(this.cmbProductionCrew.Text, Encoding.UTF8)  },
                    { "production_shift",HttpUtility.UrlEncode(this.cmbProductionShift.Text, Encoding.UTF8) },
                     {"video_no",HttpUtility.UrlEncode(videoNo, Encoding.UTF8)  },
                    { "inspection_result",HttpUtility.UrlEncode(inspectionResult, Encoding.UTF8) },
                     {"coupling_heat_no",HttpUtility.UrlEncode(this.txtHeatNo.Text, Encoding.UTF8)  },
                    { "coupling_lot_no",HttpUtility.UrlEncode(this.txtBatchNo.Text, Encoding.UTF8) },
                     { "item_record",jarray.ToString() },
                };
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                param = dataJson.ToString();
                byte[] data = encoding.GetBytes(dataJson.ToString());
                string url = CommonUtil.getServerIpAndPort() +"ThreadingOperation/saveThreadInspectionRecordOfWinform.action";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json;characterSet:utf-8";
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
                    
                    if (rowsJson.Trim().Equals("success"))
                    {
                        MessagePrompt.Show("提交成功!");
                    }
                    else
                    {
                        MessagePrompt.Show("提交失败,表单暂时保存在本地!");
                        string coupingDir = Application.StartupPath + "\\unsubmit";
                        CommonUtil.writeUnSubmitForm(HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8), param, coupingDir);
                    }
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("提交出错,表单暂时保存在本地,错误信息:"+e.Message);
                string coupingDir = Application.StartupPath + "\\unsubmit";
                CommonUtil.writeUnSubmitForm(HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8),param, coupingDir);
            }
            finally
            {
                try
                {
                    ChangeSubmitCount();
                    ChangeFrequency();
                    ClearForm();
                    IndexWindow.getForm().getThreadingProcessData();
                }
                catch (Exception ex) {
                    MessageBox.Show("表单清理数据时出错,错误信息:"+ex.Message);
                }
            }
        }
        #endregion

        #region 遍历封装提交项函数
        private void GoThroughControls(Control parContainer, List<TextBox> txtList,List<Label>lblList)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControls(parContainer.Controls[index], txtList, lblList);
                }
                else
                {
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            txtList.Add((TextBox)parContainer.Controls[index]);
                            break;
                        case "Label":
                            if(lblList!=null)
                                 lblList.Add((Label)parContainer.Controls[index]);
                            break;
                    }
                }
            }
        }
        #endregion

        #region 时间改变事件
        private void dtpInspectionTime_ValueChanged(object sender, EventArgs e)
        {
            this.dtpInspectionTime.Value = DateTime.Now;
        }
        #endregion

        #region 输入框获取焦点事件
        private void txt_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)sender;
                if (tb.Tag != null)
                {

                    if (tb.Tag.ToString().Contains("English"))
                    {
                        AlphabetKeyboardForm.getForm().inputTxt = tb;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        SetAlphaKeyboardText(tb.Name);
                        focusTextBoxName = tb.Name;
                    }
                    else
                    {
                        if (IsHaveCoupingNoAndStartRecordVideo())
                        {
                            NumberKeyboardForm.getForm().inputTxt = tb;
                            NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                            NumberKeyboardForm.getForm().Show();
                            NumberKeyboardForm.getForm().TopMost = true;
                            SetNumberKeyboardText(tb.Name);
                        }
                        else
                        {
                            txtCoupingNo.Focus();
                            MessagePrompt.Show("请检查接箍编号是否输入和视频录制是否启动!");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("鼠标获取焦点时出错,错误信息:"+ex.Message);
            }

        }
        #endregion

        #region 输入框失去焦点事件
        private void txt_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)sender;
                if (tb.Tag != null)
                {
                    if (tb.Tag.ToString().Contains("English"))
                    {
                        AlphabetKeyboardForm.getForm().Hide();
                    }
                    else
                    {
                        NumberKeyboardForm.getForm().Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("鼠标失去焦点时出错!");
            }

        }
        #endregion

        #region 鼠标点击输入框事件
        private void txt_MouseDown(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                if (tb.Tag != null)
                {

                    if (tb.Tag.ToString().Contains("English"))
                    {
                        AlphabetKeyboardForm.getForm().inputTxt = tb;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        AlphabetKeyboardForm.getForm().TopMost = true;
                        focusTextBoxName = tb.Name;
                        SetAlphaKeyboardText(tb.Name);
                    }
                    else
                    {
                       // MessageBox.Show(IsHaveCoupingNoAndStartRecordVideo().ToString());
                        if (IsHaveCoupingNoAndStartRecordVideo())
                        {
                            NumberKeyboardForm.getForm().inputTxt = tb;
                            NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                            NumberKeyboardForm.getForm().Show();
                            NumberKeyboardForm.getForm().TopMost = true;
                            SetNumberKeyboardText(tb.Name);
                        }
                        else
                        {
                            txtCoupingNo.Focus();
                            MessagePrompt.Show("请检查接箍编号是否输入和视频录制是否启动!");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                //MessagePrompt.Show(ex.Message);
                Console.WriteLine("鼠标点击时出错!");
            }
        }

        #endregion

        #region 扫码事件
        private void button1_Click(object sender, EventArgs e)
        {

            string btnName = this.button1.Text;
            if (btnName.Contains("结束扫码"))
            {
                try
                {
                    YYKeyenceReaderConsole.codeReaderOff();
                    this.button1.Text = "开始扫码";
                    this.lblReaderStatus.Text = "读码完成...";
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("关闭读码器出错,错误信息:" + ex.Message);
                }

            }
            else if (btnName.Contains("开始扫码"))
            {
                try
                {
                    if (YYKeyenceReaderConsole.readerStatus == 1)
                    {
                        YYKeyenceReaderConsole.codeReaderLon();
                        if (YYKeyenceReaderConsole.readerStatus == 1) {
                            this.lblReaderStatus.Text = "读码中...";
                            this.button1.Text = "结束扫码";
                        }
                    }
                    else {
                        MessagePrompt.Show("请检查读码器的状态!");
                    }
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("开启读码器出错,错误信息:" + ex.Message);
                }

            }
        }
        #endregion

        #region 录制视频事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text.Contains("开始录制"))
            {
                RealTimePreview();
                //try
                //{
                //    if (MainWindow.recordStatus == 3) {
                //        timestamp = CommonUtil.getMesuringRecord();
                //        RealTimePreview();
                //        MainWindow.RecordVideo(timestamp);
                //        this.lblVideoStatus.Text = "开始录制...";
                //        this.lblVideoStatus.Text = "录像中...";
                //        videosArr += timestamp + "_vcr.mp4;";
                //        if (timer != null)
                //        {
                //            countTime = 0;
                //            timer.Start();
                //        }
                //        else
                //        {
                //            timer = new System.Timers.Timer();
                //            timer.Enabled = true;
                //            timer.AutoReset = true;
                //            timer.Interval = 1000;
                //            timer.Elapsed += new System.Timers.ElapsedEventHandler(CountTimer);
                //            timer.Start();
                //        }
                //        this.button2.Text = "结束录制";
                //    }else
                //        MessagePrompt.Show("录像机暂未启动!");

                //}
                //catch (Exception ex) {
                //    MessagePrompt.Show("录制出错，错误信息:"+ex.Message);
                //}
            }
            else if (this.button2.Text.Contains("结束录制"))
            {
                if (MainWindow.recordStatus == 4) {
                    MainWindow.stopRecordVideo();
                    timer.Stop();
                    //保存timestamp到fileuploadrecord中
                    RestoreSetting();
                    this.button2.Text = "开始录制";
                    this.lblVideoStatus.Text = "录制完成...";
                    //将视频移到done文件夹下
                    string sourceFilePath = Application.StartupPath + "\\draft\\" + timestamp + ".mp4";
                    string destPath = Application.StartupPath + "\\done\\" + timestamp + ".mp4";
                    if (CommonUtil.MoveFile(sourceFilePath, destPath))
                    {
                        CommonUtil.DeleteFile(sourceFilePath);
                    }
                    this.lblTimer.Text = "";
                }
            }
        }
        #endregion

        #region 计时器
        private void CountTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new EventHandle(TimerAction), source, e);
        }


        public void TimerAction(Object source, EventArgs e)
        {
            countTime++;
            if ((15 * 60) >= countTime)
            {
                this.lblTimer.Text = "还剩:" + ((15 * 60 - countTime) / 60) + "分" + ((15 * 60 - countTime) % 60) + "秒";
            }
            else
            {
                this.lblTimer.Text = "";
                timer.Stop();
                this.EndVideoRecord();
            }
        }
        #endregion

        #region 小窗口显示实时录制视频内容
        public void RealTimePreview()
        {
            if (MainWindow.getForm() != null)
            {
                MainWindow.getForm().groupBox1.Hide(); MainWindow.getForm().groupBox2.Hide();
                MainWindow.getForm().groupBox3.Hide(); MainWindow.getForm().groupBox4.Hide();
                int width = MainWindow.getForm().Width;
                int height = MainWindow.getForm().Height;
                MainWindow.getForm().Width = 150;
                MainWindow.getForm().Height = 150;
                MainWindow.getForm().RealPlayWnd.Width = width;
                MainWindow.getForm().RealPlayWnd.Height = height;
                int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
                int x = iActulaWidth - 150;
                int y = 55;
                MainWindow.getForm().RealPlayWnd.Left = 0;
                MainWindow.getForm().RealPlayWnd.Top = 0;
                MainWindow.getForm().RealPlayWnd.Dock = DockStyle.Fill;
                MainWindow.getForm().Show();
                MainWindow.getForm().TopMost = true;
                MainWindow.getForm().Location = new Point(x, y);
                //mainWindow.FormBorderStyle = FormBorderStyle.FixedDialog;
                //MainWindow.getForm().MaximumSize = new Size(150, 150);
                //MainWindow.getForm().MinimumSize=new Size(150, 150);
            }
        }
        #endregion

        #region 重置录像机设置
        public void RestoreSetting()
        {
            
                MainWindow.getForm().FormBorderStyle = FormBorderStyle.Sizable;
                MainWindow.getForm().MaximumSize = new Size(2000, 2000);
                MainWindow.getForm().Left = MainWindow.mainWindowX;
                MainWindow.getForm().Top = MainWindow.mainWindowY;
                MainWindow.getForm().Width = MainWindow.mainWindowWidth;
                MainWindow.getForm().Height = MainWindow.mainWindowHeight;
                MainWindow.getForm().RealPlayWnd.Left = MainWindow.realTimeX;
                MainWindow.getForm().RealPlayWnd.Top = MainWindow.realTimeY;
                MainWindow.getForm().RealPlayWnd.Width = MainWindow.realTimeWidth;
                MainWindow.getForm().RealPlayWnd.Height = MainWindow.realTimeHeigh;
                MainWindow.getForm().RealPlayWnd.Dock = DockStyle.None;
                MainWindow.getForm().groupBox1.Show();
                MainWindow.getForm().groupBox2.Show();
                MainWindow.getForm().groupBox3.Show();
                MainWindow.getForm().groupBox4.Show();
                MainWindow.getForm().TopMost = false;
                MainWindow.getForm().Hide();
        }
        #endregion

        #region 根据控件名找到该控件
        private object GetControlInstance(object obj, string strControlName)
        {
            IEnumerator Controls = null;//所有控件
            Control c = null;//当前控件
            Object cResult = null;//查找结果
            if (obj.GetType() == this.GetType())//窗体
            {
                Controls = this.Controls.GetEnumerator();
            }
            else//控件
            {
                Controls = ((Control)obj).Controls.GetEnumerator();
            }
            while (Controls.MoveNext())//遍历操作
            {
                c = (Control)Controls.Current;//当前控件
                if (c.HasChildren)//当前控件是个容器
                {
                    cResult = GetControlInstance(c, strControlName);//递归查找
                    if (cResult == null)//当前容器中没有，跳出，继续查找
                        continue;
                    else//找到控件，返回
                        return cResult;
                }
                else if (c.Name == strControlName)//不是容器，同时找到控件，返回
                {
                    return c;
                }
            }
            return null;//控件不存在
        }
        #endregion

        #region 设置数字键盘Title
        private void SetNumberKeyboardText(string inputTxtName)
        {
            try
            {
                if (inputTxtName.Contains("_A_Value"))
                    inputTxtName = inputTxtName.Replace("_A_Value", "");
                if (inputTxtName.Contains("_B_Value"))
                    inputTxtName = inputTxtName.Replace("_B_Value", "");
                if (inputTxtName.Contains("_MaxA_Value"))
                    inputTxtName = inputTxtName.Replace("_MaxA_Value", "");
                if (inputTxtName.Contains("_MaxB_Value"))
                    inputTxtName = inputTxtName.Replace("_MaxB_Value", "");
                if (inputTxtName.Contains("_MinA_Value"))
                    inputTxtName = inputTxtName.Replace("_MinA_Value", "");
                if (inputTxtName.Contains("_MinB_Value"))
                    inputTxtName = inputTxtName.Replace("_MinB_Value", "");
                Label lbl = (Label)GetControlInstance(flpTabTwoContent, inputTxtName + "_lbl_Name");
                Label lbl0 = (Label)GetControlInstance(flpTabTwoContent, inputTxtName + "_lbl_Prompt");
                if (lbl != null)
                {
                    if (lbl0.Visible == true)
                        NumberKeyboardForm.getForm().lblNumberTitle.Text = lbl.Text + "(必填)";
                    else
                        NumberKeyboardForm.getForm().lblNumberTitle.Text = lbl.Text;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("设置输入法头部信息时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 设置英文键盘Title
        private void SetAlphaKeyboardText(string inputTxtName)
        {
            if (inputTxtName.Contains("_measure_tool1"))
                inputTxtName = inputTxtName.Replace("_measure_tool1", "");
            else if (inputTxtName.Contains("_measure_tool2"))
                inputTxtName = inputTxtName.Replace("_measure_tool2", "");
            else if (inputTxtName.Contains("txtCoupingNo"))
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "接箍编号";
            else if (inputTxtName.Contains("txtHeatNo"))
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "炉号";
            else if (inputTxtName.Contains("txtBatchNo"))
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "批号";
            else if (inputTxtName.Contains("txtMachineNo"))
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "机床号";
            else if (inputTxtName.Contains("txtProductionArea"))
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "产线";
            Label lbl = (Label)GetControlInstance(flpTabOneContent, inputTxtName + "_lbl_Name");
            if (lbl != null)
                AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = lbl.Text;
        }
        #endregion

        #region 清理表单
        private void ClearForm()
        {
            foreach (TextBox tb in flpTabTwoTxtList) {
                tb.Text ="";
                tb.BackColor = Color.White;
            }
            foreach (Label lbl in flpTabTwoLblList) {
                if (lbl.Tag != null) {
                    if(lbl.Tag.ToString().Contains("lblVal"))
                    lbl.Text = "";
                }
            }
            this.txtCoupingNo.Text = "";
            this.txtHeatNo.Text = "";
            this.txtBatchNo.Text = "";
            videosArr = "";
        }
        #endregion

        #region 关闭事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            //关闭之前判断是否关闭读码器和结束录像
            if (button2.Text.Trim() == "结束录像")
            {
                MessagePrompt.Show("录像机尚未关闭！");
            }
            else
            {
                RestoreSetting();
                //清空表单测量值
                ClearForm();
                this.Hide();
            }
        }
        #endregion

        #region 下拉框绘制
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
        #endregion

        #region 判断接箍编号是否存在和视频录像是否启动
        private bool IsHaveCoupingNoAndStartRecordVideo()
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(this.txtCoupingNo.Text)&&button2.Text.Contains("结束录制"))
            {
                flag = true;
            }
            return flag;
        }
        #endregion

        #region 窗体Visible改变事件
        private void ThreadingForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Person.pname))
            {
                this.lblFormTitle.Text = "现在登录的是:" + Person.pname + ",工号:" + Person.employee_no;
                this.txtOperatorNo.Text = Person.employee_no;
                this.txtOperatorName.Text = Person.pname;
            }
        }
        #endregion

        #region 显示是否必填项
        private void ChangeFrequency() {
            try
            {
                string countStr = this.lblCountSubmit.Text;
                int countSumbit = 0;
                if (!string.IsNullOrWhiteSpace(countStr))
                {
                    countSumbit = Convert.ToInt32(countStr);//提交的次数
                }
                foreach (Label lbl in flpTabTwoLblList)
                {
                    if (lbl.Name.Contains("_lbl_Prompt"))
                    {
                        float frequency = 9;
                        if (lbl.Tag != null)
                        {
                            frequency = Convert.ToSingle(lbl.Tag.ToString());
                        }
                        if (countSumbit < 4)
                            lbl.Visible = true;
                        else {
                            if (Math.Abs((countSumbit - 1) * frequency - Convert.ToInt32((countSumbit - 1) * frequency)) < 0.000001)
                                lbl.Visible = true;
                            else
                                lbl.Visible = false;
                        }
                       
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("判断是否是必选项是出错,错误信息:"+ex.Message);
            }
        }
        #endregion

        #region 统计提交数加1
        private void ChangeSubmitCount()
        {
            string countStr = this.lblCountSubmit.Text;
            int temp = 0;
            if (!string.IsNullOrWhiteSpace(countStr))
            {
                temp = Convert.ToInt32(countStr) + 1;
                this.lblCountSubmit.Text = temp.ToString();
            }
        }
        #endregion

        #region tabControl切换事件
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.tabControl1.SelectedIndex == 0)
            {
                isMeasuringToolTabSelected = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cmbContractNo.Text))
                {
                    this.tabControl1.SelectedIndex = 0;
                    MessagePrompt.Show("请选择合同号!");
                }
                else if (string.IsNullOrWhiteSpace(cmbProductionCrew.Text)) {
                    this.tabControl1.SelectedIndex = 0;
                    MessagePrompt.Show("请选择班别!");
                }
                else if (string.IsNullOrWhiteSpace(cmbProductionShift.Text))
                {
                    this.tabControl1.SelectedIndex = 0;
                    MessagePrompt.Show("请选择班次!");
                }
                else
                {
                    isMeasuringToolTabSelected = false;
                }
            }
           
        }
        #endregion

        private void ThreadingForm_Load(object sender, EventArgs e)
        {
            InitContractList();
        }
    }
}
