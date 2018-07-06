using rbt.util.excel.bean.common;
using System.Collections.Generic;

namespace rbt.util.excel.bean.impt.config
{
    /// <summary>
    /// @author Allen
    /// </summary>
    public class ImportConfigInfo : AbstractConfigInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================

        /// <summary>
        /// 讀取的 sheet index
        /// </summary>
        public virtual int SheetNum { get; set; }

        /// <summary>
        /// 讀取起始行數
        /// </summary>
        public virtual int StartRow { get; set; }

        /// <summary>
        /// 檢核空行
        /// </summary>
        public virtual string CheckEmptyRow { get; set; }

        /// <summary>
        /// 重複資料檢核欄位 (key, 以逗點分隔)
        /// </summary>
        public virtual string CheckDuplicate { get; set; }

        /// <summary>
        /// 欄位說明，非必要
        /// </summary>
        public virtual string Desc { get; set; }

        // =====================================================
        // 元素子項目
        // =====================================================

        /// <summary>
        /// functionInfo 設定
        /// </summary>
        public virtual Dictionary<string, FormatInfo> FormatInfoMap { get; set; }

        /// <summary>
        /// column 設定 List
        /// </summary>
        public virtual IList<ColumnInfo> ColumnInfoList { get; set; }

        /// <summary>
        /// column 設定 List
        /// </summary>
        public virtual IList<ParamInfo> ParamInfoList { get; set; }
    }
}