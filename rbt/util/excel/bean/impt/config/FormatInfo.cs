namespace rbt.util.excel.bean.impt.config
{
    /// <summary>
    /// format Config Info
    /// @author Allen
    /// </summary>
    public class FormatInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// format 設定 ID
        /// </summary>
        public virtual string FormatId { get; set; }

        /// <summary>
        /// 檢核之正規表示式
        /// </summary>
        public virtual string Regex { get; set; }

        /// <summary>
        /// 單欄定義自帶驗證錯誤訊息
        /// </summary>
        public virtual string RegexErrorMsg { get; set; }
    }
}