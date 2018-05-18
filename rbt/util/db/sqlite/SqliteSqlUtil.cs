using System.Data.Common;
using System.Data.SQLite;

namespace rbt.util.db.sqlite
{
    public class SqliteSqlUtil : BaseSqlUtil
    {
        public SqliteSqlUtil(DB_TYPE dbType)
            : base(dbType)
        {
        }

        public override string getPramChar()
        {
            return "@";
        }

        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }
    }
}