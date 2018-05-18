using System.Collections.Generic;

namespace rbt.util.db.model
{
    /// <summary>
    /// Table 資訊
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Table 代碼
        /// </summary>
        public string TABLE_NAME { get; set; }

        /// <summary>
        /// Table 名稱、註解
        /// </summary>
        public string TABLE_COMMENT { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> ColumnNameList { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Dictionary<string, ColumnInfo> ColumnInfoDic { get; set; }
    }

    /// <summary>
    /// COLUMN 資訊
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// COLUMN 代碼
        /// </summary>
        public string COLUMN_NAME { get; set; }

        /// <summary>
        /// COLUMN 名稱、註解
        /// </summary>
        public string COLUMN_COMMENT { get; set; }

        /// <summary>
        /// 資料類型
        /// </summary>
        public string DATA_TYPE { get; set; }

        /// <summary>
        /// 資料長度
        /// </summary>
        public int DATA_LENGTH { get; set; }

        public string DATA_DEFAULT { get; set; }

        /// <summary>
        /// 是否可為空
        /// </summary>
        public bool isNullable { get; set; }

        public bool isPK { get; set; }

        public int NUMBER_PRECISION { get; set; }
        public int NUMBER_SCALE { get; set; }
    }
}