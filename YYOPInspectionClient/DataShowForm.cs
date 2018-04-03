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
    public partial class DataShowForm : Form
    {
        public DataShowForm()
        {
            InitializeComponent();
            this.Font = new Font("宋体",12, FontStyle.Bold);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
