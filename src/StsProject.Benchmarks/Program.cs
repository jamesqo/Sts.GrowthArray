using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace StsProject.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ListVsGrowthArray>();
        }
    }
}
