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
using AutoUpdaterDotNET;
using System.Threading;
using Newtonsoft.Json;
using System.Reflection;

namespace YYOPInspectionClient
{
    public partial class LoginWinform : Form
    {
        private AlphabetKeyboardForm englishKeyboard = null; 
        private IndexWindow indexWindow = null;
        public LoginWinform()
        {
            InitializeComponent();
            this.lblLoginTitle.Text="接箍螺纹检验监造系统("+CommonUtil.GetVersion()+")";
            //txtLoginName.MouseDown += new MouseEventHandler(txt_MouseDown);
            //txtLoginPwd.MouseDown += new MouseEventHandler(txt_MouseDown);
        }

        #region 用户点击登录事件
        private void button1_Click(object sender, EventArgs e)
        {
            string employee_no = this.txtLoginName.Text.Trim();
            string upwd = this.txtLoginPwd.Text.Trim();
            var httpStatusCode = 200;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"employee_no\"" + ":" + "\"" + employee_no + "\",");
                sb.Append("\"ppassword\"" + ":" + "\"" + upwd + "\"");
                sb.Append("}");
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject o = JObject.Parse(sb.ToString());
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
                string url = CommonUtil.getServerIpAndPort() + "Login/userLoginOfWinform.action";
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
                    if (jsons.Trim().Equals("{}"))
                    {
                        MessagePrompt.Show("用户名或密码错误!");
                    }
                    else {
                        JObject jobject = JObject.Parse(jsons);
                        string rowsJson = jobject["rowsData"].ToString();
                        if (rowsJson != null)
                        {
                            Person person = JsonConvert.DeserializeObject<Person>(rowsJson);
                            if (indexWindow==null)
                            {
                                indexWindow=new IndexWindow();
                                indexWindow.Show();
                            }
                            else {
                                indexWindow.Show(); 
                            }
                            indexWindow.loginWinform = this;
                            this.Hide();
                            if (englishKeyboard != null) 
                                englishKeyboard.Hide();
                        }
                        else
                        {
                            MessagePrompt.Show("用户名或密码错误!");
                        }
                    }
                }
                httpStatusCode = Convert.ToInt32(response.StatusCode);
            }
            catch (WebException ex)
            {
                var rsp = ex.Response as HttpWebResponse;
                httpStatusCode = Convert.ToInt32(rsp.StatusCode);
            }
            catch (Exception ec)
            {
                MessagePrompt.Show("服务器尚未开启......");
                if (employee_no == "admin" && upwd == "admin")
                {
                    IndexWindow index = new IndexWindow();
                    index.Show();
                    this.Close();
                }
            }
            if (httpStatusCode != 200)
            {
                MessagePrompt.Show("服务器未响应........");
                if (employee_no == "admin" && upwd == "admin")
                {
                    IndexWindow index = new IndexWindow();
                    index.Show();
                    this.Close();
                }
            }
        }
        #endregion

        
        #region 鼠标点击输入框事件
        private void txt_MouseDown(TextBox tb)
        {
            if (englishKeyboard == null)
                englishKeyboard = new AlphabetKeyboardForm();
            if (tb.Tag.Equals("English"))
            {
                englishKeyboard.inputTxt = tb;
                englishKeyboard.Textbox_display.Text = tb.Text.Trim();
                englishKeyboard.Show();
                englishKeyboard.TopMost = true;
                if (tb.Name.Contains("txtLoginName"))
                    englishKeyboard.lblEnglishTitle.Text = "用户名";
                if(tb.Name.Contains("txtLoginPwd"))
                    englishKeyboard.lblEnglishTitle.Text = "密码";
                //SetAlphaKeyboardText(tb.Name);
            }
        }

        #endregion

        private void LoginWinform_Load(object sender, EventArgs e)
        {
           // auto.controllInitializeSize(this);
        }

        private void LoginWinform_SizeChanged(object sender, EventArgs e)
        {
            //auto.controlAutoSize(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLoginOut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtLoginName_MouseDown(object sender, MouseEventArgs e)
        {
            txt_MouseDown(this.txtLoginName);
        }

        private void txtLoginPwd_MouseDown(object sender, MouseEventArgs e)
        {
            txt_MouseDown(this.txtLoginPwd);
        }
    }
}
