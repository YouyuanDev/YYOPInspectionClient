using System;
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
    public partial class MessagePrompt : Form
    {
        //---------------------拖动无窗体的控件(开始)
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public static MessagePrompt messagePrompt=null;
        public MessagePrompt()
        {
            InitializeComponent();
        }
        #region 自定义提示框显示事件

        public static void Show(string msg)
        {

            if (messagePrompt != null)
            {
                messagePrompt.Show();
                messagePrompt.TopMost = true;
            }
            else
            {
                messagePrompt = new MessagePrompt();
                messagePrompt.TopMost = true;
                messagePrompt.Show();
            }
            messagePrompt.label2.Text = msg;
            messagePrompt.BringToFront();
            messagePrompt.TopMost = true;
        } 
        #endregion

        #region 绘制边框
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
          Color.DimGray, 2, ButtonBorderStyle.Solid, //左边
          Color.DimGray, 0, ButtonBorderStyle.Solid, //上边
          Color.DimGray, 2, ButtonBorderStyle.Solid, //右边
            Color.DimGray, 2, ButtonBorderStyle.Solid);//底边
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion

        #region 关闭提示框
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        } 
        #endregion
    }
}
