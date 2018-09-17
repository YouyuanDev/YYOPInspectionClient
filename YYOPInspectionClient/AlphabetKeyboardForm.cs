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
    public partial class AlphabetKeyboardForm : Form
    {
        //定义标识(判断输入框实在那个窗体中,0代表在ThreadingForm中,1代表在DetailForm中)
        public static int flag = 0;
        //定义英文输入法弹出时对应的鼠标焦点所在的TextBox控件
        public static  TextBox inputTxt=null;
        //定义保存测量工具编号的TextBox控件集合
        //public static List<TextBox> flpTabOneTextBoxList = new List<TextBox>();
        //定义存放测量工具编号的TextBox控件的容器控件
        //public Control containerControl = null;
        //定义当前窗体
        private static AlphabetKeyboardForm myForm = null;
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
        public static AlphabetKeyboardForm getForm()
        {
            if (myForm == null)
            {
                new AlphabetKeyboardForm();
            }

            return myForm;
        }
        #endregion

        #region 构造函数
        private AlphabetKeyboardForm()
        {
            InitializeComponent();
            myForm = this;
        }
        #endregion

        #region 数字点击事件
        private void letternum_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text += ((Button)sender).Text;
        }
        #endregion

        #region CAP点击事件(切换大小写)
        private void button_cap_Click(object sender, EventArgs e)
        {
            //切换大小写
            if (((Button)sender).Text.Contains("CAP"))
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
        #endregion

        #region 清空输入法内容事件
        private void button_clear_Click(object sender, EventArgs e)
        {
            //清屏
            this.Textbox_display.Text = "";
        }
        #endregion
       
        #region 关闭输入法事件
        private void button_close_Click(object sender, EventArgs e)
        {
            this.Textbox_display.Text = "";
            this.Hide();
        }
        #endregion

        #region 输入法Enter(确定)事件
        private void button_enter_Click(object sender, EventArgs e)
        {
            try
            {
                //定义特殊的输入框名称数组(产线、用户名、密码、接箍编号、炉号、批号、机床号)
                string[] filterArr = { "txtProductionArea", "txtLoginName", "txtLoginPwd", "txtCoupingNo",
                "txtHeatNo", "txtBatchNo", "txtMachineNo" };
                if (inputTxt != null)
                {
                    inputTxt.Text = Textbox_display.Text.Trim();
                    this.Textbox_display.Text = "";
                    //如果此时获取焦点的输入框的名称在数组中，则点击Enter后隐藏输入法
                    if (filterArr.Contains(inputTxt.Name))
                    {
                        this.Hide();
                    }
                }
                //查询鼠标焦点所在的TextBox控件在控件集合中的索引
                TextBox tb = null;int index = 0;
                if (flag == 0)
                {
                    index =ThreadingForm.flpTabOneTextBoxList.IndexOf(inputTxt);
                    if (index < ThreadingForm.flpTabOneTextBoxList.Count - 1)
                        index++;
                    //设置鼠标焦点在控件集合索引为index的控件上
                    tb = ThreadingForm.flpTabOneTextBoxList[index];
                }
                else if (flag == 1) {
                    index = DetailForm.flpTabOneTextBoxList.IndexOf(inputTxt);
                    if (index < DetailForm.flpTabOneTextBoxList.Count - 1)
                        index++;
                    //设置鼠标焦点在控件集合索引为index的控件上
                    tb = DetailForm.flpTabOneTextBoxList[index];
                }
                tb.Focus();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 输入法Backspace(退格)事件
        private void button_backspace_Click(object sender, EventArgs e)
        {
            //如果当前输入法里有内容，则去除内容的最后一个字符
            if (this.Textbox_display.Text.Length > 1)
                this.Textbox_display.Text = this.Textbox_display.Text.Substring(0, this.Textbox_display.Text.Length - 1);

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
        #endregion

    }
}
