using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    internal partial struct SmallDynamicArray<T> : IEnumerable<T>
    {
        private const int GrowthFactor = 2;
        private const int InitialCapacity = 4;

        private T[] _buf;
        private int _size;

        public static SmallDynamicArray<T> Create()
        {
            var dynamicArray = default(SmallDynamicArray<T>);
            dynamicArray._buf = Array.Empty<T>();
            return dynamicArray;
        }

        public int Capacity => _buf.Length;

        public int Size => _size;

        public bool IsFull => _size == Capacity;

        public void Append(T item)
        {
            if (IsFull)
            {
                Grow();
            }

            _buf[_size++] = item;
        }

        private void Grow()
        {
            Debug.Assert(IsFull);

            T[] newBuf;
            if (_size == 0)
            {
                newBuf = new T[InitialCapacity];
            }
            else
            {
                newBuf = new T[_size * GrowthFactor];
                Array.Copy(_buf, 0, newBuf, 0, _size);
            }
            
            _buf = newBuf;
        }

        public T this[int index] => _buf[index];

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _buf.Take(_size).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
