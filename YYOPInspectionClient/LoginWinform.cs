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
        //private AlphabetKeyboardForm englishKeyboard = null;
        private static LoginWinform myForm = null;
        public static LoginWinform getForm()
        {
            if (myForm == null)
            {
                new LoginWinform();
            }

            return myForm;
        }
        private LoginWinform()
        {
            InitializeComponent();
            this.lblLoginTitle.Text="接箍螺纹检验监造系统("+CommonUtil.GetVersion()+")"+CommonUtil.GetFirstMacAddress();
            myForm = this;
        }

        #region 用户点击登录事件
        private void button1_Click(object sender, EventArgs e)
        {
            string verification_code = CommonUtil.Encrypt();
            string employee_no = this.txtLoginName.Text.Trim();
            string upwd = this.txtLoginPwd.Text.Trim();
            try
            {
                JObject json = new JObject{
                    {"employee_no",employee_no },
                    {"ppassword",upwd },
                    {"verification_code",verification_code}
                };

                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                byte[] data = encoding.GetBytes(json.ToString());
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
                    if (jsons.Trim().Contains("{}"))
                    {
                        MessagePrompt.Show("登录异常!");
                    }
                    else
                    {
                        JObject jobject = JObject.Parse(jsons);
                        string loginFlag = jobject["success"].ToString().Trim();
                        string msg = jobject["msg"].ToString().Trim();
                        if (loginFlag.Contains("True"))
                        {
                            string rowsJson = jobject["rowsData"].ToString();
                            if (rowsJson != null)
                            {
                                Person person = JsonConvert.DeserializeObject<Person>(rowsJson);
                                
                                IndexWindow.getForm().Show();
                                
                                //indexWindow.loginWinform = this;
                                this.Hide();
                                AlphabetKeyboardForm.getForm().Close();
                            }
                            else
                            {
                                MessagePrompt.Show("系统繁忙，请稍后重试!");
                            }
                        }
                        else
                        {
                            MessagePrompt.Show(msg);
                        }
                    }
                }
            }
            catch (WebException ex) {
                MessagePrompt.Show("网络错误，错误信息:"+ex.Message);
            }
            catch (Exception ec)
            {
                MessagePrompt.Show("服务器尚未开启......");
                Application.Exit();
            }
        }
        #endregion

        
        #region 鼠标点击输入框事件
        private void txt_MouseDown(TextBox tb)
        {
             
                
            if (tb.Tag.ToString().Contains("English"))
            {
                AlphabetKeyboardForm.getForm().inputTxt = tb;
                AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                AlphabetKeyboardForm.getForm().Show();
                AlphabetKeyboardForm.getForm().TopMost = true;
                if (tb.Name.Contains("txtLoginName"))
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "用户名";
                if(tb.Name.Contains("txtLoginPwd"))
                    AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "密码";
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
