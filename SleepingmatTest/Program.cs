using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sleepingmat;

namespace SleepingmatTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();
            Exception ex = p.Parse(System.IO.File.ReadAllText("test.cfg"));
            if (ex != null)
                throw ex;
            Console.ReadLine();
        }
    }
}
