using Microsoft.Win32.SafeHandles;
using rbt.Extension;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using ZetaLongPaths;

namespace rbt.util
{
    public class FileUtil
    {
        /// <summary>
        /// StringBuffer
        /// </summary>
        private StringBuilder stringBuffer;

        /// <summary>
        /// 斷行符號 (windows 是 "\r\n" Unix 是 "\n")
        /// </summary>
        private string BROKEN_LINE_SYMBOL_Renamed = "\n";

        /// <summary>
        /// 寫出檔案是否為 是否寫入UTF8 BOM 使用
        /// </summary>
        private bool useUTF8BOM = true;

        /// <summary>
        /// 輸出檔案編碼
        /// </summary>
        private System.Text.Encoding myencoding = null;

        /// <summary>
        /// 建構子
        /// </summary>
        public FileUtil()
        {
            this.stringBuffer = new StringBuilder();
            encoding = System.Text.Encoding.UTF8;
        }

        // =========================================================================================
        // 檔案寫入
        // =========================================================================================

        /// <summary>
        /// 將內容寫入檔案 </summary>
        /// <param name="fileFullPath"> 完整路徑 + 檔案名稱 </param>
        /// <exception cref="Exception"> </exception>
        public virtual void WriteBufferToFile(string fileFullPath)
        {
            // 檢核路徑
            var directory = ZlpPathHelper.GetDirectoryPathNameFromFilePath(fileFullPath);
            DirectoryVerifyExistsAndCreate(directory);

            using (var fos = new ZlpFileInfo(fileFullPath).OpenCreate())
            {
                // 印出訊息
                Console.WriteLine("寫入 " + fileFullPath);

                var bb = encoding.GetBytes(this.stringBuffer.ToString());

                if (this.useUTF8BOM)
                {
                    // 寫入 UTF8 HEADER
                    fos.WriteByte(0xEF);
                    fos.WriteByte(0xBB);
                    fos.WriteByte(0xBF);

                    // 寫入檔案
                    fos.Write(bb, 0, bb.Length);
                }
                else
                {
                    // 寫入檔案
                    fos.Write(bb, 0, bb.Length);
                }
            }
        }

        /// <summary>
        /// 將傳入之文字，寫入檔案 (編碼請使用 setEncoding 設定) </summary>
        /// <param name="content"> </param>
        /// <param name="path"> </param>
        /// <param name="fileName"> </param>
        /// <exception cref="Exception"> </exception>
        public void WriteToFile(string content, string fileFullPath)
        {
            this.WriteToFile(encoding.GetBytes(content), fileFullPath);
        }

        /// <summary>
        /// 將傳入之byte[]寫入檔案 (包含建立路徑功能) </summary>
        /// <param name="data"> data (byte[]) </param>
        /// <param name="path"> 檔案路徑 </param>
        /// <param name="fileName"> 檔案名稱 </param>
        /// <exception cref="Exception"> </exception>
        public void WriteToFile(byte[] data, string fileFullPath)
        {
            // 檢核路徑
            var directory = ZlpPathHelper.GetDirectoryPathNameFromFilePath(fileFullPath);
            DirectoryVerifyExistsAndCreate(directory);

            using (var fos = new ZlpFileInfo(fileFullPath).OpenCreate())
            {
                // 印出訊息
                Console.WriteLine("寫入 " + fileFullPath);

                var bb = encoding.GetBytes(this.stringBuffer.ToString());

                if (this.useUTF8BOM)
                {
                    // 寫入 UTF8 HEADER

                    fos.WriteByte(0xEF);
                    fos.WriteByte(0xBB);
                    fos.WriteByte(0xBF);

                    // 寫入檔案
                    fos.Write(data, 0, data.Length);
                }
                else
                {
                    // 寫入檔案
                    fos.Write(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// 取得輸出檔案編碼 </summary>
        /// <returns> encoding </returns>
        public virtual Encoding encoding
        {
            get
            {
                return myencoding;
            }
            set
            {
                myencoding = value;
            }
        }

        /// <summary>
        /// 設定輸出檔案的斷行符號，預設為『\n』(windows 為 \r\n) </summary>
        /// <param name="bROKENLINESYMBOL"> BROKEN_LINE_SYMBOL </param>
        public virtual string BROKEN_LINE_SYMBOL
        {
            set
            {
                this.BROKEN_LINE_SYMBOL_Renamed = value;
            }
        }

        /// <summary>
        /// 設定輸出檔案是否為有BOM的UTF8檔案 </summary>
        /// <param name="useUTF8BOM"> useUTF8BOM </param>
        public virtual bool UseUTF8BOM
        {
            set
            {
                if (value)
                {
                    Console.WriteLine("目前輸出檔案設定編碼為:" + this.encoding + ",設定此項目為true後，編碼設定將不生效");
                }
                this.useUTF8BOM = value;
            }
        }

        // =========================================================================================
        // 讀取檔案
        // =========================================================================================
        /// <summary>
        /// 讀取檔案 </summary>
        /// <param name="filePath"> </param>
        /// <returns> 檔案內容 </returns>
        /// <exception cref="IOException"> </exception>
        public string ReadFileContent(string filePath, Encoding encoding = null)
        {
            var fileInfo = new ZlpFileInfo(filePath);
            if (encoding == null)
            {
                return fileInfo.ReadAllText();
            }
            else
            {
                return fileInfo.ReadAllText(encoding);
            }
        }

        /// <summary>
        /// 讀取檔案 </summary>
        /// <param name="filePath"> 完整檔案路徑 </param>
        /// <param name="fileEncodeing"> 檔案編碼 </param>
        /// <returns> 依行放入String[] </returns>
        /// <exception cref="IOException"> </exception>
        public string[] ReadFileAllLines(string filePath, Encoding encoding = null)
        {
            var fileInfo = new ZlpFileInfo(filePath);
            if (encoding == null)
            {
                return fileInfo.ReadAllLines();
            }
            else
            {
                return fileInfo.ReadAllLines(encoding);
            }
        }

        // =========================================================================================
        // 其他工具
        // =========================================================================================
        /// <summary>
        /// 取得合法檔名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string MakeFilenameValid(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();

            if (filename.EndsWith("."))
                filename = Regex.Replace(filename, @"\.+$", "");

            if (filename.Length == 0)
                throw new ArgumentException();

            if (filename.Length > 245)
                throw new System.IO.PathTooLongException();

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            return filename;
        }

        /// <summary>
        /// 取得合法路徑名
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public static string MakeFoldernameValid(string foldername)
        {
            if (foldername == null)
                throw new ArgumentNullException();

            if (foldername.EndsWith("."))
                foldername = Regex.Replace(foldername, @"\.+$", "");

            if (foldername.Length == 0)
                throw new ArgumentException();

            if (foldername.Length > 245)
                throw new System.IO.PathTooLongException();

            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                foldername = foldername.Replace(c, '_');
            }

            return foldername;
        }

        /// <summary>
        /// 掃描檔案列表 (含子目錄)
        /// </summary>
        /// <param name="targetDirectory">標的目錄</param>
        /// <param name="extensionFilters">副檔名過濾</param>
        /// <param name="realtimeActionWhenFind">發現檔案時, 即時執行項目 (避免掃描回傳時間過久, 可以在發現檔案的第一時間執行)</param>
        /// <returns></returns>
        public IList<string> ScanDirectoryFile(
            string targetDirectory,
            string[] extensionFilters = null,
            Action<string> realtimeActionWhenFind = null,
            Action<string> whenEnterNewDir = null)
        {
            //將副檔名都轉小寫, 以免比對錯誤
            for (int i = 0; extensionFilters != null && i < extensionFilters.Length; i++)
            {
                extensionFilters[i] = extensionFilters[i].ToLower();
            }

            //Scan
            return RecursiveScan(targetDirectory, extensionFilters, realtimeActionWhenFind, whenEnterNewDir);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="targetDirectory">標的目錄</param>
        /// <param name="extensionFilters">副檔名過濾</param>
        /// <param name="realtimeActionWhenFind">發現檔案時, 即時執行項目 (避免掃描回傳時間過久, 可以在發現檔案的第一時間執行)</param>
        /// <returns></returns>
        private IList<string> RecursiveScan(
            string targetDirectory,
            string[] extensionFilters = null,
            Action<string> realtimeActionWhenFind = null,
            Action<string> whenEnterNewDir = null)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WinApi.WIN32_FIND_DATAW findData;
            IntPtr findHandle = INVALID_HANDLE_VALUE;

            //檢查目錄前, 通常用於外部呼叫者定義顯示掃目錄
            //於console 中顯示掃描目錄
            if (whenEnterNewDir != null)
            {
                whenEnterNewDir(targetDirectory);
            }

            var info = new List<string>();
            try
            {
                findHandle = WinApi.FindFirstFileW(targetDirectory + @"\*", out findData);
                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    do
                    {
                        if (findData.cFileName == "." || findData.cFileName == "..") continue;

                        string fullpath = targetDirectory + (targetDirectory.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                        {
                            //為目錄時遞迴
                            info.AddRange(RecursiveScan(fullpath, extensionFilters, realtimeActionWhenFind, whenEnterNewDir));
                        }
                        else
                        {
                            var extension = ZlpPathHelper.GetExtension(fullpath);

                            if (".xdormdecodebackup" == extension)
                            {
                                var a = 1;
                            }

                            //副檔名過濾 (前一個方法已全部轉小寫, 故此處以小寫比對)
                            if (extensionFilters != null &&
                                ZlpPathHelper.GetExtension(fullpath).ToLower().NotIn(extensionFilters))
                            {
                                continue;
                            }
                            //取得完整路徑 (使用 ZlpPathHelper 避免路徑過長)
                            var fileFullPath = ZlpPathHelper.GetFullPath(fullpath);
                            //執行即時Action (exception 外層自己控制)
                            if (realtimeActionWhenFind != null)
                            {
                                realtimeActionWhenFind(fileFullPath);
                            }

                            info.Add(fileFullPath);
                        }
                    }
                    while (WinApi.FindNextFile(findHandle, out findData));
                }
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) WinApi.FindClose(findHandle);
            }
            return info;
        }

        /// <summary>
        /// 檔案是否使用中
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = false;

            SafeFileHandle fileHandle =
            WinApi.CreateFile(fileName, FileSystemRights.Modify,
                  System.IO.FileShare.Write, IntPtr.Zero,
                  System.IO.FileMode.OpenOrCreate, System.IO.FileOptions.None, IntPtr.Zero);

            if (fileHandle.IsInvalid)
            {
                //private const int ERROR_SHARING_VIOLATION = 32;
                if (Marshal.GetLastWin32Error() == 32)
                {
                    inUse = true;
                }
            }
            fileHandle.Close();
            return inUse;
        }

        /// <summary>
        /// 確認目錄存在, 不存在時新增
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryVerifyExistsAndCreate(string path)
        {
            if (!ZlpIOHelper.DirectoryExists(path))
            {
                ZlpIOHelper.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 強制刪除檔案 (排除因檔案唯讀刪除失敗問題)
        /// </summary>
        /// <param name="path"></param>
        public static void FileForceDelete(string path)
        {
            if (ZlpIOHelper.FileExists(path))
            {
                var fileinfo = new ZlpFileInfo(path);
                fileinfo.Attributes = ZetaLongPaths.Native.FileAttributes.Normal;
                fileinfo.Delete();
            };
        }
    }
}