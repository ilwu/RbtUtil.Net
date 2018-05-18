using Npgsql;
using System.Data.Common;

namespace rbt.util.db.npg
{
    public abstract class NpgDBUtil : BaseDbUtil
    {
        public abstract string GetConnectionString();

        public override DbConnection GetConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override BaseSqlUtil NewSqlUtil()
        {
            return new NpgSqlUtil(BaseSqlUtil.DB_TYPE.POSTGRESQL);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }

        protected override DbDataAdapter NewDbDataAdapter(string selectCommandText, DbConnection selectConnection)
        {
            return new NpgsqlDataAdapter(selectCommandText, (NpgsqlConnection)selectConnection);
        }

        protected override DbCommand NewDbCommand(string commandText, DbConnection connection)
        {
            return new NpgsqlCommand(commandText, (NpgsqlConnection)connection);
        }
    }
}