using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace YYOPInspectionClient
{
   
    public class CodeReader
    {
        private const int READER_COUNT = 30;      // number of readers to connect  基恩士读码器个数
        private const int RECV_DATA_MAX = 10240;   //数据量buff最大值
        private const int ACCURACY = 50; //接受读码器数据的最小精度 100毫秒 0为不等待
        private ClientSocket[] clientSocketInstance;  //基恩士读码器clientSocket数组
        private YYKeyenceReaderConsole readerWinform = new YYKeyenceReaderConsole();
        private int CommandPort = 9003;
        private int DataPort = 9004; // 9004 for data
        private Thread threadReceive;  //接受各读码器server端数据的线程
        private ThreadingProcessForm threadingProcessForm;
        private delegate void UpdateTextBoxDelegate(ThreadingProcessForm threadingProcessForm, string message);
        public CodeReader(ThreadingProcessForm threadingProcessForm=null) {
            clientSocketInstance = new ClientSocket[READER_COUNT];
            this.threadingProcessForm = threadingProcessForm;
            int readerIndex = 0;
            string CommandPortStr= readerWinform.CommandPortInput.Text.Trim();
            string DataPortStr = readerWinform.DataPortInput.Text.Trim();
            threadReceive = new Thread(codeReaderReceive);
            if (CommandPortStr != null)
            {
                CommandPort = Convert.ToInt32(CommandPortStr);
            }
            if (DataPortStr != null) {
                DataPort = Convert.ToInt32(DataPortStr);
            }
            try
            {
                //读取本地目录config.txt文件，读出所有读码器IP地址
                string str = File.ReadAllText(@"config.txt");
                str = str.Replace("\n", "");
                string[] strIPArray = str.Split('\r');
                for (int i = 0; i < strIPArray.Length; i++)
                {
                    if (strIPArray[i].Length > 2)
                    {
                        clientSocketInstance[readerIndex] = new ClientSocket(strIPArray[i], CommandPort, DataPort);  // 9003 for command, 9004 for data
                        readerIndex++;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        public bool codeReaderConnect()
        {
            bool flag = true;//是否连接成功的标识
            //连接所有读码器socket
            for (int i = 0; i < READER_COUNT; i++)
            {
                try
                {
                    if (clientSocketInstance[i] == null)
                        break;
                    clientSocketInstance[i].readerCommandEndPoint.Port = CommandPort;
                    clientSocketInstance[i].readerDataEndPoint.Port = DataPort;
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        clientSocketInstance[i].commandSocket.Close();
                    }
                    clientSocketInstance[i].commandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocketInstance[i].commandSocket.Connect(clientSocketInstance[i].readerCommandEndPoint);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    flag = false;
                    clientSocketInstance[i].commandSocket = null;
                    continue;
                }
                catch (SocketException ex)
                {
                    flag = false;
                    clientSocketInstance[i].commandSocket = null;
                    continue;
                }
                try
                {
                    if (clientSocketInstance[i].dataSocket != null)
                    {
                        clientSocketInstance[i].dataSocket.Close();
                    }
                    if (clientSocketInstance[i].readerCommandEndPoint.Port == clientSocketInstance[i].readerDataEndPoint.Port)
                    {
                        clientSocketInstance[i].dataSocket = clientSocketInstance[i].commandSocket;
                    }
                    else
                    {
                        clientSocketInstance[i].dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        clientSocketInstance[i].dataSocket.Connect(clientSocketInstance[i].readerDataEndPoint);
                        
                    }
                    clientSocketInstance[i].dataSocket.ReceiveTimeout = 100;
                }

                catch (SocketException ex)
                {
                    flag = false;
                    clientSocketInstance[i].dataSocket = null;
                    continue;
                }

            }
            return flag;
        }
        public int codeReaderLon()
        {
            int flag = 0;
            try
            {
                string lon = "LON\r";   // CR is terminator
                Byte[] command = ASCIIEncoding.ASCII.GetBytes(lon);
                for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
                {
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        clientSocketInstance[i].commandSocket.Send(command);
                    }
                    else
                    {
                        flag = 1;
                        //MessageBox.Show("读码器已经断开连接，请重新连接扫码！");
                    }
                }
            }
            catch (Exception ex)
            {
                flag = 2;
            }
            return flag;
        }
        public void codeReaderOff()
        {
            string loff = "LOFF\r"; // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(loff);
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                }
                else
                {
                    //MessageBox.Show("请重新连接读码器，然后再终止读码器扫描");
                }
            }
        }
        public void codeReaderDisConnect()
        {
            //断开所有读码器连接
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Close();
                    clientSocketInstance[i].commandSocket = null;
                }
                if (clientSocketInstance[i].dataSocket != null)
                {
                    clientSocketInstance[i].dataSocket.Close();
                    clientSocketInstance[i].dataSocket = null;
                }
            }
        }

        public void beginReceive() {
            threadReceive = new Thread(new ThreadStart(codeReaderReceive));
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }

        public void codeReaderReceive()
        {
            try
            {
                while (true)
                {
                    Byte[] recvBytes = new Byte[RECV_DATA_MAX];
                    int recvSize = 0;
                    //实际接收到的字节数
                    for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
                    {
                        if (clientSocketInstance[i].dataSocket != null)
                        {
                            try
                            {
                                recvSize = clientSocketInstance[i].dataSocket.Receive(recvBytes);
                            }
                            catch (SocketException)
                            {
                                recvSize = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (recvSize == 0)
                        {
                        }
                        else
                        {
                            recvBytes[recvSize] = 0;
                            if (threadingProcessForm != null)
                            {
                                string content= Encoding.UTF8.GetString(recvBytes);
                                if (!content.Contains("Error")) {
                                    UpdateTextBox(threadingProcessForm,content);
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    if (ACCURACY > 0)
                        Thread.Sleep(ACCURACY);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
            }

        }

        private void UpdateTextBox(ThreadingProcessForm form, string message)
        {
            if (form.textBox17.InvokeRequired)
            {
                UpdateTextBoxDelegate md = new UpdateTextBoxDelegate(UpdateTextBox);
                form.textBox17.Invoke(md, new object[] { form, message });
            }
            else
            {
                form.textBox17.Text = message;
            }
        }

    }
}
