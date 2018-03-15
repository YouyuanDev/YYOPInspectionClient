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
using System.Web;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class IndexWindow : Form
    {
        public IndexWindow()
        {
            InitializeComponent();
            getThreadingProcessData();
        }

        

        private void VideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void CodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            YYKeyenceReaderConsole window = new YYKeyenceReaderConsole();
            window.Show();
        }

        private void NewFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            ThreadingProcessForm form = new ThreadingProcessForm(this,mainWindow);
            form.Show();
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
                string pageCurrent = HttpUtility.UrlEncode(this.textBox3.Text.Trim(), Encoding.UTF8);
                string pageSize = HttpUtility.UrlEncode(this.textBox4.Text.Trim(), Encoding.UTF8);
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
                MessageBox.Show(e.Message);
            }
           
        }
        #endregion

        

        private void btnSearch_Click(object sender, EventArgs e)
        {
            getThreadingProcessData();
        }

        private void unSubmitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnSubmitForm form = new UnSubmitForm();
            form.Show();

        }
    }
}
