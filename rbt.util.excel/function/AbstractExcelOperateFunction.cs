using System.Collections.Generic;
using System.Data.Common;

namespace rbt.util.excel.function
{
    /// <summary>
    ///
    /// </summary>
    public abstract class AbstractExcelOperateFunction
    {
        /// <summary>
        ///  處理方法
        /// </summary>
        /// <param name="method">設定的方法參數</param>
        /// <param name="keyName"></param>
        /// <param name="funcParam">使用的參數</param>
        /// <param name="value">要處理的值</param>
        /// <param name="rowDataMap"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public abstract string Process(
            string method,
            string keyName,
            string funcParam,
            string value,
            Dictionary<string, object> rowDataMap,
            DbConnection connection = null
            );
    }
}