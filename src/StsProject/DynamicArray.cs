using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    internal partial struct DynamicArray<T> : IEnumerable<T>
    {
        private const int GrowthFactor = 2;
        private const int InitialCapacity = 8;

        private T[] _buf;
        private int _size;

        // Section 3: 'procedure Constructor(L)' for dynamic arrays

        public static DynamicArray<T> Create()
        {
            var dynamicArray = default(DynamicArray<T>);
            dynamicArray._buf = new T[InitialCapacity];
            return dynamicArray;
        }

        public int Capacity => _buf.Length;

        public int Size => _size;

        public bool IsFull => _size == Capacity;

        // Section 5.1: 'procedure Append(L, item)' for dynamic arrays

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

            T[] newBuf = new T[_size * GrowthFactor];
            Array.Copy(_buf, 0, newBuf, 0, _size);

            _buf = newBuf;
        }

        // Section 5.2: 'function Get_item(L, index)' for dynamic arrays

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);
                return ref _buf[index];
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(_buf, _size);

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"{nameof(Size)} = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
