using System.Xml;

namespace rbt.util.excel.util
{
    /// <summary>
    ///     字串處理工具
    ///     @author Allen Wu
    /// </summary>
    public class ExcelStringUtil : StringUtil
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="nullDefault"></param>
        /// <returns></returns>
        public static string GetNodeAttr(XmlNode node, string attrName, string nullDefault = "")
        {
            //傳入為空檢核
            if (node == null || node.Attributes == null)
            {
                return "";
            }
            //依據屬性名稱取得子節點
            var childNode = node.Attributes.GetNamedItem(attrName);
            if (childNode == null)
            {
                return nullDefault;
            }
            //回傳
            return SafeTrim(childNode.Value);
        }
    }
}