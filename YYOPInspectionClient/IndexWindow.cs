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
        private YYKeyenceReaderConsole readerCodeWindow = null;
        private MainWindow videoWindow=null;
        public IndexWindow()
        {
            InitializeComponent();
            this.Font = new Font("宋体", 12, FontStyle.Bold);
            AutoSize autoSize= new AutoSize();
            autoSize.controllInitializeSize(this);
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
            //删除vcr中的垃圾视频
            string trashDir = Application.StartupPath + "\\vcr";
            if (Directory.Exists(trashDir)) {
                DirectoryInfo dirInfo = new DirectoryInfo(trashDir);
                foreach (FileInfo file in dirInfo.GetFiles("*.mp4")) {
                    File.Delete(file.FullName);
                }
            }

            string fileuploadpath = Application.StartupPath + "\\fileuploadrecord.txt";
            string path = Application.StartupPath + "\\draft";
            //按行读取出文件可上传的文件夹名
            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            
            string line = null;
            while (true)
            {
                //查询出可上传文件中的视频文件是哪些
                StreamReader sr = new StreamReader(fileuploadpath, Encoding.Default);
                while ((line = sr.ReadLine()) != null) {
                    dirs.Add(new DirectoryInfo(path+"\\"+line));
                }
                sr.Close();
                //遍历视频文件
                if (Directory.Exists(path))
                {
                    DirectoryInfo folder = new DirectoryInfo(path);
                    foreach (DirectoryInfo sonFolder in dirs)
                    {
                        if (Directory.Exists(sonFolder.FullName)) {
                            FileInfo[] files = sonFolder.GetFiles("*.mp4");
                            if (files.Length > 0)
                            {
                                foreach (FileInfo file in files)
                                {
                                    //获取文件路径 
                                    FileInfo info = new FileInfo(file.FullName);
                                    FtpUtil.UploadFile(sonFolder, info);
                                }
                            }
                            else
                            {
                                Directory.Delete(sonFolder.FullName);
                                Util.deleteDirName(sonFolder.Name);
                            }
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
               // MessageBox.Show(rowsJson);
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
            if (readerCodeWindow == null)
            {
                readerCodeWindow = new YYKeyenceReaderConsole();
            }
            readerCodeWindow.Show();
           
        }

        private void 录像设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videoWindow== null)
            {
                videoWindow = new MainWindow();
            }
            videoWindow.Show();
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

        private void IndexWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 判断读码器和录像机是否关闭
            DateTime start = DateTime.Now;
            DateTime now = DateTime.Now;
            TimeSpan ts = now - start;
            while (true)
            {
                ts = now - start;
                if (ts.TotalSeconds > 1)
                {
                    if (YYKeyenceReaderConsole.myselfForm != null) {
                        YYKeyenceReaderConsole.codeReaderOff();
                    }
                    break;
                }
                else
                {
                    now = DateTime.Now;
                }
            }
            if (MainWindow.mainWindowForm != null) {
                MainWindow.stopRecordVideo();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataShowForm form = new DataShowForm();
            form.Show();
            //string[] str = new string[dataGridView1.Rows.Count];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Selected == true)
                {

                    form.textBox1.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox2.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox3.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox4.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox5.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox6.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox7.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox8.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox9.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox10.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox11.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox12.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox13.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox14.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox15.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox16.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox17.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox18.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    form.textBox19.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    form.textBox20.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    form.textBox21.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    form.textBox22.Text = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    form.textBox23.Text = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    form.textBox24.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox25.Text = dataGridView1.Rows[i].Cells[8].Value.ToString();
                    form.textBox26.Text = dataGridView1.Rows[i].Cells[9].Value.ToString();
                    form.textBox27.Text = dataGridView1.Rows[i].Cells[7].Value.ToString();
                    form.textBox28.Text = dataGridView1.Rows[i].Cells[10].Value.ToString();
                    form.textBox29.Text = dataGridView1.Rows[i].Cells[11].Value.ToString();
                    form.textBox30.Text = dataGridView1.Rows[i].Cells[12].Value.ToString();
                    form.textBox31.Text = dataGridView1.Rows[i].Cells[13].Value.ToString();
                    form.textBox32.Text = dataGridView1.Rows[i].Cells[14].Value.ToString();
                    form.textBox33.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    form.textBox34.Text = dataGridView1.Rows[i].Cells[15].Value.ToString();
                    form.textBox35.Text = dataGridView1.Rows[i].Cells[16].Value.ToString();
                    form.textBox36.Text = dataGridView1.Rows[i].Cells[17].Value.ToString();
                    form.textBox37.Text = dataGridView1.Rows[i].Cells[18].Value.ToString();
                    form.textBox38.Text = dataGridView1.Rows[i].Cells[19].Value.ToString();
                    form.textBox39.Text = dataGridView1.Rows[i].Cells[20].Value.ToString();
                    form.textBox40.Text = dataGridView1.Rows[i].Cells[21].Value.ToString();
                    form.textBox41.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                }
            }
        }
    }
}
