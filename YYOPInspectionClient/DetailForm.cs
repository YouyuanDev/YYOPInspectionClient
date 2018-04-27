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
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class DetailForm : Form
    {
        public string inspection_no;
        public string operator_no;
        public string thread_inspection_record_code;
        private AlphabetKeyboardForm englishKeyboard = new AlphabetKeyboardForm();
        private NumberKeyboardForm numberKeyboard = new NumberKeyboardForm();
        private List<TextBox> flpTabOneTxtList = new List<TextBox>();
        private List<TextBox> flpTabTwoTxtList = new List<TextBox>();
        public DetailForm(string operator_no,string inspection_no,string thread_inspection_record_code)
        {
            InitializeComponent();
            englishKeyboard.flpTabOneTxtList = flpTabOneTxtList;
            numberKeyboard.flpTabTwoTxtList = flpTabTwoTxtList;
            numberKeyboard.containerControl = this.flpTabTwoContent;
            this.operator_no = operator_no;
            this.inspection_no = inspection_no;
            this.thread_inspection_record_code = thread_inspection_record_code;
            try
            {
                InitContractList();
                if (!string.IsNullOrWhiteSpace(inspection_no) && !string.IsNullOrWhiteSpace(operator_no))
                {
                    GetThreadFormInitData(inspection_no);
                }
            }
            catch (Exception e) {
                MessageBox.Show("系统繁忙!");
                this.Close();
            }
        }

        #region 根据合同编号初始化检验记录表单
        public void GetThreadFormInitData(string thread_inspection_record_code)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                string json = "";
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"thread_inspection_record_code\"" + ":" + "\"" + thread_inspection_record_code + "\"");
                sb.Append("}");
                json = sb.ToString();
                JObject o = JObject.Parse(json);
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
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
                    MessageBox.Show(rowsJson);
                    if (rowsJson.Trim().Equals("fail"))
                    {
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        Console.WriteLine("初始化表单失败......");
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(rowsJson);
                        string contractInfo = jo["contractInfo"].ToString();
                        string measureInfo = jo["measureInfo"].ToString();
                        string inspectionData= jo["inspectionData"].ToString();
                        FillFormTitle(contractInfo);//填充表单合同信息
                        JArray measureArr = (JArray)JsonConvert.DeserializeObject(measureInfo);
                        JArray measureDataArr = (JArray)JsonConvert.DeserializeObject(inspectionData);
                        this.flpTabOneContent.Controls.Clear();
                        this.flpTabTwoContent.Controls.Clear();
                        //初始话测量项
                        InitMeasureTools(measureArr);
                        //填充测量项和测量值信息
                        InitMeasureToolNoAndValue(measureDataArr);
                    }
                }
            }
            catch (Exception e)
            {
                this.flpTabOneContent.Controls.Clear();
                this.flpTabTwoContent.Controls.Clear();
                Console.WriteLine("初始化表单失败......");
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
            if (!string.IsNullOrWhiteSpace(contractObj["pipe_heat_no"].ToString()))
                this.txtHeatNo.Text = contractObj["pipe_heat_no"].ToString();
            if (!string.IsNullOrWhiteSpace(contractObj["pipe_lot_no"].ToString()))
                this.txtBatchNo.Text = contractObj["pipe_lot_no"].ToString();
        }
        #endregion

        #region 初始化测量项
        private void InitMeasureTools(JArray measureArr)
        {
            //int index = 1;
            foreach (var item in measureArr)
            {
                JObject obj = (JObject)item;
                //初始化测量工具编号表单
                string measure_tool1 = obj["measure_tool1"].ToString();
                string measure_tool2 = obj["measure_tool2"].ToString();
                if (!string.IsNullOrWhiteSpace(measure_tool1) || !string.IsNullOrWhiteSpace(measure_tool1))
                {
                    Panel pnl0 = new Panel() { Width = 225, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                    Label lbl0_0 = new Label { Text = obj["measure_item_name"].ToString(), Location = new Point(30, 10), Width = 180, TextAlign = ContentAlignment.MiddleCenter };
                    pnl0.Controls.Add(lbl0_0);
                    if (!string.IsNullOrWhiteSpace(measure_tool1))
                    {
                        Label lbl0_1 = new Label { Text = obj["measure_tool1"].ToString() + ":", Location = new Point(10, 40), Width = 90, TextAlign = ContentAlignment.MiddleRight };
                        pnl0.Controls.Add(lbl0_1);
                        TextBox tb0 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool1", Location = new Point(100, 40) };
                        pnl0.Controls.Add(tb0);
                        tb0.Enter += new EventHandler(txt_Enter);
                        tb0.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tb0.Leave += new EventHandler(txt_Leave);
                    }
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        Label lbl0_2 = new Label { Text = obj["measure_tool2"].ToString() + ":", Location = new Point(10, 90), Width = 90, TextAlign = ContentAlignment.MiddleRight };
                        pnl0.Controls.Add(lbl0_2);
                        TextBox tb1 = new TextBox { Tag = "English", Name = obj["measure_item_code"].ToString() + "_measure_tool2", Location = new Point(100, 90) };
                        pnl0.Controls.Add(tb1);
                        tb1.Enter += new EventHandler(txt_Enter);
                        tb1.MouseDown += new MouseEventHandler(txt_MouseDown);
                        tb1.Leave += new EventHandler(txt_Leave);
                    }
                    this.flpTabOneContent.Controls.Add(pnl0);
                }
                //初始化测量值表单
                Panel panel1 = new Panel { Width = 225, Height = 160, BorderStyle = BorderStyle.FixedSingle };
                Label lbl1_0 = new Label { Text = obj["measure_item_name"].ToString(), Location = new Point(20, 10), Width = 180, TextAlign = ContentAlignment.MiddleCenter };
                panel1.Controls.Add(lbl1_0);
                string item_min_value = obj["item_min_value"].ToString();
                string item_max_value = obj["item_max_value"].ToString();
                string item_frequency = Convert.ToDouble(obj["item_frequency"].ToString()).ToString("P");
                ////判断该测量项是否有范围
                if (!string.IsNullOrWhiteSpace(item_min_value) && !string.IsNullOrWhiteSpace(item_max_value))
                {
                    float item_max_val = Convert.ToSingle(item_max_value);
                    float item_min_val = Convert.ToInt32(item_min_value);
                    if (item_min_val > 0 || item_max_val > 0)
                    {
                        Label lbl1_1 = new Label { Tag = item_min_val + "-" + item_max_val, Name = obj["measure_item_code"].ToString() + "_lbl", Text = "范围:{" + item_min_value + "-" + item_max_value + "}", Location = new Point(18, 50) };
                        panel1.Controls.Add(lbl1_1);
                        //添加频率
                        Label lbl1_3 = new Label { Text = "频率:" + item_frequency, Location = new Point(120, 50) };
                        panel1.Controls.Add(lbl1_3);
                    }
                    else
                    {
                        Label lbl1_2 = new Label { Text = "频率:" + item_frequency, Location = new Point(60, 50) };
                        panel1.Controls.Add(lbl1_2);
                    }
                }
                else
                {
                    //添加频率
                    Label lbl1_4 = new Label { Text = "频率:" + item_frequency, Location = new Point(60, 50) };
                    panel1.Controls.Add(lbl1_4);
                }

                //判断是否有A端B端
                if (true)
                {
                    Label lbl1_5 = new Label { Text = "A:", Location = new Point(20, 80), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                    TextBox tb3 = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(60, 80) };
                    Label lbl1_6 = new Label { Text = "B:", Location = new Point(20, 120), Width = 20, TextAlign = ContentAlignment.MiddleRight };
                    TextBox tb4 = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_B_Value", Location = new Point(60, 120) };
                    panel1.Controls.Add(lbl1_5);
                    panel1.Controls.Add(lbl1_6);
                    panel1.Controls.Add(tb3);
                    panel1.Controls.Add(tb4);
                    tb3.Enter += new EventHandler(txt_Enter);
                    tb3.MouseDown += new MouseEventHandler(txt_MouseDown);
                    tb3.Leave += new EventHandler(txt_Leave);
                    tb4.Enter += new EventHandler(txt_Enter);
                    tb4.MouseDown += new MouseEventHandler(txt_MouseDown);
                    tb4.Leave += new EventHandler(txt_Leave);
                }
                else
                {
                    TextBox tb5 = new TextBox { Tag = "Number", Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(60, 80) };
                    panel1.Controls.Add(tb5);
                    tb5.Enter += new EventHandler(txt_Enter);
                    tb5.MouseDown += new MouseEventHandler(txt_MouseDown);

                    tb5.Leave += new EventHandler(txt_Leave);
                }
                this.flpTabTwoContent.Controls.Add(panel1);
            }
            flpTabOneTxtList.Clear();
            flpTabTwoTxtList.Clear();
            GoThroughControls(this.flpTabOneContent, flpTabOneTxtList);
            GoThroughControls(this.flpTabTwoContent, flpTabTwoTxtList);
        }
        #endregion

        #region 初始化测量项数据
        private void InitMeasureToolNoAndValue(JArray measureDataArr)
        {
            try {
                foreach (var item in measureDataArr)
                {
                    JObject obj = (JObject)item;
                     
                    string itemcode = Convert.ToString(obj["itemcode"]);
                    string toolcode1 = Convert.ToString(obj["toolcode1"]);
                    string toolcode2 = Convert.ToString(obj["toolcode2"]);
                    string itemvalue = Convert.ToString(obj["itemvalue"]);
                    foreach (TextBox oneItem in flpTabOneTxtList)
                    {
                        if (oneItem.Name.Contains(itemcode))
                        {
                            if (oneItem.Name.Contains(itemcode + "_measure_tool1"))
                            {

                                if (!string.IsNullOrWhiteSpace(toolcode1))
                                {
                                    oneItem.Text = toolcode1;
                                }
                            }
                            else if (oneItem.Name.Contains(itemcode + "_measure_tool2"))
                            {
                                if (!string.IsNullOrWhiteSpace(toolcode2))
                                {
                                    oneItem.Text = toolcode2;
                                }
                            }
                        }
                    }
                    string[] sArray = itemvalue.Split(new char[2] { ';', '；' });
                    string itemValue1 = null;
                    string itemValue2 = null;
                    if (sArray.Length > 0)
                    {
                        if (sArray.Length > 1)
                        {
                            itemValue1 = sArray[0];
                            itemValue2 = sArray[1];
                        }
                        else
                        {
                            itemValue1 = sArray[0];
                        }
                    }
                    foreach (TextBox twoItem in flpTabTwoTxtList)
                    {
                        if (twoItem.Name.Contains(itemcode))
                        {
                            if (twoItem.Name.Contains(itemcode + "_A_Value"))
                            {
                                if (!string.IsNullOrWhiteSpace(itemValue1))
                                {
                                    twoItem.Text = itemValue1;
                                }
                            }
                            else if (twoItem.Name.Contains(itemcode + "_B_Value"))
                            {
                                if (!string.IsNullOrWhiteSpace(itemValue2))
                                {
                                    twoItem.Text = itemValue2;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("数据填充时出错......");
            }
        } 
        #endregion

        #region 输入框获取焦点事件
        private void txt_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag.Equals("English"))
            {
                englishKeyboard.inputTxt = tb;
                englishKeyboard.Textbox_display.Text = tb.Text.Trim();
                englishKeyboard.Show();
            }
            else
            {
                numberKeyboard.inputTxt = tb;
                numberKeyboard.Textbox_display.Text = tb.Text.Trim();
                numberKeyboard.Show();
            }
        }
        #endregion

        #region 输入框失去焦点事件
        private void txt_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag.Equals("English"))
            {
                englishKeyboard.Hide();
            }
            else
            {
                numberKeyboard.Hide();
            }
        }
        #endregion

        #region 鼠标点击输入框事件
        private void txt_MouseDown(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag.Equals("English"))
            {
                englishKeyboard.inputTxt = tb;
                englishKeyboard.Textbox_display.Text = tb.Text.Trim();
                englishKeyboard.Show();
                englishKeyboard.TopMost = true;
            }
            else
            {
                numberKeyboard.inputTxt = tb;
                numberKeyboard.Textbox_display.Text = tb.Text.Trim();
                numberKeyboard.Show();
                numberKeyboard.TopMost = true;
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
            if (operator_no.Equals(Person.employee_no))
            {
                if (!string.IsNullOrWhiteSpace(operator_no) && !string.IsNullOrWhiteSpace(inspection_no) && !string.IsNullOrWhiteSpace(thread_inspection_record_code))
                {
                    ThreadFormSubmit();
                }
                else {
                    MessageBox.Show("系统繁忙,请稍后修改!");
                }
            }
            else {
                MessageBox.Show("只能修改自己的表单数据!");
            }
          
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
                Console.WriteLine(url);
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
                    if (list != null && list.Count > 0)
                    {
                        this.cmbContractNo.DataSource = list;
                        this.cmbContractNo.ValueMember = "id";
                        this.cmbContractNo.DisplayMember = "text";
                        this.cmbContractNo.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取下拉合同号时出错......");
            }
        }
        #endregion

        #region 窗体关闭事件
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 表单提交
        private void ThreadFormSubmit()
        {
            String param = "";
            StringBuilder sb = new StringBuilder();
            try
            {

                sb.Remove(0, sb.Length);
                sb.Append("{");
                sb.Append("\"isAdd\"" + ":" + "\"" + "edit" + "\",");
                sb.Append("\"thread_inspection_record_code\"" + ":" + "\"" + HttpUtility.UrlEncode(thread_inspection_record_code, Encoding.UTF8) + "\",");
                sb.Append("\"couping_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8) + "\",");
                sb.Append("\"contract_no\"" + ":" + "\"" + HttpUtility.UrlEncode(this.cmbContractNo.SelectedValue.ToString(), Encoding.UTF8) + "\",");
                sb.Append("\"production_line\"" + ":" + "\"" + HttpUtility.UrlEncode(txtProductionArea.Text.Trim(), Encoding.UTF8) + "\",");
                sb.Append("\"machine_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtMachineNo.Text.Trim(), Encoding.UTF8) + "\",");
                //sb.Append("\"process_no\"" + ":" + "\"" + HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
                sb.Append("\"operator_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtOperatorNo.Text.Trim(), Encoding.UTF8) + "\",");
                sb.Append("\"production_crew\"" + ":" + "\"" + HttpUtility.UrlEncode(this.cmbProductionCrew.Text, Encoding.UTF8) + "\",");
                sb.Append("\"production_shift\"" + ":" + "\"" + HttpUtility.UrlEncode(this.cmbProductionShift.Text, Encoding.UTF8) + "\",");
                sb.Append("\"video_no\"" + ":" + "\"" + "" + "\",");
                //sb.Append("\"inspection_result\"" + ":" + "\"" + HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
                foreach (TextBox tb in flpTabOneTxtList)
                {
                    sb.Append("\"" + tb.Name + "\"" + ":" + "\"" + tb.Text.Trim() + "\",");
                }
                foreach (TextBox tb in flpTabTwoTxtList)
                {
                    sb.Append("\"" + tb.Name + "\"" + ":" + "\"" + tb.Text.Trim() + "\",");
                }
                string formData = sb.ToString();
                MessageBox.Show(formData);
                formData = formData.Substring(0, formData.LastIndexOf(",")) + "}";
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject o = JObject.Parse(formData);
                param = o.ToString();
                //MessageBox.Show(param);
                byte[] data = encoding.GetBytes(param);
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
                    MessageBox.Show(rowsJson);
                    if (rowsJson.Trim().Equals("success"))
                    {
                        MessageBox.Show("修改成功!");
                    }
                    else
                    {
                        MessageBox.Show("修改失败!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("修改失败!");
            }
        }
        #endregion
    }
}
