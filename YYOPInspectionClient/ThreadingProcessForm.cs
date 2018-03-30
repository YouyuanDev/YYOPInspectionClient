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
        // YYKeyenceReaderConsole codeReaderWindow;
        AutoSize auto=new AutoSize();
        //时间戳(视频和form表单保存的目录名)
        private string timestamp = null;

        private static ThreadingProcessForm myForm = null;

       // CodeReader codeReader = null;

        public static ThreadingProcessForm getMyForm()
        {
            return myForm;
        }


        public ThreadingProcessForm(IndexWindow indexWindow,MainWindow mainWindow)
        {
            InitializeComponent();
            this.Font = new Font("宋体", 15, FontStyle.Bold);
            this.comboBox1.SelectedIndex =0;
            this.indexWindow = indexWindow;
            this.mainWindow = mainWindow;
            timestamp = getMesuringRecord();
           // codeReader= new CodeReader(this);
            myForm = this;
         }

        #region 开始扫码事件
        private void button1_Click(object sender, EventArgs e)
        {
            string btnName = this.button1.Text;
            if (btnName == "结束扫码")
            {
                //YYKeyenceReaderConsole
                this.lblReaderStatus.Text = "读码器连接成功...";
                YYKeyenceReaderConsole.codeReaderOff();
                this.button1.Text = "开始扫码";
                this.lblReaderStatus.Text = "读码完成...";
            }
            else if (btnName == "开始扫码") {
                //先判断是否连接上读码器，如果没有连接上则提示
                int resLon = YYKeyenceReaderConsole.codeReaderLon();
                //读码器已经连接上
                if (resLon == 0)
                {
                    //然后开启循环读取数据
                    this.lblReaderStatus.Text = "读取中...";
                    YYKeyenceReaderConsole.threadingProcessForm = this;
                    this.button1.Text = "结束扫码";
                }
                else if (resLon == 1)
                {
                    this.lblReaderStatus.Text = "读码器尚未连接...";
                    MessageBox.Show("请检查读码器是否连接或已经断开连接!");
                }
                else
                {
                    this.lblReaderStatus.Text = "读码器尚未连接...";
                    MessageBox.Show("请检查读码器是否连接或已经断开连接!");
                }
            }
            
            //if (codeReader != null)
            //{
            //    if (btnName.Equals("开始扫码"))
            //    {
            //        //连接是否成功
            //        if (codeReader.codeReaderConnect())
            //        {
            //            //成功进入开启读码器
            //            int res = codeReader.codeReaderLon();
            //            if (res == 0)
            //            {
            //                this.button1.Text = "结束扫码";
            //                //开启读码器成功，开始接收数据
            //                codeReader.beginReceive();
            //            }
            //            else if (res == 1)
            //            {
            //                MessageBox.Show("读码器已经断开连接，请重新连接读码器!");
            //            }
            //            else
            //            {
            //                MessageBox.Show("系统繁忙,请稍后重试!");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        closeCodeReader();
            //        codeReader.codeReaderDisConnect();
            //        this.button1.Text = "开始扫码";
            //    }
            //}
            //else {
            //    MessageBox.Show("请重新新建表单!");
            //}
           
        }
        #endregion

        #region 开始录像事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text == "开始录像")
            {
                if (timestamp == null || timestamp.Length <= 0)
                {
                    timestamp = getMesuringRecord();
                }
                this.lblVideoStatus.Text = "开始录像...";
                int result = MainWindow.RecordVideo(timestamp);
                switch (result) {
                    case 0:
                        this.lblVideoStatus.Text = "录像中...";
                        this.button2.Text = "结束录像";
                        break;
                    case 1:
                        this.lblVideoStatus.Text = "录像机未连接...";
                        MessageBox.Show("录像失败,请先登录录像机!");
                        break;
                    case 2:
                        this.lblVideoStatus.Text = "录像机未启动...";
                        MessageBox.Show("录像失败,请先开启录像机预览!");
                        break;
                    case 3:
                        this.lblVideoStatus.Text = "录像失败...";
                        MessageBox.Show("录像失败,请检查配置!");
                        break;
                    case 4:
                        MessageBox.Show("录像失败!");
                        break;
                }
            }
            else if (this.button2.Text == "结束录像") {
                MainWindow.stopRecordVideo();
                this.button2.Text = "开始录像";
                this.lblVideoStatus.Text = "录像完成...";
            }
            //

            //if (mainWindow != null)
            //{
            //    if (this.button2.Text == "开始录像")
            //    {
            //        this.label1.Text = "准备中......";
            //        if (mainWindow.recordLogin() == 0)
            //        {
            //            this.label1.Text = "连接中......";
            //            if (mainWindow.recordPreview() == 0)
            //            {
            //                this.label1.Text = "连接成功......";
            //                if (timestamp == null || timestamp.Length <= 0)
            //                {
            //                    timestamp = getMesuringRecord();
            //                }
            //                if (!mainWindow.RecordVideo(timestamp))
            //                {
            //                    this.label1.Text = "录像失败......";
            //                    MessageBox.Show("录像失败!");
            //                }
            //                else
            //                {
            //                    this.label1.Text = "录像中......";
            //                    this.button2.Text = "结束录像";
            //                }
            //            }
            //            else
            //            {
            //                this.label1.Text = "录像机启动失败......";
            //                MessageBox.Show("录像机启动失败!");
            //            }
            //        }
            //        else
            //        {
            //            this.label1.Text = "连接录像机失败,请检查网络......";
            //            MessageBox.Show("连接录像机失败,请检查网络!");
            //        }
            //    }
            //    else
            //    {
            //        this.label1.Text = "关闭中......";
            //        mainWindow.stopRecordVideo();
            //        mainWindow.stopRecordPreview();
            //        mainWindow.recordLoginOut();
            //        this.button2.Text = "开始录像";
            //        this.label1.Text = "";
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("请重新新建表单!");
            //}
        }  
        #endregion

        #region 表单提交事件
        private void button3_Click(object sender, EventArgs e)
        {
            //获取时间戳，生成唯一工具使用记录编号
            if (button2.Text.Trim() == "结束录像"|| button1.Text.Trim() == "结束扫码")
            {
                MessageBox.Show("录像机或读码器尚未关闭！");
            }
            else {
                formSubmit();
            }
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
            string couping_no = HttpUtility.UrlEncode(this.textBox17.Text.TrimEnd(), Encoding.UTF8);//接箍编号
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
            //然后根据时间戳生成的目录搜索录制的视频文件获取文件名集合保存到video_no中
            string video_no =getVideoPath(timestamp); //HttpUtility.UrlEncode(this.textBox2.Text, Encoding.UTF8);//视频编号
            string inspection_result = HttpUtility.UrlEncode(getInspectionResult(this.comboBox1.SelectedItem.ToString()).ToString(), Encoding.UTF8);//检验结果
            ASCIIEncoding encoding = new ASCIIEncoding();
            string content = "";
            string json = "";
            MessageBox.Show(couping_no);
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
                json= sb.ToString();
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
                    MessageBox.Show("上传成功！");
                }
                else {
                    writeUnSubmitForm(json);
                    MessageBox.Show("上传失败，系统将数据暂时存储，请在空闲时间提交(在主页数据管理功能处提交)！");
                }
            }
            catch (Exception ex)
            {
                writeUnSubmitForm(json);
                MessageBox.Show("异常报错信息：" + ex.Message.ToString());
            }
            finally
            {
                //向可提交的视频文件中追加可提交文件夹名
                string path=Application.StartupPath + "\\fileuploadrecord.txt";
                FileStream fs = new FileStream(path,FileMode.Append,FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs,Encoding.Default);
                if (timestamp != null && timestamp.Length > 0) {
                    sw.WriteLine(timestamp);
                }
                sw.Close();
                fs.Close();
                //表单清理并关闭
                indexWindow.getThreadingProcessData();
                this.Close();
            }
        }
        #endregion

        #region 写入未提交成功的表单
        private void writeUnSubmitForm(string jsonData) 
        {
            //MessageBox.Show(jsonData);
            //判断在没有点击录制视频的时候未生成时间戳的事件
            if (timestamp == null || timestamp.Length <= 0) {
                timestamp = getMesuringRecord();
            }
            string coupingDir = Application.StartupPath + "\\draft\\" + timestamp;
            //MessageBox.Show(coupingDir);
            if (!File.Exists(coupingDir))
            {
                Directory.CreateDirectory(coupingDir);
            }
            string coupingTxt = Application.StartupPath + "\\draft\\" + timestamp +"\\"+ timestamp + ".txt";
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
            //窗口关闭时判断是否结束录像
            //string btnName = this.button2.Text;
            //if (!btnName.Equals("开始录像")) {
            //    mainWindow.RecordVideo(timestamp);
            //    mainWindow.recordPreview();
            //    mainWindow.recordLogin();
            //    this.button2.Text = "开始录像";
            //}
            //string codeBtnName = this.button1.Text;
            //if (!codeBtnName.Equals("开始扫码"))
            //{
            //   // closeCodeReader();
            //}
        }
        #endregion

        #region 遍历录制视频文件获取文件名集合
        private string getVideoPath(string pathDir)
        {
            string pathList = "";
            //时间戳存在
            if (timestamp != null && timestamp.Length > 0)
            {
                string path = Application.StartupPath + "\\draft\\" + pathDir + "\\";
                if (Directory.Exists(path))
                {
                    DirectoryInfo folder = new DirectoryInfo(path);
                    foreach (FileInfo file in folder.GetFiles("*.mp4"))
                    {
                        pathList += file.Name + ";";
                    }
                }
            }
            return pathList;
        }
        #endregion


        //#region 扫码器关闭方法
        //private void closeCodeReader()
        //{
        //    //codeReader.codeReaderOff();
        //    //if (codeReader != null)
        //    //{
        //    //    DateTime start = DateTime.Now;
        //    //    DateTime now = DateTime.Now;
        //    //    TimeSpan ts = now - start;
        //    //    while (true)
        //    //    {
        //    //        ts = now - start;
        //    //        if (ts.TotalSeconds > 1)
        //    //        {
        //    //            codeReader.codeReaderDisConnect();
        //    //            break;
        //    //        }
        //    //        else
        //    //        {
        //    //            now = DateTime.Now;
        //    //        }
        //    //    }
        //    //}
        //}
        //#endregion

        private void ThreadingProcessForm_SizeChanged(object sender, EventArgs e)
        {
            auto.controlAutoSize(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //关闭之前判断是否关闭读码器和结束录像
            if (button2.Text.Trim() == "结束录像" || button1.Text.Trim() == "结束扫码")
            {
                MessageBox.Show("录像机或读码器尚未关闭！");
            }
            else {
                this.Close();
            }

        }
    }
}
