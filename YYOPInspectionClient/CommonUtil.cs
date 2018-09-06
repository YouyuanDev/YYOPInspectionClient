using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Management;
using System.Drawing;
using System.Threading;

namespace YYOPInspectionClient
{

    public class CommonUtil
    {
        //定义临时服务器ip地址
        public static string ip = "";
        //判断是登录页面还是螺纹检验页面的标识,用于更新与服务器的连接情况
        public static bool flag = false;
        //定义一个委托用于更新LoginWinform中客户端ping服务器所需时间
        private delegate void SetLblTextCallback(string message);
        //定义一个委托用于更新ThreadingForm中客户端ping服务器所需时间
        private delegate void SetLblTextCallbackOfThreading(string message);

        #region 获取服务地址(例如:http://100.100.0.1:8080/)
        public static string getServerIpAndPort()
        {
            string serverPath=Application.StartupPath + "\\server.txt";
            StreamReader sr = new StreamReader(serverPath);
            string ipPort ="";
            if (File.Exists(serverPath))
            {
                
                    ipPort = "http://"+sr.ReadToEnd().Trim()+"/";
            }
            if (string.IsNullOrWhiteSpace(ipPort)) {
                ServerSetting server = new ServerSetting();
                ipPort = "http://" + server.txtIp.Text.Trim() + ":" + server.txtPort.Text.Trim() + "/";
            }
            return ipPort;
        }
        #endregion

        #region 上传视频文件到Tomcat服务器
        public static bool uploadVideoToTomcat(string filePath)
        {
            bool flag = false;
                    //string filePath = Application.StartupPath + "\\" + "InitVideohahaha.mp4";

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
                //Console.WriteLine("----------------开始转化视频--------------");
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
                //Console.WriteLine(srcFileName + ":" + destFileName);
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
                //Console.WriteLine("----------------转化视频完成--------------");
                //直到视频格式转换完毕，释放转换进程才能执行删除转换前的文件和上传转换后的文件
                File.Delete(srcFileName);
                if (uploadVideoToTomcat(destFileName)) {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("删除视频失败!");
                        }
                }
            }
            catch (Exception e) {
                Console.Write("格式化出错,错误信息:"+e.Message);
            }
           
         
        } 
        

        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            Console.WriteLine("....格式化中....");
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

        #region  AES加密解密

        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="encryptStr">明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns></returns>  
        public static string Encrypt()
        {
            //获取mac地址
            string str = GetFirstMacAddress();
            //获取下一天日期
            string key = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //补全key为16个字符的字符串
            for (int i = key.Length; i < 16; i++) {
                key += "0";
            }
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            //开始AES加密
            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),//密钥
                Mode = System.Security.Cryptography.CipherMode.ECB,//加密模式
                Padding = System.Security.Cryptography.PaddingMode.PKCS7//填白模式，对于AES, C# 框架中的 PKCS　＃７等同与Java框架中 PKCS #5
            };
            //字节编码， 将有特等含义的字符串转化为字节流
            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            //加密
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //将加密后的字节流转化为字符串，以便网络传输与储存。
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        // <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="decryptStr">密文</param>  
        /// <param name="key">密钥</param>  
        /// <returns></returns>  
        public static string Decrypt(string decryptStr, string key)

        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region 获取Mac地址
        public static string GetFirstMacAddress()
        {
            string macAddresses = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }
        #endregion

        #region 返回指示文件是否已被其它程序使用的布尔值
        public static Boolean FileIsUsed(String fileFullName)
        {
            Boolean result = false;
            //判断文件是否存在，如果不存在，直接返回 false
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
        #endregion//end method FileIsUsed

        #region 小窗口显示实时录制视频内容
        public static void RealTimePreview()
        {
            if (MainWindow.getForm() != null)
            {
                MainWindow.getForm().groupBox1.Hide(); MainWindow.getForm().groupBox2.Hide();
                MainWindow.getForm().groupBox3.Hide(); MainWindow.getForm().groupBox4.Hide();
                int width = MainWindow.getForm().Width;
                int height = MainWindow.getForm().Height;
                MainWindow.getForm().Width = 150;
                MainWindow.getForm().Height = 150;
                MainWindow.getForm().RealPlayWnd.Width = width;
                MainWindow.getForm().RealPlayWnd.Height = height;
                int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
                int x = iActulaWidth - 150;
                int y = 55;
                MainWindow.getForm().RealPlayWnd.Left = 0;
                MainWindow.getForm().RealPlayWnd.Top = 0;
                MainWindow.getForm().RealPlayWnd.Dock = DockStyle.Fill;
                MainWindow.getForm().Show();
                MainWindow.getForm().TopMost = true;
                MainWindow.getForm().Location = new Point(x, y);
                //mainWindow.FormBorderStyle = FormBorderStyle.FixedDialog;
                //MainWindow.getForm().MaximumSize = new Size(150, 150);
                //MainWindow.getForm().MinimumSize=new Size(150, 150);
            }
        }
        #endregion

        #region 重置录像机设置
        public static void RestoreSetting(bool isClose)
        {

            MainWindow.getForm().FormBorderStyle = FormBorderStyle.Sizable;
            MainWindow.getForm().MaximumSize = new Size(2000, 2000);
            MainWindow.getForm().Left = MainWindow.mainWindowX;
            MainWindow.getForm().Top = MainWindow.mainWindowY;
            MainWindow.getForm().Width = MainWindow.mainWindowWidth;
            MainWindow.getForm().Height = MainWindow.mainWindowHeight;
            MainWindow.getForm().RealPlayWnd.Left = MainWindow.realTimeX;
            MainWindow.getForm().RealPlayWnd.Top = MainWindow.realTimeY;
            MainWindow.getForm().RealPlayWnd.Width = MainWindow.realTimeWidth;
            MainWindow.getForm().RealPlayWnd.Height = MainWindow.realTimeHeigh;
            MainWindow.getForm().RealPlayWnd.Dock = DockStyle.None;
            MainWindow.getForm().groupBox1.Show();
            MainWindow.getForm().groupBox2.Show();
            MainWindow.getForm().groupBox3.Show();
            MainWindow.getForm().groupBox4.Show();
            MainWindow.getForm().TopMost = false;
            if (isClose)
            {
                MainWindow.getForm().Hide();
            }
            else {
                MainWindow.isRecordClick = true;
            }
        }
        #endregion

        #region 更新与服务器连接状态
        public static void UpdatePing(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //如果服务器ip为空
                if (string.IsNullOrEmpty(ip))
                {
                    //str数据格式为http://ip:port
                    string str = getServerIpAndPort();
                    //移除http://字符串
                    str = str.Replace("http://", "");
                    //移除端口
                    string[] strArr = str.Split(':');
                    ip = strArr[0];
                }
                //再次判断ip是否为空,因为上面解析ip可能会出错
                if (!string.IsNullOrEmpty(ip))
                {
                    //以下为ping服务器代码段
                    Ping pingSender = new Ping();
                    PingOptions options = new PingOptions();
                    options.DontFragment = true;
                    string data = "";
                    byte[] buf = Encoding.ASCII.GetBytes(data);
                    //调用同步send方法发送数据，结果存入reply对象;
                    PingReply reply = pingSender.Send(ip, 120, buf, options);
                    if (reply.Status == IPStatus.Success)
                    {
                        SetTextOfLogin("Ping:" + reply.RoundtripTime + "ms");
                        if (flag)
                            SetTextOfThreading("Ping:" + reply.RoundtripTime + "ms");
                    }
                    else
                    {
                        SetTextOfLogin("服务器未响应...");
                        if (flag)
                            SetTextOfThreading("服务器未响应...");
                    }
                }
            }
            catch (Exception ex)
            {

            }

        } 
        #endregion

        #region 更新LoginWinform中客户端ping服务器所需时间
        private static void SetTextOfLogin(string text)
        {
            try
            {
                if (LoginWinform.getForm().pingLbl.InvokeRequired)
                {
                    SetLblTextCallback d = new SetLblTextCallback(SetTextOfLogin);
                    LoginWinform.getForm().pingLbl.Invoke(d, new object[] { text });
                }
                else
                {
                    LoginWinform.getForm().pingLbl.Text = text;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 更新ThreadingForm中客户端ping服务器所需时间
        private static void SetTextOfThreading(string text)
        {
            try
            {
                if (ThreadingForm.getMyForm().pingLbl.InvokeRequired)
                {
                    SetLblTextCallbackOfThreading d = new SetLblTextCallbackOfThreading(SetTextOfThreading);
                    ThreadingForm.getMyForm().pingLbl.Invoke(d, new object[] { text });
                }
                else
                {
                    ThreadingForm.getMyForm().pingLbl.Text = text;
                }
            }
            catch (Exception ex)
            {

            }
        } 
        #endregion
    }
}
