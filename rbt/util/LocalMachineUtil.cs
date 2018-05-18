using System.Net;
using System.Net.Sockets;

namespace rbt.util
{
    public class LocalMachineUtil
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static string GetLocolIP()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] localIPs = ipEntry.AddressList;

            if (localIPs != null)
            {
                foreach (var addres in localIPs)
                {
                    if (addres.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var ip = addres.ToString();
                        if ("::1" == ip)
                        {
                            ip = "127.0.0.1";
                        }
                        return ip;
                    }
                }
            }
            return "";
        }
    }
}