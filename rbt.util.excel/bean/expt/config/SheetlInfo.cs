using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    ///
    /// </summary>
    public class SheetlInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// 設定ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 顯示 Sheet 名稱
        /// </summary>
        public string SheetName { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// (context | detail)*
        /// </summary>
        public Dictionary<string, object> PartInfoMap { get; set; }
    }
}