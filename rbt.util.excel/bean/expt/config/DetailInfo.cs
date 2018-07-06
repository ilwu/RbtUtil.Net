using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// Detail Config Info
    /// </summary>
    public class DetailInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /**
         * 設定ID
         */
        public string DataId { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /**
         * Column LIST
         */
        public List<ColumnInfo> ColumnInfoList { get; set; }
    }
}