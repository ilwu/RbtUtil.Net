using System.Collections.Generic;

namespace rbt.util.excel.bean.common
{
    /// <summary>
    ///
    /// </summary>
    public abstract class AbstractConfigInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// 設定ID
        /// </summary>
        public string Id { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// functionInfo 設定
        /// </summary>
        public Dictionary<string, FunctionInfo> FunctionInfoMap { get; set; }
    }
}