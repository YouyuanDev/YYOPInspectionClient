using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    internal partial class YYKeyenceReaderConsole : Form
    {
        //连接基恩士读码器个数
        private const int READER_COUNT = 30;
        //数据量buff最大值    
        private const int RECV_DATA_MAX = 10240;
        //读码器暂停读取数据时间 100毫秒 0为不等待
        private const int ACCURACY = 200;
        //基恩士读码器clientSocket数组
        public static ClientSocket[] clientSocketInstance;
        //接受各读码器server端数据的线程
        private Thread threadReceive = null;
        //定义一个代理委托用于改变控件内容
        private delegate void SetTextCallback(string message);
        //定义一个代理委托用于更新threadingProcessForm中控件的内容
        private delegate void UpdateTextBoxDelegate(object threadingProcessForm, string message);
        //定义当前窗体的变量
        private static YYKeyenceReaderConsole myselfForm = null;
        //读码器读取出来的字符串
        private static string[] strArr = null;
        //定义全局变量依次为接箍编号、炉号、批号
        private static string argCoupingNo = null, argHeatNo = null, argBatchNo = null;
        //读码器的状态,0代表未连接,1代表读码器连接成功,2代表异常
        public static int readerStatus=-1;

        #region 读码器窗体单例函数
        public static YYKeyenceReaderConsole getForm()
        {
            if (myselfForm == null)
            {
                new YYKeyenceReaderConsole();
            }
            return myselfForm;
        }
        #endregion

        #region 构造函数
        private YYKeyenceReaderConsole()
        {
            InitializeComponent();
            this.Font = new Font("宋体", 10, FontStyle.Bold);
            //初始化客户端Socket连接读码器数组
            clientSocketInstance = new ClientSocket[READER_COUNT];
            //遍历数组时的索引
            int readerIndex = 0;
            //定义读码器发送命令的端口(默认为9003)
            int CommandPort = 9003;
            //定义读码器发送数据的端口(默认为9004)
            int DataPort = 9004;
            //设置控件值
            CommandPortInput.Text = Convert.ToString(CommandPort);
            DataPortInput.Text = Convert.ToString(DataPort);
            try
            {
                //读取本地目录config.txt文件，读出所有读码器IP地址
                string configPath = Application.StartupPath + "\\config.txt";
                string str = File.ReadAllText(configPath);
                str = str.Replace("\n", "");
                string[] strIPArray = str.Split('\r');
                //两种方式初始化构造
                for (int i = 0; i < strIPArray.Length; i++)
                {
                    if (strIPArray[i].Length > 2)
                    {
                        //实例化客户端Socket连接读码器数组
                        clientSocketInstance[readerIndex] = new ClientSocket(strIPArray[i], CommandPort, DataPort);  // 9003 for command, 9004 for data
                        readerIndex++;
                    }
                }
            }
            catch (Exception e)
            {
                MessagePrompt.Show("实例化客户端连接读码器数组时错误,错误信息:" + e.Message);
            }
            finally {
                myselfForm = this;
            }
           
        }
        #endregion

        #region 读码器连接事件
        public void connect_Click(object sender, EventArgs e)
        {
            codeReaderConnect();
        }
        #endregion

        #region 读码器连接
        public static void codeReaderConnect()
        {
            for (int i = 0; i < READER_COUNT; i++)
            {
                try
                {
                    //如果客户端Socket连接读码器数组遍历完毕终止遍历
                    if (clientSocketInstance[i] == null)
                        break;
                    //设置当前ClientSocket实例的所用的端口
                    clientSocketInstance[i].readerCommandEndPoint.Port = Convert.ToInt32(myselfForm.CommandPortInput.Text);
                    clientSocketInstance[i].readerDataEndPoint.Port = Convert.ToInt32(myselfForm.DataPortInput.Text);
                    //如果当前ClientSocket实例的发送命令的socket实例已经存在，关闭socket
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        clientSocketInstance[i].commandSocket.Close();
                    }
                    //为当前ClientSocket实例创建新的发送命令的socket实例
                    clientSocketInstance[i].commandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " 连接中,请等待...");
                    //连接发送命令的socket
                    clientSocketInstance[i].commandSocket.Connect(clientSocketInstance[i].readerCommandEndPoint);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " 连接成功.");
                }
                //异常处理
                catch (ArgumentOutOfRangeException ex)
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " 连接失败,请检查网络设备.");
                    clientSocketInstance[i].commandSocket = null;
                    readerStatus = 2;
                    continue;
                }
                catch (SocketException ex)
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " 连接失败,请检查网络设备.");
                    clientSocketInstance[i].commandSocket = null;
                    readerStatus = 2;
                    continue;
                }
                try
                {
                    //如果当前ClientSocket实例的发送读码器数据的socket实例已经存在，关闭socket
                    if (clientSocketInstance[i].dataSocket != null)
                    {
                        clientSocketInstance[i].dataSocket.Close();
                    }
                    //注:readerCommandEndPoint和readerDataEndPoint在ClientSocket中有解释
                    //如果当前ClientSocket实例的readerCommandEndPoint的端口和readerDataEndPoint的端口相同
                    if (clientSocketInstance[i].readerCommandEndPoint.Port == clientSocketInstance[i].readerDataEndPoint.Port)
                    {
                        clientSocketInstance[i].dataSocket = clientSocketInstance[i].commandSocket;
                    }
                    else//创建新的读码器数据传输的socket实例
                    {
                        clientSocketInstance[i].dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " 数据端口连接中,请等待...");
                        //当前读码器数据传输的socket连接
                        clientSocketInstance[i].dataSocket.Connect(clientSocketInstance[i].readerDataEndPoint);
                        setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " 数据端口连接成功.");
                        //判断listBox_Reader控件中是否存在当前ip，不存在则添加
                        if (!myselfForm.listBox_Reader.Items.Contains(clientSocketInstance[i].readerCommandEndPoint.Address.ToString()))
                        {
                            myselfForm.listBox_Reader.Items.Add(clientSocketInstance[i].readerCommandEndPoint.Address.ToString());
                            myselfForm.listBox_Reader.Update();
                        }
                    }
                    //设置socket连接的超时时间(单位ms)
                    clientSocketInstance[i].dataSocket.ReceiveTimeout = 100;
                    readerStatus = 1;
                }
                //异常处理
                catch (SocketException ex)
                {
                    setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " 数据端口连接失败.");
                    clientSocketInstance[i].dataSocket = null;
                    readerStatus = 2;
                    continue;
                }

            }
        }
        #endregion

        #region 读码器断开连接
        private void disconnect_Click(object sender, EventArgs e)
        {
            //断开所有读码器连接
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                //如果当前实例的传输命令的socket实例存在,关闭socket
                if (clientSocketInstance[i].commandSocket != null)
                {
                    clientSocketInstance[i].commandSocket.Close();
                    clientSocketInstance[i].commandSocket = null;
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " 断开连接成功.");
                }
                //如果当前实例的传输数据的socket实例存在,关闭socket
                if (clientSocketInstance[i].dataSocket != null)
                {
                    clientSocketInstance[i].dataSocket.Close();
                    clientSocketInstance[i].dataSocket = null;
                    setLogText(clientSocketInstance[i].readerDataEndPoint.ToString() + " 断开连接成功.");
                    this.listBox_Reader.Items.Remove(clientSocketInstance[i].readerCommandEndPoint.Address.ToString());
                    this.listBox_Reader.Update();
                }
            }
        }
        #endregion

        #region 点击读码器开始读码
        private void lon_Click(object sender, EventArgs e)
        {
            codeReaderLon();
        }
        #endregion

        #region 点击读码器结束读码
        private void loff_Click(object sender, EventArgs e)
        {
            codeReaderOff();
        }
        #endregion

        #region 打印读码器内容
        private static void setLogText(string str)
        {
            try {
                myselfForm.textBox_LogConsole.AppendText(str);
                myselfForm.textBox_LogConsole.AppendText("\r\n");
                myselfForm.textBox_LogConsole.Update();
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        #region 读码器接收数据事件(暂不使用)
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
        #endregion

        #region 读码器对焦事件(向读码器发送"FTUNE\r"指令)
        private void button_FTune_Click(object sender, EventArgs e)
        {
            string tune = "FTUNE\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(tune);
            //遍历所有已连接的读码器
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                //如果发送命令的socket存在
                if (clientSocketInstance[i].commandSocket != null)
                {
                    //向读码器发送对焦指令
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " FTUNE sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                }
            }

        } 
        #endregion

        #region 读码器学习事件(向读码器发送"TUNE,01\r"指令)
        private void button_Tune_Click(object sender, EventArgs e)
        {
            string tune = "TUNE,01\r";   // CR is terminator
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(tune);
            //遍历所有已连接的读码器
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                //如果发送命令的socket存在
                if (clientSocketInstance[i].commandSocket != null)
                {
                    //向读码器发送学习指令
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " TUNE sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                }
            }
        }
        #endregion

        #region 读码器重置事件(向读码器发送"RESET\r"指令)
        private void button_Reset_Click(object sender, EventArgs e)
        {
            string reset = "RESET\r";
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(reset);
            //遍历所有已连接的读码器
            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
            {
                //如果发送命令的socket存在
                if (clientSocketInstance[i].commandSocket != null)
                {
                    //向读码器发送重置指令
                    clientSocketInstance[i].commandSocket.Send(command);
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + " RESET sent.");
                }
                else
                {
                    setLogText(clientSocketInstance[i].readerCommandEndPoint.ToString() + "is disconnected.");
                }
            }
        }
        #endregion

        #region 读码器读码事件(向读码器发送"LON\r"指令)
        public static void codeReaderLon()
        {
            try
            {
                string lon = "LON\r";   // CR is terminator
                Byte[] command = ASCIIEncoding.ASCII.GetBytes(lon);
                //遍历所有已连接的读码器
                for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
                {
                    //如果发送命令的socket存在
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        //向读码器发送读码指令
                        clientSocketInstance[i].commandSocket.Send(command);
                        readerStatus = 1;
                    }
                    else
                    {
                        SetTextTwo("读码器已经断开连接，请重新连接扫码!");
                        readerStatus = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                readerStatus = 2;
                MessageBox.Show("开启时出错："+ex.Message);
            }
        }
        #endregion

        #region 读码器结束读码事件(向读码器发送"LOFF\r"指令)
        public static void codeReaderOff()
        {
            try
            {
                string loff = "LOFF\r"; // CR is terminator
                Byte[] command = ASCIIEncoding.ASCII.GetBytes(loff);
                //遍历所有已连接的读码器
                for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
                {
                    //如果发送命令的socket存在
                    if (clientSocketInstance[i].commandSocket != null)
                    {
                        //向读码器发送结束读码指令
                        clientSocketInstance[i].commandSocket.Send(command);
                        SetTextTwo(clientSocketInstance[i].readerCommandEndPoint.ToString() + " LOFF sent.");
                        readerStatus = 1;
                    }
                    else
                    {
                        SetTextTwo(clientSocketInstance[i].readerCommandEndPoint.ToString() + " is disconnected");
                    }
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        //#region 接收读码器读出的数据
        //public void Receive()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            Byte[] recvBytes = new Byte[RECV_DATA_MAX];
        //            int recvSize = 0;
        //            //实际接收到的字节数
        //            for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
        //            {
        //                if (clientSocketInstance[i].dataSocket != null)
        //                {
        //                    try
        //                    {
        //                        recvSize = clientSocketInstance[i].dataSocket.Receive(recvBytes);
        //                    }
        //                    catch (SocketException)
        //                    {
        //                        //
        //                        // Catch the exception, if cannot receive any data.
        //                        //
        //                        recvSize = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    // MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " is disconnected.\r\n");
        //                    continue;
        //                }

        //                if (recvSize == 0)
        //                {
        //                    //  MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + " has no data.\r\n");
        //                }
        //                else
        //                {
        //                    //
        //                    // Show the receive data after converting the receive data to Shift-JIS.
        //                    // Terminating null to handle as string.
        //                    //
        //                    recvBytes[recvSize] = 0;
        //                    SetText(Encoding.GetEncoding("Shift_JIS").GetString(recvBytes));
        //                    //this.SetText("[" + clientSocketInstance[i].readerDataEndPoint.ToString() + "] " + Encoding.GetEncoding("Shift_JIS").GetString(recvBytes) + "\r\n");

        //                    //MessageBox.Show(clientSocketInstance[i].readerDataEndPoint.ToString() + "\r\n" + Encoding.GetEncoding("Shift_JIS").GetString(recvBytes)+"\r\n");
        //                    //textbox_DataConsole.Text += clientSocketInstance[i].readerCommandEndPoint.ToString() + "LOFF sent.\r\n";
        //                    //textbox_DataConsole.Update();
        //                }
        //            }
        //            if (ACCURACY > 0)
        //                Thread.Sleep(ACCURACY);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessagePrompt.Show("接收服务端发送的消息出错:" + ex.ToString());
        //    }


        //}
        //#endregion

        #region 接收数据读码器读出的数据
        public void codeReaderReceive()
        {
            try
            {
                while (true)
                {
                    //如果读码器尚未连接 线程暂停1秒中
                    if (readerStatus != 1) {
                        Thread.Sleep(1000);
                        continue;
                    }
                    Byte[] recvBytes = new Byte[RECV_DATA_MAX];
                    int recvSize = 0;
                    //实际接收到的字节数
                    for (int i = 0; i < READER_COUNT && clientSocketInstance[i] != null; i++)
                    {
                        if (clientSocketInstance[i].dataSocket != null)
                        {
                            try
                            {
                                //让当前的读码器开始读取数据
                                recvSize = clientSocketInstance[i].dataSocket.Receive(recvBytes);
                            }
                            catch (SocketException ex)
                            {
                                recvSize = 0; 
                            }
                        }
                        else
                        {
                            continue;
                        }
                        if (recvSize!= 0)
                        {
                            recvBytes[recvSize] = 0;
                            //如果读取到的数据不是Error,则设置到指定的输入框中
                            if (!Encoding.UTF8.GetString(recvBytes).TrimEnd().Contains("ERROR"))
                            {
                                UpdateTextBox(ThreadingForm.getMyForm(), Encoding.UTF8.GetString(recvBytes).TrimEnd());
                            }
                            //更新读码器读出的数据到日志中
                            SetText(DateTime.Now.ToString() + "    " + Encoding.UTF8.GetString(recvBytes));
                        }
                    }
                    if (ACCURACY > 0)
                        Thread.Sleep(ACCURACY);
                }
            }
            catch (Exception ex)
            {
                SetText("读码器读数据时出错,错误信息:" + ex.ToString());
            }
        }
        #endregion

        #region 更新TextBox内容
        private static void UpdateTextBox(Object form, string message)
        {
            try
            {
                //目前读码器读出的数据格式一般分为两种,一种时读出的量具编号,另一种时接箍编号、炉号、批号的拼接(空格拼接)
                //如果当前读码器读出的是量具编号
                if (ThreadingForm.isMeasuringToolTabSelected)
                {
                    //判断是否是跨线程访问控件
                    if (AlphabetKeyboardForm.getForm().Textbox_display.InvokeRequired)
                    {
                        UpdateTextBoxDelegate md = new UpdateTextBoxDelegate(UpdateTextBox);
                        AlphabetKeyboardForm.getForm().Textbox_display.Invoke(md, new object[] { (object)AlphabetKeyboardForm.getForm(), message });
                    }
                    else
                    {
                        //设置英文输入法中输入的内容为读码器读出的内容
                        AlphabetKeyboardForm.getForm().Textbox_display.Text = message;
                    }
                    return;
                }
                //将读码器读出的数据以空格分隔
                strArr = Regex.Split(message, "\\s+");
                //如果读码器读接箍内容则读出的数据格式目前如:"12323 43434 5454 5454"
                if (strArr.Length > 3)
                {
                    argHeatNo = strArr[1];//炉号
                    argBatchNo = strArr[2];//批号
                    argCoupingNo = strArr[3];//接箍编号
                }
                //判断是否是跨线程访问控件
                if (ThreadingForm.getMyForm().txtCoupingNo.InvokeRequired)
                {
                    UpdateTextBoxDelegate md = new UpdateTextBoxDelegate(UpdateTextBox);
                    //设置表单上接箍编号、炉号、批号控件内容
                    if (!string.IsNullOrWhiteSpace(argCoupingNo))
                        ThreadingForm.getMyForm().txtCoupingNo.Invoke(md, new object[] { form, argCoupingNo });
                    if (!string.IsNullOrWhiteSpace(argHeatNo))
                        ThreadingForm.getMyForm().txtHeatNo.Invoke(md, new object[] { form, argHeatNo });
                    if (!string.IsNullOrWhiteSpace(argBatchNo))
                        ThreadingForm.getMyForm().txtBatchNo.Invoke(md, new object[] { form, argBatchNo });
                }
                else
                {
                    //设置表单上接箍编号、炉号、批号控件内容
                    if (!string.IsNullOrWhiteSpace(argCoupingNo))
                        ThreadingForm.getMyForm().txtCoupingNo.Text = argCoupingNo;
                    if (!string.IsNullOrWhiteSpace(argHeatNo))
                        ThreadingForm.getMyForm().txtHeatNo.Text = argHeatNo;
                    if (!string.IsNullOrWhiteSpace(argBatchNo))
                        ThreadingForm.getMyForm().txtBatchNo.Text = argBatchNo;
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 向显示读码器读出数据的显示框追加内容
        private void SetText(string text)
        {
            try
            {
                //判断控件是否在另一个线程中,如果在跨线程访问控件
                if (this.textbox_DataConsole.InvokeRequired)
                {
                    //创建委托实例
                    SetTextCallback d = new SetTextCallback(SetText);
                    //调用控件的Invoke方法(Invoke方法会顺着控件树向上搜索，直到找到创建控件的那个线程（通常是主线程），然后进入那个线程改变控件的外观，确保不发生线程冲突)
                    this.textbox_DataConsole.Invoke(d, new object[] { text });
                }
                else//如果不是跨线程访问则直接操作控件
                {
                    //向textbox_DataConsole(显示读码器读出数据的控件)追加内容
                    this.textbox_DataConsole.AppendText(text);
                    //textbox_DataConsole内容换行
                    this.textbox_DataConsole.AppendText("\r\n");
                    //textbox_DataConsole内容刷新
                    this.textbox_DataConsole.Refresh();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 向显示读码器读出数据的显示框追加内容(静态方法)
        private static void SetTextTwo(string text)
        {
            try
            {
                if (myselfForm.textbox_DataConsole.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetTextTwo);
                    myselfForm.textbox_DataConsole.Invoke(d, new object[] { text });
                }
                else
                {
                    myselfForm.textbox_DataConsole.AppendText(text);
                    myselfForm.textbox_DataConsole.AppendText("\r\n");
                    myselfForm.textbox_DataConsole.Refresh();
                }
            }
            catch(Exception e)
            {

            }
        } 
        #endregion

        #region 关闭窗体事件
        private void btnHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        } 
        #endregion

        #region 清理窗体的打印日志
        private void button_Clear_Click(object sender, EventArgs e)
        {
            textBox_LogConsole.Text = "";
            textbox_DataConsole.Text = "";
        }
        #endregion

        #region 读码器窗体Load函数
        private void YYKeyenceReaderConsole_Load(object sender, EventArgs e)
        {
            try
            {
                //设置读码器读取数据后台线程
                threadReceive = new Thread(new ThreadStart(codeReaderReceive));
                threadReceive.IsBackground = true;
                threadReceive.Start();
            }
            catch (Exception ex)
            {
            }
        } 
        #endregion
    }
}
