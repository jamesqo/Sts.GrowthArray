using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace StsProject.Benchmarks
{
    class Program
    {
        static int N = 50000;

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<ListVsGrowthArray>();
            Debugger.Break();
            GrowthArray(out var collection);
            Debugger.Break();
            GC.KeepAlive(collection);
        }

        static void List(out List<object> collection)
        {
            collection = new List<object>();
            for (int i = 0; i < N; i++)
            {
                collection.Add(null);
            }
        }

        static void GrowthArray(out GrowthArray<object> collection)
        {
            collection = new GrowthArray<object>();
            for (int i = 0; i < N; i++)
            {
                collection.Append(null);
            }
        }
    }
}
