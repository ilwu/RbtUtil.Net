using rbt.util.excel.bean.common;
using rbt.util.excel.exception;
using rbt.util.excel.function;
using rbt.util.excel.util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace rbt.util.excel
{
    /// <summary>
    ///     @author Allen
    /// </summary>
    public abstract class AbstractExcelOperater : InterfaceExcelOperater
    {
        /// <summary>
        ///
        /// </summary>
        public abstract void setConnection(DbConnection connection);

        // ===========================================================================
        // 全域變數
        // ===========================================================================
        /// <summary>
        ///     config
        /// </summary>
        protected internal AbstractConfigInfo configInfo;

        /// <summary>
        ///
        /// </summary>
        protected DbConnection Conn { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnKey"></param>
        /// <param name="funcId"></param>
        /// <param name="funcParam"></param>
        /// <param name="value"></param>
        /// <param name="rowDataMap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        /// <exception cref="ExcelOperateException"></exception>
        protected string functionProcess(
            string columnKey, string funcId, string funcParam,
            string value, Dictionary<string, object> rowDataMap, DbConnection conn = null)
        {
            // 未設定 FuncId 時跳過
            if (ExcelStringUtil.IsEmpty(funcId))
            {
                return value;
            }

            // 讀取設定
            FunctionInfo functionInfo = configInfo.FunctionInfoMap[funcId];

            // 檢核
            if (functionInfo == null)
            {
                throw new ExcelOperateException("Excel 處理錯誤,function 設定不存在! funcId:[" + funcId + "]");
            }

            //取得物件
            AbstractExcelOperateFunction functionObject = functionInfo.FunctionObject;

            try
            {
                // 取得固定方法 『Process』
                MethodInfo functionMethodInfo = functionObject.GetType().GetMethod("Process");
                // 準備傳入參數
                Object[] args = { functionInfo.Method, columnKey, funcParam, value, rowDataMap, conn };
                //執行方法
                Object returnValue = functionMethodInfo.Invoke(functionObject, args);
                //回傳
                return ExcelStringUtil.SafeTrim(returnValue);
            }
            catch (ExcelOperateException)
            {
                //處理過程 catch 過,已轉成ExcelOperateException的, 不再攔截，直接拋出
                throw;
            }
            catch (Exception e)
            {
                throw new ExcelOperateException(
                    "Excel 處理錯誤, " +
                    "\r\ncolumnKey:[" + columnKey + "], " +
                    "\r\nfuncId:[" + funcId + "], " +
                    "\r\nfuncParam[" + funcParam + "]," +
                    "\r\nvalue:[" + value + "]" +
                    "\r\n" + e.StackTrace);
            }
        }
    }
}