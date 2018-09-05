using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
/// <summary>
/// Kurt Youyuan tech 2018.
/// </summary>

namespace YYOPInspectionClient
{
    class ClientSocket
    {
        public Socket commandSocket;   // socket for command
        public Socket dataSocket;      // socket for data
        public IPEndPoint readerCommandEndPoint;//一个IP地址和端口的绑定(命令传输使用)
        public IPEndPoint readerDataEndPoint;//一个IP地址和端口的绑定(数据传输使用)


        public ClientSocket(byte[] ipAddress, int readerCommandPort, int readerDataPort)
        { 
            IPAddress readerIpAddress = new IPAddress(ipAddress);
            readerCommandEndPoint = new IPEndPoint(readerIpAddress, readerCommandPort);
            readerDataEndPoint = new IPEndPoint(readerIpAddress, readerDataPort);
            commandSocket = null;
            dataSocket = null;
        }


        public ClientSocket(string ipAddress, int readerCommandPort, int readerDataPort)
        {
            IPAddress readerIpAddress = IPAddress.Parse(ipAddress);
            readerCommandEndPoint = new IPEndPoint(readerIpAddress, readerCommandPort);
            readerDataEndPoint = new IPEndPoint(readerIpAddress, readerDataPort);
            commandSocket = null;
            dataSocket = null;
        }
    }
}
