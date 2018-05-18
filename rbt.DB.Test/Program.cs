using rbt.DB.Test.Sqlite;

namespace rbt.DB.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var db = new MyDbUtil();

            var dd = db.QueryByTable("SENSOR");
            var a = 1;
        }
    }
}