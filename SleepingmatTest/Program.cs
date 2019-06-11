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
            Sleepingmat.Mat p = new Sleepingmat.Mat();
            p.Parse(System.IO.File.ReadAllText("test.mat"));
            for(int i = 0; i < 100; i++)
            {
                p.SetValue("cock" + i, i);
                Block b = new Block();
                b.name = "fuck";
                b.SetValue("cum" + i, i);
                p.blocks.Add(b);
            }

            System.IO.File.WriteAllText("test.mat", p.Capture());
        }
    }
}
