using System.Collections.Generic;
using System.Data.Common;

namespace rbt.util.db
{
    public interface IDBUtil
    {
        // ==============================================================================================
        // get Connection
        // ==============================================================================================
        /// <summary>
        ///     getConnection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        DbConnection GetConnection();

        /// <summary>
        ///  查詢單一 Table 且 order by
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="whereColumnsInfo"></param>
        /// <param name="orderByFileds"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IList<T> QueryByTableAndOrderBy<T>(string tableName, Dictionary<string, object> whereColumnsInfo,
            string[] orderByFileds, DbConnection conn = null, DbTransaction transaction = null);

        /// <summary>
        ///     查詢單一 Table
        /// </summary>
        /// <param name="tableName">查詢單一 Table</param>
        /// <param name="selectColumns"></param>
        /// <param name="whereColumnsInfo">查詢條件欄位</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction"> transaction</param>
        /// <returns></returns>
        /// <remarks></remarks>
        IList<IDictionary<string, object>> QueryByTable(
            string tableName, IList<string> selectColumns,
            IDictionary<string, object> whereColumnsInfo,
            DbConnection conn, DbTransaction transaction);

        /// <summary>
        ///     依據傳入的 sql 查詢筆數是否大於0
        /// </summary>
        /// <param name="querySql"></param>
        /// <param name="params"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool IsExist(string querySql, Dictionary<string, object> @params = null, DbConnection conn = null,
            DbTransaction transaction = null);

        /// <summary>
        ///     依據傳入的SQL查詢結果，回傳查詢筆數
        /// </summary>
        /// <param name="querySql">查詢字串</param>
        /// <param name="params">參數 [參數名稱,參數值]</param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        int QueryCount(
            string querySql,
            Dictionary<string, object> @params = null,
            DbConnection conn = null,
            DbTransaction transaction = null);

        /// <summary>
        ///     公用 DELETE
        /// </summary>
        /// <param name="tableName"> table name</param>
        /// <param name="whereColumnsInfo"> WHERE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        int Delete(string tableName, Dictionary<string, object> whereColumnsInfo, DbConnection conn = null,
            DbTransaction transaction = null);

        /// <summary>
        /// INSERT (綁定 Table Model 的方式)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int Insert<TModel>(TModel model, string tableName, DbConnection conn = null, DbTransaction transaction = null);

        /// <summary>
        ///     UPDATE
        /// </summary>
        /// <param name="tableName"> table name</param>
        /// <param name="setColumnsInfo"> UPDATE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="whereColumnsInfo"> WHERE 欄位資訊 [欄位名稱, 欄位值]</param>
        /// <param name="conn">Connection (傳入 null 時, 會自動 getConnection)</param>
        /// <param name="transaction">transaction , 有傳入才會使用, 可不傳入</param>
        /// <returns></returns>
        /// <remarks></remarks>
        int Update(string tableName, Dictionary<string, object> setColumnsInfo,
            Dictionary<string, object> whereColumnsInfo = null, DbConnection conn = null,
            DbTransaction transaction = null);
    }
}