using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace rbt.util
{
    /// <summary>
    ///     字串處理工具
    ///     @author Allen Wu
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        ///     計算關鍵字在字串中出現的次數
        /// </summary>
        /// <param name="str"> 比對字串 </param>
        /// <param name="keyWord"> 關鍵字元 </param>
        /// <returns> 次數 </returns>
        public static int CountString(string str, string keyWord)
        {
            int count = 0;

            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(keyWord))
            {
                return 0;
            }

            for (int i = 0; i <= str.Length - 1; i++)
            {
                int endindex = i + keyWord.Length;
                string word = "";

                // 剩餘未比對的長度已經不足，直接return
                if (endindex > str.Length)
                {
                    return count;
                }

                word = str.Substring(i, endindex - i);

                if (word.EndsWith(keyWord))
                {
                    count += 1;
                    i += keyWord.Length - 1;
                }
            }
            return count;
        }

        // =============================================================================
        // Get some thing
        // =============================================================================
        /// <summary>
        ///     取得字符串指定位置的字符
        /// </summary>
        /// <param name="str"> </param>
        /// <param name="num">
        ///     @return
        /// </param>
        public static string GetStrChar(string str, int num)
        {
            if (str == null || num > str.Length)
            {
                return "";
            }
            return str.Substring(num - 1, num - (num - 1));
        }

        /// <summary>
        ///     取得字串中. 特定字元之前的字串 (包含該字元)
        /// </summary>
        /// <param name="srcString">原始字串</param>
        /// <param name="ctx">特定字元</param>
        /// <param name="charNo">特殊字元的編號 (位於第幾個)</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetPrefixIndexOfChar(string srcString, char ctx, int charNo)
        {
            int length = 0;
            int count = 0;
            foreach (char c in srcString)
            {
                length += 1;
                if (c == ctx)
                {
                    count += 1;
                }
                if (charNo == count)
                {
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            return srcString.Substring(0, length);
        }

        // =============================================================================
        // Replace
        // =============================================================================

        /// <summary>
        ///     取代第一組
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos >= 0)
            {
                return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
            }
            return text;
        }

        // =============================================================================
        // Trim
        // =============================================================================
        /// <summary>
        ///     trim 字串 (避免傳入值為null)
        /// </summary>
        /// <param name="s">
        ///     @return
        /// </param>
        public static string SafeTrim(object s)
        {
            return SafeTrim(s, "");
        }

        /// <summary>
        ///     trim 字串 (避免傳入值為null)
        /// </summary>
        /// <param name="s"> 傳入字 </param>
        /// <param name="defaultStr">
        ///     預設字串
        ///     @return
        /// </param>
        public static string SafeTrim(object s, string defaultStr)
        {
            if (s == null || IsEmpty(s))
            {
                return defaultStr;
            }
            return s.ToString().Trim();
        }

        // =============================================================================
        // Padding
        // =============================================================================
        /// <summary>
        ///     Padding Char
        /// </summary>
        /// <param name="obj"> 原始字串(為直接被 toString) </param>
        /// <param name="paddingChar"> paddingChar </param>
        /// <param name="length"> padding length </param>
        /// <param name="isLeft"> 是否補在字串左邊 </param>
        /// <exception cref="Exception"> </exception>
        public static string Padding(object obj, char paddingChar, int length, bool isLeft = true)
        {
            string str = "";
            if (NotEmpty(obj))
            {
                str += SafeTrim(obj);
            }
            return Padding(str, paddingChar, length, isLeft);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paddingChar"></param>
        /// <param name="length"></param>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        public static string Padding(string str, char paddingChar, int length, bool isLeft = true)
        {
            if (IsEmpty(str)) str = "";
            string paddingCharStr = "";

            for (int i = 0; i < (length * 2); i++)
            {
                paddingCharStr += paddingChar;
            }

            for (int i = 0; (i < str.Length); i++)
            {
                var c = str.Substring(i, 1);

                if (Encoding.Default.GetBytes(c).Length > 1)
                {
                    length -= 2;
                }
                else
                {
                    length -= 1;
                }
            }

            if ((length < 1))
            {
                return str;
            }

            if (isLeft)
            {
                return (paddingCharStr.Substring(0, length) + str);
            }

            return (str + paddingCharStr.Substring(0, length));
        }

        /// <summary>
        ///     將字串補足或切割到指定長度
        /// </summary>
        /// <param name="str"> 原始字串 </param>
        /// <param name="paddingChar"> 要補的字元 </param>
        /// <param name="length"> 指定長度 </param>
        /// <param name="isLeft">
        ///     處理原字串左邊
        ///     @return
        /// </param>
        public static string PaddingAndSplit(string str, char paddingChar, int length, bool isLeft = true)
        {
            return PaddingAndSplit(str, paddingChar, length, isLeft, "UTF-8");
        }

        /// <summary>
        ///     將字串補足或切割到指定長度
        /// </summary>
        /// <param name="str"> 原始字串 </param>
        /// <param name="paddingChar"> 要補的字元 </param>
        /// <param name="length"> 指定長度 </param>
        /// <param name="isLeft"> 處理原字串左邊 </param>
        /// <param name="encodeing"> 計算長度的指定編碼 </param>
        public static string PaddingAndSplit(string str, char paddingChar, int length, bool isLeft, string encodeing = "UTF-8")
        {
            // 初始化傳入字串，避免錯誤
            if (str == null)
            {
                str = "";
            }

            str = Padding(str, paddingChar, length, isLeft);

            if (str.Length > length)
            {
                return str.Substring(0, length);
            }

            return str;
        }

        // =============================================================================
        // is 判斷
        // =============================================================================
        /// <summary>
        ///     檢核字串是否為空
        /// </summary>
        /// <param name="object"> 傳入物件 </param>
        /// <returns> true or false </returns>
        public static bool IsEmpty(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return "".Equals((obj + "").Trim());
        }

        /// <summary>
        /// 檢核字串是否為空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(string str)
        {
            if (str == null)
            {
                return true;
            }

            return "".Equals((str + "").Trim());
        }

        /// <summary>
        ///     檢核List是否為空
        /// </summary>
        /// <param name="list"> 傳入物件 </param>
        /// <returns> true or false </returns>
        public static bool IsEmpty(IList list)
        {
            if (list == null || list.Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     檢核字串是否不為空
        /// </summary>
        /// <param name="str"> </param>
        /// <returns> true or false </returns>
        public static bool NotEmpty(string str)
        {
            return !IsEmpty(str);
        }

        /// <summary>
        ///     檢核物件是否不為空
        /// </summary>
        /// <param name="obj">
        ///     @return
        /// </param>
        public static bool NotEmpty(object obj)
        {
            return !IsEmpty(obj);
        }

        /// <summary>
        ///     檢核List是否為不為空
        /// </summary>
        /// <param name="list"> 傳入物件 </param>
        /// <returns> true or false </returns>
        public static bool NotEmpty(IList list)
        {
            return !IsEmpty(list);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="nullDefault"></param>
        /// <returns></returns>
        //public static string GetNodeAttr(XNode node, string attrName, string nullDefault = "")
        //{
        //    //傳入為空檢核
        //    if (node == null || node.Attributes == null)
        //    {
        //        return "";
        //    }
        //    //依據屬性名稱取得子節點
        //    var childNode = node.Attributes.GetNamedItem(attrName);
        //    if (childNode == null)
        //    {
        //        return nullDefault;
        //    }
        //    //回傳
        //    return SafeTrim(childNode.Value);
        //}

        /// <summary>
        /// 檢核是否數字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNumber(string str)
        {
            int intout;
            return int.TryParse(SafeTrim(str), out intout);
        }

        /// <summary>
        /// 檢核身分證號格式
        /// </summary>
        /// <param name="arg_Identify"></param>
        /// <returns></returns>
        public static bool IsIDNO(object arg)
        {
            var arg_Identify = SafeTrim(arg);
            var d = false;
            if (arg_Identify.Length == 10)
            {
                arg_Identify = arg_Identify.ToUpper();
                if (arg_Identify[0] >= 0x41 && arg_Identify[0] <= 0x5A)
                {
                    var a = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                    var b = new int[11];
                    b[1] = a[(arg_Identify[0]) - 65] % 10;
                    var c = b[0] = a[(arg_Identify[0]) - 65] / 10;
                    for (var i = 1; i <= 9; i++)
                    {
                        b[i + 1] = arg_Identify[i] - 48;
                        c += b[i] * (10 - i);
                    }
                    if (((c % 10) + b[10]) % 10 == 0)
                    {
                        d = true;
                    }
                }
            }
            return d;
        }

        /// <summary>
        ///  檢核是否為統一編號
        /// </summary>
        /// <param name="strCardno"></param>
        /// <returns></returns>
        public static bool IsUniformNo(object arg)
        {
            var strCardno = SafeTrim(arg);

            if (strCardno.Trim().Length < 8)
            {
                return false;
            }
            else
            {
                int[] intTmpVal = new int[6];
                int intTmpSum = 0;
                for (var i = 0; i < 6; i++)
                {
                    //位置在奇數位置的*2，偶數位置*1，位置計算從0開始
                    if (i % 2 == 1)
                        intTmpVal[i] = overTen(int.Parse(strCardno[i].ToString()) * 2);
                    else
                        intTmpVal[i] = overTen(int.Parse(strCardno[i].ToString()));

                    intTmpSum += intTmpVal[i];
                }
                intTmpSum += overTen(int.Parse(strCardno[6].ToString()) * 4); //第6碼*4
                intTmpSum += overTen(int.Parse(strCardno[7].ToString())); //第7碼*1

                if (intTmpSum % 10 != 0) //除以10後餘0表示正確，反之則錯誤
                    return false;
            }
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        private static int overTen(int intVal) //超過10則十位數與個位數相加，直到相加後小於10
        {
            if (intVal >= 10)
                intVal = overTen((intVal / 10) + (intVal % 10));
            return intVal;
        }

        // =============================================================================
        // patten
        // =============================================================================
        /// <summary>
        /// 將傳入字串，轉為駝峰式命名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Convert2CamelCase(string str)
        {
            str = str.Replace("_", " ");
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            str = textInfo.ToTitleCase(str.ToLower());
            return str.Replace(" ", "");
        }
    }
}