using System.Collections.Generic;
using System.Linq;

namespace StsProject.Tests.TestInternal
{
    internal static class CollectionExtensions
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T item)
            where T : class
        {
            if (item != null)
            {
                collection.Add(item);
            }
        }

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