using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace StsProject.Benchmarks
{
    [CoreJob]
    [RPlotExporter]
    public class ListVsGrowthArray_GetItem
    {
        [Params(
            4,       // 1 buf (since c0 = 8)
            8,       // 1 buf (since c0 = 8)
            16,      // 2 bufs
            32,      // 3 bufs
            64,      // ...
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192,
            16384,
            32768,
            65536
        )]
        public int N;

        public List<object> _List;
        public GrowthArray<object> _GrowthArray;

        [GlobalSetup]
        public void Setup()
        {
            _List = new List<object>();
            for (int i = 0; i < N; i++)
            {
                _List.Add(null);
            }

            _GrowthArray = new GrowthArray<object>();
            for (int i = 0; i < N; i++)
            {
                _GrowthArray.Append(null);
            }
        }

        [Benchmark]
        public void List()
        {
            var collection = _List;
            for (int i = 0; i < 100; i++)
            {
                _ = collection[i];
            }
        }

        [Benchmark]
        public void GrowthArray_O1()
        {
            var collection = _GrowthArray;
            for (int i = 0; i < 100; i++)
            {
                _ = collection[i];
            }
        }

        [Benchmark]
        public void GrowthArray_OLogN()
        {
            var collection = _GrowthArray;
            for (int i = 0; i < 100; i++)
            {
                _ = collection.GetItemLogarithmic(i);
            }
        }
    }
}
