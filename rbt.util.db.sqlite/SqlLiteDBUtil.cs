using System.Data.Common;
using System.Data.SQLite;

namespace rbt.util.db.sqlite
{
    public abstract class SqlLiteDBUtil : BaseDbUtil
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override BaseSqlUtil NewSqlUtil()
        {
            return new SqliteSqlUtil(BaseSqlUtil.DB_TYPE.SQLITE);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected abstract string GetConnectionString();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override DbConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString());
        }

        /// <summary>
        /// Get ConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        public override DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new SQLiteConnectionStringBuilder(GetConnectionString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }

        protected override DbDataAdapter NewDbDataAdapter(string selectCommandText, DbConnection selectConnection)
        {
            return new SQLiteDataAdapter(selectCommandText, (SQLiteConnection)selectConnection);
        }

        protected override DbCommand NewDbCommand(string commandText, DbConnection connection)
        {
            return new SQLiteCommand(commandText, (SQLiteConnection)connection);
        }
    }
}