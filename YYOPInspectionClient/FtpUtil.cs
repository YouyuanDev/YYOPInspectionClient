using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public class FtpUtil
    {

        
         private static void EndGetStreamCallback(IAsyncResult ar)
        {
            //用户定义对象，其中包含该操作的相关信息,在这里得到FtpWebRequest
            FtpWebRequest uploadRequest = (FtpWebRequest)ar.AsyncState;

            //结束由BeginGetRequestStream启动的挂起的异步操作
            //必须调用EndGetRequestStream方法来完成异步操作
            //通常EndGetRequestStream由callback所引用的方法调用
            Stream requestStream = uploadRequest.EndGetRequestStream(ar);
            //Directory.GetAccessControl("C:\\1521108750_vcr.mp4",System.Security.AccessControl.AccessControlSections.All);
            FileStream fileStream = File.Open("C:\\eee.mp4", FileMode.Open,FileAccess.Read);

            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                //本地的文件流数据写到请求流
                requestStream.Write(buffer, 0, bytesRead);
            }

            requestStream.Close();
            fileStream.Close();

            //开始以异步方式向FTP服务器发送请求并从FTP服务器接收响应
            uploadRequest.BeginGetResponse(new AsyncCallback(EndGetResponseCallback), uploadRequest);
        }



        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpWebRequest uploadRequest = (FtpWebRequest)ar.AsyncState;

            //结束由BeginGetResponse启动的挂起的异步操作
            FtpWebResponse uploadResponse = (FtpWebResponse)uploadRequest.EndGetResponse(ar);

            MessageBox.Show(uploadResponse.StatusDescription);
            MessageBox.Show("Upload complete");
        }

        private static readonly string targetDir = "video";//服务器ip  
        private static readonly string username = "ftpadmin";//用户名  
        private static readonly string password = "123456";//密码
        private static readonly string hostname = "ftp://192.168.0.200/";

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileinfo">需要上传的文件</param>
        /// <param name="targetDir">目标路径</param>
        /// <param name="hostname">ftp地址</param>
        /// <param name="username">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        public static bool UploadFile(FileInfo fileinfo)
        {

            Uri uri = new Uri("ftp://192.168.0.200/"+fileinfo.Name);

            //定义FtpWebRequest,并设置相关属性
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(uri);
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;

            string ftpUser = "ftpadmin";
            string ftpPassWord = "123456";
            uploadRequest.Credentials = new NetworkCredential(ftpUser, ftpPassWord);

            //开始以异步方式打开请求的内容流以便写入
            uploadRequest.BeginGetRequestStream(new AsyncCallback(EndGetStreamCallback), uploadRequest);
            return true;
        }
        

        #region 上传文件


        //public static bool UpLoadFile(string localFile)
        //{
        //    bool flag =false;
        //    FileStream fs = null;
        //    Stream strm = null;
        //    FtpWebRequest reqFTP = null;
        //    if (File.Exists(localFile))
        //    {
        //        try
        //        {
        //            MessageBox.Show(localFile);
        //            FileInfo fileInf = new FileInfo(localFile);
        //            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));// 根据uri创建FtpWebRequest对象 
        //            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);// ftp用户名和密码
        //            reqFTP.KeepAlive = false;// 默认为true，连接不会被关闭 // 在一个命令之后被执行
        //            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;// 指定执行什么命令
        //            reqFTP.UseBinary = true;// 指定数据传输类型
        //            reqFTP.ContentLength = fileInf.Length;// 上传文件时通知服务器文件的大小
        //            int buffLength = 2048;// 缓冲大小设置为2kb
        //            byte[] buff = new byte[buffLength];
        //            int contentLen;
        //            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
        //            fs = fileInf.OpenRead();
        //            strm = reqFTP.GetRequestStream();// 把上传的文件写入流
        //            contentLen = fs.Read(buff, 0, buffLength);// 每次读文件流的2kb
        //            while (contentLen != 0)// 流内容没有结束
        //            {
        //                // 把内容从file stream 写入 upload stream
        //                strm.Write(buff, 0, contentLen);
        //                contentLen = fs.Read(buff, 0, buffLength);
        //            }
        //            // 关闭两个流
        //            flag = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //            flag = false;
        //        }
        //        finally{
        //            strm.Close();
        //            fs.Close();
        //        }
        //    }
        //    return flag;
        //}

        #endregion

    }
}
