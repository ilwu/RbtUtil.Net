using rbt.util.db.model;
using System.Collections.Generic;
using System.Text;

namespace rbt.util.db.sql
{
    public class SqlTableInfo : BaseTableInfo
    {
        public Dictionary<string, TableInfo> GetAllTableInfo(SqlDBUtil dbUtil, object[] queryInfo, string testModelTable = "")
        {
            // =======================================================
            // 查詢 所有的欄位資訊
            // =======================================================
            // 兜組SQL
            StringBuilder columnsQSql = new StringBuilder();
            columnsQSql.Append("SELECT a.TABLE_NAME                                                                                AS TABLE_NAME, \n");
            columnsQSql.Append("       (SELECT Cast([value] AS VARCHAR(500)) \n");
            columnsQSql.Append("        FROM   Fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, NULL, NULL)) AS TABLE_COMMENT, \n");
            columnsQSql.Append("       b.COLUMN_NAME                                                                               AS COLUMN_NAME, \n");
            columnsQSql.Append("       b.DATA_TYPE                                                                                 AS DATA_TYPE, \n");
            columnsQSql.Append("       b.CHARACTER_MAXIMUM_LENGTH                                                                  AS DATA_LENGTH, \n");
            columnsQSql.Append("       b.COLUMN_DEFAULT                                                                            AS DATA_DEFAULT, \n");
            columnsQSql.Append("       b.NUMERIC_PRECISION, \n");
            columnsQSql.Append("       b.NUMERIC_SCALE, \n");
            columnsQSql.Append("       b.IS_NULLABLE                                                                               AS NULLABLE, \n");
            columnsQSql.Append("       (SELECT Cast([value] AS VARCHAR(500)) \n");
            columnsQSql.Append("        FROM   Fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, 'column', DEFAULT) \n");
            columnsQSql.Append("        WHERE  name = 'MS_Description' \n");
            columnsQSql.Append("               AND objtype = 'COLUMN' \n");
            columnsQSql.Append("               AND objname COLLATE Chinese_Taiwan_Stroke_CI_AS = b.COLUMN_NAME)                    AS COLUMN_COMMENT, \n");
            columnsQSql.Append("       (SELECT Objectproperty(Object_id(c.CONSTRAINT_SCHEMA + '.' \n");
            columnsQSql.Append("                                        + Quotename(c.CONSTRAINT_NAME)), 'IsPrimaryKey') \n");
            columnsQSql.Append("        FROM   INFORMATION_SCHEMA.KEY_COLUMN_USAGE c \n");
            columnsQSql.Append("        WHERE  a.TABLE_NAME = c.TABLE_NAME \n");
            columnsQSql.Append("               AND b.COLUMN_NAME = c.COLUMN_NAME \n");
            columnsQSql.Append("               AND a.TABLE_CATALOG = b.TABLE_CATALOG \n");
            columnsQSql.Append("               AND a.TABLE_SCHEMA = b.TABLE_SCHEMA)                                                AS IS_PK \n");
            columnsQSql.Append("FROM   INFORMATION_SCHEMA.TABLES a \n");
            columnsQSql.Append("       LEFT JOIN INFORMATION_SCHEMA.COLUMNS b \n");
            columnsQSql.Append("              ON a.TABLE_NAME = b.TABLE_NAME \n");
            columnsQSql.Append("                 AND a.TABLE_CATALOG = b.TABLE_CATALOG \n");
            columnsQSql.Append("                 AND a.TABLE_SCHEMA = b.TABLE_SCHEMA \n");
            columnsQSql.Append("WHERE  TABLE_TYPE = 'BASE TABLE' \n");
            columnsQSql.Append("    AND  a.TABLE_CATALOG = '" + queryInfo[0] + "' \n");
            columnsQSql.Append("    AND  a.TABLE_SCHEMA = '" + queryInfo[1] + "' \n");

            // 測試縮限範圍
            if (StringUtil.NotEmpty(testModelTable))
            {
                columnsQSql.Append("AND  a.TABLE_NAME = '" + testModelTable + "' ");
            }
            columnsQSql.Append("ORDER  BY a.TABLE_NAME, \n");
            columnsQSql.Append("          ordinal_position ");

            // =======================================================
            // 查詢
            // =======================================================
            // 查詢所有的欄位List
            var allColumnList = dbUtil.Query(columnsQSql.ToString());

            // =======================================================
            // 資訊整理
            // =======================================================
            var tableDic = new Dictionary<string, TableInfo>();

            foreach (var columnData in allColumnList)
            {
                //====================
                //處理 table 資訊
                //====================
                var TABLE_NAME = StringUtil.SafeTrim(columnData["TABLE_NAME"]);
                //取得 TABLE_INFO 容器
                TableInfo tableInfo = null;
                //不存在時新增
                if (!tableDic.ContainsKey(TABLE_NAME))
                {
                    //初始化
                    tableInfo = new TableInfo();
                    tableInfo.TABLE_NAME = TABLE_NAME;
                    tableInfo.TABLE_COMMENT = StringUtil.SafeTrim(columnData["TABLE_COMMENT"]);
                    tableInfo.ColumnInfoDic = new Dictionary<string, ColumnInfo>();
                    tableInfo.ColumnNameList = new List<string>();
                    //新增
                    tableDic[TABLE_NAME] = tableInfo;
                }
                else
                {
                    tableInfo = tableDic[TABLE_NAME];
                }

                //====================
                //處理 column 資訊
                //====================
                var COLUMN_NAME = StringUtil.SafeTrim(columnData["COLUMN_NAME"]);
                //取得 COLUMN_INFO 容器
                ColumnInfo columnInfo = null;

                //不存在時新增
                if (!tableInfo.ColumnInfoDic.ContainsKey(COLUMN_NAME))
                {
                    //初始化
                    columnInfo = new ColumnInfo();
                    columnInfo.COLUMN_NAME = COLUMN_NAME;
                    columnInfo.COLUMN_COMMENT = StringUtil.SafeTrim(columnData["COLUMN_COMMENT"]);
                    columnInfo.DATA_TYPE = StringUtil.SafeTrim(columnData["DATA_TYPE"]);
                    columnInfo.DATA_LENGTH = int.Parse(StringUtil.SafeTrim(columnData["DATA_LENGTH"], "0"));
                    columnInfo.NUMBER_PRECISION = int.Parse(StringUtil.SafeTrim(columnData["NUMERIC_PRECISION"], "0"));
                    columnInfo.NUMBER_SCALE = int.Parse(StringUtil.SafeTrim(columnData["NUMERIC_SCALE"], "0"));
                    columnInfo.isNullable = "YES".Equals(StringUtil.SafeTrim(columnData["NULLABLE"]));
                    columnInfo.isPK = "1".Equals(StringUtil.SafeTrim(columnData["IS_PK"]));

                    //加入資料集
                    tableInfo.ColumnInfoDic[COLUMN_NAME] = columnInfo;
                    tableInfo.ColumnNameList.Add(COLUMN_NAME);
                }
            }

            return tableDic;
        }

        public Dictionary<string, SqlTableInfo> GetAllTable(SqlDBUtil dbUtil, string schema, string testModelTable = "")
        {
            // =======================================================
            // 查詢 所有的欄位資訊
            // =======================================================
            // 兜組SQL
            StringBuilder columnsQSql = new StringBuilder();
            columnsQSql.Append("SELECT a.TABLE_NAME                                                                                AS TABLE_NAME, \n");
            columnsQSql.Append("       (SELECT Cast([value] AS VARCHAR(500)) \n");
            columnsQSql.Append("        FROM   Fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, NULL, NULL)) AS TABLE_COMMENT, \n");
            columnsQSql.Append("       b.COLUMN_NAME                                                                               AS COLUMN_NAME, \n");
            columnsQSql.Append("       b.DATA_TYPE                                                                                 AS DATA_TYPE, \n");
            columnsQSql.Append("       b.CHARACTER_MAXIMUM_LENGTH                                                                  AS DATA_LENGTH, \n");
            columnsQSql.Append("       b.COLUMN_DEFAULT                                                                            AS DATA_DEFAULT, \n");
            columnsQSql.Append("       b.IS_NULLABLE                                                                               AS NULLABLE, \n");
            columnsQSql.Append("       (SELECT Cast([value] AS VARCHAR(500)) \n");
            columnsQSql.Append("        FROM   Fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, 'column', DEFAULT) \n");
            columnsQSql.Append("        WHERE  name = 'MS_Description' \n");
            columnsQSql.Append("               AND objtype = 'COLUMN' \n");
            columnsQSql.Append("               AND objname COLLATE Chinese_Taiwan_Stroke_CI_AS = b.COLUMN_NAME)                    AS COLUMN_COMMENT, \n");
            columnsQSql.Append("       (SELECT Objectproperty(Object_id(c.CONSTRAINT_SCHEMA + '.' \n");
            columnsQSql.Append("                                        + Quotename(c.CONSTRAINT_NAME)), 'IsPrimaryKey') \n");
            columnsQSql.Append("        FROM   INFORMATION_SCHEMA.KEY_COLUMN_USAGE c \n");
            columnsQSql.Append("        WHERE  a.TABLE_NAME = c.TABLE_NAME \n");
            columnsQSql.Append("               AND b.COLUMN_NAME = c.COLUMN_NAME)                                                  AS IS_PK \n");
            columnsQSql.Append("FROM   INFORMATION_SCHEMA.TABLES a \n");
            columnsQSql.Append("       LEFT JOIN INFORMATION_SCHEMA.COLUMNS b \n");
            columnsQSql.Append("              ON ( a.TABLE_NAME = b.TABLE_NAME ) \n");
            columnsQSql.Append("WHERE  TABLE_TYPE = 'BASE TABLE' \n");
            // 測試縮限範圍
            if (StringUtil.NotEmpty(testModelTable))
            {
                columnsQSql.Append("AND  a.TABLE_NAME = '" + testModelTable + "' ");
            }
            columnsQSql.Append("ORDER  BY a.TABLE_NAME, \n");
            columnsQSql.Append("          ordinal_position ");

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
            var tableInfoMap = new Dictionary<string, SqlTableInfo>();

            foreach (var tableName in tableColumnProcResult.ColumnInfoListByTableName.Keys)
            {
                var tableInfo = new SqlTableInfo();
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