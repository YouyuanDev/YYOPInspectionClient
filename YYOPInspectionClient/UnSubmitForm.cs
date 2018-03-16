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
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class UnSubmitForm : Form
    {
        private ASCIIEncoding encoding = new ASCIIEncoding();
        private string content = "";
        private int totalForm = 0;
        private Dictionary<string,string> videoPathList=new Dictionary<string,string>();
        public UnSubmitForm()
        {
            InitializeComponent();
            getUnSummitFile();
        }
        #region 获取所有没有上传的数据文件
        private void getUnSummitFile()
        {
            dataGridView1.Rows.Clear();
            string path = Application.StartupPath + "\\draft\\";
            if (Directory.Exists(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                foreach (DirectoryInfo dirInfo in folder.GetDirectories())
                {
                    foreach (FileInfo file in dirInfo.GetFiles("*.txt"))
                    {
                        dataGridView1.Rows.Add(false, file.Name);//显示 
                    }

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
                this.btnSelectAll.Text = "取消全选";
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
                List<string> listForm = new List<string>();
                List<string> listPath = new List<string>();
                DataGridViewCheckBoxCell cell = null;
                if (this.dataGridView1.Rows.Count > 0)
                {
                    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                    {
                        cell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[i].Cells["checkbox1"];
                        bool flag = Convert.ToBoolean(cell.Value);
                        if (flag)
                        {
                            listForm.Add(this.dataGridView1.Rows[i].Cells["filename"].Value.ToString());
                        }
                    }
                    totalForm = listForm.Count;
                    string path = Application.StartupPath + "\\draft\\";
                    //获取选中表单的路径集合
                    DirectoryInfo folder = new DirectoryInfo(path);
                    string dirName = "";
                    foreach (DirectoryInfo dirInfo in folder.GetDirectories())
                    {
                          dirName = dirInfo.Name;
                          foreach (FileInfo file in dirInfo.GetFiles("*.txt"))
                          {
                               if (listForm.Contains(file.Name)) {
                                    listPath.Add(path + dirName+"\\" + file.Name);//显示 
                               }
                          }

                     }
                    //如果有选中的数
                    if (listForm.Count > 0&&listPath.Count>0)
                    {
                        int tempTotal = 0;
                        string jsonContent = "";
                        //遍历listPath找到未提交文件，然后读出json数据
                        for (int i = 0; i < listPath.Count; i++) {
                            jsonContent = File.ReadAllText(listPath[i], Encoding.UTF8).Trim();
                            if (jsonContent != null && jsonContent.Length > 0) {
                                if (uploadUnSubmitForm(jsonContent)) {
                                    //如果上传成功删除文件
                                    File.Delete(listPath[i]);
                                    string fatherDir = Directory.GetParent(listPath[i]).FullName;
                                    //然后判断父目录中还有文件没，如果有父目录就不删除
                                    if (!Directory.EnumerateDirectories(fatherDir, "*.*", SearchOption.AllDirectories).Any()) {
                                        Directory.Delete(fatherDir);
                                    }
                                    tempTotal++;
                                }else
                                {
                                    MessageBox.Show("上传出现问题,总共上传"+totalForm+"个，成功"+tempTotal+"个!");
                                    break;
                                }
                            }
                            jsonContent = "";
                        }
                        MessageBox.Show("上传完毕，" +tempTotal + "个成功" +(totalForm-tempTotal) + "个失败!");
                        getUnSummitFile();
                    }
                    else
                    {
                        MessageBox.Show("请选中要上传的表单！");
                    }
                }
                else
                {
                    MessageBox.Show("暂未有可提交的表单！");
                }
            }
            catch (Exception ex) {
                MessageBox.Show("系统出了点问题！错误原因:"+ex.Message+",请联系系统人员维护！");
            }
           
        }
        #endregion

        #region 上传事件
        private bool uploadUnSubmitForm(string json)
        {
            bool flag = false;
            try
            {
                JObject o = JObject.Parse(json);
                String param = o.ToString();
                byte[] data = encoding.GetBytes(param);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://192.168.0.200:8080/ThreadingOperation/saveThreadingProcessByWinform.action");
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
                // MessageBox.Show("返回的内容：" + content);
                string jsons = content;
                JObject jobject = JObject.Parse(jsons);
                // MessageBox.Show(jobject["resultMsg"].ToString());
                bool result = Convert.ToBoolean(jobject["resultMsg"].ToString());
                // MessageBox.Show(result.ToString());
                if (result)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
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
        private Dictionary<string,string> getVideoPathList(string path)
        {
            //遍历draft下非formbackup目录下的所有视频文件
            DirectoryInfo root = new DirectoryInfo(path);
            foreach(DirectoryInfo file in root.GetDirectories()) {
                if (file.Name != "formbackup") {
                    foreach (FileInfo fileInfo in file.GetFiles("*.mp4")) {
                        videoPathList.Add(fileInfo.Name,fileInfo.FullName);
                    }
                }
                getVideoPathList(file.FullName);
            }
            return videoPathList;
        }
        #endregion

        
        //private static void UploadVideo()
        //{
        //    string path = Application.StartupPath + "\\draft";
        //    if (Directory.Exists(path))
        //    {
        //        DirectoryInfo folder = new DirectoryInfo(path);
        //        foreach (DirectoryInfo folderMp4 in folder.GetDirectories()) {
        //            foreach (FileInfo file in folderMp4.GetFiles("*.mp4"))
        //            {
        //                //获取文件路径
        //                MessageBox.Show(file.FullName);
        //                if (File.Exists(file.FullName)) {
        //                    FileInfo info = new FileInfo(file.FullName);
        //                    if (FtpUtil.UploadFile(folderMp4,info))
        //                    {
        //                        //File.Delete(info.FullName);
        //                        //string fatherDir = Directory.GetParent(info.FullName).FullName;
        //                        ////然后判断父目录中还有文件没，如果有父目录就不删除
        //                        //if ((folderMp4.GetFiles().Length + folderMp4.GetDirectories().Length) == 0)
        //                        //{
        //                        //    Directory.Delete(fatherDir);
        //                        //}
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
