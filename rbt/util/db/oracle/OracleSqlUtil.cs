using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace rbt.util.db.oracle
{
    public class OracleSqlUtil : BaseSqlUtil
    {
        public OracleSqlUtil(DB_TYPE dbType)
            : base(dbType)
        {
        }

        public override string getPramChar()
        {
            return ":";
        }

        protected override DbParameter NewDbParameter(string name, object value)
        {
            return new OracleParameter(name, value);
        }
    }
}