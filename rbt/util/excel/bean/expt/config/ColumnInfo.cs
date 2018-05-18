using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// column Config Info
    /// </summary>
    public class ColumnInfo : AbstractExportColumnArrtInfo
    {
        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// ColumnDetailList LIST
        /// </summary>
        public List<ColumnDetailInfo> ColumnDetailInfoList { get; set; }
    }
}