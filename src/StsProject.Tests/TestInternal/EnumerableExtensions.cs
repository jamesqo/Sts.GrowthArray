using System.Collections.Generic;
using System.Linq;

namespace StsProject.Tests.TestInternal
{
    internal static class EnumerableExtensions
    {
        public static bool ContainsDuplicates<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            var set = new HashSet<T>(comparer);
            return source.Any(item => !set.Add(item));
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T defaultValue = default(T))
        {
            return source.Any() ? source.Max() : defaultValue;
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source, T defaultValue = default(T))
        {
            return source.Any() ? source.Min() : defaultValue;
        }
    }
}