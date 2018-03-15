using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace YYOPInspectionClient
{
    public partial class ThreadingProcessForm : Form
    {
        private IndexWindow indexWindow;
        private MainWindow mainWindow;
        YYKeyenceReaderConsole codeReaderWindow;
        private List<string> videoTimestamp=new List<string>();
        public ThreadingProcessForm(IndexWindow indexWindow,MainWindow mainWindow)
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex =0;
            this.indexWindow = indexWindow;
            this.mainWindow = mainWindow;
            codeReaderWindow = new YYKeyenceReaderConsole(this);
        }

        #region 开始扫码事件
        private void button1_Click(object sender, EventArgs e)
        {
            string btnName = this.button1.Text;
            if (btnName.Equals("开始扫码"))
            {
                //首先连接扫码器
                codeReaderWindow.codeReaderConnect();
                //然后出发扫码
                codeReaderWindow.codeReaderLon();
                //获取扫码结果
                //codeReaderWindow.codeReaderReceive();
                this.button1.Text = "结束扫码";
            }
            else
            {
                //首先关闭扫码器
                codeReaderWindow.codeReaderOff();
                //窗体关闭时监听  关闭连接
                this.button1.Text = "开始扫码";
            }
        }
        #endregion

        #region 开始录像事件
        private void button2_Click(object sender, EventArgs e)
        {
                string btnName = this.button2.Text;
                if (btnName.Equals("开始录像"))
                {
                    videoTimestamp.Add(getMesuringRecord());
                    //先判断是否登录
                    mainWindow.recordLogin();
                    //然后预览
                    mainWindow.recordPreview();
                    //然后开始录像
                    mainWindow.RecordVideo(videoTimestamp[videoTimestamp.Count-1].ToString());
                    this.button2.Text = "结束录像";
                }
                else
                {
                    //停止录制的时候就是 先停止录制，然后停止预览，最后退出登陆
                    mainWindow.RecordVideo(videoTimestamp[videoTimestamp.Count - 1].ToString());
                    mainWindow.recordPreview();
                    mainWindow.recordLogin();

                    this.button2.Text = "开始录像";
                }
        }
        #endregion

        #region 表单提交事件
        private void button3_Click(object sender, EventArgs e)
        {
            //获取时间戳，生成唯一工具使用记录编号
            formSubmit();

        }
        #endregion

        #region  根据时间戳动态生成使用测量工具编号
        private string getMesuringRecord()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        } 
        #endregion

        #region 获取检验结果
        private int getInspectionResult(string str)
        {
            int result = 0;
            switch (str)
            {
                case "不合格":
                    result = 1;
                    break;
                case "":
                    result = 2;
                    break;
            }
            return result;
        } 
        #endregion

        #region 表单提交
        private void formSubmit()
        {
            //-----------------螺纹检验使用工具表
            string tool_measuring_record_no = HttpUtility.UrlEncode(getMesuringRecord(), Encoding.UTF8);//螺纹测量工具编号（时间戳获取）
            string thread_pitch_gauge_no = HttpUtility.UrlEncode(this.textBox1.Text, Encoding.UTF8);//螺纹中径规编号
            string thread_pitch_calibration_framwork = HttpUtility.UrlEncode(this.textBox2.Text, Encoding.UTF8);//螺纹中径校对对座架
            string sealing_surface_gauge_no = HttpUtility.UrlEncode(this.textBox3.Text, Encoding.UTF8);//密封面规编号
            string sealing_surface_calibration_ring_no = HttpUtility.UrlEncode(this.textBox4.Text, Encoding.UTF8);//密封面校对环编号
            string depth_caliper_no = HttpUtility.UrlEncode(this.textBox5.Text, Encoding.UTF8);//深度游表卡尺编号
            string threading_distance_gauge_no = HttpUtility.UrlEncode(this.textBox6.Text, Encoding.UTF8);//螺距表编号
            string thread_distance_calibration_sample_no = HttpUtility.UrlEncode(this.textBox7.Text, Encoding.UTF8);//螺距样块编号
            string taper_gauge_no = HttpUtility.UrlEncode(this.textBox8.Text, Encoding.UTF8);//锥度表编号
            string tooth_height_gauge_no = HttpUtility.UrlEncode(this.textBox9.Text, Encoding.UTF8);//齿高表编号
            string tooth_height_calibration_sample_no = HttpUtility.UrlEncode(this.textBox10.Text, Encoding.UTF8);//齿高样块编号
            string tooth_width_stop_gauge_no = HttpUtility.UrlEncode(this.textBox11.Text, Encoding.UTF8);//齿宽止通规
            string thread_min_length_sample_no = HttpUtility.UrlEncode(this.textBox12.Text, Encoding.UTF8);//螺纹最小长度样板
            string coupling_length_sample_no = HttpUtility.UrlEncode(this.textBox13.Text, Encoding.UTF8);//接箍长度样板
            string caliper_no = HttpUtility.UrlEncode(this.textBox14.Text, Encoding.UTF8);//游标卡尺编号
            string caliper_tolerance = HttpUtility.UrlEncode(this.textBox15.Text, Encoding.UTF8);//游标卡尺零值误差
            string collar_gauge_no = HttpUtility.UrlEncode(this.textBox16.Text, Encoding.UTF8);//内径止通规编号
            //------------------螺纹检验表
            string couping_no = HttpUtility.UrlEncode(this.textBox17.Text, Encoding.UTF8);//接箍编号
            string process_no = HttpUtility.UrlEncode(this.textBox20.Text, Encoding.UTF8);//工位编号
            string operator_no = HttpUtility.UrlEncode(this.textBox19.Text, Encoding.UTF8);//操作工编号
            string visual_inspection = HttpUtility.UrlEncode(this.textBox21.Text, Encoding.UTF8);//视觉检验
            string thread_tooth_pitch_diameter_max = HttpUtility.UrlEncode(this.textBox22.Text, Encoding.UTF8);//螺纹齿顶中径最大值
            string thread_tooth_pitch_diameter_avg = HttpUtility.UrlEncode(this.textBox23.Text, Encoding.UTF8);//螺纹齿顶中径平均值
            string thread_tooth_pitch_diameter_min = HttpUtility.UrlEncode(this.textBox24.Text, Encoding.UTF8);//螺纹齿顶中径最小值
            string thread_sealing_surface_diameter_max = HttpUtility.UrlEncode(this.textBox25.Text, Encoding.UTF8);//螺纹密封面直径最大值
            string thread_sealing_surface_diameter_avg = HttpUtility.UrlEncode(this.textBox26.Text, Encoding.UTF8);//螺纹密封面直径平均值
            string thread_sealing_surface_diameter_min = HttpUtility.UrlEncode(this.textBox27.Text, Encoding.UTF8);//螺纹密封面直径最小值
            string thread_sealing_surface_ovality = HttpUtility.UrlEncode(this.textBox28.Text, Encoding.UTF8);//螺纹及密封面椭圆度
            string thread_width = HttpUtility.UrlEncode(this.textBox29.Text, Encoding.UTF8);//螺纹齿宽
            string thread_pitch = HttpUtility.UrlEncode(this.textBox30.Text, Encoding.UTF8);//螺纹螺距
            string thread_taper = HttpUtility.UrlEncode(this.textBox31.Text, Encoding.UTF8);//螺纹锥度
            string thread_height = HttpUtility.UrlEncode(this.textBox32.Text, Encoding.UTF8);//螺纹齿高
            string thread_length_min = HttpUtility.UrlEncode(this.textBox34.Text, Encoding.UTF8);//最小螺纹长度
            string thread_bearing_surface_width = HttpUtility.UrlEncode(this.textBox35.Text, Encoding.UTF8);//承载面宽度
            string couping_inner_end_depth = HttpUtility.UrlEncode(this.textBox36.Text, Encoding.UTF8);//内端面宽度
            string thread_hole_inner_diameter = HttpUtility.UrlEncode(this.textBox37.Text, Encoding.UTF8);//通孔内径
            string couping_od = HttpUtility.UrlEncode(this.textBox38.Text, Encoding.UTF8);//接箍外径
            string couping_length = HttpUtility.UrlEncode(this.textBox39.Text, Encoding.UTF8);//接箍长度
            string thread_tooth_angle = HttpUtility.UrlEncode(this.textBox40.Text, Encoding.UTF8);//牙型角度
            string thread_throug_hole_size = HttpUtility.UrlEncode(this.textBox41.Text, Encoding.UTF8);//镗孔尺寸
            //然后搜索录制的视频文件获取文件名集合保存到video_no中
            string video_no =getVideoPath(videoTimestamp); //HttpUtility.UrlEncode(this.textBox2.Text, Encoding.UTF8);//视频编号
            string inspection_result = HttpUtility.UrlEncode(getInspectionResult(this.comboBox1.SelectedItem.ToString()).ToString(), Encoding.UTF8);//检验结果
            ASCIIEncoding encoding = new ASCIIEncoding();
            String content = "";
            try
            {
                //拼接json格式字符串
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"arg1\"" + ":" + "\"" + tool_measuring_record_no + "\",");
                sb.Append("\"arg2\"" + ":" + "\"" + thread_pitch_gauge_no + "\",");
                sb.Append("\"arg3\"" + ":" + "\"" + thread_pitch_calibration_framwork + "\",");
                sb.Append("\"arg4\"" + ":" + "\"" + sealing_surface_gauge_no + "\",");
                sb.Append("\"arg5\"" + ":" + "\"" + sealing_surface_calibration_ring_no + "\",");
                sb.Append("\"arg6\"" + ":" + "\"" + depth_caliper_no + "\",");
                sb.Append("\"arg7\"" + ":" + "\"" + threading_distance_gauge_no + "\",");
                sb.Append("\"arg8\"" + ":" + "\"" + thread_distance_calibration_sample_no + "\",");
                sb.Append("\"arg9\"" + ":" + "\"" + taper_gauge_no + "\",");
                sb.Append("\"arg10\"" + ":" + "\"" + tooth_height_gauge_no + "\",");
                sb.Append("\"arg11\"" + ":" + "\"" + tooth_height_calibration_sample_no + "\",");
                sb.Append("\"arg12\"" + ":" + "\"" + tooth_width_stop_gauge_no + "\",");
                sb.Append("\"arg13\"" + ":" + "\"" + thread_min_length_sample_no + "\",");
                sb.Append("\"arg14\"" + ":" + "\"" + coupling_length_sample_no + "\",");
                sb.Append("\"arg15\"" + ":" + "\"" + caliper_no + "\",");
                sb.Append("\"arg16\"" + ":" + "\"" + caliper_tolerance + "\",");
                sb.Append("\"arg17\"" + ":" + "\"" + collar_gauge_no + "\",");

                sb.Append("\"arg18\"" + ":" + "\"" + couping_no + "\",");
                sb.Append("\"arg19\"" + ":" + "\"" + process_no + "\",");
                sb.Append("\"arg20\"" + ":" + "\"" + operator_no + "\",");
                sb.Append("\"arg21\"" + ":" + "\"" + visual_inspection + "\",");
                sb.Append("\"arg22\"" + ":" + "\"" + thread_tooth_pitch_diameter_max + "\",");
                sb.Append("\"arg23\"" + ":" + "\"" + thread_tooth_pitch_diameter_avg + "\",");
                sb.Append("\"arg24\"" + ":" + "\"" + thread_tooth_pitch_diameter_min + "\",");
                sb.Append("\"arg25\"" + ":" + "\"" + thread_sealing_surface_diameter_max + "\",");
                sb.Append("\"arg26\"" + ":" + "\"" + thread_sealing_surface_diameter_avg + "\",");
                sb.Append("\"arg27\"" + ":" + "\"" + thread_sealing_surface_diameter_min + "\",");
                sb.Append("\"arg28\"" + ":" + "\"" + thread_sealing_surface_ovality + "\",");
                sb.Append("\"arg29\"" + ":" + "\"" + thread_width + "\",");
                sb.Append("\"arg30\"" + ":" + "\"" + thread_pitch + "\",");
                sb.Append("\"arg31\"" + ":" + "\"" + thread_taper + "\",");
                sb.Append("\"arg32\"" + ":" + "\"" + thread_height + "\",");
                sb.Append("\"arg33\"" + ":" + "\"" + thread_length_min + "\",");
                sb.Append("\"arg34\"" + ":" + "\"" + thread_bearing_surface_width + "\",");
                sb.Append("\"arg35\"" + ":" + "\"" + couping_inner_end_depth + "\",");
                sb.Append("\"arg36\"" + ":" + "\"" + thread_hole_inner_diameter + "\",");
                sb.Append("\"arg37\"" + ":" + "\"" + couping_od + "\",");
                sb.Append("\"arg38\"" + ":" + "\"" + couping_length + "\",");
                sb.Append("\"arg39\"" + ":" + "\"" + thread_tooth_angle + "\",");
                sb.Append("\"arg40\"" + ":" + "\"" + thread_throug_hole_size + "\",");
                sb.Append("\"arg41\"" + ":" + "\"" + video_no + "\",");
                sb.Append("\"arg42 \"" + ":" + "\"" + inspection_result + "\"");
                sb.Append("}");
                string json = sb.ToString();
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
                MessageBox.Show("返回的内容：" + content);
                string jsons = content;
                JObject jobject = JObject.Parse(jsons);
                MessageBox.Show(jobject["resultMsg"].ToString());
                bool result = Convert.ToBoolean(jobject["resultMsg"].ToString());
                MessageBox.Show(result.ToString());
                if (result)
                {
                    MessageBox.Show("上传成功！");

                    writeUnSubmitForm(getMesuringRecord(), json);
                }
                else {
                    MessageBox.Show("上传失败，系统将数据暂时存储，请在空闲时间提交(在主页数据管理功能处提交)！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常报错信息：" + ex.Message.ToString());
            }
            finally
            {
                //表单清理并关闭
                indexWindow.getThreadingProcessData();
                this.Close();
            }
        }
        #endregion

        #region 写入未提交成功的表单
        private void writeUnSubmitForm(string timestamp, string jsonData) 
        {
            string coupingDir = Application.StartupPath + "\\draft\\formbackup\\" + timestamp;
            if (!File.Exists(coupingDir))
            {
                Directory.CreateDirectory(coupingDir);
            }
            string coupingTxt = Application.StartupPath + "\\draft\\formbackup\\" + timestamp + "\\" + timestamp + ".txt";
            if (!File.Exists(coupingTxt))
            {
                FileStream fs = new FileStream(coupingTxt, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(jsonData);
                sw.Close();
                fs.Close();
            }
        }

        #endregion

        #region 窗体关闭事件
        private void ThreadingProcessForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            codeReaderWindow.codeReaderDisConnect();
        }
        #endregion

        #region 遍历录制视频文件获取文件名集合
        private string getVideoPath(List<string> timestamp)
        {
            string pathList = "";
            if (timestamp != null&&timestamp.Count>0) {
                foreach (string item in timestamp) {
                    string path = Application.StartupPath + "\\draft\\" + item + "\\";
                    if (Directory.Exists(path)) {
                        DirectoryInfo folder = new DirectoryInfo(path);
                        foreach (FileInfo file in folder.GetFiles("*.mp4"))
                        {
                            pathList += file.Name + ";";
                        }
                    }
                }
            }
            return pathList;
        } 
        #endregion
    } 
}
