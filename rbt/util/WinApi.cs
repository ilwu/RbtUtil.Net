using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace rbt.util
{
    /// <summary>
    ///
    /// </summary>
    public class WinApi
    {
        /// <summary>
        /// 取得目前的 Session ID
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        /// <summary>
        /// 顯示對話框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public static void ShowMessageBox(string message, string title)
        {
            int resp = 0;
            WinApi.WTSSendMessage(
                WTS_CURRENT_SERVER_HANDLE,
                WinApi.WTSGetActiveConsoleSessionId(),
                title, title.Length,
                message, message.Length,
                0, 0, out resp, false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="hServer"></param>
        /// <param name="SessionId"></param>
        /// <param name="pTitle"></param>
        /// <param name="TitleLength"></param>
        /// <param name="pMessage"></param>
        /// <param name="MessageLength"></param>
        /// <param name="Style"></param>
        /// <param name="Timeout"></param>
        /// <param name="pResponse"></param>
        /// <param name="bWait"></param>
        /// <returns></returns>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSSendMessage(
            IntPtr hServer,
            int SessionId,
            String pTitle,
            int TitleLength,
            String pMessage,
            int MessageLength,
            int Style,
            int Timeout,
            out int pResponse,
            bool bWait);

        [DllImport("kernel32.dll", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseHandle(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessID;
            public Int32 dwThreadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public Int32 Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [DllImport("advapi32.dll", SetLastError = true,
            CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandle,
            Int32 dwCreationFlags,
            IntPtr lpEnvrionment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            Int32 dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            Int32 ImpersonationLevel,
            Int32 dwTokenType,
            ref IntPtr phNewToken);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(
            Int32 sessionId,
            out IntPtr Token);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        //[DllImport("wtsapi32.dll", SetLastError = true)]
        //public static extern int WTSEnumerateSessions(
        //                System.IntPtr hServer,
        //                int Reserved,
        //                int Version,
        //                ref System.IntPtr ppSessionInfo,
        //                ref int pCount);

        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WTSEnumerateSessions(
            int hServer,
            int Reserved,
            int Version,
            ref long ppSessionInfo,
            ref int pCount);

        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(System.IntPtr pMemory);

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit,
        }

        public struct WTS_SESSION_INFO
        {
            public int SessionID;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pWinStationName;

            public WTS_CONNECTSTATE_CLASS state;
        }

        public static WTS_SESSION_INFO[] SessionEnumeration()
        {
            //Set handle of terminal server as the current terminal server
            int hServer = 0;
            bool RetVal;
            long lpBuffer = 0;
            int Count = 0;
            long p;
            WTS_SESSION_INFO Session_Info = new WTS_SESSION_INFO();
            WTS_SESSION_INFO[] arrSessionInfo;
            RetVal = WTSEnumerateSessions(hServer, 0, 1, ref lpBuffer, ref Count);
            arrSessionInfo = new WTS_SESSION_INFO[0];
            if (RetVal)
            {
                arrSessionInfo = new WTS_SESSION_INFO[Count];
                int i;
                p = lpBuffer;
                for (i = 0; i < Count; i++)
                {
                    arrSessionInfo[i] = (WTS_SESSION_INFO)Marshal.PtrToStructure(new IntPtr(p), Session_Info.GetType());
                    p += Marshal.SizeOf(Session_Info.GetType());
                }
                WTSFreeMemory(new IntPtr(lpBuffer));
            }
            else
            {
                //Insert Error Reaction Here
            }
            return arrSessionInfo;
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }
    }
}