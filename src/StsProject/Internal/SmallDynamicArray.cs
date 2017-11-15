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
    // DEVIATION FROM DynamicArray: This type is a struct instead of a class, so GrowthArray allocates
    // less memory.
    internal partial struct SmallDynamicArray<T> : IEnumerable<T>
    {
        private const int GrowthFactor = 2;
        // DEVIATION FROM DynamicArray: The "initial capacity" is 4 instead of 8.
        // NOTE: Because this dynamic array implementation waits as long as possible before allocating,
        // the true initial capacity is 0. (See comments below.)
        private const int InitialCapacity = 4;

        private T[] _buf;
        private int _size;

        // DEVIATION FROM DynamicArray: Parameterless struct constructors are not allowed in C#.
        // Therefore, SmallDynamicArray's "constructor" is a static factory method.

        public static SmallDynamicArray<T> Create()
        {
            var dynamicArray = default(SmallDynamicArray<T>);
            // DEVIATION FROM DynamicArray: The true initial capacity is 0, such that no allocations are
            // made until the first item is appended. This benefits GrowthArray because the tail will not
            // allocate until (c_0 + 1) items are appended, where c_0 is the initial capacity of the growth
            // array.
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
                // DEVIATION FROM DynamicArray: Because this initially called when the capacity is 0,
                // _size == 0 so _size * GrowthFactor == 0. This case needs to be handled specially in
                // order to allocate a bigger buffer.
                newBuf = new T[InitialCapacity];
            }
            else
            {
                newBuf = new T[_size * GrowthFactor];
                Array.Copy(_buf, 0, newBuf, 0, _size);
            }
            
            _buf = newBuf;
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);
                return ref _buf[index];
            }
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {Size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _buf.Take(_size).GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
