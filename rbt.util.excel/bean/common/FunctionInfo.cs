using rbt.util.excel.exception;
using rbt.util.excel.function;
using System;

namespace rbt.util.excel.bean.common
{
    /// <summary>
    ///     Function Config Info
    /// </summary>
    public class FunctionInfo
    {
        // =====================================================
        // 元素屬性
        // =====================================================
        /// <summary>
        ///     已經實體化的物件
        /// </summary>
        private AbstractExcelOperateFunction _functionObject;

        /// <summary>
        ///     Function 設定 ID
        /// </summary>
        public string FuncId { get; set; }

        /// <summary>
        ///     要呼叫的 class 名稱
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        ///     要呼叫的 方法 名稱
        /// </summary>
        public string Method { get; set; }

        // =====================================================
        // 工作參數
        // =====================================================

        /// <summary>
        ///     已經實體化的物件
        /// </summary>
        public AbstractExcelOperateFunction FunctionObject
        {
            get { return _functionObject ?? (_functionObject = GetFunctionClassIns()); }
        }

        private AbstractExcelOperateFunction GetFunctionClassIns()
        {
            // 取得 Class
            Type function = null;

            // 取得Class
            try
            {
                function = Type.GetType(ClassName);
            }
            catch (Exception e)
            {
                throw new ExcelOperateException("Excel 處理錯誤, function class 實體化失敗!\r\n" + e.StackTrace);
            }
            //檢核class不存在
            if (function == null)
            {
                throw new ExcelOperateException("Excel 處理錯誤, function class [" + ClassName + "] 不存在!");
            }

            // 檢核父類別
            if (!function.IsSubclassOf(typeof(AbstractExcelOperateFunction)))
            {
                throw new ExcelOperateException(
                    "Excel 處理錯誤, function class 需繼承 AbstractExcelOperateFunction! " +
                    "className:[" + ClassName + "]");
            }

            //實例化
            try
            {
                return (AbstractExcelOperateFunction)Activator.CreateInstance(function);
            }
            catch (Exception e)
            {
                throw new ExcelOperateException("Excel 處理錯誤, function class 實例化失敗!\r\n" + e.StackTrace);
            }
        }
    }
}