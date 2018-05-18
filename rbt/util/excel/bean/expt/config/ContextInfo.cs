using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// Context Config Info
    /// </summary>
    public class ContextInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// 設定ID
        /// </summary>
        public string DataId { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// TR LIST
        /// </summary>
        public List<TrInfo> TrInfoList { get; set; }
    }
}