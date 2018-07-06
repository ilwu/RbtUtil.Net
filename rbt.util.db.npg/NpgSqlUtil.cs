using Npgsql;
using rbt.Extension;
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string ConvertEscapeStr(string name)
        {
            name = name.SafeTrim();

            //TODO 未實做
            var EscapeAry = new string[] {
                //"KEY",
                //"VALUE",
            };

            if (name.ToUpper().In(EscapeAry))
            {
                return "[" + name + "]";
            }

            return name;
        }
    }
}