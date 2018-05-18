using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace rbt.util
{
    public class AesUtil
    {
        private string key = "a24b6aa825bd11e8b4670ed5f89f718b";
        private string iv = "2554863186342986";

        private byte[] keyData = null;
        private byte[] ivData = null;

        /// <summary>
        /// 用來產生符合 key 及 iv 正確度的 salt 定義
        /// </summary>
        private byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xF1 };

        /// <summary>
        /// 以預設的 key 起始 Aes
        /// </summary>
        public AesUtil()
        {
            HashKey();
        }

        /// <summary>
        /// 以指定的 key 字串, 起始 Aes
        /// </summary>
        /// <param name="key"></param>
        public AesUtil(string key)
        {
            if (key != null)
            {
                this.key = key;
            }
            HashKey();
        }

        /// <summary>
        /// 對 key 及 iv 進行 Salt 亂數處理，以符合 128 bits (16 bytes) 的規則
        /// </summary>
        private void HashKey()
        {
            if (this.key == null || this.key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (this.iv == null || this.iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(this.key);
            this.keyData = (new Rfc2898DeriveBytes(keyBytes, salt, 16)).GetBytes(16);

            byte[] ivBytes = Encoding.UTF8.GetBytes(this.iv);
            this.ivData = (new Rfc2898DeriveBytes(ivBytes, salt, 16)).GetBytes(16);
        }

        /// <summary>
        /// 將傳入的明文字串進行加密後, 再轉成 Base64 格式字串回傳
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = this.keyData;
                aesAlg.IV = this.ivData;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return Hex String of the encrypted bytes from the memory stream.
            return BitConverter.ToString(encrypted).Replace("-", "");
        }

        /// <summary>
        /// 將傳入經過 AesTk 加密的字串, 解密為明碼字串後回傳
        /// </summary>
        /// <param name="cipherText">Base64格式的加密字串</param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            byte[] cipherData = ConvertHexStringToByteArray(cipherText);

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = this.keyData;
                aesAlg.IV = this.ivData;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherData))
                {
                    try
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    catch (CryptographicException ex)
                    {
                        throw new CryptographicException("Decryption 失敗: " + ex.Message);
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// 將沒有分隔字元的連續 Hex String 轉成 byte[] 回傳
        /// </summary>
        /// <param name="hexString">例如: 1A3EB2C5</param>
        /// <returns></returns>
        private byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Hex String的位元個數須為偶數!");
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            try
            {
                for (int index = 0; index < HexAsBytes.Length; index++)
                {
                    string byteValue = hexString.Substring(index * 2, 2);
                    HexAsBytes[index] = Byte.Parse(byteValue, System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConvertHexStringToByteArray: " + ex.ToString());
                throw new FormatException("不是有效的 Hex String!");
            }

            return HexAsBytes;
        }
    }
}