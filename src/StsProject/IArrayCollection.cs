using System.Collections.Generic;

namespace StsProject
{
    // Shared interface between DynamicArray/GrowthArray. For testing purposes only.
    public interface IArrayCollection<T> : IEnumerable<T>
    {
        int Capacity { get; }

        bool IsFull { get; }

        IArrayCollectionSettings Settings { get; }

        int Size { get; }

        ref T this[int index] { get; }

        void Append(T item);

        void CopyTo(T[] array, int arrayIndex);

        T[] ToRawArray();
    }

    public static class ArrayCollectionExtensions
    {
        public static void AppendRange<T>(this IArrayCollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                collection.Append(item);
            }
        }
    }
}
