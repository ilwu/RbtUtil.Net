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
        /// Get ConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        public override DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new NpgsqlConnectionStringBuilder(GetConnectionString());
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

        /// <summary>
        /// 網路連線測試, 會測試連線 DB線路是否正常
        /// </summary>
        /// <returns></returns>
        public bool NetworkTest(int timeout = 200)
        {
            try
            {
                //取得 DB 連線字串
                var builder = (NpgsqlConnectionStringBuilder)this.GetConnectionStringBuilder();
                //測試連線
                return new NetworkUtil().IsActivityByPing(builder.Host, timeout);
            }
            catch (System.Exception)
            {
            }

            return false;
        }
    }
}