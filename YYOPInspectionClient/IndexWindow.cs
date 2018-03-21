using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
   
    public partial class IndexWindow : Form
    {
        private static Thread thread = null;
        public IndexWindow()
        {
            InitializeComponent();
            getThreadingProcessData();
            try {
                thread = new Thread(UploadVideo);
                thread.Start();
                thread.IsBackground = true;
            }
            catch(Exception e)
            {
                thread.Abort();
            }
           
        }

        private static void UploadVideo()
        {
            while (true)
            {
                //遍历视频文件
                string path = Application.StartupPath + "\\draft";
                if (Directory.Exists(path))
                {
                    DirectoryInfo folder = new DirectoryInfo(path);
                    foreach (DirectoryInfo sonFolder in folder.GetDirectories())
                    {
                        foreach (FileInfo file in sonFolder.GetFiles("*.mp4"))
                        {
                            //获取文件路径 
                            FileInfo info = new FileInfo(file.FullName);
                            FtpUtil.UploadFile(sonFolder, info);
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }

        #region 分页查询获取数据

        public void getThreadingProcessData()
        {
            try
            {
                string couping_no = HttpUtility.UrlEncode(this.textBox1.Text.Trim(), Encoding.UTF8);
                string operator_no = HttpUtility.UrlEncode(this.textBox2.Text.Trim(), Encoding.UTF8);
                string begin_time = HttpUtility.UrlEncode(this.dtpBeginTime.Value.ToString("yyyy-MM-dd"), Encoding.UTF8);
                string end_time = HttpUtility.UrlEncode(this.dtpEndTime.Value.ToString("yyyy-MM-dd"), Encoding.UTF8);
                string pageCurrent = HttpUtility.UrlEncode("", Encoding.UTF8);
               string pageSize = HttpUtility.UrlEncode("", Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"couping_no\"" + ":" + "\"" + couping_no + "\",");
                sb.Append("\"operator_no\"" + ":" + "\"" + operator_no + "\",");
                sb.Append("\"begin_time\"" + ":" + "\"" + begin_time + "\",");
                sb.Append("\"end_time\"" + ":" + "\"" + end_time + "\",");
                sb.Append("\"pageCurrent\"" + ":" + "\"" + pageCurrent + "\",");
                sb.Append("\"pageSize\"" + ":" + "\"" + pageSize + "\"");
                sb.Append("}");
                ASCIIEncoding encoding = new ASCIIEncoding();
                String content = "";
                JObject o = JObject.Parse(sb.ToString());
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://192.168.0.200:8080/ThreadingOperation/getThreadingProcessByLike.action");
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json;characterSet:UTF-8";
                request.ContentLength = data.Length;
                Stream sm = request.GetRequestStream();
                sm.Write(data, 0, data.Length);
                sm.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                Char[] readBuff = new Char[256];
                int count = streamRead.Read(readBuff, 0, 256);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    content += outputData;
                    count = streamRead.Read(readBuff, 0, 256);
                }
                response.Close();
                string jsons = content;
                JObject jobject = JObject.Parse(jsons);
                string rowsJson = jobject["rowsData"].ToString();
                List<ThreadingProcess> list = JsonConvert.DeserializeObject<List<ThreadingProcess>>(rowsJson);
                this.dataGridView1.DataSource =list;
            }
            catch (Exception e) {
                MessageBox.Show("服务器尚未开启......");
            }
           
        }
        #endregion


        private void btnSearch_Click(object sender, EventArgs e)
        {
            getThreadingProcessData();
        }
 

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            ThreadingProcessForm form = new ThreadingProcessForm(this, mainWindow);
            form.Show();
        }

        private void 未提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnSubmitForm form = new UnSubmitForm();
            form.Show();
        }

        private void 读码器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            YYKeyenceReaderConsole window = new YYKeyenceReaderConsole();
            window.Show();
        }

        private void 录像设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void fTP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPSetting setting = new FTPSetting();
            setting.Show();
        }

        private void 开启读码器ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关闭读码器ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
