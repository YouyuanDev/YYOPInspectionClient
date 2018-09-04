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
        //public string inspection_no;
        public string operator_no, thread_inspection_record_code,video_url;
        //private AlphabetKeyboardForm englishKeyboard = new AlphabetKeyboardForm();
        //private NumberKeyboardForm numberKeyboard = new NumberKeyboardForm();
        private List<TextBox> flpTabOneTxtList = new List<TextBox>();
        private List<TextBox> flpTabTwoTxtList = new List<TextBox>();
        public string videoNoArr = "";
        public IndexWindow indexWindow = null;
        private List<string> measureItemCodeList = new List<string>();
        public static Dictionary<string, TextBox> controlTxtDir = new Dictionary<string, TextBox>();
        public static Dictionary<string, Label> controlLblDir = new Dictionary<string, Label>();
        public static bool isQualified = true;
        public static string focusTextBoxName = null;
        private string tempLblTxt = "", tempLblTxt1 = "";
        private Label tempLbl = null, tempLbl1 = null, tempLbl2 = null, tempLbl3 = null;
        #region 构造函数
        public DetailForm(string operator_no, string thread_inspection_record_code)
        {
            InitializeComponent();
            try
            {
                if (!string.IsNullOrWhiteSpace(Person.pname) && !string.IsNullOrWhiteSpace(Person.employee_no))
                {
                    NumberKeyboardForm.getForm().containerControl = this.flpTabTwoContent;
                    this.operator_no = operator_no;
                    this.thread_inspection_record_code = thread_inspection_record_code;
                    if (!string.IsNullOrWhiteSpace(thread_inspection_record_code) && !string.IsNullOrWhiteSpace(operator_no))
                    {
                        GetThreadFormInitData(thread_inspection_record_code);
                    }
                    txtProductionArea.MouseDown += new MouseEventHandler(txt_MouseDown);
                    txtMachineNo.MouseDown += new MouseEventHandler(txt_MouseDown);
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
                measureItemCodeList.Clear();
                controlTxtDir.Clear();
                controlLblDir.Clear();
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
                    if (rowsJson.Trim().Contains("fail"))
                    {
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        MessagePrompt.Show("初始化表单失败");
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(rowsJson);
                        string contractInfo = "", measureInfo = "", inspectionData = "";
                        if (jo["contractInfo"]!=null)
                           contractInfo = jo["contractInfo"].ToString();
                        if(jo["measureInfo"]!=null)
                           measureInfo = jo["measureInfo"].ToString();
                        if (jo["inspectionData"] != null)
                            inspectionData = jo["inspectionData"].ToString();

                        FillFormTitle(contractInfo);//填充表单合同信息
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        if (!string.IsNullOrEmpty(measureInfo)&&!string.IsNullOrEmpty(inspectionData)) {
                            JArray measureArr = (JArray)JsonConvert.DeserializeObject(measureInfo);
                            JArray measureDataArr = (JArray)JsonConvert.DeserializeObject(inspectionData);
                            //初始化测量项
                            InitMeasureTools(measureArr);
                            //初始化测量项和测量值
                            InitMeasureToolNoAndValue(measureDataArr);
                            GoThroughControls(this.flpTabOneContent, flpTabOneTxtList);
                            GoThroughControls(this.flpTabTwoContent, flpTabTwoTxtList);
                            AlphabetKeyboardForm.flpTabOneTxtList = flpTabOneTxtList;
                            NumberKeyboardForm.flpTabTwoTxtList = flpTabTwoTxtList;
                            foreach (TextBox tb in flpTabTwoTxtList)
                            {
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
            // this.txtHeatNo.Text = coupling_heat_no;
            // this.txtBatchNo.Text = coupling_lot_no;
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
                        Label lbl0_1 = new Label { Name = obj["measure_item_code"].ToString() + "_measure_tool1_lbl", Text = obj["measure_tool1"].ToString(), Font = new Font("宋体", 12), Location = new Point(5,40), AutoSize = false, Width = 90, Height = 50, TextAlign = ContentAlignment.MiddleLeft };
                        pnlMeasureTool.Controls.Add(lbl0_1);
                        TextBox tbTool1 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool1", Location = new Point(100, 45), Width = 200 };
                        pnlMeasureTool.Controls.Add(tbTool1);
                        controlTxtDir.Add(obj["measure_item_code"].ToString() + "_measure_tool1", tbTool1);
                        tbTool1.Enter += new EventHandler(txt_Enter);
                        tbTool1.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tbTool1.Leave += new EventHandler(txt_Leave);
                    }
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        Label lbl0_2 = new Label { Name = obj["measure_item_code"].ToString() + "_measure_tool2_lbl", Text = obj["measure_tool2"].ToString(), Location = new Point(5,90), Font = new Font("宋体", 12), AutoSize = false, Width = 90, Height = 50, TextAlign = ContentAlignment.MiddleLeft };
                        pnlMeasureTool.Controls.Add(lbl0_2);
                        TextBox tbTool2 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool2", Location = new Point(100, 95), Width = 200 };
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
                    Label lblRequired = new Label { Text = "", Name = obj["measure_item_code"].ToString() + "_lbl_Prompt", Location = new Point(220, 10), Width = 100, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Red };
                    pnlMeasureValue.Controls.Add(lblMeasureName);
                    pnlMeasureValue.Controls.Add(lblRequired);
                    //2.获取是否是两端检验
                    string both_ends = "0";
                    if (obj["both_ends"] != null)
                        both_ends = obj["both_ends"].ToString();
                    string item_min_value = "", item_max_value = "", item_frequency = "", item_pos_deviation_value = "", item_neg_deviation_value = "", ovality_max = "", item_std_value = "";
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
                    Label lblRangeFrequencyOvality = new Label();
                    float item_max_val = 0, item_min_val = 0, pos_deviation_value = 0, neg_deviation_value = 0;
                    if (!string.IsNullOrWhiteSpace(item_pos_deviation_value))
                        pos_deviation_value = Convert.ToSingle(item_pos_deviation_value);
                    if (!string.IsNullOrWhiteSpace(item_neg_deviation_value))
                        neg_deviation_value = Convert.ToSingle(item_neg_deviation_value);
                    string rangeFrequencyOvalitySdVal = item_max_value + "," + item_min_value + "," + item_frequency + "," + ovality_max + "," + item_std_value;
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
                            if (Math.Abs(pos_deviation_value) - Math.Abs(neg_deviation_value) <= 0.00001)
                                lblRangeFrequencyOvality.Text = "±" + pos_deviation_value + "/" + item_frequency;
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
                        Label lblAvgA = new Label { Tag = "lblVal", Text = "", Location = new Point(290, 80), Name = obj["measure_item_code"].ToString() + "_AvgA" };
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
                            if (both_ends.Contains("1"))
                            {
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
                            if (both_ends.Contains("1"))
                            {
                                pnlMeasureValue.Controls.Add(lblOvalityB);
                                controlLblDir.Add(obj["measure_item_code"].ToString() + "_OvalityB", lblOvalityB);
                            }

                        }
                    }
                    this.flpTabTwoContent.Controls.Add(pnlMeasureValue);
                }
            }
        }
        #endregion

        #region 初始化测量项数据
        private void InitMeasureToolNoAndValue(JArray measureDataArr)
        {
            try
            {
                foreach (var item in measureDataArr)
                {
                    JObject obj = (JObject)item;
                    string itemcode = null;
                    itemcode = (obj["itemcode"] == null) ? "" : Convert.ToString(obj["itemcode"]);
                    //填充测量工具编号
                    if (controlTxtDir.ContainsKey(itemcode + "_measure_tool1"))
                    {
                        if (obj["toolcode1"] != null)
                            controlTxtDir[itemcode + "_measure_tool1"].Text = Convert.ToString(obj["toolcode1"]);
                    }
                    if (controlTxtDir.ContainsKey(itemcode + "_measure_tool2"))
                    {

                        if (obj["toolcode2"] != null)
                            controlTxtDir[itemcode + "_measure_tool2"].Text = Convert.ToString(obj["toolcode2"]);
                    }
                    //填充检测项数据
                    string[] valueArr = { }, readingMaxArr = { }, readingMinArr = { }, readingAvgArr = { }, readingOvalityArr = { };
                    string valA = "", valB = "", readingMaxA = "", readingMaxB = "", readingMinA = "", readingMinB = "", readingAvgA = "", readingAvgB = "", readingOvalityA = "", readingOvalityB = "";
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
                    if (controlLblDir.ContainsKey(itemcode + "_AvgA"))
                        controlLblDir[itemcode + "_AvgA"].Text = readingAvgA;
                    if (controlLblDir.ContainsKey(itemcode + "_AvgB"))
                        controlLblDir[itemcode + "_AvgB"].Text = readingAvgB;
                    if (controlLblDir.ContainsKey(itemcode + "_OvalityA"))
                        controlLblDir[itemcode + "_OvalityA"].Text = readingOvalityA;
                    if (controlLblDir.ContainsKey(itemcode + "_OvalityB"))
                        controlLblDir[itemcode + "_OvalityB"].Text = readingOvalityB;
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("初始化测量数据时出错,错误信息:" + ex.Message);
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
                        NumberKeyboardForm.getForm().inputTxt = tb;
                        NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        NumberKeyboardForm.getForm().Show();
                        NumberKeyboardForm.getForm().TopMost = true;
                        SetNumberKeyboardText(tb.Name);
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
                        AlphabetKeyboardForm.getForm().inputTxt = tb;
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        AlphabetKeyboardForm.getForm().Show();
                        AlphabetKeyboardForm.getForm().TopMost = true;
                        focusTextBoxName = tb.Name;
                        SetAlphaKeyboardText(tb.Name);
                    }
                    else
                    {
                        NumberKeyboardForm.getForm().inputTxt = tb;
                        NumberKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                        NumberKeyboardForm.getForm().Show();
                        NumberKeyboardForm.getForm().TopMost = true;
                        SetNumberKeyboardText(tb.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("鼠标点击时出错!");
            }
        }

        #endregion

        #region 遍历封装提交项函数
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
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            list.Add((TextBox)parContainer.Controls[index]);
                            //sb.Append("\""+ parContainer.Controls[index].Name+ "\"" + ":" + "\""+ HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
                            //parContainer.Controls[index].Text = "";
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

        //#region 初始化表单合同数组
        //public void InitContractList()
        //{
        //    try
        //    {
        //        ASCIIEncoding encoding = new ASCIIEncoding();
        //        String content = "";
        //        //JObject o = JObject.Parse(sb.ToString());
        //        String param = "";
        //        byte[] data = encoding.GetBytes(param);
        //        string url = CommonUtil.getServerIpAndPort() + "Contract/getAllDropDownContractNoOfWinform.action";
        //        Console.WriteLine(url);
        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.KeepAlive = false;
        //        request.Method = "POST";
        //        request.ContentType = "application/json;characterSet:UTF-8";
        //        request.ContentLength = data.Length;
        //        using (Stream sm = request.GetRequestStream())
        //        {
        //            sm.Write(data, 0, data.Length);
        //        }
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        Stream streamResponse = response.GetResponseStream();
        //        StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
        //        Char[] readBuff = new Char[1024];
        //        int count = streamRead.Read(readBuff, 0, 1024);
        //        while (count > 0)
        //        {
        //            String outputData = new String(readBuff, 0, count);
        //            content += outputData;
        //            count = streamRead.Read(readBuff, 0, 1024);
        //        }
        //        response.Close();
        //        string jsons = content;
        //        if (jsons != null)
        //        {
        //            JObject jobject = JObject.Parse(jsons);
        //            string rowsJson = jobject["rowsData"].ToString();
        //            List<ComboxItem> list = JsonConvert.DeserializeObject<List<ComboxItem>>(rowsJson);
        //            if (list != null && list.Count > 0)
        //            {
        //                this.cmbContractNo.DataSource = list;
        //                this.cmbContractNo.ValueMember = "id";
        //                this.cmbContractNo.DisplayMember = "text";
        //                this.cmbContractNo.SelectedIndex = 0;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("获取下拉合同号时出错......");
        //    }
        //}
        //#endregion

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
                string itemvalue = "", reading_max = "", reading_min = "", reading_avg = "", reading_ovality = "", toolcode1 = "", toolcode2 = "", measure_sample1 = "", measure_sample2 = "";
                JArray jarray = new JArray();
                foreach (string measure_item_code in measureItemCodeList)
                {
                    if (controlTxtDir.ContainsKey(measure_item_code + "_A_Value"))
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
                        reading_avg += controlLblDir[measure_item_code + "_AvgA"].Text.Trim() + ",";
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
                    itemvalue = ""; reading_max = ""; reading_min = ""; reading_avg = ""; reading_ovality = "";
                    toolcode1 = ""; toolcode2 = ""; measure_sample1 = ""; measure_sample2 = "";
                }
                string inspectionResult = "";
                if (isQualified)
                    inspectionResult = "合格";
                else
                    inspectionResult = "不合格";
                JObject dataJson = new JObject {
                    {"isAdd","add" }, {"coupling_no",HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8) },
                    {"contract_no", HttpUtility.UrlEncode(this.tbContractNo.Text, Encoding.UTF8) },
                    { "production_line", HttpUtility.UrlEncode(txtProductionArea.Text.Trim(), Encoding.UTF8) },
                    {"machine_no", HttpUtility.UrlEncode(txtMachineNo.Text.Trim(), Encoding.UTF8) },
                    { "operator_no", HttpUtility.UrlEncode(txtOperatorNo.Text.Trim(), Encoding.UTF8)},
                    {"production_crew",HttpUtility.UrlEncode(this.cmbProductionCrew.Text, Encoding.UTF8)  },
                    { "production_shift",HttpUtility.UrlEncode(this.cmbProductionShift.Text, Encoding.UTF8) },
                     {"video_no",HttpUtility.UrlEncode(videoNoArr, Encoding.UTF8)  },
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
                        MessagePrompt.Show("修改成功!");
                    }
                    else
                    {
                        MessagePrompt.Show("系统繁忙,请稍后重试!");
                    }
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
                    indexWindow.getThreadingProcessData();
                }
                catch (Exception ex)
                {
                    MessagePrompt.Show("更新数据出错,错误信息:" + ex.Message);
                }
            }
        }
        #endregion

        #region 窗体大小改变事件
        private void DetailForm_Load(object sender, EventArgs e)
        {
            //auto.controllInitializeSize(this);
        }

        private void DetailForm_SizeChanged(object sender, EventArgs e)
        {
            //if(detailForm!=null)
            //  auto.controlAutoSize(detailForm);
        }
        #endregion

        #region 设置数字键盘Title
        private void SetNumberKeyboardText(string inputTxtName)
        {
            try
            {
                if (inputTxtName.Contains("_A_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_A_Value", "");
                    tempLblTxt1 = "A端";
                }
                else if (inputTxtName.Contains("_B_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_B_Value", "");
                    tempLblTxt1 = "B端";
                }
                else if (inputTxtName.Contains("_MaxA_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_MaxA_Value", "");
                    tempLblTxt1 = "最大值A端";
                }
                else if (inputTxtName.Contains("_MaxB_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_MaxB_Value", "");
                    tempLblTxt1 = "最大值B端";
                }
                else if (inputTxtName.Contains("_MinA_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_MinA_Value", "");
                    tempLblTxt1 = "最小值A端";
                }
                else if (inputTxtName.Contains("_MinB_Value"))
                {
                    inputTxtName = inputTxtName.Replace("_MinB_Value", "");
                    tempLblTxt1 = "最小值B端";
                }
                tempLbl1 = (Label)GetControlInstance(flpTabTwoContent, inputTxtName + "_lbl_Name");
                tempLbl2 = (Label)GetControlInstance(flpTabTwoContent, inputTxtName + "_lbl_Prompt");
                tempLbl3 = (Label)GetControlInstance(flpTabTwoContent, inputTxtName + "_RangeFrequencyOvality_lbl");
                if (tempLbl1 != null)
                    NumberKeyboardForm.getForm().lblNumberTitle.Text = tempLbl1.Text;
                NumberKeyboardForm.getForm().lblNumberTitle.Text += tempLblTxt1;
                if (tempLbl3 != null)
                    NumberKeyboardForm.getForm().lblNumberTitle.Text += "(" + tempLbl3.Text + ")";
                if (tempLbl2 != null && tempLbl2.Visible == true)
                    NumberKeyboardForm.getForm().lblNumberTitle.Text += "[必填]";
            }
            catch (Exception ex)
            {
                Console.WriteLine("设置输入法头部信息时出错,错误信息:" + ex.Message);
            }
            finally
            {
                tempLblTxt1 = "";
                tempLbl1 = null;
                tempLbl2 = null;
                tempLbl3 = null;
            }
        }
        #endregion

        #region 设置英文键盘Title
        private void SetAlphaKeyboardText(string inputTxtName)
        {
            try
            {
                if (inputTxtName.Contains("_measure_tool1"))
                {
                    inputTxtName = inputTxtName.Replace("_measure_tool1", "");
                    tempLbl = (Label)GetControlInstance(flpTabOneContent, inputTxtName + "_measure_tool1_lbl");
                    if (tempLbl != null)
                        tempLblTxt = tempLbl.Text;
                }
                else if (inputTxtName.Contains("_measure_tool2"))
                {
                    inputTxtName = inputTxtName.Replace("_measure_tool2", "");
                    tempLbl = (Label)GetControlInstance(flpTabOneContent, inputTxtName + "_measure_tool2_lbl");
                    if (tempLbl != null)
                        tempLblTxt = tempLbl.Text;
                }
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
                {
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = lbl.Text;
                }
                if (!string.IsNullOrEmpty(tempLblTxt))
                {
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text += "(" + tempLblTxt + ")";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("设置输入法头部信息时出错,错误信息:" + ex.Message);
            }
            finally {
                tempLbl = null;
                tempLblTxt = "";
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
                if (!string.IsNullOrWhiteSpace(inputTxtName))
                {
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
                    //找到该测量项的值范围、和椭圆度最大值
                    float maxVal = 0, minVal = 0, txtVal = 0, maxOvality = 0, sdVal = 0;
                    Label lblRangeFrequencyOvality = (Label)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_RangeFrequencyOvality_lbl");
                    if (lblRangeFrequencyOvality != null)
                    {
                        if (lblRangeFrequencyOvality.Tag != null)
                        {
                            if (!string.IsNullOrWhiteSpace(lblRangeFrequencyOvality.Tag.ToString()))
                            {
                                string[] rangeFrequency = lblRangeFrequencyOvality.Tag.ToString().Split(',');
                                maxVal = Convert.ToSingle(rangeFrequency[0]);
                                minVal = Convert.ToSingle(rangeFrequency[1]);
                                txtVal = Convert.ToSingle(inputTxt.Text.Trim());
                                if (!string.IsNullOrWhiteSpace(rangeFrequency[3]))
                                    maxOvality = Convert.ToSingle(rangeFrequency[3]);
                                if (!string.IsNullOrWhiteSpace(rangeFrequency[4]))
                                    sdVal = Convert.ToSingle(rangeFrequency[4]);
                                if (maxVal - minVal > 0.00001)
                                {
                                    if (txtVal < minVal || txtVal > maxVal)
                                    {
                                        inputTxt.BackColor = Color.LightCoral;
                                        ThreadingForm.isQualified = false;
                                    }
                                    else
                                        inputTxt.BackColor = Color.White;
                                }
                            }
                            //找到最大值、最小值，然后判断是否存在均值和椭圆度
                            TextBox txtMaxOfA = (TextBox)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_MaxA_Value");
                            TextBox txtMaxOfB = (TextBox)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_MaxB_Value");
                            TextBox txtMinOfA = (TextBox)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_MinA_Value");
                            TextBox txtMinOfB = (TextBox)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_MinB_Value");
                            if (txtMaxOfA != null && txtMinOfA != null)
                            {
                                if (!string.IsNullOrWhiteSpace(txtMaxOfA.Text) && !string.IsNullOrWhiteSpace(txtMinOfA.Text))
                                {
                                    float avg = ((Convert.ToSingle(txtMaxOfA.Text) + Convert.ToSingle(txtMinOfA.Text)) / 2);
                                    Label lblAvgOfA = (Label)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_AvgA");
                                    //判断均值是否符合要求
                                    if (lblAvgOfA != null)
                                    {
                                        if (avg < minVal || avg > maxVal)
                                        {
                                            lblAvgOfA.ForeColor = Color.Red;
                                            ThreadingForm.isQualified = false;
                                        }
                                        else
                                            lblAvgOfA.ForeColor = Color.Black;
                                        lblAvgOfA.Text = Convert.ToString(Math.Round(avg, 2));
                                    }
                                    Label lblOvalityA = (Label)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_OvalityA");
                                    //判断椭圆度是否满足要求
                                    if (lblOvalityA != null)
                                    {
                                        float ovality = (Convert.ToSingle(txtMaxOfA.Text) - Convert.ToSingle(txtMinOfA.Text)) / sdVal;
                                        if (ovality > maxOvality || ovality < 0)
                                        {
                                            lblOvalityA.ForeColor = Color.Red;
                                            ThreadingForm.isQualified = false;
                                        }
                                        else
                                            lblOvalityA.ForeColor = Color.Black;
                                        lblOvalityA.Text = Convert.ToString(Math.Round(ovality, 2));
                                    }
                                }
                            }
                            if (txtMaxOfB != null && txtMinOfB != null)
                            {
                                if (!string.IsNullOrWhiteSpace(txtMaxOfB.Text) && !string.IsNullOrWhiteSpace(txtMinOfB.Text))
                                {
                                    float avg = ((Convert.ToSingle(txtMaxOfB.Text) + Convert.ToSingle(txtMinOfB.Text)) / 2);
                                    Label lblAvgOfB = (Label)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_AvgB");
                                    if (lblAvgOfB != null)
                                    {
                                        if (avg < minVal || avg > maxVal)
                                        {
                                            lblAvgOfB.ForeColor = Color.Red;
                                            ThreadingForm.isQualified = false;
                                        }
                                        else
                                            lblAvgOfB.ForeColor = Color.Black;
                                        lblAvgOfB.Text = Convert.ToString(Math.Round(avg, 2));
                                    }
                                    Label lblOvalityB = (Label)GetControlInstance(this.flpTabTwoContent, inputTxtName + "_OvalityB");
                                    //判断椭圆度是否满足要求
                                    if (lblOvalityB != null)
                                    {
                                        float ovality = (Convert.ToSingle(txtMaxOfB.Text) - Convert.ToSingle(txtMinOfB.Text)) / sdVal;
                                        if (ovality > maxOvality || ovality < 0)
                                        {
                                            lblOvalityB.ForeColor = Color.Red;
                                            ThreadingForm.isQualified = false;
                                        }
                                        else
                                            lblOvalityB.ForeColor = Color.Black;
                                        lblOvalityB.Text = Convert.ToString(Math.Round(ovality, 2));
                                    }
                                }
                            }
                        }
                    }

                }
                //跳转到下一个输入框
                int index = flpTabTwoTxtList.IndexOf(inputTxt);
                if (index < flpTabTwoTxtList.Count - 1)
                    index++;
                TextBox tb = flpTabTwoTxtList[index];
                if (tb != null)
                    tb.Focus();
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
                VideoPlayer player = new VideoPlayer(video_url);
                player.WindowState = FormWindowState.Maximized;
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
