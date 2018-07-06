using System.Data.Common;
using System.Data.SqlClient;
using rbt.Extension;

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