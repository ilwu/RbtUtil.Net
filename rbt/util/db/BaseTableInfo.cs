using System.Collections.Generic;

namespace rbt.util.db
{
    public abstract class BaseTableInfo
    {
        public IList<IDictionary<string, object>> ColumnDataMapList { get; set; }
        public IDictionary<string, IDictionary<string, object>> ColumnDataMapByColName { get; set; }
        public IList<string> ColumnNameSet { get; set; }
        public IList<IDictionary<string, object>> PKeyDataMapList { get; set; }
        public IList<string> PKeySet { get; set; }

        protected ProcResult Process(IList<IDictionary<string, object>> dataList)
        {
            // 依據 table 分類資料
            var columnInfoListByTableName =
                new Dictionary<string, IList<IDictionary<string, object>>>();
            var columnInfoByColNameTableName =
                new Dictionary<string, IDictionary<string, IDictionary<string, object>>>();
            var columnNameSetByTableName = new Dictionary<string, IList<string>>();

            foreach (var dataRow in dataList)
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
                if (!columnInfoListByTableName.ContainsKey(tableName))
                {
                    columnInfoListByTableName[tableName] = new List<IDictionary<string, object>>();
                }
                var columnDataMapList = columnInfoListByTableName[tableName];

                //
                if (!columnInfoByColNameTableName.ContainsKey(tableName))
                {
                    columnInfoByColNameTableName[tableName] = new Dictionary<string, IDictionary<string, object>>();
                }
                var columnDataMapByColName = columnInfoByColNameTableName[tableName];

                //
                if (!columnNameSetByTableName.ContainsKey(tableName))
                {
                    columnNameSetByTableName[tableName] = new List<string>();
                }
                var columnNameSet = columnNameSetByTableName[tableName];

                // 還不存在時初始化
                if (columnDataMapList == null)
                {
                    //
                    columnDataMapList = new List<IDictionary<string, object>>();
                    columnInfoListByTableName.Add(tableName, columnDataMapList);

                    columnDataMapByColName = new Dictionary<string, IDictionary<string, object>>();
                    columnInfoByColNameTableName.Add(tableName, columnDataMapByColName);

                    columnNameSet = new List<string>();
                    columnNameSetByTableName.Add(tableName, columnNameSet);
                }
                columnDataMapList.Add(dataRow);
                columnDataMapByColName.Add(columnName, dataRow);
                columnNameSet.Add(columnName);
            }

            return new ProcResult(columnInfoListByTableName, columnInfoByColNameTableName,
                columnNameSetByTableName);
        }

        /// <summary>
        /// @author Allen
        ///
        /// </summary>
        protected class ProcResult
        {
            internal readonly Dictionary<string, IList<IDictionary<string, object>>> ColumnInfoListByTableName;
            internal readonly Dictionary<string, IDictionary<string, IDictionary<string, object>>> ColumnInfoByColNameTableName;
            internal readonly Dictionary<string, IList<string>> ColumnNameSetByTableName;

            public ProcResult(
                Dictionary<string, IList<IDictionary<string, object>>> columnInfoListByTableName,
                Dictionary<string, IDictionary<string, IDictionary<string, object>>> columnInfoByColNameTableName,
                Dictionary<string, IList<string>> columnNameSetMapByTableName)
            {
                ColumnInfoListByTableName = columnInfoListByTableName;
                ColumnInfoByColNameTableName = columnInfoByColNameTableName;
                ColumnNameSetByTableName = columnNameSetMapByTableName;
            }
        }
    }
}