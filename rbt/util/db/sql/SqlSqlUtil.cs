using System.Data.Common;
using System.Data.SqlClient;

namespace rbt.util.db.sql
{
    public class SqlSqlUtil : BaseSqlUtil
    {
        public SqlSqlUtil(DB_TYPE dbType)
            : base(dbType)
        {
        }

        public override string getPramChar()
        {
            return "@";
        }

        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}