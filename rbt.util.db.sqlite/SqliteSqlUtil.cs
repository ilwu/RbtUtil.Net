using rbt.Extension;
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
                return "'" + name + "'";
            }

            return name;
        }
    }
}