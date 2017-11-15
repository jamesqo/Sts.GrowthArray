using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    // A BufferSpan with Size = s represents the first s items of its Buffer.

    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial struct BufferSpan<T> : IEnumerable<T>
    {
        internal BufferSpan(T[] buf)
        {
            Debug.Assert(buf != null);

            Buffer = buf;
            Size = buf.Length;
        }

        internal BufferSpan(T[] buf, int size)
        {
            Debug.Assert(buf != null);
            Debug.Assert(size >= 0 && size <= buf.Length);

            Buffer = buf;
            Size = size;
        }

        public T[] Buffer { get; }

        public int Size { get; }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Size);
                return ref Buffer[index];
            }
        }

        public void CopyTo(T[] destination, int destinationIndex)
        {
            Array.Copy(Buffer, 0, destination, destinationIndex, Size);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Buffer.Take(Size).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
