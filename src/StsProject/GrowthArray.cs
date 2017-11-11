using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial struct GrowthArray<T> : IEnumerable<T>
    {
        // The growth factor, g, is 2. However, it is not declared as a constant here because the indexing
        // algorithm cannot be adapted to any other value of g.
        private const int InitialCapacity = 8;
        private const int Log2InitialCapacity = 3;

        // Section 3: Fields for growth arrays
        // DEVIATION FROM PAPER: In the paper, Cap is a field and Hsize is a property defined in terms of Cap.
        // In this code, Hsize is a field and Capacity is a property defined in terms of Hsize.
        // This is because Hsize is used in the appending algorithm, which is performance-sensitive. It is not
        // ideal for it to make computations to calculate what the head size should be.

        private T[] _head;
        private SmallDynamicArray<T[]> _tail; // This is a mutable struct field; do not make it readonly.
        private int _size;
        private int _hsize;

        // Section 3: 'procedure Constructor(L)' for growth arrays

        public static GrowthArray<T> Create()
        {
            var growthArray = default(GrowthArray<T>);

            growthArray._head = new T[InitialCapacity];
            // DEVIATION FROM PAPER: Instead of using the type DynamicArray, which must have the same configuration
            // (g and c_0 values) as GrowthArray for comparison purposes, I use a modified dynamic array implementation
            // optimized for a small number of items. I do this because the size of the tail is O(log n).
            growthArray._tail = SmallDynamicArray<T[]>.Create();

            // DEVIATION FROM PAPER: The following lines are not necessary because 'growthArray' was default-initialized.
            // This means that all numerical fields are already set to 0.

            // growthArray._size = 0;
            // growthArray._hsize = 0;

            return growthArray;
        }

        public int Capacity => (_size - _hsize) + HeadCapacity;

        public int HeadCapacity => _head.Length;

        public bool IsFull => _size == Capacity;

        // Section 5.1: 'procedure Append(L, item)' for growth arrays

        public void Add(T item)
        {
            if (IsFull)
            {
                Grow();
            }

            _head[_hsize++] = item;
            _size++;
        }

        private void Grow()
        {
            Debug.Assert(IsFull);

            _tail.Append(_head);
            var newHcap = HeadCapacity == InitialCapacity ?
                (2 - 1) * InitialCapacity :
                2 * HeadCapacity;
            _head = new T[newHcap];
            _size = Capacity + newHcap;

            // DEVIATION FROM PAPER: I introduced an _hsize field which I must set to 0,
            // since no items have been appended to the new head.
            _hsize = 0;
        }

        // Section 5.2: 'function Get_item(L, index)' for growth arrays, O(1) implementation

        // DEVIATION FROM PAPER: The functions return a 'ref T' instead of a 'T'.
        // Instead of returning a value, they return the address of a value.
        // This is useful because a value may be updated based on its current value, for example
        // 'ref int value = L[index]; value = value + 10;', without running the calculations
        // in the indexer twice.

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);

                // DEVIATION FROM PAPER: For better performance, the 'Decompose' algorithm
                // is written inline instead of in a separate function.
                var bufferIndex = Math.Max(MathHelpers.CeilLog2(index + 1) - Log2InitialCapacity, 0);
                var elementIndex = index - (1 << (bufferIndex + Log2InitialCapacity - 1));
                return ref GetBuffer(bufferIndex)[elementIndex];
            }
        }

        public T[] GetBuffer(int bufferIndex)
        {
            Debug.Assert(bufferIndex >= 0 && bufferIndex <= _tail.Size);

            return bufferIndex < _tail.Size ? _tail[bufferIndex] : _head;
        }

        // Section 5.2: 'function Get_item(L, index)' for growth arrays, O(log n) implementation

        public ref T GetItemLogarithmic(int index)
        {
            Debug.Assert(index >= 0 && index < _size);

            int i = index;
            foreach (T[] buf in _tail)
            {
                if (i < buf.Length)
                {
                    return ref buf[i];
                }
                i -= buf.Length;
            }

            Debug.Assert(i < _head.Length);
            return ref _head[i];
        }

        // Section 5.3: Iterating for growth arrays

        // In C#, iteration is typically done with an 'enumerator' object that describes how to iterate
        // over a collection. This allows one to write 'foreach (var item in collection) { ..code.. }'
        // The implementation of the algorithm lives in the GrowthArray.Enumerator.cs file.

        public Enumerator GetEnumerator() => new Enumerator(this);

        public BufferSpan<T> GetBufferSpan(int bufferIndex)
        {
            Debug.Assert(bufferIndex >= 0 && bufferIndex <= _tail.Size);

            return bufferIndex < _tail.Size ?
                new BufferSpan<T>(_tail[bufferIndex]) :
                new BufferSpan<T>(_head, _hsize);
        }

        // Section 5.4: 'function To_raw_array(L)' for growth arrays

        public T[] ToRawArray()
        {
            var array = new T[_size];
            // DEVIATION FROM PAPER: Copying to the raw array is done in the helper function CopyTo.
            // CopyTo is more flexible than ToRawArray, since it can copy to an existing array in order
            // to avoid allocating memory.
            CopyTo(array, 0);
            return array;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(arrayIndex >= 0 && array.Length - arrayIndex >= _size);

            foreach (T[] buf in _tail)
            {
                Array.Copy(buf, 0, array, arrayIndex, buf.Length);
                arrayIndex += buf.Length;
            }

            Array.Copy(_head, 0, array, arrayIndex, _hsize);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {_size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
