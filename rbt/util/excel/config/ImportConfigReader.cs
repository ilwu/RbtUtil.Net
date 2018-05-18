using rbt.util.excel.bean.impt.config;
using rbt.util.excel.exception;
using rbt.util.excel.util;
using System;
using System.Collections.Generic;
using System.Xml;

namespace rbt.util.excel.config
{
    /// <summary>
    /// 讀取設定資訊
    /// @author Allen
    /// </summary>
    public class ImportConfigReader : BaseConfigReader
    {
        /// <summary>
        /// 依據ID讀取設定檔案
        /// </summary>
        /// <param name="configFilePath"> 設定檔案完整路徑 </param>
        /// <param name="configID"> 設定資訊 ID
        /// @return </param>
        public ImportConfigInfo read(string configFilePath, string configID)
        {
            // =========================================================
            // 讀取設定檔案
            // =========================================================
            XmlDocument document = this.readConfigFile(configFilePath);

            // =========================================================
            // 讀取 ImportConfigInfo
            // =========================================================
            ImportConfigInfo importConfigInfo = this.readExcelInfo(document, configID);

            // =========================================================
            // 讀取 FormatInfo
            // =========================================================
            importConfigInfo.FunctionInfoMap = this.readFunctionInfo(document);

            // =========================================================
            // 讀取 FormatInfo
            // =========================================================
            importConfigInfo.FormatInfoMap = this.readFormatInfo(document);

            return importConfigInfo;
        }

        /// <summary>
        /// ExportConfigInfo
        /// </summary>
        /// <param name="document"> </param>
        /// <param name="id"></param>
        private ImportConfigInfo readExcelInfo(XmlDocument document, string id)
        {
            // =========================================================
            // 讀取設定資訊
            // =========================================================
            XmlNode excelNode = document.SelectSingleNode(
                                                "//" + Constant.ELEMENT_EXCEL +
                                                "[@" + Constant.ATTRIBUTE_ID + "=\"" + id + "\"]");

            //XmlNode excelNode = document.SelectSingleNode("//" + Constant.ELEMENT_EXCEL + "[" + Constant.ATTRIBUTE_ID + "=\"" + configID + "\"]");
            if (excelNode == null)
            {
                throw new ExcelOperateException("設定資訊:[" + id + "] 不存在!");
            }

            // =========================================================
            // 讀取 excel 標籤屬性
            // =========================================================
            ImportConfigInfo importConfigInfo = new ImportConfigInfo();

            //configID
            importConfigInfo.Desc = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_ID);

            //sheetNum
            string sheetNum = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_SHEETNUM, "1");
            if (!ExcelStringUtil.isNumber(sheetNum))
            {
                throw new ExcelOperateException("屬性 sheetNum 設定錯誤! sheetNum:[" + sheetNum + "]");
            }
            importConfigInfo.SheetNum = Convert.ToInt32(sheetNum);

            //startRow
            string startRow = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_STARTROW);
            if (!ExcelStringUtil.isNumber(startRow))
            {
                throw new ExcelOperateException("屬性 startRow 設定錯誤! startRow:[" + startRow + "]");
            }
            importConfigInfo.StartRow = Convert.ToInt32(startRow);
            //CheckEmptyRow
            importConfigInfo.CheckEmptyRow = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_CHECK_EMPTY_ROW);
            //check duplicate
            importConfigInfo.CheckDuplicate = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_CHECK_DUPLICATE);
            //desc
            importConfigInfo.Desc = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_DESC);

            // =========================================================
            // 讀取 excel/read 標籤 下的 column
            // =========================================================
            //讀取 node list
            XmlNodeList columnNodeList = excelNode.SelectNodes(Constant.ELEMENT_READ + "/" + Constant.ELEMENT_COLUMN);
            //檢核
            if (ExcelStringUtil.IsEmpty(columnNodeList))
            {
                throw new ExcelOperateException("未找到任何 <column> (config/excel/read/column)");
            }

            //收集屬性設定list
            IList<ColumnInfo> columnInfoList = new List<ColumnInfo>();
            importConfigInfo.ColumnInfoList = columnInfoList;
            //紀錄KEY避免重複
            var keySet = new HashSet<string>();

            //逐筆讀取
            if (columnNodeList != null)
            {
                foreach (XmlNode columnNode in columnNodeList)
                {
                    //未設定 key 代表要略過的欄位
                    //if (ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_KEY, "@@xx") == "@@xx")
                    //{
                    //    continue;
                    //};

                    //初始化物件
                    ColumnInfo columnInfo = new ColumnInfo();
                    //讀取共通屬性
                    columnInfo.readCommonAttr(columnNode);
                    //FormatId
                    columnInfo.FormatId = ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_FORMATID);
                    //regexp
                    columnInfo.Regex = ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_REGEX);
                    //RegexErrorMsg
                    columnInfo.RegexErrorMsg = ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_REGEX_ERROR_MSG);
                    //isNull
                    columnInfo.CheckNull = ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_CHECK_NULL);
                    //pass
                    columnInfo.Pass = "true".Equals(ExcelStringUtil.GetNodeAttr(columnNode, Constant.ATTRIBUTE_PASS), StringComparison.CurrentCultureIgnoreCase);
                    //add to list
                    columnInfoList.Add(columnInfo);
                    //檢核Key 是否重複
                    this.checkKey(keySet, columnInfo.Key);
                }
            }

            // =========================================================
            // 讀取 excel/params 標籤 下的 param
            // =========================================================
            //讀取 node list
            XmlNodeList paramNodeList = excelNode.SelectNodes(Constant.ELEMENT_PARAMS + "/" + Constant.ELEMENT_PARAM);

            //收集屬性設定list
            IList<ParamInfo> paramInfoList = new List<ParamInfo>();
            importConfigInfo.ParamInfoList = paramInfoList;

            //逐筆讀取
            if (paramNodeList != null)
            {
                foreach (XmlNode paramNode in paramNodeList)
                {
                    //初始化物件
                    ParamInfo paramInfo = new ParamInfo();
                    //讀取共通屬性
                    paramInfo.readCommonAttr(paramNode);
                    //add to list
                    paramInfoList.Add(paramInfo);
                    //檢核Key 是否重複
                    this.checkKey(keySet, paramInfo.Key);
                }
            }

            return importConfigInfo;
        }

        /// <summary>
        ///     讀取 format 標籤設定 (正規表示式檢核設定)
        /// </summary>
        /// <param name="document"> XmlDocument </param>
        /// <returns></returns>
        protected Dictionary<string, FormatInfo> readFormatInfo(XmlDocument document)
        {
            // =========================================================
            // 讀取 function 設定
            // =========================================================
            XmlNodeList formatNodeList = document.SelectNodes("//" + Constant.ELEMENT_FORMAT);

            // =========================================================
            // 解析 NODE 設定
            // =========================================================
            var formatInfoMap = new Dictionary<string, FormatInfo>();

            if (formatNodeList == null)
            {
                return formatInfoMap;
            }
            foreach (XmlNode funcNode in formatNodeList)
            {
                var formatInfo = new FormatInfo();
                if (funcNode.Attributes == null)
                {
                    continue;
                }
                formatInfo.FormatId = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_FORMATID);
                formatInfo.Regex = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_REGEX);
                formatInfo.RegexErrorMsg = ExcelStringUtil.GetNodeAttr(funcNode, Constant.ATTRIBUTE_REGEX_ERROR_MSG);
                formatInfoMap.Add(formatInfo.FormatId, formatInfo);
            }

            return formatInfoMap;
        }

        /// <summary>
        /// 檢核Key 是否重複 </summary>
        /// <param name="keySet"> </param>
        /// <param name="key"> </param>
        private void checkKey(HashSet<string> keySet, string key)
        {
            if (ExcelStringUtil.IsEmpty(key))
            {
                throw new ExcelOperateException("有<column> 的 key 未設定 (為空)");
            }

            if (keySet.Contains(key))
            {
                throw new ExcelOperateException("<column> key:[" + key + "] 重複設定");
            }
            keySet.Add(key);
        }

        public static void Main(string[] args)
        {
            //ImportConfigInfo importConfigInfo = (new ImportConfigReader()).read("C:/workspace/RbtProject/srcExcelOpt/import_sample.xml", "LaborInsurance");
            //Console.WriteLine();
        }
    }
}