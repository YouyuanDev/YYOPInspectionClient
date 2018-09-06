using NVRCsharpDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class MainWindow : Form
    {
        private bool m_bInitSDK = false;
        private static bool m_bRecord = false;
        private static uint iLastErr = 0;
        private static Int32 m_lUserID = -1;
        public static Int32 m_lRealHandle = -1;
        private string str1;
        private string str2;
        private Int32 i = 0;
        private Int32 m_lTree = 0;
        private static string str;
        public static long iSelIndex = 0;
        private uint dwAChanTotalNum = 0;
        private uint dwDChanTotalNum = 0;
        private Int32 m_lPort = -1;
        private IntPtr m_ptrRealHandle;
        private int[] iIPDevID = new int[96];
        private static int loopLogo= 0;
        public static int recordStatus=0;//录像机状态,0代表未登录,1代表登录成功,2代表未启动,3代表已启动,4代表录像中，5代表其他
        public static int[] iChannelNum = new int[96];
        private CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
        public CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode;
        public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
        private PlayCtrl.DECCBFUN m_fDisplayFun = null;
        public delegate void MyDebugInfo(string str);
        public static MainWindow myForm = null;
        private delegate void SetLogCallback(string message);
        //新增，保存窗体的大小和初始位置  保存录像显示窗口的大小和初始位置
        public static int mainWindowX = 0;
        public static int mainWindowY = 0;
        public static int mainWindowWidth = 0;
        public static int mainWindowHeight = 0;
        public static int realTimeX = 0;
        public static int realTimeY = 0;
        public static int realTimeWidth = 0;
        public static int realTimeHeigh = 0;
        //设置是表单界面点击了录制视频事件，还是设置录像机页面点解了录制视频事件
        public static bool isRecordClick = true;
        public static bool isRealPicClick =true;

        #region 单例函数
        public static MainWindow getForm()
        {
            if (myForm == null)
            {
                new MainWindow();
            }
            return myForm;
        }
        #endregion

        #region 构造函数
        private MainWindow()
        {
            InitializeComponent();
            //设置窗体总体字体大小
            this.Font = new Font("宋体", 12, FontStyle.Bold);
            try
            {
                //初始化数据,用于录像机页面重置
                mainWindowX = this.Left; mainWindowY = this.Top;
                mainWindowWidth = this.Width; mainWindowHeight = this.Height;
                realTimeX = RealPlayWnd.Left; realTimeY = RealPlayWnd.Top;
                realTimeWidth = RealPlayWnd.Width; realTimeHeigh = RealPlayWnd.Height;
                RealPlayWnd.Dock = DockStyle.None;
                m_bInitSDK = CHCNetSDK.NET_DVR_Init();
                if (m_bInitSDK == false)
                {
                    DebugInfo("录像机初始化失败!");
                    recordStatus = 5;
                    return;
                }
                else
                {
                    //保存SDK日志 To save the SDK log 
                    CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
                    comboBoxView.SelectedIndex = 0;
                    for (int i = 0; i < 64; i++)
                    {
                        iIPDevID[i] = -1;
                        iChannelNum[i] = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("初始化录像机错误,错误信息:"+ex.Message);
            }
            finally {
                myForm = this;
            }
        }
        #endregion

        #region 窗体日志输出
        public static void DebugInfo(string str)
        {
            str = str + "\r\n";
            if (myForm.TextBoxInfo.InvokeRequired)
            {
                SetLogCallback d = new SetLogCallback(DebugInfo);
                myForm.TextBoxInfo.Invoke(d, new object[] { str });
            }
            else
            {
                myForm.TextBoxInfo.AppendText(str);

            }
        }
        #endregion

        #region 点击录像机登录事件
        public void btnLogin_Click(object sender, EventArgs e)
        {
            if (m_lUserID < 0)
            {
                string DVRIPAddress = textBoxIP.Text; //设备IP地址或者域名 Device IP
                Int16 DVRPortNumber = Int16.Parse(textBoxPort.Text);//设备服务端口号 Device Port
                string DVRUserName = textBoxUserName.Text;//设备登录用户名 User name to login
                string DVRPassword = textBoxPassword.Text;//设备登录密码 Password to login

                if (checkBoxHiDDNS.Checked)
                {
                    byte[] HiDDNSName = System.Text.Encoding.Default.GetBytes(textBoxIP.Text);
                    byte[] GetIPAddress = new byte[16];
                    uint dwPort = 0;
                    if (!CHCNetSDK.NET_DVR_GetDVRIPByResolveSvr_EX("www.hik-online.com", (ushort)80, HiDDNSName, (ushort)HiDDNSName.Length, null, 0, GetIPAddress, ref dwPort))
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        str = "NET_DVR_GetDVRIPByResolveSvr_EX failed, error code= " + iLastErr; //域名解析失败，输出错误号 Failed to login and output the error code
                        DebugInfo(str);
                        return;
                    }
                    else
                    {
                        DVRIPAddress = System.Text.Encoding.UTF8.GetString(GetIPAddress).TrimEnd('\0');
                        DVRPortNumber = (Int16)dwPort;
                    }
                }

                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    //str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                    str = "登录录像机失败,请检查录像机网络,错误代码:" + iLastErr;
                    DebugInfo(str);
                    recordStatus = 5;
                    return;
                }
                else
                {
                    //登录成功
                    //DebugInfo("NET_DVR_Login_V30 succ!");
                    DebugInfo("登录成功,连接上录像机!");
                    //btnLogin.Text = "Logout";
                    btnLogin.Text = "退出";
                    recordStatus = 1;
                    dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                    dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
                    if (dwDChanTotalNum > 0)
                    {
                        InfoIPChannel();
                    }
                    else
                    {
                        for (i = 0; i < dwAChanTotalNum; i++)
                        {
                            ListAnalogChannel(i + 1, 1);
                            iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                        }

                        comboBoxView.SelectedItem = 1;
                        // MessageBox.Show("This device has no IP channel!");
                    }
                }

            }
            else
            {
                //注销登录 Logout the device
                if (m_lRealHandle >= 0)
                {
                    //DebugInfo("Please stop live view firstly"); //登出前先停止预览 Stop live view before logout
                    DebugInfo("请先停止预览，然后再退出登录!");
                    return;
                }

                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "退出登录失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    DebugInfo(str);
                    return;
                }
                // DebugInfo("NET_DVR_Logout succ!");
                DebugInfo("退出成功!");
                recordStatus = 0;
                listViewIPChannel.Items.Clear();//清空通道列表 Clean up the channel list
                m_lUserID = -1;
                // btnLogin.Text = "Login";
                btnLogin.Text = "登录";
            }
            return;
        } 
        #endregion

        public void InfoIPChannel()
        {
            uint dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40);

            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
            Marshal.StructureToPtr(m_struIpParaCfgV40, ptrIpParaCfgV40, false);

            uint dwReturn = 0;
            int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_GET_IPPARACFG_V40 failed, error code= " + iLastErr;
                //获取IP资源配置信息失败，输出错误号 Failed to get configuration of IP channels and output the error code
                DebugInfo(str);
            }
            else
            {
                DebugInfo("NET_DVR_GET_IPPARACFG_V40 succ!");

                m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));

                for (i = 0; i < dwAChanTotalNum; i++)
                {
                    ListAnalogChannel(i + 1, m_struIpParaCfgV40.byAnalogChanEnable[i]);
                    iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                }

                byte byStreamType = 0;
                uint iDChanNum = 64;

                if (dwDChanTotalNum < 64)
                {
                    iDChanNum = dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                }

                for (i = 0; i < iDChanNum; i++)
                {
                    iChannelNum[i + dwAChanTotalNum] = i + (int)m_struIpParaCfgV40.dwStartDChan;
                    byStreamType = m_struIpParaCfgV40.struStreamMode[i].byGetStreamType;

                    dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40.struStreamMode[i].uGetStream);
                    switch (byStreamType)
                    {
                        //目前NVR仅支持直接从设备取流 NVR supports only the mode: get stream from device directly
                        case 0:
                            IntPtr ptrChanInfo = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfo, false);
                            m_struChanInfo = (CHCNetSDK.NET_DVR_IPCHANINFO)Marshal.PtrToStructure(ptrChanInfo, typeof(CHCNetSDK.NET_DVR_IPCHANINFO));

                            //列出IP通道 List the IP channel
                            ListIPChannel(i + 1, m_struChanInfo.byEnable, m_struChanInfo.byIPID);
                            iIPDevID[i] = m_struChanInfo.byIPID + m_struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            //列出IP通道 List the IP channel
                            ListIPChannel(i + 1, m_struChanInfoV40.byEnable, m_struChanInfoV40.wIPID);
                            iIPDevID[i] = m_struChanInfoV40.wIPID - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfoV40);
                            break;
                        default:
                            break;
                    }
                }
            }
            Marshal.FreeHGlobal(ptrIpParaCfgV40);

        }
        public void ListIPChannel(Int32 iChanNo, byte byOnline, int byIPID)
        {
            str1 = String.Format("IPCamera {0}", iChanNo);
            m_lTree++;

            if (byIPID == 0)
            {
                str2 = "X"; //通道空闲，没有添加前端设备 the channel is idle                  
            }
            else
            {
                if (byOnline == 0)
                {
                    str2 = "offline"; //通道不在线 the channel is off-line
                }
                else
                    str2 = "online"; //通道在线 The channel is on-line
            }

            listViewIPChannel.Items.Add(new ListViewItem(new string[] { str1, str2 }));//将通道添加到列表中 add the channel to the list
        }
        public void ListAnalogChannel(Int32 iChanNo, byte byEnable)
        {
            str1 = String.Format("Camera {0}", iChanNo);
            m_lTree++;

            if (byEnable == 0)
            {
                str2 = "Disabled"; //通道已被禁用 This channel has been disabled               
            }
            else
            {
                str2 = "Enabled"; //通道处于启用状态 This channel has been enabled
            }

            listViewIPChannel.Items.Add(new ListViewItem(new string[] { str1, str2 }));//将通道添加到列表中 add the channel to the list
        }

        private void listViewIPChannel_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listViewIPChannel.SelectedItems.Count > 0)
            {
                iSelIndex = listViewIPChannel.SelectedItems[0].Index;  //当前选中的行
            }
        }

        //解码回调函数
        private void DecCallbackFUN(int nPort, IntPtr pBuf, int nSize, ref PlayCtrl.FRAME_INFO pFrameInfo, int nReserved1, int nReserved2)
        {
            // 将pBuf解码后视频输入写入文件中（解码后YUV数据量极大，尤其是高清码流，不建议在回调函数中处理）
            if (pFrameInfo.nType == 3) //#define T_YV12	3
            {
                //    FileStream fs = null;
                //    BinaryWriter bw = null;
                //    try
                //    {
                //        fs = new FileStream("DecodedVideo.yuv", FileMode.Append);
                //        bw = new BinaryWriter(fs);
                //        byte[] byteBuf = new byte[nSize];
                //        Marshal.Copy(pBuf, byteBuf, 0, nSize);
                //        bw.Write(byteBuf);
                //        bw.Flush();
                //    }
                //    catch (System.Exception ex)
                //    {
                //        MessageBox.Show(ex.ToString());
                //    }
                //    finally
                //    {
                //        bw.Close();
                //        fs.Close();
                //    }
            }
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            //下面数据处理建议使用委托的方式
            MyDebugInfo AlarmInfo = new MyDebugInfo(DebugInfo);
            switch (dwDataType)
            {
                case CHCNetSDK.NET_DVR_SYSHEAD:     // sys head
                    if (dwBufSize > 0)
                    {
                        if (m_lPort >= 0)
                        {
                            return; //同一路码流不需要多次调用开流接口
                        }

                        //获取播放句柄 Get the port to play
                        if (!PlayCtrl.PlayM4_GetPort(ref m_lPort))
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_GetPort failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                            break;
                        }

                        //设置流播放模式 Set the stream mode: real-time stream mode
                        if (!PlayCtrl.PlayM4_SetStreamOpenMode(m_lPort, PlayCtrl.STREAME_REALTIME))
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "Set STREAME_REALTIME mode failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                        }

                        //打开码流，送入头数据 Open stream
                        if (!PlayCtrl.PlayM4_OpenStream(m_lPort, pBuffer, dwBufSize, 2 * 1024 * 1024))
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_OpenStream failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                            break;
                        }


                        //设置显示缓冲区个数 Set the display buffer number
                        if (!PlayCtrl.PlayM4_SetDisplayBuf(m_lPort, 15))
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_SetDisplayBuf failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                        }

                        //设置显示模式 Set the display mode
                        if (!PlayCtrl.PlayM4_SetOverlayMode(m_lPort, 0, 0/* COLORREF(0)*/)) //play off screen 
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_SetOverlayMode failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                        }

                        //设置解码回调函数，获取解码后音视频原始数据 Set callback function of decoded data
                        m_fDisplayFun = new PlayCtrl.DECCBFUN(DecCallbackFUN);
                        if (!PlayCtrl.PlayM4_SetDecCallBackEx(m_lPort, m_fDisplayFun, IntPtr.Zero, 0))
                        {
                            this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
                        }

                        //开始解码 Start to play                       
                        if (!PlayCtrl.PlayM4_Play(m_lPort, m_ptrRealHandle))
                        {
                            iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_Play failed, error code= " + iLastErr;
                            this.BeginInvoke(AlarmInfo, str);
                            break;
                        }
                    }
                    break;
                case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
                    if (dwBufSize > 0 && m_lPort != -1)
                    {
                        for (int i = 0; i < 999; i++)
                        {
                            //送入码流数据进行解码 Input the stream data to decode
                            if (!PlayCtrl.PlayM4_InputData(m_lPort, pBuffer, dwBufSize))
                            {
                                iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                                str = "PlayM4_InputData failed, error code= " + iLastErr;
                                Thread.Sleep(2);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
                default:
                    if (dwBufSize > 0 && m_lPort != -1)
                    {
                        //送入其他数据 Input the other data
                        for (int i = 0; i < 999; i++)
                        {
                            if (!PlayCtrl.PlayM4_InputData(m_lPort, pBuffer, dwBufSize))
                            {
                                iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                                str = "PlayM4_InputData failed, error code= " + iLastErr;
                                Thread.Sleep(2);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
            }
        }


        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            //录像保存路径和文件名 the path and file name to save
            //根据时间戳保存视频文件
            string sVideoDir = Application.StartupPath+"\\vcr\\";
            string sVideoFileName = sVideoDir + GetTimeStamp() + "_vcr.mp4";

            if (!Directory.Exists(sVideoDir)) {
                Directory.CreateDirectory(sVideoDir);
            }
            
            if (m_bRecord == false)
            {
                if (m_lUserID<0)
                {
                    DebugInfo("录像失败,请先登录!");
                    return;
                }
                if (m_lRealHandle < 0) {
                    DebugInfo("录像失败,请先开启预览!");
                    return;
                }
                //强制I帧 Make one key frame
                int lChannel = iChannelNum[(int)iSelIndex]; //通道号 Channel number
                CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, lChannel);

                //开始录像 Start recording
                if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    //str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                    DebugInfo("录像失败,错误代码:"+iLastErr);
                    return;
                }
                else
                {
                    DebugInfo("录像成功,录像中......");
                    recordStatus = 4;
                    //DebugInfo("NET_DVR_SaveRealData succ!");
                    btnRecord.Text = "Stop";
                    m_bRecord = true;
                }
            }
            else
            {
                //停止录像 Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    //str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                    DebugInfo("停止录像失败,错误代码:"+iLastErr);
                    return;
                }
                else
                {
                    //str = "NET_DVR_StopSaveRealData succ and the saved file is " + sVideoFileName;
                    DebugInfo("录像完成,视频保存位置:"+sVideoFileName);
                    btnRecord.Text = "Record";
                    m_bRecord = false;
                }
            }
            return;
        }

        #region 录像登录函数
        public static void recordLogin()
        {
            if (m_lUserID < 0)
            {
                string DVRIPAddress = myForm.textBoxIP.Text; //设备IP地址或者域名 Device IP
                Int16 DVRPortNumber = Int16.Parse(myForm.textBoxPort.Text);//设备服务端口号 Device Port
                string DVRUserName = myForm.textBoxUserName.Text;//设备登录用户名 User name to login
                string DVRPassword = myForm.textBoxPassword.Text;//设备登录密码 Password to login

                if (myForm.checkBoxHiDDNS.Checked)
                {
                    byte[] HiDDNSName = System.Text.Encoding.Default.GetBytes(myForm.textBoxIP.Text);
                    byte[] GetIPAddress = new byte[16];
                    uint dwPort = 0;
                    if (!CHCNetSDK.NET_DVR_GetDVRIPByResolveSvr_EX("www.hik-online.com", (ushort)80, HiDDNSName, (ushort)HiDDNSName.Length, null, 0, GetIPAddress, ref dwPort))
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        str = "NET_DVR_GetDVRIPByResolveSvr_EX failed, error code= " + iLastErr; //域名解析失败，输出错误号 Failed to login and output the error code
                        DebugInfo(str);
                        return;
                    }
                    else
                    {
                        DVRIPAddress = System.Text.Encoding.UTF8.GetString(GetIPAddress).TrimEnd('\0');
                        DVRPortNumber = (Int16)dwPort;
                    }
                }

                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref myForm.DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    //str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                    str = "登录录像机失败,请检查录像机网络,错误代码:" + iLastErr;
                    MessageBox.Show(str);
                    DebugInfo(str);
                    return;
                }
                else
                {
                    //登录成功
                    //DebugInfo("NET_DVR_Login_V30 succ!");
                    DebugInfo("登录成功,连接上录像机!");
                    recordStatus =1;

                    //btnLogin.Text = "Logout";
                    myForm.btnLogin.Text = "退出";
                     MessageBox.Show("登录成功, 连接上录像机!");
                    myForm.dwAChanTotalNum = (uint)myForm.DeviceInfo.byChanNum;
                    myForm.dwDChanTotalNum = (uint)myForm.DeviceInfo.byIPChanNum + 256 * (uint)myForm.DeviceInfo.byHighDChanNum;
                    if (myForm.dwDChanTotalNum > 0)
                    {
                        myForm.InfoIPChannel();
                    }
                    else
                    {
                        for (loopLogo = 0; loopLogo < myForm.dwAChanTotalNum; loopLogo++)
                        {
                            myForm.ListAnalogChannel(loopLogo + 1, 1);
                            iChannelNum[loopLogo] = loopLogo + (int)myForm.DeviceInfo.byStartChan;
                        }

                        myForm.comboBoxView.SelectedItem = 1;
                        // MessageBox.Show("This device has no IP channel!");
                    }
                }

            }
            else
            {
                //注销登录 Logout the device
                if (m_lRealHandle >= 0)
                {
                    //DebugInfo("Please stop live view firstly"); //登出前先停止预览 Stop live view before logout
                    DebugInfo("请先停止预览，然后再退出登录!");
                    return;
                }

                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "退出登录失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    DebugInfo(str);
                    return;
                }
                // DebugInfo("NET_DVR_Logout succ!");
                DebugInfo("退出成功!");
                recordStatus = 0;
                myForm.listViewIPChannel.Items.Clear();//清空通道列表 Clean up the channel list
                m_lUserID = -1;
                // btnLogin.Text = "Login";
                myForm.btnLogin.Text = "登录";
            }
            return;
        }
        #endregion

        #region 录像预览函数
        public static void recordPreview()
        {
            if (m_lUserID < 0)
            {
                //MessagePrompt.Show("请检查是否登录录像机!");
                return;
            }
            if (m_bRecord)
            {
                //MessagePrompt.Show("预览前请先停止正在录制的录像机!");
                return;
            }

            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = myForm.RealPlayWnd.Handle;//预览窗口 live view window
                lpPreviewInfo.lChannel = iChannelNum[(int)iSelIndex];//预览的设备通道 the device channel number
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 

                if (myForm.comboBoxView.SelectedIndex == 0)
                {
                    //打开预览 Start live view 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                }
                else
                {
                    lpPreviewInfo.hPlayWnd = IntPtr.Zero;//预览窗口 live view window
                    myForm.m_ptrRealHandle = myForm.RealPlayWnd.Handle;
                    myForm.RealData = new CHCNetSDK.REALDATACALLBACK(myForm.RealDataCallBack);//预览实时流回调函数 real-time stream callback function 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, myForm.RealData, pUser);
                }

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "预览失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    DebugInfo(str);
                    return;
                }
                else
                {
                    //预览成功
                    DebugInfo("预览成功!");
                    recordStatus = 3;
                    //DebugInfo("NET_DVR_RealPlay_V40 succ!");
                    myForm.btnPreview.Text = "关闭录像机";
                }
            }
            else
            {
                //停止预览 Stop live view 
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "停止预览失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    DebugInfo(str);
                    return;
                }

                if ((myForm.comboBoxView.SelectedIndex == 1) && (myForm.m_lPort >= 0))
                {
                    if (!PlayCtrl.PlayM4_Stop(myForm.m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(myForm.m_lPort);
                        str = "PlayM4_Stop failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    if (!PlayCtrl.PlayM4_CloseStream(myForm.m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(myForm.m_lPort);
                        str = "PlayM4_CloseStream failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    if (!PlayCtrl.PlayM4_FreePort(myForm.m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(myForm.m_lPort);
                        str = "PlayM4_FreePort failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    myForm.m_lPort = -1;
                }
                DebugInfo("停止预览成功!");
                recordStatus = 2;
                //DebugInfo("NET_DVR_StopRealPlay succ!");
                m_lRealHandle = -1;
                myForm.btnPreview.Text = "启动录像机";
                myForm.RealPlayWnd.Invalidate();//刷新窗口 refresh the window
            }
            return;
        } 
        #endregion

        #region 录制视频函数
        public static void RecordVideo(string timestamp)
        {
            try
            {
                if (m_lUserID < 0)
                {
                    DebugInfo("录像失败,请先登录[表单发过来请求]!");
                }
                if (m_lRealHandle < 0)
                {
                    DebugInfo("录像失败,请先开启预览[表单发过来请求]!");
                }
                string coupingDir = Application.StartupPath + "\\draft\\";
                //MessageBox.Show(coupingDir);
                string sVideoFileName = coupingDir+timestamp + ".mp4";
                if (m_bRecord == false)
                {
                    if (!File.Exists(coupingDir))
                    {
                        Directory.CreateDirectory(coupingDir);
                    }
                    //强制I帧 Make one key frame
                    int lChannel = iChannelNum[(int)iSelIndex]; //通道号 Channel number
                    CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, lChannel);

                    //开始录像 Start recording
                    if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                    {
                        //MessageBox.Show("开始录像");
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        //str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                        DebugInfo("录像失败[表单发过来请求],错误代码:" + iLastErr);
                    }
                    else
                    {
                        DebugInfo("录像成功[表单发过来请求]!");
                        //btnRecord.Text = "Stop";
                        m_bRecord = true;
                        recordStatus = 4;
                    }
                }
            }
            catch (Exception e)
            {
                DebugInfo("录像失败,错误代码:" + iLastErr);
            }
        }
        #endregion

        #region 停止视频录制
        public static void stopRecordVideo()
        {
            if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                DebugInfo(str);
                return;
            }
            else
            {
                m_bRecord = false;
                recordStatus = 3;
            }
        } 
        #endregion


        private void btn_Exit_Click(object sender, EventArgs e)
        {
            //停止预览
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }

            //注销登录
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                m_lUserID = -1;
            }

            CHCNetSDK.NET_DVR_Cleanup();

            //Application.Exit();
        }

        private void checkBoxHiDDNS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHiDDNS.Checked)
            {
                //HiDDNS域名方式访问设备
                label5.Text = "HiDDNS域名";
                label1.Text = "HiDDNS Domain";
                textBoxIP.Text = "a1234test";
                textBoxPort.Enabled = false;
            }
            else
            {
                //IP地址或者普通域名方式访问设备
                label5.Text = "设备IP/域名";
                label1.Text = "Device IP/Domain";
                textBoxIP.Text = "192.168.0.90";
                textBoxPort.Enabled = true;
            }
        }

        private void listViewIPChannel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int iCurChan = iChannelNum[(int)iSelIndex];
                if (iCurChan >= m_struIpParaCfgV40.dwStartDChan)
                {
                    if (DialogResult.OK == MessageBox.Show("是否配置该IP通道！", "配置提示", MessageBoxButtons.OKCancel))
                    {
                        IPChannelConfig dlg = new IPChannelConfig();
                        dlg.m_struIPParaCfgV40 = m_struIpParaCfgV40;
                        dlg.m_lUserID = m_lUserID;
                        int iCurChanIndex = iCurChan - (int)m_struIpParaCfgV40.dwStartDChan; //通道索引
                        int iCurIPDevIndex = iIPDevID[iCurChanIndex]; //设备ID索引
                        dlg.iIPDevIndex = iCurIPDevIndex;
                        dlg.iChanIndex = iCurChanIndex;
                        dlg.ShowDialog();
                    }
                }
                else
                {

                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //刷新通道列表
            listViewIPChannel.Items.Clear();
            for (i = 0; i < dwAChanTotalNum; i++)
            {
                ListAnalogChannel(i + 1, 1);
                iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
            }
            InfoIPChannel();
        }
        //预览事件
        public void btnPreview_Click_1(object sender, EventArgs e)
        {
            //recordPreview();
            if (m_lUserID < 0)
            {
                //MessagePrompt.Show("请检查是否登录录像机!");
                //MessageBox.Show("Please login the device firstly!");
                return;
            }
            if (m_bRecord)
            {
                //MessagePrompt.Show("预览前请先停止正在录制的录像机!");
                //MessageBox.Show("Please stop recording firstly!");
                return;
            }

            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口 live view window
                lpPreviewInfo.lChannel = iChannelNum[(int)iSelIndex];//预览的设备通道 the device channel number
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 

                if (comboBoxView.SelectedIndex == 0)
                {
                    //打开预览 Start live view 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                }
                else
                {
                    lpPreviewInfo.hPlayWnd = IntPtr.Zero;//预览窗口 live view window
                    m_ptrRealHandle = RealPlayWnd.Handle;
                    RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数 real-time stream callback function 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, RealData, pUser);
                }

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "预览失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    DebugInfo(str);
                    return;
                }
                else
                {
                    //预览成功
                    DebugInfo("预览成功!");
                    recordStatus = 3;
                    //DebugInfo("NET_DVR_RealPlay_V40 succ!");
                    btnPreview.Text = "关闭录像机";
                }
            }
            else
            {
                //停止预览 Stop live view 
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "停止预览失败,错误代码:" + iLastErr;
                    //str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    DebugInfo(str);
                    return;
                }

                if ((comboBoxView.SelectedIndex == 1) && (m_lPort >= 0))
                {
                    if (!PlayCtrl.PlayM4_Stop(m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_Stop failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    if (!PlayCtrl.PlayM4_CloseStream(m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_CloseStream failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    if (!PlayCtrl.PlayM4_FreePort(m_lPort))
                    {
                        iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_FreePort failed, error code= " + iLastErr;
                        DebugInfo(str);
                    }
                    m_lPort = -1;
                }
                DebugInfo("停止预览成功!");
                //DebugInfo("NET_DVR_StopRealPlay succ!");
                m_lRealHandle = -1;
                recordStatus = 2;
                btnPreview.Text = "启动录像机";
                RealPlayWnd.Invalidate();//刷新窗口 refresh the window
            }
            return;
        }

        // 表单开启录像成功后实时显示预览
        public static void RealTimePreview(MainWindow window)
        {
            window.RealPlayWnd.BringToFront();
        }

        private void btnBMP_Click_1(object sender, EventArgs e)
        {

        }

        private void btnJPEG_Click_1(object sender, EventArgs e)
        {
            int lChannel = iChannelNum[(int)iSelIndex]; //通道号 Channel number

            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 0xff-Auto(使用当前码流分辨率) 
            //抓图分辨率需要设备支持，更多取值请参考SDK文档

            //JPEG抓图保存成文件 Capture a JPEG picture
            string sJpegPicFileName;
            sJpegPicFileName = "img\\" + GetTimeStamp() + "_capture.jpg";//图片保存路径和文件名 the path and file name to save

            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(m_lUserID, lChannel, ref lpJpegPara, sJpegPicFileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
                DebugInfo(str);
                return;
            }
            else
            {
                str = "NET_DVR_CaptureJPEGPicture succ and the saved file is " + sJpegPicFileName;
                DebugInfo(str);
            }

            //JEPG抓图，数据保存在缓冲区中 Capture a JPEG picture and save in the buffer
            uint iBuffSize = 400000; //缓冲区大小需要不小于一张图片数据的大小 The buffer size should not be less than the picture size
            byte[] byJpegPicBuffer = new byte[iBuffSize];
            uint dwSizeReturned = 0;

            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture_NEW(m_lUserID, lChannel, ref lpJpegPara, byJpegPicBuffer, iBuffSize, ref dwSizeReturned))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CaptureJPEGPicture_NEW failed, error code= " + iLastErr;
                DebugInfo(str);
                return;
            }
            else
            {
                //将缓冲区里的JPEG图片数据写入文件 save the data into a file
                string str = "buffertest.jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwSizeReturned;
                fs.Write(byJpegPicBuffer, 0, iLen);
                fs.Close();

                str = "NET_DVR_CaptureJPEGPicture_NEW succ and save the data in buffer to 'buffertest.jpg'.";
                DebugInfo(str);
            }

            return;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
        }
        #region 录像窗口最大化事件

        private void RealPlayWnd_Click(object sender, EventArgs e)
        {
            //设置窗口点击
            if (isRecordClick)
            {
                if (isRealPicClick)
                {
                    CommonUtil.RealTimePreview();
                    int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
                    int iActulaHeight = Screen.PrimaryScreen.Bounds.Height;
                    this.Width = iActulaWidth;
                    this.Height = iActulaHeight;
                    this.Location = new Point(0, 0);
                    isRealPicClick = false;
                }
                else {
                    CommonUtil.RestoreSetting(false);
                    isRealPicClick =true;
                    return;
                }
                //isRecordClick = false;
            }
            else {
                //isRecordClick=true;
                int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
                int iActulaHeight = Screen.PrimaryScreen.Bounds.Height;
                if (RealPlayWnd.Tag.ToString().Contains("normal"))
                {
                    this.Width = iActulaWidth;
                    this.Height = iActulaHeight;
                    this.Location = new Point(0, 0);
                    RealPlayWnd.Tag = "max";
                }
                else
                {
                    RealPlayWnd.Tag = "normal";
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    this.Width = 150; this.Height = 150;
                    int x = iActulaWidth - 150;
                    int y = 55;
                    this.Location = new Point(x, y);
                }
            }
        } 
        #endregion
    }
}
