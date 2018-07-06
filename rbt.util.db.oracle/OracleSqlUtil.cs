using Oracle.ManagedDataAccess.Client;
using rbt.Extension;
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string ConvertEscapeStr(string name)
        {
            name = name.SafeTrim();
            var EscapeAry = new string[] {
                "KEY",
                "VALUE",
            };

            if (name.ToUpper().In(EscapeAry))
            {
                return "[" + name + "]";
            }

            return name;
        }
    }
}