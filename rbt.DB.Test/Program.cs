using rbt.DB.Test.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbt.DB.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MyDbUtil();

            var dd = db.QueryByTable("SENSOR");
            var a = 1;
        }
    }
}
