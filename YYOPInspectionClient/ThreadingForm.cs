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
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class ThreadingForm : Form
    {
        //防止合同combobox自动执行选中事件
        private bool flag = false;
        private StringBuilder sb = new StringBuilder();
        private AlphabetKeyboardForm englishKeyboard = new AlphabetKeyboardForm();
        private NumberKeyboardForm numberKeyboard = new NumberKeyboardForm();
        private List<TextBox> flpTabOneTxtList = new List<TextBox>();
        private List<TextBox> flpTabTwoTxtList = new List<TextBox>();
        private IndexWindow indexWindow;
        private MainWindow mainWindow;
        // YYKeyenceReaderConsole codeReaderWindow;
        AutoSize auto = new AutoSize();
        //时间戳(视频和form表单保存的目录名)
        private string timestamp = null;

        private static ThreadingForm myForm = null;
        public static ThreadingForm getMyForm()
        {
            return myForm;
        }

        public ThreadingForm(IndexWindow indexWindow, MainWindow mainWindow)
        {
            InitializeComponent();
            englishKeyboard.flpTabOneTxtList = flpTabOneTxtList;
            numberKeyboard.flpTabTwoTxtList = flpTabTwoTxtList;
            numberKeyboard.containerControl = this.flpTabTwoContent;
            this.indexWindow = indexWindow;
            this.mainWindow = mainWindow;
            timestamp =CommonUtil.getMesuringRecord();
            this.lblVideoStatus.Text = "录像未启动...";
            myForm = this;
            //this.Font = new Font("宋体", 15, FontStyle.Bold);
            //1------------初始化合同Combobox
            InitContractList();
            flag = true;
            //2------------初始化检验记录表单
            InitThreadForm();
        }

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
                string url = CommonUtil.getServerIpAndPort()+"Contract/getAllDropDownContractNoOfWinform.action";
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
                    if (list != null && list.Count > 0){ 
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

        #region 初始化检验记录表单
        public void InitThreadForm()
        {
            if (this.cmbContractNo.Text.Trim().Length != 0)
            {
                string contract_no = this.cmbContractNo.SelectedValue.ToString();
                GetThreadFormInitData(contract_no);
            }
        } 
        #endregion

        #region 根据合同编号初始化检验记录表单
        public void GetThreadFormInitData(string contract_no)
        {
            try {
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
                        Console.WriteLine("初始化表单失败......");
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
            } catch (Exception e) {
                this.flpTabOneContent.Controls.Clear();
                this.flpTabTwoContent.Controls.Clear();
                Console.WriteLine("初始化表单失败......");
            }
        }
        #endregion

        #region 合同编号选中事件

        private void cmbContractNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag) {
                ClearCntrValue(this);
               // ClearCntrValue(this.pnlTabOneFooter);
                string contract_no = this.cmbContractNo.SelectedValue.ToString();
                GetThreadFormInitData(contract_no);
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
                this.lblThreadType.Text ="螺纹类型:"+contractObj["threading_type"].ToString();
            }
            else {
                this.lblThreadType.Text = "";
            }
            if (!string.IsNullOrWhiteSpace(contractObj["od"].ToString()))
            {
                this.txtOdDiameter.Text = contractObj["od"].ToString();
                this.lblOd.Text ="外径:"+contractObj["od"].ToString();
            }
            else {
                this.lblOd.Text = "";
            }
            if (!string.IsNullOrWhiteSpace(contractObj["wt"].ToString()))
            {
                this.txtTreadWt.Text = contractObj["wt"].ToString();
                this.lblWt.Text = "壁厚:"+ contractObj["wt"].ToString();
            }
            else {
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
            if (!string.IsNullOrWhiteSpace(Person.employee_no))
                this.txtOperatorNo.Text = Person.employee_no;
            if (!string.IsNullOrWhiteSpace(Person.pname))
                this.txtOperatorName.Text = Person.pname;
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
                        Label lbl0_1 = new Label {Text=obj["measure_tool1"].ToString()+":",Location=new Point(10,40),Width=90,TextAlign=ContentAlignment.MiddleRight };
                        pnl0.Controls.Add(lbl0_1);
                        TextBox tb0 = new TextBox { Tag="English",Name = obj["measure_item_code"].ToString() + "_measure_tool1", Location = new Point(100, 40) };
                        pnl0.Controls.Add(tb0);
                        tb0.Enter += new EventHandler(txt_Enter);
                        tb0.MouseDown+=new MouseEventHandler(txt_MouseDown);
                        tb0.Leave += new EventHandler(txt_Leave);
                    }
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        Label lbl0_2 = new Label { Text = obj["measure_tool2"].ToString() + ":", Location = new Point(10, 90),Width = 90, TextAlign = ContentAlignment.MiddleRight };
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
                    float item_min_val = Convert.ToSingle(item_min_value);
                    if (item_min_val > 0 || item_max_val > 0)
                    {
                        Label lbl1_1 = new Label {Tag=item_min_val+"-"+item_max_val, Name = obj["measure_item_code"].ToString()+"_lbl",Text = "范围:{" + item_min_value + "-" + item_max_value + "}", Location = new Point(18, 50) };
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
                if(Convert.ToInt32(obj["both_ends"].ToString())==1)
                {
                    Label lbl1_5 = new Label {Text="A:",Location=new Point(20,80),Width=20, TextAlign = ContentAlignment.MiddleRight };
                    TextBox tb3 = new TextBox { Tag="Number",Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(60, 80) };
                    Label lbl1_6 = new Label { Text="B:", Location = new Point(20,120), Width = 20,TextAlign=ContentAlignment.MiddleRight };
                    TextBox tb4 = new TextBox { Tag = "Number",Name = obj["measure_item_code"].ToString() + "_B_Value", Location = new Point(60, 120) };
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
            GoThroughControls(this.flpTabOneContent,flpTabOneTxtList);
            GoThroughControls(this.flpTabTwoContent, flpTabTwoTxtList);
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
            if (button2.Text.Trim() == "结束录像" || button1.Text.Trim() == "结束扫码")
            {
                MessageBox.Show("录像机或读码器尚未关闭！");
            }
            else
            {
                ThreadFormSubmit();
            }
        }
        #endregion

        #region 表单关闭事件
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            //关闭之前判断是否关闭读码器和结束录像
            if (button2.Text.Trim() == "结束录像" || button1.Text.Trim() == "结束扫码")
            {
                MessageBox.Show("录像机或读码器尚未关闭！");
            }
            else
            {
                RestoreSetting();
                this.Close();
            }
        }
        #endregion

        #region 表单提交
        private void ThreadFormSubmit()
        {
            String param = "";
            try {
                sb.Remove(0, sb.Length);
                sb.Append("{");
                sb.Append("\"isAdd\"" + ":" + "\"" + "add" + "\",");
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
                    if (rowsJson.Trim().Equals("success"))
                    {
                        MessageBox.Show("提交成功!");
                    }
                    else
                    {
                        MessageBox.Show("提交失败,表单暂时保存在本地!");
                    }
                }
            } catch (Exception e) {
                string coupingDir = Application.StartupPath + "\\unsubmit";
                CommonUtil.writeUnSubmitForm(HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8), param, coupingDir);
            }
            finally{
                //向可提交的视频文件中追加可提交文件夹名
                //string path = Application.StartupPath + "\\fileuploadrecord.txt";
                //FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                //if (timestamp != null && timestamp.Length > 0)
                //{
                //    sw.WriteLine(timestamp);
                //}
                //sw.Close();
                //fs.Close();
                indexWindow.getThreadingProcessData();
            }
            
        } 
        #endregion

        #region 遍历封装提交项函数
        private void GoThroughControls(Control parContainer,List<TextBox>list)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControls(parContainer.Controls[index],list);
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

        #region 时间改变事件
        private void dtpInspectionTime_ValueChanged(object sender, EventArgs e)
        {
            this.dtpInspectionTime.Value = DateTime.Now;
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

        #region 扫码事件
        private void button1_Click(object sender, EventArgs e)
        {
            string btnName = this.button1.Text;
            if (btnName == "结束扫码")
            {
                //YYKeyenceReaderConsole
                this.lblReaderStatus.Text = "读码器连接成功...";
                YYKeyenceReaderConsole.codeReaderOff();
                this.button1.Text = "开始扫码";
                this.lblReaderStatus.Text = "读码完成...";
            }
            else if (btnName == "开始扫码")
            {
                //先判断是否连接上读码器，如果没有连接上则提示
                int resLon = YYKeyenceReaderConsole.codeReaderLon();
                //读码器已经连接上
                if (resLon == 0)
                {
                    //然后开启循环读取数据
                    this.lblReaderStatus.Text = "读取中...";
                    YYKeyenceReaderConsole.threadingProcessForm = this;
                    this.button1.Text = "结束扫码";
                }
                else if (resLon == 1)
                {
                    this.lblReaderStatus.Text = "读码器尚未连接...";
                    MessageBox.Show("请检查读码器是否连接或已经断开连接!");
                }
                else
                {
                    this.lblReaderStatus.Text = "读码器尚未连接...";
                    MessageBox.Show("请检查读码器是否连接或已经断开连接!");
                }
            }
        }
        #endregion

        #region 录制视频事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text == "开始录像")
            {
                if (timestamp == null || timestamp.Length <= 0)
                {
                    timestamp =CommonUtil.getMesuringRecord();
                }
                this.lblVideoStatus.Text = "开始录像...";
                int result = MainWindow.RecordVideo(timestamp);
                switch (result)
                {
                    case 0:
                        this.lblVideoStatus.Text = "录像中...";
                        RealTimePreview();
                        this.button2.Text = "结束录像";
                        break;
                    case 1:
                        this.lblVideoStatus.Text = "录像机未连接...";
                        MessageBox.Show("录像失败,请先登录录像机!");
                        break;
                    case 2:
                        this.lblVideoStatus.Text = "录像机未启动...";
                        MessageBox.Show("录像失败,请先开启录像机预览!");
                        break;
                    case 3:
                        this.lblVideoStatus.Text = "录像失败...";
                        MessageBox.Show("录像失败,请检查配置!");
                        break;
                    case 4:
                        MessageBox.Show("录像失败!");
                        break;
                }
            }
            else if (this.button2.Text == "结束录像")
            {
                MainWindow.stopRecordVideo();
                RestoreSetting();
                this.button2.Text = "开始录像";
                this.lblVideoStatus.Text = "录像完成...";
            }
        }
        #endregion

        public void RealTimePreview()
        {
            MainWindow mainWindow = MainWindow.mainWindowForm;
            if (mainWindow != null)
            {
                mainWindow.groupBox1.Hide(); mainWindow.groupBox2.Hide();
                mainWindow.groupBox3.Hide(); mainWindow.groupBox4.Hide();
                int width = mainWindow.Width;
                int height = mainWindow.Height;
                mainWindow.RealPlayWnd.Left = 0;
                mainWindow.RealPlayWnd.Top = 0;
                mainWindow.RealPlayWnd.Width = width;
                mainWindow.RealPlayWnd.Height = height;
                mainWindow.RealPlayWnd.Dock = DockStyle.Fill;
                mainWindow.Width = 150;
                mainWindow.Height = 150;
                int x = Screen.PrimaryScreen.WorkingArea.Width - mainWindow.RealPlayWnd.Width - 10;
                int y = Screen.PrimaryScreen.WorkingArea.Height / 2 - mainWindow.RealPlayWnd.Height;
                mainWindow.Location = new Point(x, y);
                mainWindow.FormBorderStyle = FormBorderStyle.None;
                mainWindow.Show();
                mainWindow.TopMost = true;
            }
        }
        public void RestoreSetting()
        {
            MainWindow mainWindow = MainWindow.mainWindowForm;
            if (mainWindow != null)
            {
                mainWindow.Left = MainWindow.mainWindowX;
                mainWindow.Top = MainWindow.mainWindowY;
                mainWindow.Width = MainWindow.mainWindowWidth;
                mainWindow.Height = MainWindow.mainWindowHeight;
                mainWindow.RealPlayWnd.Left = MainWindow.realTimeX;
                mainWindow.RealPlayWnd.Top = MainWindow.realTimeY;
                mainWindow.RealPlayWnd.Width = MainWindow.realTimeWidth;
                mainWindow.RealPlayWnd.Height = MainWindow.realTimeHeigh;
                mainWindow.RealPlayWnd.Dock = DockStyle.None;
                mainWindow.groupBox1.Show();
                mainWindow.groupBox2.Show();
                mainWindow.groupBox3.Show();
                mainWindow.groupBox4.Show();
                mainWindow.TopMost = false;
                mainWindow.FormBorderStyle = FormBorderStyle.Sizable;
                mainWindow.Hide();
            }
        }
    }
}
