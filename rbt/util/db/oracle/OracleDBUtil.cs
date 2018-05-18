using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace rbt.util.db.oracle
{
    public abstract class OracleDBUtil : BaseDbUtil
    {
        protected abstract string GetConnectionString();

        public override DbConnection GetConnection()
        {
            return new OracleConnection(GetConnectionString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override BaseSqlUtil NewSqlUtil()
        {
            return new OracleSqlUtil(BaseSqlUtil.DB_TYPE.ORACLE);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new OracleParameter(name, value);
        }

        protected override DbDataAdapter NewDbDataAdapter(string selectCommandText, DbConnection selectConnection)
        {
            return new OracleDataAdapter(selectCommandText, (OracleConnection)selectConnection);
        }

        protected override DbCommand NewDbCommand(string commandText, DbConnection connection)
        {
            return new OracleCommand(commandText, (OracleConnection)connection);
        }
    }
}