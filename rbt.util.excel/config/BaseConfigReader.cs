using rbt.util.excel.bean.common;
using rbt.util.excel.exception;
using rbt.util.excel.util;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace rbt.util.excel.config
{
    /// <summary>
    ///     設定檔讀取, 共通 (base)
    /// </summary>
    public class BaseConfigReader
    {
        /// <summary>
        ///     讀取設定檔案
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        protected XmlDocument readConfigFile(string configFilePath)
        {
            configFilePath = configFilePath.Trim();

            if (!File.Exists(configFilePath))
            {
                throw new ExcelOperateException("設定檔案不存在：[" + configFilePath + "]");
            }

            // =========================================================
            // 讀取設定檔案
            // =========================================================
            var document = new XmlDocument();

            // =========================================================
            // 讀取 Document
            // =========================================================
            document.Load(configFilePath);

            return document;
        }

        /// <summary>
        ///     讀取 function 標籤設定
        /// </summary>
        /// <param name="document"> XmlDocument </param>
        /// <returns></returns>
        protected Dictionary<string, FunctionInfo> readFunctionInfo(XmlDocument document)
        {
            // =========================================================
            // 讀取 function 設定
            // =========================================================
            XmlNodeList functionNodeList = document.SelectNodes("//" + Constant.ELEMENT_FUNCTION);

            // =========================================================
            // 解析 NODE 設定
            // =========================================================
            var functionInfoMap = new Dictionary<string, FunctionInfo>();

            if (functionNodeList == null)
            {
                return functionInfoMap;
            }
            foreach (XmlNode funcNode in functionNodeList)
            {
                var functionInfo = new FunctionInfo();
                if (funcNode.Attributes == null)
                {
                    continue;
                }
                functionInfo.FuncId = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_FUNCID);
                functionInfo.ClassName = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_CLASSNAME);
                functionInfo.Method = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_METHOD);
                functionInfoMap.Add(functionInfo.FuncId, functionInfo);
            }

            return functionInfoMap;
        }
    }
}