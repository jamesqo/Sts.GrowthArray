using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StsProject.Profiling
{
    class Program
    {
        static int N;
        static GrowthArray<object> collection;

        static void Main(string[] args)
        {
            Console.WriteLine("Profiling");
            Console.ReadKey();
            N = 50000;
            GrowthArray();
            GC.KeepAlive(collection);
        }

        static void GrowthArray()
        {
            collection = new GrowthArray<object>();
            for (int i = 0; i < N; i++)
            {
                collection.Append(null);
            }
        }
    }
}
