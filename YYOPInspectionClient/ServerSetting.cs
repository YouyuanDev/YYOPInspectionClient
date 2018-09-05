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
    public partial class ServerSetting : Form
    {
        #region 构造函数
        public ServerSetting()
        {
            InitializeComponent();
        }
        #endregion

        #region 窗体关闭事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
        #endregion
    }
}
