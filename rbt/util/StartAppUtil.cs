using log4net;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace rbt.util
{
    /// <summary>
    ///
    /// </summary>
    public class StartAppUtil
    {
        /// <summary>
        /// LOG
        /// </summary>
        private static readonly ILog LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <param name="IsSilenceMode"></param>
        /// <returns></returns>
        public static string CreateProcess(string app, string path, bool IsSilenceMode = true)
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

            result = WinApi.DuplicateTokenEx(
                  hToken,
                  GENERIC_ALL_ACCESS,
                  ref sa,
                  (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                  (int)TOKEN_TYPE.TokenPrimary,
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

        private const int GENERIC_ALL_ACCESS = 0x10000000;

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        private enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }
    }
}