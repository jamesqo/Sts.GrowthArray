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
            Size = array.Length;
        }

        internal Buffer(T[] array, int size)
        {
            Verify.NotNull(array, nameof(array));
            Verify.InRange(size >= 0 && size <= array.Length, nameof(size));

            Array = array;
            Size = size;
        }

        public T[] Array { get; }

        public int Size { get; }

        public ref T this[int index] => ref Array[index];

        public void CopyTo(T[] destination, int destinationIndex)
        {
            System.Array.Copy(Array, 0, destination, destinationIndex, Size);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"{nameof(Size)} = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Array.Take(Size).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
