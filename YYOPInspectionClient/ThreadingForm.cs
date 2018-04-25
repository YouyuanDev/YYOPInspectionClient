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
        public ThreadingForm()
        {
            InitializeComponent();
             

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
                        TextBox tb0 = new TextBox { Name = obj["measure_item_code"].ToString() + "_measure_tool1", Location = new Point(60, 40) };
                        pnl0.Controls.Add(tb0);
                    }
                    if (!string.IsNullOrWhiteSpace(measure_tool2))
                    {
                        TextBox tb1 = new TextBox { Name = obj["measure_item_code"].ToString() + "_measure_tool2", Location = new Point(60, 90) };
                        pnl0.Controls.Add(tb1);
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
                        Label lbl1_1 = new Label { Text = "范围:{" + item_min_value + "-" + item_max_value + "}", Location = new Point(18, 50) };
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

                //判断是否是A端B端
                if (true)
                {
                    TextBox tb3 = new TextBox { Name = obj["measure_item_code"].ToString() + "_A_Value", Location = new Point(60, 80) };
                    TextBox tb4 = new TextBox { Name = obj["measure_item_code"].ToString() + "_B_Value", Location = new Point(60, 120) };
                    panel1.Controls.Add(tb3);
                    panel1.Controls.Add(tb4);
                }
                else
                {
                    TextBox tb5 = new TextBox { Name = obj["measure_item_code"].ToString() + "_Value", Location = new Point(60, 80) };
                    panel1.Controls.Add(tb5);
                }
                this.flpTabTwoContent.Controls.Add(panel1);
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

        }
        #endregion

        private void ThreadFormSubmit() {
            sb.Remove(0,sb.Length);
            sb.Append("{");
            sb.Append("\"couping_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtCoupingNo.Text.Trim(), Encoding.UTF8) + "\",");
            sb.Append("\"contract_no\"" + ":" + "\"" + HttpUtility.UrlEncode(this.cmbContractNo.SelectedValue.ToString(), Encoding.UTF8) + "\",");
            sb.Append("\"production_line\"" + ":" + "\"" + HttpUtility.UrlEncode(txtProductionArea.Text.Trim(), Encoding.UTF8) + "\",");
            sb.Append("\"machine_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtMachineNo.Text.Trim(), Encoding.UTF8) + "\",");
            //sb.Append("\"process_no\"" + ":" + "\"" + HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
            sb.Append("\"operator_no\"" + ":" + "\"" + HttpUtility.UrlEncode(txtOperatorNo.Text.Trim(), Encoding.UTF8) + "\",");
            sb.Append("\"production_crew\"" + ":" + "\"" + this.cmbProductionCrew.Text + "\",");
             sb.Append("\"production_shift\"" + ":" + "\"" + this.cmbProductionShift.Text + "\",");
            sb.Append("\"video_no\"" + ":" + "\"" + HttpUtility.UrlEncode("", Encoding.UTF8) + "\",");
            //sb.Append("\"inspection_result\"" + ":" + "\"" + HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
            GoThroughControls(this.flpTabOneContent);
            GoThroughControls(this.flpTabTwoContent);
            string formData = sb.ToString();
            formData = formData.Substring(0, formData.LastIndexOf(","));
            sb.Append("}");
            MessageBox.Show(formData);
        }


        #region 遍历封装提交项函数
        private void GoThroughControls(Control parContainer)
        {
            for (int index = 0; index < parContainer.Controls.Count; index++)
            {
                // 如果是容器类控件，递归调用自己
                if (parContainer.Controls[index].HasChildren)
                {
                    GoThroughControls(parContainer.Controls[index]);
                }
                else
                {
                    switch (parContainer.Controls[index].GetType().Name)
                    {
                        case "TextBox":
                            sb.Append("\""+ parContainer.Controls[index].Name+ "\"" + ":" + "\""+ HttpUtility.UrlEncode(parContainer.Controls[index].Text.Trim(), Encoding.UTF8) + "\",");
                            //parContainer.Controls[index].Text = "";
                            break;
                    }
                }
            }
        } 
        #endregion

    }
}
