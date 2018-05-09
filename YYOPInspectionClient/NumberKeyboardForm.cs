using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

            if (this.Textbox_display.Text.Equals("0")&&!num.Equals("."))
            {//覆盖0值
                this.Textbox_display.Text = num;
            }
            else if(num.Equals("."))
            {//追加小数点
                if (!this.Textbox_display.Text.Contains("."))
                {
                    if (!string.IsNullOrWhiteSpace(this.Textbox_display.Text))
                        this.Textbox_display.Text += num;
                    else
                        this.Textbox_display.Text="0"+num;
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
                if (inputTxt != null)
                {
                    inputTxt.Text = this.Textbox_display.Text.Trim();
                    this.Textbox_display.Text = "";
                    string inputTxtName = inputTxt.Name;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(inputTxtName))
                        {
                            //获取和该控件相同名称的Label
                            if (inputTxtName.Contains("_A_Value"))
                            {
                                inputTxtName = inputTxtName.Replace("_A_Value", "");
                            }
                            if (inputTxtName.Contains("_B_Value"))
                            {
                                inputTxtName = inputTxtName.Replace("_B_Value", "");
                            }

                            if (containerControl != null)
                            {
                                Label lbl = (Label)GetControlInstance(containerControl, inputTxtName + "_lbl");
                                if (lbl != null)
                                {
                                    float val1 = 0f, val2 = 0;
                                    string lblTag = Convert.ToString(lbl.Tag);
                                    if (!string.IsNullOrWhiteSpace(lblTag))
                                    {
                                        string[] valArr = lblTag.Split(new char[] { '-' });
                                        if (valArr.Length > 0)
                                        {
                                            val1 = Convert.ToSingle(valArr[0]);
                                            if (valArr.Length > 1)
                                            {
                                                val2 = Convert.ToSingle(valArr[1]);
                                            }
                                        }
                                        if (val1 > 0)
                                        {
                                            float inputVal = Convert.ToSingle(inputTxt.Text);
                                            if (inputVal < val1)
                                            {
                                                inputTxt.BackColor = Color.LightCoral;
                                            }
                                        }
                                        if (val2 > 0)
                                        {
                                            float inputVal = Convert.ToSingle(inputTxt.Text);
                                            if (inputVal > val2)
                                            {
                                                inputTxt.BackColor = Color.LightCoral;
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                MessageBox.Show("没有初始化");
                            }

                            //if (obj != null) {
                            //    Label nowLbl = (Label)obj;
                            //    string lblTag =Convert.ToString(nowLbl.Tag);
                            //    float val1 = 0f, val2 = 0 ;
                            //    if (!string.IsNullOrWhiteSpace(lblTag)) {
                            //        string[] valArr = lblTag.Split(new char[] { '-'});
                            //        if (valArr.Length > 0) {
                            //            val1 =Convert.ToSingle(valArr[0]);
                            //            if (valArr.Length > 1) {
                            //                val2= Convert.ToSingle(valArr[1]);
                            //            }
                            //        }
                            //    }
                            //    if (val1 > 0) {
                            //        float inputVal = Convert.ToSingle(inputTxt.Text);
                            //        if (inputVal < val1){
                            //            inputTxt.BackColor = Color.LightCoral;
                            //        }
                            //    }
                            //    if (val2 > 0) {
                            //        float inputVal = Convert.ToSingle(inputTxt.Text);
                            //        if (inputVal>val2)
                            //        {
                            //            inputTxt.BackColor = Color.LightCoral;
                            //        }
                            //    }
                            //}
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("判断大小的时候错误.......");
                    }


                    int index = flpTabTwoTxtList.IndexOf(inputTxt);
                    if (index < flpTabTwoTxtList.Count - 1)
                        index++;
                    TextBox tb = flpTabTwoTxtList[index];
                    tb.Focus();
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

    }
}
