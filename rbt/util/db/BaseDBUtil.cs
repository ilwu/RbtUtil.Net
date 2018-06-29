using Oracle.ManagedDataAccess.Client;
using rbt.util.db.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace rbt.util.db
{
    /// <summary>
    ///     DB 物件化操作工具, 抽象化類別
    ///     @author Allen
    /// </summary>
    public abstract class BaseDbUtil : IDBUtil
    {
        private BaseSqlUtil _mySqlUtil;

        private BaseSqlUtil SqlUtil()
        {
            return _mySqlUtil ?? (_mySqlUtil = NewSqlUtil());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Find<T>() where T : BaseDBEntity, IDBEntity
        {
            var model = Activator.CreateInstance<T>();
            model.SetDAO(this);
            return model;
        }

        // ==============================================================================================
        // 需實做的方法：指定不同的 DB 存取物件
        // ==============================================================================================
        public abstract BaseSqlUtil NewSqlUtil();

        /// <summary>
        /// 需實做的實例化 DbParameter 方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract DbParameter NewDbParameter(string name, object value);

        /// <summary>
        ///需實做的實例化 DbCommand 方法
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        protected abstract DbCommand NewDbCommand(string commandText, DbConnection connection);

        /// <summary>
        ///需實做的實例化 DbDataAdapter 方法
        /// </summary>
        /// <param name="selectCommandText"></param>
        /// <param name="selectConnection"></param>
        /// <returns></returns>
        protected abstract DbDataAdapter NewDbDataAdapter(string selectCommandText, DbConnection selectConnection);

        // ==============================================================================================
        // get Connection
        // ==============================================================================================
        /// <summary>
        ///     getConnection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract DbConnection GetConnection();

        // ==============================================================================================
        // Get ConnectionStringBuilder
        // ==============================================================================================
        /// <summary>
        ///     Get ConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract DbConnectionStringBuilder GetConnectionStringBuilder();

        // ==============================================================================================
        // QUERY Count
        // ==============================================================================================

        /// <summary>
        ///     依據傳入的SQL查詢結果，回傳查詢筆數
        /// </summary>
        /// <param name="querySql">查詢字串</param>
        /// <param name="params">參數 [參數名稱,參數值]</param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int QueryCount(
            string querySql,
            Dictionary<string, object> @params = null,
            DbConnection conn = null,
            DbTransaction transaction = null)
        {
            //套上筆數查詢參數
            querySql = "select count(*) as CNT from (" + querySql + ") zz ";
            //查詢
            var dataList = Query(querySql, @params, conn, transaction);
            //回傳筆數
            foreach (var item in dataList)
            {
                return Convert.ToInt32(StringUtil.SafeTrim(item["CNT"], "0"));
            }
            return 0;
        }

        /// <summary>
        ///     查詢單一 table 中資料的筆數
        /// </summary>
        /// <param name="conn"> Connection (傳入 null 時, 會自動 getConnection) </param>
        /// <param name="tableName"> tablen Name </param>
        /// <param name="whereColumnsInfo"> 條件欄位 [欄位名稱,參數值]</param>
        /// <param name="transaction"></param>
        public int QueryCountByTable(string tableName, Dictionary<string, object> whereColumnsInfo,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            //兜組 SQL
            var sql = SqlUtil().genQuerySQL(tableName, null, whereColumnsInfo, ref paramsList);

            //套上筆數查詢參數
            sql = "select count(*) as CNT from (" + sql + ") zz ";

            //查詢
            var dataList = Query(sql, paramsList, conn, transaction);

            //回傳筆數
            foreach (var item in dataList)
            {
                return Convert.ToInt32(StringUtil.SafeTrim(item["CNT"], "0"));
            }
            return 0;
        }

        // ==============================================================================================
        // DB By Table
        // ==============================================================================================

        /// <summary>
        ///     查詢單一 Table
        /// </summary>
        /// <param name="tableName">查詢單一 Table</param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<T> QueryByTable<T>(string tableName, Dictionary<string, object> whereColumnsInfo = null,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            return this.MapToList<T>(QueryByTable(tableName, whereColumnsInfo, conn, transaction));
        }

        /// <summary>
        ///     查詢單一 Table
        /// </summary>
        /// <param name="tableName">查詢單一 Table</param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IDictionary<string, object>> QueryByTable(string tableName, Dictionary<string, object> whereColumnsInfo = null,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            // 產生查詢 SQL
            var queryStr = SqlUtil().genQuerySQL(tableName, null, whereColumnsInfo, ref paramsList);

            // 查詢
            return Query(queryStr, paramsList, conn, transaction);
        }

        /// <summary>
        ///  查詢單一 Table 且 order by
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="whereColumnsInfo"></param>
        /// <param name="orderByFileds"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IList<T> QueryByTableAndOrderBy<T>(string tableName, Dictionary<string, object> whereColumnsInfo,
            string[] orderByFileds, DbConnection conn = null, DbTransaction transaction = null)
        {
            return this.MapToList<T>(QueryByTableAndOrderBy(tableName, whereColumnsInfo, orderByFileds, conn, transaction));
        }

        /// <summary>
        /// 查詢單一 Table 且 order by
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereColumnsInfo"></param>
        /// <param name="orderByFileds"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IDictionary<string, object>> QueryByTableAndOrderBy(string tableName, Dictionary<string, object> whereColumnsInfo,
            string[] orderByFileds, DbConnection conn = null, DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            // 產生查詢 SQL
            var queryStr = SqlUtil().genQuerySQL(tableName, null, whereColumnsInfo, ref paramsList);

            //order by
            queryStr += SqlUtil().GenOrderBySql(orderByFileds);

            // 查詢
            return Query(queryStr, paramsList, conn, transaction);
        }

        /// <summary>
        ///     查詢單一 Table
        /// </summary>
        /// <param name="tableName">查詢單一 Table</param>
        /// <param name="selectColumns"></param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<T> QueryByTable<T>(string tableName, List<string> selectColumns,
            Dictionary<string, object> whereColumnsInfo, DbConnection conn, DbTransaction transaction)
        {
            return this.MapToList<T>(QueryByTable(tableName, selectColumns, whereColumnsInfo, conn, transaction));
        }

        /// <summary>
        ///     查詢單一 Table
        /// </summary>
        /// <param name="tableName">查詢單一 Table</param>
        /// <param name="selectColumns"></param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IDictionary<string, object>> QueryByTable(
            string tableName, IList<string> selectColumns,
            IDictionary<string, object> whereColumnsInfo,
            DbConnection conn, DbTransaction transaction)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            // 產生查詢 SQL
            var queryStr = SqlUtil().genQuerySQL(tableName, selectColumns, whereColumnsInfo, ref paramsList);

            // 查詢
            return Query(queryStr, paramsList, conn, transaction);
        }

        // ==============================================================================================
        // DB QUERY
        // ==============================================================================================
        /// <summary>
        ///     Query by native SQL
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="params">dictionary.add(參數名稱,參數值)</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<T> Query<T>(
            string querySql,
            IDictionary<string, object> paramsDic = null,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            var result = Query(querySql, paramsDic, conn, transaction, commandType);
            return this.MapToList<T>(result);
        }

        /// <summary>
        ///     Query by native SQL
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="paramsDic">dictionary.add(參數名稱,參數值)</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IDictionary<string, object>> Query(string querySql,
            IDictionary<string, object> paramsDic = null,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            return this.Query(querySql, Dic2Paramater(paramsDic), conn, transaction);
        }

        /// <summary>
        ///     Query by native SQL
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="paramList">DbParameter List</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IDictionary<string, object>> Query(
            string querySql,
            IList<IDataParameter> paramList,
            DbConnection conn,
            DbTransaction transaction,
            CommandType commandType = CommandType.Text
            )
        {
            //查詢
            var datatable = QueryDataTable(querySql, paramList, conn, transaction, commandType);

            //轉為 Dictionary
            if (datatable == null) return null;

            //return
            var dicList = new List<IDictionary<string, object>>();

            foreach (DataRow row in datatable.Rows)
            {
                var rowRic = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in datatable.Columns)
                {
                    rowRic.Add(dataColumn.ColumnName, row[dataColumn.ColumnName]);
                }
                dicList.Add(rowRic);
            }

            return dicList;
        }

        /// <summary>
        ///     Query by native SQL
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="paramList">DbParameter List</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataTable QueryDataTable(
            string querySql,
            IDictionary<string, object> paramsDic = null,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            return QueryDataTable(querySql, Dic2Paramater(paramsDic), conn, transaction, commandType);
        }

        /// <summary>
        ///     Query by native SQL
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="paramList">DbParameter List</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataTable QueryDataTable(
            string querySql,
            IList<IDataParameter> paramList,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            //紀錄是否需把 connection  銷毀
            var needDesposeConn = false;
            //紀錄是否需把 connection  關掉
            var needCloseConn = false;

            DbDataAdapter dataAdapter = null;

            try
            {
                //把全形空白換成半形
                querySql = querySql.Replace("　", " ");

                //=========================================
                //connection , 未傳入時，自己產生一個
                //=========================================
                if (conn == null)
                {
                    conn = GetConnection();
                    needDesposeConn = true;
                }

                //開啟連線物件
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    needCloseConn = true;
                }

                //=========================================
                //建立command
                //=========================================
                //參數值放入 command
                var dbCommand = NewDbCommand(querySql, conn);
                if ((paramList != null))
                {
                    foreach (var param in paramList)
                    {
                        dbCommand.Parameters.Add(param);
                    }
                }

                dbCommand.CommandType = commandType;

                //=========================================
                //Transaction
                //=========================================
                //有傳入 Transaction 時使用之
                if (!needCloseConn && (transaction != null))
                {
                    dbCommand.Transaction = transaction;
                }

                //=========================================
                //查詢
                //=========================================
                //執行指令
                dataAdapter = NewDbDataAdapter("", conn);
                dataAdapter.SelectCommand = dbCommand;

                //查詢
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                var sParaValue = "";

                if ((paramList != null))
                {
                    foreach (var param in paramList)
                    {
                        sParaValue += param.ParameterName + ":[" + StringUtil.SafeTrim(param.Value) + "]" + "\r\n";
                    }
                }

                var errorMessage =
                    "DBUtil Query Error!" + System.Environment.NewLine +
                    "Exception Type:[" + ex.GetType() + "]" + System.Environment.NewLine +
                    "Exception Message:[" + ex.Message + "]" + System.Environment.NewLine +
                    "SQL:[" + System.Environment.NewLine + querySql + System.Environment.NewLine + "]" + System.Environment.NewLine +
                    sParaValue + System.Environment.NewLine +
                    ex.StackTrace;

                throw new Exception(errorMessage, ex);
            }
            finally
            {
                if ((dataAdapter != null))
                {
                    dataAdapter.Dispose();
                }

                if (((conn != null)))
                {
                    if (needCloseConn)
                    {
                        conn.Close();
                    }
                    if (needDesposeConn)
                    {
                        conn.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///   Query by native SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querySql"></param>
        /// <param name="paramList"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IList<T> Query<T>(string querySql, List<IDataParameter> paramList, DbConnection conn,
            DbTransaction transaction)
        {
            return this.MapToList<T>(this.Query(querySql, paramList, conn, transaction));
        }

        /// <summary>
        ///     查詢傳入的 SQL 中 第一筆, 第一欄的值
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="params">dictionary.add(參數名稱,參數值)</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string QueryFirstValue(string querySql, Dictionary<string, object> @params = null,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            //查詢
            var dataList = Query(querySql, @params, conn, transaction);
            //查詢傳入的 SQL 中 第一筆, 第一欄的值
            foreach (Dictionary<string, object> item in dataList)
            {
                foreach (var val in item.Values)
                {
                    return StringUtil.SafeTrim(val);
                }
            }
            return "";
        }

        /// <summary>
        ///     查詢傳入的 SQL, 並回傳結果第一筆
        /// </summary>
        /// <param name="querySql">SQL 字串</param>
        /// <param name="paramList">dictionary.add(參數名稱,參數值)</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Dictionary<string, object> QueryFirstRow(string querySql, Dictionary<string, object> paramList = null,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            //查詢
            var dataList = Query(querySql, paramList, conn, transaction);
            //查詢傳入的 SQL 中 第一筆
            foreach (Dictionary<string, object> item in dataList)
            {
                return item;
            }

            return null;
        }

        // ==============================================================================================
        // DB INSTER
        // ==============================================================================================

        /// <summary>
        ///     INSERT
        /// </summary>
        /// <param name="tableName"> table name</param>
        /// <param name="insertColumnsInfo"> INSER 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Insert(string tableName, IDictionary<string, object> insertColumnsInfo, DbConnection conn = null,
            DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            // 產生INSERT SQL
            var insertStr = SqlUtil().genInsertSQL(tableName, insertColumnsInfo, ref paramsList);

            //執行
            return ExcuteByDbParameter(insertStr, paramsList, conn, transaction);
        }

        /// <summary>
        /// INSERT (綁定 Table Model 的方式)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Insert<TModel>(TModel model, DbConnection conn = null, DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            var isOracle = this.GetConnection().GetType() == typeof(OracleConnection);

            // 產生INSERT SQL
            var insertStr = SqlUtil().genInsertSQL<TModel>(model, ref paramsList);

            //執行
            return ExcuteByDbParameter(insertStr, paramsList, conn, transaction);
        }

        /// <summary>
        /// INSERT (綁定 Table Model 的方式)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Insert<TModel>(TModel model, string tableName, DbConnection conn = null, DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            // 產生INSERT SQL
            var insertStr = SqlUtil().genInsertSQL<TModel>(model, ref paramsList, tableName);

            //執行
            return ExcuteByDbParameter(insertStr, paramsList, conn, transaction);
        }

        // ==============================================================================================
        // DB UPDATE
        // ==============================================================================================

        /// <summary>
        ///     UPDATE
        /// </summary>
        /// <param name="tableName"> table name</param>
        /// <param name="setColumnsInfo"> UPDATE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="whereColumnsInfo"> WHERE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Update(string tableName, Dictionary<string, object> setColumnsInfo,
            Dictionary<string, object> whereColumnsInfo = null, DbConnection conn = null,
            DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            //產生UPDATE SQL
            var updateSql = SqlUtil().genUpdateSQL(tableName, setColumnsInfo, whereColumnsInfo, ref paramsList);

            //執行
            return ExcuteByDbParameter(updateSql, paramsList, conn, transaction);
        }

        // ==============================================================================================
        // DB DELETE
        // ==============================================================================================

        /// <summary>
        ///     公用 DELETE
        /// </summary>
        /// <param name="tableName"> table name</param>
        /// <param name="whereColumnsInfo"> WHERE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Delete(string tableName, Dictionary<string, object> whereColumnsInfo, DbConnection conn = null,
            DbTransaction transaction = null)
        {
            //參數存放區
            IList<IDataParameter> paramsList = null;

            //產生UPDATE SQL
            var deleteSql = SqlUtil().genDeleteSQL(tableName, whereColumnsInfo, ref paramsList);

            //執行
            return ExcuteByDbParameter(deleteSql, paramsList, conn, transaction);
        }

        // ==============================================================================================
        // DB EXECUTE
        // ==============================================================================================

        /// <summary>
        ///     execute SQL
        /// </summary>
        /// <param name="executeSql">SQL 字串</param>
        /// <param name="paramList">DbParameter List</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ExcuteByDbParameter(
            string executeSql,
            IList<IDataParameter> paramList,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            //紀錄是否需把 connection  銷毀
            var needDesposeConn = false;
            //紀錄是否需把 connection  關掉
            var needCloseConn = false;

            try
            {
                //把全形空白換成半形
                executeSql = executeSql.Replace("　", " ");

                //=========================================
                //connection , 未傳入時，自己產生一個
                //=========================================
                if (conn == null)
                {
                    conn = GetConnection();
                    needDesposeConn = true;
                }

                //開啟連線物件
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    needCloseConn = true;
                }

                //=========================================
                //建立command
                //=========================================
                //cerate
                var dbCommand = NewDbCommand(executeSql, conn);
                if ((paramList != null))
                {
                    foreach (var param in paramList)
                    {
                        dbCommand.Parameters.Add(param);
                    }
                }

                //CommandType
                dbCommand.CommandType = commandType;
                //=========================================
                //Transaction
                //=========================================
                //有傳入 Transaction 時使用之
                if (!needCloseConn && (transaction != null))
                {
                    dbCommand.Transaction = transaction;
                }

                //=========================================
                //執行指令
                //=========================================
                return dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var sParaValue = "";

                if ((paramList != null))
                {
                    foreach (var param in paramList)
                    {
                        sParaValue += param.ParameterName + ":[" + StringUtil.SafeTrim(param.Value) + "]" + "\r\n";
                    }
                }
                throw new Exception(executeSql + "\r\n" + sParaValue + "\r\n" + ex.Message);
            }
            finally
            {
                if (((conn != null)))
                {
                    if (needCloseConn)
                    {
                        conn.Close();
                    }
                    if (needDesposeConn)
                    {
                        conn.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///     execute SQL
        /// </summary>
        /// <param name="executeSql">SQL 字串</param>
        /// <param name="params">params</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Excute(
            string executeSql,
            IDictionary<string, object> paramsDic = null,
            DbConnection conn = null,
            DbTransaction transaction = null,
            CommandType commandType = CommandType.Text
            )
        {
            //將傳入參數轉換為 IDataParameter
            var dbParamList = new List<IDataParameter>();
            if (paramsDic != null && paramsDic.Keys.Count > 0)
            {
                foreach (var param in paramsDic)
                {
                    dbParamList.Add(this.NewDbParameter(param.Key, param.Value));
                }
            }

            return ExcuteByDbParameter(executeSql, dbParamList, conn, transaction, commandType);
        }

        // ==============================================================================================
        // 是否存在
        // ==============================================================================================

        /// <summary>
        ///     依據傳入的 sql 查詢筆數是否大於0
        /// </summary>
        /// <param name="querySql"></param>
        /// <param name="params"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsExist(string querySql, Dictionary<string, object> @params = null, DbConnection conn = null,
            DbTransaction transaction = null)
        {
            //依據傳入的 sql 查詢筆數是否大於0
            return Query(querySql, @params, conn, transaction).Count > 0;
        }

        /// <summary>
        ///     以 table 查詢資料是否存在
        /// </summary>
        /// <param name="tableName">Table 名稱</param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsExistByTable(string tableName, Dictionary<string, object> whereColumnsInfo = null,
            DbConnection conn = null, DbTransaction transaction = null)
        {
            //依據傳入的 sql 查詢筆數是否大於0
            return QueryByTable(tableName, null, whereColumnsInfo, conn, transaction).Count > 0;
        }

        // ==============================================================================================
        // SEQ
        // ==============================================================================================
        /// <summary>
        /// 取得 Sequences
        /// </summary>
        /// <param name="sequencesName">sequences 名稱</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetSequences(string sequencesName, DbConnection conn = null, DbTransaction transaction = null)
        {
            var sql = "Select " + sequencesName + ".nextval FROM DUAL";
            return QueryFirstValue(sql, null, conn, transaction);
        }

        // ==============================================================================================
        // 其他
        // ==============================================================================================
        /// <summary>
        /// 對 dataTable 的 row index 做排序 (非 view)
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="sortConditionStr"></param>
        /// <remarks></remarks>
        public static void SortDataTable(DataTable dataTable, string sortConditionStr)
        {
            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            var dv = dataTable.DefaultView;
            dv.Sort = sortConditionStr;
            dataTable = dv.ToTable();
        }

        // ==============================================================================================
        // obj mapping
        // ==============================================================================================
        public IDictionary<Type, Func<object, object>> ConvertFuncMap;

        public IDictionary<Type, Func<object, object>> GetConvertFuncMap()
        {
            if (ConvertFuncMap == null) ConvertFuncMap = ClassUtil.GetConvertFuncMap();
            return ConvertFuncMap;
        }

        private IList<T> MapToList<T>(IList<IDictionary<string, object>> list)
        {
            var newList = new List<T>();
            foreach (var dic in list)
            {
                newList.Add(this.MapTo<T>(dic));
            }
            return newList;
        }

        private T MapTo<T>(IDictionary<string, object> dic)
        {
            if (dic != null)
            {
                T rtn = (T)Activator.CreateInstance(typeof(T));
                foreach (var p in typeof(T).GetProperties())
                {
                    var pName = "";
                    //忽略 db 欄位大小寫和 model 的不同
                    if (dic.ContainsKey(p.Name))
                    {
                        pName = p.Name;
                    }
                    else if (dic.ContainsKey(p.Name.ToUpper())) { pName = p.Name.ToUpper(); }
                    else if (dic.ContainsKey(p.Name.ToLower())) { pName = p.Name.ToLower(); }

                    if (pName != "")
                    {
                        // 只處理資料庫欄位有對應到的 Property
                        MethodInfo mInfo = p.GetSetMethod();
                        if (mInfo != null)
                        {
                            Type ptype = p.PropertyType;

                            try
                            {
                                if (Convert.IsDBNull(dic[pName]))
                                {
                                    p.SetValue(rtn, null);
                                }
                                else if (GetConvertFuncMap().ContainsKey(ptype))
                                {
                                    Func<object, object> converter = ConvertFuncMap[ptype];
                                    object cvtVal = converter(dic[pName]);
                                    p.SetValue(rtn, cvtVal);
                                }
                                else
                                {
                                    p.SetValue(rtn, dic[pName]);
                                }
                            }
                            catch (System.ArgumentException ex)
                            {
                                throw new ArgumentException("Property '" + p.Name + "：'" + ex.Message);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Property '" + p.Name + "' 沒有定義 set method");
                        }
                    }
                }
                return rtn;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 將 IDictionary 狀態的參數轉為 IDataParameter
        /// </summary>
        /// <param name="paramsDic"></param>
        private IList<IDataParameter> Dic2Paramater(IDictionary<string, object> paramsDic)
        {
            //將傳入參數轉換為 IDataParameter
            var dbParamList = new List<IDataParameter>();
            if (paramsDic != null && paramsDic.Keys.Count > 0)
            {
                foreach (var param in paramsDic)
                {
                    dbParamList.Add(this.NewDbParameter(param.Key, param.Value));
                }
            }
            return dbParamList;
        }

        // ''' <summary>
        // ''' 對 dataTable 做排序
        // ''' </summary>
        // ''' <param name="dataTable"></param>
        // ''' <param name="sortColumnArray">排序欄位</param>
        // ''' <remarks></remarks>
        //Public Shared Sub SortDataTable(BydataTable As DataTable, ByVal sortColumnArray As String())
        //    If sortColumnArray Is Nothing OrElse sortColumnArray.Length = 0 Then
        //        Return
        //    End If

        //    Dim orderByStr As String = ""
        //    For Each str As String In sortColumnArray
        //        orderByStr &= "," & str
        //    Next
        //    dataTable.DefaultView.Sort = orderByStr
        //    dataTable.AcceptChanges()
        //    dataTable = dataTable.DefaultView.Table
        //End Sub
        // ''' <summary>
        // ''' 對 dataTable 做排序
        // ''' </summary>
        // ''' <param name="dataTable"></param>
        // ''' <param name="sortColumnStr">排序字串</param>
        // ''' <remarks></remarks>
        //Public Shared Sub SortDataTable(BydataTable As DataTable, ByVal sortColumnStr As String)
        //    Dim newDataTable As DataTable = dataTable.Clone()
        //    newDataTable.Select("", sortColumnStr)

        //    For Each newRow As DataRow In dataTable.Select("", sortColumnStr)
        //        newDataTable.ImportRow(newRow)
        //    Next
        //    dataTable = newDataTable
        //End Sub
    }
}