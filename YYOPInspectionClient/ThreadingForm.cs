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
        private bool autoSelectContractNo = false;
        //输入框集合,存放检验记录使用测量工具
        public static List<TextBox> flpTabOneTextBoxList = new List<TextBox>();
        //输入框集合,存放检验记录测量的数据
        public static List<TextBox> flpTabTwoTextBoxList = new List<TextBox>();
        //测量值输入框集合(参数为:测量项名称、测量值对应的TextBox控件)
        public static Dictionary<string, TextBox> controlTxtDir = new Dictionary<string, TextBox>();
        //定义是否测量数据是否合法的集合
        public static Dictionary<string, bool> qualifiedList = new Dictionary<string, bool>();
        //测量项编号集合
        private List<string> measureItemCodeList = new List<string>();
        //时间戳(视频和form表单保存的目录名)
        private string videosArr = "";
        //定义时间戳变量
        private string timestamp = null;
        //定义计时器从开始到现在的时间(单位秒)
        private int countTime = 0;
        //定义当前窗体变量
        private static ThreadingForm myForm = null;
        //定义录制视频计时器
        public System.Timers.Timer timer = null;
        //定义鼠标事件的委托
        public delegate void EventHandle(object sender, EventArgs e);
        //定义是否打开测量工具tab
        public static bool isMeasuringToolTabSelected=true;
        //定义当是三支全检的时候检验到第几支
        private static int fullInspectionCount =0;
        //判断是否时三只全检
        private static bool isFullInspection = false;
        //存放测量项信息得集合
        public static Dictionary<string, Dictionary<string, string>> measureInfoDic = new Dictionary<string, Dictionary<string, string>>();
        //存放必填标识"*"号得Label集合
        private Dictionary<string, Label> requiredMarkDic = new Dictionary<string, Label>();
        //存放输入法头部信息得集合
        private Dictionary<string, string>keyboardTitleDic = new Dictionary<string, string>();
        #region 单例函数
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
            try
            {
                if (!string.IsNullOrWhiteSpace(Person.pname) && !string.IsNullOrWhiteSpace(Person.employee_no))
                {
                    //将英文输入法中显示测量值的控件容器指定为当前测量值的控件容器
                    //AlphabetKeyboardForm.getForm().containerControl = this.flpTabOneContent;
                    //将数字输入法中显示测量值的控件容器指定为当前测量值的控件容器
                    //NumberKeyboardForm.getForm().containerControl = this.flpTabTwoContent;
                    //设置时间戳
                    timestamp = CommonUtil.getMesuringRecord();
                    //设置读码器和录像机状态标识
                    if (YYKeyenceReaderConsole.readerStatus == -1)
                        this.lblReaderStatus.Text = "读码器未连接...";
                    else if (YYKeyenceReaderConsole.readerStatus == 1)
                        this.lblReaderStatus.Text = "读码器已启动...";
                    else
                        this.lblReaderStatus.Text = "读码器异常...";
                    if (MainWindow.recordStatus == 0)
                        this.lblVideoStatus.Text = "录像机未连接...";
                    else if (MainWindow.recordStatus == 1)
                        this.lblVideoStatus.Text = "录像机已连接...";
                    else if (MainWindow.recordStatus == 2)
                        this.lblVideoStatus.Text = "录像机未启动...";
                    else if (MainWindow.recordStatus == 3)
                        this.lblVideoStatus.Text = "录像机已启动...";
                    else if (MainWindow.recordStatus == 4)
                        this.lblVideoStatus.Text = "录像中...";
                    else
                        this.lblVideoStatus.Text = "录像机异常...";
                    autoSelectContractNo = true;
                    //初始化表单
                    InitThreadForm();
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
                MessagePrompt.Show("新建表单时出错!");
            }
            finally {
                myForm = this;
            }
        }
        #endregion

        #region 窗体Load事件
        private void ThreadingForm_Load(object sender, EventArgs e)
        {
            //初始化表单合同数组
            InitContractList();
        }
        #endregion

        #region 窗体Shown事件
        private void ThreadingForm_Shown(object sender, EventArgs e)
        {
            //开始定时器，用于检测客户端与服务器连接情况(ping测试)
            CommonUtil.flag = true;
            System.Timers.Timer t = new System.Timers.Timer(10000);//实例化Timer类，设置时间间隔
            t.Elapsed += new System.Timers.ElapsedEventHandler(CommonUtil.UpdatePing);//到达时间的时候执行事件
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }


        #endregion

        #region 初始化检验表单
        public void InitThreadForm()
        {
            //判断是否选中的有合同编号
            if (!string.IsNullOrWhiteSpace(this.cmbContractNo.Text))
            {
                //根据合同编号创建表单
                GetThreadFormInitData(this.cmbContractNo.Text);
            }
        }
        #endregion

        #region 结束录制视频
        public void EndVideoRecord()
        {
            MainWindow.stopRecordVideo();
            //保存timestamp到fileuploadrecord中
            CommonUtil.RestoreSetting(true);
            this.button2.Text = "开始录制视频";
            this.lblVideoStatus.Text = "录像完成...";
            string sourceFilePath = Application.StartupPath + "\\draft\\" + timestamp + ".mp4";
            string destPath = Application.StartupPath + "\\done\\" + timestamp + ".mp4";
            if (CommonUtil.MoveFile(sourceFilePath, destPath))
            {
                CommonUtil.DeleteFile(sourceFilePath);
            }
            this.lblTimer.Text = "";
            MessagePrompt.Show("录制视频最长为15分钟,如果未完成检验请再次点击开始录制!");
        }
        #endregion

        #region 获取所有合同编号，追加到下拉框中
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
                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
                if (content != null)
                {
                    JObject jobject = JObject.Parse(content);
                    string rowsJson = jobject["rowsData"].ToString();
                    //将获取json数据转成对应的对象
                    List<ComboxItem> list = JsonConvert.DeserializeObject<List<ComboxItem>>(rowsJson);
                    //遍历对象，将查询的合同编号添加到下拉框中
                    foreach (ComboxItem item in list)
                    {
                        this.cmbContractNo.Items.Add(item.Text);
                    }
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("获取下拉合同号时出错,错误信息:"+e.Message);
            }
        }
        #endregion

        #region 根据合同编号初始化表单
        public void GetThreadFormInitData(string contract_no)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject json = new JObject{
                    {"contract_no",contract_no }
                };
                byte[] data = encoding.GetBytes(json.ToString());
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
                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
                if (content != null)
                {
                    JObject jobject = JObject.Parse(content);
                    string rowsJson = jobject["rowsData"].ToString();
                    if (rowsJson.Trim().Contains("fail"))
                    {
                        MessagePrompt.Show("初始化表单时出错!");
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(rowsJson);
                        Console.WriteLine(jo);
                        //获取合同信息
                        string contractInfo = jo["contractInfo"].ToString();
                        //获取测量信息
                        string measureInfo = jo["measureInfo"].ToString();
                        //填充表单合同信息
                        FillFormTitle(contractInfo);
                        JArray measureArr = (JArray)JsonConvert.DeserializeObject(measureInfo);
                        //初始化测量项信息
                        InitMeasureTools(measureArr);
                    }
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("初始化表单出错，错误信息:" + e.Message);
            }
        }
        #endregion

        #region 合同编号选中事件
        private void cmbContractNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (autoSelectContractNo)
            {
                try
                {
                    //清理集合元素
                    ClearCntrValue(this);
                    flpTabOneTextBoxList.Clear();
                    flpTabTwoTextBoxList.Clear();
                    measureItemCodeList.Clear();
                    controlTxtDir.Clear();
                    qualifiedList.Clear();
                    measureInfoDic.Clear();
                    requiredMarkDic.Clear();
                    keyboardTitleDic.Clear();
                    videosArr = "";
                    countTime = 0;
                    string contract_no = this.cmbContractNo.Text.Trim();
                    GetThreadFormInitData(contract_no);
                    this.lblCountSubmit.Text = "1";
                    ChangeFrequency();
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("合同切换时出错,错误信息:"+ex.Message);
                }

            }
        }
        #endregion

        #region 初始化表单合同信息
        private void FillFormTitle(string contractInfo)
        {
            //将放置合同信息的TextBox控件清空
            ClearCntrValue(this);
            try
            {
                //设置合同信息到对应的TextBox控件中
                JObject contractObj = (JObject)JsonConvert.DeserializeObject(contractInfo);
                if (!string.IsNullOrWhiteSpace(contractObj["machining_contract_no"].ToString()))
                    this.txtMachiningContractNo.Text = contractObj["machining_contract_no"].ToString();
                if (!string.IsNullOrWhiteSpace(contractObj["threading_type"].ToString()))
                {
                    this.txtThreadType.Text = contractObj["threading_type"].ToString();
                    this.lblThreadType.Text = "螺纹类型:" + contractObj["threading_type"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(contractObj["od"].ToString()))
                {
                    this.txtOdDiameter.Text = contractObj["od"].ToString();
                    this.lblOd.Text = "外径:" + contractObj["od"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(contractObj["wt"].ToString()))
                {
                    this.txtTreadWt.Text = contractObj["wt"].ToString();
                    this.lblWt.Text = "壁厚:" + contractObj["wt"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(contractObj["pipe_steel_grade"].ToString()))
                    this.lblSteelGrade.Text = "钢级:" + contractObj["wt"].ToString();
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
                if (!string.IsNullOrWhiteSpace(Person.employee_no))
                    this.txtOperatorNo.Text = Person.employee_no;
                if (!string.IsNullOrWhiteSpace(Person.pname))
                    this.txtOperatorName.Text = Person.pname;
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("解析合同信息时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 初始化表单测量项
        private void InitMeasureTools(JArray measureArr)
        {
            foreach (var item in measureArr)
            {
                JObject obj = (JObject)item;
                //获取是否是两端检验
                string both_ends = Convert.ToString(obj["both_ends"]);
                //读数类型数组
                string[] readtyps = { };
                if (obj["reading_types"] != null)
                    readtyps = Convert.ToString(obj["reading_types"]).Split(',');
                //测量项编号
                string measure_item_code = Convert.ToString(obj["measure_item_code"]);
                //测量项名称
                string measure_item_name = Convert.ToString(obj["measure_item_name"]);
                //测量项最大值
                string item_max_value = Convert.ToString(obj["item_max_value"]);
                //测量项最小值
                string item_min_value = Convert.ToString(obj["item_min_value"]);
                //检测频率
                string item_frequency = Convert.ToString(obj["item_frequency"]);
                //测量项椭圆度最大值
                string ovality_max = Convert.ToString(obj["ovality_max"]);
                //测量项目标值
                string item_std_value = Convert.ToString(obj["item_std_value"]);
                //测量项正偏差
                string item_pos_deviation_value = Convert.ToString(obj["item_pos_deviation_value"]);
                //测量项负偏差
                string item_neg_deviation_value = Convert.ToString(obj["item_neg_deviation_value"]);
                //测量项测量工具1名称
                string measure_tool1 = Convert.ToString(obj["measure_tool1"]);
                //测量项测量工具2名称
                string measure_tool2 = Convert.ToString(obj["measure_tool2"]);

                //定义保存该测量项值得字典集合
                Dictionary<string, string> itemDic = new Dictionary<string, string>();
                //依次为测量项名称、最大值、最小值、检验频率、椭圆度最大值、目标值、是否合法
                itemDic.Add("measure_item_name", measure_item_name);
                itemDic.Add("item_max_value", item_max_value);
                itemDic.Add("item_min_value", item_min_value);
                itemDic.Add("item_frequency", item_frequency);
                itemDic.Add("ovality_max", ovality_max);
                itemDic.Add("item_std_value", item_std_value);
                measureInfoDic.Add(measure_item_code, itemDic);
                //设置默认添加的测量数据都合法
                qualifiedList.Add(measure_item_code, true);
                //将测量工具编号添加到集合中
                measureItemCodeList.Add(measure_item_code);
                //------------------------------------------初始化测量工具编号表单
                //如果测量工具1、2有一个不为空
                if (!string.IsNullOrWhiteSpace(measure_tool1) || !string.IsNullOrWhiteSpace(measure_tool2))
                {
                    //创建存放测量工具编号的Panel控件
                    Panel pnlMeasureTool = new Panel() { Width = 312, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                    //创建存放测量项名称的label控件
                    Label lbl0_0 = new Label { Text = measure_item_name, Location = new Point(50, 10), AutoSize = true, TextAlign = ContentAlignment.MiddleCenter };
                    pnlMeasureTool.Controls.Add(lbl0_0);
                    //如果测量工具1不为空
                    if (!string.IsNullOrWhiteSpace(measure_tool1))
                    {
                        //创建存放当前测量项工具1名称的Label控件
                        Label lbl0_1 = new Label { Text = measure_tool1, Font = new Font("宋体", 12), Location = new Point(5, 40), AutoSize = false, Width = 90, Height = 50, TextAlign = ContentAlignment.MiddleLeft };
                        pnlMeasureTool.Controls.Add(lbl0_1);
                        //创建存放当前测量项工具1编号的TextBox控件
                        string controlTool1Name = Convert.ToString(obj["measure_item_code"]) + "_measure_tool1";
                        TextBox tbTool1 = new TextBox { Tag = "English", Name = controlTool1Name, Location = new Point(100, 45), Width = 200 };
                        flpTabOneTextBoxList.Add(tbTool1);
                        keyboardTitleDic.Add(controlTool1Name, measure_tool1);
                        pnlMeasureTool.Controls.Add(tbTool1);
                        controlTxtDir.Add(controlTool1Name, tbTool1);
                    }
                    //如果测量工具2不为空(同上)
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        Label lbl0_2 = new Label { Name = obj["measure_item_code"].ToString() + "_measure_tool2_lbl", Text = obj["measure_tool2"].ToString(), Location = new Point(5, 90), Font = new Font("宋体", 12), AutoSize = false, Width = 90, Height = 50, TextAlign = ContentAlignment.MiddleLeft };
                        pnlMeasureTool.Controls.Add(lbl0_2);
                        string controlTool2Name = Convert.ToString(obj["measure_item_code"]) + "_measure_tool2";
                        TextBox tbTool2 = new TextBox { Tag = "English", Name = controlTool2Name, Location = new Point(100, 95), Width = 200 };
                        flpTabOneTextBoxList.Add(tbTool2);
                        keyboardTitleDic.Add(controlTool2Name, measure_tool2);
                        pnlMeasureTool.Controls.Add(tbTool2);
                        controlTxtDir.Add(controlTool2Name, tbTool2);
                    }
                    this.flpTabOneContent.Controls.Add(pnlMeasureTool);
                }
                //--------------------------------------初始化测量值表单
                //1.先获取readingTypes(1代表单值,2代表最大值,3代表最小值,4代表均值,5代表椭圆度)
                if (readtyps.Length > 0)
                {
                    //添加测量项的名字和是否必填提示
                    Panel pnlMeasureValue = new Panel { Width = 450, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                    //测量项名称
                    Label lblMeasureName = new Label { Text = measure_item_name, Location = new Point(10, 10), AutoSize = true };
                    //定义label控件用于显示正负偏差或检验频率
                    Label lblRangeFrequency = new Label { AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(150, 10) };
                    //是否必填标识
                    Label lblRequired = new Label {Tag=measure_item_name,AutoSize = true, Text = "*必填", Name = measure_item_code + "_lbl_Prompt", Location = new Point(320, 10), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Red };
                    requiredMarkDic.Add(measure_item_code, lblRequired);
                    pnlMeasureValue.Controls.Add(lblMeasureName);
                    pnlMeasureValue.Controls.Add(lblRequired);
                    //上面Label控件显示得内容
                    string txt = "";
                    //判断是否有正负偏差
                    double d_item_pos_deviation_value = Convert.ToDouble(item_pos_deviation_value);
                    double d_item_neg_deviation_value = Convert.ToDouble(item_neg_deviation_value);
                    if (d_item_pos_deviation_value != 0 || d_item_neg_deviation_value != 0)
                    {
                        if (Math.Abs(d_item_pos_deviation_value) == Math.Abs(d_item_neg_deviation_value))
                        {
                            txt += "±" + item_pos_deviation_value;
                        }
                        else
                        {
                            txt += (item_neg_deviation_value + "～" + item_pos_deviation_value);
                        }
                        txt += ("/" + Convert.ToDouble(item_frequency).ToString("00%"));
                    }
                    else
                    {
                        txt += Convert.ToDouble(item_frequency).ToString("00%");
                    }
                    lblRangeFrequency.Text = txt;
                    pnlMeasureValue.Controls.Add(lblRangeFrequency);
                    //Panel头部提示 最大值、最小值、均值、椭圆度
                    Label lblMax = new Label { Text = "最大", AutoSize = true };
                    Label lblMin = new Label { Text = "最小", AutoSize = true };
                    Label lblAvg = new Label { Text = "均值", AutoSize = true };
                    Label lblOvality = new Label { Text = "椭圆度", AutoSize = true };
                    //两端都测量
                    if (both_ends.Contains("1"))
                    {
                        //Label控件用于存放测量项A端名称
                        Label lblA = new Label { Text = "A", Location = new Point(20, 80), Width=20, TextAlign = ContentAlignment.MiddleRight };
                        //Label控件用于存放测量项B端名称
                        Label lblB = new Label { Text = "B", Location = new Point(20, 120), Width =20, TextAlign = ContentAlignment.MiddleRight };
                        pnlMeasureValue.Controls.Add(lblA);
                        pnlMeasureValue.Controls.Add(lblB);
                        //TextBox控件用于存放测量项A端值、最大值、最小值、均值、椭圆度
                        TextBox tbA = new TextBox { Tag = "Number", Name = measure_item_code + "_A_Value", Width = 70 };
                        TextBox tbMaxA = new TextBox { Tag = "Number", Name = measure_item_code + "_MaxA_Value", Width = 70 };
                        TextBox tbMinA = new TextBox { Tag = "Number", Name = measure_item_code + "_MinA_Value", Width = 70 };
                        TextBox tbAvgA = new TextBox { Tag = "Number", Name = measure_item_code + "_AvgA", Width = 70 };
                        TextBox tbOvalityA = new TextBox { Tag = "Number", Name = measure_item_code + "_OvalityA", Width = 70 };
                        //TextBox控件用于存放测量项B端值、最大值、最小值、均值、椭圆度
                        TextBox tbB = new TextBox { Tag = "Number", Name = measure_item_code + "_B_Value", Width = 70 };
                        TextBox tbMaxB = new TextBox { Tag = "Number", Name = measure_item_code + "_MaxB_Value", Width = 70 };
                        TextBox tbMinB = new TextBox { Tag = "Number", Name = measure_item_code + "_MinB_Value", Width = 70 };
                        TextBox tbAvgB = new TextBox { Tag = "Number", Name = measure_item_code + "_AvgB", Width = 70 };
                        TextBox tbOvalityB = new TextBox { Tag = "Number", Name = measure_item_code + "_OvalityB", Width = 70 };
                        List<Label> lblList = new List<Label>();
                        List<TextBox> tbAList = new List<TextBox>();
                        List<TextBox> tbBList = new List<TextBox>();
                        //该测量项包含单值
                        if (readtyps.Contains("1"))
                        {
                            tbAList.Add(tbA);
                            tbBList.Add(tbB);
                            flpTabTwoTextBoxList.Add(tbA);
                            flpTabTwoTextBoxList.Add(tbB);
                            pnlMeasureValue.Controls.Add(tbA);
                            pnlMeasureValue.Controls.Add(tbB);
                            keyboardTitleDic.Add(measure_item_code + "_A_Value",measure_item_name+"A端("+txt+")");
                            keyboardTitleDic.Add(measure_item_code + "_B_Value", measure_item_name + "B端(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_A_Value", tbA);
                            controlTxtDir.Add(measure_item_code + "_B_Value", tbB);
                        }
                        //该测量项包含最大值
                        if (readtyps.Contains("2"))
                        {
                            lblList.Add(lblMax);
                            tbAList.Add(tbMaxA);
                            tbBList.Add(tbMaxB);
                            flpTabTwoTextBoxList.Add(tbMaxA);
                            flpTabTwoTextBoxList.Add(tbMaxB);
                            pnlMeasureValue.Controls.Add(lblMax);
                            pnlMeasureValue.Controls.Add(tbMaxA);
                            pnlMeasureValue.Controls.Add(tbMaxB);
                            keyboardTitleDic.Add(measure_item_code + "_MaxA_Value", measure_item_name + "最大值A端(" + txt + ")");
                            keyboardTitleDic.Add(measure_item_code + "_MaxB_Value", measure_item_name + "最大值B端(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_MaxA_Value", tbMaxA);
                            controlTxtDir.Add(measure_item_code + "_MaxB_Value", tbMaxB);
                        }
                        //该测量项包含最小值
                        if (readtyps.Contains("3"))
                        {
                            lblList.Add(lblMin);
                            tbAList.Add(tbMinA);
                            tbBList.Add(tbMinB);
                            flpTabTwoTextBoxList.Add(tbMinA);
                            flpTabTwoTextBoxList.Add(tbMinB);
                            pnlMeasureValue.Controls.Add(lblMin);
                            pnlMeasureValue.Controls.Add(tbMinA);
                            pnlMeasureValue.Controls.Add(tbMinB);
                            keyboardTitleDic.Add(measure_item_code + "_MinA_Value", measure_item_name + "最小值A端(" + txt + ")");
                            keyboardTitleDic.Add(measure_item_code + "_MinB_Value", measure_item_name + "最小值B端(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_MinA_Value", tbMinA);
                            controlTxtDir.Add(measure_item_code + "_MinB_Value", tbMinB);
                        }
                        //该测量项包含均值
                        if (readtyps.Contains("4"))
                        {
                            lblList.Add(lblAvg);
                            tbAList.Add(tbAvgA);
                            tbBList.Add(tbAvgB);
                            flpTabTwoTextBoxList.Add(tbAvgA);
                            flpTabTwoTextBoxList.Add(tbAvgB);
                            pnlMeasureValue.Controls.Add(lblAvg);
                            pnlMeasureValue.Controls.Add(tbAvgA);
                            pnlMeasureValue.Controls.Add(tbAvgB);
                            keyboardTitleDic.Add(measure_item_code + "_AvgA", measure_item_name + "均值A端(" + txt + ")");
                            keyboardTitleDic.Add(measure_item_code + "_AvgB", measure_item_name + "均值B端(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_AvgA", tbAvgA);
                            controlTxtDir.Add(measure_item_code + "_AvgB", tbAvgB);
                        }
                        //该测量项包含椭圆度
                        if (readtyps.Contains("5"))
                        {
                            lblList.Add(lblOvality);
                            tbAList.Add(tbOvalityA);
                            tbBList.Add(tbOvalityB);
                            flpTabTwoTextBoxList.Add(tbOvalityA);
                            flpTabTwoTextBoxList.Add(tbOvalityB);
                            pnlMeasureValue.Controls.Add(lblOvality);
                            pnlMeasureValue.Controls.Add(tbOvalityA);
                            pnlMeasureValue.Controls.Add(tbOvalityB);
                            if (!string.IsNullOrWhiteSpace(ovality_max) && Convert.ToSingle(ovality_max) > 0)
                            {
                                keyboardTitleDic.Add(measure_item_code + "_OvalityA", measure_item_name + "椭圆度A端" + "(<=" + ovality_max + ")");
                                keyboardTitleDic.Add(measure_item_code + "_OvalityB", measure_item_name + "椭圆度B端" + "(<=" + ovality_max + ")");
                            }
                            else
                            {
                                keyboardTitleDic.Add(measure_item_code + "_OvalityA", measure_item_name + "椭圆度A端");
                                keyboardTitleDic.Add(measure_item_code + "_OvalityB", measure_item_name + "椭圆度B端");
                            }
                            controlTxtDir.Add(measure_item_code + "_OvalityA", tbOvalityA);
                            controlTxtDir.Add(measure_item_code + "_OvalityB", tbOvalityB);
                        }
                        //计算TextBox在Panel容器的位置
                        if (tbAList.Count > 0) {
                            int block_width = (450-60)/(tbAList.Count+1);
                            for (int i = 0; i < tbAList.Count; i++)
                            {
                                tbAList[i].Location = new Point(block_width*(i+1)+20, 80);
                            }
                            for (int i = 0; i < tbBList.Count; i++)
                            {
                                tbBList[i].Location = new Point(block_width * (i + 1) + 20, 120);
                            }
                            for (int i = 0; i < lblList.Count; i++)
                            {
                                lblList[i].Location = new Point(block_width * (i + 1) + 20, 40);
                            }
                            lblA.Location = new Point(block_width-40,80);
                            lblB.Location = new Point(block_width -40,120);
                        }
                    }
                    //一端测量
                    else
                    {
                        //TextBox控件用于存放测量项A端值、最大值、最小值、均值、椭圆度
                        TextBox tbA = new TextBox { Tag = "Number", Name = measure_item_code + "_A_Value" };
                        TextBox tbMaxA = new TextBox { Tag = "Number", Name = measure_item_code + "_MaxA_Value" };
                        TextBox tbMinA = new TextBox { Tag = "Number", Name = measure_item_code + "_MinA_Value" };
                        TextBox tbAvgA = new TextBox { Tag = "Number", Name = measure_item_code + "_AvgA" };
                        TextBox tbOvalityA = new TextBox { Tag = "Number", Name = measure_item_code + "_OvalityA" };
                        List<Label> lblList = new List<Label>();
                        List<TextBox> tbAList = new List<TextBox>();
                        //该测量项包含单值
                        if (readtyps.Contains("1"))
                        {
                            tbAList.Add(tbA);
                            flpTabTwoTextBoxList.Add(tbA);
                            pnlMeasureValue.Controls.Add(tbA);
                            keyboardTitleDic.Add(measure_item_code + "_A_Value", measure_item_name+"("+txt+")");
                            controlTxtDir.Add(measure_item_code + "_A_Value", tbA);
                        }
                        //该测量项包含最大值
                        if (readtyps.Contains("2"))
                        {
                            lblList.Add(lblMax);
                            tbAList.Add(tbMaxA);
                            flpTabTwoTextBoxList.Add(tbMaxA);
                            pnlMeasureValue.Controls.Add(lblMax);
                            pnlMeasureValue.Controls.Add(tbMaxA);
                            keyboardTitleDic.Add(measure_item_code + "_MaxA_Value", measure_item_name+"最大值" + "(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_MaxA_Value", tbMaxA);
                        }
                        //该测量项包含最小值
                        if (readtyps.Contains("3"))
                        {
                            lblList.Add(lblMin);
                            tbAList.Add(tbMinA);
                            flpTabTwoTextBoxList.Add(tbMinA);
                            pnlMeasureValue.Controls.Add(lblMin);
                            pnlMeasureValue.Controls.Add(tbMinA);
                            keyboardTitleDic.Add(measure_item_code + "_MinA_Value", measure_item_name + "最小值" + "(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_MinA_Value", tbMinA);
                        }
                        //该测量项包含均值
                        if (readtyps.Contains("4"))
                        {
                            lblList.Add(lblAvg);
                            tbAList.Add(tbAvgA);
                            flpTabTwoTextBoxList.Add(tbAvgA);
                            pnlMeasureValue.Controls.Add(lblAvg);
                            pnlMeasureValue.Controls.Add(tbAvgA);
                            keyboardTitleDic.Add(measure_item_code + "_AvgA", measure_item_name + "均值" + "(" + txt + ")");
                            controlTxtDir.Add(measure_item_code + "_AvgA", tbAvgA);
                        }
                        //该测量项包含椭圆度
                        if (readtyps.Contains("5"))
                        {
                            lblList.Add(lblOvality);
                            tbAList.Add(tbOvalityA);
                            flpTabTwoTextBoxList.Add(tbOvalityA);
                            pnlMeasureValue.Controls.Add(lblOvality);
                            pnlMeasureValue.Controls.Add(tbOvalityA);
                            if (!string.IsNullOrWhiteSpace(ovality_max) && Convert.ToSingle(ovality_max) > 0)
                            {
                                keyboardTitleDic.Add(measure_item_code + "_OvalityA", measure_item_name + "椭圆度" + "(<=" + ovality_max + ")");
                            }
                            else
                            {
                                keyboardTitleDic.Add(measure_item_code + "_OvalityA", measure_item_name + "椭圆度");
                            }
                            controlTxtDir.Add(measure_item_code + "_OvalityA", tbOvalityA);
                        }
                        //计算TextBox在Panel容器的位置
                        if (tbAList.Count > 0)
                        {
                            int block_width = (450 - 60) / (tbAList.Count + 1);
                            for (int i = 0; i < tbAList.Count; i++)
                            {
                                tbAList[i].Location = new Point(block_width * (i + 1) + 20, 80);
                            }
                            for (int i = 0; i < lblList.Count; i++)
                            {
                                lblList[i].Location = new Point(block_width * (i + 1) + 20, 40);
                            }
                        }
                    }
                    this.flpTabTwoContent.Controls.Add(pnlMeasureValue);
                }
            }
            BindMouseEvent();
        }
        #endregion

        #region 为输入框绑定事件
        public void BindMouseEvent()
        {
            foreach (TextBox item in controlTxtDir.Values) {
                item.Enter += new EventHandler(txt_Enter);
                item.MouseDown += new MouseEventHandler(txt_MouseDown);
                item.Leave += new EventHandler(txt_Leave);
            }
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
                    //判断控件的名称
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
                //重置录像机设置
                CommonUtil.RestoreSetting(true);
                this.Hide();
            }
        }
        #endregion

        #region 表单提交
        private void ThreadFormSubmit()
        {
            String param = "";
            bool submitFlag = true;
            //判断是否有必填项没有填
            string requiredTxt = CheckRequired();
            if (!string.IsNullOrWhiteSpace(requiredTxt))
            {
                MessagePrompt.Show(requiredTxt);
                return;
            }
            try
            {
                string videoNo = videosArr;
                //一条item_record检验记录包括(检测项编码、最大值、最小值、均值、椭圆度、量具编号1、量具编号2、检测项值)
                //检测项值、最大值、最小值、均值、椭圆度分为A、B
                string itemvalue = "", reading_max = "", reading_min = "", reading_avg = "", reading_ovality = "", toolcode1 = "", toolcode2 = "", measure_sample1 = "", measure_sample2 = "";
                //遍历所有属于某个测量项的值
                JArray jarray = new JArray();
                foreach (string measure_item_code in measureItemCodeList)
                {
                    //根据测量项名称在集合中找到对应的存放测量项值的控件，并获取控件的值
                    if (controlTxtDir.ContainsKey(measure_item_code + "_A_Value"))
                        itemvalue += controlTxtDir[measure_item_code + "_A_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_B_Value"))
                        itemvalue += ("," + controlTxtDir[measure_item_code + "_B_Value"].Text.Trim());
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MaxA_Value"))
                        reading_max += controlTxtDir[measure_item_code + "_MaxA_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MaxB_Value"))
                        reading_max += ("," + controlTxtDir[measure_item_code + "_MaxB_Value"].Text.Trim());
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MinA_Value"))
                        reading_min += controlTxtDir[measure_item_code + "_MinA_Value"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_MinB_Value"))
                        reading_min += ("," + controlTxtDir[measure_item_code + "_MinB_Value"].Text.Trim());
                    if (controlTxtDir.ContainsKey(measure_item_code + "_measure_tool1"))
                        toolcode1 += controlTxtDir[measure_item_code + "_measure_tool1"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_measure_tool2"))
                        toolcode2 += controlTxtDir[measure_item_code + "_measure_tool2"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_AvgA"))
                        reading_avg += controlTxtDir[measure_item_code + "_AvgA"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_AvgB"))
                        reading_avg += ("," + controlTxtDir[measure_item_code + "_AvgB"].Text.Trim());
                    if (controlTxtDir.ContainsKey(measure_item_code + "_OvalityA"))
                        reading_ovality += controlTxtDir[measure_item_code + "_OvalityA"].Text.Trim();
                    if (controlTxtDir.ContainsKey(measure_item_code + "_OvalityB"))
                        reading_ovality += ("," + controlTxtDir[measure_item_code + "_OvalityB"].Text.Trim());
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
                    //封装测量项的值
                    JObject jobject = new JObject{
                        {"itemcode", measure_item_code },
                        {"itemvalue",HttpUtility.UrlEncode(itemvalue,Encoding.UTF8)},
                        { "reading_max",reading_max},
                        {"reading_min",reading_min},
                        { "reading_avg",reading_avg},
                        {"reading_ovality",reading_ovality},
                        { "toolcode1",toolcode1},
                        {"toolcode2",toolcode2},
                        { "measure_sample1",measure_sample1},
                        { "measure_sample2",measure_sample2}
                    };
                    jarray.Add(jobject);
                    itemvalue = ""; reading_max = ""; reading_min = ""; reading_avg = ""; reading_ovality = "";
                    toolcode1 = ""; toolcode2 = ""; measure_sample1 = ""; measure_sample2 = "";
                }
                string inspectionResult = "合格";
                //遍历每个测量项，判断是否符合标准
                foreach (var item in qualifiedList)
                {
                    if (item.Value == false)
                    {
                        inspectionResult = "不合格";
                        break;
                    }
                }
              
                if (inspectionResult.Contains("不合格"))
                {
                    if (MessageBox.Show("数据不合格,确定要提交吗?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        submitFlag = true;
                    else
                        submitFlag = false;
                }
                if (submitFlag)
                {
                    //封装检验数据
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
                    string url = CommonUtil.getServerIpAndPort() + "ThreadingOperation/saveThreadInspectionRecordOfWinform.action";
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
                    using (StreamReader sr = new StreamReader(streamResponse))
                    {
                        content = sr.ReadToEnd();
                    }
                    response.Close();
                    if (content != null)
                    {
                        JObject jobject = JObject.Parse(content);
                        string rowsJson = jobject["rowsData"].ToString();
                        //如果提交表单成功
                        if (rowsJson.Trim().Contains("success"))
                        {
                            MessagePrompt.Show("提交成功!");
                        }
                        else//提交失败，将表单数据保存到本地
                        {
                            MessagePrompt.Show("提交失败,表单暂时保存在本地!");
                            string coupingDir = Application.StartupPath + "\\unsubmit";
                            //保存表单到本地
                            CommonUtil.writeUnSubmitForm(HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8), param, coupingDir);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("提交出错,表单暂时保存在本地,错误信息:" + e.Message);
                string coupingDir = Application.StartupPath + "\\unsubmit";
                CommonUtil.writeUnSubmitForm(HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8), param, coupingDir);
            }
            finally
            {
                try
                {
                    if (submitFlag) {
                        //改变表单提交次数
                        ChangeSubmitCount();
                        //改变检验频率
                        ChangeFrequency();
                        //刷新主页面检验记录
                        IndexWindow.getForm().getThreadingProcessData();
                    }
                    //清理表单
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("表单清理数据时出错,错误信息:" + ex.Message);
                }
            }
        }
        #endregion

        #region 遍历封装提交项函数
        private void GoThroughControls(Control parContainer, List<TextBox> txtList)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControls(parContainer.Controls[index], txtList);
                }
                else
                {
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            txtList.Add((TextBox)parContainer.Controls[index]);
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
                    //如果输入框为英文输入法输入框
                    if (tb.Tag.ToString().Contains("English"))
                    {
                        //打开英文输入法
                        AlphabetKeyboardForm.inputTxt = tb;
                        AlphabetKeyboardForm.flag = 0;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        //设置输入法头部对应输入框名称
                        SetAlphaKeyboardText(tb);
                        //设置变量focusTextBoxName为当前鼠标焦点所在输入框的名称
                        //focusTextBoxName = tb.Name;
                    }
                    else
                    {
                        //判断接箍编号是否存在和视频录像是否启动（设置同上）
                        if (IsHaveCoupingNoAndStartRecordVideo())
                        {
                            NumberKeyboardForm.inputTxt = tb;
                            NumberKeyboardForm.flag = 0;
                            NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                            NumberKeyboardForm.getForm().Show();
                            NumberKeyboardForm.getForm().TopMost = true;
                            SetNumberKeyboardText(tb);
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
        private void txt_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                if (tb.Tag != null)
                {
                    //注释同上
                    if (tb.Tag.ToString().Contains("English"))
                    {
                        AlphabetKeyboardForm.inputTxt = tb;
                        AlphabetKeyboardForm.flag = 0;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        AlphabetKeyboardForm.getForm().TopMost = true;
                        //focusTextBoxName = tb.Name;
                        SetAlphaKeyboardText(tb);
                    }
                    else
                    {
                        if (IsHaveCoupingNoAndStartRecordVideo())
                        {
                            NumberKeyboardForm.inputTxt = tb;
                            NumberKeyboardForm.flag = 0;
                            NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                            NumberKeyboardForm.getForm().Show();
                            NumberKeyboardForm.getForm().TopMost = true;
                            SetNumberKeyboardText(tb);
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

        #region 点击开始扫码事件
        private void button1_Click(object sender, EventArgs e)
        {

            string btnName = this.button1.Text;
            //如果已经在扫码中，则关闭扫码
            if (btnName.Contains("结束扫码"))
            {
                try
                {
                    //关闭读码器读码
                    YYKeyenceReaderConsole.codeReaderOff();
                    this.button1.Text = "开始扫码";
                    this.lblReaderStatus.Text = "扫码完成...";
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
                    //如果读码器已连接成功
                    if (YYKeyenceReaderConsole.readerStatus == 1)
                    {
                        //开始读码器读码
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

        #region 点击开始录制视频事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text.Contains("开始录制"))
            {
                try
                {
                    //如果录像机已连接
                    if (MainWindow.recordStatus == 3)
                    {
                        //获取时间戳，做为视频文件名
                        timestamp = CommonUtil.getMesuringRecord();
                        //启动录像机
                        CommonUtil.RealTimePreview();
                        //开始录制视频
                        MainWindow.RecordVideo(timestamp);
                        //设置是表单界面点击了录制视频事件标识
                        MainWindow.isRecordClick = false;
                        this.lblVideoStatus.Text = "开始录制...";
                        this.lblVideoStatus.Text = "录像中...";
                        videosArr += timestamp + "_vcr.mp4;";
                        //开始录制视频倒计时
                        if (timer != null)
                        {
                            countTime = 0;
                            timer.Start();
                        }
                        else
                        {
                            timer = new System.Timers.Timer();
                            timer.Enabled = true;
                            timer.AutoReset = true;
                            timer.Interval = 1000;
                            timer.Elapsed += new System.Timers.ElapsedEventHandler(CountTimer);
                            timer.Start();
                        }
                        this.button2.Text = "结束录制";
                    }
                    else
                        MessagePrompt.Show("录像机暂未启动!");

                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("录制出错，错误信息:" + ex.Message);
                }
            }
            else if (this.button2.Text.Contains("结束录制"))
            {
                //如果录像机连接正常且在录像中
                if (MainWindow.recordStatus == 4) {
                    //停止录制
                    MainWindow.stopRecordVideo();
                    //停止计时器
                    timer.Stop();
                    //重置录像机页面
                    CommonUtil.RestoreSetting(true);
                    this.button2.Text = "开始录制";
                    this.lblVideoStatus.Text = "录制完成...";
                    //将录制完成的视频移到done文件夹下
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
            //如果从计时开始到现在小于15分钟
            if ((15 * 60) >= countTime)
            {
                this.lblTimer.Text = "还剩:" + ((15 * 60 - countTime) / 60) + "分" + ((15 * 60 - countTime) % 60) + "秒";
            }
            else
            {
                this.lblTimer.Text = "";
                //停止计时器
                timer.Stop();
                //终止录像
                this.EndVideoRecord();
            }
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
        private void SetNumberKeyboardText(TextBox tb)
        {
            try
            {
                //获取控件得Name属性
                string item_name =tb.Name;
                //数字键盘Title、测量项编号
                string item_title = "",item_code="";
                //如果当前鼠标焦点所在的控件的名称包含以下字符串，则先替换以下字符串，目的是取得相应的测量项编号
                if (item_name.Contains("_A_Value"))
                    item_code = item_name.Replace("_A_Value", "");
                else if (item_name.Contains("_B_Value"))
                    item_code = item_name.Replace("_B_Value", "");
                else if (item_name.Contains("_MaxA_Value"))
                    item_code = item_name.Replace("_MaxA_Value", "");
                else if (item_name.Contains("_MaxB_Value"))
                    item_code = item_name.Replace("_MaxB_Value", "");
                else if (item_name.Contains("_MinA_Value"))
                    item_code = item_name.Replace("_MinA_Value", "");
                else if (item_name.Contains("_MinB_Value"))
                    item_code = item_name.Replace("_MinB_Value", "");
                else if (item_name.Contains("_AvgA"))
                    item_code = item_name.Replace("_AvgA", "");
                else if (item_name.Contains("_AvgB"))
                    item_code = item_name.Replace("_AvgB", "");
                else if (item_name.Contains("_OvalityA"))
                    item_code = item_name.Replace("_OvalityA", "");
                else if (item_name.Contains("_OvalityB"))
                    item_code = item_name.Replace("_OvalityB", "");
                //根据输入框Name查找键盘title内容
                item_title= keyboardTitleDic[item_name];
                item_title.Replace("()","");
                //查找显示是否为必填得label控件
                Label requiredLabel=requiredMarkDic[item_code];
                if (requiredLabel != null && requiredLabel.Visible == true)
                    item_title += "[必填]";
                    NumberKeyboardForm.getForm().lblNumberTitle.Text= item_title;
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        #region 设置英文键盘Title
        private void SetAlphaKeyboardText(TextBox tb)
        {
            try
            {
                //注释同上
                string tb_name = tb.Name;
                string keyboard_title = "";
                if (tb_name.Contains("_measure_tool1")|| tb_name.Contains("_measure_tool2"))
                {
                    keyboard_title = keyboardTitleDic[tb_name];
                }
                else if (tb_name.Contains("txtCoupingNo"))
                    keyboard_title = "接箍编号";
                else if (tb_name.Contains("txtHeatNo"))
                    keyboard_title = "炉号";
                else if (tb_name.Contains("txtBatchNo"))
                    keyboard_title = "批号";
                else if (tb_name.Contains("txtMachineNo"))
                    keyboard_title = "机床号";
                else if (tb_name.Contains("txtProductionArea"))
                    keyboard_title = "产线";
                if (!string.IsNullOrEmpty(keyboard_title))
                {
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text=keyboard_title;
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 清理表单
        private void ClearForm()
        {
            //遍历所有存放测量值的输入框，然后清理输入框内容并设置输入框背景色为白色
            foreach (TextBox tb in flpTabTwoTextBoxList) {
                tb.Text ="";
                tb.BackColor = Color.White;
            }
            this.txtCoupingNo.Text = "";
            this.txtHeatNo.Text = "";
            this.txtBatchNo.Text = "";
            videosArr = "";
            //重置各测量项数据合法
            List<string> list = new List<string>();
            list.AddRange(qualifiedList.Keys);
            for (int i=0;i< list.Count;i++)
            {
                qualifiedList[list[i]] = true;
            }
        }
        #endregion

        #region 关闭窗体事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            //关闭之前判断是否关闭读码器和结束录像
            if (button2.Text.Trim() == "结束录像")
            {
                MessagePrompt.Show("录像机尚未关闭！");
            }
            else
            {
                //重置录像机页面
                CommonUtil.RestoreSetting(true);
                //清空表单测量值
                ClearForm();
                this.Hide();
            }
        }
        #endregion

        #region 判断接箍编号是否存在和视频录像是否启动
        private bool IsHaveCoupingNoAndStartRecordVideo()
        {
            return true;
            //bool flag = false;
            //if (!string.IsNullOrWhiteSpace(this.txtCoupingNo.Text) && button2.Text.Contains("结束录制"))
            //{
            //    flag = true;
            //}
            //return flag;
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
                //获取总提交次数字符串类型
                string countStr = this.lblCountSubmit.Text;
                int countSumbit = 0;
                if (!string.IsNullOrWhiteSpace(countStr))
                {
                    countSumbit = Convert.ToInt32(countStr);//提交的次数
                }
                //如果是三支全检并且点击三支全检后表单提交的次数小于3
                if (isFullInspection && fullInspectionCount < 3)
                {
                    //三支全检的次数
                    fullInspectionCount++;
                }
                else {
                    isFullInspection = false; fullInspectionCount = 0;
                }

                foreach (Label lbl in requiredMarkDic.Values)
                {
                    //如果时三支全检 并且还未达到三次
                    if (isFullInspection && fullInspectionCount < 3)
                    {
                        lbl.Visible = true;
                        continue;
                    }
                    if (lbl.Name.Contains("_lbl_Prompt"))
                    {
                        float frequency =1;
                        if (lbl.Tag != null)
                        {
                            //获取该测量项的检验频率
                            string item_frequency=measureInfoDic[lbl.Name.Replace("_lbl_Prompt", "")]["item_frequency"];
                            frequency = Convert.ToSingle(item_frequency);
                        }
                        if (countSumbit < 4)
                            lbl.Visible = true;
                        else {
                            //判断此次测量项是否必填
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

        #region 设置表单提交次数加1
        private void ChangeSubmitCount()
        {
            //获取提交的次数
            string countStr = this.lblCountSubmit.Text;
            int temp = 0;
            if (!string.IsNullOrWhiteSpace(countStr))
            {
                //设置提交表单的次数+1
                temp = Convert.ToInt32(countStr) + 1;
                this.lblCountSubmit.Text = temp.ToString();
            }
        }

        
        #endregion

        #region tabControl切换事件
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果切换到输入测量工具编号tab
            //if (this.tabControl1.SelectedIndex == 0)
            //{
            //    isMeasuringToolTabSelected = true;
            //}
            //else
            //{
            //    //否则判断是否选择了以下内容
            //    if (string.IsNullOrWhiteSpace(cmbContractNo.Text))
            //    {
            //        this.tabControl1.SelectedIndex = 0;
            //        MessagePrompt.Show("请选择合同号!");
            //    }
            //    else if (string.IsNullOrWhiteSpace(cmbProductionCrew.Text))
            //    {
            //        this.tabControl1.SelectedIndex = 0;
            //        MessagePrompt.Show("请选择班别!");
            //    }
            //    else if (string.IsNullOrWhiteSpace(cmbProductionShift.Text))
            //    {
            //        this.tabControl1.SelectedIndex = 0;
            //        MessagePrompt.Show("请选择班次!");
            //    }
            //    else if (JudgeMeasureToolsNoIsNull())
            //    {
            //        this.tabControl1.SelectedIndex = 0;
            //        MessagePrompt.Show("存在没有输入的量具编号!");
            //    }
            //    else
            //    {
            //        isMeasuringToolTabSelected = false;
            //    }
            //}

        }
        #endregion

        #region 判断量具编号是否全部输入
        public bool JudgeMeasureToolsNoIsNull()
        {
            bool flag = false;
            List<TextBox> list = new List<TextBox>();
            //遍历tab中所有的输入框，然后保存到list集合中
            GoThroughControlsMeasureInput(flpTabOneContent, list);
            //遍历list集合，如果每一个输入框值都不为空返回true,否则返回false
            foreach (TextBox tb in list)
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                    flag = true;
            }
            return flag;
        }

        
        #endregion

        #region 遍历量具编号输入框，判断是否输入量具编号
        private void GoThroughControlsMeasureInput(Control parContainer, List<TextBox> txtList)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControlsMeasureInput(parContainer.Controls[index], txtList);
                }
                else
                {
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            txtList.Add((TextBox)parContainer.Controls[index]);
                            break;
                    }
                }
            }
        }


        #endregion

        #region 3支全检
        private void btnChanger_Click(object sender, EventArgs e)
        {
            isFullInspection = true;
            fullInspectionCount = 0;
            ChangeFrequency();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            isFullInspection = true;
            fullInspectionCount = 0;
            ChangeFrequency();
        }
        #endregion

        #region 取消3支全检
        private void button4_Click(object sender, EventArgs e)
        {
            isFullInspection = false;
            fullInspectionCount = 0;
            ChangeFrequency();
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

        #region 检查必填项是否都填
        public string CheckRequired()
        {
            string flag = "";
            foreach (KeyValuePair<string, Label> kv in requiredMarkDic)
            {
                if (kv.Value.Visible == true)
                {
                    string item_code = kv.Key;
                    //找出对应的输入框，判断是否填内容
                        if (controlTxtDir.ContainsKey(item_code + "_A_Value")&& controlTxtDir[item_code + "_A_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_A_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag);
                                break;
                            }
                        }
                        if (controlTxtDir.ContainsKey(item_code + "_B_Value") && controlTxtDir[item_code + "_B_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_B_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag); break;
                            }
                        }
                        if (controlTxtDir.ContainsKey(item_code + "_MaxA_Value") && controlTxtDir[item_code + "_MaxA_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_MaxA_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag); break;
                            }
                        }
                        if (controlTxtDir.ContainsKey(item_code + "_MaxB_Value") && controlTxtDir[item_code + "_MaxB_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_MaxB_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag); break;
                            }
                        }
                        if (controlTxtDir.ContainsKey(item_code + "_MinA_Value") && controlTxtDir[item_code + "_MinA_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_MinA_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag); break;
                            }
                        }
                        if (controlTxtDir.ContainsKey(item_code + "_MinB_Value") && controlTxtDir[item_code + "_MinB_Value"] != null)
                        {
                            if (string.IsNullOrWhiteSpace(controlTxtDir[item_code + "_MinB_Value"].Text))
                            {
                                flag = Convert.ToString(kv.Value.Tag); break;
                            }
                        }
                }
            }
            if (!string.IsNullOrWhiteSpace(flag))
                flag += "不能为必填项!";
            return flag;
        }
        #endregion
    }
}
