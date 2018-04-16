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
    public partial class ThreadingForm : Form
    {
        public ThreadingForm()
        {
            InitializeComponent();
        }
        #region 窗体风格事件

        private void splitBtn_Click(object sender, EventArgs e)
        {
            string iconDownPath = Application.StartupPath + "\\icon\\down.png";
            string iconUpPath = Application.StartupPath + "\\icon\\up.png";
            if (this.splitBtn.Tag.Equals("down"))
            {

                this.splitContainer1.SplitterDistance = 25;
                this.splitContainer1.FixedPanel = FixedPanel.Panel1;
                this.splitContainer1.Panel1.AutoScroll = false;
                this.splitBtn.Image = Image.FromFile(iconDownPath);
                this.splitBtn.Tag = "up";
            }
            else
            {
                this.splitContainer1.SplitterDistance =185;
                this.splitContainer1.FixedPanel = FixedPanel.Panel1;
                this.splitContainer1.Panel1.AutoScroll = false;
                this.splitBtn.Image = Image.FromFile(iconUpPath);
                this.splitBtn.Tag = "down";
            }
        } 
        #endregion
    }
}
