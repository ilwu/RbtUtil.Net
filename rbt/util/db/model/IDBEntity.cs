using System.Collections.Generic;

namespace rbt.util.db.model
{
    /// <summary>
    ///
    /// </summary>
    public interface IDBEntity
    {
        /// <summary>
        /// 記錄異動過的欄位
        /// </summary>
        HashSet<string> GetModeifyField();

        /// <summary>
        /// DBEntityOpExtension 的操作儲存區
        /// </summary>
        EntityOperatingModel GetOperating();

        void SetDAO(IDBUtil dao);

        IDBUtil GetDAO();

        /// <summary>
        /// 取得 Table 名稱 (回傳 Entity Model 對應的 TableName)
        /// </summary>
        /// <returns></returns>
        string GetTableName();
    }
}