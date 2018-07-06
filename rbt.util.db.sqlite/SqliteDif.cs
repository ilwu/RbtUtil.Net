using rbt.util;
using rbt.util.db.sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RbtLibrary.rbt.util.db.sqlite
{
    /// <summary>
    /// 未完成
    /// </summary>
    internal class SqliteDif
    {
        private string _testTable = "";

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceDb"></param>
        /// <param name="sourceSchema"></param>
        /// <param name="targetDb"></param>
        /// <param name="targetSchema"></param>
        /// <param name="resultFilePath"></param>
        /// <param name="resultFileName"></param>
        public SqliteDif(
            SqlLiteDBUtil sourceDb, string sourceSchema, SqlLiteDBUtil targetDb, string targetSchema,
            string resultFilePath, string resultFileName)
        {
            //FileUtil fu = new FileUtil();
            var fu = new StringBuilder();

            // ==============================================================
            // 取得資料庫的 Schema 資訊
            // ==============================================================
            // 取得來源的table Schema 資料

            Console.WriteLine("取得來源資料庫 Schema..");
            var sourceTableInfoMap = GetAllTable(sourceDb, sourceSchema);
            // System.out.println(new BeanUtil().showContent(sourceTableInfoMap));
            // 取得來源的table Schema 資料
            Console.WriteLine("取得目標資料庫 Schema..");
            var targetTableInfoMap = GetAllTable(targetDb, targetSchema);
            Console.WriteLine("比對中..");

            // ==============================================================
            // 比對缺少的 TABLE
            // ==============================================================
            var firstAddFlag = true;

            foreach (var tableName in sourceTableInfoMap.Keys)
            {
                if (!targetTableInfoMap.ContainsKey(tableName))
                {
                    Console.WriteLine("目標缺少 TABLE :[" + tableName + "]");

                    if (firstAddFlag)
                    {
                        fu.AppendLine("/*==============================================================*/");
                        fu.AppendLine("/* 新增 Table*/");
                        fu.AppendLine("/*==============================================================*/");
                        firstAddFlag = false;
                    }

                    // 產生 create sql
                    var sourceTableInfo = sourceTableInfoMap[tableName];
                    fu.AppendLine(GanCreateSql(targetSchema, tableName, sourceTableInfo.ColumnDataMapList,
                        sourceTableInfo.GetpKeySet()));
                }
            }

            // ==============================================================
            // 各 table 欄位比對
            // ==============================================================
            foreach (var tableName in sourceTableInfoMap.Keys)
            {
                // 目標無此 table 時跳過
                if (!targetTableInfoMap.ContainsKey(tableName))
                {
                    continue;
                }

                // 取得來源 TableInfo
                var sourceTableInfo = sourceTableInfoMap[tableName];
                // 取得目標 TableInfo
                var targetTableInfo = targetTableInfoMap[tableName];

                // 新增欄位
                var addSql = DiffAddColumn(targetSchema, tableName, sourceTableInfo.ColumnDataMapList,
                    targetTableInfo.ColumnNameSet);

                // 修改欄位
                var modifySql = DiffModifyColumn(targetSchema, tableName, sourceTableInfo.ColumnDataMapByColName,
                    targetTableInfo.ColumnDataMapByColName);

                if (StringUtil.NotEmpty(addSql) || StringUtil.NotEmpty(modifySql))
                {
                    fu.AppendLine("/*==============================================================*/");
                    fu.AppendLine("/* 異動 Table : " + tableName + " */");
                    fu.AppendLine("/*==============================================================*/");
                    fu.AppendLine(addSql);
                    fu.AppendLine(modifySql);
                }
            }

            // ==============================================================
            // 反向比對
            // ==============================================================
            //FileUtil reFu = new FileUtil();
            var reFu = new StringBuilder();
            foreach (var tableName in targetTableInfoMap.Keys)
            {
                // 比對缺少TABLE
                if (!sourceTableInfoMap.ContainsKey(tableName))
                {
                    reFu.AppendLine("--來源缺少 TABLE :[" + tableName + "]");
                    continue;
                }
                // 比對缺少欄位
                // 取得來源 TableInfo
                var sourceTableInfo = sourceTableInfoMap[tableName];
                // 取得目標 TableInfo
                var targetTableInfo = targetTableInfoMap[tableName];

                foreach (var columnName in targetTableInfo.ColumnNameSet)
                {
                    if (!sourceTableInfo.ColumnNameSet.Contains(columnName))
                    {
                        reFu.AppendLine("--來源 TABLE " + tableName + " 缺少欄位 :[" + columnName + "]");
                    }
                }
            }

            if (StringUtil.NotEmpty(fu.ToString()))
            {
                fu.AppendLine("/*==============================================================*/");
                fu.AppendLine("/* 反向比對差異部分 */");
                fu.AppendLine("/*==============================================================*/");
                fu.AppendLine(reFu.ToString());
            }

            //TODO
            //fu.writeToFile(resultFilePath, resultFileName);
        }

        /// <summary>
        /// 比對新增欄位 </summary>
        /// <param name="targetSchema"> </param>
        /// <param name="tableName"> </param>
        /// <param name="srcColumnDataList"> </param>
        /// <param name="targetColumnSet">
        /// @return </param>
        private string DiffAddColumn(string targetSchema, string tableName,
            List<Dictionary<string, object>> srcColumnDataList, List<string> targetColumnSet)
        {
            var fu = new StringBuilder();
            foreach (var srcColumnInfo in srcColumnDataList)
            {
                var columnName = StringUtil.SafeTrim(srcColumnInfo["COLUMN_NAME"]);
                var dataType = StringUtil.SafeTrim(srcColumnInfo["DATA_TYPE"]);
                var dataLength = StringUtil.SafeTrim(srcColumnInfo["DATA_LENGTH"]);
                var dataPrecision = StringUtil.SafeTrim(srcColumnInfo["DATA_PRECISION"]);
                var dataDefault = StringUtil.SafeTrim(srcColumnInfo["DATA_DEFAULT"]);
                var nullable = StringUtil.SafeTrim(srcColumnInfo["NULLABLE"]);
                var charLength = StringUtil.SafeTrim(srcColumnInfo["CHAR_LENGTH"]);

                // 存在時略過
                if (targetColumnSet.Contains(columnName))
                {
                    continue;
                }

                fu.AppendLine("ALTER TABLE " + targetSchema + "." + tableName + " ADD (" +
                              GetCloumSql(columnName, dataType, dataLength, dataPrecision, dataDefault,
                                  nullable, charLength) + ");");
            }

            var result = fu.ToString();
            if (StringUtil.NotEmpty(result))
            {
                result = "--新增欄位\r\n" + result;
            }

            return result;
        }

        private string DiffModifyColumn(string targetSchema, string tableName,
            Dictionary<string, Dictionary<string, object>> srcColumnInfoByColName,
            Dictionary<string, Dictionary<string, object>> targetColumnInfoByColName)
        {
            //FileUtil fu = new FileUtil();
            var fu = new StringBuilder();

            foreach (var srcColumnInfo in srcColumnInfoByColName.Values)
            {
                // 欄位名稱
                var columnName = StringUtil.SafeTrim(srcColumnInfo["COLUMN_NAME"]);
                // 目標DB欄位資訊
                var targetColumnInfo = targetColumnInfoByColName[columnName];
                if (targetColumnInfo == null)
                {
                    continue;
                }

                //FileUtil subFu = new FileUtil();
                var subFu = new StringBuilder();

                // 比較欄位型態
                var srcDataType = StringUtil.SafeTrim(srcColumnInfo["DATA_TYPE"]);
                var srcDataLength = StringUtil.SafeTrim(srcColumnInfo["DATA_LENGTH"]);
                var srcDataPrecision = StringUtil.SafeTrim(srcColumnInfo["DATA_PRECISION"]);
                var srcCharLength = StringUtil.SafeTrim(srcColumnInfo["CHAR_LENGTH"]);
                var targetDataType = StringUtil.SafeTrim(targetColumnInfo["DATA_TYPE"]);
                var targetDataLength = StringUtil.SafeTrim(targetColumnInfo["DATA_LENGTH"]);
                var targetDataPrecision = StringUtil.SafeTrim(targetColumnInfo["DATA_PRECISION"]);
                var targetCharLength = StringUtil.SafeTrim(targetColumnInfo["CHAR_LENGTH"]);

                if (!srcDataType.Equals(targetDataType) || !srcDataLength.Equals(targetDataLength) ||
                    !srcDataPrecision.Equals(targetDataPrecision) || !srcCharLength.Equals(targetCharLength))
                {
                    // ALTER TABLE TES.ALBUM MODIFY (MODTIME LONG RAW(13))
                    if ("NVARCHAR2".Equals(srcDataType) && "NVARCHAR2".Equals(targetDataType) &&
                        srcCharLength.Equals(targetCharLength))
                    {
                        // 若型態為 NVARCHAR2 時，DATA_LENGTH 可能依據資料庫不同而異, 故只比對 CHAR_LENGTH
                    }
                    else
                    {
                        subFu.AppendLine("ALTER TABLE " + targetSchema + "." + tableName + " MODIFY (" + columnName +
                                         " " +
                                         getType(srcDataType, srcDataLength, srcDataPrecision, srcCharLength) +
                                         ");");
                    }
                }

                // 比較nullable
                var srcNullable = StringUtil.SafeTrim(srcColumnInfo["NULLABLE"]);
                var targetNullable = StringUtil.SafeTrim(targetColumnInfo["NULLABLE"]);

                if (!srcNullable.Equals(targetNullable))
                {
                    subFu.AppendLine("ALTER TABLE " + targetSchema + "." + tableName + " MODIFY (" + columnName + " " +
                                     ("N".Equals(srcNullable) ? "NOT " : "") + "NULL);");
                }

                // 比較 DATA_DEFAULT
                var srcDataDefault = StringUtil.SafeTrim(srcColumnInfo["DATA_DEFAULT"]);
                var targetDataDefault = StringUtil.SafeTrim(targetColumnInfo["DATA_DEFAULT"]);

                if (!srcDataDefault.Equals(targetDataDefault) &&
                    !(IsNull(srcDataDefault) && IsNull(srcDataDefault)))
                // 特殊判斷, 將 NULL 字串也視為空 (已經設過 DEFAULT NULL 者, 雖然DEFAULT 預設不塞, 但撈出來為為 NULL 字串) -  一般判斷
                {
                    subFu.AppendLine("ALTER TABLE " + targetSchema + "." + tableName + " MODIFY (" + columnName +
                                     " DEFAULT " + (IsNull(srcDataDefault) ? "NULL " : srcDataDefault) + ");");
                }

                //
                if (StringUtil.NotEmpty(subFu.ToString()))
                {
                    fu.AppendLine("--異動前:[" +
                                  GetCloumSql(columnName, targetDataType, targetDataLength, targetDataPrecision,
                                      targetDataDefault, targetNullable, targetCharLength) + "]");
                    fu.AppendLine("--異動後:[" +
                                  GetCloumSql(columnName, srcDataType, srcDataLength, srcDataPrecision,
                                      srcDataDefault, srcNullable, srcCharLength) + "]");
                    fu.AppendLine(subFu.ToString());
                }
            }

            var result = fu.ToString();
            if (StringUtil.NotEmpty(result))
            {
                result = "--修改\r\n" + result;
            }
            return result;
        }

        private bool IsNull(string value)
        {
            if (StringUtil.IsEmpty(value))
            {
                return true;
            }
            if ("NULL".Equals(StringUtil.SafeTrim(value)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 產生 Create Table 的 SQL </summary>
        /// <param name="targetSchema"> </param>
        /// <param name="tableName"> </param>
        /// <param name="columnDataList"> </param>
        /// <param name="pKeySet">
        /// @return </param>
        private string GanCreateSql(string targetSchema, string tableName,
            List<Dictionary<string, object>> columnDataList, List<string> pKeySet)
        {
            //FileUtil fu = new FileUtil();
            var fu = new StringBuilder();

            fu.AppendLine("CREATE TABLE " + targetSchema + "." + tableName + " ");
            fu.AppendLine("( ");
            // 一般欄位
            for (var i = 0; i < columnDataList.Count; i++)
            {
                var columnData = columnDataList[i];
                var tableComments = StringUtil.SafeTrim(columnData["TABLE_COMMENTS"]);
                var columnName = StringUtil.SafeTrim(columnData["COLUMN_NAME"]);
                var dataType = StringUtil.SafeTrim(columnData["DATA_TYPE"]);
                var dataLength = StringUtil.SafeTrim(columnData["DATA_LENGTH"]);
                var dataPrecision = StringUtil.SafeTrim(columnData["DATA_PRECISION"]);
                var dataDefault = StringUtil.SafeTrim(columnData["DATA_DEFAULT"]);
                var nullable = StringUtil.SafeTrim(columnData["NULLABLE"]);
                var charLength = StringUtil.SafeTrim(columnData["CHAR_LENGTH"]);

                fu.Append("	" +
                          GetCloumSql(columnName, dataType, dataLength, dataPrecision, dataDefault, nullable,
                              charLength));

                if (i != columnDataList.Count || (i == columnDataList.Count && pKeySet.Count > 0))
                {
                    fu.Append(",");
                }
                fu.AppendLine("");

                // PK
                if (pKeySet.Count > 0)
                {
                    fu.Append("	CONSTRAINT PK_" + tableName + " PRIMARY KEY (");

                    var pkStr = "";

                    foreach (var pkey in pKeySet)
                    {
                        pkStr += "," + pkey;
                    }

                    fu.Append(pkStr.Substring(1));
                    fu.Append(")");
                    fu.AppendLine("");
                }
                else
                {
                }

                fu.AppendLine(");");
                fu.AppendLine("");
            }

            // 註解TODO
            //fu.AppendLine("COMMENT ON TABLE " + targetSchema + "." + tableName + " IS '" + tableComments + "';");
            foreach (var columnData in columnDataList)
            {
                var COLUMN_NAME = StringUtil.SafeTrim(columnData["COLUMN_NAME"]);
                var comments = StringUtil.SafeTrim(columnData["COMMENTS"]);

                fu.Append("COMMENT ON COLUMN ");
                fu.Append(targetSchema + ".");
                fu.Append(tableName + ".");
                fu.Append(COLUMN_NAME);
                fu.Append(" IS ");
                fu.Append("'" + comments + "';");
                fu.AppendLine();
            }

            return fu.ToString();
        }

        /// <summary>
        /// 產生欄位建置 SQL </summary>
        /// <param name="COLUMN_NAME"> </param>
        /// <param name="DATA_TYPE"> </param>
        /// <param name="DATA_LENGTH"> </param>
        /// <param name="DATA_PRECISION"> </param>
        /// <param name="DATA_DEFAULT"> </param>
        /// <param name="nullable">
        /// @return </param>
        private string GetCloumSql(string columnName, string dataType, string dataLength, string dataPrecision,
            string dataDefault, string nullable, string charLength)
        {
            var str = "";

            str += columnName;
            str += "	" + getType(dataType, dataLength, dataPrecision, charLength);
            if (StringUtil.NotEmpty(dataDefault))
            {
                str += "	DEFAULT	" + dataDefault;
            }
            if ("N".Equals(nullable))
            {
                str += "	NOT NULL";
            }
            return str;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="dataLength"></param>
        /// <param name="dataPrecision"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        private string getType(string dataType, string dataLength, string dataPrecision, string charLength)
        {
            if ("NUMBER".Equals(dataType.ToUpper()))
            {
                if (StringUtil.IsEmpty(dataPrecision))
                {
                    return "NUMBER";
                }
                return "NUMBER(" + dataPrecision + ")";
            }

            if ("NVARCHAR2".Equals(dataType.ToUpper()))
            {
                return dataType + "(" + charLength + ")";
            }

            if ("NVARCHAR2".Equals(dataType.ToUpper()))
            {
                return dataType + "(" + charLength + ")";
            }

            //為DATE時，無須長度
            if ("DATE".Equals(dataType.ToUpper()))
            {
                return "DATE";
            }

            //BLOB
            if ("BLOB".Equals(dataType.ToUpper()))
            {
                return "BLOB";
            }

            return dataType + "(" + dataLength + ")";
        }

        /// <param name="dbUtil"> </param>
        /// <param name="schema">
        /// @return </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public java.util.Dictionary<String, TableInfo> getAllTable(com.rbt.util.db.AbstractDBUtil dbUtil, String schema) throws Exception
        public virtual Dictionary<string, TableInfo> GetAllTable(SqlLiteDBUtil dbUtil, string schema)
        {
            // =======================================================
            // 查詢 所有的欄位資訊
            // =======================================================
            // 兜組SQL
            var columnsQSql = new StringBuilder();
            columnsQSql.Append("SELECT col.OWNER, ");
            columnsQSql.Append("	   col.TABLE_NAME, ");
            columnsQSql.Append("	   tabComt.COMMENTS AS TABLE_COMMENTS, ");
            columnsQSql.Append("	   col.COLUMN_ID, ");
            columnsQSql.Append("	   col.COLUMN_NAME, ");
            columnsQSql.Append("	   col.DATA_TYPE, ");
            columnsQSql.Append("	   col.DATA_LENGTH, ");
            columnsQSql.Append("	   col.DATA_PRECISION, ");
            columnsQSql.Append("	   col.DATA_DEFAULT, ");
            columnsQSql.Append("	   col.CHAR_LENGTH, ");
            columnsQSql.Append("	   col.NULLABLE, ");
            columnsQSql.Append("	   colComt.COMMENTS ");
            columnsQSql.Append("FROM   ALL_TAB_COLUMNS col ");
            columnsQSql.Append("	   JOIN ALL_TABLES tab ");
            columnsQSql.Append("		 ON col.OWNER = tab.OWNER ");
            columnsQSql.Append("			AND col.TABLE_NAME = tab.TABLE_NAME ");
            columnsQSql.Append("	   LEFT JOIN ALL_COL_COMMENTS colComt ");
            columnsQSql.Append("			  ON col.OWNER = colComt.Owner ");
            columnsQSql.Append("				 AND col.TABLE_NAME = colComt.TABLE_NAME ");
            columnsQSql.Append("				 AND col.COLUMN_NAME = colComt.COLUMN_NAME ");
            columnsQSql.Append("	   LEFT JOIN sys.USER_TAB_COMMENTS tabComt ");
            columnsQSql.Append("			  ON tabComt.TABLE_TYPE = 'TABLE' ");
            columnsQSql.Append("				 AND tabComt.TABLE_NAME = col.TABLE_NAME ");
            columnsQSql.Append("WHERE  col.OWNER = '" + schema + "' ");
            // 測試縮限範圍
            if (StringUtil.NotEmpty(_testTable))
            {
                columnsQSql.Append("AND  col.TABLE_NAME = '" + _testTable + "' ");
            }
            columnsQSql.Append("ORDER  BY col.TABLE_NAME, ");
            columnsQSql.Append("		  col.COLUMN_ID ");

            // 查詢所有的欄位List
            DataTable allColumnList = dbUtil.QueryDataTable(columnsQSql.ToString());
            // 依據 table 分類
            var tableColumnProcResult = Process(allColumnList);

            // =======================================================
            // 查詢 所有的欄位資訊
            // =======================================================
            var pKeyQSql = new StringBuilder();
            pKeyQSql.Append("SELECT C.OWNER, ");
            pKeyQSql.Append("	   C.TABLE_NAME, ");
            pKeyQSql.Append("	   D.POSITION, ");
            pKeyQSql.Append("	   D.COLUMN_NAME ");
            pKeyQSql.Append("FROM   ALL_CONSTRAINTS C ");
            pKeyQSql.Append("	   JOIN ALL_CONS_COLUMNS D ");
            pKeyQSql.Append("		 ON C.OWNER = D.OWNER ");
            pKeyQSql.Append("			AND C.CONSTRAINT_NAME = D.CONSTRAINT_NAME ");
            pKeyQSql.Append("WHERE  C.CONSTRAINT_TYPE = 'P' ");
            pKeyQSql.Append("	   AND C.OWNER = '" + schema + "' ");
            // 測試縮限範圍
            if (StringUtil.NotEmpty(_testTable))
            {
                columnsQSql.Append("AND  C.TABLE_NAME = '" + _testTable + "' ");
            }
            pKeyQSql.Append("ORDER  BY C.TABLE_NAME, ");
            pKeyQSql.Append("		  D.POSITION ");

            DataTable allPKeyColumnList = dbUtil.QueryDataTable(pKeyQSql.ToString());
            // 依據 table 分類
            var pkColumnProcResult = Process(allPKeyColumnList);

            // =======================================================
            // 組 tableInfo
            // =======================================================
            var tableInfoMap = new Dictionary<string, TableInfo>();

            foreach (var tableName in tableColumnProcResult.ColumnInfoListByTableName.Keys)
            {
                var tableInfo = new TableInfo(this);
                // 一般欄位
                tableInfo.columnDataMapList = tableColumnProcResult.ColumnInfoListByTableName[tableName];
                tableInfo.columnDataMapByColName = tableColumnProcResult.ColumnInfoByColNameTableName[tableName];
                tableInfo.columnNameSet = tableColumnProcResult.ColumnNameSetByTableName[tableName];
                // PK欄位
                tableInfo.PKeyDataMapList = pkColumnProcResult.ColumnInfoListByTableName[tableName];
                tableInfo.PKeySet = pkColumnProcResult.ColumnNameSetByTableName[tableName];
                // PUT
                tableInfoMap.Add(tableName, tableInfo);
            }
            return tableInfoMap;
        }

        private ProcResult Process(DataTable dataTable)
        {
            // 依據 table 分類資料
            var columnInfoListByTableName =
                new Dictionary<string, List<Dictionary<string, object>>>();
            var columnInfoByColNameTableName =
                new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var columnNameSetByTableName = new Dictionary<string, List<string>>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                // TABLE NAME
                var tableName = StringUtil.SafeTrim(dataRow["TABLE_NAME"]);
                // COLUMN_NAME
                var columnName = StringUtil.SafeTrim(dataRow["COLUMN_NAME"]);
                // 為空者略過
                if (StringUtil.IsEmpty(tableName))
                {
                    continue;
                }

                // 取得已收集的 List
                var columnDataMapList = columnInfoListByTableName[tableName];
                var columnDataMapByColName =
                    columnInfoByColNameTableName[tableName];
                var columnNameSet = columnNameSetByTableName[tableName];
                // 還不存在時初始化
                if (columnDataMapList == null)
                {
                    //
                    columnDataMapList = new List<Dictionary<string, object>>();
                    columnInfoListByTableName.Add(tableName, columnDataMapList);

                    columnDataMapByColName = new Dictionary<string, Dictionary<string, object>>();
                    columnInfoByColNameTableName.Add(tableName, columnDataMapByColName);

                    columnNameSet = new List<string>();
                    columnNameSetByTableName.Add(tableName, columnNameSet);
                }

                //TODO
                //columnDataMapList.Add(dataRow);
                //columnDataMapByColName.Add(columnName, dataRow);
                columnNameSet.Add(columnName);
            }

            return new ProcResult(this, columnInfoListByTableName, columnInfoByColNameTableName,
                columnNameSetByTableName);
        }

        /// <summary>
        /// @author Allen
        ///
        /// </summary>
        private class ProcResult
        {
            private readonly SqliteDif _outerInstance;

            //
            internal readonly Dictionary<string, List<Dictionary<string, object>>> ColumnInfoListByTableName;

            internal readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> ColumnInfoByColNameTableName;
            internal readonly Dictionary<string, List<string>> ColumnNameSetByTableName;

            public ProcResult(SqliteDif outerInstance,
                Dictionary<string, List<Dictionary<string, object>>> columnInfoListByTableName,
                Dictionary<string, Dictionary<string, Dictionary<string, object>>> columnInfoByColNameTableName,
                Dictionary<string, List<string>> columnNameSetMapByTableName)
            {
                _outerInstance = outerInstance;
                ColumnInfoListByTableName = columnInfoListByTableName;
                ColumnInfoByColNameTableName = columnInfoByColNameTableName;
                ColumnNameSetByTableName = columnNameSetMapByTableName;
            }
        }

        /// <summary>
        /// @author Allen
        ///
        /// </summary>
        internal sealed class TableInfo
        {
            private readonly SqliteDif _outerInstance;

            public TableInfo(SqliteDif outerInstance)
            {
                _outerInstance = outerInstance;
            }

            internal List<Dictionary<string, object>> columnDataMapList;
            internal Dictionary<string, Dictionary<string, object>> columnDataMapByColName;
            internal List<string> columnNameSet;
            internal List<Dictionary<string, object>> PKeyDataMapList;
            internal List<string> PKeySet;

            /// <returns> the columnDataMapList </returns>
            public List<Dictionary<string, object>> ColumnDataMapList
            {
                get { return columnDataMapList; }
                set { columnDataMapList = value; }
            }

            /// <returns> the pKeyDataMapList </returns>
            public List<Dictionary<string, object>> GetpKeyDataMapList()
            {
                return PKeyDataMapList;
            }

            /// <param name="pKeyDataMapList"> the pKeyDataMapList to set </param>
            public void SetpKeyDataMapList(List<Dictionary<string, object>> pKeyDataMapList)
            {
                PKeyDataMapList = pKeyDataMapList;
            }

            /// <returns> the pKeySet </returns>
            public List<string> GetpKeySet()
            {
                if (PKeySet == null)
                {
                    return new List<string>();
                }
                return PKeySet;
            }

            /// <param name="pKeySet"> the pKeySet to set </param>
            public void SetpKeySet(List<string> pKeySet)
            {
                PKeySet = pKeySet;
            }

            /// <returns> the columnDataMapByColName </returns>
            public Dictionary<string, Dictionary<string, object>> ColumnDataMapByColName
            {
                get { return columnDataMapByColName; }
                set { columnDataMapByColName = value; }
            }

            /// <returns> the columnNameSet </returns>
            public List<string> ColumnNameSet
            {
                get { return columnNameSet; }
                set { columnNameSet = value; }
            }
        }
    }
}