using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class UnSubmitForm : Form
    {
        private ASCIIEncoding encoding = new ASCIIEncoding();
        private string content = "";
        private int totalForm = 0;
        private Dictionary<string, string> videoPathList = new Dictionary<string, string>();
        //---------------------拖动无窗体的控件(开始)
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //---------------------拖动无窗体的控件(结束)
        public UnSubmitForm()
        {
            InitializeComponent();
            this.Font = new Font("宋体", 12, FontStyle.Bold);
            getUnSummitFile();
        }
        #region 获取所有没有上传的数据文件
        private void getUnSummitFile()
        {
            dataGridView1.Rows.Clear();
            string path = Application.StartupPath + "\\unsubmit\\";
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles("*.txt"))
                {
                    dataGridView1.Rows.Add(false, file.Name);//显示 
                }
            }
        }
        #endregion

        #region 全选事件
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (this.btnSelectAll.Text == "全选")
            {
                for (int i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = true;
                }
                if (this.dataGridView1.Rows.Count > 0)
                {
                    this.btnSelectAll.Text = "取消全选";
                }
            }
            else
            {
                for (int i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = false;
                }
                this.btnSelectAll.Text = "全选";
            }
        }
        #endregion

        #region 上传未提交表单事件
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> listFormDir = new Dictionary<string, string>();
                DataGridViewCheckBoxCell cell = null;
                if (this.dataGridView1.Rows.Count > 0)
                {
                    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                    {
                        cell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[i].Cells["checkbox1"];
                        bool flag = Convert.ToBoolean(cell.Value);
                        if (flag)
                        {
                            listFormDir.Add(this.dataGridView1.Rows[i].Cells["filename"].Value.ToString(), "");
                        }
                    }
                    totalForm = listFormDir.Count();
                    string path = Application.StartupPath + "\\unsubmit\\";
                    //获取选中表单的路径集合
                    DirectoryInfo folder = new DirectoryInfo(path);
                    string dirName = "";
                    foreach (FileInfo file in folder.GetFiles("*.txt"))
                    {
                        if (listFormDir.ContainsKey(file.Name))
                        {
                            listFormDir[file.Name] = path + dirName + "\\" + file.Name;
                        }
                    }

                    //如果有选中的数
                    if (listFormDir.Count > 0)
                    {
                        int tempTotal = 0;
                        string jsonContent = "";
                        //遍历listPath找到未提交文件，然后读出json数据
                        foreach (string item in listFormDir.Values)
                        {
                            jsonContent = File.ReadAllText(item, Encoding.UTF8).Trim();
                            if (jsonContent != null && jsonContent.Length > 0)
                            {
                                if (uploadUnSubmitForm(jsonContent))
                                {
                                    //如果上传成功删除文件
                                    File.Delete(item);
                                    tempTotal++;
                                }
                                else
                                {
                                    MessagePrompt.Show("上传出现问题,总共上传" + totalForm + "个，成功" + tempTotal + "个!");
                                    break;
                                }
                                jsonContent = "";
                            }
                        }
                        MessagePrompt.Show("上传完毕，" + tempTotal + "个成功" + (totalForm - tempTotal) + "个失败!");
                        getUnSummitFile();
                    }
                    else
                    {
                        MessagePrompt.Show("请选中要上传的表单！");
                    }
                }
                else
                {
                    MessagePrompt.Show("暂未有可提交的表单！");
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("系统出了点问题！错误原因:" + ex.Message + ",请联系系统人员维护！");
            }

        }
        #endregion

        #region 上传事件
        private bool uploadUnSubmitForm(string json)
        {
            bool flag = false;
            try
            {
                // JObject o = JObject.Parse(json);
                //String param = o.ToString();
                byte[] data = encoding.GetBytes(json);
                string url = CommonUtil.getServerIpAndPort() + "ThreadingOperation/saveThreadInspectionRecordOfWinform.action";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json;characterSet:utf-8";
                request.ContentLength = data.Length;
                using (Stream sm = request.GetRequestStream())
                {
                    sm.Write(data, 0, data.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                Char[] readBuff = new Char[1024];
                int count = streamRead.Read(readBuff, 0, 1024);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    content += outputData;
                    count = streamRead.Read(readBuff, 0, 1024);
                }
                response.Close();
                string jsons = content;
                if (jsons != null)
                {
                    JObject jobject = JObject.Parse(jsons);
                    string rowsJson = jobject["rowsData"].ToString();
                    if (rowsJson.Trim().Equals("success"))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 获取未上传的所有视频录像路径集合
        private Dictionary<string, string> getVideoPathList(string path)
        {
            //遍历draft下非formbackup目录下的所有视频文件
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo file in root.GetDirectories())
            {
                if (file.Name != "formbackup")
                {
                    foreach (FileInfo fileInfo in file.GetFiles("*.mp4"))
                    {
                        videoPathList.Add(fileInfo.Name, fileInfo.FullName);
                    }
                }
                getVideoPathList(file.FullName);
            }
            return videoPathList;
        }



        #endregion


        #region 窗体关闭事件
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 窗体绘制
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel1.ClientRectangle,
           Color.DimGray, 3, ButtonBorderStyle.Solid, //左边
           Color.DimGray, 3, ButtonBorderStyle.Solid, //上边
          Color.DimGray, 3, ButtonBorderStyle.Solid, //右边
          Color.DimGray, 0, ButtonBorderStyle.Solid);//底边
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
           Color.DimGray, 3, ButtonBorderStyle.Solid, //左边
           Color.DimGray, 0, ButtonBorderStyle.Solid, //上边
           Color.DimGray, 3, ButtonBorderStyle.Solid, //右边
             Color.DimGray, 3, ButtonBorderStyle.Solid);//底边
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        #endregion

    }
}
