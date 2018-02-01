using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace StsProject.Benchmarks
{
    [CoreJob]
    [RPlotExporter]
    public class ListVsGrowthArray
    {
        [Params(
            // decimal
            10, /*25, 50,
            100,*/ 250, /*500,
            1000, 2500,*/ 5000/*,
            10000, 25000, 50000,
            // binary
            // each row is 2^n, 2^n + 1, and the average of 2^n and 2^(n + 1)
            16, 17, 24,
            32, 33, 48,
            64, 65, 96,
            128, 129, 192,
            256, 257, 384,
            512, 513, 768,
            1024, 1025, 1536,
            2048, 2049, 3072,
            4096, 4097, 6144,
            8192, 8193, 12288,
            16384, 16385, 24576,
            32768, 32769*/, 49152/*
            */)]
        public int N;

        [Benchmark]
        public void List()
        {
            var collection = new List<object>();
            for (int i = 0; i < N; i++)
            {
                collection.Add(null);
            }
        }

        [Benchmark]
        public void GrowthArray()
        {
            var collection = new GrowthArray<object>();
            for (int i = 0; i < N; i++)
            {
                collection.Append(null);
            }
        }
    }
}