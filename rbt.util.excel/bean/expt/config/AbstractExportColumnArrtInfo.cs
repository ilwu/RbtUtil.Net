using rbt.util.excel.util;
using System;
using System.Xml;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// 抽象化類別：與 資料欄位 有關的屬性設定資訊
    /// </summary>
    public abstract class AbstractExportColumnArrtInfo : AbstractStyleInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        /// key 從 Dic 帶過來的 key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 要顯示的值 (若key 參數有設定時，優先使用 key)
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 要處理顯示值的 function id
        /// </summary>
        public string FuncId { get; set; }

        /// <summary>
        /// 功能帶入參數
        /// </summary>
        public string FuncParam { get; set; }

        /// <summary>
        /// 合併列數
        /// </summary>
        public int Colspan { get; set; }

        // =====================================================
        // 公用程式
        // =====================================================

        /// <summary>
        /// 讀取 Node 中,與 Data Column 類型元素 相關的屬性
        /// </summary>
        /// <param name="node">XmlNode</param>
        public void readDataColumnAttr(XmlNode node)
        {
            if (node == null || node.Attributes == null)
            {
                return;
            }
            //key
            Key = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_KEY);
            //funcId
            FuncId = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_FUNCID);

            //FuncParam
            FuncParam = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_FUNC_PARAM);

            //colspan
            Colspan = Convert.ToInt32(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_COLSPAN, "0"));

            // defaultValue (先讀取 node, node 無值時,  讀取 attr)
            XmlNode defaultValueNode = node.SelectSingleNode(Constant.ELEMENT_DEFAULT_VALUE);
            string defaultValue = "";
            if (defaultValueNode != null)
            {
                defaultValue = defaultValueNode.InnerText;
            }
            if (ExcelStringUtil.IsEmpty(defaultValue) && node.Attributes != null)
            {
                defaultValue = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_DEFAULT_VALUE);
            }
            DefaultValue = defaultValue;
        }
    }
}