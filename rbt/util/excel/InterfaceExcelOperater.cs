using System.Data.Common;

namespace rbt.util.excel
{
    /// <summary>
    /// </summary>
    public interface InterfaceExcelOperater
    {
        /// <summary>
        /// 設定系統所使用的 DbConnection
        /// </summary>
        void setConnection(DbConnection connection);
    }
}