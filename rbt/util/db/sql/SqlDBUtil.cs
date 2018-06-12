using System.Data.Common;
using System.Data.SqlClient;

namespace rbt.util.db.sql
{
    public abstract class SqlDBUtil : BaseDbUtil
    {
        public abstract string GetConnectionString();

        public override DbConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// Get ConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        public override DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder(GetConnectionString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override BaseSqlUtil NewSqlUtil()
        {
            return new SqlSqlUtil(BaseSqlUtil.DB_TYPE.SQLSERVER);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        protected override DbDataAdapter NewDbDataAdapter(string selectCommandText, DbConnection selectConnection)
        {
            return new SqlDataAdapter(selectCommandText, (SqlConnection)selectConnection);
        }

        protected override DbCommand NewDbCommand(string commandText, DbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection)connection);
        }
    }
}