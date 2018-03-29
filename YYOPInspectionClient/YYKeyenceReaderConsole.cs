using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    internal partial class YYKeyenceReaderConsole : Form
    {
        private const int READER_COUNT =30;      // number of readers to connect  基恩士读码器个数
        private const int RECV_DATA_MAX = 10240;   //数据量buff最大值
        private const int ACCURACY = 50; //接受读码器数据的最小精度 100毫秒 0为不等待
        public static ClientSocket[] clientSocketInstance;  //基恩士读码器clientSocket数组
        private Thread threadReceive;  //接受各读码器server端数据的线程
        //delegate void SetTextCallback(string text);   //用于子线程修改textbox
        public static ThreadingProcessForm threadingProcessForm=null;
        private delegate void SetTextCallback(string message);
        private delegate void UpdateTextBoxDelegate(ThreadingProcessForm threadingProcessForm, string message);
        private static YYKeyenceReaderConsole myselfForm=null;
        public YYKeyenceReaderConsole()
        {
            InitializeComponent();
            if(myselfForm==null)
               myselfForm = this;
            //
            // Allocate Instances of ClientSocket, and set IP address, command port number and
            // data port number.
            //
            clientSocketInstance = new ClientSocket[READER_COUNT];
            //this.threadingProcessForm = threadingProcessForm;
            int readerIndex = 0;
            int CommandPort = 9003; // 9003 for command
            int DataPort = 9004; // 9004 for data
            CommandPortInput.Text = Convert.ToString(CommandPort);
            DataPortInput.Text = Convert.ToString(DataPort);
            //
            // First reader to connect.
            //

            try
            {
                //读取本地目录config.txt文件，读出所有读码器IP地址
                string str = File.ReadAllText(@"config.txt");
                str = str.Replace("\n", "");
                string[] strIPArray = str.Split('\r');
                //this.SetText(strIPArray[1]);
                //两种方式初始化构造
                //byte[] ip1 = { 192, 168, 0, 101 };
                //string ip1 = "192.168.0.101";
                for (int i = 0; i < strIPArray.Length; i++)
                {
                    if (strIPArray[i].Length > 2)
                    {
                        //MessageBox.Show(strIPArray[i].ToString());
                        clientSocketInstance[readerIndex] = new ClientSocket(strIPArray[i], CommandPort, DataPort);  // 9003 for command, 9004 for data

                        setLogText(clientSocketInstance[readerIndex].readerCommandEndPoint.ToString() + " Failed to connect.");
                        readerIndex++;
                    }
                }



            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }


            //
            // Second reader to connect.
            //
            //byte[] ip2 = { 192, 168, 0, 102 };
            // clientSocketInstance[readerIndex++] = new ClientSocket(ip2, CommandPort, DataPort);  // 9003 for command, 9004 for data

            //设置读码器数据读线程
            threadReceive = new Thread(new ThreadStart(codeReaderReceive));
            //设置为后台线程
            threadReceive.IsBackground = true;
            threadReceive.Start();

        }

        private void connect_Click(object sender, EventArgs e)
        {
            this.connect.Text = "Connecting...";
            this.connect.Update();
            //连接所有读码器socket
            for (int i = 0; i < READER_COUNT; i++)
            {
                //
                // Connect to the command port.
                //
                try
                {
                    if (clientSocketInstance[i] == null)
                        break;
                    clientSocketInstance[i].readerCommandEndPoint.Port = Convert.ToInt32(CommandPortInput.Text);
                    clientSocketInstance[i].readerDataEndPoint.Port = Convert.ToInt32(DataPortInput.Text);
                    //
                    // Close the socket if opened.
                    //
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        clientSocketInstance[i].commandSocket.Close();
                    }

                    //
                    // Create a new socket.
                    //
                    clientSocketInstance[i].commandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " Connecting..");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " Connecting..\r\n";
                    //textBox_LogConsole.Update();

                    clientSocketInstance[i].commandSocket.Connect(clientSocketInstance[i].readerCommandEndPoint);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " Connected.");

                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " Connected.\r\n";
                    //textBox_LogConsole.Update();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    //
                    // Catch exceptions and show the message.
                    //
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " Failed to connect.");
                    //textBox_LogConsole.Text = clientSocketInstance[i].readerCommandEndPoint.ToString() + " Failed to connect.\r\n";
                    //textBox_LogConsole.Update();
                    MessageBox.Show(ex.Message);
                    clientSocketInstance[i].commandSocket = null;
                    continue;
                }
                catch (SocketException ex)
                {
                    //
                    // Catch exceptions and show the message.
                    //
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " failed to connect.");
                    //textBox_LogConsole.Text = clientSocketInstance[i].readerCommandEndPoint.ToString() + " Failed to connect.\r\n";
                    //textBox_LogConsole.Update();
                    MessageBox.Show(ex.Message);
                    clientSocketInstance[i].commandSocket = null;
                    continue;
                }

                //
                // Connect to the data port.
                //
                try
                {
                    //
                    // Close the socket if opend.
                    //
                    if (clientSocketInstance[i].dataSocket != null)
                    {
                        clientSocketInstance[i].dataSocket.Close();
                    }

                    //
                    // If the same port number is used for command port and data port, unify the sockets and skip a new connection. 
                    //
                    if (clientSocketInstance[i].readerCommandEndPoint.Port == clientSocketInstance[i].readerDataEndPoint.Port)
                    {
                        clientSocketInstance[i].dataSocket = clientSocketInstance[i].commandSocket;
                    }
                    else
                    {
                        //
                        // Create a new socket.
                        //
                        clientSocketInstance[i].dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " Connecting..");
                        //textBox_LogConsole.Text = clientSocketInstance[i].readerDataEndPoint.ToString() + " Connecting..\r\n";
                        //textBox_LogConsole.Update();

                        clientSocketInstance[i].dataSocket.Connect(clientSocketInstance[i].readerDataEndPoint);
                        setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " connected.");
                        if (!listBox_Reader.Items.Contains(clientSocketInstance[i].readerCommandEndPoint.Address.ToString()))
                        {
                            this.listBox_Reader.Items.Add(clientSocketInstance[i].readerCommandEndPoint.Address.ToString());
                            this.listBox_Reader.Update();
                        }
                        //textBox_LogConsole.Text = clientSocketInstance[i].readerDataEndPoint.ToString() + " Connected.\r\n";
                        //textBox_LogConsole.Update();
                    }
                    //
                    // Set 100 milliseconds to receive timeout.
                    //
                    clientSocketInstance[i].dataSocket.ReceiveTimeout = 100;



                }

                catch (SocketException ex)
                {
                    //
                    // Catch exceptions and show the message.
                    //
                    setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " failed to connect.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerDataEndPoint.ToString() + " Failed to connect.\r\n";
                    //textBox_LogConsole.Update();
                    MessageBox.Show(ex.Message);
                    clientSocketInstance[i].dataSocket = null;
                    continue;
                }

            }


            this.connect.Text = "Connect All";
            this.connect.Update();

        }

        private void disconnect_Click(object sender, EventArgs e)
        {
            //断开所有读码器连接
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                //
                // Close the command socket.
                //
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Close();
                    clientSocketInstance[i].commandSocket = null;
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " disconnected.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " Disconnected.\r\n";
                    //textBox_LogConsole.Update();
                }

                //
                // Close the data socket.
                //
                if (clientSocketInstance[i].dataSocket != null)
                {
                    clientSocketInstance[i].dataSocket.Close();
                    clientSocketInstance[i].dataSocket = null;
                    setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " disconnected.");
                    this.listBox_Reader.Items.Remove(clientSocketInstance[i].readerCommandEndPoint.Address.ToString());
                    this.listBox_Reader.Update();
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerDataEndPoint.ToString() + " Disconnected.\r\n";
                    //textBox_LogConsole.Update();
                }
            }
        }

        private void lon_Click(object sender, EventArgs e)
        {
            //
            // Send "LON" command.
            // 
            string lon = "LON\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(lon);

            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " LON sent.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " LON Sent.\r\n";
                    // textBox_LogConsole.Update();
                    // MessageBox.Show(clientSocketInstance[i].readerCommandEndPoint.ToString() + " LON Sent.");
                }
                else
                {

                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " is disconnected.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " is disconnected.\r\n";
                    //textBox_LogConsole.Update();

                }
            }
        }

        private void loff_Click(object sender, EventArgs e)
        {
            //停止读码器触发读取
            //
            // Send "LOFF" command.
            //  

            string loff = "LOFF\r"; // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(loff);

            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " LOFF sent.");


                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "LOFF sent.\r\n";
                    //textBox_LogConsole.Update();
                }
                else
                {

                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " is disconnected");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.\r\n";
                    //textBox_LogConsole.Update();
                    //MessageBox.Show(clientSocketInstance[i].readerCommandEndPoint.ToString() + " is disconnected.");
                }
            }

        }


        private   void setLogText(string str)
        {
            //textBox_LogConsole.Text += str+"\r\n";
            textBox_LogConsole.AppendText(str);
            textBox_LogConsole.AppendText("\r\n");
            textBox_LogConsole.Update();
           // ThreadingProcessForm.getMyForm().textBox17.Text=str;
        }

        private static void setLogTextTwo(YYKeyenceReaderConsole readerForm, string str) {

            //if (readerForm.textBox_LogConsole.InvokeRequired)
            //{
            //    UpdateLogDelegate md = new UpdateLogDelegate(setLogTextTwo);
            //    readerForm.Invoke(new Action(() =>
            //    {
            //        readerForm.textBox_LogConsole.AppendText(str);
            //        readerForm.textBox_LogConsole.AppendText("\r\n");
            //        readerForm.textBox_LogConsole.Update();
            //    }));
            //    //form.textBox17.Invoke(md, new object[] { form, message });
            //    //UpdateLogDelegate delegate= new UpdateLogDelegate(Up);
            //}
            //else
            //{
            //    readerForm.textBox_LogConsole.AppendText(str);
            //    readerForm.textBox_LogConsole.AppendText("\r\n");
            //    readerForm.textBox_LogConsole.Update();
            //}


            //textBox_LogConsole.AppendText(str);
            //textBox_LogConsole.AppendText("\r\n");
            //textBox_LogConsole.Update();
        }
        public void Receive()
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
                                //
                                // Catch the exception, if cannot receive any data.
                                //
                                recvSize = 0;
                            }
                        }
                        else
                        {
                            // MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " is disconnected.\r\n");
                            continue;
                        }

                        if (recvSize == 0)
                        {
                            //  MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " has no data.\r\n");
                        }
                        else
                        {
                            //
                            // Show the receive data after converting the receive data to Shift-JIS.
                            // Terminating null to handle as string.
                            //
                            recvBytes[recvSize] = 0;
                            //this.SetText("[" + clientSocketInstance[i].readerDataEndPoint.ToString() + "] " + Encoding.GetEncoding("Shift_JIS").GetString(recvBytes) + "\r\n");

                            //MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + "\r\n" + Encoding.GetEncoding("Shift_JIS").GetString(recvBytes)+"\r\n");
                            //textbox_DataConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "LOFF sent.\r\n";
                            //textbox_DataConsole.Update();
                        }
                    }
                    if (ACCURACY > 0)
                        Thread.Sleep(ACCURACY);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
            }


        }

        //private void SetText(string text)
        //{
        //    // InvokeRequired required compares the thread ID of the
        //    // calling thread to the thread ID of the creating thread.
        //    // If these threads are different, it returns true.


        //    if (this.textbox_DataConsole.InvokeRequired)
        //    {
        //        SetTextCallback d = new SetTextCallback(SetText);
        //        this.Invoke(d, new object[] { text });
        //    }
        //    else
        //    {
        //        this.textbox_DataConsole.AppendText(text);
        //        this.textbox_DataConsole.AppendText("\r\n");
        //        this.textbox_DataConsole.Refresh();
        //    }
        //}

        private void receive_Click(object sender, EventArgs e)
        {
            //Byte[] recvBytes = new Byte[RECV_DATA_MAX];
            //int recvSize = 0;

            //for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            //{
            //    if (clientSocketInstance[i].dataSocket != null)
            //    {
            //        try
            //        {
            //            recvSize = clientSocketInstance[i].dataSocket.Receive(recvBytes);
            //        }
            //        catch (SocketException)
            //        {
            //            //
            //            // Catch the exception, if cannot receive any data.
            //            //
            //            recvSize = 0;
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " is disconnected.\r\n");
            //        continue;
            //    }

            //    if (recvSize == 0)
            //    {
            //        MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " has no data.\r\n");
            //    }
            //    else
            //    {
            //        //
            //        // Show the receive data after converting the receive data to Shift-JIS.
            //        // Terminating null to handle as string.
            //        //
            //        recvBytes[recvSize] = 0;
            //        MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + "\r\n" + Encoding.GetEncoding("Shift_JIS").GetString(recvBytes));
            //    }
            //}
        }

        private void button_FTune_Click(object sender, EventArgs e)
        {
            //调节读码器焦距
            //
            // Send "FTUNE" command.
            // 
            string tune = "FTUNE\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(tune);

            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " FTUNE sent.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " FTUNE Sent.\r\n";
                    //textBox_LogConsole.Update();
                    // MessageBox.Show(clientSocketInstance[i].readerCommandEndPoint.ToString() + " FTUNE Sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.\r\n";
                    //textBox_LogConsole.Update();

                }
            }

        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            //清屏
            textBox_LogConsole.Text = "";
            textbox_DataConsole.Text = "";

        }

        private void button_Tune_Click(object sender, EventArgs e)
        {
            //开始调整
            //
            // Send "TUNE" command.
            // 
            string tune = "TUNE,01\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(tune);

            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " TUNE sent.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " TUNE Sent.\r\n";
                    //textBox_LogConsole.Update();
                    // MessageBox.Show(clientSocketInstance[i].readerCommandEndPoint.ToString() + " TUNE Sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.\r\n";
                    //textBox_LogConsole.Update();

                }
            }
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            //重置读码器
            //
            // Send "RESET" command.
            // 
            string reset = "RESET\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(reset);

            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " RESET sent.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + " RESET Sent.\r\n";
                    //textBox_LogConsole.Update();
                    // MessageBox.Show(clientSocketInstance[i].readerCommandEndPoint.ToString() + " RESET Sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                    //textBox_LogConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.\r\n";
                    //textBox_LogConsole.Update();

                }
            }
        }

        //表单页面调用的方法
        public static  int codeReaderLon()
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



        //更新表单中的读码器值
        //private void UpdateTextBox(ThreadingProcessForm form, string message)
        //{
        //    if (form.textBox17.InvokeRequired)
        //    {
        //        UpdateTextBoxDelegate md = new UpdateTextBoxDelegate(UpdateTextBox);
        //        form.textBox17.Invoke(md, new object[] { form, message });
        //    }
        //    else
        //    {
        //        form.textBox17.Text = message;
        //    }
        //}
       
        public static void codeReaderReceive()
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
                            //MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " is disconnected.\r\n");
                            continue;
                        }

                        if (recvSize == 0)
                        {
                            //MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " has no data.\r\n");
                        }
                        else
                        {
                            recvBytes[recvSize] = 0;
                            if (threadingProcessForm != null)
                            {
                                
                                UpdateTextBox(threadingProcessForm, Encoding.UTF8.GetString(recvBytes));
                                // UpdateTextBox(threadingProcessForm, Encoding.GetEncoding("UTF-8").GetString(recvBytes));
                                //this.Invoke((EventHandler)delegate { threadingProcessForm.textBox17.Text = Encoding.GetEncoding("Shift_JIS").GetString(recvBytes); });
                                //MessageBox.Show(Encoding.GetEncoding("Shift_JIS").GetString(recvBytes));
                                //ThreadingProcessForm.getMyForm().textBox17.Text = Encoding.UTF8.GetString(recvBytes);
                                SetText(Encoding.UTF8.GetString(recvBytes));
                            }
                        }
                    }
                    if (ACCURACY > 0)
                        Thread.Sleep(ACCURACY);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
            }

        }

        private static void UpdateTextBox(ThreadingProcessForm form, string message)
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
        private static void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (myselfForm.textbox_DataConsole.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                myselfForm.textbox_DataConsole.Invoke(d, new object[] { text });
            }
            else
            {
                myselfForm.textbox_DataConsole.AppendText(text);
                myselfForm.textbox_DataConsole.AppendText("\r\n");
                myselfForm.textbox_DataConsole.Refresh();
            }
        }

    }
}
