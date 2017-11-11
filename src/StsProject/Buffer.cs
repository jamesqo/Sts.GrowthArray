using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StsProject.Internal;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial struct Buffer<T> : IEnumerable<T>
    {
        internal static readonly Buffer<T> Empty = new Buffer<T>(System.Array.Empty<T>());

        internal Buffer(T[] array)
        {
            Verify.NotNull(array, nameof(array));

            Array = array;
            Count = array.Length;
        }

        internal Buffer(T[] array, int count)
        {
            Verify.NotNull(array, nameof(array));
            Verify.InRange(count >= 0 && count <= array.Length, nameof(count));

            Array = array;
            Count = count;
        }

        public T[] Array { get; }

        public int Count { get; }

        public ref T this[int index] => ref Array[index];

        public void CopyTo(T[] destination, int destinationIndex)
        {
            System.Array.Copy(Array, 0, destination, destinationIndex, Count);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"{nameof(Count)} = {Count}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Array.Take(Count).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
