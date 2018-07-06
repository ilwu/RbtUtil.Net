using System;
using System.Runtime.Serialization;

namespace rbt.util.exception
{
    /// <summary>
    ///     DBEntityOpExtension 套件處理錯誤時拋出
    /// </summary>
    public class DBEntityOpException : Exception, ISerializable
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public DBEntityOpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public DBEntityOpException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DBEntityOpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}