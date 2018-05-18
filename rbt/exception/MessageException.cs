using System;
using System.Runtime.Serialization;

namespace rbt.util.exception
{
    /// <summary>
    ///     Excel Operate 套件處理錯誤時拋出
    /// </summary>
    public class MessageException : Exception, ISerializable
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public MessageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MessageException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}