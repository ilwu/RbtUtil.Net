using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace rbt.util
{
    /// <summary>
    ///
    /// </summary>
    public class WinApi
    {
        #region Win32 Structs

        public enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        public enum SW
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }

        public enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

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

        public struct WTS_SESSION_INFO
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pWinStationName;

            public int SessionID;
            public WTS_CONNECTSTATE_CLASS state;
        }

        #endregion Win32 Structs

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

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

        [DllImport("kernel32.dll", SetLastError = true,
                    CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("advapi32.dll", SetLastError = true,
                    CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
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

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            Int32 dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            Int32 ImpersonationLevel,
            Int32 dwTokenType,
            ref IntPtr phNewToken);

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out WinApi.PerformanceInformation PerformanceInformation, [In] int Size);

        public static WTS_SESSION_INFO[] SessionEnumeration()
        {
            //Set handle of terminal server as the current terminal server
            IntPtr hServer = IntPtr.Zero;
            bool RetVal;
            IntPtr lpBuffer = IntPtr.Zero;
            int Count = 0;
            IntPtr p;
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
                    arrSessionInfo[i] = (WTS_SESSION_INFO)Marshal.PtrToStructure(p, Session_Info.GetType());
                    p += Marshal.SizeOf(Session_Info.GetType());
                }
                WTSFreeMemory(lpBuffer);
            }
            else
            {
                //Insert Error Reaction Here
            }
            return arrSessionInfo;
        }

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

        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            int Reserved,
            int Version,
            ref IntPtr ppSessionInfo,
            ref int pCount);

        //[DllImport("wtsapi32.dll", SetLastError = true)]
        //public static extern int WTSEnumerateSessions(
        //                System.IntPtr hServer,
        //                int Reserved,
        //                int Version,
        //                ref System.IntPtr ppSessionInfo,
        //                ref int pCount);
        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(System.IntPtr pMemory);

        /// <summary>
        /// 取得目前的 Session ID
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(
            Int32 sessionId,
            out IntPtr Token);

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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATAW
        {
            public System.IO.FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName,
                                                        FileSystemRights dwDesiredAccess,
                                                        FileShare dwShareMode,
                                                        IntPtr securityAttrs,
                                                        FileMode dwCreationDisposition,
                                                        FileOptions dwFlagsAndAttributes,
                                                        IntPtr hTemplateFile);
    }
}