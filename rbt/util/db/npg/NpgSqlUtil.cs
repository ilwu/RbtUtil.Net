using Npgsql;
using System.Data.Common;

namespace rbt.util.db.npg
{
    public class NpgSqlUtil : BaseSqlUtil
    {
        public NpgSqlUtil(DB_TYPE dbType)
            : base(dbType)
        {
        }

        public override string getPramChar()
        {
            return "@";
        }

        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }
    }
}