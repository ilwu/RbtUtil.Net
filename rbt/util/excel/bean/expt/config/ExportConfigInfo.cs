using CSharpJExcel.Jxl.Format;
using rbt.util.excel.bean.common;
using System.Collections.Generic;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// 匯出 CONFIG 設定
    /// </summary>
    public class ExportConfigInfo : AbstractConfigInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// 輸出檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 頁面大小
        /// </summary>
        public PaperSize PaperSize { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// 範圍 style 設定
        /// </summary>
        public StyleInfo StyleInfo { get; set; }

        /// <summary>
        /// Sheet 設定 List
        /// </summary>
        public List<SheetlInfo> SheetList { get; set; }
    }
}