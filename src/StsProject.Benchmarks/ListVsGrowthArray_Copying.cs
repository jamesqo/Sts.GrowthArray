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
    public class ListVsGrowthArray_Copying
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

        // long is used instead of object to circumvent type checks by the CLR.
        // Type checks make array assignments slower.
        public List<long> _List;
        public GrowthArray<long> _GrowthArray;
        public long[] _Target;

        [GlobalSetup]
        public void Setup()
        {
            _List = new List<long>();
            for (int i = 0; i < N; i++)
            {
                _List.Add(0);
            }

            _GrowthArray = new GrowthArray<long>();
            for (int i = 0; i < N; i++)
            {
                _GrowthArray.Append(0);
            }
        }

        [IterationSetup]
        public void IterSetup()
        {
            _Target = new long[N];
        }

        [Benchmark]
        public void List()
        {
            _List.CopyTo(_Target);
        }

        [Benchmark]
        public void GrowthArray()
        {
            _GrowthArray.CopyTo(_Target, 0);
        }
    }
}
