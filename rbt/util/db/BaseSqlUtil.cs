using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace rbt.util.db
{
    /// <summary>
    /// SQL 處理工具
    /// @author Allen
    /// </summary>
    public abstract class BaseSqlUtil
    {
        public abstract string getPramChar();

        /// <summary>
        /// 螢幕顯示狀態
        /// </summary>
        public enum DB_TYPE
        {
            ORACLE,
            SQLSERVER,
            POSTGRESQL,
            SQLITE
        }

        /// <summary>
        ///
        /// </summary>
        protected DB_TYPE dbType;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbType"></param>
        public BaseSqlUtil(DB_TYPE dbType)
        {
            this.dbType = dbType;
        }

        /// <summary>
        /// 需實做的實例化方法 : 在組 sql 語法前, 判定欄位是否有關鍵字, 並將關鍵字轉為加上跳脫字元的樣式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract string ConvertEscapeStr(string name);

        /// <summary>
        /// 需實做的實例化方法 : 讓不同的資料庫實做, 取得特化的 DbParameter (ex OracleParameter, SqlParameter ...)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract DbParameter NewDbParameter(string name, object value);

        /// <summary>
        /// 產生Query SQL
        /// </summary>
        /// <param name="tableName"> table name </param>
        /// <param name="selectColumns"> select 欄位字名稱 </param>
        /// <param name="whereColumnsInfo"> where 參數 [欄位名稱, 參數值] </param>
        /// <param name="resultParamsList"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string genQuerySQL(
            string tableName, IList<string> selectColumns,
            IDictionary<string, object> whereColumnsInfo,
            ref IList<IDataParameter> resultParamsList)
        {
            StringBuilder sqlSB = new StringBuilder();
            // =================================================
            // 兜組 Query SQL
            // =================================================
            sqlSB.Append(genSelectSQL(selectColumns));
            sqlSB.Append("FROM " + tableName + " ");
            sqlSB.Append(genWhereSQL(whereColumnsInfo, ref resultParamsList));

            return sqlSB.ToString();
        }

        /// <summary>
        /// 產生 select 字串
        /// </summary>
        /// <param name="selectColumns"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string genSelectSQL(IList<string> selectColumns)
        {
            if (selectColumns == null || selectColumns.Count == 0)
            {
                return "SELECT * ";
            }
            StringBuilder sqlSB = new StringBuilder();
            foreach (string column in selectColumns)
            {
                sqlSB.Append("," + this.ConvertEscapeStr(column) + " ");
            }
            return "SELECT " + sqlSB.ToString().Substring(1) + " ";
        }

        // ==============================================================================================
        // SQL : INSERT
        // =============================================================================================
        /// <summary>
        /// 產生Insert SQL
        /// </summary>
        /// <param name="tableName"> table Name </param>
        /// <param name="insertColumnsInfo"> insert 欄位 LinkedHashMap </param>
        /// <param name="resultParamsList"></param>
        /// <param name="isSetParam"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string genInsertSQL(
            string tableName,
            IDictionary<string, object> insertColumnsInfo,
            ref IList<IDataParameter> resultParamsList, bool isSetParam = false)
        {
            if (StringUtil.IsEmpty(tableName))
            {
                throw new Exception("開發階段錯誤(" + typeof(BaseSqlUtil).ToString() + "): 未傳入 table 名稱 (genInsertSQL)");
            }

            //初始化
            if (resultParamsList == null)
            {
                resultParamsList = new List<IDataParameter>();
            }

            string sqlSB = "";
            // =================================================
            // 兜組 INSERT SQL
            // =================================================
            sqlSB += "INSERT INTO " + tableName + " (";

            // 放入所有欄位名稱
            foreach (string columnName in insertColumnsInfo.Keys)
            {
                //key
                if (columnName.StartsWith("##"))
                {
                    //前綴帶 ## 時, 代表 value 為 function, 需將 columnName 換回正確名稱
                    var myColumnName = columnName.Substring(2);
                    sqlSB += this.ConvertEscapeStr(myColumnName) + ", ";
                }
                else if (insertColumnsInfo[columnName] != null && !DBNull.Value.Equals(insertColumnsInfo[columnName]))
                {
                    //oracle , 為空時 不insert
                    if (StringUtil.IsEmpty(insertColumnsInfo[columnName]) && this.dbType == DB_TYPE.ORACLE) continue;

                    //不為空值時,該欄位才 insert
                    sqlSB += this.ConvertEscapeStr(columnName) + ", ";
                }
            }
            // 最後一個逗號換成右刮號
            sqlSB = sqlSB.Substring(0, sqlSB.Length - 2) + " )";

            // VALUES
            sqlSB += " VALUES (";

            foreach (string columnName in insertColumnsInfo.Keys)
            {
                string value = StringUtil.SafeTrim(insertColumnsInfo[columnName]);

                if (insertColumnsInfo[columnName] == null || DBNull.Value.Equals(insertColumnsInfo[columnName]))
                {
                    //為空時，該欄位略過
                }
                else if (StringUtil.IsEmpty(insertColumnsInfo[columnName]) && this.dbType == DB_TYPE.ORACLE)
                {
                    //oracle , 為空時 不insert
                }
                else if (columnName.StartsWith("##"))
                {
                    //key 前綴帶 ## 時, 代表 value 為 function
                    sqlSB += value + ", ";
                }
                else
                {
                    if (isSetParam)
                    {
                        sqlSB += "'" + procEscChar(value) + "', ";
                    }
                    else
                    {
                        sqlSB += getPramChar() + columnName + ", ";
                        resultParamsList.Add(NewDbParameter(getPramChar() + columnName, insertColumnsInfo[columnName]));
                    }
                }
            }
            // 最後一個逗號換成右刮號
            sqlSB = sqlSB.Substring(0, sqlSB.Length - 2) + " )";

            // return
            return sqlSB;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="resultParamsList"></param>
        /// <returns></returns>
        public string genInsertSQL<TModel>(TModel model, ref IList<IDataParameter> resultParamsList, string tableName = "")
        {
            //初始化
            if (resultParamsList == null)
            {
                resultParamsList = new List<IDataParameter>();
            }

            if (model == null)
            {
                throw new ArgumentException("傳入 insert 的 TbModel 為空 (null!)");
            }

            var modelType = typeof(TModel);
            var rtn = (TModel)Activator.CreateInstance(modelType);

            // =================================================
            // 兜組 INSERT SQL
            // =================================================
            //上半部
            string sqlSB = "";
            if (tableName == "") tableName = modelType.Name;
            sqlSB += "INSERT INTO " + tableName + " (";

            foreach (var property in modelType.GetProperties())
            {
                // 只處理資料庫欄位有對應到的 Property
                MethodInfo mInfo = property.GetGetMethod();

                if (mInfo != null)
                {
                    var propValue = property.GetValue(model);

                    //不為空值時,該欄位才 insert
                    if (propValue != null)
                    {
                        //oracle , 為空時 不insert
                        if (StringUtil.IsEmpty(propValue) && this.dbType == DB_TYPE.ORACLE) continue;
                        sqlSB += this.ConvertEscapeStr(property.Name) + ", ";
                    }
                }
            }

            // 最後一個逗號換成右刮號
            sqlSB = sqlSB.Substring(0, sqlSB.Length - 2) + " )";

            //下半部
            // VALUES
            sqlSB += " VALUES (";

            foreach (var property in modelType.GetProperties())
            {
                // 只處理資料庫欄位有對應到的 Property
                MethodInfo mInfo = property.GetGetMethod();

                if (mInfo != null)
                {
                    var propValue = property.GetValue(model);

                    //不為空值時,該欄位才 insert
                    if (propValue != null)
                    {
                        //oracle , 為空時 不insert
                        if (StringUtil.IsEmpty(propValue) && this.dbType == DB_TYPE.ORACLE) continue;

                        sqlSB += getPramChar() + property.Name + ", ";
                        //

                        if (property.PropertyType == typeof(DateTime) && this.dbType == DB_TYPE.SQLITE)
                        {
                            //DateTime insert 無時間問題
                            resultParamsList.Add(
                                NewDbParameter(
                                    getPramChar() + property.Name, ((DateTime)propValue).ToString("yyyy-MM-dd HH:mm:ss")
                                    ));
                        }
                        else
                        {
                            resultParamsList.Add(NewDbParameter(getPramChar() + property.Name, propValue));
                        }
                    }
                }
            }

            // 最後一個逗號換成右刮號
            sqlSB = sqlSB.Substring(0, sqlSB.Length - 2) + " )";

            // return
            return sqlSB;
        }

        // ==============================================================================================
        // SQL : UPDATE
        // ==============================================================================================
        /// <summary>
        /// 產生UPDATE SQL , 參數值有 null 時, 自動拿掉該參數，直接將null補在兜組的SQL中
        /// ex. set aa=? ==>set aa=null, where aa=? ==> where aa is null
        /// </summary>
        /// <param name="tableName"> table Name </param>
        /// <param name="setColumnsInfo"> 異動欄位 LinkedHashMap </param>
        /// <param name="whereColumnsInfo"> 異動條件 LinkedHashMap </param>
        /// <param name="resultParamsList">回傳的 DbParameter ,會將需要的參數值塞好 (可放nothing, 但外部要定義(dim)好後傳進來接值)</param>
        /// <param name="isSetParam"> 是否將參數值兜組在SQL中</param>
        /// <returns></returns>
        public string genUpdateSQL(
            string tableName,
            IDictionary<string, object> setColumnsInfo,
            IDictionary<string, object> whereColumnsInfo,
            ref IList<IDataParameter> resultParamsList, bool isSetParam = false)
        {
            if (StringUtil.IsEmpty(tableName))
            {
                throw new Exception("開發階段錯誤(" + typeof(BaseSqlUtil).ToString() + "): 未傳入 table 名稱 (genUpdateSQL)");
            }

            if (setColumnsInfo == null || setColumnsInfo.Count == 0)
            {
                throw new Exception("開發階段錯誤(" + typeof(BaseSqlUtil).ToString() + "genUpdateSQL): 未傳入 要 update 的欄位");
            }

            //初始化
            if (resultParamsList == null)
            {
                resultParamsList = new List<IDataParameter>();
            }

            string sqlSB = "";
            // =================================================
            // 兜組 UPDATE SQL
            // =================================================
            sqlSB += "UPDATE " + tableName + " ";
            sqlSB += "SET ";

            // 放入所有SET欄位名稱
            foreach (string columnName in setColumnsInfo.Keys)
            {
                if (setColumnsInfo[columnName] == null)
                {
                    sqlSB += this.ConvertEscapeStr(columnName) + " = null, ";
                }
                else if (columnName.StartsWith("##"))
                {
                    //key 前綴帶 ## 時, 代表 value 為 function
                    sqlSB += this.ConvertEscapeStr(columnName.Substring(2)) + " = " + StringUtil.SafeTrim(setColumnsInfo[columnName]) + ", ";
                }
                else
                {
                    if (isSetParam)
                    {
                        sqlSB += this.ConvertEscapeStr(columnName) + " = '" + procEscChar(setColumnsInfo[columnName]) + "', ";
                    }
                    else
                    {
                        sqlSB += this.ConvertEscapeStr(columnName) + " = " + this.getPramChar() + "SET_" + columnName + ", ";
                        resultParamsList.Add(NewDbParameter("" + this.getPramChar() + "SET_" + columnName, setColumnsInfo[columnName]));
                    }
                }
            }
            // 最後一個逗號換成空白
            sqlSB = sqlSB.Substring(0, sqlSB.Length - 2) + " ";

            // 放入 Where 條件
            sqlSB += genWhereSQL(whereColumnsInfo, ref resultParamsList);

            // return
            return sqlSB;
        }

        // ==============================================================================================
        // SQL : DELETE
        // ==============================================================================================
        /// <summary>
        /// 產生Delete SQL </summary>
        /// <param name="tableName"> table 名稱</param>
        /// <param name="whereColumnsInfo">[欄位名稱, 欄位值]</param>
        /// <param name="resultParamsList">回傳的 DbParameter ,會將需要的參數值塞好 (可放nothing, 但外部要定義(dim)好後傳進來接值)</param>
        /// <returns></returns>
        public string genDeleteSQL(
            string tableName,
            IDictionary<string, object> whereColumnsInfo,
            ref IList<IDataParameter> resultParamsList)
        {
            if (StringUtil.IsEmpty(tableName))
            {
                throw new Exception("開發階段錯誤(" + typeof(BaseSqlUtil).ToString() + "): 未傳入 table 名稱 (genDeleteSQL)");
            }

            // =================================================
            // 兜組 Delete SQL
            // =================================================
            StringBuilder sqlSB = new StringBuilder();
            // delete
            sqlSB.Append("DELETE FROM " + tableName + " ");
            // where
            sqlSB.Append(genWhereSQL(whereColumnsInfo, ref resultParamsList));

            // return
            return sqlSB.ToString();
        }

        // ==============================================================================================
        // SQL : WHERE
        // ==============================================================================================

        /// <summary>
        /// 產生where 條件式 SQL 字串
        /// </summary>
        /// <param name="whereColumnsInfo">[欄位名稱, 欄位值]</param>
        /// <param name="resultParamsList">回傳的 DbParameter ,會將需要的參數值塞好 (可放nothing, 但外部要定義(dim)好後傳進來接值)</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string genWhereSQL(IDictionary<string, object> whereColumnsInfo, ref IList<IDataParameter> resultParamsList)
        {
            //初始化
            if (resultParamsList == null)
            {
                resultParamsList = new List<IDataParameter>();
                //resultParamsList = new List<OracleParameter>();
            }

            // 無where 條件時直接回傳
            if (whereColumnsInfo == null || whereColumnsInfo.Count == 0)
            {
                return "";
            }

            int index = 1;
            StringBuilder sqlSB = new StringBuilder();
            sqlSB.Append("WHERE ");
            foreach (string columnName in whereColumnsInfo.Keys)
            {
                object value = whereColumnsInfo[columnName];
                Type valueType = null;
                if ((whereColumnsInfo[columnName] != null))
                {
                    valueType = whereColumnsInfo[columnName].GetType();
                }
                string paramName = this.getPramChar() + columnName + "_" + index;
                index += 1;

                //判斷 columnName.StartsWith 者，"判斷字符" 長者應該放在前面判斷

                if (value == null)
                {
                    // 傳入值為 null
                    sqlSB.Append(this.ConvertEscapeStr(columnName) + " is null and ");
                }
                else if (object.ReferenceEquals(valueType, typeof(List<string>)))
                {
                    // 傳入值為 List
                    List<string> list = (List<string>)value;
                    if (list.Count == 0)
                    {
                        continue;
                    }
                    sqlSB.Append(this.ConvertEscapeStr(columnName) + " in (" + GenInSql(list) + ") and ");
                }
                else if (object.ReferenceEquals(valueType, typeof(string[])))
                {
                    // 傳入值為 String Array
                    string[] strAry = (string[])value;
                    if (strAry.Length == 0)
                    {
                        continue;
                    }
                    sqlSB.Append(this.ConvertEscapeStr(columnName) + " in (" + GenInSql(strAry) + ") and ");
                }
                else if (columnName.StartsWith("%%"))
                {
                    //like
                    string myColumnName = columnName.Substring(2);
                    paramName = paramName.Replace("%%", "");
                    resultParamsList.Add(NewDbParameter(paramName, "%" + value + "%"));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " like " + paramName + " and ");
                }
                else if (columnName.StartsWith("#@!"))
                {
                    //like (不自動加上模糊字元)
                    string myColumnName = columnName.Substring(3);
                    paramName = paramName.Replace("#@!", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " like " + paramName + " and ");
                }
                else if (columnName.StartsWith("!@#"))
                {
                    //陳述式
                    string myColumnName = columnName.Substring(3);
                    paramName = paramName.Replace("!@#", "");
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " " + value + " and ");
                }
                else if (columnName.StartsWith(">="))
                {
                    //大於等於
                    string myColumnName = columnName.Substring(2);
                    paramName = paramName.Replace(">=", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " >= " + paramName + " and ");
                }
                else if (columnName.StartsWith("<="))
                {
                    //小於等於
                    string myColumnName = columnName.Substring(2);
                    paramName = paramName.Replace("<=", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " <= " + paramName + " and ");
                }
                else if (columnName.StartsWith("<>"))
                {
                    //不等於
                    string myColumnName = columnName.Substring(2);
                    paramName = paramName.Replace("<>", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " <> " + paramName + " and ");
                }
                else if (columnName.StartsWith(">"))
                {
                    //大於
                    string myColumnName = columnName.Substring(1);
                    paramName = paramName.Replace(">", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " > " + paramName + " and ");
                }
                else if (columnName.StartsWith("<"))
                {
                    //小於
                    string myColumnName = columnName.Substring(1);
                    paramName = paramName.Replace("<", "");
                    resultParamsList.Add(NewDbParameter(paramName, value));
                    sqlSB.Append(this.ConvertEscapeStr(myColumnName) + " < " + paramName + " and ");
                }
                else
                {
                    //一般
                    resultParamsList.Add(NewDbParameter("" + this.getPramChar() + "WHERE_" + columnName, value));
                    sqlSB.Append(this.ConvertEscapeStr(columnName) + " = " + this.getPramChar() + "WHERE_" + columnName + " and ");
                }
            }

            //去掉最後一個多出來的 and
            return sqlSB.ToString().Substring(0, sqlSB.Length - 5);
        }

        // ==============================================================================================
        // SQL : IN
        // ==============================================================================================
        /// <summary>
        /// 分隔符號字串 => SQL IN 字串 </summary>
        /// <param name="str"> </param>
        /// <param name="spliteStr">
        /// @return </param>
        public static string GenInSql(string str, char spliteStr)
        {
            // 兜組 msgid IN 參數
            return GenInSql(str.Split(spliteStr));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string GenInSql(IList<string> paramList)
        {
            return GenInSql(((List<string>)paramList).ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        public static string GenInSql(string[] paramaters)
        {
            if (paramaters == null || paramaters.Length == 0)
            {
                return "";
            }
            string result = "";
            foreach (var param in paramaters)
            {
                if (param.Length > 0)
                {
                    result += ",'" + StringUtil.SafeTrim(param) + "'";
                }
                else
                {
                    result += ",NULL";
                }
            }

            return result.Substring(1);
        }

        // ==============================================================================================
        // SQL : IN
        // ==============================================================================================
        /// <summary>
        /// 產生ORDER BY String
        /// </summary>
        /// <param name="orderByFileds">排序欄位</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GenOrderBySql(string[] orderByFileds)
        {
            if (orderByFileds == null || orderByFileds.Length == 0)
            {
                return "";
            }
            string result = "";
            foreach (string param in orderByFileds)
            {
                if (StringUtil.NotEmpty(param))
                {
                    result += ", " + this.ConvertEscapeStr(StringUtil.SafeTrim(param));
                }
            }

            return " ORDER BY " + result.Substring(1);
        }

        // ==============================================================================================
        // OTHER
        // ==============================================================================================
        private static string procEscChar(object obj)
        {
            string value = StringUtil.SafeTrim(obj);
            value = value.Replace("'", "''");
            value = value.Replace("\"", "\\\"");
            return value;
        }
    }
}