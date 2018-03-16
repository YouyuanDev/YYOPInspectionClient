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
        private static DirectoryInfo dirInfo = null;
        private static FileInfo fileInfo = null;
        public static bool UploadFile(DirectoryInfo dirinfo, FileInfo fileinfo) {
           
            bool flag = false;
            try
            {
                dirInfo = dirinfo;
                fileInfo = fileinfo;
                //MessageBox.Show(fileInfo.FullName);
                Uri uri = new Uri("ftp://192.168.0.200/" + fileinfo.Name);
                //定义FtpWebRequest,并设置相关属性
                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(uri);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                string ftpUser = "ftpadmin";
                string ftpPassWord = "123456";
                uploadRequest.Credentials = new NetworkCredential(ftpUser, ftpPassWord);
                //开始以异步方式打开请求的内容流以便写入

                //FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                
                using (Stream rs = uploadRequest.GetRequestStream())
                using (FileStream fs = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024 * 4];
                    int count = fs.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        rs.Write(buffer, 0, count);
                        count = fs.Read(buffer, 0, buffer.Length);
                    }
                    fs.Close();
                    flag = true;
                }
                //byte[] buffer = new byte[1024];
                //    int bytesRead;
                //    while (true)
                //    {
                //        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                //        if (bytesRead == 0)
                //            break;
                //        //本地的文件流数据写到请求流
                //        requestStream.Write(buffer, 0, bytesRead);
                //    }
                //    requestStream.Close();
                //    fileStream.Close();
                
                flag = true;
                if (File.Exists(fileInfo.FullName))
                {
                    File.Delete(fileInfo.FullName);
                }
                string fatherDir = dirinfo.FullName;
               
                //然后判断父目录中还有文件没，如果有父目录就不删除
                if ((dirInfo.GetFiles().Length + dirInfo.GetDirectories().Length) == 0)
                {
                    Directory.Delete(fatherDir);
                }
            }
            catch (Exception e) {
                flag = false;
            }
            return flag;
        }


        ///// <summary>
        ///// 上传文件
        ///// </summary>
        ///// <param name="fileinfo">需要上传的文件</param>
        ///// <param name="targetDir">目标路径</param>
        ///// <param name="hostname">ftp地址</param>
        ///// <param name="username">ftp用户名</param>
        ///// <param name="password">ftp密码</param>
        //public static bool UploadFile(DirectoryInfo dirinfo,FileInfo fileinfo)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        dirInfo = dirinfo;
        //        fileInfo = fileinfo;
        //        Uri uri = new Uri("ftp://192.168.0.200/" + fileinfo.Name);
        //        //定义FtpWebRequest,并设置相关属性
        //        FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(uri);
        //        uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        string ftpUser = "ftpadmin";
        //        string ftpPassWord = "123456";
        //        uploadRequest.Credentials = new NetworkCredential(ftpUser, ftpPassWord);
        //        //开始以异步方式打开请求的内容流以便写入
        //        uploadRequest.BeginGetRequestStream(new AsyncCallback(EndGetStreamCallback), uploadRequest);
        //        flag = true;
        //    }
        //    catch (Exception ex) {
        //    }
        //    return flag;
        //}


        //private static void EndGetStreamCallback(IAsyncResult ar)
        //{
        //    //用户定义对象，其中包含该操作的相关信息,在这里得到FtpWebRequest
        //    FtpWebRequest uploadRequest = (FtpWebRequest)ar.AsyncState;
        //    //结束由BeginGetRequestStream启动的挂起的异步操作
        //    //必须调用EndGetRequestStream方法来完成异步操作
        //    //通常EndGetRequestStream由callback所引用的方法调用
        //    Stream requestStream = uploadRequest.EndGetRequestStream(ar);
        //    //Directory.GetAccessControl("C:\\1521108750_vcr.mp4",System.Security.AccessControl.AccessControlSections.All);
        //    FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open,FileAccess.Read);

        //    byte[] buffer = new byte[1024];
        //    int bytesRead;
        //    while (true)
        //    {
        //        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
        //        if (bytesRead == 0)
        //            break;
        //        //本地的文件流数据写到请求流
        //        requestStream.Write(buffer, 0, bytesRead);
        //    }
        //    requestStream.Close();
        //    fileStream.Close();
        //    //开始以异步方式向FTP服务器发送请求并从FTP服务器接收响应
        //    uploadRequest.BeginGetResponse(new AsyncCallback(EndGetResponseCallback), uploadRequest);
        //}
        //private static void EndGetResponseCallback(IAsyncResult ar)
        //{
        //    FtpWebRequest uploadRequest = (FtpWebRequest)ar.AsyncState;
        //    //结束由BeginGetResponse启动的挂起的异步操作
        //    FtpWebResponse uploadResponse = (FtpWebResponse)uploadRequest.EndGetResponse(ar);
        //    if (uploadResponse.StatusCode.ToString()== "ClosingData") {
        //        if (File.Exists(fileInfo.FullName)) {
        //            File.Delete(fileInfo.FullName);
        //        }
        //        string fatherDir = Directory.GetParent(fileInfo.FullName).FullName;
        //        //然后判断父目录中还有文件没，如果有父目录就不删除
        //        if ((dirInfo.GetFiles().Length + dirInfo.GetDirectories().Length) == 0)
        //        {
        //            Directory.Delete(fatherDir);
        //        }
        //    }
        //    //MessageBox.Show(uploadResponse.StatusDescription +":"+ uploadResponse.StatusCode);
        //    //MessageBox.Show("Upload complete");
        //}
    }
}
