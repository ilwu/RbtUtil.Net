using System;
using System.Runtime.Serialization;

namespace rbt.util.excel.exception
{
    /// <summary>
    ///     Excel Operate 套件處理錯誤時拋出
    /// </summary>
    public class ExcelOperateException : Exception, ISerializable
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public ExcelOperateException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ExcelOperateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ExcelOperateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}