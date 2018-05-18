using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace rbt.util
{
    /// <summary>
    ///
    /// </summary>
    public class NetworkUtil
    {
        /// <summary>
        /// 利用IPAddress屬性配合Ping進行遠端Server的確認。
        /// </summary>
        /// <returns></returns>
        public bool IsActivityByPing(string IPv4Address, int timeout = 100)
        {
            try
            {
                IPAddress tIP = IPAddress.Parse(IPv4Address);
                Ping tPingControl = new Ping();
                PingReply tReply = tPingControl.Send(tIP, timeout);
                tPingControl.Dispose();
                return tReply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 利用IPAddress與Port屬性配合TCPClient進行遠端Server的確認。
        /// </summary>
        /// <returns>true：存在；false：不存在</returns>
        public bool IsActivityByTcpIp(string IPv4Address, int Port, int timeout = 30)
        {
            try
            {
                IPEndPoint tIPEndPoint = new IPEndPoint(IPAddress.Parse(IPv4Address), Port);
                TcpClient tClient = new TcpClient();
                tClient.Connect(tIPEndPoint);
                bool tResult = tClient.Connected;
                tClient.Close();
                return tResult;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}