using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial struct BufferSpan<T> : IEnumerable<T>
    {
        internal BufferSpan(T[] array)
        {
            Debug.Assert(array != null);

            Array = array;
            Size = array.Length;
        }

        internal BufferSpan(T[] array, int size)
        {
            Debug.Assert(array != null);
            Debug.Assert(size >= 0 && size <= array.Length);

            Array = array;
            Size = size;
        }

        public T[] Array { get; }

        public int Size { get; }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Size);
                return ref Array[index];
            }
        }

        public void CopyTo(T[] destination, int destinationIndex)
        {
            System.Array.Copy(Array, 0, destination, destinationIndex, Size);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Array.Take(Size).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
