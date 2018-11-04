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
using System.Runtime.InteropServices;

namespace YYOPInspectionClient
{
    public partial class LoginWinform : Form
    {
        #region 全局变量
        //拖动窗体使用
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        //定义登录窗体实例变量
        private static LoginWinform myForm = null;
        //定义计时器 测试与服务器的连接情况
        private static  System.Timers.Timer t = new System.Timers.Timer(5000);//实例化Timer类，设置时间间隔
        #endregion

        #region 获取登录窗体实例
        public static LoginWinform getForm()
        {
            if (myForm == null)
            {
                new LoginWinform();
            }
            return myForm;
        } 
        #endregion

        #region 构造函数
        private LoginWinform()
        {
            InitializeComponent();
            //设置窗体头部标题(版本号和客户端mac地址)
            this.lblLoginTitle.Text = "接箍螺纹检验监造系统(" + CommonUtil.GetVersion() + ")" + CommonUtil.GetFirstMacAddress();
            myForm = this;
            //开始测试客户端与服务器连接是否通畅
            t.Elapsed += new System.Timers.ElapsedEventHandler(CommonUtil.UpdatePing);//到达时间的时候执行事件
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        } 
        #endregion

        #region 用户点击登录事件
        private void button1_Click(object sender, EventArgs e)
        {
            //加密当前mac地址
            string verification_code = CommonUtil.Encrypt();
            //操作工工号
            string employee_no = this.txtLoginName.Text.Trim();
            //密码
            string upwd = this.txtLoginPwd.Text.Trim();
            this.button1.Text = "登录中...";
            this.button1.Enabled = false;
            try
            {
                //封装客户端登录所传数据
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
                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
                if (content != null)
                {
                    //如果返回的数据为"{}"
                    if (content.Trim().Contains("{}"))
                    {
                        MessagePrompt.Show("登录异常!");
                    }
                    else
                    {
                        //如果登录成功,返回的数据格式为{success:'True/False',msg:'',rowsData:''},rowsData存放的为当前登录用户的信息
                        JObject jobject = JObject.Parse(content);
                        string loginFlag = jobject["success"].ToString().Trim();
                        string msg = jobject["msg"].ToString().Trim();
                        if (loginFlag.Contains("True"))
                        {
                            string rowsJson = jobject["rowsData"].ToString();
                            if (rowsJson != null)
                            {
                                //登录成功返回当前登录用户的信息,将返回的json格式的信息转换成指定对象
                                Person person = JsonConvert.DeserializeObject<Person>(rowsJson);
                                IndexWindow.getForm().Show();
                                NumberKeyboardForm.getForm();
                                AlphabetKeyboardForm.getForm();
                                ScanerHook.executeScanerHook();
                                this.Hide();
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
                this.button1.Enabled = true;
                this.button1.Text = "登录";
            }
            catch (WebException ex) {
                MessagePrompt.Show("网络错误，错误信息:"+ex.Message);
                this.button1.Enabled = true;
                this.button1.Text = "登录";
            }
            catch (Exception ec)
            {
                MessagePrompt.Show("连接服务器失败,失败原因:"+ec.Message);
                this.button1.Enabled = true;
                this.button1.Text = "登录";
            }
        }
        #endregion

        #region 用户名输入框获取焦点事件
        private void txtLoginName_MouseDown(object sender, MouseEventArgs e)
        {
            txt_MouseDown(this.txtLoginName);
        }
        #endregion

        #region 密码输入框获取焦点事件
        private void txtLoginPwd_MouseDown(object sender, MouseEventArgs e)
        {
            txt_MouseDown(this.txtLoginPwd);
        }
        #endregion

        #region 输入框获取焦点事件
        private void txt_MouseDown(TextBox tb)
        {
            try
            {
                if (tb.Tag.ToString().Contains("English"))
                {
                    //设置英文输入法中输入框变量等于当前获取焦点的输入框
                    AlphabetKeyboardForm.inputTxt = tb;
                    //设置英文输入法输入框内容为当前输入框内容
                    AlphabetKeyboardForm.getForm().Textbox_display.Text = tb.Text.Trim();
                    AlphabetKeyboardForm.getForm().Show();
                    //设置输入法在最顶部显示
                    AlphabetKeyboardForm.getForm().TopMost = true;
                    //设置英文输入法头部显示的输入框名称为当前输入框名称
                    if (tb.Name.Contains("txtLoginName"))
                        AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "用户名";
                    if (tb.Name.Contains("txtLoginPwd"))
                        AlphabetKeyboardForm.getForm().lblEnglishTitle.Text = "密码";
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("系统出错，错误信息:" + e.Message);
            }
        }

        #endregion

        #region 鼠标按下(在整个窗体)   
        private void LoginWinform_MouseDown(object sender, MouseEventArgs e)
        {
            DragAndDropForm();
        }
        #endregion

        #region 鼠标按下(在窗体标题上的panel控件上)   
        private void pnlLoginTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DragAndDropForm();
        }
        #endregion

        #region 鼠标按下(在窗体标题上的label控件上)   

        private void lblLoginTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DragAndDropForm();
        }
        #endregion

        #region 通过调用api，通过消息来实现拖动 
        private void DragAndDropForm()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0);
        }

        #endregion

        #region 退出登录事件
        private void btnLoginOut_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region 关闭登录窗体事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region 窗体显示与隐藏
        private void LoginWinform_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                t.Start();
            }
            else
            {
                t.Stop();
            }
        }
        #endregion

        #region 窗体关闭事件
        private void LoginWinform_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ScanerHook.GetScanerHookInstance().Stop();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
