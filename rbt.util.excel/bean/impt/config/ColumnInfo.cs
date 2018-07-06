namespace rbt.util.excel.bean.impt.config
{
    /// <summary>
    /// column Config Info
    /// @author Allen
    /// </summary>
    public class ColumnInfo : AbstractImportCommonAttrInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================

        // =====================================================
        // 元素子項目
        // =====================================================
        /// <summary>
        /// 要驗證的定義ID
        /// </summary>
        public virtual string FormatId { get; set; }

        /// <summary>
        /// 單欄定義自帶驗證
        /// </summary>
        public virtual string Regex { get; set; }

        /// <summary>
        /// 單欄定義自帶驗證錯誤訊息
        /// </summary>
        public virtual string RegexErrorMsg { get; set; }

        /// <summary>
        /// 是否可為空值
        /// </summary>
        public virtual string CheckNull { get; set; }

        /// <summary>
        /// 忽略此欄位
        /// </summary>
        public virtual bool Pass { get; set; }
    }
}