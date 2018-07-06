using log4net;
using rbt.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace rbt.util
{
    /// <summary>
    ///
    /// </summary>
    public class LocalMachineUtil
    {
        private readonly static ILog LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static string GetExecutablePathByID(int processID)
        {
            ObjectQuery sq = new ObjectQuery
                ("Select * from Win32_Process Where ProcessID = '" + processID + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);

            foreach (System.Management.ManagementObject p in searcher.Get())
            {
                return p["ExecutablePath"].SafeTrim();
            }
            return "";
        }

        /// <summary>
        /// 於 windows service 中, 無法透由正規方法取到目前登入 user ID 時, 利用此方法為替代方案
        /// 20180706 因多使用者同時登入 (切換使用者, 未登出)時, 會產生多個 explorer, 造成誤判, 故停用
        /// </summary>
        /// <returns></returns>
        //public static string GetCurrentUserIDInService()
        //{
        //    return GetProcessOwner("explorer");
        //}

        /// <summary>
        /// 取得處理程序的使用者名稱
        /// 1.未找到時回傳 null
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetProcessOwnerByProcessName(string processName)
        {
            //GetProcessesByName
            var psList = Process.GetProcessesByName(processName);
            //未找到時回傳 空字串
            if (psList == null || psList.Length == 0) return null;
            //取得 owner
            var ownerList = new List<string>();
            foreach (var proc in psList)
            {
                var owner = GetProcessOwnerByID(proc.Id, processName);
                if (!ownerList.Contains(owner))
                {
                    ownerList.Add(owner);
                }
            }

            return ownerList;
        }

        /// <summary>
        /// 以PID 取得 處理程序的使用者名稱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetProcessOwnerByID(int id, string executableFileName)
        {
            ObjectQuery sq = new ObjectQuery
                ("Select * from Win32_Process Where ProcessID = '" + id + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);

            foreach (System.Management.ManagementObject p in searcher.Get())
            {
                var ExecutablePath = p["ExecutablePath"].SafeTrim();

                if (ExecutablePath.NotEmpty() &&
                    System.IO.Path.GetFileNameWithoutExtension(ExecutablePath).ToLower() == executableFileName.ToLower())
                {
                    string[] OwnerInfo = new string[2];
                    p.InvokeMethod("GetOwner", (object[])OwnerInfo);
                    return OwnerInfo[0];
                }
            }
            return "";
        }

        #region GetLocolIP

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
                        if ("::1".Equals(ip))
                        {
                            ip = "127.0.0.1";
                        }
                        return ip;
                    }
                }
            }
            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetLocolIPs()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] localIPs = ipEntry.AddressList;

            var ipList = new List<string>();

            if (localIPs != null)
            {
                foreach (var addres in localIPs)
                {
                    if (addres.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var ip = addres.ToString();
                        if ("::1".Equals(ip))
                        {
                            ip = "127.0.0.1";
                        }
                        ipList.Add(ip);
                    }
                }
            }
            return ipList;
        }

        #endregion GetLocolIP

        #region 記憶體相關

        /// <summary>
        /// 取得剩餘的記憶體(未使用) Available Physical Memory
        /// </summary>
        /// <returns></returns>
        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            WinApi.PerformanceInformation pi = new WinApi.PerformanceInformation();
            if (WinApi.GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 取得總記憶體容量
        /// </summary>
        /// <returns></returns>
        public static Int64 GetTotalMemoryInMiB()
        {
            var pi = new WinApi.PerformanceInformation();
            if (WinApi.GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 取得目前記憶體用量 (全系統)
        /// </summary>
        /// <returns></returns>
        public static decimal GetTotalMemoryUsePercent()
        {
            Int64 phav = GetPhysicalAvailableMemoryInMiB();
            Int64 tot = GetTotalMemoryInMiB();
            decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
            decimal percentOccupied = 100 - percentFree;

            return percentOccupied;
        }

        #endregion 記憶體相關

        #region 啟動程式 StartProcess

        #region ProcessExtensions

        private class ProcessExtensions
        {
            #region Win32 Constants

            public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
            public const int CREATE_NO_WINDOW = 0x08000000;

            public const int CREATE_NEW_CONSOLE = 0x00000010;

            public const uint INVALID_SESSION_ID = 0xFFFFFFFF;
            public static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

            #endregion Win32 Constants

            #region DllImports

            [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern bool CreateProcessAsUser(
                IntPtr hToken,
                String lpApplicationName,
                String lpCommandLine,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool bInheritHandle,
                uint dwCreationFlags,
                IntPtr lpEnvironment,
                String lpCurrentDirectory,
                ref STARTUPINFO lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);

            [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
            public static extern bool DuplicateTokenEx(
                IntPtr ExistingTokenHandle,
                uint dwDesiredAccess,
                IntPtr lpThreadAttributes,
                int TokenType,
                int ImpersonationLevel,
                ref IntPtr DuplicateTokenHandle);

            [DllImport("userenv.dll", SetLastError = true)]
            public static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

            [DllImport("userenv.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hSnapshot);

            [DllImport("kernel32.dll")]
            public static extern uint WTSGetActiveConsoleSessionId();

            [DllImport("Wtsapi32.dll")]
            private static extern uint WTSQueryUserToken(uint SessionId, ref IntPtr phToken);

            [DllImport("wtsapi32.dll", SetLastError = true)]
            public static extern int WTSEnumerateSessions(
                IntPtr hServer,
                int Reserved,
                int Version,
                ref IntPtr ppSessionInfo,
                ref int pCount);

            #endregion DllImports

            #region Win32 Structs

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }

            public enum SECURITY_IMPERSONATION_LEVEL
            {
                SecurityAnonymous = 0,
                SecurityIdentification = 1,
                SecurityImpersonation = 2,
                SecurityDelegation = 3,
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct STARTUPINFO
            {
                public int cb;
                public String lpReserved;
                public String lpDesktop;
                public String lpTitle;
                public uint dwX;
                public uint dwY;
                public uint dwXSize;
                public uint dwYSize;
                public uint dwXCountChars;
                public uint dwYCountChars;
                public uint dwFillAttribute;
                public uint dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            public enum TOKEN_TYPE
            {
                TokenPrimary = 1,
                TokenImpersonation = 2
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WTS_SESSION_INFO
            {
                public readonly UInt32 SessionID;

                [MarshalAs(UnmanagedType.LPStr)]
                public readonly String pWinStationName;

                public readonly WinApi.WTS_CONNECTSTATE_CLASS State;
            }

            #endregion Win32 Structs

            // Gets the user token from the currently active session
            public static bool GetSessionUserToken(ref IntPtr phUserToken, ref int errorCode)
            {
                var bResult = false;
                var hImpersonationToken = IntPtr.Zero;
                var activeSessionId = INVALID_SESSION_ID;
                var pSessionInfo = IntPtr.Zero;
                var sessionCount = 0;

                // Get a handle to the user access token for the current active session.
                if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
                {
                    var arrayElementSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                    var current = pSessionInfo;

                    for (var i = 0; i < sessionCount; i++)
                    {
                        var si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                        current += arrayElementSize;

                        if (si.State == WinApi.WTS_CONNECTSTATE_CLASS.WTSActive)
                        {
                            activeSessionId = si.SessionID;
                        }
                    }
                }

                // If enumerating did not work, fall back to the old method
                if (activeSessionId == INVALID_SESSION_ID)
                {
                    activeSessionId = WTSGetActiveConsoleSessionId();
                }

                if (WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
                {
                    // Convert the impersonation token to a primary token
                    bResult = DuplicateTokenEx(hImpersonationToken, 0, IntPtr.Zero,
                        (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, (int)TOKEN_TYPE.TokenPrimary,
                        ref phUserToken);

                    CloseHandle(hImpersonationToken);

                    if (!bResult)
                    {
                        errorCode = Marshal.GetLastWin32Error();
                    }
                }

                return bResult;
            }
        }

        #endregion ProcessExtensions

        /// <summary>
        /// 以呼叫者的身份執行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <param name="IsSilenceMode"></param>
        /// <returns></returns>
        public static string StartProcessAsLoader(string app, string path, bool IsSilenceMode = true)
        {
            bool result;
            IntPtr hToken = WindowsIdentity.GetCurrent().Token;
            IntPtr hDupedToken = IntPtr.Zero;

            var pi = new WinApi.PROCESS_INFORMATION();
            var sa = new WinApi.SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            var si = new WinApi.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            int dwSessionID = WinApi.WTSGetActiveConsoleSessionId();
            LOG.Debug("session id:" + dwSessionID);
            result = WinApi.WTSQueryUserToken(dwSessionID, out hToken);

            if (!result)
            {
                var error = "WTSQueryUserToken failed :" + Marshal.GetLastWin32Error();

                LOG.Error(error);
                if (!IsSilenceMode)
                {
                    WinApi.ShowMessageBox(error, "CreateProcess Error");
                }
            }

            var GENERIC_ALL_ACCESS = 0x10000000;

            result = WinApi.DuplicateTokenEx(
                  hToken,
                  GENERIC_ALL_ACCESS,
                  ref sa,
                  (int)WinApi.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                  (int)WinApi.TOKEN_TYPE.TokenPrimary,
                  ref hDupedToken
               );

            if (!result)
            {
                var error = "DuplicateTokenEx failed :" + Marshal.GetLastWin32Error();

                LOG.Error(error);
                if (!IsSilenceMode)
                {
                    WinApi.ShowMessageBox(error, "CreateProcess Error");
                }
            }

            IntPtr lpEnvironment = IntPtr.Zero;
            result = WinApi.CreateEnvironmentBlock(out lpEnvironment, hDupedToken, false);

            if (!result)
            {
                var error = "CreateEnvironmentBlock failed :" + Marshal.GetLastWin32Error();

                LOG.Error(error);
                if (!IsSilenceMode)
                {
                    WinApi.ShowMessageBox(error, "CreateProcess Error");
                }
            }

            result = WinApi.CreateProcessAsUser(
                                 hDupedToken,
                                 app,
                                 String.Empty,
                                 ref sa, ref sa,
                                 false, 0, IntPtr.Zero,
                                 path, ref si, ref pi);

            var resultMessage = "";

            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                resultMessage = (error == 2) ?
                "檔案名稱錯誤 (需用完整路徑名) CreateProcessAsUser : 2" :
                String.Format("CreateProcessAsUser Error: {0}", error);

                //ShowMessageBox("檔案名稱錯誤 (需用完整路徑名)", "AlertService Message");
            }

            if (pi.hProcess != IntPtr.Zero)
                WinApi.CloseHandle(pi.hProcess);
            if (pi.hThread != IntPtr.Zero)
                WinApi.CloseHandle(pi.hThread);
            if (hDupedToken != IntPtr.Zero)
                WinApi.CloseHandle(hDupedToken);

            return resultMessage;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            var hUserToken = IntPtr.Zero;
            var startInfo = new ProcessExtensions.STARTUPINFO();
            var procInfo = new ProcessExtensions.PROCESS_INFORMATION();
            var pEnv = IntPtr.Zero;

            startInfo.cb = Marshal.SizeOf(typeof(ProcessExtensions.STARTUPINFO));

            try
            {
                int errorCode = 0;
                return ProcessExtensions.GetSessionUserToken(ref hUserToken, ref errorCode);
            }
            finally
            {
                ProcessExtensions.CloseHandle(hUserToken);
                if (pEnv != IntPtr.Zero)
                {
                    ProcessExtensions.DestroyEnvironmentBlock(pEnv);
                }
                ProcessExtensions.CloseHandle(procInfo.hThread);
                ProcessExtensions.CloseHandle(procInfo.hProcess);
            }
        }

        /// <summary>
        /// 以登入者的權限啟動程式
        /// from https://github.com/murrayju/CreateProcessAsUser
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="cmdLine"></param>
        /// <param name="workDir"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null, bool visible = true)
        {
            var hUserToken = IntPtr.Zero;
            var startInfo = new ProcessExtensions.STARTUPINFO();
            var procInfo = new ProcessExtensions.PROCESS_INFORMATION();
            var pEnv = IntPtr.Zero;
            int iResultOfCreateProcessAsUser;

            startInfo.cb = Marshal.SizeOf(typeof(ProcessExtensions.STARTUPINFO));

            try
            {
                int errorCode = 0;
                if (!ProcessExtensions.GetSessionUserToken(ref hUserToken, ref errorCode))
                {
                    LOG.Error("StartProcessAsCurrentUser: GetSessionUserToken failed. errorCode:[" + errorCode + "]");
                }

                uint dwCreationFlags = ProcessExtensions.CREATE_UNICODE_ENVIRONMENT | (uint)(visible ? ProcessExtensions.CREATE_NEW_CONSOLE : ProcessExtensions.CREATE_NO_WINDOW);
                startInfo.wShowWindow = (short)(visible ? WinApi.SW.SW_SHOW : WinApi.SW.SW_HIDE);
                startInfo.lpDesktop = "winsta0\\default";

                if (!ProcessExtensions.CreateEnvironmentBlock(ref pEnv, hUserToken, false))
                {
                    throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");
                }

                if (!ProcessExtensions.CreateProcessAsUser(hUserToken,
                    appPath, // Application Name
                    cmdLine, // Command Line
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    dwCreationFlags,
                    pEnv,
                    workDir, // Working directory
                    ref startInfo,
                    out procInfo))
                {
                    iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
                    throw new Exception("StartProcessAsCurrentUser: CreateProcessAsUser failed.  Error Code -" + iResultOfCreateProcessAsUser);
                }

                iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
            }
            finally
            {
                ProcessExtensions.CloseHandle(hUserToken);
                if (pEnv != IntPtr.Zero)
                {
                    ProcessExtensions.DestroyEnvironmentBlock(pEnv);
                }
                ProcessExtensions.CloseHandle(procInfo.hThread);
                ProcessExtensions.CloseHandle(procInfo.hProcess);
            }

            return true;
        }

        #endregion 啟動程式 StartProcess
    }
}