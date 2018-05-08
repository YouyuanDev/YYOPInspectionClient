﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
                        //删除文件
                        Console.WriteLine("上传完成--------------------------------------");
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
            if (flag) {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex) {
                    Console.WriteLine("删除视频失败!");
                }
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

        #region 判断文件是否被其他进程占用
        public static bool JudgeFileIsUsing(string fileFullName)
        {
            bool result = false;
            if (!System.IO.File.Exists(fileFullName))
            {
                result = false;
            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用
             //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    result = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    result = true;
                }
                catch (System.Exception ex)
                {
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }//end: 如果文件存在的处理逻辑
             //返回指示文件是否已被其它程序使用的值
            return result;
        }
        #endregion

        #region 视频转换
        public static void FormatAndUploadVideo(string ffmpegPath, string sourcePath, string filePath)
        {
            try
            {
                Console.WriteLine("----------------开始转化视频--------------");
                Process p = new Process();
                p.StartInfo.FileName = ffmpegPath;
                p.StartInfo.UseShellExecute = false;
                string srcFileName = filePath;
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = System.IO.Path.GetExtension(filePath);
                string newFileName = fileName + "_vcr" + extension;
                srcFileName = filePath;
                string destFileName = sourcePath + "\\" + newFileName;
                if (!File.Exists(destFileName))
                {
                    var file0=File.Create(destFileName);
                    file0.Close();
                }
                Console.WriteLine(srcFileName + ":" + destFileName);
                //p.StartInfo.Arguments = "-c:v libx264 -strict -2 -s 1280x720 -b 1000k";
                p.StartInfo.Arguments = "-i " + srcFileName + " -y  -vcodec h264 -b 500000 -acodec aac " + destFileName;    //执行参数
                p.StartInfo.UseShellExecute = false;  ////不使用系统外壳程序启动进程
                p.StartInfo.CreateNoWindow = true;  //不显示dos程序窗口
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中
                p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
                p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.BeginErrorReadLine();//开始异步读取
                p.WaitForExit();//阻塞等待进程结束
                p.Close();//关闭进程
                p.Dispose();//释放资源
                Console.WriteLine("----------------转化视频完成--------------");
                //直到视频格式转换完毕，释放转换进程才能执行删除转换前的文件和上传转换后的文件
                File.Delete(srcFileName);
                uploadVideoToTomcat(destFileName);
            }
            catch (Exception e) {
                Console.Write("格式化出错....................");
            }
           
         
        } 
        

        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

            Console.WriteLine(e.Data);
            Console.WriteLine("....格式化出错....");
        }

        private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            Console.WriteLine(".......格式化输出.......");
        }
        #endregion

        #region 时间戳转日期
        public static string ConvertTimeStamp(string time)
        {
            string returntime = time;
            try
            {
                long jsTimeStamp = long.Parse(time);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
                returntime = dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception e)
            {
                Console.WriteLine("日期转化失败!");
            }
            return returntime;
        } 
        #endregion

        #region 获取系统版本
        public static string GetVersion()
        {
            string str = "";
            try
            {
                str = Assembly.GetEntryAssembly().GetName().Version.ToString() + "版本";
            }
            catch (Exception e)
            {
                Console.WriteLine("系统繁忙!");
            }
            return str;
        }
        #endregion

        #region 移动文件到指定路径
        public static bool MoveFile(string sourceDir, string destDir)
        {
            bool flag = true;
            try
            {
                File.Copy(sourceDir, destDir);
            }
            catch (Exception e)
            {
                Console.WriteLine("文件被占用");
                flag=false;
            }
            return flag;
        }
        #endregion

        #region 删除指定文件
        public static bool DeleteFile(string filePath)
        {
            bool flag = true;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        } 
        #endregion
    }
}
