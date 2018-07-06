using rbt.util.excel.util;
using System.Xml;

namespace rbt.util.excel.bean.impt.config
{
    /// <summary>
    /// import 共通屬性設定資訊
    /// @author Allen
    /// </summary>
    public abstract class AbstractImportCommonAttrInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================

        // =====================================================
        // 公用程式
        // =====================================================
        /// <summary>
        /// 讀取 Node 中,共通屬性 </summary>
        /// <param name="node"> </param>
        public virtual void readCommonAttr(XmlNode node)
        {
            if (node == null)
            {
                return;
            }
            // key
            this.Key = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_KEY);
            // desc
            this.Desc = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_DESC);
            // funcId
            this.FuncId = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_FUNCID);
            // index
            this.Index = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_INDEX);

            // defaultValue (先讀取 node, node 無值時,  讀取 attr)
            XmlNode defaultValueNode = node.SelectSingleNode(Constant.ELEMENT_DEFAULT_VALUE);

            string defaultValue = "";
            if (defaultValueNode != null)
            {
                defaultValue = defaultValueNode.Value;
            }
            if (ExcelStringUtil.IsEmpty(defaultValue))
            {
                defaultValue = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_DEFAULT_VALUE);
            }
            this.DefaultValue = defaultValue;

            // funcParam (先讀取 node, node 無值時,  讀取 attr)
            XmlNode funcParamNode = node.SelectSingleNode(Constant.ELEMENT_FUNC_PARAM);
            string funcParam = "";
            if (funcParamNode != null)
            {
                funcParam = funcParamNode.Value;
            }
            if (ExcelStringUtil.IsEmpty(funcParam))
            {
                funcParam = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_FUNC_PARAM);
            }
            this.FuncParam = funcParam;
        }

        // =====================================================
        // gatter & setter
        // =====================================================

        // key 回傳map 物件的 map key
        public string Key { get; set; }

        // 預設值
        public string DefaultValue { get; set; }

        // 欄位說明，非必要，錯誤時顯示名稱，及備註用
        public string Desc { get; set; }

        // 要處理輸入值的 function id
        public string FuncId { get; set; }

        // 功能帶入參數
        public string FuncParam { get; set; }

        // 參數排列順序
        public string Index { get; set; }
    }
}