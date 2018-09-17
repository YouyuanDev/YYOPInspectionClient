using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class NumberKeyboardForm : Form
    {
        //定义保存测量值的TextBox控件集合
        //public static List<TextBox> flpTabTwoTextBoxList;

        //定义标识(判断输入框实在那个窗体中,0代表在ThreadingForm中,1代表在DetailForm中)
        public static int flag = 0;
        //定义数字输入法弹出时对应的鼠标焦点所在的TextBox控件
        public static TextBox inputTxt=null;
        //定义存放测量值的TextBox控件的容器控件
        //public Control containerControl=null;
        //定义存放测量项是否合法的标识集合
        //public static Dictionary<string, bool> qualifiedList = new Dictionary<string, bool>();
        //定义当前窗体
        private static NumberKeyboardForm myForm = null;

        //---------------------拖动无窗体的控件(开始)
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //---------------------拖动无窗体的控件(结束)
        #region 单例函数
        public static NumberKeyboardForm getForm()
        {
            if (myForm == null)
            {
                new NumberKeyboardForm();
            }

            return myForm;
        }
        #endregion

        #region 构造函数
        public NumberKeyboardForm()
        {
            InitializeComponent();
            myForm = this;
        }
        #endregion

        #region 向输入法内容框中追加点击的内容
        private void addNum(string num)
        {
            //如果输入法内容为"合格"
            if (this.Textbox_display.Text.Contains("合格"))
            {
                this.Textbox_display.Text = "0";
            }
            //如果输入法内容为"0"或者"-0"并且不为"."
            if ((this.Textbox_display.Text.Equals("0") || this.Textbox_display.Text.Equals("-0")) && !num.Equals("."))
            {   
                //设置输入法内容为点击的内容
                this.Textbox_display.Text = num;
            }
            else if (num.Equals("."))//如果点击的控件是"."
            {
                //如果输入法内容不包含"."
                if (!this.Textbox_display.Text.Contains("."))
                {
                    //如果输入法内容不为空并且不等于"-"
                    if (!string.IsNullOrWhiteSpace(this.Textbox_display.Text) && !Textbox_display.Text.Equals("-"))
                        this.Textbox_display.Text += num;
                    else
                    {
                        //如果输入法内容包含"-"
                        if (Textbox_display.Text.Contains("-"))
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
        #endregion

        #region 关闭数字输入法
        private void button_close_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "0";
            this.Hide();
        }
        #endregion

        #region 点击合格事件
        private void button_accept_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "合格";
        }
        #endregion

        #region 点击不合格事件
        private void button_not_accept_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "不合格";
        }
        #endregion

        #region 点击Enter(确定)事件
        private void button_enter_Click(object sender, EventArgs e)
        {
            //回车,清楚末尾无用的.
            if (this.Textbox_display.Text.Contains(".") && this.Textbox_display.Text.LastIndexOf('.') == this.Textbox_display.Text.Length - 1)
            {
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);
            }
            //判断输入法内容是否包含"."
            if (Textbox_display.Text.Contains("."))
            {
                try
                {
                    //将数字专程float类型 为了转换如"3."格式的数字
                    float tempf = Convert.ToSingle(Textbox_display.Text);
                    Textbox_display.Text = tempf.ToString();
                }
                catch
                {
                    Textbox_display.Text = "0";
                }

            }
            //鼠标焦点所在的TextBox控件不为空
            if (inputTxt != null)
            {
                //设置输入框的内容为输入法输入的内容
                inputTxt.Text = this.Textbox_display.Text.Trim();
                //将输入法内容清空
                this.Textbox_display.Text = "";
                //获取鼠标焦点所在的TextBox控件
                string inputTxtName = inputTxt.Name;
                try
                {
                    //如果鼠标焦点所在的TextBox控件名称为空
                    if (string.IsNullOrWhiteSpace(inputTxtName))
                    {
                        return;
                    }
                    //当前输入框得类型,0代表时最大值、最小值和均值，1代表椭圆度
                    int tbType=0;
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
                    {
                        inputTxtName = inputTxtName.Replace("_OvalityA", "");
                        tbType = 1;
                    }
                    else if (inputTxtName.Contains("_OvalityB"))
                    {
                        inputTxtName = inputTxtName.Replace("_OvalityB", "");
                        tbType = 1;
                    }
                    //找到该测量项的值范围、和椭圆度最大值
                    float maxVal = 0, minVal = 0, txtVal = float.MaxValue, maxOvality = 0, sdVal = 0;
                    Dictionary<string, string> dic = null;
                    if (flag == 0)
                    {
                        dic = ThreadingForm.measureInfoDic[inputTxtName];
                    }
                    else if (flag == 1) {
                        dic = DetailForm.measureInfoDic[inputTxtName];
                    }
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
                    if (!string.IsNullOrWhiteSpace(inputTxt.Text.Trim())) {
                        txtVal = Convert.ToSingle(inputTxt.Text.Trim());
                    }
                    if (maxVal - minVal > 0 && txtVal!=float.MaxValue)
                    {
                        //如果输入法输入的值不符合标准
                        if (txtVal < minVal || txtVal > maxVal)
                        {
                            //设置存放测量值的输入框的背景色
                            inputTxt.BackColor = Color.LightCoral;
                            //如果该集合中包含该控件的名称
                            if (flag == 0)
                            {
                                if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                {
                                    ThreadingForm.qualifiedList[inputTxtName] = false;
                                }
                            }
                            else if (flag == 1) {
                                if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                {
                                    DetailForm.qualifiedList[inputTxtName] = false;
                                }
                            }
                        }
                        else
                        {
                            inputTxt.BackColor = Color.White;
                            if (flag == 0)
                            {
                                if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                {
                                    ThreadingForm.qualifiedList[inputTxtName] = true;
                                }
                            }
                            else if (flag == 1)
                            {
                                if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                {
                                    DetailForm.qualifiedList[inputTxtName] = true;
                                }
                            }
                        }
                    }
                    //找到该测量项A端、B端最大值、最小值，然后判断是否存在均值和椭圆度
                    TextBox txtMaxOfA = null, txtMaxOfB = null, txtMinOfA = null, txtMinOfB = null, tbAvgOfA = null, tbOvalityA = null,
                          tbAvgOfB = null, tbOvalityB = null;
                    if (flag == 0)
                    {
                        txtMaxOfA = ThreadingForm.controlTxtDir[inputTxtName + "_MaxA_Value"];
                        txtMaxOfB = ThreadingForm.controlTxtDir[inputTxtName + "_MaxB_Value"];
                        txtMinOfA = ThreadingForm.controlTxtDir[inputTxtName + "_MinA_Value"];
                        txtMinOfB = ThreadingForm.controlTxtDir[inputTxtName + "_MinB_Value"];
                        tbAvgOfA =  ThreadingForm.controlTxtDir[inputTxtName + "_AvgA"];
                        tbAvgOfB = ThreadingForm.controlTxtDir[inputTxtName + "_AvgB"];
                        tbOvalityA = ThreadingForm.controlTxtDir[inputTxtName + "_OvalityA"];
                        tbOvalityB = ThreadingForm.controlTxtDir[inputTxtName + "_OvalityB"];
                    }
                    else if (flag == 1) {
                        txtMaxOfA = DetailForm.controlTxtDir[inputTxtName + "_MaxA_Value"];
                        txtMaxOfB = DetailForm.controlTxtDir[inputTxtName + "_MaxB_Value"];
                        txtMinOfA = DetailForm.controlTxtDir[inputTxtName + "_MinA_Value"];
                        txtMinOfB = DetailForm.controlTxtDir[inputTxtName + "_MinB_Value"];
                        tbAvgOfA = DetailForm.controlTxtDir[inputTxtName + "_AvgA"];
                        tbAvgOfB = DetailForm.controlTxtDir[inputTxtName + "_AvgB"];
                        tbOvalityA = DetailForm.controlTxtDir[inputTxtName + "_OvalityA"];
                        tbOvalityB = DetailForm.controlTxtDir[inputTxtName + "_OvalityB"];
                    }
                    //判断输入的数值是否合理
                    bool reasonableFlag = false;
                    switch (tbType) {
                        case 0:
                            if (maxVal * 10 < txtVal)
                                reasonableFlag = true;
                            break;
                        case 1:
                            if (maxOvality > 0 && maxOvality * 10 < txtVal)
                                reasonableFlag = true;
                            break;
                    }
                    if (reasonableFlag)
                    {
                        MessagePrompt.Show("输入的数据偏差过大, 请检查!");
                    }
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
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = false;
                                }
                                else if (flag == 1) {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = false;
                                }
                            }
                            else
                            {
                                tbAvgOfA.BackColor = Color.White;
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = true;
                                }
                                else if (flag == 1) {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = true;
                                }
                               
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
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = false;
                                }
                                else if (flag == 1) {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = false;
                                }
                            }
                            else
                            {
                                tbOvalityA.BackColor = Color.White;
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = true;
                                }
                                else if (flag == 1) {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = true;
                                }
                               
                            }
                            tbOvalityA.Text = Convert.ToString(Math.Round(ovality, 2));
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
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                    {
                                        ThreadingForm.qualifiedList[inputTxtName] = false;
                                    }
                                }
                                else if (flag == 1) {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                    {
                                        DetailForm.qualifiedList[inputTxtName] = false;
                                    }
                                }
                               
                            }
                            else
                            {
                                tbAvgOfB.BackColor = Color.White;
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = true;
                                }
                                else if (flag == 1)
                                {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = true;
                                }
                               
                            }
                            tbAvgOfB.Text = Convert.ToString(Math.Round(avg, 2));
                        }
                        //判断椭圆度是否满足要求
                        if (tbOvalityB != null)
                        {
                            float ovality = (Convert.ToSingle(txtMaxOfB.Text) - Convert.ToSingle(txtMinOfB.Text)) / sdVal;
                            if (ovality > maxOvality || ovality < 0)
                            {
                                tbOvalityB.BackColor = Color.LightCoral;
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = false;
                                }
                                else if (flag == 1)
                                {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = false;
                                }
                               
                            }
                            else
                            {
                                tbOvalityB.BackColor = Color.White;
                                if (flag == 0)
                                {
                                    if (ThreadingForm.qualifiedList.ContainsKey(inputTxtName))
                                        ThreadingForm.qualifiedList[inputTxtName] = true;
                                }
                                else if (flag == 1)
                                {
                                    if (DetailForm.qualifiedList.ContainsKey(inputTxtName))
                                        DetailForm.qualifiedList[inputTxtName] = true;
                                }
                            }
                            tbOvalityB.Text = Convert.ToString(Math.Round(ovality, 2));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("英文键盘回车时出错,错误信息:" + ex.Message);
                }
                finally //跳转到下一个输入框
                {
                    //查询鼠标焦点所在的TextBox控件在控件集合中的索引
                    TextBox tb = null;int index =0;
                    if (flag == 0)
                    {
                        index =ThreadingForm.flpTabTwoTextBoxList.IndexOf(inputTxt);
                        if (index < ThreadingForm.flpTabTwoTextBoxList.Count - 1)
                            index++;
                        tb = ThreadingForm.flpTabTwoTextBoxList[index];
                    }
                    else if (flag == 1) {
                        index = DetailForm.flpTabTwoTextBoxList.IndexOf(inputTxt);
                        if (index < DetailForm.flpTabTwoTextBoxList.Count - 1)
                            index++;
                        tb = DetailForm.flpTabTwoTextBoxList[index];
                    }
                    //设置鼠标焦点在控件集合索引为index的控件上
                    if (tb != null)
                        tb.Focus();
                }
            }
        }
        #endregion

        #region 点击Backspace(退格)键
        private void button_backspace_Click(object sender, EventArgs e)
        {
            //如果输入法内有内容并且大于1
            if (this.Textbox_display.Text.Length > 1)
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);
            //设置输入法内默认值为0
            if (this.Textbox_display.Text.Length == 1)
                this.Textbox_display.Text = "0";

        }
        #endregion

        #region 数字点击事件
        private void num_Click(object sender, EventArgs e)
        {
            //数字0-9  符号.
            addNum(((Button)sender).Text);
        }
        #endregion

        #region 清空输入法内容事件(clear)
        private void button_clear_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "0";
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
        private void lblNumberTitle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
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

        #region 点击减号(-)事件
        private void btnMinus_Click(object sender, EventArgs e)
        {
            //如果当前输入法内容为合格，则不做任何操作
            if (this.Textbox_display.Text.Contains("合格"))
            {
                return;
            }
            //如果输入法内容包含了"-"号
            if (Textbox_display.Text.Contains("-"))
            {
                //将输入法的内容的"-"号替换为空字符串
                Textbox_display.Text = Textbox_display.Text.Replace('-', ' ').Trim();
            }
            else
            {
                //将输入法的内容的首位添加一个"-"号
                Textbox_display.Text = Textbox_display.Text.Insert(0, "-");
            }
        }

        #endregion

        #region 判断是否是数字
        private static bool IsNumber(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                    objNumberPattern.IsMatch(strNumber);
        }
        #endregion

        private void NumberKeyboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
