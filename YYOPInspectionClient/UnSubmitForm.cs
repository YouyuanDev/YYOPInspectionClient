using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class UnSubmitForm : Form
    {
        public UnSubmitForm()
        {
            InitializeComponent();
            //getUnSummitFile();
        }
        #region 获取所有没有上传的数据文件
        private void getUnSummitFile()
        {
            string path = Application.StartupPath + "\\draft\\formbackup\\";
            if (Directory.Exists(path)) {
                DirectoryInfo folder = new DirectoryInfo(path);
                foreach (DirectoryInfo dirInfo in folder.GetDirectories())
                {
                    foreach (FileInfo file in dirInfo.GetFiles("*.txt"))
                    {
                        dataGridView1.Rows.Add(file.Name);//显示 
                    }

                }
            }

        } 
        #endregion
    }
}
