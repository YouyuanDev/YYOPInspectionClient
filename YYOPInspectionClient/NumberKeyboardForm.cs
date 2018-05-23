using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class NumberKeyboardForm : Form
    {
        public TextBox inputTxt;
        public List<TextBox> flpTabTwoTxtList;
        public Control containerControl=null;
        public int type = 1;//标识是登录页面还是表单，0代表登录页面，1代表表单


        //---------------------拖动无窗体的控件(开始)
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //---------------------拖动无窗体的控件(结束)
        public NumberKeyboardForm()
        {
            InitializeComponent();
        }

        private void addNum(string num)
        {
            if (this.Textbox_display.Text.Contains("合格"))
            {
                this.Textbox_display.Text = "0";
            }

            if ((this.Textbox_display.Text.Equals("0")|| this.Textbox_display.Text.Equals("-0")) &&!num.Equals("."))
            {//覆盖0值
                this.Textbox_display.Text = num;
            }
            else if(num.Equals("."))
            {//追加小数点
                if (!this.Textbox_display.Text.Contains("."))
                {
                    if (!string.IsNullOrWhiteSpace(this.Textbox_display.Text) && !Textbox_display.Text.Equals("-"))
                        this.Textbox_display.Text += num;
                    else {
                        if(Textbox_display.Text.Contains("-"))
                            this.Textbox_display.Text = "-0" + num;
                        else
                            this.Textbox_display.Text = "0" + num;
                    }
                        

                }
            }
            else
            {
                //追加数字
                this.Textbox_display.Text += num;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            //隐藏数字键盘
            this.Textbox_display.Text = "0";
            this.Hide();

        }

        private void button_accept_Click(object sender, EventArgs e)
        {
            //合格
            this.Textbox_display.Text = "合格";
        }

        private void button_not_accept_Click(object sender, EventArgs e)
        {
            //不合格
            this.Textbox_display.Text = "不合格";
        }

        private void button_enter_Click(object sender, EventArgs e)
        {
            if (type == 0)
            {
                inputTxt.Text = Textbox_display.Text.Trim();
                this.Textbox_display.Text = "";
            }
            else {
                //回车,清楚末尾无用的.
                if (this.Textbox_display.Text.Contains(".") && this.Textbox_display.Text.LastIndexOf('.') == this.Textbox_display.Text.Length - 1)
                {
                    this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);
                }
                //
                if (Textbox_display.Text.Contains(".")) {
                    //sbyte"13.s"
                    try { 
                        float tempf = Convert.ToSingle(Textbox_display.Text);
                        Textbox_display.Text = tempf.ToString();
                     }
                    catch
                    {
                        Textbox_display.Text = "0";
                    }

                }

                if (inputTxt != null)
                {
                    inputTxt.Text = this.Textbox_display.Text.Trim();
                    this.Textbox_display.Text = "";
                    string inputTxtName = inputTxt.Name;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(inputTxtName)) {
                            if(inputTxtName.Contains("_A_Value"))
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
                            float maxVal = 0, minVal = 0, txtVal = 0,maxOvality=0,sdVal=0;
                            Label lblRangeFrequencyOvality = (Label)GetControlInstance(containerControl, inputTxtName + "_RangeFrequencyOvality_lbl");
                            if (lblRangeFrequencyOvality != null) {
                                if (lblRangeFrequencyOvality.Tag!=null){
                                    if (!string.IsNullOrWhiteSpace(lblRangeFrequencyOvality.Tag.ToString())) {
                                        string[] rangeFrequency = lblRangeFrequencyOvality.Tag.ToString().Split(',');
                                        maxVal = Convert.ToSingle(rangeFrequency[0]);
                                        minVal = Convert.ToSingle(rangeFrequency[1]);
                                        txtVal = Convert.ToSingle(inputTxt.Text.Trim());
                                        if(!string.IsNullOrWhiteSpace(rangeFrequency[3]))
                                           maxOvality= Convert.ToSingle(rangeFrequency[3]);
                                        if (!string.IsNullOrWhiteSpace(rangeFrequency[4]))
                                            sdVal = Convert.ToSingle(rangeFrequency[4]);
                                        if (maxVal - minVal>0.00001) {
                                            if(txtVal<minVal||txtVal>maxVal)
                                                inputTxt.BackColor = Color.LightCoral;
                                            else
                                                inputTxt.BackColor = Color.White;
                                        }
                                    }
                                    //找到最大值、最小值，然后判断是否存在均值和椭圆度
                                    TextBox txtMaxOfA = (TextBox)GetControlInstance(containerControl, inputTxtName + "_MaxA_Value");
                                    TextBox txtMaxOfB = (TextBox)GetControlInstance(containerControl, inputTxtName + "_MaxB_Value");
                                    TextBox txtMinOfA = (TextBox)GetControlInstance(containerControl, inputTxtName + "_MinA_Value");
                                    TextBox txtMinOfB = (TextBox)GetControlInstance(containerControl, inputTxtName + "_MinB_Value");
                                    if (txtMaxOfA != null && txtMinOfA != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(txtMaxOfA.Text) && !string.IsNullOrWhiteSpace(txtMinOfA.Text)) {
                                            float avg = ((Convert.ToSingle(txtMaxOfA.Text) + Convert.ToSingle(txtMinOfA.Text)) / 2);
                                            Label lblAvgOfA = (Label)GetControlInstance(containerControl, inputTxtName + "_AvgA");
                                            //判断均值是否符合要求
                                            if (lblAvgOfA != null) {
                                                if (avg < minVal || avg > maxVal)
                                                    lblAvgOfA.ForeColor = Color.Red;
                                                else
                                                    lblAvgOfA.ForeColor = Color.Black;
                                                lblAvgOfA.Text = Convert.ToString(Math.Round(avg,2)) ;
                                            }
                                            Label lblOvalityA = (Label)GetControlInstance(containerControl, inputTxtName + "_OvalityA");
                                            //判断椭圆度是否满足要求
                                            if (lblOvalityA != null) {
                                                float ovality = (Convert.ToSingle(txtMaxOfA.Text) - Convert.ToSingle(txtMinOfA.Text)) / sdVal;
                                                if (ovality > maxOvality || ovality < 0)
                                                    lblOvalityA.ForeColor = Color.Red;
                                                else
                                                    lblOvalityA.ForeColor = Color.Black;
                                                lblOvalityA.Text = Convert.ToString(Math.Round(ovality,2));
                                            }
                                        }
                                     }
                                    if (txtMaxOfB != null && txtMinOfB != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(txtMaxOfB.Text) && !string.IsNullOrWhiteSpace(txtMinOfB.Text))
                                        {
                                            float avg = ((Convert.ToSingle(txtMaxOfB.Text) + Convert.ToSingle(txtMinOfB.Text)) / 2);
                                            Label lblAvgOfB = (Label)GetControlInstance(containerControl, inputTxtName + "_AvgB");
                                            if (lblAvgOfB != null)
                                            {
                                                if (avg < minVal || avg > maxVal)
                                                    lblAvgOfB.ForeColor = Color.Red;
                                                else
                                                    lblAvgOfB.ForeColor = Color.Black;
                                                lblAvgOfB.Text = Convert.ToString(Math.Round(avg,2));
                                            }
                                            Label lblOvalityB= (Label)GetControlInstance(containerControl, inputTxtName + "_OvalityB");
                                            //判断椭圆度是否满足要求
                                            if (lblOvalityB != null)
                                            {
                                                float ovality = (Convert.ToSingle(txtMaxOfB.Text) - Convert.ToSingle(txtMinOfB.Text)) / sdVal;
                                                if (ovality > maxOvality||ovality<0)
                                                    lblOvalityB.ForeColor = Color.Red;
                                                else
                                                    lblOvalityB.ForeColor = Color.Black;
                                                lblOvalityB.Text = Convert.ToString(Math.Round(ovality,2));
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        //if (!string.IsNullOrWhiteSpace(inputTxtName))
                        //{
                        //    //获取和该控件相同名称的Label
                        //    if (inputTxtName.Contains("_A_Value"))
                        //    {
                        //        inputTxtName = inputTxtName.Replace("_A_Value", "");
                        //    }
                        //    if (inputTxtName.Contains("_B_Value"))
                        //    {
                        //        inputTxtName = inputTxtName.Replace("_B_Value", "");
                        //    }
                        //    if (containerControl != null)
                        //    {
                        //        Label lbl = (Label)GetControlInstance(containerControl, inputTxtName + "_RangeFrequency_lbl");
                        //        if (lbl != null)
                        //        {
                        //            float val1 = -100000000f, val2 =100000000f;
                        //            string lblTag = Convert.ToString(lbl.Tag);
                        //            if (!string.IsNullOrWhiteSpace(lblTag))
                        //            {
                        //                string[] valArr = lblTag.Split('-');
                        //                if (valArr.Length > 0)
                        //                {
                        //                    val1 = Convert.ToSingle(valArr[0]);
                        //                    if (valArr.Length > 1)
                        //                    {
                        //                        val2 = Convert.ToSingle(valArr[1]);
                        //                    }
                        //                }
                        //                float inputVal = Convert.ToSingle(inputTxt.Text);
                        //                Console.WriteLine("输入的值:" + inputVal);
                        //                if (inputVal <val1)
                        //                {
                        //                      inputTxt.BackColor = Color.LightCoral;
                        //                }
                        //                if (inputVal > val2)
                        //                 {
                        //                    inputTxt.BackColor = Color.LightCoral;
                        //                 }

                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        MessagePrompt.Show("没有初始化");
                        //    }
                        //}
                        int index = flpTabTwoTxtList.IndexOf(inputTxt);
                        if (index < flpTabTwoTxtList.Count - 1)
                            index++;
                        TextBox tb = flpTabTwoTxtList[index];
                        if (tb != null)
                            tb.Focus();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("判断大小的时候错误.......");
                    }
                }
            }

            
        }

        private void button_backspace_Click(object sender, EventArgs e)
        {
            //退格键
            if (this.Textbox_display.Text.Length > 1)
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);
            if (this.Textbox_display.Text.Length == 1)
                this.Textbox_display.Text = "0";

        }


        private void num_Click(object sender, EventArgs e)
        {
            //数字0-9  符号.
            addNum(((Button)sender).Text);
        }



        private void button_clear_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "0";
        }

        public static Control GetControl(Control ct, string name)
        {
            
            Control[] ctls = ct.Controls.Find(name, false);
            
            if (ctls.Length > 0)
            {
                return ctls[0];
            }
            else
            {
                return null;
            }
        }

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

        #region 窗体绘制边框和拖动事件
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
            Color.DimGray, 2, ButtonBorderStyle.Solid, //左边
            Color.DimGray, 0, ButtonBorderStyle.Solid, //上边
            Color.DimGray, 2, ButtonBorderStyle.Solid, //右边
              Color.DimGray, 2, ButtonBorderStyle.Solid);//底边
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel1.ClientRectangle,
            Color.DimGray, 2, ButtonBorderStyle.Solid, //左边
            Color.DimGray, 2, ButtonBorderStyle.Solid, //上边
           Color.DimGray, 2, ButtonBorderStyle.Solid, //右边
           Color.DimGray, 0, ButtonBorderStyle.Solid);//底边
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion

        private void btnMinus_Click(object sender, EventArgs e)
        {
            if (this.Textbox_display.Text.Contains("合格"))
            {
                return;
            }
            if (Textbox_display.Text.Contains("-"))
            {
                Textbox_display.Text=Textbox_display.Text.Replace('-', ' ').Trim();
            }
            else {
                Textbox_display.Text=Textbox_display.Text.Insert(0,"-");
            }
            //string s = Textbox_display.Text;
            //int mouseIndex = this.Textbox_display.SelectionStart;
            //s = s.Insert(mouseIndex, "-");
            //Textbox_display.Text = s;
            //Textbox_display.SelectionStart = mouseIndex + 1;
            //Textbox_display.Focus();
        }
         
    }
}
