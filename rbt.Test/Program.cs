using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rbt.util;
using rbt.Extension;

namespace rbt.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = "1234567890X";
            var b = a.SplitWithLength(2);
            foreach (var item in b)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}