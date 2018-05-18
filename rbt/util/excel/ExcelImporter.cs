using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using rbt.util.excel.bean.impt.config;
using rbt.util.excel.config;
using rbt.util.excel.exception;
using rbt.util.excel.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace rbt.util.excel
{
    /// <summary>
    /// @author Allen
    /// </summary>
    public class ExcelImporter : AbstractExcelOperater
    {
        /// <summary>
        /// 設定系統所使用的 DbConnection
        /// </summary>
        public override void setConnection(DbConnection connection)
        {
            Conn = connection;
        }

        /// <summary>
        /// 讀取 xlsx 格式 , 並返回 DataTable
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="configFilePath"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public virtual DataTable readXlsx(Stream inStream, string configFilePath, string configId, bool isCloseStream = true)
        {
            return read(inStream, configFilePath, configId, true, isCloseStream);
        }

        /// <summary>
        /// 讀取檔案並傳回 DataTable
        /// </summary>
        /// <param name="inStream">Excel 檔案 input Stream (以 StreamReader 讀取檔案)</param>
        /// <param name="configFilePath">設定檔案路徑</param>
        /// <param name="configId">設定資料ID</param>
        /// <param name="isXlsx">傳入檔案是否為 xlsx 格式</param>
        /// <returns></returns>
        public virtual DataTable read(
            Stream inStream, string configFilePath, string configId,
            bool isXlsx = false,
            bool isCloseStream = true
            )
        {
            // ==============================================
            // 讀取設定檔
            // ==============================================
            var importConfigInfo = prepareConfigInfo(configFilePath, configId);

            // ==============================================
            // 讀取 excel 檔案
            // ==============================================
            var result = this.read(inStream, importConfigInfo, isXlsx);

            // ==============================================
            // IList<Dictionary<string, object>> 轉 DataTable
            // ==============================================
            DataTable datatable = new DataTable();
            foreach (var columnInfo in importConfigInfo.ColumnInfoList)
            {
                datatable.Columns.Add(new DataColumn(columnInfo.Key));
            }

            foreach (var row in result)
            {
                var dataRow = datatable.NewRow();

                foreach (KeyValuePair<string, object> keyValuePair in row)
                {
                    dataRow[keyValuePair.Key] = keyValuePair.Value;
                }
                datatable.Rows.Add(dataRow);
            }

            return datatable;
        }

        /// <summary>
        /// 讀取檔案並傳回 並傳回 TModel List
        /// </summary>
        /// <typeparam name="TModel">回傳的 model 型態</typeparam>
        /// <param name="inStream">Excel 檔案 input Stream (以 StreamReader 讀取檔案)</param>
        /// <param name="configFilePath">設定檔案路徑</param>
        /// <param name="configId">設定資料ID</param>
        /// <param name="isXlsx">傳入檔案是否為 xlsx 格式</param>
        /// <returns></returns>
        public IList<TModel> readXlsx<TModel>(Stream inStream, string configFilePath, string configId)
        {
            return read<TModel>(inStream, configFilePath, configId, true);
        }

        /// <summary>
        /// 讀取檔案並傳回 並傳回 TModel List
        /// </summary>
        /// <typeparam name="TModel">回傳的 model 型態</typeparam>
        /// <param name="inStream">Excel 檔案 input Stream (以 StreamReader 讀取檔案)</param>
        /// <param name="configFilePath">設定檔案路徑</param>
        /// <param name="configId">設定資料ID</param>
        /// <param name="isXlsx">傳入檔案是否為 xlsx 格式</param>
        /// <returns></returns>
        public IList<TModel> read<TModel>(
            Stream inStream, string configFilePath, string configId,
             bool isXlsx = false,
             bool isCloseStream = true
            )
        {
            // ==============================================
            // 讀取設定檔
            // ==============================================
            var importConfigInfo = prepareConfigInfo(configFilePath, configId);

            // ==============================================
            // 讀取 excel 檔案
            // ==============================================
            var dicList = this.read(inStream, importConfigInfo, isXlsx, isCloseStream);

            // ==============================================
            // 轉 Model List
            // ==============================================
            //回傳容器
            var resultList = new List<TModel>();
            //取得傳入 model 所有 property
            var properties = typeof(TModel).GetProperties();
            var ConvertFuncMap = GetConvertFuncMap();

            foreach (var rowDataDic in dicList)
            {
                TModel model = (TModel)Activator.CreateInstance(typeof(TModel));
                var isContains = false;
                foreach (var keyName in rowDataDic.Keys)
                {
                    foreach (var propertyInfo in properties)
                    {
                        if (propertyInfo.Name.Equals(keyName))
                        {
                            MethodInfo mInfo = propertyInfo.GetSetMethod();
                            if (mInfo != null)
                            {
                                Type ptype = propertyInfo.PropertyType;
                                if (ConvertFuncMap.ContainsKey(ptype))
                                {
                                    Func<object, object> converter = ConvertFuncMap[ptype];
                                    object cvtVal = converter(rowDataDic[propertyInfo.Name]);
                                    propertyInfo.SetValue(model, cvtVal);
                                }
                                else
                                {
                                    propertyInfo.SetValue(model, rowDataDic[propertyInfo.Name]);
                                }
                                isContains = true;
                                break;
                            }
                        }
                    }
                    if (!isContains)
                    {
                        throw new Exception("[" + keyName + "]在" + model.GetType().ToString() + "中找不到!");
                    }
                }
                resultList.Add(model);
            }

            return resultList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private Dictionary<Type, Func<object, object>> GetConvertFuncMap()
        {
            var ConvertFuncMap = new Dictionary<Type, Func<object, object>>();

            ConvertFuncMap.Add(typeof(Int16?), (object x) =>
            {
                return x != null ? Convert.ToInt16(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int16), (object x) =>
            {
                return x != null ? Convert.ToInt16(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int32?), (object x) =>
            {
                return x != null ? Convert.ToInt32(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int32), (object x) =>
            {
                return x != null ? Convert.ToInt32(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int64?), (object x) =>
            {
                return x != null ? Convert.ToInt64(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int64), (object x) =>
            {
                return x != null ? Convert.ToInt64(x) : x;
            });
            ConvertFuncMap.Add(typeof(decimal?), (object x) =>
            {
                return x != null ? Convert.ToDecimal(x) : x;
            });
            ConvertFuncMap.Add(typeof(decimal), (object x) =>
            {
                return x != null ? Convert.ToDecimal(x) : x;
            });
            ConvertFuncMap.Add(typeof(double?), (object x) =>
            {
                return x != null ? Convert.ToDouble(x) : x;
            });
            ConvertFuncMap.Add(typeof(double), (object x) =>
            {
                return x != null ? Convert.ToDouble(x) : x;
            });

            return ConvertFuncMap;
        }

        /// <summary>
        /// 讀取設定檔
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        private ImportConfigInfo prepareConfigInfo(string configFilePath, string configId)
        {
            // ==============================================
            // 傳入參數檢核
            // ==============================================
            if (ExcelStringUtil.IsEmpty(configFilePath))
            {
                throw new ExcelOperateException("Excel 檔案匯入設定檔未設定 ! [configFilePath]");
            }

            if (ExcelStringUtil.IsEmpty(configId))
            {
                throw new ExcelOperateException("Excel 檔案匯入設定檔未設定 ! [configID]");
            }

            if (!File.Exists(configFilePath.Trim()))
            {
                throw new ExcelOperateException("Excel 檔案匯入設定檔不存在 ! [" + configFilePath.Trim() + "]");
            }

            // ==============================================
            // 讀取設定檔
            // ==============================================
            return new ImportConfigReader().read(configFilePath, configId);
        }

        /// <summary>
        /// 讀取 excel 檔案
        /// </summary>
        /// <param name="inStream">Excel 檔案 input Stream (以 StreamReader 讀取檔案)</param>
        /// <param name="importConfigInfo">設定檔資料</param>
        /// <param name="isXlsx">是否為 xlsx </param>
        /// <returns></returns>
        public IList<Dictionary<string, object>> readXlsx(
                                Stream inStream, ImportConfigInfo importConfigInfo, bool isCloseStream = true)
        {
            return read(inStream, importConfigInfo, true, isCloseStream);
        }

        /// <summary>
        /// 讀取Excel 並回傳 IList[Dictionary[string, object]]
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="configFilePath"></param>
        /// <param name="configId"></param>
        /// <param name="isXlsx"></param>
        /// <returns></returns>
        public IList<Dictionary<string, object>> ReadDic(
            Stream inStream, string configFilePath, string configId,
            bool isXlsx = false,
            bool isCloseStream = true
            )
        {
            // ==============================================
            // 讀取設定檔
            // ==============================================
            var importConfigInfo = prepareConfigInfo(configFilePath, configId);

            // ==============================================
            // 讀取 excel 檔案
            // ==============================================
            return this.read(inStream, importConfigInfo, isXlsx, isCloseStream);
        }

        /// <summary>
        /// 讀取Excel 並回傳 IList[Dictionary[string, object]]
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="configFilePath"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public IList<Dictionary<string, object>> ReadDicXlsx(
                       Stream inStream, string configFilePath, string configId,
                       bool isCloseStream = true)
        {
            return ReadDic(inStream, configFilePath, configId, true, isCloseStream);
        }

        /// <summary>
        /// 讀取 excel 檔案
        /// </summary>
        /// <param name="inStream">Excel 檔案 input Stream (以 StreamReader 讀取檔案)</param>
        /// <param name="importConfigInfo">設定檔資料</param>
        /// <param name="isXlsx">是否為 xlsx </param>
        /// <param name="isCloseStream">是否自動關閉 inStream  (多 sheet 用)</param>
        /// <returns></returns>
        public IList<Dictionary<string, object>> read(
                                Stream inStream, ImportConfigInfo importConfigInfo,
                                bool isXlsx = false, bool isCloseStream = true)
        {
            this.configInfo = importConfigInfo;

            // ==============================================
            // 讀取檔案內容
            // ==============================================
            IList<Dictionary<string, object>> dataList = this.parseXsl(inStream, importConfigInfo, isXlsx, isCloseStream);

            // ==============================================
            // 建立以 key 為引索的 設定 map, 與收集欄位list
            // ==============================================
            // Map
            Dictionary<string, AbstractImportCommonAttrInfo> paramInfoMap =
                new Dictionary<string, AbstractImportCommonAttrInfo>();
            // List
            List<AbstractImportCommonAttrInfo> paramInfoList = new List<AbstractImportCommonAttrInfo>();

            foreach (ColumnInfo columnInfo in importConfigInfo.ColumnInfoList)
            {
                dicPut(paramInfoMap, columnInfo.Key, columnInfo);
                paramInfoList.Add(columnInfo);
            }

            foreach (ParamInfo paramInfo in importConfigInfo.ParamInfoList)
            {
                dicPut(paramInfoMap, paramInfo.Key, paramInfo);
                paramInfoList.Add(paramInfo);
            }

            // ==============================================
            // param (其他額外設定欄位)
            // ==============================================
            foreach (Dictionary<string, object> rowDataMap in dataList)
            {
                foreach (ParamInfo paramInfo in importConfigInfo.ParamInfoList)
                {
                    // 取得預設值
                    string defaultValue = paramInfo.DefaultValue;
                    // 存入 map
                    dicPut(rowDataMap, paramInfo.Key, defaultValue);
                }
            }

            // ==============================================
            // 欄位值額外處理
            // ==============================================
            string errorMessage = "";

            int rowNum = importConfigInfo.SheetNum;
            foreach (Dictionary<string, object> rowDataMap in dataList)
            {
                foreach (ColumnInfo columnInfo in importConfigInfo.ColumnInfoList)
                {
                    // 取得欄位 key
                    string key = columnInfo.Key;
                    // 取得欄位說明
                    string desc = columnInfo.Desc;
                    // 取得值
                    string value = ExcelStringUtil.SafeTrim(rowDataMap[key]);

                    // =======================================
                    // 資料檢核
                    // =======================================
                    // 該欄位已檢核出錯誤時, 不繼續進行檢核

                    // 有限定欄位不可為空時,進行資料檢查
                    if ("true".Equals(columnInfo.CheckNull, StringComparison.CurrentCultureIgnoreCase) && ExcelStringUtil.IsEmpty(value))
                    {
                        errorMessage += "<br/>第" + rowNum + "行, 欄位：【" + desc + "." + key + "】資料內容不可為空";
                        continue;
                    }
                    // 有設定 formatId 時,進行資料檢查
                    if (ExcelStringUtil.NotEmpty(columnInfo.FormatId))
                    {
                        errorMessage += this.validateDataByFormatId(columnInfo.FormatId, value, key, desc, rowNum);
                        continue;
                    }
                    // 有設定 regex 時,進行資料檢查
                    if (ExcelStringUtil.NotEmpty(columnInfo.Regex))
                    {
                        errorMessage += this.validateData(value, columnInfo.Regex, key, desc, rowNum, columnInfo.RegexErrorMsg);
                        continue;
                    }
                }
                rowNum++;
            }

            // 有檢核到錯誤時，拋出
            if (ExcelStringUtil.NotEmpty(errorMessage))
            {
                throw new ExcelOperateException(errorMessage);
            }

            // ==============================================
            // 欄位值額外處理
            // ==============================================
            foreach (Dictionary<string, object> rowDataMap in dataList)
            {
                foreach (KeyValuePair<string, AbstractImportCommonAttrInfo> infoEntry in paramInfoMap)
                {
                    // key
                    string key = infoEntry.Key;
                    // 資料值
                    string value = ExcelStringUtil.SafeTrim(rowDataMap[key]);
                    // 設定檔
                    AbstractImportCommonAttrInfo info = infoEntry.Value;
                    // 進行額外處理
                    value = this.functionProcess(key, info.FuncId, info.FuncParam, value, rowDataMap, this.Conn);
                    // 放回資料
                    dicPut(rowDataMap, key, value);
                }
            }

            // ==============================================
            // 檢核資料重覆
            // ==============================================
            // 未設定時跳出
            if (ExcelStringUtil.NotEmpty(importConfigInfo.CheckDuplicate))
            {
                // 檢核欄位陣列
                string[] duplicateColumns = importConfigInfo.CheckDuplicate.Split(',');
                // 檢核欄位值
                Dictionary<string, int> duplicateValueStoreMap = new Dictionary<string, int>();

                // 重置 rowNum
                rowNum = 1;
                foreach (Dictionary<string, object> cellMap in dataList)
                {
                    string keyContent = "";

                    // 組成 key
                    foreach (string checkColumnKey in duplicateColumns)
                    {
                        keyContent += ExcelStringUtil.SafeTrim(cellMap[checkColumnKey], "NULL") + "_";
                    }
                    // 檢核同樣的內容是否已存在
                    if (duplicateValueStoreMap.ContainsKey(keyContent))
                    {
                        int num = duplicateValueStoreMap[keyContent];
                        string errormessage = "";
                        foreach (string checkColumnKey in duplicateColumns)
                        {
                            errormessage += "【" + paramInfoMap[checkColumnKey].Desc + "." +
                                            paramInfoMap[checkColumnKey].Key + "】:[" + cellMap[checkColumnKey] +
                                            "]<br/>";
                        }
                        throw new ExcelOperateException(
                            "<br/>第[" + num + "]行資料與第[" + (rowNum + importConfigInfo.StartRow) + "]行重複!(鍵值)<br/>" + errormessage);
                    }
                    duplicateValueStoreMap[keyContent] = rowNum + importConfigInfo.StartRow;
                    rowNum++;
                }
            }

            // ==============================================
            // 欄位List 排序
            // ==============================================
            paramInfoList.Sort(new ComparatorAnonymousInnerClassHelper(this));

            // ==============================================
            // 資料欄位重新排序
            // ==============================================
            IList<Dictionary<string, object>> sortDataList = new List<Dictionary<string, object>>();

            foreach (Dictionary<string, object> rowDataMap in dataList)
            {
                Dictionary<string, object> sortRowDataMap = new Dictionary<string, object>();
                foreach (AbstractImportCommonAttrInfo commonAttrInfo in paramInfoList)
                {
                    string key = commonAttrInfo.Key;
                    dicPut(sortRowDataMap, key, rowDataMap[key]);
                }
                sortDataList.Add(sortRowDataMap);
            }

            dataList = sortDataList;

            // ==============================================
            // 移除不需要的欄位
            // ==============================================
            foreach (Dictionary<string, object> rowDataMap in dataList)
            {
                foreach (ColumnInfo columnInfo in importConfigInfo.ColumnInfoList)
                {
                    // 欄位設定為 pass 時，移除該欄位
                    if (columnInfo.Pass)
                    {
                        rowDataMap.Remove(columnInfo.Key);
                    }
                }
            }

            return dataList;
        }

        private sealed class ComparatorAnonymousInnerClassHelper : IComparer<AbstractImportCommonAttrInfo>
        {
            private readonly ExcelImporter outerInstance;

            public ComparatorAnonymousInnerClassHelper(ExcelImporter outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public int Compare(AbstractImportCommonAttrInfo info1, AbstractImportCommonAttrInfo info2)
            {
                string index1 = ExcelStringUtil.SafeTrim(info1.Index);
                string index2 = ExcelStringUtil.SafeTrim(info2.Index);
                if (ExcelStringUtil.IsEmpty(index1) || !ExcelStringUtil.isNumber(index1))
                {
                    return 1;
                }
                if (ExcelStringUtil.IsEmpty(index2) || !ExcelStringUtil.isNumber(index2))
                {
                    return -1;
                }
                if (ExcelStringUtil.isNumber(index1) && ExcelStringUtil.isNumber(index2))
                {
                    return Convert.ToInt32(index1) - Convert.ToInt32(index2);
                }
                return 0;
            }
        }

        /// <param name="formatId"> </param>
        /// <param name="value"> </param>
        /// <param name="key"> </param>
        /// <param name="desc"> </param>
        /// <param name="rowNum">
        /// @return </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private String validateDataByFormatId(String formatId, String value, String key, String desc, int rowNum) throws Exception
        private string validateDataByFormatId(string formatId, string value, string key, string desc, int rowNum)
        {
            Dictionary<string, FormatInfo> formatInfoMap = ((ImportConfigInfo)this.configInfo).FormatInfoMap;

            if (formatInfoMap == null || !formatInfoMap.ContainsKey(formatId))
            {
                throw new ExcelOperateException("formatId 設定不存在 : formatId = [" + formatId + "]");
            }

            // 讀取設定
            FormatInfo formatInfo = formatInfoMap[formatId];

            // 檢核設定不存在
            if (formatInfo == null)
            {
                throw new ExcelOperateException("Excel 處理錯誤,format 設定不存在! formatId:[" + formatId + "]");
            }

            // 處理檢核
            return this.validateData(value, formatInfo.Regex, key, desc, rowNum, formatInfo.RegexErrorMsg);
        }

        /// <summary>
        /// 驗證資料
        /// </summary>
        /// <param name="value"> 欄位值 </param>
        /// <param name="regExp"> regExp </param>
        /// <param name="key"> 設定值 key </param>
        /// <param name="desc"> 設定值 key </param>
        /// <param name="rowNum"> 行數</param>
        private string validateData(string value, string regExp, string key,
            string desc, int rowNum, string errormsg)
        {
            // ===========================================
            // 格式驗證
            // ===========================================
            Match matcher = new Regex(regExp).Match(value);
            if (!matcher.Success)
            {
                //return "<br/>第" + rowNum + "行, 資料內容格式錯誤！欄位【" + desc + "-" + key + "】:【" + value + "】";
                //return "<br/>第" + rowNum + "筆, " + errormsg + "！欄位【" + desc + "】:【" + value + "】";
                return "<br/>第" + rowNum + "筆【" + desc + "】欄位資料格式錯誤，" + errormsg + "，傳入資料內容：【" + value + "】";
            }
            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="importConfigInfo"></param>
        /// <param name="isXlsx"></param>
        /// <param name="isCloseStream"></param>
        /// <returns></returns>
        private IList<Dictionary<string, object>> parseXsl(
            Stream inStream, ImportConfigInfo importConfigInfo,
             bool isXlsx, bool isCloseStream = true)
        {
            IWorkbook wb = null;

            //以複製 Stream 方式使用, 避免 inStream 需要被重用
            using (MemoryStream memstream = new MemoryStream())
            {
                inStream.CopyTo(memstream);
                memstream.Position = 0; // <-- Add this, to make it work

                if (isXlsx)
                {
                    //2010以上格式
                    wb = new XSSFWorkbook(memstream);
                }
                else
                {
                    wb = new HSSFWorkbook(memstream);
                }
            }
            inStream.Position = 0;

            // 錯誤訊息
            string blankLineMessage = "";
            // 欄位資料List
            IList<Dictionary<string, object>> dataRowList = new List<Dictionary<string, object>>();

            // ==============================================
            // 設定檔資料
            // ==============================================
            // 起始行數
            int startRow = importConfigInfo.StartRow;
            // Sheet 名稱
            int sheetNum = importConfigInfo.SheetNum;
            if (sheetNum < 1)
            {
                throw new ExcelOperateException(" sheetNum 屬性設定錯誤, 不可小於 1");
            }
            // 取得欄位讀取設定
            IList<ColumnInfo> columnInfoList = importConfigInfo.ColumnInfoList;

            try
            {
                // ==============================================
                // 讀取檔案資料
                // ==============================================
                ISheet sheet = null;
                try
                {
                    // 讀取 sheet
                    sheet = wb.GetSheetAt(sheetNum - 1);
                }
                catch (Exception e)
                {
                    throw new ExcelOperateException("檔案解析失敗", e);
                }

                if (sheet == null)
                {
                    throw new ExcelOperateException(" 檔案解析錯誤，第[" + sheetNum + "]個 sheet 不存在");
                }

                // ==============================================
                // 讀取行
                // ==============================================
                // 取得行數
                int lastRowNum = sheet.LastRowNum;

                // 檢核無內容
                if (lastRowNum - startRow < 0)
                {
                    throw new ExcelOperateException("上傳檔案無資料內容! rows[" + lastRowNum + "], startRow:[" + startRow + "]");
                }

                // 檢核是否以下都是空行
                bool tailBlankLine = false;

                for (int rowNum = (startRow - 1); rowNum <= lastRowNum; rowNum++)
                {
                    // 取得 row (列)
                    var row = sheet.GetRow(rowNum);
                    if (row == null) continue;

                    // 資料MAP
                    Dictionary<string, object> dataRowMap = new Dictionary<string, object>();
                    //
                    bool isBlankline = true;
                    for (int colNum = 0; colNum < columnInfoList.Count; colNum++)
                    {
                        // 取得欄位參數設定
                        ColumnInfo columnInfo = columnInfoList[colNum];

                        string cellContent = "";

                        //長度足夠時才讀取 cell
                        if ((colNum + 1) <= row.Cells.Count)
                        {
                            var cell = row.Cells[colNum];

                            //// 取得 cell 內容 (並去空白)
                            ////如果是數字型 就要取 NumericCellValue  這屬性的值
                            //if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                            //{
                            //    cellContent = StringUtil.SafeTrim(cell.NumericCellValue);
                            //}
                            ////如果是字串型 就要取 StringCellValue  這屬性的值
                            //else if (cell.CellType == NPOI.SS.UserModel.CellType.String)
                            //{
                            //    cellContent = StringUtil.SafeTrim(cell.StringCellValue);
                            //}

                            cellContent = ExcelStringUtil.SafeTrim(GetFormattedCellValue(cell));
                        }

                        // 不為空時異動 flag
                        if (ExcelStringUtil.NotEmpty(cellContent))
                        {
                            isBlankline = false;
                        }

                        // 為空時，放入預設值
                        if (ExcelStringUtil.IsEmpty(cellContent))
                        {
                            cellContent = columnInfo.DefaultValue;
                        }
                        // 依據Key放入放入 map
                        dicPut(dataRowMap, columnInfo.Key, cellContent);
                    }

                    // 本行無資料時
                    if (isBlankline)
                    {
                        // 尾部空行標記
                        tailBlankLine = true;
                        // 空行訊息
                        blankLineMessage += "第" + (rowNum + 1) + "行為空行，請重新檢查資料";
                        // 空行略過 加入List
                        continue;
                    }

                    tailBlankLine = false;
                    // 加入List
                    dataRowList.Add(dataRowMap);
                }

                // ==============================================
                // 檢核
                // ==============================================
                // 1.排除以下都是空行
                // 2.有設定需檢核空行
                // 3.錯誤訊息不為空
                if (!tailBlankLine &&
                    "true".Equals(importConfigInfo.CheckEmptyRow, StringComparison.CurrentCultureIgnoreCase) &&
                    ExcelStringUtil.NotEmpty(blankLineMessage))
                {
                    throw new ExcelOperateException("資料內容有誤!\n" + blankLineMessage);
                }

                return dataRowList;
            }
            finally
            {
                if (inStream != null && isCloseStream)
                {
                    inStream.Dispose();
                }
            }
        }

        private string GetFormattedCellValue(ICell cell, IFormulaEvaluator eval = null)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue;

                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            DateTime date = cell.DateCellValue;
                            //ICellStyle style = cell.CellStyle;
                            // Excel uses lowercase m for month whereas .Net uses uppercase
                            //string format = style.GetDataFormatString().Replace('m', 'M');
                            return date.ToString("yyyyMMdd");
                        }
                        else
                        {
                            return cell.NumericCellValue.ToString();
                        }

                    case CellType.Boolean:
                        return cell.BooleanCellValue ? "TRUE" : "FALSE";

                    case CellType.Formula:
                        if (eval != null)
                            return GetFormattedCellValue(eval.EvaluateInCell(cell));
                        else
                            return cell.CellFormula;

                    case CellType.Error:
                        return FormulaError.ForInt(cell.ErrorCellValue).String;
                }
            }
            // null or blank cell, or unknown cell type
            return string.Empty;
        }

        ///// <summary>
        ///// 讀取 xsl 檔案內容
        ///// </summary>
        ///// <param name="inStream"> </param>
        ///// <param name="importConfigInfo"></param>
        ///// <param name="isDontShowReadFileSuppressWarnings"></param>
        //private IList<Dictionary<string, object>> parseXslOld(Stream inStream, ImportConfigInfo importConfigInfo,
        //    bool isDontShowReadFileSuppressWarnings)
        //{
        //    // ==============================================
        //    // 初始化參數
        //    // ==============================================
        //    // Workbook
        //    Workbook wb = null;
        //    // 錯誤訊息
        //    string blankLineMessage = "";
        //    // 欄位資料List
        //    IList<Dictionary<string, object>> dataRowList = new List<Dictionary<string, object>>();

        //    // ==============================================
        //    // 設定檔資料
        //    // ==============================================
        //    // 起始行數
        //    int startRow = importConfigInfo.StartRow;
        //    // Sheet 名稱
        //    int sheetNum = importConfigInfo.SheetNum;
        //    if (sheetNum < 1)
        //    {
        //        throw new ExcelOperateException(" sheetNum 屬性設定錯誤, 不可小於 1");
        //    }
        //    // 取得欄位讀取設定
        //    IList<ColumnInfo> columnInfoList = importConfigInfo.ColumnInfoList;

        //    try
        //    {
        //        // ==============================================
        //        // 讀取檔案資料
        //        // ==============================================
        //        // Workbook
        //        WorkbookSettings wbSetting = new WorkbookSettings();
        //        // 不顯示讀取時格式錯誤
        //        wbSetting.setSuppressWarnings(isDontShowReadFileSuppressWarnings);
        //        // Workbook
        //        try
        //        {
        //            wb = Workbook.getWorkbook(inStream, wbSetting);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new ExcelOperateException("檔案解析失敗", e);
        //        }

        //        // 讀取 sheet
        //        Sheet sheet = wb.getSheet(sheetNum - 1);

        //        if (sheet == null)
        //        {
        //            throw new ExcelOperateException(" 檔案解析錯誤，第[" + sheetNum + "]個 sheet 不存在");
        //        }

        //        // ==============================================
        //        // 讀取行
        //        // ==============================================
        //        // 取得行數
        //        int rows = sheet.getRows();
        //        // 檢核無內容
        //        if (rows - startRow < 0)
        //        {
        //            throw new ExcelOperateException("上傳檔案無資料內容! rows[" + rows + "], startRow:[" + startRow + "]");
        //        }

        //        // 檢核是否以下都是空行
        //        bool tailBlankLine = false;

        //        for (int rowNum = (startRow - 1); rowNum < rows; rowNum++)
        //        {
        //            // 取得 Cell 陣列
        //            Cell[] cells = sheet.getRow(rowNum);
        //            // 資料MAP
        //            Dictionary<string, object> dataRowMap = new Dictionary<string, object>();
        //            //
        //            bool isBlankline = true;
        //            for (int colNum = 0; colNum < columnInfoList.Count; colNum++)
        //            {
        //                // 取得欄位參數設定
        //                ColumnInfo columnInfo = columnInfoList[colNum];

        //                string cellContent = "";

        //                //長度足夠時才讀取 cell
        //                if ((colNum + 1) <= cells.Length)
        //                {
        //                    // 取得 cell 內容 (並去空白)
        //                    cellContent = StringUtil.SafeTrim(cells[colNum].getContents());
        //                }

        //                // 不為空時異動 flag
        //                if (StringUtil.NotEmpty(cellContent))
        //                {
        //                    isBlankline = false;
        //                }

        //                // 為空時，放入預設值
        //                if (StringUtil.IsEmpty(cellContent))
        //                {
        //                    cellContent = columnInfo.DefaultValue;
        //                }
        //                // 依據Key放入放入 map
        //                dicPut(dataRowMap, columnInfo.Key, cellContent);
        //            }

        //            // 本行無資料時
        //            if (isBlankline)
        //            {
        //                // 尾部空行標記
        //                tailBlankLine = true;
        //                // 空行訊息
        //                blankLineMessage += "第" + (rowNum + 1) + "行為空行，請重新檢查資料";
        //                // 空行略過 加入List
        //                continue;
        //            }

        //            tailBlankLine = false;
        //            // 加入List
        //            dataRowList.Add(dataRowMap);
        //        }

        //        // ==============================================
        //        // 檢核
        //        // ==============================================
        //        // 1.排除以下都是空行
        //        // 2.有設定需檢核空行
        //        // 3.錯誤訊息不為空
        //        if (!tailBlankLine &&
        //            "true".Equals(importConfigInfo.CheckEmptyRow, StringComparison.CurrentCultureIgnoreCase) &&
        //            StringUtil.NotEmpty(blankLineMessage))
        //        {
        //            throw new ExcelOperateException("資料內容有誤!\n" + blankLineMessage);
        //        }

        //        return dataRowList;
        //    }
        //    finally
        //    {
        //        if (inStream != null)
        //        {
        //            inStream.Dispose();
        //        }
        //        if (wb != null)
        //        {
        //            wb.close();
        //        }
        //    }
        //}

        private void dicPut(IDictionary dic, string key, object obj)
        {
            if (dic.Contains(key))
            {
                dic[key] = obj;
            }
            else
            {
                dic.Add(key, obj);
            }
        }

        /// <summary>
        /// 測試
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //const string configFile = "C:/workspace/RbtProject/srcExcelOpt/import_sample.xml";
            const string configFile = "e:/TESN/ExcelConfig/Import/Import_UnitRawData.xml";

            const string configID = "UnitRawData";
            const string importFile = "q:/統一編號/utn3386k_xlsx/utn3386K_00.xlsx";

            ImportConfigInfo importConfigInfo = (new ImportConfigReader()).read(configFile, configID);

            IList<Dictionary<string, object>> result;

            using (StreamReader sr = new StreamReader(importFile))
            {
                result = new ExcelImporter().readXlsx(sr.BaseStream, importConfigInfo);
            }

            //Console.Write(result);
        }
    }
}