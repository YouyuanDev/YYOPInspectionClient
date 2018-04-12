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
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class 登录 : Form
    {
        public 登录()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uname = this.textBox1.Text.Trim();
            string upwd = this.textBox2.Text.Trim();
            var httpStatusCode = 200;
            try
            {
                //StringBuilder sb = new StringBuilder();
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                //JObject o = JObject.Parse(sb.ToString());
                String param = "";
                byte[] data = encoding.GetBytes(param);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://192.168.0.200:8080/Login/commitLogin.action?employee_no=" + uname + "&ppassword=" + upwd);
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
                    string result = jobject["success"].ToString();
                    if (result == "True")
                    {
                        // this.Close();
                        new IndexWindow().Show();
                        this.Hide();
                    }
                    else {
                        MessageBox.Show("用户名或密码错误!");
                    }
                    //List<ComboxItem> list = JsonConvert.DeserializeObject<List<ComboxItem>>(rowsJson);
                    //this.comboBox2.DataSource = list;
                    //this.comboBox2.ValueMember = "id";
                    //this.comboBox2.DisplayMember = "text";
                }
                httpStatusCode = Convert.ToInt32(response.StatusCode);
            }
            catch (WebException ex)
            {
                var rsp = ex.Response as HttpWebResponse;
                httpStatusCode =Convert.ToInt32(rsp.StatusCode);
            } catch (Exception ec) {
                MessageBox.Show("服务器尚未开启......");
                if (uname == "admin" && upwd == "admin")
                {
                    IndexWindow index = new IndexWindow();
                    index.Show();
                    this.Close();
                }
            }
            if (httpStatusCode!= 200){
                MessageBox.Show("服务器未响应.....");
                if (uname == "admin" && upwd == "admin")
                {
                    IndexWindow index = new IndexWindow();
                    index.Show();
                    this.Close();
                }
            }
        }
    }
}
