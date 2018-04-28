using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YYOPInspectionClient
{
    public class CommonUtil
    {

        #region 获取服务地址(例如:http://100.100.0.1:8080/)
        public static string getServerIpAndPort()
        {
            ServerSetting server = new ServerSetting();
            return "http://" + server.txtIp.Text.Trim() + ":" + server.txtPort.Text.Trim() + "/";
        }
        #endregion

        #region 上传视频文件到Tomcat服务器
        public static bool uploadVideoToTomcat(string filePath)
        {
            //string filePath = Application.StartupPath + "\\" + "InitVideohahaha.mp4";
            bool flag = false;
            string serverAddress = getServerIpAndPort();
            string url = serverAddress + "ThreadingOperation/uploadVideoFile.action";
            // 时间戳，用做boundary
            string timeStamp = DateTime.Now.Ticks.ToString("x");
            //根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpReq.Method = "POST";
            httpReq.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
            httpReq.Timeout = 300000;  //设置获得响应的超时时间（300秒）
            httpReq.ContentType = "multipart/form-data; boundary=" + timeStamp;
            //文件
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            //头信息
            string boundary = "--" + timeStamp;
            string dataFormat = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";
            string header = string.Format(dataFormat, "file", Path.GetFileName(filePath));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(header);
            //结束边界
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + timeStamp + "--\r\n");
            long length = fileStream.Length + postHeaderBytes.Length + boundaryBytes.Length;
            httpReq.ContentLength = length;//请求内容长度
            try
            {
                //每次上传4k
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];
                //已上传的字节数
                long offset = 0;
                int size = binaryReader.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();
                //发送请求头部消息
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    size = binaryReader.Read(buffer, 0, bufferLength);
                }
                //添加尾部边界
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();
                //获取服务器端的响应
                using (HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    string returnValue = readStream.ReadToEnd();
                    if (returnValue.Trim().Equals("success"))
                    {
                        flag = true;
                    }
                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件传输异常： " + ex.Message);
            }
            finally
            {
                fileStream.Close();
                binaryReader.Close();
            }
            return flag;
        }
        #endregion

        #region 写入未提交成功的表单
        public static void writeUnSubmitForm(string couping_no, string jsonData,string savePath)
        {
            //未提交成功的json数据文件名:接箍编号_时间戳
            string timestamp = getMesuringRecord();
            string unsubmitFileName = couping_no + "_" + timestamp + ".txt";
             
            if (!File.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string coupingTxt = savePath+"\\" + unsubmitFileName;
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

        #region  获取时间戳
        public static string getMesuringRecord()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        #endregion

        public static string ConvertTimeStamp(string time) {
            string returntime = time;
            try
            {
                long jsTimeStamp = long.Parse(time);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
                returntime = dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception e) {
                Console.WriteLine("日期转化失败!");
            }
            return returntime;
        }


    }
}
