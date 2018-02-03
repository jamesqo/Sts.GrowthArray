using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace StsProject.Benchmarks
{
    [CoreJob]
    //[RPlotExporter]
    [CsvMeasurementsExporter]
    public class ListVsGrowthArray_Iteration
    {
        [Params(
            4,
            8,
            16,
            32,
            64,
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
            foreach (var item in collection)
            {
            }
        }

        [Benchmark]
        public void GrowthArray()
        {
            var collection = _GrowthArray;
            foreach (var item in collection)
            {
            }
        }
    }
}
