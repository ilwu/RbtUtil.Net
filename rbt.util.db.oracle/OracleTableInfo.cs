using System.Collections.Generic;
using System.Text;

namespace rbt.util.db.oracle
{
    public class OracleTableInfo : BaseTableInfo
    {
        public Dictionary<string, OracleTableInfo> GetAllTable(OracleDBUtil dbUtil, string schema, string testModelTable = "")
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
            columnsQSql.Append("	   col.DATA_SCALE, ");
            columnsQSql.Append("	   col.DATA_DEFAULT, ");
            columnsQSql.Append("	   col.CHAR_LENGTH, ");
            columnsQSql.Append("	   col.NULLABLE, ");
            columnsQSql.Append("	   colComt.COMMENTS ");
            columnsQSql.Append("FROM   ALL_TAB_COLUMNS col ");
            //columnsQSql.Append("	   JOIN ALL_TABLES tab ");
            //columnsQSql.Append("		 ON col.OWNER = tab.OWNER ");
            //columnsQSql.Append("			AND col.TABLE_NAME = tab.TABLE_NAME ");
            columnsQSql.Append("	   LEFT JOIN ALL_COL_COMMENTS colComt ");
            columnsQSql.Append("			  ON col.OWNER = colComt.Owner ");
            columnsQSql.Append("				 AND col.TABLE_NAME = colComt.TABLE_NAME ");
            columnsQSql.Append("				 AND col.COLUMN_NAME = colComt.COLUMN_NAME ");
            columnsQSql.Append("	   LEFT JOIN sys.USER_TAB_COMMENTS tabComt ");
            columnsQSql.Append("			  ON tabComt.TABLE_TYPE in ('TABLE','VIEW') "); //改為連 View 一起撈
            columnsQSql.Append("				 AND tabComt.TABLE_NAME = col.TABLE_NAME ");
            columnsQSql.Append("WHERE  col.OWNER = '" + schema + "' ");
            // 測試縮限範圍
            if (StringUtil.NotEmpty(testModelTable))
            {
                columnsQSql.Append("AND  col.TABLE_NAME = '" + testModelTable + "' ");
            }
            columnsQSql.Append("ORDER  BY col.TABLE_NAME, ");
            columnsQSql.Append("		  col.COLUMN_ID ");

            // 查詢所有的欄位List
            var allColumnList = dbUtil.Query(columnsQSql.ToString());
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
            if (StringUtil.NotEmpty(testModelTable))
            {
                columnsQSql.Append("AND  C.TABLE_NAME = '" + testModelTable + "' ");
            }
            pKeyQSql.Append("ORDER  BY C.TABLE_NAME, ");
            pKeyQSql.Append("		  D.POSITION ");

            var allPKeyColumnList = dbUtil.Query(pKeyQSql.ToString());
            // 依據 table 分類
            var pkColumnProcResult = Process(allPKeyColumnList);

            // =======================================================
            // 組 tableInfo
            // =======================================================
            var tableInfoMap = new Dictionary<string, OracleTableInfo>();

            foreach (var tableName in tableColumnProcResult.ColumnInfoListByTableName.Keys)
            {
                var tableInfo = new OracleTableInfo();
                // 一般欄位
                tableInfo.ColumnDataMapList = tableColumnProcResult.ColumnInfoListByTableName[tableName];
                tableInfo.ColumnDataMapByColName = tableColumnProcResult.ColumnInfoByColNameTableName[tableName];
                tableInfo.ColumnNameSet = tableColumnProcResult.ColumnNameSetByTableName[tableName];
                // PK欄位

                if (pkColumnProcResult.ColumnInfoListByTableName.ContainsKey(tableName))
                {
                    tableInfo.PKeyDataMapList = pkColumnProcResult.ColumnInfoListByTableName[tableName];
                    tableInfo.PKeySet = pkColumnProcResult.ColumnNameSetByTableName[tableName];
                }
                else
                {
                    tableInfo.PKeyDataMapList = new List<IDictionary<string, object>>();
                    tableInfo.PKeySet = new List<string>();
                }

                // PUT
                tableInfoMap.Add(tableName, tableInfo);
            }
            return tableInfoMap;
        }
    }
}