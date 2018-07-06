using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ZetaLongPaths;

namespace rbt.util
{
    public class FTPUtil
    {
        private Config config = new Config();

        public FTPUtil(string hostPath, string userID, string password)
        {
            config.HostPath = hostPath;
            config.UserID = userID;
            config.Password = password;
        }

        public FTPUtil(Config FtpConfig)
        {
            this.config = FtpConfig;
        }

        public class Config
        {
            public string HostPath { get; set; }
            public string UserID { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// 檔案刪除
        /// </summary>
        /// <param name="fileFullpath"></param>
        public void DeleteFile(string fileFullpath)
        {
            // =============================================================
            // ftpRequest
            // =============================================================
            var ftpRequest = GetFtpWebRequest(fileFullpath);
            ftpRequest.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequest.KeepAlive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            // =============================================================
            // commit
            // =============================================================
            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
            response.Close();
        }

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileFullpath"></param>
        /// <returns></returns>
        public Stream GetFile(string fileFullpath)
        {
            // =============================================================
            // ftpRequest
            // =============================================================
            var ftpRequest = GetFtpWebRequest(fileFullpath);
            ftpRequest.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequest.KeepAlive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            return ((FtpWebResponse)ftpRequest.GetResponse()).GetResponseStream();
        }

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public void DownLoadFileTo(string fileFullPath, string targetFileFullPath)
        {
            // =============================================================
            // ftpRequest
            // =============================================================
            var ftpRequest = GetFtpWebRequest(fileFullPath);
            ftpRequest.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequest.KeepAlive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (var readerStream = response.GetResponseStream())
                {
                    using (var fileStream = new ZlpFileInfo(targetFileFullPath).OpenCreate())
                    {
                        byte[] buffer = new byte[10240];
                        int read;
                        while ((read = readerStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public byte[] DownLoadFile(string fileFullPath)
        {
            // =============================================================
            // ftpRequest
            // =============================================================
            var ftpRequest = GetFtpWebRequest(fileFullPath);
            ftpRequest.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequest.KeepAlive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
            {
                try
                {
                    using (Stream readerStream = response.GetResponseStream())
                    {
                        using (MemoryStream msStream = new MemoryStream())
                        {
                            int bytesRead = 0;
                            byte[] buffer = new byte[2049];
                            while ((true))
                            {
                                bytesRead = readerStream.Read(buffer, 0, buffer.Length);
                                if ((bytesRead == 0))
                                    break;
                                msStream.Write(buffer, 0, bytesRead);
                            }
                            return msStream.ToArray();
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("檔案下載失敗:" + response.StatusDescription);
                }
            }
        }

        /// <summary>
        ///     ''' 檔案上傳
        ///     ''' </summary>
        ///     ''' <param name="fileStream"></param>
        ///     ''' <param name="contentLength"></param>
        ///     ''' <param name="ftpSubPath"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public void UploadFile(
            Stream fileStream,
            string fileName
            )
        {
            // =============================================================
            // 檢查並產生目錄
            // =============================================================
            //CheckAndCreateDir(config.HostPath);

            // =============================================================
            // ftpRequest
            // =============================================================
            //FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + "/" + fileName));
            var ftpRequest = GetFtpWebRequest(fileName);
            ftpRequest.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequest.KeepAlive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.ContentLength = fileStream.Length;

            // =============================================================
            // 檔案讀取並上傳
            // =============================================================
            using (Stream ftpStream = ftpRequest.GetRequestStream())
            {
                const int buffLength = 2048;
                byte[] buff = new byte[2048];
                int contentLen = fileStream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    ftpStream.Write(buff, 0, contentLen);
                    contentLen = fileStream.Read(buff, 0, buffLength);
                }
            }

            // =============================================================
            // commit
            // =============================================================
            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
            response.Close();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="currfilePath"></param>
        /// <returns></returns>
        private FtpWebRequest GetFtpWebRequest(string currfilePath)
        {
            var uri = GenDirPath(currfilePath);
            if (uri.Scheme.ToLower() == "ftps")
            {
                //uri = new Uri("ftp://" + uri.Host + ":" + uri.Port + "/" + uri.LocalPath);
                WebRequest.RegisterPrefix("ftps", new FtpsWebRequestCreator());
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;
                //uri.Scheme = "ftps";
            }

            //init
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
            //判斷 ftps協定
            if (this.config.HostPath.Trim().ToLower().StartsWith("ftps://"))
            {
                ftpRequest.EnableSsl = true;
            }
            return ftpRequest;
        }

        //增加這個方法 add this method
        public bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private sealed class FtpsWebRequestCreator : IWebRequestCreate
        {
            public WebRequest Create(Uri uri)
            {
                FtpWebRequest webRequest = (FtpWebRequest)WebRequest.Create(uri.AbsoluteUri.Remove(3, 1)); // Removes the "s" in "ftps://".
                webRequest.EnableSsl = true;
                return webRequest;
            }
        }

        /// <summary>
        ///     '''
        ///     ''' </summary>
        ///     ''' <param name="fileUploadID"></param>
        ///     ''' <param name="subPath"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private Uri GenDirPath(string currfilePath)
        {
            // =============================================================
            // 兜組檔案路徑與名稱
            // =============================================================
            string serverPath = this.config.HostPath.Trim();
            if (!serverPath.EndsWith("/"))
            {
                serverPath += "/";
            }

            var uri = new Uri(serverPath + currfilePath);

            if (uri.Scheme != Uri.UriSchemeFtp && uri.Scheme != Uri.UriSchemeFtp + "s")
            {
                throw new Exception("傳入連結錯誤,並非 FTP 連結![" + uri.AbsoluteUri + "]");
            }

            return uri;
        }

        private void CheckAndCreateDir(string ftpFullPath)
        {
            // 去除空白行
            ftpFullPath = ftpFullPath.Trim();
            // 若前面沒有 ftp 則加上
            if (!ftpFullPath.ToLower().StartsWith("ftp://"))
                ftpFullPath = "ftp://" + ftpFullPath;
            // 在最後面加上 目錄字元
            if (!ftpFullPath.EndsWith("/"))
                ftpFullPath = ftpFullPath + "/";

            // 計算有幾層目錄 ( 若只有3個 '3' 字元時, 代表為 ftp 根目錄 'ftp://xxxx/' )
            int dirCount = StringUtil.CountString(ftpFullPath, "/");
            if (dirCount <= 3)
                return;

            // 檢查並建立目錄
            MarkDir(ftpFullPath, dirCount);
        }

        private void MarkDir(string ftpFullPath, int level)
        {
            if (level <= 3)
                return;

            string currFtpFullPath = StringUtil.GetPrefixIndexOfChar(ftpFullPath, System.Convert.ToChar("/"), level);
            if (!MakeFtpDir(currFtpFullPath))
            {
                MarkDir(ftpFullPath, level - 1);
                MakeFtpDir(currFtpFullPath);
            }
        }

        private bool MakeFtpDir(string currFtpFullPath)
        {
            FtpWebRequest ftpRequestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(currFtpFullPath));
            ftpRequestDir.Credentials = new NetworkCredential(this.config.UserID, this.config.Password);
            ftpRequestDir.Method = WebRequestMethods.Ftp.MakeDirectory;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)ftpRequestDir.GetResponse())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}