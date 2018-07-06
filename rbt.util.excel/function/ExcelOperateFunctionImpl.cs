using rbt.util.excel.exception;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace rbt.util.excel.function
{
    /// <summary>
    /// @author Allen
    /// </summary>
    public class ExcelOperateFunctionImpl : AbstractExcelOperateFunction
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="keyName"></param>
        /// <param name="funcParam"></param>
        /// <param name="value"></param>
        /// <param name="rowDataMap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        /// <exception cref="ExcelOperateException"></exception>
        public override string Process(
            string method, string keyName, string funcParam, string value,
            Dictionary<string, object> rowDataMap,
            DbConnection conn = null)
        {
            //依據 method 參數, 進行處理
            if ("SEQ".Equals(method, StringComparison.CurrentCultureIgnoreCase))
            {
                //產生序號
                return this.genSeq(funcParam);
            }
            if ("CURR_ROC_DATE".Equals(method, StringComparison.CurrentCultureIgnoreCase))
            {
                //取得目前的民國年日期
                return this.CurrRocDate;
            }
            if ("TO_UPPER".Equals(method, StringComparison.CurrentCultureIgnoreCase))
            {
                //轉大寫
                return (value + "").ToUpper();
            }
            if ("TO_LOWER".Equals(method, StringComparison.CurrentCultureIgnoreCase))
            {
                //轉小寫
                return (value + "").ToLower();
            }

            throw new ExcelOperateException("Excel 處理錯誤, [" + this.GetType().FullName + "] 未設定 method :[" + method + "]");
        }

        /// <summary>
        /// 取得目前的民國年日期 </summary>
        /// <returns> yyy年 MM月 dd日 </returns>
        private string CurrRocDate
        {
            get
            {
                DateTime date = DateTime.Now;
                int year = date.Year;
                string md = date.ToString("年 MM月 dd日");
                return (year - 1911) + md;
            }
        }

        private Dictionary<string, int> seqMap;

        private string genSeq(string sqlSetID)
        {
            //從1開始
            int seq = 1;
            //未傳入 sqlSetID 時，用預設值
            if (StringUtil.IsEmpty(sqlSetID))
            {
                sqlSetID = "#DEFUALT_FUNC_PARAM#";
            }
            if (this.seqMap == null)
            {
                this.seqMap = new Dictionary<string, int>();
            }
            if (this.seqMap.ContainsKey(sqlSetID))
            {
                seq = this.seqMap[sqlSetID] + 1;
            }
            //記錄到存放的 Set
            this.seqMap[sqlSetID] = seq;
            return seq + "";
        }
    }
}