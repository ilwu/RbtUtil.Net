using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public virtual void writeToFile(string fileFullPath)
        {
            FileStream fos = null;

            try
            {
                // 檢路徑
                //(new PathUtil()).chkAndCreateDir((new System.IO.DirectoryInfo(fileFullPath)).Parent.FullName);

                // 建立檔案
                fos = new FileStream(fileFullPath, FileMode.Create);
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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // close
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// 將內容寫入檔案 (包含建立路徑功能) </summary>
        /// <param name="path"> 檔案路徑 </param>
        /// <param name="fileName"> 檔案名稱 </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void writeToFile(String path, String fileName) throws Exception
        public virtual void writeToFile(string path, string fileName)
        {
            FileStream fos = null;

            try
            {
                // 檢查並建立路徑
                this.chkAndCreateDir(path);

                // 建立檔案
                fos = new FileStream(path + Path.DirectorySeparatorChar + fileName, FileMode.Create);

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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // close
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                        fos.Dispose();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// 將傳入之文字，寫入檔案 (編碼請使用 setEncoding 設定) </summary>
        /// <param name="content"> </param>
        /// <param name="path"> </param>
        /// <param name="fileName"> </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void writeToFile(String content, String path, String fileName) throws Exception
        public virtual void writeToFile(string content, string path, string fileName)
        {
            this.writeToFile(encoding.GetBytes(content), path, fileName);
        }

        /// <summary>
        /// 將傳入之byte[]寫入檔案 (包含建立路徑功能) </summary>
        /// <param name="data"> data (byte[]) </param>
        /// <param name="path"> 檔案路徑 </param>
        /// <param name="fileName"> 檔案名稱 </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void writeToFile(byte[] data, String path, String fileName) throws Exception
        public virtual void writeToFile(byte[] data, string path, string fileName)
        {
            FileStream fos = null;

            try
            {
                // 檢查並建立路徑
                this.chkAndCreateDir(path);

                // 建立檔案
                fos = new FileStream(path + Path.DirectorySeparatorChar + fileName, FileMode.Create);

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
            finally
            {
                // close
                if (fos != null)
                {
                    try
                    {
                        fos.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
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
        // 其他工具
        // =========================================================================================
        public void chkAndCreateDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 擷取完整路徑中的檔案名稱 </summary>
        /// <param name="filePath"> 檔案完整路徑 </param>
        /// <returns> 檔案名稱 </returns>
        public virtual string getFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// 讀取檔案 </summary>
        /// <param name="filePath"> </param>
        /// <returns> 檔案內容 </returns>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String readFileContent(String filePath) throws java.io.IOException
        public virtual string readFileContent(string filePath)
        {
            StringBuilder sbBuilder = new StringBuilder();
            using (System.IO.StreamReader sr = System.IO.File.OpenText(filePath))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    sbBuilder.AppendLine(s);
                }
            }
            return sbBuilder.ToString();
        }

        /// <summary>
        /// 讀取檔案 </summary>
        /// <param name="filePath"> 完整檔案路徑 </param>
        /// <param name="fileEncodeing"> 檔案編碼 </param>
        /// <returns> 依行放入String[] </returns>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String[] readFileContent(String filePath, String fileEncodeing) throws java.io.IOException
        public virtual string[] readFileContent(string filePath, string fileEncodeing)
        {
            List<string> resultList = new List<string>();
            using (System.IO.StreamReader sr = System.IO.File.OpenText(filePath))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    resultList.Add(s);
                }
            }
            return resultList.ToArray();
        }

        /// <summary>
        /// 讀取檔案並回傳內容 </summary>
        /// <param name="filePath"> </param>
        /// <param name="fileEncodeing">
        /// @return </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String readFileText(String filePath, String fileEncodeing) throws java.io.IOException
        public virtual string readFileText(string filePath, string fileEncodeing)
        {
            string[] lines = this.readFileContent(filePath, fileEncodeing);
            StringBuilder sb = new StringBuilder();

            foreach (string str in lines)
            {
                sb.Append(str);
                sb.Append(this.BROKEN_LINE_SYMBOL_Renamed);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 檢查是否為合法檔名
        /// </summary>
        /// <param name="fullFileName">完整路徑+檔案名稱</param>
        /// <returns></returns>
        public virtual bool isAllowFullFileName(string fullFileName)
        {
            try
            {
                File.Create(fullFileName);
            }
            catch (Exception)
            {
                return false;
            }
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }
            return true;
        }
    }
}