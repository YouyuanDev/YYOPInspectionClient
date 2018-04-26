using System;
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
                    this.Textbox_display.Text += num;
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
            //回车,清楚末尾无用的.
            if(this.Textbox_display.Text.Contains(".")&& this.Textbox_display.Text.LastIndexOf('.')== this.Textbox_display.Text.Length - 1)
            {
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);
            }
            //
            if (inputTxt != null) {
                inputTxt.Text = this.Textbox_display.Text.Trim();
                this.Textbox_display.Text = "";
                int index = flpTabTwoTxtList.IndexOf(inputTxt);
                if (index < flpTabTwoTxtList.Count - 1)
                    index++;
                TextBox tb = flpTabTwoTxtList[index];
                tb.Focus();
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
    }
}
