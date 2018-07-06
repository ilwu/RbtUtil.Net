using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// "tr" tag Config Info
    /// </summary>
    public class TrInfo : AbstractStyleInfo
    {
        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// Sheet 設定 List
        /// </summary>
        public List<TdInfo> TdInfoList { get; set; }
    }
}