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
    public partial class AlphabetKeyboardForm : Form
    {
        public  TextBox inputTxt;
        public  List<TextBox> flpTabOneTxtList;
        public Control containerControl = null;
        public AlphabetKeyboardForm()
        {
            InitializeComponent();
        }

        private void letternum_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text += ((Button)sender).Text;
        }

        private void button_cap_Click(object sender, EventArgs e)
        {
            //切换大小写
            if (((Button)sender).Text.Equals("CAP"))
            {//大写切换为小写
                ((Button)sender).Text = "cap";
                this.buttonA.Text = this.buttonA.Text.ToLower();
                this.buttonB.Text = this.buttonB.Text.ToLower();
                this.buttonC.Text = this.buttonC.Text.ToLower();
                this.buttonD.Text = this.buttonD.Text.ToLower();
                this.buttonE.Text = this.buttonE.Text.ToLower();
                this.buttonF.Text = this.buttonF.Text.ToLower();
                this.buttonG.Text = this.buttonG.Text.ToLower();
                this.buttonH.Text = this.buttonH.Text.ToLower();
                this.buttonI.Text = this.buttonI.Text.ToLower();
                this.buttonJ.Text = this.buttonJ.Text.ToLower();
                this.buttonK.Text = this.buttonK.Text.ToLower();
                this.buttonL.Text = this.buttonL.Text.ToLower();
                this.buttonM.Text = this.buttonM.Text.ToLower();
                this.buttonN.Text = this.buttonN.Text.ToLower();
                this.buttonO.Text = this.buttonO.Text.ToLower();
                this.buttonP.Text = this.buttonP.Text.ToLower();
                this.buttonQ.Text = this.buttonQ.Text.ToLower();
                this.buttonR.Text = this.buttonR.Text.ToLower();
                this.buttonS.Text = this.buttonS.Text.ToLower();
                this.buttonT.Text = this.buttonT.Text.ToLower();
                this.buttonU.Text = this.buttonU.Text.ToLower();
                this.buttonV.Text = this.buttonV.Text.ToLower();
                this.buttonW.Text = this.buttonW.Text.ToLower();
                this.buttonX.Text = this.buttonX.Text.ToLower();
                this.buttonY.Text = this.buttonY.Text.ToLower();
                this.buttonZ.Text = this.buttonZ.Text.ToLower();
            }
            else
            {//小写切换为大写
                ((Button)sender).Text = "CAP";
                this.buttonA.Text = this.buttonA.Text.ToUpper();
                this.buttonB.Text = this.buttonB.Text.ToUpper();
                this.buttonC.Text = this.buttonC.Text.ToUpper();
                this.buttonD.Text = this.buttonD.Text.ToUpper();
                this.buttonE.Text = this.buttonE.Text.ToUpper();
                this.buttonF.Text = this.buttonF.Text.ToUpper();
                this.buttonG.Text = this.buttonG.Text.ToUpper();
                this.buttonH.Text = this.buttonH.Text.ToUpper();
                this.buttonI.Text = this.buttonI.Text.ToUpper();
                this.buttonJ.Text = this.buttonJ.Text.ToUpper();
                this.buttonK.Text = this.buttonK.Text.ToUpper();
                this.buttonL.Text = this.buttonL.Text.ToUpper();
                this.buttonM.Text = this.buttonM.Text.ToUpper();
                this.buttonN.Text = this.buttonN.Text.ToUpper();
                this.buttonO.Text = this.buttonO.Text.ToUpper();
                this.buttonP.Text = this.buttonP.Text.ToUpper();
                this.buttonQ.Text = this.buttonQ.Text.ToUpper();
                this.buttonR.Text = this.buttonR.Text.ToUpper();
                this.buttonS.Text = this.buttonS.Text.ToUpper();
                this.buttonT.Text = this.buttonT.Text.ToUpper();
                this.buttonU.Text = this.buttonU.Text.ToUpper();
                this.buttonV.Text = this.buttonV.Text.ToUpper();
                this.buttonW.Text = this.buttonW.Text.ToUpper();
                this.buttonX.Text = this.buttonX.Text.ToUpper();
                this.buttonY.Text = this.buttonY.Text.ToUpper();
                this.buttonZ.Text = this.buttonZ.Text.ToUpper();
            }


        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            //清屏
            this.Textbox_display.Text = "";
        }
        private void button_close_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "";
            this.Hide();
        }

        private void button_enter_Click(object sender, EventArgs e)
        {
            //输入
            if (inputTxt != null){
                inputTxt.Text = Textbox_display.Text.Trim();
                this.Textbox_display.Text = "";
                int index = flpTabOneTxtList.IndexOf(inputTxt);
                if(index<flpTabOneTxtList.Count-1)
                    index++;
                TextBox tb = flpTabOneTxtList[index];
                tb.Focus();
            }
        }

        private void button_backspace_Click(object sender, EventArgs e)
        {
            //退格键
            if (this.Textbox_display.Text.Length > 1)
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);

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
