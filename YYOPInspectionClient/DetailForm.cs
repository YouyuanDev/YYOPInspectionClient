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
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class DetailForm : Form
    {
        //定义操作工工号、螺纹检验记录编号、录制视频路径(";"拼接的字符串)、解析过后的视频路径数组
        public  string operator_no, thread_inspection_record_code,video_url, videoNoArr = "";
        //输入框集合,存放检验记录使用测量工具
        public static List<TextBox> flpTabOneTextBoxList = new List<TextBox>();
        //输入框集合,存放检验记录测量的数据
        public static List<TextBox> flpTabTwoTextBoxList = new List<TextBox>();
        //测量项编号集合
        private List<string> measureItemCodeList = new List<string>();
        //测量值输入框集合
        public static Dictionary<string, TextBox> controlTxtDir = new Dictionary<string, TextBox>();
        //定义是否测量数据是否合法的集合
        public static Dictionary<string, bool> qualifiedList = new Dictionary<string, bool>();
        //存放测量项信息得集合
        public static Dictionary<string, Dictionary<string, string>> measureInfoDic = new Dictionary<string, Dictionary<string, string>>();
        //存放必填标识"*"号得Label集合
        private Dictionary<string, Label> requiredMarkDic = new Dictionary<string, Label>();
        //存放输入法头部信息得集合
        private Dictionary<string, string> keyboardTitleDic = new Dictionary<string, string>();

        #region 构造函数
        public DetailForm(string operator_no, string thread_inspection_record_code)
        {
            InitializeComponent();
            try
            {
                //判断用户是否在线
                if (string.IsNullOrWhiteSpace(Person.pname) || string.IsNullOrWhiteSpace(Person.employee_no))
                {
                    MessagePrompt.Show("您已掉线,请重新登录!");
                    this.Close();
                    Application.Exit();
                }
                this.operator_no = operator_no;
                this.thread_inspection_record_code = thread_inspection_record_code;
                //将英文输入法中显示测量值的控件容器指定为当前测量值的控件容器
                //AlphabetKeyboardForm.getForm().containerControl = this.flpTabOneContent;
                //将数字输入法中显示测量值的控件容器指定为当前测量值的控件容器
                //NumberKeyboardForm.getForm().containerControl = this.flpTabTwoContent;
                //为机床号、产线输入框注册获取焦点的事件
                txtProductionArea.MouseDown += new MouseEventHandler(txt_MouseDown);
                txtMachineNo.MouseDown += new MouseEventHandler(txt_MouseDown);
                //如果螺纹检验记录编号和操作工工号不为空
                if (!string.IsNullOrWhiteSpace(thread_inspection_record_code) && !string.IsNullOrWhiteSpace(operator_no))
                {
                    //获取螺纹检验记录
                    GetThreadFormInitData(thread_inspection_record_code);
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("系统繁忙!");
                this.Close();
            }
        }
        #endregion

        #region 根据检验记录编号获取检验记录
        public void GetThreadFormInitData(string thread_inspection_record_code)
        {
            try
            {
                //清理集合内容
                measureItemCodeList.Clear();
                qualifiedList.Clear();
                controlTxtDir.Clear();
                flpTabOneTextBoxList.Clear();
                flpTabTwoTextBoxList.Clear();
                qualifiedList.Clear();
                measureInfoDic.Clear();
                requiredMarkDic.Clear();
                keyboardTitleDic.Clear();
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject param = new JObject {
                    { "thread_inspection_record_code",thread_inspection_record_code}
                };
                byte[] data = encoding.GetBytes(param.ToString());
                string url = CommonUtil.getServerIpAndPort() + "StaticMeasure/getMeasureDataByInspectionNoOfWinform.action";
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
                    //获取返回的数据
                    string rowsJson = jobject["rowsData"].ToString();
                    if (rowsJson.Trim().Contains("fail"))
                    {
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        MessagePrompt.Show("初始化表单失败");
                    }
                    else
                    {
                        //将json格式的数据转成json对象
                        JObject jo = (JObject)JsonConvert.DeserializeObject(rowsJson);
                        //定义合同信息、测量项信息、检验记录信息变量
                        string contractInfo = "", measureInfo = "", inspectionData = "";
                        if (jo["contractInfo"]!=null)
                           contractInfo = jo["contractInfo"].ToString();
                        if(jo["measureInfo"]!=null)
                           measureInfo = jo["measureInfo"].ToString();
                        if (jo["inspectionData"] != null)
                            inspectionData = jo["inspectionData"].ToString();
                        //填充表单合同信息
                        FillFormTitle(contractInfo);
                        //清理测量工具控件（flpTabOneContent为存放测量工具控件的FlowFlayoutPanel容器）
                        this.flpTabOneContent.Controls.Clear();
                        //清理测量值控件（flpTabOneContent为存放测量值控件的FlowFlayoutPanel容器）
                        this.flpTabTwoContent.Controls.Clear();
                        if (!string.IsNullOrEmpty(measureInfo)&&!string.IsNullOrEmpty(inspectionData)) {
                            JArray measureArr = (JArray)JsonConvert.DeserializeObject(measureInfo);
                            JArray measureDataArr = (JArray)JsonConvert.DeserializeObject(inspectionData);
                            //初始化测量项
                            InitMeasureTools(measureArr);
                            //初始化测量项和测量值
                            InitMeasureToolNoAndValue(measureDataArr);
                            //遍历测量项值得TextBox集合
                            foreach (TextBox tb in flpTabTwoTextBoxList)
                            {
                                //判断值是否符合标准，不符合则标红
                                JudgeValIsRight(tb);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.flpTabOneContent.Controls.Clear();
                this.flpTabTwoContent.Controls.Clear();
                MessagePrompt.Show("初始化表单出错,错误信息:" + e.Message);
            }
        }
        #endregion

        #region 填充表单合同信息
        private void FillFormTitle(string contractInfo)
        {
            try
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

            }
            catch (Exception ex)
            {
                MessagePrompt.Show("填充合同信息时出错,错误信息:"+ex.Message);
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
                    Label lblRequired = new Label { Tag = measure_item_name, AutoSize = true, Text = "*必填", Name = measure_item_code + "_lbl_Prompt", Location = new Point(320, 10), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Red };
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
                        Label lblA = new Label { Text = "A", Location = new Point(20, 80), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                        //Label控件用于存放测量项B端名称
                        Label lblB = new Label { Text = "B", Location = new Point(20, 120), Width = 20, TextAlign = ContentAlignment.MiddleRight };
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
                            keyboardTitleDic.Add(measure_item_code + "_A_Value", measure_item_name + "A端(" + txt + ")");
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
                        if (tbAList.Count > 0)
                        {
                            int block_width = (450 - 60) / (tbAList.Count + 1);
                            for (int i = 0; i < tbAList.Count; i++)
                            {
                                tbAList[i].Location = new Point(block_width * (i + 1) + 20, 80);
                            }
                            for (int i = 0; i < tbBList.Count; i++)
                            {
                                tbBList[i].Location = new Point(block_width * (i + 1) + 20, 120);
                            }
                            for (int i = 0; i < lblList.Count; i++)
                            {
                                lblList[i].Location = new Point(block_width * (i + 1) + 20, 40);
                            }
                            lblA.Location = new Point(block_width - 40, 80);
                            lblB.Location = new Point(block_width - 40, 120);
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
                            keyboardTitleDic.Add(measure_item_code + "_A_Value", measure_item_name + "(" + txt + ")");
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
                            keyboardTitleDic.Add(measure_item_code + "_MaxA_Value", measure_item_name + "最大值" + "(" + txt + ")");
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

        #region 初始化测量项数据
        private void InitMeasureToolNoAndValue(JArray measureDataArr)
        {
            try
            {
                //遍历获取到的检验记录数据
                foreach (var item in measureDataArr)
                {
                    JObject obj = (JObject)item;
                    string itemcode = null;
                    //获取当前测量项编号
                    itemcode = (obj["itemcode"] == null) ? "" : Convert.ToString(obj["itemcode"]);
                    //根据测量项编号找到存放测量工具1编号的TextBox控件，然后赋值
                    if (controlTxtDir.ContainsKey(itemcode + "_measure_tool1"))
                    {
                        if (obj["toolcode1"] != null)
                            controlTxtDir[itemcode + "_measure_tool1"].Text = Convert.ToString(obj["toolcode1"]);
                    }
                    //根据测量项编号找到存放测量工具2编号的TextBox控件，然后赋值
                    if (controlTxtDir.ContainsKey(itemcode + "_measure_tool2"))
                    {

                        if (obj["toolcode2"] != null)
                            controlTxtDir[itemcode + "_measure_tool2"].Text = Convert.ToString(obj["toolcode2"]);
                    }
                    //填充检测项数据
                    string[] valueArr = { }, readingMaxArr = { }, readingMinArr = { }, readingAvgArr = { }, readingOvalityArr = { };
                    string valA = "", valB = "", readingMaxA = "", readingMaxB = "", readingMinA = "", readingMinB = "", readingAvgA = "", readingAvgB = "", readingOvalityA = "", readingOvalityB = "";
                    //获取当前测量项的测量值
                    if (obj["itemvalue"] != null)
                    {
                        valueArr = Convert.ToString(obj["itemvalue"]).Split(',');
                        for (int i = 0; i < valueArr.Length; i++)
                        {
                            if (i == 0)
                                valA = valueArr[i];
                            else if (i == 1)
                                valB = valueArr[i];
                        }
                    }
                    //获取当前测量项的最大值
                    if (obj["reading_max"] != null)
                    {
                        readingMaxArr = Convert.ToString(obj["reading_max"]).Split(',');
                        for (int i = 0; i < readingMaxArr.Length; i++)
                        {
                            if (i == 0)
                                readingMaxA = readingMaxArr[i];
                            else if (i == 1)
                                readingMaxB = readingMaxArr[i];
                        }
                    }
                    //获取当前测量项的最小值
                    if (obj["reading_min"] != null)
                    {
                        readingMinArr = Convert.ToString(obj["reading_min"]).Split(',');
                        for (int i = 0; i < readingMinArr.Length; i++)
                        {
                            if (i == 0)
                                readingMinA = readingMinArr[i];
                            else if (i == 1)
                                readingMinB = readingMinArr[i];
                        }
                    }
                    //获取当前测量项的平均值
                    if (obj["reading_avg"] != null)
                    {
                        readingAvgArr = Convert.ToString(obj["reading_avg"]).Split(',');
                        for (int i = 0; i < readingAvgArr.Length; i++)
                        {
                            if (i == 0)
                                readingAvgA = readingAvgArr[i];
                            else if (i == 1)
                                readingAvgB = readingAvgArr[i];
                        }
                    }
                    //获取当前测量项的椭圆度
                    if (obj["reading_ovality"] != null)
                    {
                        readingOvalityArr = Convert.ToString(obj["reading_ovality"]).Split(',');
                        for (int i = 0; i < readingOvalityArr.Length; i++)
                        {
                            if (i == 0)
                                readingOvalityA = readingOvalityArr[i];
                            else if (i == 1)
                                readingOvalityB = readingOvalityArr[i];
                        }
                    }
                    //将A、B端的最大值、最小值、椭圆度填充到对应的控件中
                    if (controlTxtDir.ContainsKey(itemcode + "_A_Value"))
                        controlTxtDir[itemcode + "_A_Value"].Text = valA;
                    if (controlTxtDir.ContainsKey(itemcode + "_B_Value"))
                        controlTxtDir[itemcode + "_B_Value"].Text = valB;
                    if (controlTxtDir.ContainsKey(itemcode + "_MaxA_Value"))
                        controlTxtDir[itemcode + "_MaxA_Value"].Text = readingMaxA;
                    if (controlTxtDir.ContainsKey(itemcode + "_MaxB_Value"))
                        controlTxtDir[itemcode + "_MaxB_Value"].Text = readingMaxB;
                    if (controlTxtDir.ContainsKey(itemcode + "_MinA_Value"))
                        controlTxtDir[itemcode + "_MinA_Value"].Text = readingMinA;
                    if (controlTxtDir.ContainsKey(itemcode + "_MinB_Value"))
                        controlTxtDir[itemcode + "_MinB_Value"].Text = readingMinB;
                    if (controlTxtDir.ContainsKey(itemcode + "_AvgA"))
                        controlTxtDir[itemcode + "_AvgA"].Text = readingAvgA;
                    if (controlTxtDir.ContainsKey(itemcode + "_AvgB"))
                        controlTxtDir[itemcode + "_AvgB"].Text = readingAvgB;
                    if (controlTxtDir.ContainsKey(itemcode + "_OvalityA"))
                        controlTxtDir[itemcode + "_OvalityA"].Text = readingOvalityA;
                    if (controlTxtDir.ContainsKey(itemcode + "_OvalityB"))
                        controlTxtDir[itemcode + "_OvalityB"].Text = readingOvalityB;
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("初始化测量数据时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 为输入框绑定事件
        public void BindMouseEvent()
        {
            foreach (TextBox item in controlTxtDir.Values)
            {
                item.Enter += new EventHandler(txt_Enter);
                item.MouseDown += new MouseEventHandler(txt_MouseDown);
                item.Leave += new EventHandler(txt_Leave);
            }
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
                    //如果当前TextBox为英文输入法属性
                    if (tb.Tag.ToString().Contains("English"))
                    {
                        //将英文输入法中的TextBox控件执行当前控件
                        AlphabetKeyboardForm.inputTxt = tb;
                        AlphabetKeyboardForm.flag =1;
                        //将英文输入法中的输入内容设置为当前输入框的输入内容
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        //显示英文输入法
                        AlphabetKeyboardForm.getForm().Show();
                        //设置英文输入法的Title
                        SetAlphaKeyboardText(tb);
                        //设置全局鼠标所在焦点输入框名称变量为当前输入框名称
                        //focusTextBoxName = tb.Name;
                    }
                    else//如果当前TextBox为数字输入法属性
                    {
                        NumberKeyboardForm.inputTxt = tb;
                        NumberKeyboardForm.flag = 1;
                        NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        NumberKeyboardForm.getForm().Show();
                        NumberKeyboardForm.getForm().TopMost = true;
                        SetNumberKeyboardText(tb);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("鼠标获取焦点时出错!");
            }

        }
        #endregion

        #region 输入框失去焦点事件
        private void txt_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag.ToString().Contains("English"))
            {
                AlphabetKeyboardForm.getForm().Hide();
            }
            else
            {
                NumberKeyboardForm.getForm().Hide();
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
                        AlphabetKeyboardForm.inputTxt = tb;
                        AlphabetKeyboardForm.flag = 1;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        AlphabetKeyboardForm.getForm().TopMost = true;
                        //focusTextBoxName = tb.Name;
                        SetAlphaKeyboardText(tb);
                    }
                    else
                    {
                        NumberKeyboardForm.inputTxt = tb;
                        NumberKeyboardForm.flag = 1;
                        NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        NumberKeyboardForm.getForm().Show();
                        NumberKeyboardForm.getForm().TopMost = true;
                        SetNumberKeyboardText(tb);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("鼠标点击时出错!");
            }
        }

        #endregion

        #region 遍历容器中所有的控件，并将TextBox控件添加到指定的集合中
        private void GoThroughControls(Control parContainer, List<TextBox> list)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControls(parContainer.Controls[index], list);
                }
                else
                {
                    //判断该控件的类型
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            list.Add((TextBox)parContainer.Controls[index]);
                            break;
                    }
                }
            }
        }
        #endregion

        #region 提交修改事件
        private void btnFormSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(operator_no) && !string.IsNullOrWhiteSpace(thread_inspection_record_code))
                {
                    if (!string.IsNullOrWhiteSpace(Person.pname) && !string.IsNullOrWhiteSpace(Person.employee_no))
                    {
                        string inspection_time = this.lblInspectionTime.Text;
                        if (!string.IsNullOrWhiteSpace(inspection_time))
                        {
                            DateTime dt = DateTime.Parse(inspection_time);
                            if (Convert.ToSingle((DateTime.Now - dt).TotalHours.ToString()) > 12)
                                MessagePrompt.Show("已超过12小时的记录不能修改!");
                            else
                            {
                                if (operator_no.Equals(Person.employee_no))
                                    ThreadFormSubmit();
                                else
                                    MessagePrompt.Show("只能修改自己的表单数据!");
                            }
                        }
                    }
                    else
                    {
                        MessagePrompt.Show("您已掉线，请重新登录!");
                        Application.Exit();
                    }
                }
                else
                {
                    MessagePrompt.Show("系统繁忙,请稍后修改!");
                }
            }
            catch (Exception ex) {
                MessagePrompt.Show("系统出错，错误信息:"+ex.Message);
            }
        }
        #endregion

        #region 窗体关闭事件
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion

        #region 表单提交
        private void ThreadFormSubmit()
        {
            try
            {
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
                bool submitFlag = false;
                if (inspectionResult.Contains("不合格"))
                {
                    if (MessageBox.Show("数据不合格,确定要提交吗?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        submitFlag = true;
                    else
                        submitFlag = false;
                }
                if (!string.IsNullOrWhiteSpace(thread_inspection_record_code))
                {

                    if (submitFlag)
                    {
                        //封装检验数据
                        JObject dataJson = new JObject {
                    {"isAdd","edit" }, {"coupling_no",HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8) },
                    { "thread_inspection_record_code", HttpUtility.UrlEncode(thread_inspection_record_code, Encoding.UTF8) },
                    { "contract_no", HttpUtility.UrlEncode(this.tbContractNo.Text, Encoding.UTF8) },
                    { "production_line", HttpUtility.UrlEncode(txtProductionArea.Text.Trim(), Encoding.UTF8) },
                    {"machine_no", HttpUtility.UrlEncode(txtMachineNo.Text.Trim(), Encoding.UTF8) },
                    { "operator_no", HttpUtility.UrlEncode(txtOperatorNo.Text.Trim(), Encoding.UTF8)},
                    {"production_crew",HttpUtility.UrlEncode(this.cmbProductionCrew.Text, Encoding.UTF8)  },
                    { "production_shift",HttpUtility.UrlEncode(this.cmbProductionShift.Text, Encoding.UTF8) },
                     {"video_no",HttpUtility.UrlEncode(video_url, Encoding.UTF8)  },
                    { "inspection_result",HttpUtility.UrlEncode(inspectionResult, Encoding.UTF8) },
                     {"coupling_heat_no",HttpUtility.UrlEncode(this.txtHeatNo.Text, Encoding.UTF8)  },
                    { "coupling_lot_no",HttpUtility.UrlEncode(this.txtBatchNo.Text, Encoding.UTF8) },
                     { "item_record",jarray.ToString() },
                   };
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        String content = "";
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
                                MessagePrompt.Show("修改成功!");
                            }
                            else//提交失败，将表单数据保存到本地
                            {
                                MessagePrompt.Show("修改失败!");
                            }
                        }
                    }
                }
                else
                {
                    MessagePrompt.Show("修改失败!");
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("修改提交出错,错误信息:" + e.Message);
            }
            finally
            {
                try
                {
                    this.Close();
                    IndexWindow.getForm().getThreadingProcessData();
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("更新数据出错,错误信息:" + ex.Message);
                }
            }
        }
        #endregion

        #region 设置数字键盘Title
        private void SetNumberKeyboardText(TextBox tb)
        {
            try
            {
                //获取控件得Name属性
                string item_name = tb.Name;
                //数字键盘Title、测量项编号
                string item_title = "", item_code = "";
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
                item_title = keyboardTitleDic[item_name];
                item_title.Replace("()", "");
                //查找显示是否为必填得label控件
                Label requiredLabel = requiredMarkDic[item_code];
                if (requiredLabel != null && requiredLabel.Visible == true)
                    item_title += "[必填]";
                NumberKeyboardForm.getForm().lblNumberTitle.Text = item_title;
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
                if (tb_name.Contains("_measure_tool1") || tb_name.Contains("_measure_tool2"))
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
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = keyboard_title;
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 下拉框绘制

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

        #region 窗体关闭事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 窗体Visible改变事件
        private void DetailForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Person.pname))
                this.lblDetailFormTitle.Text = "现在登录的是:" + Person.pname + ",工号:" + Person.employee_no;
        }
        #endregion

        #region 判断是否标红
        private void JudgeValIsRight(TextBox inputTxt)
        {
            string inputTxtName = inputTxt.Name;
            try
            {
                //如果输入框内容为空
                if (string.IsNullOrWhiteSpace(inputTxtName))
                {
                    return;
                }
                //获取测量项编号
                //如果控件名包含以下字符则将这些字符替换为空
                if (inputTxtName.Contains("_A_Value"))
                    inputTxtName = inputTxtName.Replace("_A_Value", "");
                else if (inputTxtName.Contains("_B_Value"))
                    inputTxtName = inputTxtName.Replace("_B_Value", "");
                else if (inputTxtName.Contains("_MaxA_Value"))
                    inputTxtName = inputTxtName.Replace("_MaxA_Value", "");
                else if (inputTxtName.Contains("_MaxB_Value"))
                    inputTxtName = inputTxtName.Replace("_MaxB_Value", "");
                else if (inputTxtName.Contains("_MinA_Value"))
                    inputTxtName = inputTxtName.Replace("_MinA_Value", "");
                else if (inputTxtName.Contains("_MinB_Value"))
                    inputTxtName = inputTxtName.Replace("_MinB_Value", "");
                else if (inputTxtName.Contains("_AvgA"))
                    inputTxtName = inputTxtName.Replace("_AvgA", "");
                else if (inputTxtName.Contains("_AvgB"))
                    inputTxtName = inputTxtName.Replace("_AvgB", "");
                else if (inputTxtName.Contains("_OvalityA"))
                    inputTxtName = inputTxtName.Replace("_OvalityA", "");
                else if (inputTxtName.Contains("_OvalityB"))
                    inputTxtName = inputTxtName.Replace("_OvalityB", "");
                //找到该测量项的值范围、和椭圆度最大值
                float maxVal = 0, minVal = 0, txtVal = 0, maxOvality = 0, sdVal = 0;
                Dictionary<string, string> dic = DetailForm.measureInfoDic[inputTxtName];
                if (dic != null)
                {
                    if (CommonUtil.IsNumeric(Convert.ToString(dic["item_max_value"])))
                        maxVal = Convert.ToSingle(dic["item_max_value"]);
                    if (CommonUtil.IsNumeric(Convert.ToString(dic["item_min_value"])))
                        minVal = Convert.ToSingle(dic["item_min_value"]);
                    if (CommonUtil.IsNumeric(Convert.ToString(dic["ovality_max"])))
                        maxOvality = Convert.ToSingle(dic["ovality_max"]);
                    if (CommonUtil.IsNumeric(Convert.ToString(dic["item_std_value"])))
                        sdVal = Convert.ToSingle(dic["item_std_value"]);
                }
                if (maxVal - minVal > 0 && !string.IsNullOrWhiteSpace(inputTxt.Text.Trim()))
                {
                    //如果输入法输入的值不符合标准
                    if (txtVal < minVal || txtVal > maxVal)
                    {
                        //设置存放测量值的输入框的背景色
                        inputTxt.BackColor = Color.LightCoral;
                        //如果该集合中包含该控件的名称
                        if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                        {
                            DetailForm.qualifiedList[inputTxtName] = false;
                        }
                    }
                    else
                    {
                        inputTxt.BackColor = Color.White;
                        if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                        {
                            DetailForm.qualifiedList[inputTxtName] = true;
                        }
                    }
                }
                //找到该测量项A端、B端最大值、最小值，然后判断是否存在均值和椭圆度
                TextBox txtMaxOfA = null, txtMaxOfB = null, txtMinOfA = null, txtMinOfB = null, tbAvgOfA = null, tbOvalityA = null,
                      tbAvgOfB = null, tbOvalityB = null;
                txtMaxOfA = DetailForm.controlTxtDir[inputTxtName + "_MaxA_Value"];
                txtMaxOfB = DetailForm.controlTxtDir[inputTxtName + "_MaxB_Value"];
                txtMinOfA = DetailForm.controlTxtDir[inputTxtName + "_MinA_Value"];
                txtMinOfB = DetailForm.controlTxtDir[inputTxtName + "_MinB_Value"];
                tbAvgOfA = DetailForm.controlTxtDir[inputTxtName + "_AvgA"];
                tbAvgOfB = DetailForm.controlTxtDir[inputTxtName + "_AvgB"];
                tbOvalityA = DetailForm.controlTxtDir[inputTxtName + "_OvalityA"];
                tbOvalityB = DetailForm.controlTxtDir[inputTxtName + "_OvalityB"];
                //如果测量项A端最大值、最小值不为空
                if (txtMaxOfA != null && !string.IsNullOrWhiteSpace(txtMaxOfA.Text)
                    && txtMinOfA != null && !string.IsNullOrWhiteSpace(txtMinOfA.Text))
                {
                    //获取均值
                    float avg = ((Convert.ToSingle(txtMaxOfA.Text) + Convert.ToSingle(txtMinOfA.Text)) / 2);
                    //判断均值是否符合要求
                    if (tbAvgOfA != null)
                    {
                        //如果均值不符合标准
                        if (avg < minVal || avg > maxVal)
                        {
                            //设置显示均值的label控件的标红
                            tbAvgOfA.BackColor = Color.LightCoral;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = false;
                        }
                        else
                        {
                            tbAvgOfA.BackColor = Color.White;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = true;
                        }
                        tbAvgOfA.Text = Convert.ToString(Math.Round(avg, 2));
                    }
                    //获取该测量项显示椭圆度的TextBox控件
                    //如果显示椭圆度的label控件存在
                    if (tbOvalityA != null)
                    {
                        //计算该测量项的椭圆度
                        float ovality = (Convert.ToSingle(txtMaxOfA.Text) - Convert.ToSingle(txtMinOfA.Text)) / sdVal;
                        //如果该测量项A端椭圆度不符合标准
                        if (ovality > maxOvality || ovality < 0)
                        {
                            //同上
                            tbOvalityA.BackColor = Color.LightCoral;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = false;
                        }
                        else
                        {
                            tbOvalityA.BackColor = Color.White;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = true;
                        }
                    }
                }
                //同上
                if (txtMaxOfB != null && !string.IsNullOrWhiteSpace(txtMaxOfB.Text)
                    && txtMinOfB != null && !string.IsNullOrWhiteSpace(txtMinOfB.Text))
                {
                    float avg = ((Convert.ToSingle(txtMaxOfB.Text) + Convert.ToSingle(txtMinOfB.Text)) / 2);
                    if (tbAvgOfB != null)
                    {
                        if (avg < minVal || avg > maxVal)
                        {
                            tbAvgOfB.BackColor = Color.LightCoral;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                            {
                                DetailForm.qualifiedList[inputTxtName] = false;
                            }
                        }
                        else
                        {
                            tbAvgOfB.BackColor = Color.White;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = true;
                        }
                    }
                    //判断椭圆度是否满足要求
                    if (tbOvalityB != null)
                    {
                        float ovality = (Convert.ToSingle(txtMaxOfB.Text) - Convert.ToSingle(txtMinOfB.Text)) / sdVal;
                        if (ovality > maxOvality || ovality < 0)
                        {
                            tbOvalityB.BackColor = Color.LightCoral;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = false;
                        }
                        else
                        {
                            tbOvalityB.BackColor = Color.White;
                            if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                DetailForm.qualifiedList[inputTxtName] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("英文键盘回车时出错,错误信息:" + ex.Message);
            }
        }
        #endregion

        #region 视频查看
        private void btnBrowseVideo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(video_url))
            {
                //打开视频播放窗体
                VideoPlayer player = new VideoPlayer(video_url);
                //设置视频播放窗体最大化显示
                player.WindowState = FormWindowState.Maximized;
                //设置窗体放置最前面
                player.TopMost = true;
                player.Show();
            }
            else {
                MessagePrompt.Show("尚未找到录制的视频!");
            }
        } 
        #endregion
    }
}
