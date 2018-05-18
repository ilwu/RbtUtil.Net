using CSharpJExcel.Jxl;
using CSharpJExcel.Jxl.Format;
using CSharpJExcel.Jxl.Write;
using rbt.util.excel.bean.expt;
using rbt.util.excel.bean.expt.config;
using rbt.util.excel.exception;
using rbt.util.excel.util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace rbt.util.excel
{
    /// <summary>
    ///     匯出Excel 檔案
    ///     @author Allen
    /// </summary>
    public class ExcelExporter : AbstractExcelOperater
    {
        // ===========================================================================
        // 設定資料庫連線
        // ===========================================================================

        // ===========================================================================
        // 名稱
        // ===========================================================================
        /// <summary>
        ///     最大 column index
        /// </summary>
        private const string KEY_MAX_COL = "KEY_MAX_COL";

        ///
        private const string KEY_COLUMN_COLSPAN_PERFIX = "#COLUMN_COLSPAN#";

        /// <summary>
        ///     設定系統所使用的 DbConnection
        /// </summary>
        public override void setConnection(DbConnection connection)
        {
            Conn = connection;
        }

        // ===========================================================================
        // 功能區
        // ===========================================================================

        /// <summary>
        ///     產生Excel
        /// </summary>
        /// <param name="exportConfigInfo"> </param>
        /// <param name="exportDataSet"> </param>
        /// <param name="outString"></param>
        public virtual void export(ExportConfigInfo exportConfigInfo, ExportDataSet exportDataSet, Stream outString)
        {
            configInfo = exportConfigInfo;

            try
            {
                // =========================================================
                // 建立 Workbook
                // =========================================================
                WritableWorkbook writableWorkbook = Workbook.createWorkbook(outString);

                for (int sheetlIndex = 0; sheetlIndex < exportConfigInfo.SheetList.Count; sheetlIndex++)
                {
                    // =====================================================
                    // 建立 sheet
                    // =====================================================
                    // 取得 sheetlInfo 設定
                    SheetlInfo sheetlInfo = exportConfigInfo.SheetList[sheetlIndex];

                    // 取得 sheetName
                    string sheetName = (ExcelStringUtil.IsEmpty(sheetlInfo.SheetName))
                        ? "Sheet" + (sheetlIndex + 1)
                        : sheetlInfo.SheetName;

                    // 建立 sheet
                    WritableSheet writableSheet = writableWorkbook.createSheet(sheetName, sheetlIndex);

                    // 版面設定
                    // setPageSetup Parameters:
                    // p - the page orientation
                    // ps - the paper size
                    // hm - the header margin, in inches
                    // fm - the footer margin, in inches
                    writableSheet.setPageSetup(PageOrientation.LANDSCAPE, exportConfigInfo.PaperSize, 0, 0);
                    writableSheet.getSettings().setLeftMargin(0);
                    writableSheet.getSettings().setRightMargin(0);

                    // =====================================================
                    // 處理前準備
                    // =====================================================
                    // 列指標
                    int targetRowIndex = 0;
                    // 紀錄已使用的儲存格 (cell)
                    var usedCells = new Dictionary<int, HashSet<string>>();
                    // 紀錄欄(column)的最大欄寬
                    var maxWidthMap = new Dictionary<string, int>();

                    // =====================================================
                    // 資訊
                    // =====================================================
                    foreach (var entry in sheetlInfo.PartInfoMap)
                    {
                        if (entry.Value == null)
                        {
                            return;
                        }

                        // 內容為 context
                        if (entry.Key.StartsWith(Constant.ELEMENT_CONTEXT))
                        {
                            //取得 context 設定檔設定資料
                            var contextInfo = (ContextInfo)entry.Value;
                            //取得 匯出資料
                            Dictionary<string, object> dataMap = exportDataSet.getContext(contextInfo.DataId);

                            targetRowIndex = WriteContext(
                                writableSheet,
                                contextInfo,
                                targetRowIndex,
                                dataMap,
                                usedCells,
                                maxWidthMap);
                        }

                        //內容為 detail
                        if (entry.Key.StartsWith(Constant.ELEMENT_DETAIL))
                        {
                            //取得 context 設定檔設定資料
                            var detailInfo = (DetailInfo)entry.Value;
                            //取得 匯出資料
                            var columnDataSetList = exportDataSet.getDetail(detailInfo.DataId);

                            targetRowIndex = EnterWriteDetail(
                                writableSheet,
                                detailInfo,
                                targetRowIndex,
                                columnDataSetList,
                                usedCells,
                                maxWidthMap);
                        }
                    }

                    // =====================================================
                    // 設定欄寬
                    // =====================================================
                    // 取得最大欄位 index
                    int maxColIndex = maxWidthMap[KEY_MAX_COL];
                    for (int colIndex = 0; colIndex <= maxColIndex; colIndex++)
                    {
                        // 取得欄寬
                        int colWidth = 0;
                        //取得 MAP 中的值 (tr、td 設定)
                        if (maxWidthMap.ContainsKey(colIndex + ""))
                        {
                            colWidth = maxWidthMap[colIndex + ""];
                        }
                        //若 tr td 未設定時，取 style 設定
                        if (colWidth == 0)
                        {
                            colWidth = Convert.ToInt32(ExcelStringUtil.SafeTrim(exportConfigInfo.StyleInfo.Width, "0"));
                        }

                        // 以上都未設定時使用預設值
                        //if (colWidth == 0)
                        //{
                        //    colWidth = Convert.ToInt32(Constant.DEFAULT_WIDTH);
                        //}

                        if (colWidth > 0)
                        {
                            writableSheet.setColumnView(colIndex, colWidth);
                        }
                    }
                }

                writableWorkbook.write();
                writableWorkbook.close();
            }
            catch (Exception e)
            {
                throw new ExcelOperateException("EXCEL 檔案匯出處理錯誤! \r\n" + e.Message + "\r\n" + e.StackTrace);
            }
        }

        private int EnterWriteDetail(WritableSheet writableSheet, DetailInfo detailInfo, int targetRowIndex,
            IList<ColumnDataSet> columnDataSetList, Dictionary<int, HashSet<string>> usedCells,
            Dictionary<string, int> maxWidthMap)
        {
            // 取得欄位設定欄位
            List<ColumnInfo> columnInfoList = detailInfo.ColumnInfoList;

            // 無資料跳出
            if (ExcelStringUtil.IsEmpty(detailInfo.ColumnInfoList) || ExcelStringUtil.IsEmpty(columnDataSetList) || columnDataSetList.Count == 0)
            {
                return targetRowIndex;
            }

            // 計算 rowspan
            foreach (ColumnDataSet columnDataSet in columnDataSetList)
            {
                countRowspan(columnInfoList, columnDataSet);
            }

            // excel 欄位輸出
            foreach (ColumnDataSet columnDataSet in columnDataSetList)
            {
                targetRowIndex = writeDetail(
                    writableSheet,
                    columnInfoList,
                    columnDataSet,
                    targetRowIndex,
                    usedCells,
                    maxWidthMap);
            }

            return targetRowIndex;
        }

        private int writeDetail(WritableSheet writableSheet, IEnumerable<ColumnInfo> columnInfoList,
            ColumnDataSet columnDataSet, int targetRowIndex, Dictionary<int, HashSet<string>> usedCells,
            Dictionary<string, int> maxWidthMap)
        {
            int targetColIndex = 0;
            int newTargetRowIndex = targetRowIndex;

            foreach (ColumnInfo columnInfo in columnInfoList)
            {
                // 取得子欄位
                List<ColumnDetailInfo> columnDetailInfoList = columnInfo.ColumnDetailInfoList;

                // 為子欄位陣列時，進行遞迴處理
                if (ExcelStringUtil.NotEmpty(columnDetailInfoList))
                {
                    foreach (ColumnDetailInfo columnDetailInfo in columnDetailInfoList)
                    {
                        // 設定元素類別
                        string type = columnDetailInfo.Type;
                        // dataId
                        string dataId = columnDetailInfo.DataId;
                        // 欄位下的欄位
                        List<ColumnInfo> childColumnInfoList = columnDetailInfo.ColumnInfoList;

                        // ELEMENT_SINGLE
                        if (string.Equals(type, Constant.ELEMENT_SINGLE, StringComparison.OrdinalIgnoreCase))
                        {
                            // 遞迴處理
                            newTargetRowIndex = writeDetail(
                                writableSheet, childColumnInfoList,
                                columnDataSet.getSingle(dataId),
                                newTargetRowIndex, usedCells, maxWidthMap);
                        }
                        if (string.Equals(type, Constant.ELEMENT_ARRAY, StringComparison.OrdinalIgnoreCase))
                        {
                            // 取得 array 元素的資料集
                            List<ColumnDataSet> arrayDataList = columnDataSet.getArray(dataId);

                            // 逐筆處理
                            foreach (ColumnDataSet arrayColumnDataSet in arrayDataList)
                            {
                                // 遞迴處理
                                newTargetRowIndex = writeDetail(writableSheet, childColumnInfoList, arrayColumnDataSet,
                                    newTargetRowIndex, usedCells, maxWidthMap);
                            }
                        }
                    }
                    continue;
                }

                // 取得 key
                string key = columnInfo.Key;
                // 取得欄位設定
                WritableCellFormat cellFormat = getCellFormat(columnInfo, columnInfo);
                // 取得要放入 cell 的值
                string content = perpareContent(key, columnInfo.DefaultValue, columnInfo.FuncId, columnInfo.FuncParam,
                    columnDataSet.ColumnDataMap);
                // 取得寬度設定
                int width = Convert.ToInt32(ExcelStringUtil.SafeTrim(columnInfo.Width, "0"));
                // 取得還未使用的 column
                targetColIndex = getUnUsedCol(usedCells, targetRowIndex, targetColIndex);

                // 取得 rowspan (之前已計算好)
                if (!columnDataSet.ColumnDataMap.ContainsKey(KEY_COLUMN_COLSPAN_PERFIX + key))
                {
                    throw new Exception("指定的索引鍵不在字典中 [" + key + "]");
                }

                var rowspan = (int)columnDataSet.ColumnDataMap[KEY_COLUMN_COLSPAN_PERFIX + key];
                // colspan
                int colspan = columnInfo.Colspan;

                if (colspan > 1 || rowspan > 1)
                {
                    // 合併儲存格
                    merageCell(writableSheet, usedCells, targetColIndex, targetRowIndex, colspan, rowspan, maxWidthMap,
                        width);
                    // addCell
                    //addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap, width, key);
                    addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap,
                        width);

                    // 移動 col 指標
                    if (colspan > 0)
                    {
                        targetColIndex += colspan;
                    }
                    else
                    {
                        targetColIndex++;
                    }
                }
                else
                {
                    // addCell
                    //addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap, width, key);
                    addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap,
                        width);
                    // 移動 col 指標
                    targetColIndex++;
                }
            }
            targetRowIndex++;
            // newTargetRowIndex++;
            return (targetRowIndex > newTargetRowIndex) ? targetRowIndex : newTargetRowIndex;
        }

        /// <summary>
        ///     計算 rowspan
        /// </summary>
        /// <param name="columnInfoList"></param>
        /// <param name="columnDataSet"></param>
        private int countRowspan(List<ColumnInfo> columnInfoList, ColumnDataSet columnDataSet)
        {
            int myRowspan = 0;
            foreach (ColumnInfo columnInfo in columnInfoList)
            {
                // 取得子欄位
                List<ColumnDetailInfo> columnDetailInfoList = columnInfo.ColumnDetailInfoList;

                // 無子欄位設定時略過
                if (ExcelStringUtil.IsEmpty(columnDetailInfoList))
                {
                    continue;
                }

                int currRowAdd = 0;
                foreach (ColumnDetailInfo columnDetailInfo in columnDetailInfoList)
                {
                    // 設定元素類別
                    string type = columnDetailInfo.Type;
                    // dataId
                    string dataId = columnDetailInfo.DataId;
                    // 欄位下的欄位
                    List<ColumnInfo> childColumnInfoList = columnDetailInfo.ColumnInfoList;

                    // ELEMENT_SINGLE
                    if (string.Equals(type, Constant.ELEMENT_SINGLE, StringComparison.OrdinalIgnoreCase))
                    {
                        // 遞迴處理
                        currRowAdd += countRowspan(childColumnInfoList, columnDataSet.getSingle(dataId));
                    }

                    if (string.Equals(type, Constant.ELEMENT_ARRAY, StringComparison.OrdinalIgnoreCase))
                    {
                        // 取得 array 元素的資料集
                        List<ColumnDataSet> arrayDataList = columnDataSet.getArray(dataId);
                        if (arrayDataList == null || arrayDataList.Count == 0)
                        {
                            arrayDataList = new List<ColumnDataSet>();
                            arrayDataList.Add(new ColumnDataSet());
                        }
                        // 逐筆處理
                        foreach (ColumnDataSet arrayColumnDataSet in arrayDataList)
                        {
                            // 遞迴處理
                            currRowAdd += countRowspan(childColumnInfoList, arrayColumnDataSet);
                        }
                    }
                    // 取得最大 rowspan
                    if (currRowAdd > myRowspan)
                    {
                        myRowspan = currRowAdd;
                    }
                }
            }

            // 加上基礎的1列
            if (myRowspan == 0)
            {
                myRowspan = 1;
            }

            foreach (ColumnInfo columnInfo in columnInfoList)
            {
                // 取得子欄位
                List<ColumnDetailInfo> columnDetailInfoList = columnInfo.ColumnDetailInfoList;

                // 有子欄位設定時略過
                if (ExcelStringUtil.NotEmpty(columnDetailInfoList))
                {
                    continue;
                }

                // 取得 key
                string key = columnInfo.Key;
                // 把 rowspan值 以 key 加上固定前綴之後, 放入 資料MAP
                try
                {
                    columnDataSet.ColumnDataMap.Add(KEY_COLUMN_COLSPAN_PERFIX + key, myRowspan);
                }
                catch (ArgumentException)
                {
                    //throw new Exception("detail 的 key :[" + key + "] 重複，請檢查設定檔");
                }
            }
            return myRowspan;
        }

        private int WriteContext(
            WritableSheet writableSheet,
            ContextInfo contextInfo,
            int targetRowIndex,
            Dictionary<string, object> dataMap,
            Dictionary<int, HashSet<string>> usedCells,
            Dictionary<string, int> maxWidthMap)
        {
            // 無資料時跳出
            if (contextInfo.TrInfoList == null)
            {
                return targetRowIndex;
            }

            // 逐列處理
            for (int row = 0; row < contextInfo.TrInfoList.Count; row++)
            {
                //取得 TrInfo
                TrInfo trInfo = contextInfo.TrInfoList[row];

                // col index 指標
                int targetColIndex = 0;

                for (int col = 0; col < trInfo.TdInfoList.Count; col++)
                {
                    // 取得 TdInfo
                    TdInfo tdInfo = trInfo.TdInfoList[col];
                    // 取得欄位設定
                    WritableCellFormat cellFormat = getCellFormat(trInfo, tdInfo);
                    // 取得要放入 cell 的值
                    string content = perpareContent(tdInfo.Key, tdInfo.DefaultValue, tdInfo.FuncId, tdInfo.FuncParam,
                        dataMap);
                    // 取得寬度設定
                    int width = Convert.ToInt32(ExcelStringUtil.SafeTrim(tdInfo.Width, "0"));
                    // 取得還未使用的 column
                    targetColIndex = getUnUsedCol(usedCells, targetRowIndex, targetColIndex);

                    if (tdInfo.Colspan > 1 || tdInfo.Rowspan > 1)
                    {
                        // 合併儲存格
                        merageCell(writableSheet, usedCells, targetColIndex, targetRowIndex, tdInfo.Colspan,
                            tdInfo.Rowspan, maxWidthMap, width);
                        // addCell
                        //addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap, width, tdInfo.Key);
                        addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat,
                            maxWidthMap, width);

                        // 移動 col 指標
                        if (tdInfo.Colspan > 0)
                        {
                            targetColIndex += tdInfo.Colspan;
                        }
                        else
                        {
                            targetColIndex++;
                        }
                    }
                    else
                    {
                        // addCell
                        //addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat, maxWidthMap, width, tdInfo.Key);
                        addCell(writableSheet, usedCells, targetColIndex, targetRowIndex, content, cellFormat,
                            maxWidthMap, width);
                        // 移動 col 指標
                        targetColIndex++;
                    }
                }

                // 取得列高設定
                //height of the row in 1/20ths of a point
                int height = Convert.ToInt32(ExcelStringUtil.SafeTrim(trInfo.Height, "0")) * 20;
                if (height > 0)
                {
                    writableSheet.setRowView(targetRowIndex, height);
                }

                targetRowIndex++;
            }
            return targetRowIndex;
        }

        /// <summary>
        ///     設定已使用欄位
        /// </summary>
        /// <param name="usedCells"> </param>
        /// <param name="rowNum"> </param>
        /// <param name="colNum"> </param>
        private void setUsed(Dictionary<int, HashSet<string>> usedCells, int rowNum, int colNum)
        {
            HashSet<string> colNumSet = usedCells.ContainsKey(rowNum) ? usedCells[rowNum] : null;
            if (colNumSet == null)
            {
                colNumSet = new HashSet<string>();
                usedCells[rowNum] = colNumSet;
            }
            colNumSet.Add(colNum + "");
        }

        /// <summary>
        ///     設定已使用欄位 (範圍)
        /// </summary>
        /// <param name="usedCells"> </param>
        /// <param name="startCol"> </param>
        /// <param name="startRow"> </param>
        /// <param name="endCol"> </param>
        /// <param name="endRow"> </param>
        private void setUsed(Dictionary<int, HashSet<string>> usedCells, int startCol, int startRow, int endCol, int endRow)
        {
            for (int rowNum = startRow; rowNum <= endRow; rowNum++)
            {
                for (int colNum = startCol; colNum <= endCol; colNum++)
                {
                    setUsed(usedCells, rowNum, colNum);
                }
            }
        }

        /// <summary>
        ///     取得還未使用的 column index
        /// </summary>
        /// <param name="usedCells"> 已使用區域紀錄物件 </param>
        /// <param name="rowNum"> row index </param>
        /// <param name="startColNum">
        ///     開始尋找的 column index
        ///     @return
        /// </param>
        private int getUnUsedCol(Dictionary<int, HashSet<string>> usedCells, int rowNum, int startColNum)
        {
            if (!usedCells.ContainsKey(rowNum))
            {
                return 0;
            }

            HashSet<string> colNumSet = usedCells.ContainsKey(rowNum) ? usedCells[rowNum] : null;
            if (colNumSet == null)
            {
                return 0;
            }
            int newColNum = startColNum;
            while (colNumSet.Contains(newColNum + ""))
            {
                newColNum++;
            }
            return newColNum;
        }

        /// <summary>
        ///     合併儲存格
        /// </summary>
        /// <param name="writableSheet"> WritableSheet </param>
        /// <param name="usedCells"> 已使用區域紀錄物件 </param>
        /// <param name="startCol"> 開始的 column index (行) </param>
        /// <param name="startRow"> 開始的 row index (列) </param>
        /// <param name="colspan"> colspan </param>
        /// <param name="rowspan"> rowspan </param>
        /// <param name="maxWidthMap"> 最大欄寬紀錄物件 </param>
        /// <param name="width"> 欄寬 </param>
        private void merageCell(WritableSheet writableSheet, Dictionary<int, HashSet<string>> usedCells, int startCol,
            int startRow, int colspan, int rowspan, Dictionary<string, int> maxWidthMap, int width)
        {
            int endCol = startCol;
            if (colspan > 1)
            {
                endCol += colspan - 1;
            }
            int endRow = startRow;
            if (rowspan > 1)
            {
                endRow += rowspan - 1;
            }
            // 合併儲存格
            writableSheet.mergeCells(startCol, startRow, endCol, endRow);
            // 記錄使用區域
            setUsed(usedCells, startCol, startRow, endCol, endRow);
            // 記錄欄寬
            for (int colNum = startCol; colNum <= endCol; colNum++)
            {
                setMaxWidth(maxWidthMap, width, colNum);
            }
        }

        /// <summary>
        ///     add Cell
        /// </summary>
        /// <param name="writableSheet"> WritableSheet </param>
        /// <param name="usedCells"> 已使用區域紀錄物件 </param>
        /// <param name="colIndex"> column index </param>
        /// <param name="rowIndex"> row index </param>
        /// <param name="content"> 文字內容 </param>
        /// <param name="cellFormat"> cell 格式設定物件 </param>
        /// <param name="maxWidthMap"> 最大欄寬紀錄物件 </param>
        /// <param name="width"> 欄寬 </param>
        private void addCell(
            WritableSheet writableSheet,
            Dictionary<int, HashSet<string>> usedCells,
            int colIndex,
            int rowIndex,
            string content,
            WritableCellFormat cellFormat,
            Dictionary<string, int> maxWidthMap,
            int width)
        {
            // System.out.println(key + ": [" + rowIndex + "," + colIndex + "],[" + content + "]");
            // 新增 cell
            writableSheet.addCell(new Label(colIndex, rowIndex, content, cellFormat));
            // 記錄使用區域
            setUsed(usedCells, rowIndex, colIndex);
            // 記錄欄寬
            setMaxWidth(maxWidthMap, width, colIndex);
        }

        /// <summary>
        ///     記錄欄寬
        /// </summary>
        /// <param name="maxWidthMap"> </param>
        /// <param name="width"> </param>
        /// <param name="colIndex"> </param>
        private void setMaxWidth(Dictionary<string, int> maxWidthMap, int width, int colIndex)
        {
            // ================================================
            // 紀錄最大 INDEX
            // ================================================
            // 取得目前最大的 index
            int maxColIndex = maxWidthMap.ContainsKey(KEY_MAX_COL) ? maxWidthMap[KEY_MAX_COL] : 0;
            //Convert.ToInt32(StringUtil.SafeTrim(maxWidthMap[KEY_MAX_COL], "0"));
            // 傳入值大於現值時，更新
            if (colIndex > maxColIndex)
            {
                maxWidthMap[KEY_MAX_COL] = colIndex;
            }

            // ================================================
            // 紀錄最大欄寬
            // ================================================
            string key = "" + colIndex;
            // 取得該index目前設定值
            int maxWidth = maxWidthMap.ContainsKey(key) ? maxWidthMap[key] : 0;
            //Convert.ToInt32(StringUtil.SafeTrim(maxWidthMap[key], "0"));
            // 傳入值大於現值時，更新
            if (width > maxWidth)
            {
                maxWidthMap[key] = width;
            }
        }

        /// <summary>
        ///     準備要放入 cell 的資料
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="value"> </param>
        /// <param name="funcId"> </param>
        /// <param name="funcParam"> </param>
        /// <param name="dataMap">
        ///     @return
        /// </param>
        private string perpareContent(string key, string value, string funcId, string funcParam,
            Dictionary<string, object> dataMap)
        {
            string content = "";

            // KEY 有設定時優先處理
            if (dataMap != null && ExcelStringUtil.NotEmpty(key) && dataMap.ContainsKey(key))
            {
                content = ExcelStringUtil.SafeTrim(dataMap[key]);
            }
            // 取不到值時,
            if (ExcelStringUtil.IsEmpty(content))
            {
                content = value;
            }
            // 呼叫處理的 function
            if (ExcelStringUtil.NotEmpty(funcId))
            {
                content = functionProcess(key, funcId, funcParam, value, dataMap, Conn);
            }
            return content;
        }

        /// <summary>
        ///     設定 Cell 格式 (Detal時, 沒有 tr 和 td, 此時兩個參數傳入同一個物件, 不影響判斷)
        /// </summary>
        /// <param name="trInfo" />
        /// <param name="tdInfo" />
        private WritableCellFormat getCellFormat(AbstractStyleInfo trInfo, AbstractStyleInfo tdInfo)
        {
            //style 設定
            StyleInfo styleInfo = ((ExportConfigInfo)configInfo).StyleInfo;

            // 字體名稱
            WritableFont.FontName font = styleInfo.Font;
            if (tdInfo.Font != null)
            {
                font = tdInfo.Font;
            }
            else if (trInfo.Font != null)
            {
                font = trInfo.Font;
            }

            // 字體大小
            int size = 0;
            if (ExcelStringUtil.NotEmpty(tdInfo.Size))
            {
                size = Convert.ToInt32(ExcelStringUtil.SafeTrim(tdInfo.Size, "0"));
            }
            else if (ExcelStringUtil.NotEmpty(trInfo.Size))
            {
                size = Convert.ToInt32(ExcelStringUtil.SafeTrim(trInfo.Size, "0"));
            }
            if (size == 0)
            {
                size = Convert.ToInt32(styleInfo.Size);
            }

            // 粗體
            bool isBold = ("true".Equals(styleInfo.Bold, StringComparison.CurrentCultureIgnoreCase));
            if (ExcelStringUtil.NotEmpty(tdInfo.Bold))
            {
                isBold = ("true".Equals(tdInfo.Bold, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (ExcelStringUtil.NotEmpty(trInfo.Bold))
            {
                isBold = ("true".Equals(trInfo.Bold, StringComparison.CurrentCultureIgnoreCase));
            }

            // 斜體
            bool isItalic = ("true".Equals(styleInfo.Italic, StringComparison.CurrentCultureIgnoreCase));
            if (ExcelStringUtil.NotEmpty(tdInfo.Italic))
            {
                isItalic = ("true".Equals(tdInfo.Italic, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (ExcelStringUtil.NotEmpty(trInfo.Bold))
            {
                isItalic = ("true".Equals(trInfo.Italic, StringComparison.CurrentCultureIgnoreCase));
            }

            // 底線
            UnderlineStyle underlineStyle = styleInfo.Underline;
            if (tdInfo.Underline != null)
            {
                underlineStyle = tdInfo.Underline;
            }
            else if (trInfo.Underline != null)
            {
                underlineStyle = trInfo.Underline;
            }

            // 字體顏色
            Colour color = styleInfo.Color;
            if (tdInfo.Color != null)
            {
                color = tdInfo.Color;
            }
            else if (trInfo.Color != null)
            {
                color = trInfo.Color;
            }

            // 水平位置
            Alignment align = styleInfo.Align;
            if (tdInfo.Align != null)
            {
                align = tdInfo.Align;
            }
            else if (trInfo.Align != null)
            {
                align = trInfo.Align;
            }

            // 垂直位置
            VerticalAlignment valign = styleInfo.Valign;
            if (tdInfo.Valign != null)
            {
                valign = tdInfo.Valign;
            }
            else if (trInfo.Valign != null)
            {
                valign = trInfo.Valign;
            }

            // 文字換行
            bool isTextWrap = ("true".Equals(styleInfo.Wrap, StringComparison.CurrentCultureIgnoreCase));
            if (ExcelStringUtil.NotEmpty(tdInfo.Wrap))
            {
                isTextWrap = ("true".Equals(tdInfo.Wrap, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (ExcelStringUtil.NotEmpty(trInfo.Wrap))
            {
                isTextWrap = ("true".Equals(trInfo.Wrap, StringComparison.CurrentCultureIgnoreCase));
            }

            // 邊線位置
            Border borderSide = styleInfo.BorderSide;
            if (tdInfo.BorderSide != null)
            {
                borderSide = tdInfo.BorderSide;
            }
            else if (trInfo.BorderSide != null)
            {
                borderSide = trInfo.BorderSide;
            }

            // 邊線樣式
            BorderLineStyle borderStyle = styleInfo.BorderStyle;
            if (tdInfo.BorderStyle != null)
            {
                borderStyle = tdInfo.BorderStyle;
            }
            else if (trInfo.Valign != null)
            {
                borderStyle = trInfo.BorderStyle;
            }

            // 背景顏色
            Colour background = styleInfo.Background;
            if (tdInfo.Background != null)
            {
                background = tdInfo.Background;
            }
            else if (trInfo.Background != null)
            {
                background = trInfo.Background;
            }

            // 產生字型設定
            var writableFont = new WritableFont(font, size, (isBold) ? WritableFont.BOLD : WritableFont.NO_BOLD,
                isItalic, underlineStyle, color);

            // 資料列cell格式
            var writableCellFormat = new WritableCellFormat(writableFont);
            // 水平置中
            writableCellFormat.setAlignment(align);
            // 垂直置中
            writableCellFormat.setVerticalAlignment(valign);
            // 換行
            writableCellFormat.setWrap(isTextWrap);
            // 背景顏色
            writableCellFormat.setBackground(background);
            // 邊線
            writableCellFormat.setBorder(borderSide, borderStyle);

            return writableCellFormat;
        }
    }
}