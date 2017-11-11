﻿using System.Collections.Generic;
using System.Linq;

namespace StsProject.Tests.TestInternal
{
    internal static class TheoryDataExtensions
    {
        public static IEnumerable<object[]> ToTheoryData<T>(this IEnumerable<T> source)
        {
            return source.Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> ToTheoryData<T1, T2>(this IEnumerable<(T1, T2)> source)
        {
            return source.Select(x => new object[] { x.Item1, x.Item2 });
        }

        public static IEnumerable<object[]> ToTheoryData<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> source)
        {
            return source.Select(x => new object[] { x.Item1, x.Item2, x.Item3 });
        }

        public static IEnumerable<object[]> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> source)
        {
            return source.Select(x => new object[] { x.Item1, x.Item2, x.Item3, x.Item4 });
        }
    }
}