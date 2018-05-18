using rbt.util.db.sqlite;

namespace rbt.DB.Test.Sqlite
{
    internal class MyDbUtil : SqlLiteDBUtil
    {
        protected override string GetConnectionString()
        {
            return @"Data Source=d:\CloudStation\Drive\_【GSEO】\workspace\rbt\rbt.DB.Test\db\data.db;";
        }
    }
}