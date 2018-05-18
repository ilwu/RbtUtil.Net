using CSharpJExcel.Jxl.Format;
using rbt.util.excel.bean.expt.config;
using rbt.util.excel.exception;
using rbt.util.excel.util;
using System;
using System.Collections.Generic;
using System.Xml;

namespace rbt.util.excel.config
{
    /// <summary>
    /// 讀取設定資訊
    /// </summary>
    public class ExportConfigReader : BaseConfigReader
    {
        /// <summary>
        /// 依據ID讀取設定檔案
        /// </summary>
        /// <param name="configFilePath">設定檔案完整路徑</param>
        /// <param name="id">設定資訊 ID</param>
        /// <returns></returns>
        public ExportConfigInfo read(string configFilePath, string id)
        {
            // =========================================================
            // 讀取設定檔案
            // =========================================================
            XmlDocument document = readConfigFile(configFilePath);

            // =========================================================
            // 讀取 ExportConfigInfo
            // =========================================================
            ExportConfigInfo exportConfigInfo = readExcelInfo(document, id);

            // =========================================================
            // 讀取 FormatInfo
            // =========================================================
            exportConfigInfo.FunctionInfoMap = readFunctionInfo(document);

            return exportConfigInfo;
        }

        private ExportConfigInfo readExcelInfo(XmlDocument document, string id)
        {
            // =========================================================
            // 讀取設定資訊
            // =========================================================
            XmlNode excelNode =
                document.SelectSingleNode(
                "//" + Constant.ELEMENT_EXCEL +
                "[@" + Constant.ATTRIBUTE_ID + "=\"" + id + "\"]");

            if (excelNode == null)
            {
                throw new ExcelOperateException("設定資訊:[" + id + "] 不存在!");
            }

            // =========================================================
            // 讀取 excelInfo
            // =========================================================
            var exportConfigInfo = new ExportConfigInfo
            {
                Id = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_ID),
                FileName = ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_FILENAME),
                PaperSize =
                    this.getPaperSize(
                        ExcelStringUtil.GetNodeAttr(excelNode, Constant.ATTRIBUTE_PAPERSIZE))
            };

            // =========================================================
            // 讀取 範圍 style 設定
            // =========================================================
            // 讀取 style node
            XmlNode styleNode = excelNode.SelectSingleNode(Constant.ELEMENT_STYLE);
            // 讀取屬性設定
            var styleInfo = new StyleInfo();
            styleInfo.readStyleAttr(styleNode);
            // 將未設定的屬性，設為系統預設值
            styleInfo.setEmptyAttrToSystemDefault();
            // 放入 excelInfo
            exportConfigInfo.StyleInfo = styleInfo;

            // =========================================================
            // 讀取 sheet 設定
            // =========================================================」
            XmlNodeList sheetNodeList = excelNode.SelectNodes(Constant.ELEMENT_SHEET);

            // 記錄已使用的 dataId
            var dataIdSet = new Dictionary<string, Boolean>();
            // 逐筆讀取
            var sheetList = new List<SheetlInfo>();

            if (sheetNodeList != null)
            {
                foreach (XmlNode sheetNode in sheetNodeList)
                {
                    // sheet 基本資訊
                    var sheetlInfo = new SheetlInfo
                    {
                        Id = ExcelStringUtil.GetNodeAttr(sheetNode, Constant.ATTRIBUTE_ID),
                        SheetName = ExcelStringUtil.GetNodeAttr(sheetNode, Constant.ATTRIBUTE_SHEETNAME)
                    };

                    // sheet 以下的 part 設定
                    var partInfoMap = new Dictionary<string, object>();

                    // 取得Node list
                    XmlNodeList nodeList =
                        sheetNode.SelectNodes("(" + Constant.ELEMENT_CONTEXT + "|" + Constant.ELEMENT_DETAIL + ")");

                    // 解析 node 設定
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        // 取得 node
                        XmlNode partInfoNode = nodeList[i];
                        // 取得 dataId
                        if (partInfoNode.Attributes != null)
                        {
                            string dataId = ExcelStringUtil.GetNodeAttr(partInfoNode, Constant.ATTRIBUTE_DATAID);
                            // 檢核 dataId 不可重複
                            if (dataIdSet.ContainsKey(dataId))
                            {
                                throw new ExcelOperateException(
                                    " <sheet> 標籤下的 (context|detail) 標籤, dataId 不可重複! " +
                                    "[" + dataId + "] (取用 ExportDataSet 中資料時會造成異常)");
                            }
                            dataIdSet.Add(dataId, true);

                            // 依據標籤類型，進行解析
                            if (Constant.ELEMENT_CONTEXT.Equals(partInfoNode.Name))
                            {
                                var contextInfo = new ContextInfo();
                                contextInfo.DataId = dataId;
                                contextInfo.TrInfoList = this.readContextInfo(partInfoNode);
                                partInfoMap.Add(partInfoNode.Name + "_" + i, contextInfo);
                            }
                            else if (Constant.ELEMENT_DETAIL.Equals(partInfoNode.Name))
                            {
                                var xmlNodeList = partInfoNode.SelectNodes(Constant.ELEMENT_COLUMN);
                                if (xmlNodeList != null && xmlNodeList.Count < 1)
                                {
                                    throw new ExcelOperateException("<detail> 標籤下, 不可無 <column> 設定!");
                                }
                                var detailInfo = new DetailInfo
                                {
                                    DataId = dataId,
                                    ColumnInfoList = this.readDetailInfo(partInfoNode)
                                };
                                partInfoMap.Add(partInfoNode.Name + "_" + i, detailInfo);
                            }
                        }
                    }
                    //
                    sheetlInfo.PartInfoMap = partInfoMap;

                    // 放入List
                    sheetList.Add(sheetlInfo);
                }
            }
            // 放入 excelInfo
            exportConfigInfo.SheetList = sheetList;

            return exportConfigInfo;
        }

        private List<ColumnInfo> readDetailInfo(XmlNode partNode)
        {
            List<ColumnInfo> columnInfoList = new List<ColumnInfo>();

            // 取得 column node list
            XmlNodeList columnList = partNode.SelectNodes(Constant.ELEMENT_COLUMN);

            foreach (XmlNode columnNode in columnList)
            {
                ColumnInfo columnInfo = new ColumnInfo();
                // 讀取資料元素設定
                columnInfo.readDataColumnAttr(columnNode);
                // 讀取 style 屬性設定
                columnInfo.readStyleAttr(columnNode);

                // 取得子元素list
                XmlNodeList columnDeatilNodList = columnNode.SelectNodes("(" + Constant.ELEMENT_ARRAY + "|" + Constant.ELEMENT_SINGLE + ")");

                // column(array|single)[遞迴]
                List<ColumnDetailInfo> columnDetailInfoList = new List<ColumnDetailInfo>();
                if (columnDeatilNodList != null)
                {
                    foreach (XmlNode columnDetailNode in columnDeatilNodList)
                    {
                        ColumnDetailInfo columnDetailInfo = new ColumnDetailInfo();
                        // type
                        columnDetailInfo.Type = columnDetailNode.Name;
                        // dataId
                        columnDetailInfo.DataId = ExcelStringUtil.GetNodeAttr(columnDetailNode, Constant.ATTRIBUTE_DATAID);
                        // column
                        var xmlNodeList = columnDetailNode.SelectNodes(Constant.ELEMENT_COLUMN);
                        if (xmlNodeList != null && xmlNodeList.Count > 0)
                        {
                            columnDetailInfo.ColumnInfoList = this.readDetailInfo(columnDetailNode);
                        }
                        // add to list
                        columnDetailInfoList.Add(columnDetailInfo);
                    }
                }
                columnInfo.ColumnDetailInfoList = columnDetailInfoList;

                columnInfoList.Add(columnInfo);
            }
            return columnInfoList;
        }

        /// <summary>
        /// 解析 sheet 層的設定
        /// </summary>
        /// <param name="partNode">子項目名稱</param>
        private List<TrInfo> readContextInfo(XmlNode partNode)
        {
            List<TrInfo> trInfoList = new List<TrInfo>();

            // 未設定時返回
            if (partNode == null)
            {
                return trInfoList;
            }

            // 取得 tr node list
            XmlNodeList trNodeList = partNode.SelectNodes(Constant.ELEMENT_TR);

            // 未設定時返回
            if (trNodeList == null || trNodeList.Count == 0)
            {
                return trInfoList;
            }

            foreach (XmlNode trNode in trNodeList)
            {
                TrInfo trInfo = new TrInfo();
                // 讀取 style 屬性設定
                trInfo.readStyleAttr(trNode);
                // 取得 TD 設定 list 設定
                XmlNodeList tdNodeList = trNode.SelectNodes(Constant.ELEMENT_TD);
                // 檢核
                if (tdNodeList == null || tdNodeList.Count == 0)
                {
                    throw new ExcelOperateException("<tr> 標籤下, 不可無 <td> 設定!");
                }

                // 取得TD 設定
                List<TdInfo> tdInfoList = new List<TdInfo>();
                foreach (XmlNode tdNode in tdNodeList)
                {
                    TdInfo tdInfo = new TdInfo();

                    // 讀取rowspan 屬性 (TdInfo 獨有)
                    if (tdNode.Attributes != null)
                    {
                        tdInfo.Rowspan = Convert.ToInt32(ExcelStringUtil.GetNodeAttr(tdNode, Constant.ATTRIBUTE_ROWSPAN, "0"));
                    }

                    // 讀取資料元素設定
                    tdInfo.readDataColumnAttr(tdNode);

                    // 讀取 style 屬性設定
                    tdInfo.readStyleAttr(tdNode);

                    tdInfoList.Add(tdInfo);
                }
                trInfo.TdInfoList = tdInfoList;
                trInfoList.Add(trInfo);
            }

            return trInfoList;
        }

        /// <summary>
        /// 轉換頁面大小 </summary>
        /// <param name="paperSize">
        /// @return </param>
        private PaperSize getPaperSize(string paperSize)
        {
            if ("A2".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.A2;
            }
            if ("A3".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.A3;
            }
            if ("A4".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.A4;
            }
            if ("A5".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.A5;
            }
            if ("B4".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.B4;
            }
            if ("B5".Equals(paperSize, StringComparison.CurrentCultureIgnoreCase))
            {
                return PaperSize.B5;
            }
            return PaperSize.A4;
        }
    }
}