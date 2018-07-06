using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// ColumnDetail Config Info
    /// </summary>
    public class ColumnDetailInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// 欄位類型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 對應 DataId
        /// </summary>
        public string DataId { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// 元素子項目
        /// </summary>
        public List<ColumnInfo> ColumnInfoList { get; set; }
    }
}