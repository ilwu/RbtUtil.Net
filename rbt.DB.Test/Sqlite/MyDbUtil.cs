using rbt.util.db.sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbt.DB.Test.Sqlite
{
    class MyDbUtil : SqlLiteDBUtil
    {
        protected override string GetConnectionString()
        {
            return @"Data Source=d:\CloudStation\Drive\_【GSEO】\workspace\rbt\rbt.DB.Test\db\data.db;";
        }
    }
}
