using System;
using System.Collections;
using System.Collections.Generic;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    internal partial struct DynamicArray<T>
    {
        // In C#, 'foreach (var item in collection) { ..code.. }' is translated by the compiler to:
        //
        // var e = collection.GetEnumerator();
        // while (e.MoveNext())
        // {
        //     var item = e.Current;
        //     ..code..
        // }
        //
        // Under the above call pattern, the following type defines an algorithm equivalent to the one
        // presented in Section 5.3 for dynamic arrays.

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _buf;
            private readonly int _size;

            private int _index;

            internal Enumerator(T[] buf, int size)
            {
                _buf = buf;
                _size = size;
                _index = -1;
            }

            public T Current => _buf[_index];

            public void Dispose()
            {
            }

            public bool MoveNext() => ++_index < _size;

            [ExcludeFromCodeCoverage]
            object IEnumerator.Current => Current;

            [ExcludeFromCodeCoverage]
            void IEnumerator.Reset() => throw new NotSupportedException();
        }
    }
}
